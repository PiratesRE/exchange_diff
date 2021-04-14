using System;
using Microsoft.Exchange.Clients;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class OwaLocalizedStrings
	{
		internal static LocalizedString ErrorWrongCASServerBecauseOfOutOfDateDNSCache
		{
			get
			{
				return new LocalizedString(-23402676.ToString(), OwaLocalizedStrings.ResourceManager, new object[0]);
			}
		}

		private static readonly ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager(typeof(Strings).FullName, typeof(Strings).Assembly);
	}
}
