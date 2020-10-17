using APILZW.Services;
using LZWCompress.Controllers;
using LZWCompress.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Mime;

namespace APILZW.Controllers {

    /// <summary>
    /// Class containing API methods
    /// </summary>
    [Route("api/")]
    [ApiController]
    public class FileCompressController : ControllerBase {
        #region Objects
        //Object that stores the project path
        private static string routeDirectory = Environment.CurrentDirectory;
        #endregion

        #region Methods
        /// <summary>
        /// Default API method
        /// </summary>
        /// <returns> Returns an OK status </returns>
        [HttpGet("home")]
        public ActionResult Get() {
            return Ok();
        }

        /// <summary>
        /// Method to get the file sent
        /// </summary>
        /// <param name="file">File sent</param>
        /// <returns>Returns the compressed file</returns>
        [HttpPost ("compress")]
        public ActionResult Compress([FromForm] IFormFile file) {
            if (file.Length != 0 && file != null) {
                FileController fileController = new FileController();
                fileController.CompressFile(file, routeDirectory);
                DataMapping(file);
                return ReturnLZWFile(file);

            } else {
                return StatusCode(500, "InternalServerError");
            }
        }

        /// <summary>
        /// Method to get the file sent
        /// </summary>
        /// <param name="file">File sent</param>
        /// <returns>Return the decompressed file</returns>
        [HttpPost("decompress")]
        public ActionResult Decompress([FromForm] IFormFile file) {
            if (file != null) {
                FileController fileController = new FileController();
                string path = fileController.DecompressFile(file, routeDirectory);
                return ReturnTextFile(file);
            } else {
                return StatusCode(500, "InternalServerError");
            }

        }

        /// <summary>
        /// Method to build the returnable file
        /// </summary>
        /// <param name="file">File sent</param>
        /// <returns></returns>
        public ActionResult ReturnLZWFile([FromForm] IFormFile file) {
            return PhysicalFile(Storage.Instance.actualFile.CompressedFilePath, MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(file.FileName)}.lzw");
        }

        /// <summary>
        /// Method to build the returnable file
        /// </summary>
        /// <param name="file">File sent</param>
        /// <returns></returns>
        public ActionResult ReturnTextFile([FromForm] IFormFile file) {
            return PhysicalFile(Path.Combine(routeDirectory, "decompress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt");
        }

        /// <summary>
        /// Method to store the metadata
        /// </summary>
        /// <param name="file">File sent</param>
        public void DataMapping(IFormFile file) {
            Storage.Instance.actualFile = new FileHistory();
            Storage.Instance.actualFile.FileName = Path.GetFileNameWithoutExtension(file.FileName);
            Storage.Instance.actualFile.CompressedFilePath = Path.Combine(
                routeDirectory, "compress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.lzw");
            Storage.Instance.actualFile.CompressionFactor = (double)(new FileInfo(Storage.Instance.actualFile.CompressedFilePath).Length / (double)file.Length);
            Storage.Instance.actualFile.CompressionRatio = (double)(file.Length / (double)(new FileInfo(Storage.Instance.actualFile.CompressedFilePath).Length));
            Storage.Instance.actualFile.ReductionPortentage = (double)((double)(new FileInfo(Storage.Instance.actualFile.CompressedFilePath).Length) * 100) / (double)file.Length;
            Storage.Instance.files.Add(Storage.Instance.actualFile);
        }

        #endregion
    }
}
