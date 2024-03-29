﻿namespace MDR_Harvester;

public class ObjectController
{
    private readonly ILoggingHelper _loggingHelper;
    private readonly IMonDataLayer _monDataLayer;
    private readonly IStorageDataLayer _storageDataLayer;
    private readonly IObjectProcessor _processor;
    private readonly Source _source;

    public ObjectController(ILoggingHelper loggingHelper, IMonDataLayer monDataLayer, IStorageDataLayer storageDataLayer,
                            Source source, IObjectProcessor processor)
    {
        _loggingHelper = loggingHelper;
        _monDataLayer = monDataLayer;
        _storageDataLayer = storageDataLayer;
        _processor = processor;
        _source = source;
    }

    public int? LoopThroughFiles(int harvest_type_id, int harvest_id)
    {
        // Loop through the available records a chunk at a time (may be 1 for smaller record sources)
        // First get the total number of records in the system for this source
        // Set up the outer limit and get the relevant records for each pass.

        int amount_to_fetch = _monDataLayer.FetchFileRecordsCount(harvest_type_id);
        int chunk = _source.harvest_chunk ?? 0;
        int k = 0;
        for (int m = 0; m < amount_to_fetch; m += chunk)
        {
            // if (k > 2000) break; // for testing...

            IEnumerable<ObjectFileRecord> file_list = _monDataLayer
                    .FetchObjectFileRecordsByOffset(m, chunk, harvest_type_id);

            foreach (ObjectFileRecord rec in file_list)
            {
                // if (k > 50) break; // for testing...

                k++;
                string? filePath = rec.local_path;
                if (filePath is not null && File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    FullDataObject? s = _processor.ProcessData(jsonString, rec.last_downloaded, _loggingHelper);

                    if (s is not null)
                    {
                        // store the data in the database			
                        _storageDataLayer.StoreFullObject(s, _source);

                        // update file record with last processed datetime
                        // (if not in test mode)
                        if (harvest_type_id != 3)
                        {
                            _monDataLayer.UpdateFileRecLastHarvested(rec.id, _source.source_type!, harvest_id);
                        }
                    }
                }

                if (k % chunk == 0) _loggingHelper.LogLine("Records harvested: " + k.ToString());
            }

        }
        return k;
    }
}