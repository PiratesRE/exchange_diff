using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class RemoteOrgTrackingAuthority : TrackingAuthority
	{
		public override SearchScope AssociatedScope
		{
			get
			{
				return SearchScope.Organization;
			}
		}

		public override bool IsAllowedScope(SearchScope scope)
		{
			return false;
		}

		public override string ToString()
		{
			return "Type=RemoteOrgTrackingAuthority";
		}

		private RemoteOrgTrackingAuthority() : base(TrackingAuthorityKind.RemoteOrg)
		{
		}

		public static RemoteOrgTrackingAuthority Instance = new RemoteOrgTrackingAuthority();
	}
}
