using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class CurrentSiteTrackingAuthority : TrackingAuthority
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
			return true;
		}

		public override string ToString()
		{
			return "Type=CurrentSiteTrackingAuthority";
		}

		private CurrentSiteTrackingAuthority() : base(TrackingAuthorityKind.CurrentSite)
		{
		}

		public static CurrentSiteTrackingAuthority Instance = new CurrentSiteTrackingAuthority();
	}
}
