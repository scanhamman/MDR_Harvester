﻿using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace MDR_Harvester
{
    public class ObjectController
    {
        LoggingHelper _logger;
        IMonitorDataLayer _mon_repo;
        IStorageDataLayer _storage_repo;
        IObjectProcessor _processor;
        ISource _source;

        public ObjectController(LoggingHelper logger, IMonitorDataLayer mon_repo, IStorageDataLayer storage_repo,
                              ISource source, IObjectProcessor processor)
        {
            _logger = logger;
            _mon_repo = mon_repo;
            _storage_repo = storage_repo;
            _processor = processor;
            _source = source;
        }

        public int? LoopThroughFiles(int harvest_type_id, int harvest_id)
        {
            // Loop through the available records a chunk at a time (may be 1 for smaller record sources)
            // First get the total number of records in the system for this source
            // Set up the outer limit and get the relevant records for each pass

            int total_amount = _mon_repo.FetchFileRecordsCount(_source.id, _source.source_type, harvest_type_id);
            int chunk = _source.harvest_chunk;
            int k = 0;
            for (int m = 0; m < total_amount; m += chunk)
            {
                // if (k > 2000) break; // for testing...

                IEnumerable<ObjectFileRecord> file_list = _mon_repo
                        .FetchObjectFileRecordsByOffset(_source.id, m, chunk, harvest_type_id);

                int n = 0; string filePath = "";
                foreach (ObjectFileRecord rec in file_list)
                {
                    // if (k > 50) break; // for testing...

                    n++; k++;
                    filePath = rec.local_path;
                    if (File.Exists(filePath))
                    {
                        string jsonString = File.ReadAllText(filePath);
                        FullDataObject? s = _processor.ProcessData(jsonString, rec.last_downloaded);

                        if (s is not null)
                        {
                            // store the data in the database			
                            _storage_repo.StoreFullObject(s, _source);

                            // update file record with last processed datetime
                            // (if not in test mode)
                            if (harvest_type_id != 3)
                            {
                                _mon_repo.UpdateFileRecLastHarvested(rec.id, _source.source_type, harvest_id);
                            }
                        }
                    }

                    if (k % chunk == 0) _logger.LogLine("Records harvested: " + k.ToString());
                }

            }

            return k;
        }

    }
}
