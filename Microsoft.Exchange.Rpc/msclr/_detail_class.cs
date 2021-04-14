using System;

namespace msclr
{
	internal struct _detail_class
	{
		public static string _safe_true = _detail_class.dummy_struct.dummy_string;

		public static string _safe_false = null;

		public struct dummy_struct
		{
			public static readonly string dummy_string = "";
		}
	}
}
