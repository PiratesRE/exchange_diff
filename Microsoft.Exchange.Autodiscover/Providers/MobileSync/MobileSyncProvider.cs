using System;
using System.Xml;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover.Providers.MobileSync
{
	[Provider("04A1AD64-3877-4008-B0D2-6AB963D9E40E", "http://schemas.microsoft.com/exchange/autodiscover/mobilesync/requestschema/2006", "MobileSyncRequestSchema2006.xsd", "http://schemas.microsoft.com/exchange/autodiscover/mobilesync/responseschema/2006", "MobileSyncResponseSchema2006.xsd", Description = "MobileSync 12 provider for Exchange Autodiscover Service")]
	internal class MobileSyncProvider : Provider
	{
		protected string ResponseNs { get; set; }

		public MobileSyncProvider(RequestData requestData) : base(requestData)
		{
			this.ResponseNs = base.RequestData.ResponseSchemas[0];
			if (base.RequestData.EMailAddress == null)
			{
				throw new InvalidOperationException("The Autodiscover request must contain an EMailAddress");
			}
			base.ResolveRequestedADRecipient();
		}

		public void WriteErrorResponse(XmlWriter xmlFragment, string errorCode, string message, string debugData, RequestData requestData, string name, string emailAddress)
		{
			if (string.IsNullOrEmpty(this.ResponseNs))
			{
				this.ResponseNs = "http://schemas.microsoft.com/exchange/autodiscover/mobilesync/responseschema/2006";
			}
			Common.StartEnvelope(xmlFragment);
			xmlFragment.WriteStartElement("Response", this.ResponseNs);
			xmlFragment.WriteElementString("Culture", this.ResponseNs, MobileSyncProvider.defaultCulture);
			if (string.IsNullOrEmpty(name))
			{
				this.WriteUserFragment(xmlFragment, null, requestData.EMailAddress);
			}
			else
			{
				this.WriteUserFragment(xmlFragment, name, emailAddress);
			}
			xmlFragment.WriteStartElement("Action", this.ResponseNs);
			xmlFragment.WriteStartElement("Error", this.ResponseNs);
			xmlFragment.WriteElementString("Status", this.ResponseNs, errorCode);
			xmlFragment.WriteElementString("Message", this.ResponseNs, message);
			xmlFragment.WriteElementString("DebugData", debugData);
			xmlFragment.WriteEndElement();
			xmlFragment.WriteEndElement();
			xmlFragment.WriteEndElement();
			Common.EndEnvelope(xmlFragment);
			Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_WarnProvErrorResponse, Common.PeriodicKey, new object[]
			{
				requestData.Timestamp,
				requestData.ComputerNameHash,
				errorCode,
				message,
				debugData,
				requestData.EMailAddress,
				requestData.LegacyDN,
				base.GetType().AssemblyQualifiedName
			});
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("MobileSyncConfigFailure", base.RequestData.LegacyDN ?? base.RequestData.EMailAddress);
		}

		public override string Get302RedirectUrl()
		{
			return null;
		}

		protected override bool WriteRedirectXml(XmlWriter xmlFragment)
		{
			bool result;
			if (base.RequestedRecipient == null)
			{
				this.WriteErrorResponse(xmlFragment, "2", Strings.MobileSyncAddressNotFound.ToString(), string.Empty, base.RequestData, null, null);
				result = true;
			}
			else if ((base.RequestedRecipient.RecipientType == RecipientType.MailUser || base.RequestedRecipient.RecipientType == RecipientType.MailContact) && string.Compare(base.RequestedRecipient.ExternalEmailAddress.PrefixString, "SMTP", StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = true;
				string mobileName;
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.EnableMobileSyncRedirectBypass.Enabled && MobileRedirectOptimization.TryGetEasServerFromConfig(base.RequestedRecipient, base.RequestData.UserAgent, out mobileName))
				{
					Common.StartEnvelope(xmlFragment);
					xmlFragment.WriteStartElement("Response", this.ResponseNs);
					xmlFragment.WriteElementString("Culture", this.ResponseNs, MobileSyncProvider.defaultCulture);
					this.WriteUserFragment(xmlFragment, base.RequestedRecipient.Name, base.RequestedRecipient.ExternalEmailAddress.AddressString);
					xmlFragment.WriteStartElement("Action", this.ResponseNs);
					xmlFragment.WriteStartElement("Settings", this.ResponseNs);
					this.WriteServerFragment(xmlFragment, mobileName, null, null, false);
					xmlFragment.WriteEndElement();
					xmlFragment.WriteEndElement();
					xmlFragment.WriteEndElement();
					Common.EndEnvelope(xmlFragment);
					Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_InfoProvRedirectBypassConfigurationResponse, Common.PeriodicKey, new object[]
					{
						(base.RequestData.LegacyDN == null) ? base.RequestData.EMailAddress : base.RequestData.LegacyDN
					});
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("MobileSyncConfigSuccess", base.RequestData.LegacyDN ?? base.RequestData.EMailAddress);
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("RedirectBypass", true);
				}
				else
				{
					Common.StartEnvelope(xmlFragment);
					xmlFragment.WriteStartElement("Response", this.ResponseNs);
					xmlFragment.WriteElementString("Culture", this.ResponseNs, MobileSyncProvider.defaultCulture);
					this.WriteUserFragment(xmlFragment, base.RequestedRecipient.Name, base.RequestedRecipient.ExternalEmailAddress.AddressString);
					xmlFragment.WriteStartElement("Action", this.ResponseNs);
					xmlFragment.WriteElementString("Redirect", this.ResponseNs, base.RequestedRecipient.ExternalEmailAddress.AddressString);
					xmlFragment.WriteEndElement();
					xmlFragment.WriteEndElement();
					Common.EndEnvelope(xmlFragment);
					Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_InfoProvRedirectionResponse, Common.PeriodicKey, new object[]
					{
						base.RequestedRecipient.ExternalEmailAddress.AddressString,
						base.RequestedRecipient.ExternalEmailAddress.AddressString,
						base.RequestedRecipient.LegacyExchangeDN,
						base.GetType().AssemblyQualifiedName
					});
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.SetRedirectionType(RedirectionType.EmailAddressRedirect);
				}
			}
			else if (base.RequestedRecipient.RecipientType == RecipientType.UserMailbox)
			{
				result = false;
			}
			else
			{
				this.WriteErrorResponse(xmlFragment, "9", Strings.MobileSyncBadAddress.ToString(), base.RequestedRecipient.RecipientType.ToString(), base.RequestData, base.RequestedRecipient.Name, base.RequestedRecipient.PrimarySmtpAddress.ToString());
				result = true;
			}
			return result;
		}

		protected override void WriteConfigXml(XmlWriter xmlFragment)
		{
			if (base.RequestedRecipient == null)
			{
				this.WriteErrorResponse(xmlFragment, "2", Strings.MobileSyncAddressNotFound.ToString(), string.Empty, base.RequestData, null, null);
				return;
			}
			if (base.RequestedRecipient.RecipientType != RecipientType.UserMailbox)
			{
				this.WriteErrorResponse(xmlFragment, "9", Strings.MobileSyncBadAddress.ToString(), base.RequestedRecipient.RecipientType.ToString(), base.RequestData, base.RequestedRecipient.DisplayName, base.RequestedRecipient.PrimarySmtpAddress.ToString());
				return;
			}
			ADUser adUser = base.RequestedRecipient as ADUser;
			ExchangePrincipal exchangePrincipal = Common.GetExchangePrincipal(adUser);
			if (exchangePrincipal == null)
			{
				this.WriteErrorResponse(xmlFragment, "1", Strings.ActiveDirectoryFailure.ToString(), base.RequestedRecipient.RecipientType.ToString(), base.RequestData, base.RequestedRecipient.DisplayName, base.RequestedRecipient.PrimarySmtpAddress.ToString());
				return;
			}
			string mobileName = null;
			string certUrl = null;
			string certTemplate = null;
			bool certEnabled = false;
			VariantConfigurationSnapshot configuration = exchangePrincipal.GetConfiguration();
			if (Common.SkipServiceTopologyInDatacenter(configuration))
			{
				Uri datacenterFrontEndEasUrl = FrontEndLocator.GetDatacenterFrontEndEasUrl();
				if (datacenterFrontEndEasUrl != null)
				{
					mobileName = datacenterFrontEndEasUrl.ToString();
				}
			}
			else
			{
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\autodisc\\src\\Providers\\MobileSync\\Web\\MobileSyncProvider.cs", "WriteConfigXml", 376);
				Site site = currentServiceTopology.GetSite(exchangePrincipal.MailboxInfo.Location.ServerFqdn, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\Providers\\MobileSync\\Web\\MobileSyncProvider.cs", "WriteConfigXml", 377);
				SiteCostComparer<MobileSyncService> comparer = new SiteCostComparer<MobileSyncService>(currentServiceTopology, site);
				MobileSyncService cheapest = null;
				MobileSyncService cheapestToProxy = null;
				currentServiceTopology.ForEach<MobileSyncService>(delegate(MobileSyncService service)
				{
					if (service.ClientAccessType == ClientAccessType.External)
					{
						if (service.ServerVersionNumber == exchangePrincipal.MailboxInfo.Location.ServerVersion && (cheapest == null || comparer.Compare(cheapest, service) > 0))
						{
							cheapest = service;
							return;
						}
						if (cheapestToProxy == null || comparer.Compare(cheapestToProxy, service) > 0)
						{
							if (service.ServerVersionNumber > exchangePrincipal.MailboxInfo.Location.ServerVersion)
							{
								cheapestToProxy = service;
								return;
							}
							ServerVersion serverVersion = new ServerVersion(exchangePrincipal.MailboxInfo.Location.ServerVersion);
							ServerVersion serverVersion2 = new ServerVersion(service.ServerVersionNumber);
							if (serverVersion2.Major >= serverVersion.Major && serverVersion2.Minor >= serverVersion.Minor)
							{
								cheapestToProxy = service;
							}
						}
					}
				}, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\Providers\\MobileSync\\Web\\MobileSyncProvider.cs", "WriteConfigXml", 388);
				if (cheapest == null && cheapestToProxy == null)
				{
					this.WriteErrorResponse(xmlFragment, "1", Strings.ExternalUrlNotFound.ToString(), base.RequestedRecipient.RecipientType.ToString(), base.RequestData, base.RequestedRecipient.DisplayName, base.RequestedRecipient.PrimarySmtpAddress.ToString());
					return;
				}
				if (cheapest == null || (cheapestToProxy != null && comparer.Compare(cheapest, cheapestToProxy) > 0))
				{
					cheapest = cheapestToProxy;
				}
				mobileName = cheapest.Url.ToString();
				certUrl = cheapest.CertificateAuthorityUrl;
				certTemplate = cheapest.CertEnrollTemplateName;
				certEnabled = cheapest.IsCertEnrollEnabled;
			}
			Common.StartEnvelope(xmlFragment);
			xmlFragment.WriteStartElement("Response", this.ResponseNs);
			xmlFragment.WriteElementString("Culture", this.ResponseNs, MobileSyncProvider.defaultCulture);
			this.WriteUserFragment(xmlFragment, base.RequestedRecipient.DisplayName, base.RequestedRecipient.PrimarySmtpAddress.ToString());
			xmlFragment.WriteStartElement("Action", this.ResponseNs);
			xmlFragment.WriteStartElement("Settings", this.ResponseNs);
			this.WriteServerFragment(xmlFragment, mobileName, certUrl, certTemplate, certEnabled);
			xmlFragment.WriteEndElement();
			this.WriteMailboxesFragment(xmlFragment, exchangePrincipal);
			xmlFragment.WriteEndElement();
			xmlFragment.WriteEndElement();
			Common.EndEnvelope(xmlFragment);
			Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_InfoProvConfigurationResponse, Common.PeriodicKey, new object[]
			{
				(base.RequestData.LegacyDN == null) ? base.RequestData.EMailAddress : base.RequestData.LegacyDN
			});
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("MobileSyncConfigSuccess", base.RequestData.LegacyDN ?? base.RequestData.EMailAddress);
		}

		protected virtual void WriteMailboxesFragment(XmlWriter xmlFragment, ExchangePrincipal exchangePrincipal)
		{
		}

		private void WriteUserFragment(XmlWriter xmlFragment, string name, string emailAddress)
		{
			xmlFragment.WriteStartElement("User", this.ResponseNs);
			if (!string.IsNullOrEmpty(name))
			{
				xmlFragment.WriteElementString("DisplayName", this.ResponseNs, name);
			}
			xmlFragment.WriteElementString("EMailAddress", this.ResponseNs, emailAddress);
			xmlFragment.WriteEndElement();
		}

		private void WriteServerFragment(XmlWriter xmlFragment, string mobileName, string certUrl, string certTemplate, bool certEnabled)
		{
			xmlFragment.WriteStartElement("Server", this.ResponseNs);
			xmlFragment.WriteElementString("Type", this.ResponseNs, "MobileSync");
			xmlFragment.WriteElementString("Url", this.ResponseNs, mobileName);
			xmlFragment.WriteElementString("Name", this.ResponseNs, mobileName);
			xmlFragment.WriteEndElement();
			if (certEnabled)
			{
				xmlFragment.WriteStartElement("Server", this.ResponseNs);
				xmlFragment.WriteElementString("Type", this.ResponseNs, "CertEnroll");
				xmlFragment.WriteElementString("Url", this.ResponseNs, certUrl);
				xmlFragment.WriteElementString("Name", this.ResponseNs, null);
				xmlFragment.WriteElementString("ServerData", this.ResponseNs, certTemplate);
				xmlFragment.WriteEndElement();
			}
		}

		private static readonly string defaultCulture = "en:us";
	}
}
