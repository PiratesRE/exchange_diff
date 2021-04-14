using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class LegacyExchangeServerTrackingAuthority : TrackingAuthority
	{
		public override SearchScope AssociatedScope
		{
			get
			{
				return SearchScope.Site;
			}
		}

		public override bool IsAllowedScope(SearchScope scope)
		{
			return false;
		}

		public override string ToString()
		{
			return "Type=LegacyExchangeServerTrackingAuthority";
		}

		private LegacyExchangeServerTrackingAuthority() : base(TrackingAuthorityKind.LegacyExchangeServer)
		{
		}

		public static LegacyExchangeServerTrackingAuthority Instance = new LegacyExchangeServerTrackingAuthority();
	}
}
