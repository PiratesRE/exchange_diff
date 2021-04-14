using System;
using System.Text;

namespace Microsoft.Exchange.RpcHttpModules
{
	public static class Utilities
	{
		public static string EncodeToBase64(string input)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
		}

		public static string DecodeFromBase64(string input)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(input));
		}
	}
}
