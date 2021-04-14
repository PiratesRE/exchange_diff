using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Win32;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal sealed class ServerToServerEwsCallingContext : BaseServiceCallingContext<ExchangeServiceBinding>
	{
		public ServerToServerEwsCallingContext(IDictionary<Uri, string> remoteUrls)
		{
			this.remoteUrls = (remoteUrls ?? new Dictionary<Uri, string>());
		}

		public override ExchangeServiceBinding CreateServiceBinding(Uri serviceUrl)
		{
			Tracer.TraceInformation("ServerToServerEwsCallingContext.CreateServiceBinding", new object[0]);
			ExchangeServiceBinding exchangeServiceBinding = base.CreateServiceBinding(serviceUrl);
			if (this.remoteUrls.ContainsKey(serviceUrl))
			{
				OAuthCredentials oauthCredentialsForAppToken = OAuthCredentials.GetOAuthCredentialsForAppToken(OrganizationId.ForestWideOrgId, this.remoteUrls[serviceUrl]);
				exchangeServiceBinding.UseDefaultCredentials = false;
				exchangeServiceBinding.Credentials = oauthCredentialsForAppToken;
				exchangeServiceBinding.ManagementRole = new ManagementRoleType
				{
					ApplicationRoles = new string[]
					{
						"MailboxSearchApplication"
					}
				};
			}
			else
			{
				exchangeServiceBinding.UseDefaultCredentials = true;
				exchangeServiceBinding.Disposed += delegate(object param0, EventArgs param1)
				{
					ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Remove(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ServerToServerEwsCallingContext.CertificateErrorHandler));
				};
				ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ServerToServerEwsCallingContext.CertificateErrorHandler));
			}
			return exchangeServiceBinding;
		}

		public override void SetServiceApiContext(ExchangeServiceBinding binding, string mailboxEmailAddress)
		{
			base.SetServiceApiContext(binding, mailboxEmailAddress);
			OAuthCredentials oauthCredentials = binding.Credentials as OAuthCredentials;
			if (oauthCredentials != null)
			{
				oauthCredentials.ClientRequestId = new Guid?(binding.HttpContext.ClientRequestId);
			}
			if (!ServerToServerEwsCallingContext.IsCrossPremiseServiceBinding(binding))
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

		internal static bool CertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				Tracer.TraceInformation("Accepting SSL certificate because the only error is invalid hostname", new object[0]);
				return true;
			}
			if (ServerToServerEwsCallingContext.AllowInternalUntrustedCerts())
			{
				Tracer.TraceInformation("Accepting SSL certificate because registry config AllowInternalUntrustedCerts tells to ignore errors", new object[0]);
				return true;
			}
			Tracer.TraceInformation("Failed because SSL certificate contains the following errors: {0}", new object[]
			{
				sslPolicyErrors
			});
			return false;
		}

		private static bool AllowInternalUntrustedCerts()
		{
			if (ServerToServerEwsCallingContext.allowInternalUntrustedCerts == null || ServerToServerEwsCallingContext.allowInternalUntrustedCerts == null)
			{
				ServerToServerEwsCallingContext.allowInternalUntrustedCerts = new bool?(true);
				RegistryKey registryKey = null;
				try
				{
					registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA");
					if (registryKey != null)
					{
						object value = registryKey.GetValue("AllowInternalUntrustedCerts");
						if (value != null && value is int)
						{
							Tracer.TraceInformation("Registry value: {0} was found. Its value is: {1}", new object[]
							{
								"AllowInternalUntrustedCerts",
								value
							});
							ServerToServerEwsCallingContext.allowInternalUntrustedCerts = new bool?((int)value != 0);
						}
						else
						{
							Tracer.TraceInformation("Registry value: {0} was not found or invalid. Use default value: {1}.", new object[]
							{
								"AllowInternalUntrustedCerts",
								true
							});
						}
					}
				}
				catch (SecurityException)
				{
					Tracer.TraceInformation("Failed reading registry key. Use default value: {0}", new object[]
					{
						true
					});
				}
				finally
				{
					if (registryKey != null)
					{
						registryKey.Close();
					}
				}
			}
			return ServerToServerEwsCallingContext.allowInternalUntrustedCerts.Value;
		}

		private static bool IsCrossPremiseServiceBinding(ExchangeServiceBinding binding)
		{
			return !binding.UseDefaultCredentials;
		}

		private static bool? allowInternalUntrustedCerts;

		private readonly IDictionary<Uri, string> remoteUrls;
	}
}
