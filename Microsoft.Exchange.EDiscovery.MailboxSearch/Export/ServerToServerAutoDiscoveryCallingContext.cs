using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.EDiscovery.Export.AutoDiscoverProxy;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class ServerToServerAutoDiscoveryCallingContext : BaseServiceCallingContext<DefaultBinding_Autodiscover>
	{
		public ServerToServerAutoDiscoveryCallingContext(IDictionary<Uri, string> remoteUrls)
		{
			this.remoteUrls = (remoteUrls ?? new Dictionary<Uri, string>());
		}

		public override DefaultBinding_Autodiscover CreateServiceBinding(Uri serviceUrl)
		{
			Tracer.TraceInformation("ServerToServerEwsCallingContext.CreateServiceBinding", new object[0]);
			DefaultBinding_Autodiscover defaultBinding_Autodiscover = base.CreateServiceBinding(serviceUrl);
			if (this.remoteUrls.ContainsKey(serviceUrl))
			{
				OAuthCredentials oauthCredentialsForAppToken = OAuthCredentials.GetOAuthCredentialsForAppToken(OrganizationId.ForestWideOrgId, this.remoteUrls[serviceUrl]);
				defaultBinding_Autodiscover.UseDefaultCredentials = false;
				defaultBinding_Autodiscover.Credentials = oauthCredentialsForAppToken;
			}
			else
			{
				defaultBinding_Autodiscover.UseDefaultCredentials = true;
				defaultBinding_Autodiscover.Disposed += delegate(object param0, EventArgs param1)
				{
					ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Remove(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ServerToServerEwsCallingContext.CertificateErrorHandler));
				};
				ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ServerToServerEwsCallingContext.CertificateErrorHandler));
			}
			return defaultBinding_Autodiscover;
		}

		public override void SetServiceApiContext(DefaultBinding_Autodiscover binding, string mailboxEmailAddress)
		{
			base.SetServiceApiContext(binding, mailboxEmailAddress);
			ICredentials credentials = binding.Credentials;
			if (!ServerToServerAutoDiscoveryCallingContext.IsCrossPremiseServiceBinding(binding))
			{
				binding.OpenAsAdminOrSystemService = new OpenAsAdminOrSystemServiceType
				{
					LogonType = SpecialLogonType.Admin,
					ConnectingSID = new ConnectingSIDType
					{
						Item = new PrimarySmtpAddressType
						{
							Value = mailboxEmailAddress
						}
					}
				};
			}
		}

		private static bool IsCrossPremiseServiceBinding(DefaultBinding_Autodiscover binding)
		{
			return !binding.UseDefaultCredentials;
		}

		private readonly IDictionary<Uri, string> remoteUrls;
	}
}
