using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Exchange.Autodiscover.ConfigurationCache;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Autodiscover;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover.Providers.Outlook
{
	[Provider("6404ffe9-1b2e-4baf-832d-58558859913a", "http://schemas.microsoft.com/exchange/autodiscover/outlook/requestschema/2006", "OutlookRequestSchema2006.xsd", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "OutlookResponseSchema2006a.xsd", Description = "Outlook 12 provider for Exchange Autodiscover Service")]
	internal class OutlookAutoDiscoverProvider : Provider
	{
		private static void SimpleRemoveXmlDeclaration(ref string xmlString)
		{
			Match match = OutlookAutoDiscoverProvider.xmlDeclarationRegEx.Match(xmlString);
			if (match.Success && match.Index == 0)
			{
				xmlString = xmlString.Substring(match.Length);
			}
		}

		static OutlookAutoDiscoverProvider()
		{
			string text = ConfigurationManager.AppSettings["ActiveManagerCacheExpirationIntervalSecs"];
			int num;
			if (!string.IsNullOrEmpty(text) && int.TryParse(text, out num))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<int>(0L, "Setting an ActiveManager timeout of {0} seconds", num);
				TimeSpan cacheUpdateInterval = new TimeSpan(0, 0, num);
				ActiveManager.GetCachingActiveManagerInstance().SetCacheUpdateInterval(cacheUpdateInterval);
			}
		}

		public OutlookAutoDiscoverProvider(RequestData requestData) : base(requestData)
		{
			bool flag = AutodiscoverProxy.IsRedirectFaultInjectionEnabledOnRequest(requestData.CallerCapabilities.CanFollowRedirect);
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.RedirectOutlookClient.Enabled && base.Caller == null)
			{
				flag = true;
			}
			if (flag)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string>(0L, "[OutlookAutoDiscoverProvider()] Autodiscover caller \"{0}\" was not found in AD.", Common.GetIdentityNameForTrace(requestData.User.Identity));
				this.redirectPod = requestData.RedirectPod;
				this.requestedEmailAddress = requestData.EMailAddress;
			}
			else
			{
				base.ResolveRequestedADRecipient();
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("RequestedRecipient", (base.RequestedRecipient == null) ? "null" : base.RequestedRecipient.Guid.ToString());
			}
			this.outlookProviderCollection = ServerConfigurationCache.Singleton.OutlookProviderCache.Providers();
			this.clientMapiHttpResponseVersion = requestData.MapiHttpVersion;
			if (base.RequestedRecipient != null)
			{
				if (string.IsNullOrEmpty(requestData.EMailAddress))
				{
					requestData.EMailAddress = base.RequestedRecipient.PrimarySmtpAddress.ToString();
				}
				this.settingsProvider = new UserSettingsProvider(base.RequestedRecipient, requestData.EMailAddress, requestData.CallerCapabilities, requestData.UserAuthType, requestData.UseClientCertificateAuthentication, null, this.clientMapiHttpResponseVersion > 0);
			}
		}

		public override string Get302RedirectUrl()
		{
			string result = null;
			if (base.RequestedRecipient != null && (base.RequestedRecipient.RecipientType == RecipientType.UserMailbox || AutodiscoverCommonUserSettings.IsArchiveMailUser(base.RequestedRecipient)))
			{
				UserConfigurationSettings currentUserConfigurationSettings = this.GetCurrentUserConfigurationSettings();
				result = currentUserConfigurationSettings.GetString(UserConfigurationSettingName.RedirectUrl);
			}
			return result;
		}

		protected override bool WriteRedirectXml(XmlWriter xmlFragment)
		{
			bool result;
			if (base.RequestedRecipient == null || AutodiscoverProxy.IsRedirectFaultInjectionEnabledOnRequest(base.RequestData.CallerCapabilities.CanFollowRedirect))
			{
				SmtpProxyAddress smtpProxyAddress;
				if (!string.IsNullOrEmpty(this.requestedEmailAddress) && !string.IsNullOrEmpty(this.redirectPod) && SmtpProxyAddress.TryEncapsulate(ProxyAddressPrefix.Smtp.ToString(), this.requestedEmailAddress, this.redirectPod, out smtpProxyAddress))
				{
					this.proxyResponse = OutlookAutoDiscoverProvider.autodiscoverProxy.ProxyingAutodiscoverRequestIfApplicable(base.RequestData.ProxyRequestData, this.redirectPod);
					if (this.proxyResponse == null)
					{
						Common.StartEnvelope(xmlFragment);
						xmlFragment.WriteStartElement("Response", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
						xmlFragment.WriteStartElement("Account", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
						xmlFragment.WriteElementString("Action", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "redirectAddr");
						xmlFragment.WriteElementString("RedirectAddr", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", smtpProxyAddress.AddressString);
						xmlFragment.WriteEndElement();
						xmlFragment.WriteEndElement();
						RequestDetailsLoggerBase<RequestDetailsLogger>.Current.SetRedirectionType(RedirectionType.EmailAddressRedirect);
						Common.EndEnvelope(xmlFragment);
						string text = string.IsNullOrEmpty(smtpProxyAddress.AddressString) ? "(null)" : smtpProxyAddress.AddressString;
						Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_InfoProvRedirectionResponse, Common.PeriodicKey, new object[]
						{
							text,
							text,
							"(null)",
							base.GetType().AssemblyQualifiedName
						});
					}
					else
					{
						OutlookAutoDiscoverProvider.SimpleRemoveXmlDeclaration(ref this.proxyResponse);
						xmlFragment.WriteRaw(this.proxyResponse);
						this.proxyResponse = null;
					}
				}
				else
				{
					if (base.RequestData != null)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("RequestedUser", base.RequestData.EMailAddress);
					}
					Common.GenerateErrorResponseDontLog(xmlFragment, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "500", Strings.OutlookAddressNotFound.ToString(), string.Empty, base.RequestData, base.GetType().AssemblyQualifiedName);
				}
				result = true;
			}
			else if (AutodiscoverCommonUserSettings.IsArchiveMailUser(base.RequestedRecipient))
			{
				result = false;
			}
			else if (base.RequestedRecipient.RecipientType == RecipientType.MailUser || base.RequestedRecipient.RecipientType == RecipientType.MailContact)
			{
				result = true;
				ProxyAddress proxyAddress = base.RequestedRecipient.ExternalEmailAddress;
				if (Common.IsCustomEmailRedirectEnabled())
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("CustomRedirEnabled", "1");
					string customAttribute = base.RequestedRecipient.CustomAttribute15;
					if (customAttribute != null && customAttribute.Length > 0)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("CustomRedirEmail", customAttribute);
						ProxyAddress proxyAddress2 = null;
						if (ProxyAddress.TryParse("SMTP:" + customAttribute, out proxyAddress2))
						{
							proxyAddress = proxyAddress2;
						}
					}
				}
				if (proxyAddress == null || string.IsNullOrEmpty(proxyAddress.PrefixString) || string.Compare(proxyAddress.PrefixString, "SMTP", StringComparison.OrdinalIgnoreCase) != 0)
				{
					Common.GenerateErrorResponse(xmlFragment, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "602", Strings.OutlookBadAddress.ToString(), base.RequestedRecipient.RecipientType.ToString(), base.RequestData, base.GetType().AssemblyQualifiedName);
				}
				else
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("RedirectReason", "MailUser or MailContact");
					Common.StartEnvelope(xmlFragment);
					xmlFragment.WriteStartElement("Response", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
					xmlFragment.WriteStartElement("Account", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
					xmlFragment.WriteElementString("Action", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "redirectAddr");
					xmlFragment.WriteElementString("RedirectAddr", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", proxyAddress.AddressString);
					xmlFragment.WriteEndElement();
					xmlFragment.WriteEndElement();
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.SetRedirectionType(RedirectionType.EmailAddressRedirect);
					Common.EndEnvelope(xmlFragment);
					string text2 = string.IsNullOrEmpty(proxyAddress.AddressString) ? "(null)" : proxyAddress.AddressString;
					string text3 = string.IsNullOrEmpty(base.RequestedRecipient.LegacyExchangeDN) ? "(null)" : base.RequestedRecipient.LegacyExchangeDN;
					Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_InfoProvRedirectionResponse, Common.PeriodicKey, new object[]
					{
						text2,
						text2,
						text3,
						base.GetType().AssemblyQualifiedName
					});
				}
			}
			else if (base.RequestedRecipient.RecipientTypeDetails == RecipientTypeDetails.RemoteGroupMailbox)
			{
				result = true;
				ProxyAddress rawExternalEmailAddress = base.RequestedRecipient.RawExternalEmailAddress;
				if (rawExternalEmailAddress == null || !SmtpAddress.IsValidSmtpAddress(rawExternalEmailAddress.AddressString))
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("ErrorReason", "RemoteGroupMailbox with empty or bad TargetAddress");
					Common.GenerateErrorResponse(xmlFragment, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "602", Strings.OutlookInvalidEMailAddress.ToString(), base.RequestedRecipient.RecipientType.ToString(), base.RequestData, base.GetType().AssemblyQualifiedName);
				}
				else if (string.Compare(rawExternalEmailAddress.AddressString, base.RequestedRecipient.PrimarySmtpAddress.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("ErrorReason", "RemoteGroupMailbox TargetAddress same as Primary SMTP");
					Common.GenerateErrorResponse(xmlFragment, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "602", Strings.OutlookBadAddress.ToString(), base.RequestedRecipient.RecipientType.ToString(), base.RequestData, base.GetType().AssemblyQualifiedName);
				}
				else
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("RedirectReason", "RemoteGroupMailbox");
					this.WriteClientRedirection(xmlFragment, rawExternalEmailAddress.AddressString);
				}
			}
			else if (base.RequestedRecipient.RecipientType == RecipientType.UserMailbox)
			{
				result = false;
			}
			else
			{
				Common.GenerateErrorResponse(xmlFragment, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "501", Strings.OutlookBadAddress.ToString(), base.RequestedRecipient.RecipientType.ToString(), base.RequestData, base.GetType().AssemblyQualifiedName);
				result = true;
			}
			return result;
		}

		protected override void WriteConfigXml(XmlWriter xmlFragment)
		{
			if (base.RequestedRecipient == null)
			{
				Common.GenerateErrorResponseDontLog(xmlFragment, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "500", Strings.OutlookAddressNotFound.ToString(), string.Empty, base.RequestData, base.GetType().AssemblyQualifiedName);
				return;
			}
			if (base.RequestedRecipient.RecipientType != RecipientType.UserMailbox && !AutodiscoverCommonUserSettings.IsArchiveMailUser(base.RequestedRecipient))
			{
				Common.GenerateErrorResponse(xmlFragment, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "501", Strings.OutlookBadAddress.ToString(), base.RequestedRecipient.RecipientType.ToString(), base.RequestData, base.GetType().AssemblyQualifiedName);
				return;
			}
			base.RequestData.Budget.CheckOverBudget();
			UserConfigurationSettings currentUserConfigurationSettings = this.GetCurrentUserConfigurationSettings();
			ADUser adUser = base.RequestedRecipient as ADUser;
			ExchangePrincipal exchangePrincipal = Common.GetExchangePrincipal(adUser);
			VariantConfigurationSnapshot configuration = exchangePrincipal.GetConfiguration();
			if (string.IsNullOrEmpty(exchangePrincipal.MailboxInfo.Location.ServerFqdn) || exchangePrincipal.MailboxInfo.Location.ServerVersion < Server.E2007MinVersion)
			{
				Common.StartEnvelope(xmlFragment);
				xmlFragment.WriteStartElement("Response", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
				xmlFragment.WriteStartElement("Error", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
				xmlFragment.WriteAttributeString("Time", base.RequestData.Timestamp);
				xmlFragment.WriteAttributeString("Id", base.RequestData.ComputerNameHash);
				if (string.IsNullOrEmpty(exchangePrincipal.MailboxInfo.Location.ServerFqdn))
				{
					xmlFragment.WriteElementString("ErrorCode", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "505");
					xmlFragment.WriteElementString("DebugData", "mailbox server not found");
				}
				else
				{
					xmlFragment.WriteElementString("ErrorCode", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "503");
					xmlFragment.WriteElementString("Message", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", Strings.OutlookServerTooOld.ToString());
					xmlFragment.WriteElementString("DebugData", string.Empty);
				}
				xmlFragment.WriteStartElement("Account", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
				this.WriteAbbreviatedProtocol(xmlFragment, ClientAccessType.Internal);
				this.WriteAbbreviatedProtocol(xmlFragment, ClientAccessType.External);
				xmlFragment.WriteEndElement();
				xmlFragment.WriteEndElement();
				xmlFragment.WriteEndElement();
				Common.EndEnvelope(xmlFragment);
				return;
			}
			bool flag = exchangePrincipal.MailboxInfo.Location.ServerVersion >= Server.E14MinVersion;
			Common.StartEnvelope(xmlFragment);
			string text;
			if ((text = currentUserConfigurationSettings.GetString(UserConfigurationSettingName.UserDisplayName)) == null)
			{
				text = (base.RequestedRecipient.DisplayName ?? string.Empty);
			}
			string value3 = text;
			string value2 = currentUserConfigurationSettings.GetString(UserConfigurationSettingName.UserDN) ?? base.RequestedRecipient.LegacyExchangeDN;
			xmlFragment.WriteStartElement("Response", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
			xmlFragment.WriteStartElement("User", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
			xmlFragment.WriteElementString("DisplayName", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", value3);
			xmlFragment.WriteElementString("LegacyDN", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", value2);
			xmlFragment.WriteElementString("AutoDiscoverSMTPAddress", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", currentUserConfigurationSettings.GetString(UserConfigurationSettingName.AutoDiscoverSMTPAddress));
			if (!currentUserConfigurationSettings.Get<bool>(UserConfigurationSettingName.ShowGalAsDefaultView))
			{
				xmlFragment.WriteElementString("DefaultABView", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "contacts");
			}
			xmlFragment.WriteElementString("DeploymentId", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", currentUserConfigurationSettings.GetString(UserConfigurationSettingName.UserDeploymentId));
			xmlFragment.WriteEndElement();
			xmlFragment.WriteStartElement("Account", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
			xmlFragment.WriteElementString("AccountType", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "email");
			xmlFragment.WriteElementString("Action", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "settings");
			xmlFragment.WriteElementString("MicrosoftOnline", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", currentUserConfigurationSettings.GetString(UserConfigurationSettingName.UserMSOnline));
			this.WriteClientVersionRequirements(xmlFragment);
			ClientAccessModes clientAccessModes = this.settingsProvider.GetClientAccessModes();
			OutlookProvider internalOutlookProvider = null;
			OutlookProvider externalOutlookProvider = null;
			MapiHttpProvider mapiHttpProvider = new MapiHttpProvider(delegate(string key, object value)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo(key, value);
			});
			Version minimumMapiHttpAutodiscoverVersion;
			if (ServerConfigurationCache.Singleton.EnableMapiHttpAutodiscover != null && ServerConfigurationCache.Singleton.EnableMapiHttpAutodiscover.Value)
			{
				minimumMapiHttpAutodiscoverVersion = ServerConfigurationCache.Singleton.MinimumMapiHttpAutodiscoverVersion;
			}
			else
			{
				minimumMapiHttpAutodiscoverVersion = MapiHttpProvider.MinimumMapiHttpServerVersion;
			}
			bool flag2 = mapiHttpProvider.ShouldWriteMapiHttpProtocolNode(currentUserConfigurationSettings, this.clientMapiHttpResponseVersion, ServerConfigurationCache.Singleton.EnableMapiHttpAutodiscover, minimumMapiHttpAutodiscoverVersion, configuration.Autodiscover.UseMapiHttpADSetting.Enabled, configuration.Autodiscover.MapiHttp.Enabled, configuration.Autodiscover.MapiHttpForOutlook14.Enabled, this.settingsProvider.CallerCapabilities.OutlookClientVersion);
			if (flag2)
			{
				flag2 = mapiHttpProvider.TryWriteMapiHttpNodes(currentUserConfigurationSettings, xmlFragment, this.clientMapiHttpResponseVersion, clientAccessModes);
			}
			foreach (OutlookProvider outlookProvider in this.outlookProviderCollection)
			{
				switch (this.GetProtocolType(outlookProvider))
				{
				case ProtocolType.Internal:
					if (!flag2 && (clientAccessModes & ClientAccessModes.InternalAccess) != ClientAccessModes.None)
					{
						this.WriteProtocolXML(outlookProvider, exchangePrincipal, currentUserConfigurationSettings, xmlFragment);
						internalOutlookProvider = outlookProvider;
					}
					break;
				case ProtocolType.External:
					if (!flag2 && (clientAccessModes & ClientAccessModes.ExternalAccess) != ClientAccessModes.None)
					{
						this.WriteProtocolXML(outlookProvider, exchangePrincipal, currentUserConfigurationSettings, xmlFragment);
						externalOutlookProvider = outlookProvider;
					}
					break;
				case ProtocolType.Web:
					this.WriteWebProtocolXML(currentUserConfigurationSettings, xmlFragment);
					break;
				case ProtocolType.Custom:
					this.WriteProtocolXML(outlookProvider, exchangePrincipal, currentUserConfigurationSettings, xmlFragment);
					break;
				}
			}
			if (flag)
			{
				this.WriteExHttpNodes(xmlFragment, currentUserConfigurationSettings, clientAccessModes, internalOutlookProvider, externalOutlookProvider);
			}
			AlternateMailboxCollection alternateMailboxCollection = currentUserConfigurationSettings.Get<AlternateMailboxCollection>(UserConfigurationSettingName.AlternateMailboxes);
			if (alternateMailboxCollection != null)
			{
				foreach (Microsoft.Exchange.Autodiscover.ConfigurationSettings.AlternateMailbox alternateMailbox in alternateMailboxCollection)
				{
					xmlFragment.WriteStartElement("AlternativeMailbox", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
					xmlFragment.WriteElementString("Type", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", alternateMailbox.Type);
					xmlFragment.WriteElementString("DisplayName", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", alternateMailbox.DisplayName);
					if (!string.IsNullOrEmpty(alternateMailbox.SmtpAddress))
					{
						xmlFragment.WriteElementString("SmtpAddress", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", alternateMailbox.SmtpAddress);
					}
					else
					{
						xmlFragment.WriteElementString("LegacyDN", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", alternateMailbox.LegacyDN);
						if (!string.IsNullOrEmpty(alternateMailbox.Server))
						{
							xmlFragment.WriteElementString("Server", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", alternateMailbox.Server);
						}
					}
					xmlFragment.WriteElementString("OwnerSmtpAddress", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", alternateMailbox.OwnerSmtpAddress);
					xmlFragment.WriteEndElement();
				}
			}
			string @string = currentUserConfigurationSettings.GetString(UserConfigurationSettingName.PublicFolderInformation);
			if (@string != null)
			{
				xmlFragment.WriteStartElement("PublicFolderInformation", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
				xmlFragment.WriteElementString("SmtpAddress", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", @string);
				xmlFragment.WriteEndElement();
			}
			xmlFragment.WriteEndElement();
			xmlFragment.WriteEndElement();
			Common.EndEnvelope(xmlFragment);
			Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_InfoProvConfigurationResponse, Common.PeriodicKey, new object[]
			{
				base.RequestData.LegacyDN ?? base.RequestData.EMailAddress
			});
		}

		private void WriteClientRedirection(XmlWriter xmlFragment, string forwardingAddress)
		{
			Common.StartEnvelope(xmlFragment);
			xmlFragment.WriteStartElement("Response", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
			xmlFragment.WriteStartElement("Account", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
			xmlFragment.WriteElementString("Action", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "redirectAddr");
			xmlFragment.WriteElementString("RedirectAddr", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", forwardingAddress);
			xmlFragment.WriteEndElement();
			xmlFragment.WriteEndElement();
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.SetRedirectionType(RedirectionType.EmailAddressRedirect);
			Common.EndEnvelope(xmlFragment);
		}

		private void WriteClientVersionRequirements(XmlWriter xmlFragment)
		{
			ClientVersion requiredClientVersion = this.GetRequiredClientVersion();
			if (requiredClientVersion != null)
			{
				xmlFragment.WriteStartElement("OutlookClientVersion", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
				xmlFragment.WriteElementString("MinRequiredVersion", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", requiredClientVersion.VersionString);
				xmlFragment.WriteElementString("ExpiryDate", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", requiredClientVersion.ExpirationDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));
				xmlFragment.WriteEndElement();
			}
		}

		private ClientVersion GetRequiredClientVersion()
		{
			ClientVersion result = null;
			OutlookProvider outlookProvider = null;
			foreach (OutlookProvider outlookProvider2 in this.outlookProviderCollection)
			{
				if (this.GetProtocolType(outlookProvider2) == ProtocolType.Internal)
				{
					outlookProvider = outlookProvider2;
					break;
				}
			}
			if (outlookProvider != null)
			{
				ClientVersionCollection requiredClientVersionCollection = outlookProvider.GetRequiredClientVersionCollection();
				if (requiredClientVersionCollection != null && this.settingsProvider.CallerCapabilities.OutlookClientVersion != null)
				{
					result = requiredClientVersionCollection.GetRequiredClientVersion(this.settingsProvider.CallerCapabilities.OutlookClientVersion);
				}
			}
			return result;
		}

		private void GetSettingsForProtocol(ProtocolType protocolType, UserConfigurationSettings settings, out string ewsUrl, out string umUrl, out string ecpUrl, out string oabUrl, out string serverExclusiveConnect)
		{
			switch (protocolType)
			{
			case ProtocolType.Internal:
				ewsUrl = settings.GetString(UserConfigurationSettingName.InternalEwsUrl);
				umUrl = settings.GetString(UserConfigurationSettingName.InternalUMUrl);
				ecpUrl = settings.GetString(UserConfigurationSettingName.InternalEcpUrl);
				oabUrl = settings.GetString(UserConfigurationSettingName.InternalOABUrl);
				serverExclusiveConnect = settings.GetString(UserConfigurationSettingName.InternalServerExclusiveConnect);
				return;
			case ProtocolType.External:
				ewsUrl = settings.GetString(UserConfigurationSettingName.ExternalEwsUrl);
				umUrl = settings.GetString(UserConfigurationSettingName.ExternalUMUrl);
				ecpUrl = settings.GetString(UserConfigurationSettingName.ExternalEcpUrl);
				oabUrl = settings.GetString(UserConfigurationSettingName.ExternalOABUrl);
				serverExclusiveConnect = settings.GetString(UserConfigurationSettingName.ExternalServerExclusiveConnect);
				return;
			default:
				ewsUrl = null;
				umUrl = null;
				ecpUrl = null;
				oabUrl = null;
				serverExclusiveConnect = null;
				return;
			}
		}

		private Dictionary<UserConfigurationSettingName, string> GetExHttpSettings(UserConfigurationSettings settings, ClientAccessModes modes)
		{
			Dictionary<UserConfigurationSettingName, string> dictionary = null;
			Dictionary<UserConfigurationSettingName, string> dictionary2 = null;
			if ((modes & ClientAccessModes.InternalAccess) == ClientAccessModes.InternalAccess)
			{
				dictionary = this.GetAvailableUrls(OutlookAutoDiscoverProvider.InternalExHttpSettingNames, settings);
			}
			if ((modes & ClientAccessModes.ExternalAccess) == ClientAccessModes.ExternalAccess)
			{
				dictionary2 = this.GetAvailableUrls(OutlookAutoDiscoverProvider.ExternalExHttpSettingNames, settings);
			}
			if (dictionary2 != null && dictionary != null)
			{
				return dictionary.Union(dictionary2).ToDictionary((KeyValuePair<UserConfigurationSettingName, string> url) => url.Key, (KeyValuePair<UserConfigurationSettingName, string> url) => url.Value);
			}
			if (dictionary2 != null)
			{
				return dictionary2;
			}
			return dictionary;
		}

		private string GetAndRemoveUrl(Dictionary<UserConfigurationSettingName, string> exHttpSettings, UserConfigurationSettingName internalUrlName, UserConfigurationSettingName externalUrlName)
		{
			string result = null;
			if (exHttpSettings.TryGetValue(internalUrlName, out result))
			{
				exHttpSettings.Remove(internalUrlName);
			}
			else if (exHttpSettings.TryGetValue(externalUrlName, out result))
			{
				exHttpSettings.Remove(externalUrlName);
			}
			return result;
		}

		private Dictionary<UserConfigurationSettingName, string> GetAvailableUrls(UserConfigurationSettingName[] urlNames, UserConfigurationSettings settings)
		{
			return (from settingsName in urlNames
			where !string.IsNullOrEmpty(settings.GetString(settingsName))
			select settingsName).ToDictionary((UserConfigurationSettingName key) => key, new Func<UserConfigurationSettingName, string>(settings.GetString));
		}

		private void WriteAbbreviatedProtocol(XmlWriter xmlFragment, ClientAccessType clientAccess)
		{
			string text = null;
			if (Common.SkipServiceTopologyInDatacenter(null))
			{
				Uri datacenterFrontEndWebServicesUrl = FrontEndLocator.GetDatacenterFrontEndWebServicesUrl();
				if (datacenterFrontEndWebServicesUrl != null)
				{
					text = datacenterFrontEndWebServicesUrl.ToString();
				}
			}
			else
			{
				WebServicesService wss = null;
				ServiceTopology st = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\autodisc\\src\\Providers\\Outlook\\Web\\OutlookProvider.cs", "WriteAbbreviatedProtocol", 998);
				st.ForEach<WebServicesService>(delegate(WebServicesService service)
				{
					if (wss == null && st.IsServiceOnCurrentServer<WebServicesService>(service, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\Providers\\Outlook\\Web\\OutlookProvider.cs", "WriteAbbreviatedProtocol", 1002) && service.ClientAccessType == clientAccess)
					{
						wss = service;
					}
				}, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\Providers\\Outlook\\Web\\OutlookProvider.cs", "WriteAbbreviatedProtocol", 999);
				if (wss != null)
				{
					text = wss.Url.ToString();
				}
			}
			if (text != null)
			{
				xmlFragment.WriteStartElement("Protocol", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
				xmlFragment.WriteElementString("Type", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", (clientAccess == ClientAccessType.Internal) ? "EXCH" : "EXPR");
				xmlFragment.WriteElementString("ASUrl", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", text);
				xmlFragment.WriteEndElement();
			}
		}

		private void WriteWebProtocolXML(UserConfigurationSettings settings, XmlWriter xmlFragment)
		{
			xmlFragment.WriteStartElement("Protocol", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
			xmlFragment.WriteElementString("Type", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "WEB");
			this.WriteWebProtocolAccessTypeXML(ClientAccessType.Internal, settings.Get<OwaUrlCollection>(UserConfigurationSettingName.InternalWebClientUrls), settings.GetString(UserConfigurationSettingName.InternalEwsUrl), xmlFragment);
			this.WriteWebProtocolAccessTypeXML(ClientAccessType.External, settings.Get<OwaUrlCollection>(UserConfigurationSettingName.ExternalWebClientUrls), settings.GetString(UserConfigurationSettingName.ExternalEwsUrl), xmlFragment);
			xmlFragment.WriteEndElement();
		}

		private void WriteWebProtocolAccessTypeXML(ClientAccessType caType, OwaUrlCollection owaUrls, string ewsUrl, XmlWriter xmlFragment)
		{
			if (owaUrls != null && owaUrls.Count > 0)
			{
				xmlFragment.WriteStartElement(caType.ToString(), "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
				foreach (OwaUrl owaUrl in owaUrls)
				{
					xmlFragment.WriteStartElement("OWAUrl", null);
					xmlFragment.WriteAttributeString("AuthenticationMethod", null, owaUrl.AuthenticationMethods);
					xmlFragment.WriteValue(owaUrl.Url);
					xmlFragment.WriteEndElement();
				}
				if (!string.IsNullOrEmpty(ewsUrl))
				{
					xmlFragment.WriteStartElement("Protocol", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
					xmlFragment.WriteElementString("Type", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", (caType == ClientAccessType.Internal) ? "EXCH" : "EXPR");
					xmlFragment.WriteElementString("ASUrl", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", ewsUrl);
					xmlFragment.WriteEndElement();
				}
				xmlFragment.WriteEndElement();
			}
		}

		private void WriteCommonProtocolXml(XmlWriter xmlFragment, UserConfigurationSettings settings, ProtocolType protocolType, string authPackageText, string sslText, string ewsUrl, string umWSUrl, string ecpUrl, string oabUrl, string serverExclusiveConnect, string certPrincipalName, int? timeToLive)
		{
			if (!string.IsNullOrEmpty(sslText))
			{
				xmlFragment.WriteElementString("SSL", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", sslText);
			}
			if (!string.IsNullOrEmpty(authPackageText))
			{
				if (this.settingsProvider.UseClientCertificateAuthentication)
				{
					authPackageText = "Certificate";
				}
				xmlFragment.WriteElementString("AuthPackage", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", authPackageText);
			}
			if (timeToLive != null)
			{
				xmlFragment.WriteElementString("TTL", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", timeToLive.Value.ToString());
			}
			if (!string.IsNullOrEmpty(ewsUrl))
			{
				this.WriteServiceEndpoint(protocolType, xmlFragment, "ASUrl", ewsUrl);
				xmlFragment.WriteElementString("EwsUrl", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", ewsUrl);
				xmlFragment.WriteElementString("EmwsUrl", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", ewsUrl);
				string @string = settings.GetString(UserConfigurationSettingName.CrossOrganizationSharingEnabled);
				if (!string.IsNullOrEmpty(@string) && @string.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
				{
					this.WriteServiceEndpoint(protocolType, xmlFragment, "SharingUrl", ewsUrl);
				}
			}
			if (ecpUrl != null)
			{
				xmlFragment.WriteElementString("EcpUrl", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", ecpUrl);
				xmlFragment.WriteElementString("EcpUrl-um", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", settings.GetString(UserConfigurationSettingName.EcpVoicemailUrlFragment));
				xmlFragment.WriteElementString("EcpUrl-aggr", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", settings.GetString(UserConfigurationSettingName.EcpEmailSubscriptionsUrlFragment));
				xmlFragment.WriteElementString("EcpUrl-mt", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", settings.GetString(UserConfigurationSettingName.EcpDeliveryReportUrlFragment));
				xmlFragment.WriteElementString("EcpUrl-ret", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", settings.GetString(UserConfigurationSettingName.EcpRetentionPolicyTagsUrlFragment));
				string string2 = settings.GetString(UserConfigurationSettingName.EcpTextMessagingUrlFragment);
				if (!string.IsNullOrEmpty(string2))
				{
					xmlFragment.WriteElementString("EcpUrl-sms", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string2);
				}
				string string3 = settings.GetString(UserConfigurationSettingName.EcpPublishingUrlFragment);
				if (!string.IsNullOrEmpty(string3))
				{
					xmlFragment.WriteElementString("EcpUrl-publish", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string3);
				}
				string string4 = settings.GetString(UserConfigurationSettingName.EcpPhotoUrlFragment);
				if (!string.IsNullOrEmpty(string4))
				{
					xmlFragment.WriteElementString("EcpUrl-photo", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string4);
				}
				string string5 = settings.GetString(UserConfigurationSettingName.EcpConnectUrlFragment);
				if (!string.IsNullOrEmpty(string5))
				{
					xmlFragment.WriteElementString("EcpUrl-connect", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string5);
				}
				string string6 = settings.GetString(UserConfigurationSettingName.EcpTeamMailboxUrlFragment);
				string string7 = settings.GetString(UserConfigurationSettingName.EcpTeamMailboxCreatingUrlFragment);
				string string8 = settings.GetString(UserConfigurationSettingName.EcpTeamMailboxEditingUrlFragment);
				if (!string.IsNullOrEmpty(string6) && !string.IsNullOrEmpty(string7) && !string.IsNullOrEmpty(string8))
				{
					xmlFragment.WriteElementString("EcpUrl-tm", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string6);
					xmlFragment.WriteElementString("EcpUrl-tmCreating", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string7);
					xmlFragment.WriteElementString("EcpUrl-tmEditing", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string8);
				}
				string string9 = settings.GetString(UserConfigurationSettingName.SiteMailboxCreationURL);
				if (!string.IsNullOrEmpty(string9))
				{
					xmlFragment.WriteElementString("SiteMailboxCreationURL", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string9);
				}
				string string10 = settings.GetString(UserConfigurationSettingName.EcpExtensionInstallationUrlFragment);
				if (!string.IsNullOrEmpty(string10))
				{
					xmlFragment.WriteElementString("EcpUrl-extinstall", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string10);
				}
			}
			if (!string.IsNullOrEmpty(ewsUrl))
			{
				this.WriteServiceEndpoint(protocolType, xmlFragment, "OOFUrl", ewsUrl);
			}
			if (!string.IsNullOrEmpty(umWSUrl))
			{
				this.WriteServiceEndpoint(protocolType, xmlFragment, "UMUrl", umWSUrl);
			}
			if (!string.IsNullOrEmpty(oabUrl))
			{
				this.WriteServiceEndpoint(protocolType, xmlFragment, "OABUrl", oabUrl);
			}
			if (!string.IsNullOrEmpty(serverExclusiveConnect))
			{
				xmlFragment.WriteElementString("ServerExclusiveConnect", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", serverExclusiveConnect);
			}
			if (string.Compare(sslText, "Off", StringComparison.OrdinalIgnoreCase) == 0)
			{
				certPrincipalName = "None";
			}
			if (!string.IsNullOrEmpty(certPrincipalName))
			{
				xmlFragment.WriteElementString("CertPrincipalName", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", certPrincipalName);
			}
		}

		private void WriteProtocolXML(OutlookProvider opc, ExchangePrincipal exchangePrincipal, UserConfigurationSettings settings, XmlWriter xmlFragment)
		{
			string text = null;
			string umWSUrl = null;
			string ecpUrl = null;
			string oabUrl = null;
			string serverExclusiveConnect = null;
			string certPrincipalName = null;
			string value = null;
			string sslText = null;
			string authPackageText = null;
			ProtocolType protocolType = this.GetProtocolType(opc);
			this.GetSettingsForProtocol(protocolType, settings, out text, out umWSUrl, out ecpUrl, out oabUrl, out serverExclusiveConnect);
			if (!string.IsNullOrEmpty(opc.Server))
			{
				value = opc.Server;
			}
			if (protocolType == ProtocolType.External)
			{
				value = settings.GetString(UserConfigurationSettingName.ExternalMailboxServer);
				if (string.IsNullOrEmpty(value))
				{
					value = settings.GetString(UserConfigurationSettingName.InternalRpcHttpServer);
					if (string.IsNullOrEmpty(value))
					{
						return;
					}
					sslText = settings.GetString(UserConfigurationSettingName.InternalRpcHttpConnectivityRequiresSsl);
					authPackageText = settings.GetString(UserConfigurationSettingName.InternalRpcHttpAuthenticationMethod);
				}
				else
				{
					sslText = settings.GetString(UserConfigurationSettingName.ExternalMailboxServerRequiresSSL);
					authPackageText = settings.GetString(UserConfigurationSettingName.ExternalMailboxServerAuthenticationMethods);
				}
			}
			if (protocolType == ProtocolType.Internal)
			{
				authPackageText = settings.GetString(UserConfigurationSettingName.InternalMailboxServerAuthenticationMethods);
			}
			xmlFragment.WriteStartElement("Protocol", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
			xmlFragment.WriteElementString("Type", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", opc.Name);
			if (protocolType == ProtocolType.Internal)
			{
				string @string = settings.GetString(UserConfigurationSettingName.InternalRpcClientServer);
				if (@string != null)
				{
					xmlFragment.WriteElementString("Server", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", @string);
					xmlFragment.WriteElementString("ServerDN", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", settings.GetString(UserConfigurationSettingName.InternalMailboxServerDN));
					xmlFragment.WriteElementString("ServerVersion", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", exchangePrincipal.MailboxInfo.Location.ServerVersion.ToString("X"));
				}
				string string2 = settings.GetString(UserConfigurationSettingName.MailboxDN);
				if (!string.IsNullOrEmpty(string2))
				{
					xmlFragment.WriteElementString("MdbDN", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string2);
				}
				string string3 = settings.GetString(UserConfigurationSettingName.PublicFolderServer);
				if (!string.IsNullOrEmpty(string3))
				{
					xmlFragment.WriteElementString("PublicFolderServer", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string3);
				}
				if (!VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.NoADLookupForUser.Enabled)
				{
					string string4 = settings.GetString(UserConfigurationSettingName.ActiveDirectoryServer);
					if (!string.IsNullOrEmpty(string4))
					{
						xmlFragment.WriteElementString("AD", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string4);
					}
				}
			}
			else if (!string.IsNullOrEmpty(value))
			{
				xmlFragment.WriteElementString("Server", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", value);
			}
			int? timeToLive = null;
			if (opc.TTL != OutlookAutoDiscoverProvider.defaultConfig.Member.TTL)
			{
				timeToLive = new int?(opc.TTL);
			}
			if (opc.CertPrincipalName != OutlookAutoDiscoverProvider.defaultConfig.Member.CertPrincipalName)
			{
				certPrincipalName = opc.CertPrincipalName;
			}
			this.WriteCommonProtocolXml(xmlFragment, settings, protocolType, authPackageText, sslText, text, umWSUrl, ecpUrl, oabUrl, serverExclusiveConnect, certPrincipalName, timeToLive);
			if (protocolType == ProtocolType.External)
			{
				if (!string.IsNullOrEmpty(text))
				{
					xmlFragment.WriteElementString("EwsPartnerUrl", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", text);
				}
				string string5 = settings.GetString(UserConfigurationSettingName.GroupingInformation);
				if (!string.IsNullOrEmpty(string5))
				{
					xmlFragment.WriteElementString("GroupingInformation", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", string5);
				}
			}
			xmlFragment.WriteEndElement();
		}

		private void WriteExHttpNode(XmlWriter xmlFragment, UserConfigurationSettings settings, Dictionary<UserConfigurationSettingName, string> exHttpSettings, ProtocolType protocolType, string serverName, string authPackageText, string sslText, string certPrincipalName)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				return;
			}
			string andRemoveUrl = this.GetAndRemoveUrl(exHttpSettings, UserConfigurationSettingName.InternalEwsUrl, UserConfigurationSettingName.ExternalEwsUrl);
			string andRemoveUrl2 = this.GetAndRemoveUrl(exHttpSettings, UserConfigurationSettingName.InternalUMUrl, UserConfigurationSettingName.ExternalUMUrl);
			string andRemoveUrl3 = this.GetAndRemoveUrl(exHttpSettings, UserConfigurationSettingName.InternalEcpUrl, UserConfigurationSettingName.ExternalEcpUrl);
			string andRemoveUrl4 = this.GetAndRemoveUrl(exHttpSettings, UserConfigurationSettingName.InternalOABUrl, UserConfigurationSettingName.ExternalOABUrl);
			xmlFragment.WriteStartElement("Protocol", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
			xmlFragment.WriteElementString("Type", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", "EXHTTP");
			xmlFragment.WriteElementString("Server", "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a", serverName);
			this.WriteCommonProtocolXml(xmlFragment, settings, protocolType, authPackageText, sslText, andRemoveUrl, andRemoveUrl2, andRemoveUrl3, andRemoveUrl4, "On", certPrincipalName, null);
			xmlFragment.WriteEndElement();
		}

		private void WriteExHttpNodes(XmlWriter xmlFragment, UserConfigurationSettings settings, ClientAccessModes clientAccessModes, OutlookProvider internalOutlookProvider, OutlookProvider externalOutlookProvider)
		{
			Dictionary<UserConfigurationSettingName, string> exHttpSettings = this.GetExHttpSettings(settings, clientAccessModes);
			if (!this.settingsProvider.CallerCapabilities.CanHandleExHttpNodesInAutoDiscResponse || exHttpSettings == null)
			{
				return;
			}
			if ((clientAccessModes & ClientAccessModes.InternalAccess) == ClientAccessModes.InternalAccess)
			{
				string certPrincipalName = null;
				if (internalOutlookProvider != null && internalOutlookProvider.CertPrincipalName != OutlookAutoDiscoverProvider.defaultConfig.Member.CertPrincipalName)
				{
					certPrincipalName = internalOutlookProvider.CertPrincipalName;
				}
				this.WriteExHttpNode(xmlFragment, settings, exHttpSettings, ProtocolType.Internal, settings.GetString(UserConfigurationSettingName.InternalRpcHttpServer), settings.GetString(UserConfigurationSettingName.InternalRpcHttpAuthenticationMethod), settings.GetString(UserConfigurationSettingName.InternalRpcHttpConnectivityRequiresSsl), certPrincipalName);
			}
			if ((clientAccessModes & ClientAccessModes.ExternalAccess) == ClientAccessModes.ExternalAccess)
			{
				string certPrincipalName2 = null;
				if (externalOutlookProvider != null && externalOutlookProvider.CertPrincipalName != OutlookAutoDiscoverProvider.defaultConfig.Member.CertPrincipalName)
				{
					certPrincipalName2 = externalOutlookProvider.CertPrincipalName;
				}
				this.WriteExHttpNode(xmlFragment, settings, exHttpSettings, ProtocolType.External, settings.GetString(UserConfigurationSettingName.ExternalMailboxServer), settings.GetString(UserConfigurationSettingName.ExternalMailboxServerAuthenticationMethods), settings.GetString(UserConfigurationSettingName.ExternalMailboxServerRequiresSSL), certPrincipalName2);
			}
		}

		private void WriteServiceEndpoint(ProtocolType protocolType, XmlWriter xmlWriter, string name, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Value in an element cannot be null or empty. Value = {0}", value ?? "<Null>");
			}
			xmlWriter.WriteStartElement(name, "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a");
			if ((protocolType == ProtocolType.External || protocolType == ProtocolType.Internal) && this.settingsProvider.UseClientCertificateAuthentication)
			{
				xmlWriter.WriteAttributeString("AuthenticationMethod", null, "Certificate");
			}
			xmlWriter.WriteValue(value);
			xmlWriter.WriteEndElement();
		}

		private ProtocolType GetProtocolType(OutlookProvider opc)
		{
			string a;
			if ((a = opc.Name.ToUpper()) != null)
			{
				if (a == "EXPR")
				{
					return ProtocolType.External;
				}
				if (a == "EXCH")
				{
					return ProtocolType.Internal;
				}
				if (a == "WEB")
				{
					return ProtocolType.Web;
				}
			}
			return ProtocolType.Custom;
		}

		private UserConfigurationSettings GetCurrentUserConfigurationSettings()
		{
			if (this.currentUserSettings == null)
			{
				this.settingsProvider.WaitForCaches();
				this.currentUserSettings = this.settingsProvider.GetUserSettings(OutlookAutoDiscoverProvider.userSettingsToLoad.Member, base.RequestData.Budget);
			}
			return this.currentUserSettings;
		}

		public const string ResponseNs = "http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a";

		private const string InternalAccess = "EXCH";

		private const string ExternalAccess = "EXPR";

		private const string WebAccess = "WEB";

		private const string OnText = "On";

		private const string OffText = "Off";

		private const string NoneText = "None";

		public const string ProtocolElementName = "Protocol";

		public const string TypeElementName = "Type";

		private static readonly Regex xmlDeclarationRegEx = new Regex("<\\?xml [\\w|=|\"|.|\\s|-]+\\?>");

		private static readonly LazyMember<OutlookProvider> defaultConfig = new LazyMember<OutlookProvider>(delegate()
		{
			OutlookProvider outlookProvider = new OutlookProvider();
			outlookProvider.InitializeDefaults();
			return outlookProvider;
		});

		private static LazyMember<HashSet<UserConfigurationSettingName>> userSettingsToLoad = new LazyMember<HashSet<UserConfigurationSettingName>>(delegate()
		{
			UserConfigurationSettingName[] collection = new UserConfigurationSettingName[]
			{
				UserConfigurationSettingName.ActiveDirectoryServer,
				UserConfigurationSettingName.AlternateMailboxes,
				UserConfigurationSettingName.CrossOrganizationSharingEnabled,
				UserConfigurationSettingName.EcpDeliveryReportUrlFragment,
				UserConfigurationSettingName.EcpEmailSubscriptionsUrlFragment,
				UserConfigurationSettingName.EcpRetentionPolicyTagsUrlFragment,
				UserConfigurationSettingName.EcpTextMessagingUrlFragment,
				UserConfigurationSettingName.EcpVoicemailUrlFragment,
				UserConfigurationSettingName.EcpPublishingUrlFragment,
				UserConfigurationSettingName.EcpPhotoUrlFragment,
				UserConfigurationSettingName.EcpConnectUrlFragment,
				UserConfigurationSettingName.EcpTeamMailboxUrlFragment,
				UserConfigurationSettingName.EcpTeamMailboxCreatingUrlFragment,
				UserConfigurationSettingName.EcpTeamMailboxEditingUrlFragment,
				UserConfigurationSettingName.EcpExtensionInstallationUrlFragment,
				UserConfigurationSettingName.ExternalEcpUrl,
				UserConfigurationSettingName.ExternalEwsUrl,
				UserConfigurationSettingName.ExternalMailboxServer,
				UserConfigurationSettingName.ExternalMailboxServerAuthenticationMethods,
				UserConfigurationSettingName.ExternalMailboxServerRequiresSSL,
				UserConfigurationSettingName.ExternalOABUrl,
				UserConfigurationSettingName.ExternalUMUrl,
				UserConfigurationSettingName.ExternalWebClientUrls,
				UserConfigurationSettingName.InternalEcpUrl,
				UserConfigurationSettingName.InternalEwsUrl,
				UserConfigurationSettingName.InternalMailboxServer,
				UserConfigurationSettingName.InternalMailboxServerDN,
				UserConfigurationSettingName.InternalOABUrl,
				UserConfigurationSettingName.InternalUMUrl,
				UserConfigurationSettingName.InternalRpcClientServer,
				UserConfigurationSettingName.InternalRpcHttpAuthenticationMethod,
				UserConfigurationSettingName.InternalRpcHttpConnectivityRequiresSsl,
				UserConfigurationSettingName.InternalRpcHttpServer,
				UserConfigurationSettingName.InternalWebClientUrls,
				UserConfigurationSettingName.MailboxDN,
				UserConfigurationSettingName.PublicFolderServer,
				UserConfigurationSettingName.UserMSOnline,
				UserConfigurationSettingName.UserDeploymentId,
				UserConfigurationSettingName.UserDisplayName,
				UserConfigurationSettingName.UserDN,
				UserConfigurationSettingName.InternalServerExclusiveConnect,
				UserConfigurationSettingName.ExternalServerExclusiveConnect,
				UserConfigurationSettingName.ShowGalAsDefaultView,
				UserConfigurationSettingName.AutoDiscoverSMTPAddress,
				UserConfigurationSettingName.RedirectUrl,
				UserConfigurationSettingName.EwsPartnerUrl,
				UserConfigurationSettingName.PublicFolderInformation,
				UserConfigurationSettingName.InternalMailboxServerAuthenticationMethods,
				UserConfigurationSettingName.SiteMailboxCreationURL,
				UserConfigurationSettingName.GroupingInformation,
				UserConfigurationSettingName.MapiHttpUrls,
				UserConfigurationSettingName.MailboxVersion,
				UserConfigurationSettingName.MapiHttpEnabled,
				UserConfigurationSettingName.MapiHttpEnabledForUser
			};
			return new HashSet<UserConfigurationSettingName>(collection);
		});

		private static readonly UserConfigurationSettingName[] InternalExHttpSettingNames = new UserConfigurationSettingName[]
		{
			UserConfigurationSettingName.InternalEwsUrl,
			UserConfigurationSettingName.InternalUMUrl,
			UserConfigurationSettingName.InternalEcpUrl,
			UserConfigurationSettingName.InternalOABUrl
		};

		private static readonly UserConfigurationSettingName[] ExternalExHttpSettingNames = new UserConfigurationSettingName[]
		{
			UserConfigurationSettingName.ExternalEwsUrl,
			UserConfigurationSettingName.ExternalUMUrl,
			UserConfigurationSettingName.ExternalEcpUrl,
			UserConfigurationSettingName.ExternalOABUrl
		};

		private UserConfigurationSettings currentUserSettings;

		private IEnumerable<OutlookProvider> outlookProviderCollection;

		private UserSettingsProvider settingsProvider;

		private string proxyResponse;

		private string redirectPod;

		private string requestedEmailAddress;

		private static readonly AutodiscoverProxy autodiscoverProxy = new AutodiscoverProxy();

		private int clientMapiHttpResponseVersion;
	}
}
