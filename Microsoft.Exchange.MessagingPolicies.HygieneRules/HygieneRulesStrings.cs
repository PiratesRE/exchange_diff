using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies
{
	internal static class HygieneRulesStrings
	{
		public static LocalizedString CannotSetHeader(string name, string value)
		{
			return new LocalizedString("CannotSetHeader", HygieneRulesStrings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString InvalidHeaderName(string name)
		{
			return new LocalizedString("InvalidHeaderName", HygieneRulesStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InvalidAddress(string address)
		{
			return new LocalizedString("InvalidAddress", HygieneRulesStrings.ResourceManager, new object[]
			{
				address
			});
		}

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MessagingPolicies.HygieneRulesStrings", typeof(HygieneRulesStrings).GetTypeInfo().Assembly);

		private enum ParamIDs
		{
			CannotSetHeader,
			InvalidHeaderName,
			InvalidAddress
		}
	}
}
