﻿using Dapper;
using Npgsql;

namespace MDR_Harvester;

public class ADCompObjectTableBuilder
{
    private readonly string _db_conn;

    public ADCompObjectTableBuilder(string db_conn)
    {
        _db_conn = db_conn;
    }

    private void Execute_SQL(string sql_string)
    {
        using var conn = new NpgsqlConnection(_db_conn);
        conn.Execute(sql_string);
    }

    public void create_table_data_objects()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.data_objects;
          CREATE TABLE adcomp.data_objects(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 3001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , sd_sid                 VARCHAR         NULL
          , display_title          VARCHAR         NULL
          , version                VARCHAR         NULL
          , doi                    VARCHAR         NULL 
          , doi_status_id          INT             NULL
          , publication_year       INT             NULL
          , object_class_id        INT             NULL
          , object_type_id         INT             NULL
          , managing_org_id        INT             NULL
          , managing_org           VARCHAR         NULL
          , lang_code              VARCHAR         NULL
          , access_type_id         INT             NULL
          , access_details         VARCHAR         NULL
          , access_details_url     VARCHAR         NULL
          , url_last_checked       DATE            NULL
          , eosc_category          INT             NULL
          , add_study_contribs     BOOLEAN         NULL
          , add_study_topics       BOOLEAN         NULL
          , datetime_of_data_fetch TIMESTAMPTZ     NULL
        );
        CREATE INDEX data_objects_sd_oid ON adcomp.data_objects(sd_oid);
        CREATE INDEX data_objects_sd_sid ON adcomp.data_objects(sd_sid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_datasets()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_datasets;
        CREATE TABLE adcomp.object_datasets(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , record_keys_type_id    INT             NULL 
          , record_keys_details    VARCHAR         NULL    
          , deident_type_id        INT             NULL  
          , deident_direct 	       BOOLEAN         NULL   
          , deident_hipaa 	       BOOLEAN         NULL   
          , deident_dates 	       BOOLEAN         NULL   
          , deident_nonarr 	       BOOLEAN         NULL   
          , deident_kanon	       BOOLEAN         NULL   
          , deident_details        VARCHAR         NULL    
          , consent_type_id        INT             NULL  
          , consent_noncommercial  BOOLEAN         NULL
          , consent_geog_restrict  BOOLEAN         NULL
          , consent_research_type  BOOLEAN         NULL
          , consent_genetic_only   BOOLEAN         NULL
          , consent_no_methods     BOOLEAN         NULL
          , consent_details        VARCHAR         NULL 
        );
        CREATE INDEX object_datasets_sd_oid ON adcomp.object_datasets(sd_oid);";

        Execute_SQL(sql_string);
    }
		

    public void create_table_object_dates()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_dates;
        CREATE TABLE adcomp.object_dates(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , date_type_id           INT             NULL
          , is_date_range          BOOLEAN         NULL default false
          , date_as_string         VARCHAR         NULL
          , start_year             INT             NULL
          , start_month            INT             NULL
          , start_day              INT             NULL
          , end_year               INT             NULL
          , end_month              INT             NULL
          , end_day                INT             NULL
          , details                VARCHAR         NULL
        );
        CREATE INDEX object_dates_sd_oid ON adcomp.object_dates(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_instances()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_instances;
        CREATE TABLE adcomp.object_instances(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , instance_type_id       INT             NOT NULL 
          , system_id      INT             NULL
          , system         VARCHAR         NULL
          , url                    VARCHAR         NULL
          , url_accessible         BOOLEAN         NULL
          , url_last_checked       DATE            NULL
          , resource_type_id       INT             NULL
          , resource_size          VARCHAR         NULL
          , resource_size_units    VARCHAR         NULL
          , resource_comments      VARCHAR         NULL
        );
        CREATE INDEX object_instances_sd_oid ON adcomp.object_instances(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_contributors()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_contributors;
        CREATE TABLE adcomp.object_contributors(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , contrib_type_id        INT             NULL
          , is_individual          BOOLEAN         NULL
          , organisation_id        INT             NULL
          , organisation_name      VARCHAR         NULL
          , person_id              INT             NULL
          , person_given_name      VARCHAR         NULL
          , person_family_name     VARCHAR         NULL
          , person_full_name       VARCHAR         NULL
          , person_identifier      VARCHAR         NULL
          , identifier_type        VARCHAR         NULL
          , person_affiliation     VARCHAR         NULL
          , affil_org_id           VARCHAR         NULL
          , affil_org_id_type      VARCHAR         NULL
        );
        CREATE INDEX object_contributors_sd_oid ON adcomp.object_contributors(sd_oid);";

        Execute_SQL(sql_string);
    }

   
    public void create_table_object_titles()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_titles;
        CREATE TABLE adcomp.object_titles(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , title_type_id          INT             NULL
          , title_text             VARCHAR         NULL
          , lang_code              VARCHAR         NULL
          , lang_usage_id          INT             NOT NULL default 11
          , is_default             BOOLEAN         NULL
          , comments               VARCHAR         NULL
          , comparison_text        VARCHAR         NULL
        );
        CREATE INDEX object_titles_sd_oid ON adcomp.object_titles(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_topics()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_topics;
        CREATE TABLE adcomp.object_topics(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , topic_type_id          INT             NULL
          , mesh_coded             BOOLEAN         NULL
          , topic_code             VARCHAR         NULL
          , topic_value            VARCHAR         NULL
          , topic_qualcode         VARCHAR         NULL
          , topic_qualvalue        VARCHAR         NULL
          , original_ct_id         INT             NULL
          , original_ct_code       VARCHAR         NULL
          , original_value         VARCHAR         NULL
          , comments               VARCHAR         NULL
        );
        CREATE INDEX object_topics_sd_oid ON adcomp.object_topics(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_comments()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_comments;
        CREATE TABLE adcomp.object_comments(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , ref_type               VARCHAR         NULL 
          , ref_source             VARCHAR         NULL 
          , pmid                   VARCHAR         NULL 
          , pmid_version           VARCHAR         NULL 
          , notes                  VARCHAR         NULL 
        );
        CREATE INDEX object_comments_sd_oid ON adcomp.object_comments(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_descriptions()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_descriptions;
        CREATE TABLE adcomp.object_descriptions(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , description_type_id    INT             NULL
          , label                  VARCHAR         NULL
          , description_text       VARCHAR         NULL
          , lang_code              VARCHAR         NULL
        );
        CREATE INDEX object_descriptions_sd_oid ON adcomp.object_descriptions(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_identifiers()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_identifiers;
        CREATE TABLE adcomp.object_identifiers(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , identifier_value       VARCHAR         NULL
          , identifier_type_id     INT             NULL
          , source_id      INT             NULL
          , source         VARCHAR         NULL
          , identifier_date        VARCHAR         NULL
        );
        CREATE INDEX object_identifiers_sd_oid ON adcomp.object_identifiers(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_db_links()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_db_links;
        CREATE TABLE adcomp.object_db_links(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , db_sequence            INT             NULL
          , db_name                VARCHAR         NULL
          , id_in_db               VARCHAR         NULL
        );
        CREATE INDEX object_db_links_sd_oid ON adcomp.object_db_links(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_publication_types()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_publication_types;
        CREATE TABLE adcomp.object_publication_types(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , type_name              VARCHAR         NULL
        );
        CREATE INDEX object_publication_types_sd_oid ON adcomp.object_publication_types(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_relationships()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_relationships;
        CREATE TABLE adcomp.object_relationships(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , relationship_type_id   INT             NULL
          , target_sd_oid          CHAR(24)        NULL
          , targetsd_sid           VARCHAR         NULL
          , targetseq_num          INT             NULL
        );
        CREATE INDEX object_relationships_sd_oid ON adcomp.object_relationships(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_object_rights()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.object_rights;
        CREATE TABLE adcomp.object_rights(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , rights_name            VARCHAR         NULL
          , rights_uri             VARCHAR         NULL
          , comments               VARCHAR         NULL
        );
        CREATE INDEX object_rights_sd_oid ON adcomp.object_rights(sd_oid);";

        Execute_SQL(sql_string);
    }


    public void create_table_citation_objects()
    {
        string sql_string = @"DROP TABLE IF EXISTS adcomp.citation_objects;
        CREATE TABLE adcomp.citation_objects(
            id                     INT             GENERATED ALWAYS AS IDENTITY (START WITH 4001 INCREMENT BY 1) PRIMARY KEY
          , sd_oid                 CHAR(24)        NULL
          , display_title          VARCHAR         NULL
          , version                VARCHAR         NULL
          , doi                    VARCHAR         NULL 
          , doi_status_id          INT             NULL
          , publication_year       INT             NULL
          , object_class_id        INT             NULL
          , object_type_id         INT             NULL
          , managing_org_id        INT             NULL
          , managing_org           VARCHAR         NULL
          , lang_code              VARCHAR         NULL
          , access_type_id         INT             NULL
          , access_details         VARCHAR         NULL
          , access_details_url     VARCHAR         NULL
          , url_last_checked       DATE            NULL
          , eosc_category          INT             NULL
          , add_study_contribs     BOOLEAN         NULL
          , add_study_topics       BOOLEAN         NULL
          , datetime_of_data_fetch TIMESTAMPTZ     NULL
          , journal_title          VARCHAR         NULL
          , pissn                  VARCHAR         NULL
          , eissn                  VARCHAR         NULL
        );
        CREATE INDEX citation_objects_sd_oid ON adcomp.citation_objects(sd_oid);";

        Execute_SQL(sql_string);
    }
}