using System;
using System.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Internal
{
	internal class Globals
	{
		public static Guid ComponentGuid
		{
			get
			{
				return Globals.componentGuid;
			}
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string formatString, params object[] parameters)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
		}

		internal const string ComponentGuidString = "{82956720-170a-4dc6-8984-fb1816647d4e}";

		public static readonly StringPool StringPool = new StringPool(StringComparer.Ordinal);

		private static readonly Guid componentGuid = new Guid("{82956720-170a-4dc6-8984-fb1816647d4e}");
	}
}
