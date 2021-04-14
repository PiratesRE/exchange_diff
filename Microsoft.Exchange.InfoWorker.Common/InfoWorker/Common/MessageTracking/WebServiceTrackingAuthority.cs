using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal abstract class WebServiceTrackingAuthority : TrackingAuthority
	{
		public Uri Uri
		{
			get
			{
				return this.uri;
			}
		}

		protected abstract void SetAuthenticationMechanism(ExchangeServiceBinding ewsBinding);

		public abstract string Domain { get; }

		public virtual SmtpAddress ProxyRecipient
		{
			get
			{
				return SmtpAddress.Empty;
			}
			protected set
			{
			}
		}

		protected WebServiceTrackingAuthority(TrackingAuthorityKind responsibleTracker, Uri uri) : base(responsibleTracker)
		{
			this.uri = uri;
		}

		public IWebServiceBinding GetEwsBinding(DirectoryContext directoryContext)
		{
			IClientProxy clientProxy;
			switch (base.TrackingAuthorityKind)
			{
			case TrackingAuthorityKind.RemoteSiteInCurrentOrg:
			{
				ExchangeServiceBinding exchangeServiceBinding = new ExchangeServiceBinding("MessageTracking", WebServiceTrackingAuthority.noValidationCallback);
				this.SetAuthenticationMechanism(exchangeServiceBinding);
				exchangeServiceBinding.Proxy = new WebProxy();
				RemoteSiteInCurrentOrgTrackingAuthority remoteSiteInCurrentOrgTrackingAuthority = (RemoteSiteInCurrentOrgTrackingAuthority)this;
				exchangeServiceBinding.Url = this.Uri.ToString();
				exchangeServiceBinding.UserAgent = WebServiceTrackingAuthority.EwsUserAgentString;
				exchangeServiceBinding.RequestServerVersionValue = new RequestServerVersion();
				exchangeServiceBinding.RequestServerVersionValue.Version = VersionConverter.GetExchangeVersionType(remoteSiteInCurrentOrgTrackingAuthority.ServerVersion);
				exchangeServiceBinding.CookieContainer = new CookieContainer();
				clientProxy = new ClientProxyEWS(exchangeServiceBinding, this.Uri, remoteSiteInCurrentOrgTrackingAuthority.ServerVersion);
				break;
			}
			case TrackingAuthorityKind.RemoteForest:
				clientProxy = new ClientProxyRD(directoryContext, this.ProxyRecipient, this.Domain, ExchangeVersion.Exchange2010);
				break;
			case TrackingAuthorityKind.RemoteTrustedOrg:
				clientProxy = new ClientProxyRD(directoryContext, this.ProxyRecipient, this.Domain, ExchangeVersion.Exchange2010_SP1);
				break;
			default:
				throw new NotImplementedException();
			}
			return new WebServiceBinding(clientProxy, directoryContext, this);
		}

		private static RemoteCertificateValidationCallback noValidationCallback = (object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslErr) => true;

		private Uri uri;

		private static readonly string EwsUserAgentString = WellKnownUserAgent.GetEwsNegoAuthUserAgent("Microsoft.Exchange.InfoWorker.Common.MessageTracking");
	}
}
