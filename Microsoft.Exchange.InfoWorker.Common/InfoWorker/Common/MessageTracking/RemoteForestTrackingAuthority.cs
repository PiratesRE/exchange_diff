using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class RemoteForestTrackingAuthority : ADAuthenticationTrackingAuthority
	{
		public static RemoteForestTrackingAuthority Create(string domain, SmtpAddress proxyRecipient)
		{
			return new RemoteForestTrackingAuthority(domain, proxyRecipient);
		}

		public override bool IsAllowedScope(SearchScope scope)
		{
			return scope == SearchScope.Organization || scope == SearchScope.World;
		}

		public override string ToString()
		{
			return string.Format("Type=RemoteForestTrackingAuthority,Domain={0}", this.domain);
		}

		public override SearchScope AssociatedScope
		{
			get
			{
				return SearchScope.Forest;
			}
		}

		public override string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public override SmtpAddress ProxyRecipient { get; protected set; }

		private RemoteForestTrackingAuthority(string domain, SmtpAddress proxyRecipient) : base(TrackingAuthorityKind.RemoteForest, null)
		{
			if (string.IsNullOrEmpty(domain) && SmtpAddress.Empty.Equals(proxyRecipient))
			{
				throw new ArgumentException("Either domain or proxyRecipient must be supplied, otherwise we cannot autodiscover the remote forest");
			}
			this.ProxyRecipient = proxyRecipient;
			this.domain = domain;
		}

		private string domain;
	}
}
