using System;
using System.Collections.Generic;
using System.Text;

namespace LZWCompress.Model {

    /// <summary>
    /// Class for storing file metadata
    /// </summary>
    public class FileHistory {
        #region Parameters
        public String FileName { get; set; } //Original file name
        public String FileCompressName { get; set; }
        public String CompressedFilePath { get; set; }
        public double CompressionRatio { get; set; }
        public double CompressionFactor { get; set; }
        public double ReductionPortentage { get; set; }
        #endregion

        #region Contructor
        public FileHistory() { }
        #endregion
    }
}
