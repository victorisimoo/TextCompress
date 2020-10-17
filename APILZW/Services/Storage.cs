using LZWCompress.Model;
using System.Collections.Generic;

namespace APILZW.Services {
    public class Storage {

        #region Parameters
        private static Storage _instance = null;
        public List<FileHistory> files = new List<FileHistory>();
        public FileHistory actualFile = new FileHistory();
        #endregion

        #region Method
        /// <summary>
        /// Method to return instant initialized
        /// </summary>
        public static Storage Instance {
            get {
                if (_instance == null) _instance = new Storage();
                return _instance;
            }
        }
        #endregion

    }
}
