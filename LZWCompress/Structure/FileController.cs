using LZWCompress.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LZWCompress.Controllers {
    public class FileController : ICompressor {


        public void CompressFile(IFormFile file, string routeDirectory) {
            Compress(file, routeDirectory);
        }

        public string DecompressFile(IFormFile file, string routeDirectory) {
            return Decompress(file, routeDirectory);
        }

        public void Compress(IFormFile file, string routeDirectory) {

            var bufferLenght = 1000;
            var lettersDictionary = new Dictionary<string, string>();
            var bufferEscritura = new List<byte>();
            var listaCaracteres = new List<string>();
            var byteBuffer = new byte[bufferLenght];
            var aux = string.Empty;
            var letter = string.Empty;
            var anterior = string.Empty;

            if (!Directory.Exists(Path.Combine(routeDirectory, "compress"))) {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "compress"));
            }

            using (var readerFile = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "compress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.lzw"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {
                        while (readerFile.BaseStream.Position != readerFile.BaseStream.Length) {
                            byteBuffer = readerFile.ReadBytes(bufferLenght);
                            for (int i = 0; i < byteBuffer.Count(); i++) {
                                letter = Convert.ToString(Convert.ToChar(byteBuffer[i]));
                                if (!lettersDictionary.ContainsKey(letter)) {
                                    var stringnum = Convert.ToString(lettersDictionary.Count() + 1, 2);
                                    lettersDictionary.Add(letter, stringnum);
                                    letter = string.Empty;
                                }
                            }
                        }

                        writer.Write(Encoding.ASCII.GetBytes(Convert.ToString(lettersDictionary.Count).PadLeft(8, '0').ToCharArray()));

                        foreach (var fila in lettersDictionary) {
                            writer.Write(Convert.ToByte(Convert.ToChar(fila.Key[0])));
                        }

                        readerFile.BaseStream.Position = 0;
                        letter = string.Empty;

                        while (readerFile.BaseStream.Position != readerFile.BaseStream.Length) {
                            byteBuffer = readerFile.ReadBytes(bufferLenght);
                            for (int i = 0; i < byteBuffer.Count(); i++) {
                                letter += Convert.ToString(Convert.ToChar(byteBuffer[i]));
                                if (!lettersDictionary.ContainsKey(letter)) {
                                    var stringnum = Convert.ToString(lettersDictionary.Count() + 1, 2);
                                    lettersDictionary.Add(letter, stringnum);
                                    listaCaracteres.Add(lettersDictionary[anterior]);
                                    anterior = string.Empty;
                                    anterior += letter.Last();
                                    letter = anterior;
                                } else {
                                    anterior = letter;
                                }
                            }
                        }

                        listaCaracteres.Add(lettersDictionary[letter]);

                        var cantMaxBits = Math.Log2((float)lettersDictionary.Count);
                        cantMaxBits = cantMaxBits % 1 >= 0.5 ? Convert.ToInt32(cantMaxBits) : Convert.ToInt32(cantMaxBits) + 1;

                        writer.Write(Convert.ToByte(cantMaxBits));

                        for (int i = 0; i < listaCaracteres.Count; i++) {
                            listaCaracteres[i] = listaCaracteres[i].PadLeft(Convert.ToInt32(cantMaxBits), '0');
                        }

                        foreach(var item in listaCaracteres) {
                            aux += item;
                            if (aux.Length >= 8) {
                                var max = aux.Length / 8;
                                for (int i = 0; i < max; i++) {
                                    bufferEscritura.Add(Convert.ToByte(Convert.ToInt32(aux.Substring(0, 8), 2)));
                                    aux = aux.Substring(8);
                                }
                            }
                        }

                        if (aux.Length != 0) {
                            bufferEscritura.Add(Convert.ToByte(Convert.ToInt32(aux.PadRight(8, '0'), 2)));
                        }

                        writer.Write(bufferEscritura.ToArray());

                    }
                }
            }
        }

        public string Decompress(IFormFile file, string routeDirectory) {

            return " ";
        }
    }
}
