﻿using MDR_Harvester.Extensions;
using System.Globalization;
using System.Text.Json;

namespace MDR_Harvester.Yoda; 

public class YodaProcessor : IStudyProcessor
{
    public Study? ProcessData(string jsonString, DateTime? downloadDatetime, ILoggingHelper _logging_helper)
    {
        ///////////////////////////////////////////////////////////////////////////////////////
        // Set up and deserialise string 
        ///////////////////////////////////////////////////////////////////////////////////////

        var json_options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        Yoda_Record? r = JsonSerializer.Deserialize<Yoda_Record?>(jsonString, json_options);

        if(r is null)
        {
            _logging_helper.LogError($"Unable to deserialise json file to Who_Record\n{jsonString[..1000]}... (first 1000 characters)");
            return null;
        }

        Study s = new();

        // get date retrieved in object fetch
        // transfer to study and data object records

        List<StudyIdentifier> identifiers = new();
        List<StudyTitle> titles = new();
        List<StudyReference> references = new();
        List<StudyOrganisation> organisations = new();
        List<StudyTopic> topics = new();
        List<StudyCondition> conditions = new();

        List<DataObject> data_objects = new();
        List<ObjectDataset> object_datasets = new();
        List<ObjectTitle> object_titles = new();
        List<ObjectInstance> object_instances = new();

        
        ///////////////////////////////////////////////////////////////////////////////////////
        // Basics - id, Submission date, titles
        ///////////////////////////////////////////////////////////////////////////////////////
        
        string sid = r.sd_sid!;
        s.sd_sid = sid;
        s.datetime_of_data_fetch = downloadDatetime;

        string? yoda_title = r.yoda_title?.FullClean();
        s.display_title = yoda_title;

        // name_base derived from CTG during download, if possible.
        // In most cases the name_base will be the NCT title.

        string? name_base_title = r.name_base_title?.LineClean();  
        string? name_base = string.IsNullOrEmpty(name_base_title) ? yoda_title : name_base_title;

        var st_titles = r.study_titles;  
        if (st_titles?.Any() is true)
        {
            foreach (var t in st_titles)
            {
                string? title_text = t.title_text.LineClean(); 
                int? title_type_id = t.title_type_id; 
                string? title_type = t.title_type; 
                bool? is_default = t.is_default;
                string? comments = t.comments;
                titles.Add(new StudyTitle(sid, title_text, title_type_id, title_type, is_default, comments));
            }
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////
        // Study basic attributes - type,status, description, enrolment, gender
        ///////////////////////////////////////////////////////////////////////////////////////

        s.brief_description = r.brief_description?.FullClean();
        s.study_status_id = 21;
        s.study_status = "Completed";  // assumption for entry onto web site

        // Study type only really relevant for non registered studies (others will have type identified
        // in registered study entry here, usually previously obtained from the ctg or isrctn entry.
        
        s.study_type_id = r.study_type_id;
        s.study_type = s.study_type_id switch
        {
            11 => "interventional",
            12 => "observational",
            13 => "observational patient registry",
            14 => "expanded access",
            15 => "funded programme",
            16 => "not yet known",
            _ => "not yet known"
        };

        s.study_enrolment = r.enrolment == "" ? null : r.enrolment;  // nulls are empty strings after scraping process

        string? percent_female = r.percent_female;
        double tolerance = .0001;
        if (!string.IsNullOrEmpty(percent_female) && percent_female != "N/A")
        {
            if (percent_female.EndsWith("%"))
            {
                percent_female = percent_female[..^1];
            }

            if (Single.TryParse(percent_female, out float female_percentage))
            {
                if (female_percentage == 0)
                {
                    s.study_gender_elig_id = 910;
                    s.study_gender_elig = "Male";
                }
                else if (Math.Abs(female_percentage - 100) < tolerance)
                {
                    s.study_gender_elig_id = 905;
                    s.study_gender_elig = "Female";
                }
                else
                {
                    s.study_gender_elig_id = 900;
                    s.study_gender_elig = "All";
                }
            }
        }
        else
        {
            s.study_gender_elig_id = 915;
            s.study_gender_elig = "Not provided";
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////
        // Study identifiers
        ///////////////////////////////////////////////////////////////////////////////////////

        // Normally a protocol id will be the only addition (may be a duplicate of one already in the system).

        var study_idents = r.study_identifiers;  
        if (study_idents?.Any() is true)
        {
            foreach (var i in study_idents)
            {
                string? identifier_value = i.identifier_value; 
                int? identifier_type_id = i.identifier_type_id; 
                string? identifier_type = i.identifier_type;
                int? source_id = i.source_id;                  
                string? source = i.source?.TidyOrgName(sid).StandardisePharmaName();

                if (source_id == 0)
                {
                    source_id = null;  // Otherwise 0 is inserted from the JSON file by default
                }
                identifiers.Add(new StudyIdentifier(sid, identifier_value, identifier_type_id, identifier_type,
                                                    source_id, source));
            }
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////
        // Study sponsor
        ///////////////////////////////////////////////////////////////////////////////////////
        
        // only sponsor known, and only relevant for non registered studies (others will  
        // have the sponsor identified in registered study entry).
        // If study registered elsewhere sponsor details wil be ignored during the aggregation.

        int? sponsor_org_id; string? sponsor_org;
        int? sponsor_id = r.sponsor_id; 
        string? sponsor = r.sponsor;
        if (!string.IsNullOrEmpty(sponsor))
        {
            sponsor_org_id = sponsor_id;
            sponsor_org = sponsor.TidyOrgName(sid).StandardisePharmaName();
        }
        else
        {
            sponsor_org_id = null;
            sponsor_org = "No organisation name provided in source data";
        }
        organisations.Add(new StudyOrganisation(sid, 54, "Study Sponsor", sponsor_org_id, sponsor_org));

        
        ///////////////////////////////////////////////////////////////////////////////////////
        // Study topics and conditions
        ///////////////////////////////////////////////////////////////////////////////////////

        string? compound_generic_name = r.compound_generic_name.LineClean();
        string? compound_product_name = r.compound_product_name.LineClean();
        string? conditions_studied = r.conditions_studied.LineClean();
        
        if (!string.IsNullOrEmpty(compound_generic_name))
        {
            topics.Add(new StudyTopic(sid, 12, "chemical / agent", compound_generic_name));
        }

        if (!string.IsNullOrEmpty(compound_product_name))
        {
            if (compound_product_name.IsNotInTopicsAlready(topics))
            {
                compound_product_name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(compound_product_name.ToLower());
                topics.Add(new StudyTopic(sid, 12, "chemical / agent", compound_product_name));
            }
        }

        if (!string.IsNullOrEmpty(conditions_studied))
        {
            conditions.Add(new StudyCondition(sid, conditions_studied, null, null, null));
            conditions = conditions.RemoveNonInformativeConditions();
        }
        
        
        ///////////////////////////////////////////////////////////////////////////////////////
        // Study references
        ///////////////////////////////////////////////////////////////////////////////////////

        // Create study references (pmids).
        
        var refs = r.study_references;
        if (refs?.Any() is true)
        {
            foreach (var sr in refs)
            {
                string? pmid = sr.pmid;
                string? link = sr.link;
                    
                // normally only 1 if there is one there at all 
                references.Add(new StudyReference(sid, pmid, link, null, 202, "Journal article - results", ""));
            }
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////
        // Yoda web page data object
        ///////////////////////////////////////////////////////////////////////////////////////
       
        string object_title = "Yoda web page";
        string object_display_title = name_base + " :: " + "Yoda web page";
        string? remote_url = r.remote_url;
        string sd_oid = sid + " :: 38 :: " + object_title;

        data_objects.Add(new DataObject(sd_oid, sid, object_title, object_display_title, null, 23, "Text", 38, "Study Overview",
                            101901, "Yoda", 12, downloadDatetime));
        object_instances.Add(new ObjectInstance(sd_oid, 101901, "Yoda",
                            remote_url, true, 35, "Web text"));
        
        
        ///////////////////////////////////////////////////////////////////////////////////////
        // Supplementary docs data objects
        ///////////////////////////////////////////////////////////////////////////////////////

        // then for each supp doc...
        var sds = r.supp_docs;
        if (sds?.Any() is true)
        {
            foreach (var sd in sds)
            {
                // Get object parameters
                
                int? object_type_id = sd.doc_type_id;                
                string? object_type = sd.doc_type;
                string? comment = sd.comment;
                string? url = sd.url;
                int object_class_id = object_type_id is 80 or 69 ? 14 : 23;
                string object_class = object_class_id == 14 ? "Datasets" : "Text";
                
                if (object_type_id is not null)
                {
                    object_display_title = name_base + " :: " + object_type;
                    sd_oid = sid + " :: " + object_type_id + " :: " + object_type;

                    if (comment == "Available now")
                    {
                        data_objects.Add(new DataObject(sd_oid, sid, object_type, object_display_title, null, object_class_id, object_class, object_type_id, object_type,
                                        101901, "Yoda", 11, downloadDatetime));

                        // create instance as resource exists
                        // get file type from link if possible
                        int resource_type_id = 0; string resource_type = "Not yet known";
                        if (url is not null)
                        {
                            if (url.ToLower().EndsWith(".pdf"))
                            {
                                resource_type_id = 11;
                                resource_type = "PDF";
                            }
                            else if (url.ToLower().EndsWith(".xls") || url.ToLower().EndsWith(".xlsx"))
                            {
                                resource_type_id = 18;
                                resource_type = "Excel Spreadsheet(s)";
                            }
                        }
                        object_instances.Add(new ObjectInstance(sd_oid, 101901, "Yoda", url, true, resource_type_id, resource_type));
                    }
                    else
                    {
                        string access_details = "Material provided under managed access. Please follow the link to the yoda site for details of the application process.";
                        data_objects.Add(new DataObject(sd_oid, sid, object_type, object_display_title, null, object_class_id, object_class, object_type_id, object_type,
                                        101901, "Yoda", 17, "Case by case download", access_details,
                                        "https://yoda.yale.edu/how-request-data", null, downloadDatetime));
                    }

                    // for datasets also add dataset properties - even if they are largely unknown
                    if (object_type_id == 80)
                    {
                        object_datasets.Add(new ObjectDataset(sd_oid, 0, "Not known", null,
                                                    2, "De-identification applied",
                                                    "Yoda states that “...researchers will be granted access to participant-level study data that are devoid of personally identifiable information; current best guidelines for de-identification of data will be used.”",
                                                    0, "Not known", null));
                    }
                }
            }
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////
        // Construct final study object
        ///////////////////////////////////////////////////////////////////////////////////////
        
        s.identifiers = identifiers;
        s.titles = titles;
        s.references = references;
        s.organisations = organisations;
        s.topics = topics;
        s.conditions = conditions;

        s.data_objects = data_objects;
        s.object_datasets = object_datasets;
        s.object_titles = object_titles;
        s.object_instances = object_instances;

        return s;
    }
}








