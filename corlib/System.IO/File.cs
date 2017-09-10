#if !LOCALTEST

using System.Text;
namespace System.IO {
	public static class File {

		public static FileStream OpenRead(string path) {
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

        public static FileStream OpenWrite(string path)
        {
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
		}

		public static StreamReader OpenText(string path) {
			return new StreamReader(path);
		}

		public static bool Exists(string path) {
			if (string.IsNullOrEmpty(path)) {
				return false;
			}
			int error;
			return FileInternal.ExistsFile(path, out error);
		}

        public static string ReadAllText(string path)
        {
            return ReadAllText(path, Encoding.UTF8);
        }


        public static string ReadAllText(string path, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(path, encoding))
            {
                return reader.ReadToEnd();
            }
        }
	}
}

#endif
