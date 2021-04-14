using System;
using System.Text;

namespace System
{
	public static class CTSGlobals
	{
		public const int ReadBufferSize = 16384;

		public static Encoding AsciiEncoding = Encoding.GetEncoding("us-ascii");

		public static Encoding UnicodeEncoding = Encoding.GetEncoding("utf-16");

		public static Encoding Utf8Encoding = Encoding.GetEncoding("utf-8");
	}
}
