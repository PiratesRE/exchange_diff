using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal abstract class TrackingAuthority
	{
		public abstract SearchScope AssociatedScope { get; }

		public TrackingAuthorityKind TrackingAuthorityKind
		{
			get
			{
				return this.trackingAuthorityKind;
			}
		}

		public abstract bool IsAllowedScope(SearchScope scope);

		protected TrackingAuthority(TrackingAuthorityKind responsibleTracker)
		{
			this.trackingAuthorityKind = responsibleTracker;
		}

		private TrackingAuthorityKind trackingAuthorityKind;
	}
}
