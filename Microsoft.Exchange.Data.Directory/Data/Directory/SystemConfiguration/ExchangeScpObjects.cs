using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class ExchangeScpObjects
	{
		internal static class AutodiscoverContainer
		{
			public const string Name = "Microsoft Exchange Autodiscover";

			public const string CN = "CN=Microsoft Exchange Autodiscover";
		}

		internal static class AutodiscoverUrlKeyword
		{
			public const string Value = "77378F46-2C66-4aa9-A6A6-3E7A48B19596";

			public static readonly QueryFilter Filter = new TextFilter(ADServiceConnectionPointSchema.Keywords, "77378F46-2C66-4aa9-A6A6-3E7A48B19596", MatchOptions.FullString, MatchFlags.IgnoreCase);
		}

		internal static class AutodiscoverDomainPointerKeyword
		{
			public const string Value = "67661d7F-8FC4-4fa7-BFAC-E1D7794C1F68";

			public static readonly QueryFilter Filter = new TextFilter(ADServiceConnectionPointSchema.Keywords, "67661d7F-8FC4-4fa7-BFAC-E1D7794C1F68", MatchOptions.FullString, MatchFlags.IgnoreCase);
		}

		internal static class AutodiscoverTrustedHosterKeyword
		{
			public const string Value = "D3614C7C-D214-4F1F-BD4C-00D91C67F93F";

			public static readonly QueryFilter Filter = new TextFilter(ADServiceConnectionPointSchema.Keywords, "D3614C7C-D214-4F1F-BD4C-00D91C67F93F", MatchOptions.FullString, MatchFlags.IgnoreCase);
		}

		internal static class DomainToApplicationUriKeyword
		{
			public const string Value = "E1AA5F5E-2341-4FCB-8560-E3AB6F081468";

			public const string DomainPrefix = "Domain=";

			public static readonly QueryFilter Filter = new TextFilter(ADServiceConnectionPointSchema.Keywords, "E1AA5F5E-2341-4FCB-8560-E3AB6F081468", MatchOptions.FullString, MatchFlags.IgnoreCase);
		}
	}
}
