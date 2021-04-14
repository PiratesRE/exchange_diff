using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.Management.PowerShell
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2344500137U, "ExchangePSSnapInDescription");
		}

		public static string ExchangePSSnapInDescription
		{
			get
			{
				return Strings.ResourceManager.GetString("ExchangePSSnapInDescription");
			}
		}

		public static string GetLocalizedString(Strings.IDs key)
		{
			return Strings.ResourceManager.GetString(Strings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(1);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Management.PowerShell.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ExchangePSSnapInDescription = 2344500137U
		}
	}
}
