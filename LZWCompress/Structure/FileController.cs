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
            var dictionaryOfLetters = new Dictionary<string, string>();
            var bbfWriting = new List<byte>();
            var characterList = new List<string>();
            var byteBuffer = new byte[bufferLenght];
            var aux = string.Empty;
            var letter = string.Empty;
            var lastLetter = string.Empty;

            if (!Directory.Exists(Path.Combine(routeDirectory, "compress"))) {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "compress"));
            }

            using (var readerFile = new BinaryReader(file.OpenReadStream())) {
                using (var stream = new FileStream(Path.Combine(routeDirectory, "compress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.lzw"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(stream)) {
                        while (readerFile.BaseStream.Position != readerFile.BaseStream.Length) {
                            byteBuffer = readerFile.ReadBytes(bufferLenght);
                            for (int i = 0; i < byteBuffer.Count(); i++) {
                                letter = Convert.ToString(Convert.ToChar(byteBuffer[i]));
                                if (!dictionaryOfLetters.ContainsKey(letter)) {
                                    var number = Convert.ToString(dictionaryOfLetters.Count() + 1, 2);
                                    dictionaryOfLetters.Add(letter, number);
                                    letter = string.Empty;
                                }
                            }
                        }

                        writer.Write(Encoding.ASCII.GetBytes(Convert.ToString(dictionaryOfLetters.Count).PadLeft(8, '0').ToCharArray()));

                        foreach (var rowList in dictionaryOfLetters) {
                            writer.Write(Convert.ToByte(Convert.ToChar(rowList.Key[0])));
                        }

                        readerFile.BaseStream.Position = 0;
                        letter = string.Empty;

                        while (readerFile.BaseStream.Position != readerFile.BaseStream.Length) {
                            byteBuffer = readerFile.ReadBytes(bufferLenght);
                            for (int i = 0; i < byteBuffer.Count(); i++) {
                                letter += Convert.ToString(Convert.ToChar(byteBuffer[i]));
                                if (!dictionaryOfLetters.ContainsKey(letter)) {
                                    var num = Convert.ToString(dictionaryOfLetters.Count() + 1, 2);
                                    dictionaryOfLetters.Add(letter, num);
                                    characterList.Add(dictionaryOfLetters[lastLetter]);
                                    lastLetter = string.Empty;
                                    lastLetter += letter.Last();
                                    letter = lastLetter;
                                } else {
                                    lastLetter = letter;
                                }
                            }
                        }

                        characterList.Add(dictionaryOfLetters[letter]);

                        var mBites = Math.Log2((float)dictionaryOfLetters.Count);
                        mBites = mBites % 1 >= 0.5 ? Convert.ToInt32(mBites) : Convert.ToInt32(mBites) + 1;

                        writer.Write(Convert.ToByte(mBites));

                        for (int i = 0; i < characterList.Count; i++) {
                            characterList[i] = characterList[i].PadLeft(Convert.ToInt32(mBites), '0');
                        }

                        foreach(var character in characterList) {
                            aux += character;
                            if (aux.Length >= 8) {
                                var max = aux.Length / 8;
                                for (int i = 0; i < max; i++) {
                                    bbfWriting.Add(Convert.ToByte(Convert.ToInt32(aux.Substring(0, 8), 2)));
                                    aux = aux.Substring(8);
                                }
                            }
                        }

                        if (aux.Length != 0) {
                            bbfWriting.Add(Convert.ToByte(Convert.ToInt32(aux.PadRight(8, '0'), 2)));
                        }

                        writer.Write(bbfWriting.ToArray());

                    }
                }
            }
        }

        public string Decompress(IFormFile file, string routeDirectory) {

            return " ";
        }
    }
}
