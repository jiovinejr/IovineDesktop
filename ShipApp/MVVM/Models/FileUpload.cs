﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApp.MVVM.Models
{
    public class FileUpload
    {
        public long Id { get; set; }
        public string? FileName { get; set; }
        public string? FileDriveId { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime TimeUploaded { get; set; }

        public override string ToString()
        {
            return $"FileName: {FileName}, DriveId: {FileDriveId}, IsProcessed: {IsProcessed}, TimeUploaded: {TimeUploaded}";
        }


    }


}
