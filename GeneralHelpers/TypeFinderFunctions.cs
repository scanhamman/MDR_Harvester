﻿using System.Text.RegularExpressions;

namespace MDR_Harvester.Extensions;

public static class TypeHelpers
{
    public static int? GetStatusId(this string? study_status)
    {
        if (string.IsNullOrEmpty(study_status))
        {
            return null;
        }
        return study_status.ToLower() switch
        {
            "completed" => 21,
            "recruiting" => 14,
            "ongoing" => 25,
            "active, not recruiting" => 15,
            "not yet recruiting" => 16,
            "unknown status" => 0,
            "withdrawn" => 11,
            "available" => 12,
            "withheld" => 13,
            "no longer available" or
                "temporarily not available" => 17,
            "suspended" => 18,
            "terminated" => 22,
            "prematurely ended" => 22,
            "enrolling by invitation" => 19,
            "approved for marketing" => 20,
            "other" => 24,
            _ => null,
        };
    }

    public static int? GetTypeId(this string? study_type)
    {
        if (string.IsNullOrEmpty(study_type))
        {
            return null;
        }
        return study_type.ToLower() switch
        {
            "interventional" => 11,
            "observational" => 12,
            "observational patient registry" => 13,
            "expanded access"=> 14,
            "funded programme" => 15,
            "not yet known" => 0,
            _ => null
        };
    }


    public static int? GetGenderEligId(this string? gender_elig)
    {
        if (string.IsNullOrEmpty(gender_elig))
        {
            return null;
        }
        return gender_elig.ToLower() switch
        {
            "both" => 900,
            "all" => 900,
            "female" => 905,
            "male" => 910,
            "not provided" => 915,
            "not specified" => 915,
            _ => null,
        };
    }


    public static int? GetTimeUnitsId(this string? time_units)
    {
        if (string.IsNullOrEmpty(time_units))
        {
            return null;
        }
        return time_units.ToLower() switch
        {
            "seconds" => 11,
            "minutes" => 12,
            "hours" => 13,
            "days" => 14,
            "weeks" => 15,
            "months" => 16,
            "years" => 17,
            "not provided" => 0,
            _ => null,
        };
    }

    public static int? GetPhaseId(this string? phase)
    {
        if (string.IsNullOrEmpty(phase))
        {
            return null;
        }
        return phase.ToLower() switch
        {
            "n/a" or "na" or "not applicable" => 100,
            "early phase 1" => 105,
            "phase 1" => 110,
            "phase 1/phase 2" => 115,
            "phase 2" => 120,
            "phase 2/phase 3" => 125,
            "phase 3" => 130,
            "phase 4" => 135,
            "not provided" => 140,
            _ => null,
        };
    }
    
    public static int? GetPrimaryPurposeId(this string primary_purpose)
    {
        if (string.IsNullOrEmpty(primary_purpose))
        {
            return null;
        }
        return primary_purpose.ToLower() switch
        {
            "treatment" => 400,
            "prevention" => 405,
            "diagnostic" => 410,
            "supportive care" => 415,
            "screening" => 420,
            "health services research" => 425,
            "basic science" => 430,
            "device feasibility" => 435,
            "other" => 440,
            "not provided" => 445,
            "educational/counseling/training" => 450,
            _ => null,
        };
    }
  
    public static int? GetAllocationTypeId(this string allocation_type)
    {
        if (string.IsNullOrEmpty(allocation_type))
        {
            return null;
        }
        return allocation_type.ToLower() switch
        {
            "n/a" or "na" or "not applicable" => 200,
            "randomized" => 205,
            "non-randomized" => 210,
            "not provided" => 215,
            _ => null,
        };
    }
    
    
    public static int? GetDesignTypeId(this string design_type)
    {
        if (string.IsNullOrEmpty(design_type))
        {
            return null;
        }
        return design_type.ToLower() switch
        {
            "single group assignment" => 300,
            "parallel assignment" => 305,
            "crossover assignment" => 310,
            "factorial assignment" => 315,
            "sequential assignment" => 320,
            "not provided" => 325,
            _ => null,
        };
    }
    
    public static int? GetMaskingTypeId(this string masking_type)
    {
        if (string.IsNullOrEmpty(masking_type))
        {
            return null;
        }
        return masking_type.ToLower() switch
        {
            "none (open label)" or "none" => 500,
            "single" => 505,
            "double" => 510,
            "triple" => 515,
            "quadruple" => 520,
            "not provided" => 525,
            _ => null,
        };
    }


    public static int? GetObsModelTypeId(this string obs_model_type)
    {
        if (string.IsNullOrEmpty(obs_model_type))
        {
            return null;
        }
        return obs_model_type.ToLower() switch
        {
            "cohort" => 600,
            "case control" or "case-control" => 605,
            "case-only" => 610,
            "case-crossover" => 615,
            "ecologic or community" => 620,
            "family-based" or "family based" => 625,
            "other" => 630,
            "not provided" => 635,
            "defined population" => 640,
            "natural history" => 645,
            _ => null,
        };
    }

    public static int? GetTimePerspectiveId(this string time_perspective)
    {
        if (string.IsNullOrEmpty(time_perspective))
        {
            return null;
        }

        return time_perspective.ToLower() switch
        {
            "retrospective" => 700,
            "prospective" => 705,
            "cross-sectional" => 710,
            "other" => 715,
            "not provided" => 720,
            "retrospective/prospective" => 725,
            "longitudinal" => 730,
            _ => null,
        };
    }

    public static int? GetSpecimenRetentionId(this string specimen_retention)
    {
        if (string.IsNullOrEmpty(specimen_retention))
        {
            return null;
        }

        return specimen_retention.ToLower() switch
        {
            "none retained" => 800,
            "samples with dna" => 805,
            "samples without dna" => 810,
            "not provided" => 815,
            _ => null,
        };
    }
    
    
    public static int? GetCTTypeId(this string ct_type)
    {
        if (string.IsNullOrEmpty(ct_type))
        {
            return null;
        }
        return ct_type.ToLower() switch
        {
            "authors" => 11,
            "icd 10" => 12,
        	"icd 11" => 13,
        	"mesh" => 14,
        	"snomed ct" => 15,
        	"meddra" => 16,
        	"nci thesaurus" => 17,
        	"cochrane pico terminology" => 18,
        	"cdisc controlled terminology" => 19,
        	"loinc" => 20,
        	"atc drug classification" => 21,
        	"who Drug" => 22,
        	"iupac chemical names" => 23,
        	"inchi chemical identifier" => 24,
        	"enzyme commission numbers / names" => 25,
            "hgnc human genome codes / names" => 26	,
        	"taxonomic names" => 27,
        	"none" => 28,
        	"cas" => 29,
        	"mesh tree" => 30,
            "not yet known" => 0,
            _ => null,
        };
    }

    public static string? GetTimeUnits(this string? time_units)
    {

        if (string.IsNullOrEmpty(time_units))
        {
            return null;
        }

        // was not classified previously...
        // starts with "Other" and has brackets around the text
        string time_string = time_units.Replace("Other", "").Trim();
        time_string = time_string.TrimStart('(').TrimEnd(')').ToLower();

        if (Regex.Match(time_string, @"\d+y").Success)
        {
            return "Years";
        }
        if (Regex.Match(time_string, @"\d+m").Success)
        {
            return "Months";
        }
        if (Regex.Match(time_string, @"\d+w").Success)
        {
            return "Weeks";
        }
        if (Regex.Match(time_string, @"\d+d").Success)
        {
            return "Days";
        }
        if (Regex.Match(time_string, @"^\d+$").Success)
        {
            return "Years";  // default of years for numbers on their own
        }
        
        return null;  // if all above fail

    }
}
