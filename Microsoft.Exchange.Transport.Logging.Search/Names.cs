using System;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal class Names<T> where T : struct
	{
		public static string[] Map = Enum.GetNames(typeof(T));
	}
}
