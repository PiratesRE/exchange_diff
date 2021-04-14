using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.AnchorService
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1191245078U, "AnchorMailboxNotFound");
			Strings.stringIDs.Add(3166985769U, "MultipleAnchorMailboxesFound");
		}

		public static LocalizedString AnchorMailboxNotFound
		{
			get
			{
				return new LocalizedString("AnchorMailboxNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleAnchorMailboxesFound
		{
			get
			{
				return new LocalizedString("MultipleAnchorMailboxesFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.AnchorService.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			AnchorMailboxNotFound = 1191245078U,
			MultipleAnchorMailboxesFound = 3166985769U
		}
	}
}
