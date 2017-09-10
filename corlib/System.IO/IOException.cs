#if !LOCALTEST

namespace System.IO {
	public class IOException : SystemException {

		public IOException() : base("I/O Error") { }
		public IOException(string message) : base(message) { }
		public IOException(string message,Exception inner) : base(message,inner) { }

	}
}

#endif
