using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class EwsHelper
	{
		private static ClientAccessType ClientAccessType
		{
			get
			{
				if (!LocalServer.GetServer().IsClientAccessServer)
				{
					return ClientAccessType.Internal;
				}
				return ClientAccessType.InternalNLBBypass;
			}
		}

		private static bool IsDatacenter
		{
			get
			{
				return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			}
		}

		public static string DiscoverEwsUrl(IExchangePrincipal mailbox)
		{
			Uri uri;
			if (!EwsHelper.IsDatacenter)
			{
				EwsHelper.Tracer.TraceDebug(0L, "Not running in DC. Will use ServiceTopology to discover the url");
				ServerVersion mailboxVersion = new ServerVersion(mailbox.MailboxInfo.Location.ServerVersion);
				IList<WebServicesService> list;
				try
				{
					ClientAccessType clientAccessType = EwsHelper.ClientAccessType;
					EwsHelper.Tracer.TraceDebug<ClientAccessType>(0L, "Will try to discover the URL for EWS with the following client access type: {0}", clientAccessType);
					ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Management\\EwsDriver\\EwsHelper.cs", "DiscoverEwsUrl", 119);
					list = currentServiceTopology.FindAll<WebServicesService>(mailbox, clientAccessType, delegate(WebServicesService service)
					{
						ServerVersion serverVersion = new ServerVersion(service.ServerVersionNumber);
						return mailboxVersion.Major == serverVersion.Major && mailboxVersion.Minor <= serverVersion.Minor && !service.IsOutOfService;
					}, "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Management\\EwsDriver\\EwsHelper.cs", "DiscoverEwsUrl", 121);
				}
				catch (LocalizedException arg)
				{
					EwsHelper.Tracer.TraceError<IExchangePrincipal, LocalizedException>(0L, "Unable to discover internal URL for EWS for mailbox {0} due exception {1}", mailbox, arg);
					return null;
				}
				if (list.Count == 0)
				{
					EwsHelper.Tracer.TraceError<IExchangePrincipal>(0L, "Unable to find internal URL for EWS for mailbox {0}", mailbox);
					return null;
				}
				WebServicesService webServicesService;
				if ((webServicesService = list.FirstOrDefault((WebServicesService service) => string.Equals(service.ServerFullyQualifiedDomainName, mailbox.MailboxInfo.Location.ServerFqdn, StringComparison.OrdinalIgnoreCase) && service.Url != null)) == null && (webServicesService = list.FirstOrDefault(delegate(WebServicesService service)
				{
					ServerVersion serverVersion = new ServerVersion(service.ServerVersionNumber);
					return service.Url != null && mailboxVersion.Major == serverVersion.Major && mailboxVersion.Minor == serverVersion.Minor;
				})) == null)
				{
					webServicesService = list.FirstOrDefault((WebServicesService service) => service.Url != null);
				}
				WebServicesService webServicesService2 = webServicesService;
				uri = ((webServicesService2 == null) ? null : webServicesService2.Url);
				goto IL_1EA;
			}
			EwsHelper.Tracer.TraceDebug(0L, "Running in DC. Will use BackEndLocator to discover the url");
			if (EwsHelper.discoveryEwsInternalUrl == null)
			{
				EwsHelper.discoveryEwsInternalUrl = (Func<IMailboxInfo, Uri>)Delegate.CreateDelegate(typeof(Func<IMailboxInfo, Uri>), Type.GetType("Microsoft.Exchange.Data.ApplicationLogic.Cafe.BackEndLocator, Microsoft.Exchange.Data.ApplicationLogic").GetMethod("GetBackEndWebServicesUrl", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					typeof(IMailboxInfo)
				}, null));
			}
			uri = EwsHelper.discoveryEwsInternalUrl(mailbox.MailboxInfo);
			IL_1EA:
			if (!(uri == null))
			{
				return uri.ToString();
			}
			return null;
		}

		public static string DiscoverExternalEwsUrl(ExchangePrincipal mailbox)
		{
			if (EwsHelper.discoveryEwsExternalUrl == null)
			{
				EwsHelper.discoveryEwsExternalUrl = (Func<IExchangePrincipal, Uri>)Delegate.CreateDelegate(typeof(Func<IExchangePrincipal, Uri>), Type.GetType("Microsoft.Exchange.Data.ApplicationLogic.Cafe.FrontEndLocator, Microsoft.Exchange.Data.ApplicationLogic").GetMethod("GetFrontEndWebServicesUrl", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					typeof(IExchangePrincipal)
				}, null));
			}
			Uri uri = EwsHelper.discoveryEwsExternalUrl(mailbox);
			if (!(uri == null))
			{
				return uri.ToString();
			}
			return null;
		}

		public static bool CertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			SslPolicyInfo.Instance.LastValidationTime = DateTime.UtcNow;
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				EwsHelper.Tracer.TraceDebug(0L, "Accepting SSL certificate because the only error is invalid hostname");
				return true;
			}
			if (SslConfiguration.AllowInternalUntrustedCerts)
			{
				EwsHelper.Tracer.TraceDebug(0L, "Accepting SSL certificate because registry config AllowInternalUntrustedCerts tells to ignore errors");
				return true;
			}
			EwsHelper.Tracer.TraceError<SslPolicyErrors>(0L, "Failed because SSL certificate contains the following errors: {0}", sslPolicyErrors);
			SslPolicyInfo.Instance.Add(new SslError(sslPolicyErrors, SslConfiguration.AllowInternalUntrustedCerts, SslConfiguration.AllowExternalUntrustedCerts));
			return false;
		}

		public static ExchangeObjectVersion EwsVersionToExchangeObjectVersion(ExchangeVersion ewsVersion)
		{
			if (ewsVersion < 1)
			{
				return ExchangeObjectVersion.Exchange2007;
			}
			if (ewsVersion < 4)
			{
				return ExchangeObjectVersion.Exchange2010;
			}
			return ExchangeObjectVersion.Exchange2012;
		}

		public static OpenAsAdminOrSystemServiceType CreateSpecialLogonAuthenticationHeader(IExchangePrincipal mailbox, SpecialLogonType logonType, OpenAsAdminOrSystemServiceBudgetTypeType budgetType, ExchangeVersion requestedServerVersion)
		{
			OpenAsAdminOrSystemServiceType openAsAdminOrSystemServiceType = new OpenAsAdminOrSystemServiceType
			{
				ConnectingSID = new ConnectingSIDType
				{
					Item = new PrimarySmtpAddressType
					{
						Value = mailbox.MailboxInfo.PrimarySmtpAddress.ToString()
					}
				},
				LogonType = logonType
			};
			if (requestedServerVersion >= 4)
			{
				openAsAdminOrSystemServiceType.BudgetType = (int)budgetType;
				openAsAdminOrSystemServiceType.BudgetTypeSpecified = true;
			}
			return openAsAdminOrSystemServiceType;
		}

		public static SerializedSecurityContextType CreateSerializedSecurityContext(IExchangePrincipal mailbox, SecurityAccessToken securityAccessToken)
		{
			return new SerializedSecurityContextType
			{
				UserSid = securityAccessToken.UserSid,
				GroupSids = EwsHelper.SidStringAndAttributesConverter(securityAccessToken.GroupSids),
				RestrictedGroupSids = EwsHelper.SidStringAndAttributesConverter(securityAccessToken.RestrictedGroupSids),
				PrimarySmtpAddress = mailbox.MailboxInfo.PrimarySmtpAddress.ToString()
			};
		}

		private static SidAndAttributesType[] SidStringAndAttributesConverter(SidStringAndAttributes[] sidStringAndAttributesArray)
		{
			if (sidStringAndAttributesArray == null)
			{
				return null;
			}
			return Array.ConvertAll<SidStringAndAttributes, SidAndAttributesType>(sidStringAndAttributesArray, (SidStringAndAttributes sidStringAndAttributes) => new SidAndAttributesType
			{
				SecurityIdentifier = sidStringAndAttributes.SecurityIdentifier,
				Attributes = sidStringAndAttributes.Attributes
			});
		}

		private static readonly Trace Tracer = ExTraceGlobals.StorageTracer;

		private static Func<IMailboxInfo, Uri> discoveryEwsInternalUrl;

		private static Func<IExchangePrincipal, Uri> discoveryEwsExternalUrl;
	}
}
