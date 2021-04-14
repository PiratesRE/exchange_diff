using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class UndefinedTrackingAuthority : TrackingAuthority
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
			return "Type=UndefinedTrackingAuthority";
		}

		private UndefinedTrackingAuthority() : base(TrackingAuthorityKind.Undefined)
		{
		}

		public static UndefinedTrackingAuthority Instance = new UndefinedTrackingAuthority();
	}
}
