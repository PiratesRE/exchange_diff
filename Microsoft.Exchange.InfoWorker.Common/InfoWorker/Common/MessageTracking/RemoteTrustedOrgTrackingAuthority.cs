using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class RemoteTrustedOrgTrackingAuthority : WebServiceTrackingAuthority
	{
		public static RemoteTrustedOrgTrackingAuthority Create(string domain, SmtpAddress proxyRecipient)
		{
			return new RemoteTrustedOrgTrackingAuthority(domain, TrackingAuthorityKind.RemoteTrustedOrg, proxyRecipient);
		}

		protected override void SetAuthenticationMechanism(ExchangeServiceBinding ewsBinding)
		{
			throw new NotImplementedException();
		}

		public override bool IsAllowedScope(SearchScope scope)
		{
			return scope == SearchScope.World;
		}

		public override string ToString()
		{
			return string.Format("Type=RemoteTrustedOrgTrackingAuthority,Domain={0}", this.domain);
		}

		public override SearchScope AssociatedScope
		{
			get
			{
				return SearchScope.Organization;
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

		private RemoteTrustedOrgTrackingAuthority(string domain, TrackingAuthorityKind responsibleTracker, SmtpAddress proxyRecipient) : base(responsibleTracker, null)
		{
			if (string.IsNullOrEmpty(domain) && SmtpAddress.Empty.Equals(proxyRecipient))
			{
				throw new ArgumentException("Either domain or proxyRecipient must be supplied, otherwise we cannot autodiscover the remote trusted organization");
			}
			this.domain = domain;
			this.ProxyRecipient = proxyRecipient;
		}

		private string domain;
	}
}
