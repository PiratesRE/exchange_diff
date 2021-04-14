using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Autodiscover.ConfigurationCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Autodiscover;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	internal class UserSettingsProvider
	{
		static UserSettingsProvider()
		{
			UserSettingsProvider.rootOrgSystemConfigSession.ServerTimeout = new TimeSpan?(TimeSpan.FromMinutes(1.0));
			UserSettingsProvider.expensiveRequestCount = 0;
			UserSettingsProvider.casVersionString = UserSettingsProvider.GetCasVersionString();
			UserSettingsProvider.ecpInternalSettings = new Dictionary<UserConfigurationSettingName, string>();
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpUrl, string.Empty);
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpDeliveryReportUrl, "PersonalSettings/DeliveryReport.aspx?rfr=olk&exsvurl=1&IsOWA=<IsOWA>&MsgID=<MsgID>&Mbx=<Mbx>");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpEmailSubscriptionsUrl, "?rfr=olk&p=personalsettings/EmailSubscriptions.slab&exsvurl=1");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpRetentionPolicyTagsUrl, "?rfr=olk&p=organize/retentionpolicytags.slab&exsvurl=1");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpTextMessagingUrl, "?rfr=olk&p=sms/textmessaging.slab&exsvurl=1");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpVoicemailUrl, "?rfr=olk&p=customize/voicemail.aspx&exsvurl=1");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpPublishingUrl, "customize/calendarpublishing.slab?rfr=olk&exsvurl=1&FldID=<FldID>");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpPhotoUrl, "PersonalSettings/EditAccount.aspx?rfr=olk&chgPhoto=1&exsvurl=1");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpConnectUrl, "Connect/Main.aspx?rfr=olk&exsvurl=1&Provider=<Provider>&Action=<Action>");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpTeamMailboxUrl, "?rfr=olk&ftr=TeamMailbox&exsvurl=1");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpTeamMailboxCreatingUrl, "?rfr=olk&ftr=TeamMailboxCreating&SPUrl=<SPUrl>&Title=<Title>&SPTMAppUrl=<SPTMAppUrl>&exsvurl=1");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpTeamMailboxEditingUrl, "?rfr=olk&ftr=TeamMailboxEditing&Id=<Id>&exsvurl=1");
			UserSettingsProvider.ecpInternalSettings.Add(UserConfigurationSettingName.InternalEcpExtensionInstallationUrl, "Extension/InstalledExtensions.slab?rfr=olk&exsvurl=1");
			UserSettingsProvider.ecpExternalSettings = new Dictionary<UserConfigurationSettingName, string>();
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpUrl, string.Empty);
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpDeliveryReportUrl, "PersonalSettings/DeliveryReport.aspx?rfr=olk&exsvurl=1&IsOWA=<IsOWA>&MsgID=<MsgID>&Mbx=<Mbx>");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpEmailSubscriptionsUrl, "?rfr=olk&p=personalsettings/EmailSubscriptions.slab&exsvurl=1");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpRetentionPolicyTagsUrl, "?rfr=olk&p=organize/retentionpolicytags.slab&exsvurl=1");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpTextMessagingUrl, "?rfr=olk&p=sms/textmessaging.slab&exsvurl=1");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpVoicemailUrl, "?rfr=olk&p=customize/voicemail.aspx&exsvurl=1");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpPublishingUrl, "customize/calendarpublishing.slab?rfr=olk&exsvurl=1&FldID=<FldID>");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpPhotoUrl, "PersonalSettings/EditAccount.aspx?rfr=olk&chgPhoto=1&exsvurl=1");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpConnectUrl, "Connect/Main.aspx?rfr=olk&exsvurl=1&Provider=<Provider>&Action=<Action>");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpTeamMailboxUrl, "?rfr=olk&ftr=TeamMailbox&exsvurl=1");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpTeamMailboxCreatingUrl, "?rfr=olk&ftr=TeamMailboxCreating&SPUrl=<SPUrl>&Title=<Title>&SPTMAppUrl=<SPTMAppUrl>&exsvurl=1");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpTeamMailboxEditingUrl, "?rfr=olk&ftr=TeamMailboxEditing&Id=<Id>&exsvurl=1");
			UserSettingsProvider.ecpExternalSettings.Add(UserConfigurationSettingName.ExternalEcpExtensionInstallationUrl, "Extension/InstalledExtensions.slab?rfr=olk&exsvurl=1");
			UserSettingsProvider.extensionAttributes = new Dictionary<string, ADPropertyDefinition>(15, StringComparer.CurrentCultureIgnoreCase);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute1", ADRecipientSchema.CustomAttribute1);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute2", ADRecipientSchema.CustomAttribute2);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute3", ADRecipientSchema.CustomAttribute3);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute4", ADRecipientSchema.CustomAttribute4);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute5", ADRecipientSchema.CustomAttribute5);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute6", ADRecipientSchema.CustomAttribute6);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute7", ADRecipientSchema.CustomAttribute7);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute8", ADRecipientSchema.CustomAttribute8);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute9", ADRecipientSchema.CustomAttribute9);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute10", ADRecipientSchema.CustomAttribute10);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute11", ADRecipientSchema.CustomAttribute11);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute12", ADRecipientSchema.CustomAttribute12);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute13", ADRecipientSchema.CustomAttribute13);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute14", ADRecipientSchema.CustomAttribute14);
			UserSettingsProvider.extensionAttributes.Add("extensionattribute15", ADRecipientSchema.CustomAttribute15);
			UserSettingsProvider.serviceEndpointSettings = new HashSet<UserConfigurationSettingName>
			{
				UserConfigurationSettingName.SPMySiteHostURL
			};
			UserSettingsProvider.serviceEndpointSettingsWithSiteMailboxCreation = new HashSet<UserConfigurationSettingName>(UserSettingsProvider.serviceEndpointSettings);
			UserSettingsProvider.serviceEndpointSettingsWithSiteMailboxCreation.Add(UserConfigurationSettingName.SiteMailboxCreationURL);
		}

		private static string GetCasVersionString()
		{
			FileVersionInfo serverVersion = Common.ServerVersion;
			return string.Format("{0:d}.{1:d2}.{2:d4}.{3:d3}", new object[]
			{
				serverVersion.FileMajorPart,
				serverVersion.FileMinorPart,
				serverVersion.FileBuildPart,
				serverVersion.FilePrivatePart
			});
		}

		public UserSettingsProvider(ADRecipient adRecipient, string emailAddress, CallerRequestedCapabilities callerCapabilties, string userAuthType, bool useClientCertificateAuthentication, ExchangeServerVersion? requestedVersion, bool doesClientUnderstandMapiHttp)
		{
			this.recipient = adRecipient;
			this.emailAddress = emailAddress;
			this.useClientCertificateAuthentication = useClientCertificateAuthentication;
			this.requestedVersion = requestedVersion;
			this.serverConfigCache = ServerConfigurationCache.Singleton;
			this.doesClientUnderstandMapiHttp = doesClientUnderstandMapiHttp;
			this.CallerCapabilities = callerCapabilties;
		}

		public UserConfigurationSettings GetRedirectionOrErrorSettings()
		{
			UserConfigurationSettings userConfigurationSettings = new UserConfigurationSettings();
			bool flag;
			if (Common.DoesEmailAddressReferenceArchive(this.recipient, this.emailAddress))
			{
				flag = false;
			}
			else if (this.recipient.RecipientType == RecipientType.MailUser || this.recipient.RecipientType == RecipientType.MailContact)
			{
				flag = true;
				ProxyAddress externalEmailAddress = this.recipient.ExternalEmailAddress;
				if (externalEmailAddress == null || string.IsNullOrEmpty(externalEmailAddress.PrefixString) || string.Compare(externalEmailAddress.PrefixString, "SMTP", StringComparison.OrdinalIgnoreCase) != 0)
				{
					userConfigurationSettings.ErrorCode = UserConfigurationSettingsErrorCode.InvalidUser;
					userConfigurationSettings.ErrorMessage = string.Format(Strings.InvalidUser, this.emailAddress);
				}
				else
				{
					userConfigurationSettings.ErrorCode = UserConfigurationSettingsErrorCode.RedirectAddress;
					userConfigurationSettings.ErrorMessage = Strings.RedirectAddress;
					userConfigurationSettings.RedirectTarget = externalEmailAddress.AddressString;
				}
			}
			else if (this.recipient.RecipientType == RecipientType.UserMailbox)
			{
				flag = false;
			}
			else
			{
				userConfigurationSettings.ErrorCode = UserConfigurationSettingsErrorCode.InvalidUser;
				userConfigurationSettings.ErrorMessage = string.Format(Strings.InvalidUser, this.emailAddress);
				flag = true;
			}
			if (!flag)
			{
				return null;
			}
			return userConfigurationSettings;
		}

		public UserConfigurationSettings GetUserSettings(HashSet<UserConfigurationSettingName> requestedSettings, IBudget budget)
		{
			UserConfigurationSettings userConfigurationSettings = new UserConfigurationSettings(requestedSettings);
			bool flag = false;
			ADUser caller = HttpContext.Current.Items["CallerRecipient"] as ADUser;
			if (this.recipient == null)
			{
				userConfigurationSettings.ErrorCode = UserConfigurationSettingsErrorCode.InvalidUser;
				userConfigurationSettings.ErrorMessage = string.Format(Strings.InvalidUser, this.emailAddress);
			}
			else
			{
				if (!(flag = UserSettingsProvider.IsArchiveEmailAddress(caller, this.recipient, this.emailAddress)))
				{
					if (this.recipient.RecipientType != RecipientType.UserMailbox)
					{
						goto IL_16F0;
					}
				}
				try
				{
					Interlocked.Increment(ref UserSettingsProvider.expensiveRequestCount);
					ADUser aduser = this.recipient as ADUser;
					if (!this.serverConfigCache.AllCachesAreLoaded)
					{
						this.WaitForCaches();
					}
					MailboxDatabase configFromSourceObject = this.serverConfigCache.MdbCache.GetConfigFromSourceObject(aduser.Database);
					Guid guid = Guid.Empty;
					if (flag)
					{
						guid = aduser.ArchiveGuid;
					}
					else
					{
						Guid empty = Guid.Empty;
						if (AutodiscoverCommonUserSettings.TryGetExchangeGuidFromEmailAddress(this.emailAddress, out empty) && empty.Equals(aduser.ArchiveGuid))
						{
							flag = true;
							guid = empty;
						}
					}
					ExchangePrincipal ep;
					if (!Guid.Empty.Equals(guid))
					{
						ep = Common.GetExchangePrincipal(guid, aduser.OrganizationId);
						RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("ArchiveGuid", guid.ToString());
					}
					else
					{
						ep = Common.GetExchangePrincipal(aduser);
					}
					if (ep == null)
					{
						userConfigurationSettings.ErrorCode = UserConfigurationSettingsErrorCode.InvalidRequest;
						userConfigurationSettings.ErrorMessage = Strings.ActiveDirectoryFailure;
						return userConfigurationSettings;
					}
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("ExchangePrincipal", ep.MailboxInfo.PrimarySmtpAddress.ToString());
					if (ep.MailboxInfo.OrganizationId != null && ep.MailboxInfo.OrganizationId.OrganizationalUnit != null)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("ExchangePrincipalOrg", ep.MailboxInfo.OrganizationId.OrganizationalUnit.Name);
					}
					Site userSite = null;
					Site site = null;
					Site preferredSite = null;
					VariantConfigurationSnapshot configuration = ep.GetConfiguration();
					bool flag2 = Common.SkipServiceTopologyInDatacenter(configuration);
					RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericInfo("SkipST", flag2);
					LegacyDN legacyDN;
					if (flag2)
					{
						Site site2 = new Site(new TopologySite(LocalSiteCache.LocalSite));
						preferredSite = site2;
						userSite = site2;
						legacyDN = LegacyDN.Parse(ep.MailboxInfo.Location.RpcClientAccessServerLegacyDn);
					}
					else
					{
						ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetUserSettings", 455);
						try
						{
							site = currentServiceTopology.GetSite(ep.MailboxInfo.Location.ServerFqdn, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetUserSettings", 460);
							userSite = site;
							if (!configuration.OAB.SkipServiceTopologyDiscovery.Enabled)
							{
								string sourceCafeServer = CafeHelper.GetSourceCafeServer(HttpContext.Current.Request);
								if (!string.IsNullOrEmpty(sourceCafeServer))
								{
									userSite = currentServiceTopology.GetSite(sourceCafeServer, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetUserSettings", 478);
								}
							}
						}
						catch (ServerNotInSiteException arg)
						{
							ExTraceGlobals.FrameworkTracer.TraceError<string, ServerNotInSiteException>((long)this.GetHashCode(), "Failed to find site of server {0}. Exception: {1}", ep.MailboxInfo.Location.ServerFqdn, arg);
						}
						catch (ServerNotFoundException arg2)
						{
							ExTraceGlobals.FrameworkTracer.TraceError<string, ServerNotFoundException>((long)this.GetHashCode(), "Failed to find server {0}. Exception: {1}", ep.MailboxInfo.Location.ServerFqdn, arg2);
						}
						if (ep.MailboxInfo.Location.ServerVersion >= Server.E2007MinVersion)
						{
							if (!this.TryGetPreferredSite(ep, out preferredSite, out legacyDN))
							{
								CorruptDataException ex = new CorruptDataException(new LocalizedString(string.Format(Strings.SettingIsNotAvailable, "PreferredSite")));
								Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrWebException, Common.PeriodicKey, new object[]
								{
									ex.Message,
									ex.StackTrace
								});
								preferredSite = site;
								legacyDN = LegacyDN.Parse(ep.MailboxInfo.Location.RpcClientAccessServerLegacyDn);
							}
						}
						else
						{
							preferredSite = site;
							legacyDN = LegacyDN.Parse(ep.MailboxInfo.Location.RpcClientAccessServerLegacyDn);
						}
					}
					string rpcClientAccessFqdn = this.GetRpcClientAccessFqdn(new ServerId(legacyDN.ToString()));
					bool flag3 = ep.MailboxInfo.Location.ServerVersion < Server.E2007MinVersion;
					if (this.requestedVersion != null && this.requestedVersion.Value >= ExchangeServerVersion.E14 && requestedSettings.Count == 3 && requestedSettings.Contains(UserConfigurationSettingName.ExternalEwsUrl) && requestedSettings.Contains(UserConfigurationSettingName.ExternalEwsVersion) && requestedSettings.Contains(UserConfigurationSettingName.InteropExternalEwsUrl))
					{
						flag3 = false;
					}
					if (string.IsNullOrEmpty(ep.MailboxInfo.Location.ServerFqdn) || flag3)
					{
						userConfigurationSettings.ErrorCode = UserConfigurationSettingsErrorCode.InvalidRequest;
						userConfigurationSettings.ErrorMessage = Strings.OutlookServerTooOld;
						return userConfigurationSettings;
					}
					if (UserSettingsProvider.orgGuidString == null)
					{
						ADObjectId orgContainerId = UserSettingsProvider.rootOrgSystemConfigSession.GetOrgContainerId();
						UserSettingsProvider.orgGuidString = orgContainerId.ObjectGuid.ToString();
					}
					AutodiscoverCommonUserSettings settingsFromRecipient = AutodiscoverCommonUserSettings.GetSettingsFromRecipient(aduser, this.emailAddress);
					string domain = settingsFromRecipient.PrimarySmtpAddress.Domain;
					Guid mailboxGuid = settingsFromRecipient.MailboxGuid;
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.UserDisplayName, settingsFromRecipient.AccountDisplayName, requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.UserDN, settingsFromRecipient.AccountLegacyDn, requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.UserDeploymentId, UserSettingsProvider.orgGuidString, requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.UserMSOnline, UserSettingsProvider.isMSOnline.ToString(), requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ShowGalAsDefaultView, aduser.ShowGalAsDefaultView, requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.AutoDiscoverSMTPAddress, UserSettingsProvider.GetAutoDiscoverSMTPAddress(aduser), requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.CasVersion, UserSettingsProvider.casVersionString, requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EwsSupportedSchemas, "Exchange2007, Exchange2007_SP1, Exchange2010, Exchange2010_SP1, Exchange2010_SP2, Exchange2013, Exchange2013_SP1", requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalMailboxServer, ep.MailboxInfo.Location.ServerFqdn, requestedSettings, userConfigurationSettings);
					if (preferredSite != null)
					{
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.GroupingInformation, preferredSite.Name, requestedSettings, userConfigurationSettings);
					}
					bool flag4 = ep.MailboxInfo.Location.ServerVersion >= Server.E15MinVersion;
					bool flag5 = ep.MailboxInfo.Location.ServerVersion >= Server.E14MinVersion;
					string value;
					string text;
					string text2;
					if (flag4)
					{
						value = ExchangeRpcClientAccess.CreatePersonalizedServerRedirectLegacyDN(legacyDN, mailboxGuid, domain).ToString();
						text = ExchangeRpcClientAccess.CreatePersonalizedServer(mailboxGuid, domain);
						text2 = Database.GetDatabaseLegacyDNFromRcaLegacyDN(ExchangeRpcClientAccess.CreatePersonalizedServerRedirectLegacyDN(legacyDN, mailboxGuid, domain), false).ToString();
					}
					else
					{
						value = legacyDN.ToString();
						text = rpcClientAccessFqdn;
						text2 = Database.GetDatabaseLegacyDNFromRcaLegacyDN(legacyDN, false).ToString();
					}
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalMailboxServerDN, value, requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.CrossOrganizationSharingEnabled, this.IsOrganizationalSharingEnabled(aduser).ToString(), requestedSettings, userConfigurationSettings);
					if (requestedSettings.Contains(UserConfigurationSettingName.MailboxVersion))
					{
						userConfigurationSettings.Add(UserConfigurationSettingName.MailboxVersion, UserSettingsProvider.FormatServerVersion(ep.MailboxInfo.Location.ServerVersion));
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.InternalRpcClientServer) && !string.IsNullOrEmpty(text))
					{
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalRpcClientServer, text, requestedSettings, userConfigurationSettings);
					}
					if (!string.IsNullOrEmpty(aduser.OriginatingServer))
					{
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ActiveDirectoryServer, aduser.OriginatingServer, requestedSettings, userConfigurationSettings);
					}
					if (text2 != null)
					{
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.MailboxDN, text2, requestedSettings, userConfigurationSettings);
					}
					OfflineAddressBookCacheEntry offlineAddressBookCacheEntry = null;
					if (requestedSettings.Contains(UserConfigurationSettingName.InternalOABUrl) || requestedSettings.Contains(UserConfigurationSettingName.ExternalOABUrl))
					{
						offlineAddressBookCacheEntry = this.CalculateOab(aduser, configFromSourceObject);
					}
					Uri datacenterFrontEndWebServicesUrl;
					Uri uri;
					int e15MinVersion;
					if (flag2)
					{
						datacenterFrontEndWebServicesUrl = FrontEndLocator.GetDatacenterFrontEndWebServicesUrl();
						uri = datacenterFrontEndWebServicesUrl;
						e15MinVersion = Server.E15MinVersion;
					}
					else
					{
						this.GetServiceUrls<WebServicesService>(ep, site, this.requestedVersion, out datacenterFrontEndWebServicesUrl, out uri, out e15MinVersion, false);
					}
					Uri uri3;
					int serverVersion;
					if (flag2)
					{
						Uri uri2 = datacenterFrontEndWebServicesUrl;
						uri3 = uri;
						serverVersion = e15MinVersion;
					}
					else
					{
						Uri uri2;
						this.GetServiceUrls<WebServicesService>(ep, site, this.requestedVersion, out uri2, out uri3, out serverVersion, true);
					}
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpDeliveryReportUrlFragment, this.AppendRealmForEcpUrl("PersonalSettings/DeliveryReport.aspx?rfr=olk&exsvurl=1&IsOWA=<IsOWA>&MsgID=<MsgID>&Mbx=<Mbx>", aduser), requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpEmailSubscriptionsUrlFragment, this.AppendRealmForEcpUrl("?rfr=olk&p=personalsettings/EmailSubscriptions.slab&exsvurl=1", aduser), requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpRetentionPolicyTagsUrlFragment, this.AppendRealmForEcpUrl("?rfr=olk&p=organize/retentionpolicytags.slab&exsvurl=1", aduser), requestedSettings, userConfigurationSettings);
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpVoicemailUrlFragment, this.AppendRealmForEcpUrl("?rfr=olk&p=customize/voicemail.aspx&exsvurl=1", aduser), requestedSettings, userConfigurationSettings);
					bool flag6 = ep.MailboxInfo.Configuration.IsMachineToPersonMessagingEnabled && !ep.MailboxInfo.Configuration.IsPersonToPersonMessagingEnabled;
					if (flag6)
					{
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpTextMessagingUrlFragment, this.AppendRealmForEcpUrl("?rfr=olk&p=sms/textmessaging.slab&exsvurl=1", aduser), requestedSettings, userConfigurationSettings);
					}
					bool flag7 = false;
					try
					{
						IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(ep.MailboxInfo.OrganizationId), 793, "GetUserSettings", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs");
						SharingPolicy sharingPolicy = DirectoryHelper.ReadSharingPolicy(ep.MailboxInfo.MailboxGuid, ep.MailboxInfo.IsArchive, tenantOrRootOrgRecipientSession);
						flag7 = (sharingPolicy != null && sharingPolicy.IsAllowedForAnonymousCalendarSharing());
					}
					catch (ObjectNotFoundException)
					{
					}
					if (flag7)
					{
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpPublishingUrlFragment, this.AppendRealmForEcpUrl("customize/calendarpublishing.slab?rfr=olk&exsvurl=1&FldID=<FldID>", aduser), requestedSettings, userConfigurationSettings);
					}
					bool flag8 = this.IsTeamMailboxEnabledConnection(ep);
					if (flag8)
					{
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpTeamMailboxUrlFragment, this.AppendRealmForEcpUrl("?rfr=olk&ftr=TeamMailbox&exsvurl=1", aduser), requestedSettings, userConfigurationSettings);
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpTeamMailboxCreatingUrlFragment, this.AppendRealmForEcpUrl("?rfr=olk&ftr=TeamMailboxCreating&SPUrl=<SPUrl>&Title=<Title>&SPTMAppUrl=<SPTMAppUrl>&exsvurl=1", aduser), requestedSettings, userConfigurationSettings);
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpTeamMailboxEditingUrlFragment, this.AppendRealmForEcpUrl("?rfr=olk&ftr=TeamMailboxEditing&Id=<Id>&exsvurl=1", aduser), requestedSettings, userConfigurationSettings);
					}
					UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpExtensionInstallationUrlFragment, this.AppendRealmForEcpUrl("Extension/InstalledExtensions.slab?rfr=olk&exsvurl=1", aduser), requestedSettings, userConfigurationSettings);
					this.AddPhotoEcpUrlsIfRequested(ep, aduser, requestedSettings, userConfigurationSettings);
					this.AddEcpConnectUrlIfRequested(aduser, requestedSettings, userConfigurationSettings);
					if (UserSettingsProvider.IsEcpSettingRequested(requestedSettings))
					{
						Uri uri4;
						Uri uri5;
						if (flag2)
						{
							uri4 = FrontEndLocator.GetDatacenterFrontEndEcpUrl();
							uri5 = uri4;
						}
						else
						{
							this.GetServiceUrls<EcpService>(ep, site, out uri4, out uri5);
						}
						if (uri4 != null)
						{
							if (!uri4.OriginalString.EndsWith("/"))
							{
								uri4 = new Uri(uri4.OriginalString + "/");
							}
							foreach (UserConfigurationSettingName userConfigurationSettingName in UserSettingsProvider.ecpInternalSettings.Keys)
							{
								if ((flag6 || userConfigurationSettingName != UserConfigurationSettingName.InternalEcpTextMessagingUrl) && (flag7 || userConfigurationSettingName != UserConfigurationSettingName.InternalEcpPublishingUrl) && (userConfigurationSettingName != UserConfigurationSettingName.InternalEcpConnectUrl || VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.EcpInternalExternalUrl.Enabled) && requestedSettings.Contains(userConfigurationSettingName))
								{
									string text3 = uri4 + UserSettingsProvider.ecpInternalSettings[userConfigurationSettingName];
									if (!string.IsNullOrEmpty(UserSettingsProvider.ecpInternalSettings[userConfigurationSettingName]))
									{
										text3 = this.AppendRealmForEcpUrl(text3, aduser);
									}
									userConfigurationSettings.Add(userConfigurationSettingName, text3);
								}
							}
						}
						if (uri5 != null)
						{
							if (!uri5.OriginalString.EndsWith("/"))
							{
								uri5 = new Uri(uri5.OriginalString + "/");
							}
							foreach (UserConfigurationSettingName userConfigurationSettingName2 in UserSettingsProvider.ecpExternalSettings.Keys)
							{
								if ((flag6 || userConfigurationSettingName2 != UserConfigurationSettingName.ExternalEcpTextMessagingUrl) && (flag7 || userConfigurationSettingName2 != UserConfigurationSettingName.ExternalEcpPublishingUrl) && (userConfigurationSettingName2 != UserConfigurationSettingName.ExternalEcpConnectUrl || VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.EcpInternalExternalUrl.Enabled) && requestedSettings.Contains(userConfigurationSettingName2))
								{
									string text4 = uri5 + UserSettingsProvider.ecpExternalSettings[userConfigurationSettingName2];
									if (!string.IsNullOrEmpty(UserSettingsProvider.ecpExternalSettings[userConfigurationSettingName2]))
									{
										text4 = this.AppendRealmForEcpUrl(text4, aduser);
									}
									userConfigurationSettings.Add(userConfigurationSettingName2, text4);
								}
							}
						}
					}
					Uri uri6 = null;
					Uri uri7 = null;
					if (requestedSettings.Contains(UserConfigurationSettingName.InternalUMUrl) || requestedSettings.Contains(UserConfigurationSettingName.ExternalUMUrl))
					{
						this.GetUnifiedMessagingUrls(ep, site, datacenterFrontEndWebServicesUrl, uri, out uri6, out uri7);
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.InternalPop3Connections))
					{
						PopImapSmtpConnectionCollection popImapSmtpConnectionCollection = this.GetPopImapConnectionSettings<Pop3Service>(ep, ClientAccessType.Internal);
						if (popImapSmtpConnectionCollection != null)
						{
							UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalPop3Connections, popImapSmtpConnectionCollection, requestedSettings, userConfigurationSettings);
						}
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.ExternalPop3Connections))
					{
						PopImapSmtpConnectionCollection popImapSmtpConnectionCollection = this.GetPopImapConnectionSettings<Pop3Service>(ep, ClientAccessType.External);
						if (popImapSmtpConnectionCollection != null)
						{
							UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalPop3Connections, popImapSmtpConnectionCollection, requestedSettings, userConfigurationSettings);
						}
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.InternalImap4Connections))
					{
						PopImapSmtpConnectionCollection popImapSmtpConnectionCollection = this.GetPopImapConnectionSettings<Imap4Service>(ep, ClientAccessType.Internal);
						if (popImapSmtpConnectionCollection != null)
						{
							UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalImap4Connections, popImapSmtpConnectionCollection, requestedSettings, userConfigurationSettings);
						}
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.ExternalImap4Connections))
					{
						PopImapSmtpConnectionCollection popImapSmtpConnectionCollection = this.GetPopImapConnectionSettings<Imap4Service>(ep, ClientAccessType.External);
						if (popImapSmtpConnectionCollection != null)
						{
							UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalImap4Connections, popImapSmtpConnectionCollection, requestedSettings, userConfigurationSettings);
						}
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.InternalSmtpConnections))
					{
						PopImapSmtpConnectionCollection popImapSmtpConnectionCollection = this.GetSmtpConnectionSettings(ep, ClientAccessType.Internal);
						if (popImapSmtpConnectionCollection != null)
						{
							UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalSmtpConnections, popImapSmtpConnectionCollection, requestedSettings, userConfigurationSettings);
						}
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.ExternalSmtpConnections))
					{
						PopImapSmtpConnectionCollection popImapSmtpConnectionCollection = this.GetSmtpConnectionSettings(ep, ClientAccessType.External);
						if (popImapSmtpConnectionCollection != null)
						{
							UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalSmtpConnections, popImapSmtpConnectionCollection, requestedSettings, userConfigurationSettings);
						}
					}
					if (flag5)
					{
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalServerExclusiveConnect, "off", requestedSettings, userConfigurationSettings);
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalServerExclusiveConnect, "on", requestedSettings, userConfigurationSettings);
					}
					ClientAccessModes clientAccessModes = this.GetClientAccessModes();
					OutlookProvider configFromSourceObject2 = this.serverConfigCache.OutlookProviderCache.GetConfigFromSourceObject("WEB");
					if (configFromSourceObject2 != null)
					{
						if (requestedSettings.Contains(UserConfigurationSettingName.InternalWebClientUrls))
						{
							OwaUrlCollection owaUrlCollectionForClientAccess = this.GetOwaUrlCollectionForClientAccess(ep, aduser, ClientAccessType.Internal);
							if (owaUrlCollectionForClientAccess != null)
							{
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalWebClientUrls, owaUrlCollectionForClientAccess, requestedSettings, userConfigurationSettings);
							}
						}
						if (requestedSettings.Contains(UserConfigurationSettingName.ExternalWebClientUrls))
						{
							OwaUrlCollection owaUrlCollectionForClientAccess2 = this.GetOwaUrlCollectionForClientAccess(ep, aduser, ClientAccessType.External);
							if (owaUrlCollectionForClientAccess2 != null)
							{
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalWebClientUrls, owaUrlCollectionForClientAccess2, requestedSettings, userConfigurationSettings);
							}
						}
					}
					string value2 = null;
					string text5 = null;
					if ((clientAccessModes & ClientAccessModes.InternalAccess) != ClientAccessModes.None)
					{
						configFromSourceObject2 = this.serverConfigCache.OutlookProviderCache.GetConfigFromSourceObject("EXCH");
						if (configFromSourceObject2 != null)
						{
							if (datacenterFrontEndWebServicesUrl != null)
							{
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalEwsUrl, datacenterFrontEndWebServicesUrl.AbsoluteUri, requestedSettings, userConfigurationSettings);
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalEmwsUrl, datacenterFrontEndWebServicesUrl.AbsoluteUri, requestedSettings, userConfigurationSettings);
							}
							if (uri6 != null)
							{
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalUMUrl, uri6.AbsoluteUri, requestedSettings, userConfigurationSettings);
							}
							if (offlineAddressBookCacheEntry != null)
							{
								if (flag2)
								{
									Uri datacenterFrontEndOabUrl = FrontEndLocator.GetDatacenterFrontEndOabUrl();
									if (datacenterFrontEndOabUrl != null && !string.IsNullOrEmpty(datacenterFrontEndOabUrl.AbsoluteUri))
									{
										UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalOABUrl, UserSettingsProvider.GetOabServiceUrl(offlineAddressBookCacheEntry, datacenterFrontEndOabUrl.AbsoluteUri, this.IsClientNegoExCapable()), requestedSettings, userConfigurationSettings);
									}
								}
								else
								{
									this.GetOabSettings(ep, userSite, offlineAddressBookCacheEntry, requestedSettings, userConfigurationSettings, true);
								}
							}
							if (!flag2)
							{
								this.GetProtocolUserSettings(configFromSourceObject2, ep, preferredSite, requestedSettings, userConfigurationSettings, ref value2, ref text5);
							}
						}
					}
					string text6 = null;
					if ((clientAccessModes & ClientAccessModes.ExternalAccess) != ClientAccessModes.None)
					{
						configFromSourceObject2 = this.serverConfigCache.OutlookProviderCache.GetConfigFromSourceObject("EXPR");
						if (configFromSourceObject2 != null)
						{
							if (uri != null)
							{
								string value3 = uri.AbsoluteUri;
								if (this.CallerCapabilities.IsCallerCrossForestAvailabilityService && flag4 && !string.IsNullOrEmpty(this.emailAddress) && e15MinVersion >= Server.E15MinVersion)
								{
									value3 = Common.AddUserHintToUrl(uri, this.emailAddress);
								}
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalEwsUrl, value3, requestedSettings, userConfigurationSettings);
								if (flag5)
								{
									UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EwsPartnerUrl, uri.AbsoluteUri, requestedSettings, userConfigurationSettings);
								}
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalEmwsUrl, uri.AbsoluteUri, requestedSettings, userConfigurationSettings);
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalEwsVersion, UserSettingsProvider.FormatServerVersion(e15MinVersion), requestedSettings, userConfigurationSettings);
							}
							if (uri3 != null)
							{
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InteropExternalEwsUrl, uri3.AbsoluteUri, requestedSettings, userConfigurationSettings);
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InteropExternalEwsVersion, UserSettingsProvider.FormatServerVersion(serverVersion), requestedSettings, userConfigurationSettings);
							}
							if (uri7 != null)
							{
								UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalUMUrl, uri7.AbsoluteUri, requestedSettings, userConfigurationSettings);
							}
							if (offlineAddressBookCacheEntry != null)
							{
								if (flag2)
								{
									Uri datacenterFrontEndOabUrl2 = FrontEndLocator.GetDatacenterFrontEndOabUrl();
									if (datacenterFrontEndOabUrl2 != null && !string.IsNullOrEmpty(datacenterFrontEndOabUrl2.AbsoluteUri))
									{
										UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalOABUrl, UserSettingsProvider.GetOabServiceUrl(offlineAddressBookCacheEntry, datacenterFrontEndOabUrl2.AbsoluteUri, this.IsClientNegoExCapable()), requestedSettings, userConfigurationSettings);
									}
								}
								else
								{
									this.GetOabSettings(ep, userSite, offlineAddressBookCacheEntry, requestedSettings, userConfigurationSettings, false);
								}
							}
							if (flag2)
							{
								MiniVirtualDirectory datacenterRpcHttpVdir = FrontEndLocator.GetDatacenterRpcHttpVdir();
								if (datacenterRpcHttpVdir != null)
								{
									string text7 = (datacenterRpcHttpVdir.ExternalUrl == null) ? null : datacenterRpcHttpVdir.ExternalUrl.DnsSafeHost;
									value2 = text7;
									text6 = AutodiscoverRpcHttpSettings.GetAuthPackageStringFromAuthMethod(this.GetClientUsableAuthMethod(datacenterRpcHttpVdir.ExternalClientAuthenticationMethod, datacenterRpcHttpVdir.IISAuthenticationMethods, true));
									UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalMailboxServer, text7, requestedSettings, userConfigurationSettings);
									UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalMailboxServerRequiresSSL, "On", requestedSettings, userConfigurationSettings);
									UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalMailboxServerAuthenticationMethods, text6, requestedSettings, userConfigurationSettings);
									UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExchangeRpcUrl, datacenterRpcHttpVdir.XropUrl, requestedSettings, userConfigurationSettings);
								}
							}
							else
							{
								this.GetProtocolUserSettings(configFromSourceObject2, ep, preferredSite, requestedSettings, userConfigurationSettings, ref value2, ref text6);
							}
						}
					}
					if (!string.IsNullOrEmpty(value2))
					{
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.PublicFolderServer, value2, requestedSettings, userConfigurationSettings);
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.InternalMailboxServerAuthenticationMethods))
					{
						string internalMailboxServerAuthenticationMethods = UserSettingsProvider.GetInternalMailboxServerAuthenticationMethods(ep.MailboxInfo.Location.ServerVersion, VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Autodiscover.AnonymousAuthentication.Enabled, new string[]
						{
							text5,
							text6
						});
						if (!string.IsNullOrEmpty(internalMailboxServerAuthenticationMethods))
						{
							UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalMailboxServerAuthenticationMethods, internalMailboxServerAuthenticationMethods, requestedSettings, userConfigurationSettings);
						}
					}
					UserSettingsProvider.AddPhotosUrlIfRequested(datacenterFrontEndWebServicesUrl, uri, ep, requestedSettings, userConfigurationSettings);
					Organization organization = OrganizationCache.Singleton.Get(aduser.OrganizationId);
					if (organization != null)
					{
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.MapiHttpEnabled, organization.MapiHttpEnabled, requestedSettings, userConfigurationSettings);
						UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.MapiHttpEnabledForUser, this.recipient.MapiHttpEnabled, requestedSettings, userConfigurationSettings);
					}
					if (flag2)
					{
						MapiHttpProtocolUrls value4 = new MapiHttpProtocolUrls(null, FrontEndLocator.GetDatacenterMapiHttpUrl(), text, DateTime.MinValue);
						userConfigurationSettings.Add(UserConfigurationSettingName.MapiHttpUrls, value4);
					}
					else
					{
						MapiHttpSettingsProvider mapiHttpSettingsProvider = new MapiHttpSettingsProvider((ClientAccessType clientAccessType) => this.GetPreferredService<MapiHttpService>(ep.MailboxInfo.Location.ServerVersion, preferredSite, clientAccessType, new UserSettingsProvider.IncludeServiceDelegate<MapiHttpService>(UserSettingsProvider.AnyService<MapiHttpService>), new UserSettingsProvider.IsValidCasVersionDelegate(UserSettingsProvider.ClientAccessServerEqualHigherMajorVersionThanMailboxServer)));
						mapiHttpSettingsProvider.DiscoverSettings(requestedSettings, text, userConfigurationSettings);
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.AlternateMailboxes) && !flag)
					{
						AlternateMailboxCollection alternateMailboxCollection = new AlternateMailboxCollection();
						if (aduser.ArchiveDomain != null)
						{
							AlternateMailbox item = new AlternateMailbox
							{
								Type = "Archive",
								DisplayName = AutodiscoverCommonUserSettings.GetArchiveDisplayName(aduser),
								SmtpAddress = this.GetArchiveSmtpAddress(aduser),
								OwnerSmtpAddress = aduser.PrimarySmtpAddress.ToString()
							};
							alternateMailboxCollection.Add(item);
						}
						else if (aduser.ArchiveGuid != Guid.Empty)
						{
							string text8 = ExchangeRpcClientAccess.CreatePersonalizedServer(aduser.ArchiveGuid, domain);
							alternateMailboxCollection.Add(this.GetIntraForestArchiveMailbox(aduser, flag4 ? text8 : rpcClientAccessFqdn));
						}
						foreach (ADObjectId adobjectId in aduser.DelegateListBL)
						{
							if (!ADObjectId.Equals(adobjectId, aduser.Id))
							{
								ADUser aduser2 = this.GetADRecipientForUser(aduser.OrganizationId, adobjectId) as ADUser;
								if (aduser2 != null && aduser2.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
								{
									if (!TeamMailbox.IsLocalTeamMailbox(aduser2) && !TeamMailbox.IsRemoteTeamMailbox(aduser2))
									{
										alternateMailboxCollection.AddRange(this.GetAlternativeMailboxCollection(aduser2, "Delegate"));
									}
									else if (flag8 && TeamMailbox.IsActiveTeamMailbox(aduser2) && aduser.TeamMailboxShowInClientList.Contains(adobjectId))
									{
										alternateMailboxCollection.AddRange(this.GetAlternativeMailboxCollection(aduser2, "TeamMailbox"));
									}
								}
							}
						}
						if (alternateMailboxCollection.Count > 0)
						{
							userConfigurationSettings.Add(UserConfigurationSettingName.AlternateMailboxes, alternateMailboxCollection);
						}
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.DocumentSharingLocations))
					{
						DocumentSharingLocationCollection documentSharingLocationCollection = new DocumentSharingLocationCollection();
						documentSharingLocationCollection.Discover(aduser.Alias);
						if (documentSharingLocationCollection.Count > 0)
						{
							userConfigurationSettings.Add(UserConfigurationSettingName.DocumentSharingLocations, documentSharingLocationCollection);
						}
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.MobileMailboxPolicy))
					{
						string policyDataForUser = MobilePolicySettingsHelper.GetPolicyDataForUser(aduser, budget);
						if (!string.IsNullOrEmpty(policyDataForUser))
						{
							userConfigurationSettings.Add(UserConfigurationSettingName.MobileMailboxPolicy, policyDataForUser);
						}
					}
					if (requestedSettings.Contains(UserConfigurationSettingName.PublicFolderInformation))
					{
						TenantPublicFolderConfiguration value5 = TenantPublicFolderConfigurationCache.Instance.GetValue(aduser.OrganizationId);
						PublicFolderRecipient publicFolderRecipient = value5.GetPublicFolderRecipient(ep.MailboxInfo.MailboxGuid, ep.DefaultPublicFolderMailbox);
						if (publicFolderRecipient != null)
						{
							SmtpAddress primarySmtpAddress = publicFolderRecipient.PrimarySmtpAddress;
							if (publicFolderRecipient.PrimarySmtpAddress.IsValidAddress)
							{
								userConfigurationSettings.Add(UserConfigurationSettingName.PublicFolderInformation, publicFolderRecipient.PrimarySmtpAddress.ToString());
							}
						}
					}
					HashSet<UserConfigurationSettingName> hashSet = UserSettingsProvider.serviceEndpointSettings;
					if (flag8)
					{
						hashSet = UserSettingsProvider.serviceEndpointSettingsWithSiteMailboxCreation;
					}
					if (hashSet.Count((UserConfigurationSettingName s) => requestedSettings.Contains(s)) > 0)
					{
						Organization organization2 = OrganizationCache.Singleton.Get(aduser.OrganizationId);
						string text9 = (organization2 != null) ? organization2.ServiceEndpoints : string.Empty;
						if (!string.IsNullOrWhiteSpace(text9))
						{
							Dictionary<string, string> dictionary = Organization.ParseServiceEndpointsAttribute(text9);
							foreach (UserConfigurationSettingName userConfigurationSettingName3 in hashSet)
							{
								string value6;
								if (requestedSettings.Contains(userConfigurationSettingName3) && dictionary.TryGetValue(userConfigurationSettingName3.ToString(), out value6) && !string.IsNullOrWhiteSpace(value6))
								{
									userConfigurationSettings.Add(userConfigurationSettingName3, value6);
								}
							}
						}
					}
					return userConfigurationSettings;
				}
				finally
				{
					Interlocked.Decrement(ref UserSettingsProvider.expensiveRequestCount);
				}
				IL_16F0:
				userConfigurationSettings.ErrorCode = UserConfigurationSettingsErrorCode.InvalidUser;
				userConfigurationSettings.ErrorMessage = string.Format(Strings.InvalidUser, this.emailAddress);
			}
			return userConfigurationSettings;
		}

		public bool UseClientCertificateAuthentication
		{
			get
			{
				return this.useClientCertificateAuthentication;
			}
		}

		public CallerRequestedCapabilities CallerCapabilities { get; private set; }

		internal static string GetInternalMailboxServerAuthenticationMethods(int serverVersion, bool isAnonymousAuthenticationEnabled, params string[] authenticationMethods)
		{
			if (serverVersion >= Server.E15MinVersion)
			{
				if (!isAnonymousAuthenticationEnabled)
				{
					if (authenticationMethods == null)
					{
						goto IL_39;
					}
					if (!authenticationMethods.Any((string authMethod) => string.Equals("Negotiate", authMethod, StringComparison.OrdinalIgnoreCase)))
					{
						goto IL_39;
					}
				}
				return "Anonymous";
			}
			IL_39:
			return null;
		}

		private static bool IsArchiveEmailAddress(ADUser caller, ADRecipient requestedRecipient, string emailAddress)
		{
			return AutodiscoverCommonUserSettings.IsArchiveMailUser(requestedRecipient) || (caller != null && AutodiscoverCommonUserSettings.HasLocalArchive(caller) && AutodiscoverCommonUserSettings.IsEmailAddressTargetingArchive(caller, emailAddress));
		}

		private static string FormatServerVersion(int serverVersion)
		{
			ServerVersion serverVersion2 = new ServerVersion(serverVersion);
			return string.Format(CultureInfo.InvariantCulture, "{0:d}.{1:d2}.{2:d4}.{3:d3}", new object[]
			{
				serverVersion2.Major,
				serverVersion2.Minor,
				serverVersion2.Build,
				serverVersion2.Revision
			});
		}

		private static bool IsEcpSettingRequested(ICollection<UserConfigurationSettingName> requestedSettings)
		{
			foreach (UserConfigurationSettingName key in requestedSettings)
			{
				if (UserSettingsProvider.ecpInternalSettings.ContainsKey(key) || UserSettingsProvider.ecpExternalSettings.ContainsKey(key))
				{
					return true;
				}
			}
			return false;
		}

		private static string GetOabServiceUrl(OfflineAddressBookCacheEntry oab, string baseOabUrl, bool clientIsLiveSspCapable)
		{
			string text = baseOabUrl;
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			if (!text.EndsWith("/OAB/", StringComparison.OrdinalIgnoreCase))
			{
				text += "OAB/";
			}
			if (clientIsLiveSspCapable && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.WindowsLiveID.Enabled)
			{
				text += "Nego2/";
			}
			return text + oab.Guid.ToString() + "/";
		}

		private static void AddSettingIfRequested(UserConfigurationSettingName settingName, object value, ICollection<UserConfigurationSettingName> requestedSettings, UserConfigurationSettings settings)
		{
			if (requestedSettings.Contains(settingName))
			{
				settings.Add(settingName, value);
			}
		}

		private static string GetAutoDiscoverSMTPAddress(ADUser adu)
		{
			ProxyAddressCollection emailAddresses = adu.EmailAddresses;
			foreach (ProxyAddress proxyAddress in emailAddresses)
			{
				if (proxyAddress.IsPrimaryAddress && proxyAddress.Prefix == UserSettingsProvider.autodiscoverPrefix)
				{
					return proxyAddress.AddressString;
				}
			}
			return adu.PrimarySmtpAddress.ToString();
		}

		private static bool IsServiceVersionAcceptable(ExchangeServerVersion minVersionToUse, ExchangeServerVersion serviceVersion)
		{
			return (minVersionToUse == ExchangeServerVersion.E12 && serviceVersion == ExchangeServerVersion.E12) || (minVersionToUse >= ExchangeServerVersion.E14 && serviceVersion <= ExchangeServerVersion.E14_SP1 && minVersionToUse <= serviceVersion) || (minVersionToUse >= ExchangeServerVersion.E14_SP1 && serviceVersion <= ExchangeServerVersion.E14_SP2 && minVersionToUse <= serviceVersion) || (minVersionToUse == ExchangeServerVersion.E15 && serviceVersion == ExchangeServerVersion.E15);
		}

		private static bool IsLiveFrontEndOrDownLevel(HttpService service)
		{
			return (service.IsFrontEnd || UserSettingsProvider.GetServerVersion(service.ServerVersionNumber) < ExchangeServerVersion.E15) && !service.IsOutOfService;
		}

		private static bool ClientAccessServerEqualHigherMajorVersionThanMailboxServer(ExchangeServerVersion casVersion, ClientAccessType clientAccessType, ExchangeServerVersion mailboxVersion)
		{
			if (mailboxVersion <= casVersion)
			{
				bool flag = UserSettingsProvider.IsServiceVersionAcceptable(mailboxVersion, casVersion);
				if (flag)
				{
					return true;
				}
				if (clientAccessType == ClientAccessType.External)
				{
					return true;
				}
			}
			return false;
		}

		private static bool AnyClientAccessServer(ExchangeServerVersion casVersion, ClientAccessType clientAccessType, ExchangeServerVersion mailboxVersion)
		{
			return true;
		}

		private static bool AnyService<TService>(TService service) where TService : Service
		{
			return true;
		}

		private static void AddPhotosUrlIfRequested(Uri internalEwsUrl, Uri externalEwsUrl, ExchangePrincipal mailbox, HashSet<UserConfigurationSettingName> requestedSettings, UserConfigurationSettings settings)
		{
			if (!UserSettingsProvider.MailboxSupportsHighResolutionPhotos(mailbox))
			{
				return;
			}
			if (internalEwsUrl != null)
			{
				UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalPhotosUrl, UserSettingsProvider.GetPhotosUrlFromEwsUrl(internalEwsUrl).AbsoluteUri, requestedSettings, settings);
			}
			if (externalEwsUrl != null)
			{
				UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalPhotosUrl, UserSettingsProvider.GetPhotosUrlFromEwsUrl(externalEwsUrl).AbsoluteUri, requestedSettings, settings);
			}
		}

		private static bool MailboxSupportsHighResolutionPhotos(ExchangePrincipal mailbox)
		{
			return mailbox != null && mailbox.MailboxInfo.Location.ServerVersion >= Server.E15MinVersion;
		}

		private static Uri GetPhotosUrlFromEwsUrl(Uri ewsUrl)
		{
			UriBuilder uriBuilder = new UriBuilder(ewsUrl);
			UriBuilder uriBuilder2 = uriBuilder;
			uriBuilder2.Path += "/s";
			return uriBuilder.Uri;
		}

		private void AddEcpConnectUrlIfRequested(ADUser adUser, HashSet<UserConfigurationSettingName> requestedSettings, UserConfigurationSettings settings)
		{
			if (!UserSettingsProvider.isMSOnline)
			{
				return;
			}
			UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpConnectUrlFragment, this.AppendRealmForEcpUrl("Connect/Main.aspx?rfr=olk&exsvurl=1&Provider=<Provider>&Action=<Action>", adUser), requestedSettings, settings);
		}

		private void AddPhotoEcpUrlsIfRequested(ExchangePrincipal mailbox, ADUser adUser, HashSet<UserConfigurationSettingName> requestedSettings, UserConfigurationSettings settings)
		{
			if (!UserSettingsProvider.MailboxSupportsHighResolutionPhotos(mailbox))
			{
				return;
			}
			UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.EcpPhotoUrlFragment, this.AppendRealmForEcpUrl("PersonalSettings/EditAccount.aspx?rfr=olk&chgPhoto=1&exsvurl=1", adUser), requestedSettings, settings);
		}

		private string AppendRealmForEcpUrl(string ecpUrl, ADUser adUser)
		{
			SmtpAddress smtpAddress = new SmtpAddress(adUser.UserPrincipalName);
			if (smtpAddress.IsValidAddress)
			{
				ecpUrl = ecpUrl + "&realm=" + HttpUtility.UrlEncode(smtpAddress.Domain);
			}
			return ecpUrl;
		}

		private string AppendRealmForOwaUrl(string owaUrl, ADUser adUser)
		{
			SmtpAddress smtpAddress = new SmtpAddress(adUser.UserPrincipalName);
			if (smtpAddress.IsValidAddress)
			{
				owaUrl = owaUrl + HttpUtility.UrlEncode(smtpAddress.Domain) + "/";
			}
			return owaUrl;
		}

		private bool IsOrganizationalSharingEnabled(ADUser user)
		{
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(user.OrganizationId);
			if (organizationIdCacheValue == null || organizationIdCacheValue.FederatedOrganizationId == null || !organizationIdCacheValue.FederatedOrganizationId.Enabled || organizationIdCacheValue.FederatedOrganizationId.AccountNamespace == null || organizationIdCacheValue.FederatedDomains == null)
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "[IsOrganizationalSharingEnabled()] Federation not enabled for organization: {0}", user.OrganizationId);
				return false;
			}
			return true;
		}

		internal void WaitForCaches()
		{
			if (UserSettingsProvider.expensiveRequestCount < 10)
			{
				int num = 0;
				while (num < 10 && !this.serverConfigCache.AllCachesAreLoaded)
				{
					Thread.Sleep(1000);
					num++;
				}
			}
		}

		internal ClientAccessModes GetClientAccessModes()
		{
			if (this.CallerCapabilities.IsCallerCrossForestAvailabilityService)
			{
				return ClientAccessModes.InternalAccess | ClientAccessModes.ExternalAccess;
			}
			if (!this.recipient.MAPIEnabled)
			{
				return ClientAccessModes.None;
			}
			if (this.recipient.MAPIBlockOutlookRpcHttp)
			{
				return ClientAccessModes.InternalAccess;
			}
			if (this.recipient.MAPIBlockOutlookExternalConnectivity)
			{
				return ClientAccessModes.InternalAccess;
			}
			return ClientAccessModes.InternalAccess | ClientAccessModes.ExternalAccess;
		}

		private OwaUrlCollection GetOwaUrlCollectionForClientAccess(ExchangePrincipal ep, ADUser adUser, ClientAccessType caType)
		{
			OwaUrlCollection owaUrlCollection = null;
			VariantConfigurationSnapshot configuration = ep.GetConfiguration();
			if (Common.SkipServiceTopologyInDatacenter(configuration))
			{
				owaUrlCollection = new OwaUrlCollection();
				Uri datacenterFrontEndOwaUrl = FrontEndLocator.GetDatacenterFrontEndOwaUrl();
				if (datacenterFrontEndOwaUrl != null)
				{
					OwaUrl owaUrl = new OwaUrl();
					owaUrl.Url = this.FixupOwaUrl(datacenterFrontEndOwaUrl.ToString(), adUser, caType);
					if (caType == ClientAccessType.Internal)
					{
						owaUrl.AuthenticationMethods = VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.OWAUrl.OwaInternalAuthMethods;
					}
					else if (caType == ClientAccessType.External)
					{
						owaUrl.AuthenticationMethods = VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.OWAUrl.OwaExternalAuthMethods;
					}
					owaUrlCollection.Add(owaUrl);
				}
			}
			else
			{
				ExchangeServerVersion versionToUse = UserSettingsProvider.GetServerVersion(ep.MailboxInfo.Location.ServerVersion);
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetOwaUrlCollectionForClientAccess", 2123);
				IList<OwaService> list = currentServiceTopology.FindAll<OwaService>(ep, caType, new Predicate<OwaService>(UserSettingsProvider.IsLiveFrontEndOrDownLevel), "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetOwaUrlCollectionForClientAccess", 2124);
				if (list.Count == 0)
				{
					list = new List<OwaService>();
					OwaService owaService = currentServiceTopology.FindAny<OwaService>(caType, (OwaService service) => UserSettingsProvider.GetServerVersion(service.ServerVersionNumber) == versionToUse && UserSettingsProvider.IsLiveFrontEndOrDownLevel(service), "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetOwaUrlCollectionForClientAccess", 2137);
					if (owaService != null)
					{
						list.Add(owaService);
					}
				}
				if (list.Count != 0)
				{
					List<OwaService> list2 = new List<OwaService>(list.Count);
					OwaServiceUriComparer comparer = new OwaServiceUriComparer();
					foreach (OwaService owaService2 in list)
					{
						if (UserSettingsProvider.GetServerVersion(owaService2.ServerVersionNumber) == versionToUse)
						{
							int num = list2.BinarySearch(owaService2, comparer);
							if (num == ~list.Count)
							{
								list2.Add(owaService2);
							}
							else if (num < 0)
							{
								list2.Insert(~num, owaService2);
							}
						}
					}
					owaUrlCollection = new OwaUrlCollection();
					foreach (OwaService owaService3 in list2)
					{
						owaUrlCollection.Add(new OwaUrl
						{
							AuthenticationMethods = owaService3.AuthenticationMethod.ToString(),
							Url = this.FixupOwaUrl(owaService3.Url.ToString(), adUser, caType)
						});
					}
				}
			}
			return owaUrlCollection;
		}

		private string FixupOwaUrl(string url, ADUser adUser, ClientAccessType caType)
		{
			string text = url;
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			if (Common.IsMultiTenancyEnabled && caType == ClientAccessType.External)
			{
				text = this.AppendRealmForOwaUrl(text, adUser);
			}
			return text;
		}

		private PopImapSmtpConnectionCollection GetPopImapConnectionSettings<T>(ExchangePrincipal ep, ClientAccessType caType) where T : EmailTransportService
		{
			Predicate<T> predicate = null;
			Predicate<T> predicate2 = null;
			List<ProtocolConnectionSettings> list = new List<ProtocolConnectionSettings>();
			VariantConfigurationSnapshot configuration = ep.GetConfiguration();
			if (Common.SkipServiceTopologyInDatacenter(configuration))
			{
				ProtocolConnectionSettings frontEndPop3SettingsForLocalServer = FrontEndLocator.GetFrontEndPop3SettingsForLocalServer();
				ProtocolConnectionSettings frontEndImap4SettingsForLocalServer = FrontEndLocator.GetFrontEndImap4SettingsForLocalServer();
				if (frontEndPop3SettingsForLocalServer != null)
				{
					list.Add(frontEndPop3SettingsForLocalServer);
				}
				if (frontEndImap4SettingsForLocalServer != null)
				{
					list.Add(frontEndImap4SettingsForLocalServer);
				}
			}
			else
			{
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetPopImapConnectionSettings", 2253);
				ServiceTopology serviceTopology = currentServiceTopology;
				if (predicate == null)
				{
					predicate = ((T service) => (service.ServerRole & ServerRole.Cafe) == ServerRole.Cafe);
				}
				IList<T> list2 = serviceTopology.FindAll<T>(ep, caType, predicate, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetPopImapConnectionSettings", 2254);
				if (list2.Count == 0)
				{
					list2 = new List<T>();
					ServiceTopology serviceTopology2 = currentServiceTopology;
					if (predicate2 == null)
					{
						predicate2 = ((T service) => (service.ServerRole & ServerRole.Cafe) == ServerRole.Cafe && service.PopImapTransport);
					}
					T t = serviceTopology2.FindAny<T>(caType, predicate2, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetPopImapConnectionSettings", 2267);
					if (t != null)
					{
						list2.Add(t);
					}
				}
				if (list2.Count != 0)
				{
					foreach (T t2 in list2)
					{
						if (t2.PopImapTransport)
						{
							IList<ProtocolConnectionSettings> collection;
							if (caType == ClientAccessType.External)
							{
								collection = t2.ExternalConnectionSettings;
							}
							else
							{
								collection = t2.InternalConnectionSettings;
							}
							list.AddRange(collection);
						}
					}
				}
			}
			PopImapSmtpConnectionCollection popImapSmtpConnectionCollection = null;
			foreach (ProtocolConnectionSettings protocolConnectionSettings in list)
			{
				if (popImapSmtpConnectionCollection == null)
				{
					popImapSmtpConnectionCollection = new PopImapSmtpConnectionCollection();
				}
				PopImapSmtpConnection popImapSmtpConnection = new PopImapSmtpConnection();
				if (protocolConnectionSettings.EncryptionType != null)
				{
					popImapSmtpConnection.EncryptionMethod = protocolConnectionSettings.EncryptionType.ToString();
				}
				if (protocolConnectionSettings.Hostname != null)
				{
					popImapSmtpConnection.Hostname = protocolConnectionSettings.Hostname.ToString();
				}
				popImapSmtpConnection.Port = protocolConnectionSettings.Port;
				popImapSmtpConnectionCollection.Add(popImapSmtpConnection);
			}
			return popImapSmtpConnectionCollection;
		}

		private PopImapSmtpConnectionCollection GetSmtpConnectionSettings(ExchangePrincipal ep, ClientAccessType caType)
		{
			List<ProtocolConnectionSettings> list = new List<ProtocolConnectionSettings>();
			VariantConfigurationSnapshot configuration = ep.GetConfiguration();
			if (Common.SkipServiceTopologyInDatacenter(configuration))
			{
				ProtocolConnectionSettings frontEndSmtpSettingsForLocalServer = FrontEndLocator.GetFrontEndSmtpSettingsForLocalServer();
				if (frontEndSmtpSettingsForLocalServer != null)
				{
					list.Add(frontEndSmtpSettingsForLocalServer);
				}
			}
			else
			{
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetSmtpConnectionSettings", 2346);
				IList<SmtpService> list2 = currentServiceTopology.FindAll<SmtpService>(ep, caType, (SmtpService service) => (service.ServerRole & ServerRole.Cafe) == ServerRole.Cafe, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetSmtpConnectionSettings", 2347);
				if (list2.Count == 0)
				{
					list2 = new List<SmtpService>();
					SmtpService smtpService = currentServiceTopology.FindAny<SmtpService>(caType, (SmtpService service) => (service.ServerRole & ServerRole.Cafe) == ServerRole.Cafe, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetSmtpConnectionSettings", 2360);
					if (smtpService != null)
					{
						list2.Add(smtpService);
					}
				}
				if (list2.Count != 0)
				{
					foreach (SmtpService smtpService2 in list2)
					{
						list.AddRange(smtpService2.ProtocolConnectionSettings);
					}
				}
			}
			PopImapSmtpConnectionCollection popImapSmtpConnectionCollection = null;
			foreach (ProtocolConnectionSettings protocolConnectionSettings in list)
			{
				if (popImapSmtpConnectionCollection == null)
				{
					popImapSmtpConnectionCollection = new PopImapSmtpConnectionCollection();
				}
				PopImapSmtpConnection popImapSmtpConnection = new PopImapSmtpConnection();
				if (protocolConnectionSettings.EncryptionType != null)
				{
					popImapSmtpConnection.EncryptionMethod = protocolConnectionSettings.EncryptionType.ToString();
				}
				if (protocolConnectionSettings.Hostname != null)
				{
					popImapSmtpConnection.Hostname = protocolConnectionSettings.Hostname.ToString();
				}
				popImapSmtpConnection.Port = protocolConnectionSettings.Port;
				popImapSmtpConnectionCollection.Add(popImapSmtpConnection);
			}
			return popImapSmtpConnectionCollection;
		}

		private void GetProtocolUserSettings(OutlookProvider opc, ExchangePrincipal ep, Site preferredSite, ICollection<UserConfigurationSettingName> requestedSettings, UserConfigurationSettings settings, ref string publicFolderServer, ref string authPackageString)
		{
			bool flag = string.Equals(opc.Name, "EXCH", StringComparison.OrdinalIgnoreCase);
			ClientAccessType clientAccessType = flag ? ClientAccessType.Internal : ClientAccessType.External;
			RpcHttpService preferredService = this.GetPreferredService<RpcHttpService>(ep.MailboxInfo.Location.ServerVersion, preferredSite, clientAccessType, new UserSettingsProvider.IncludeServiceDelegate<RpcHttpService>(UserSettingsProvider.AnyService<RpcHttpService>), new UserSettingsProvider.IsValidCasVersionDelegate(UserSettingsProvider.ClientAccessServerEqualHigherMajorVersionThanMailboxServer));
			if (preferredService == null || preferredService.Url == null)
			{
				return;
			}
			AutodiscoverRpcHttpSettings rpcHttpAuthSettingsFromService = AutodiscoverRpcHttpSettings.GetRpcHttpAuthSettingsFromService(preferredService, clientAccessType, (AuthenticationMethod clientAuthMethod, ICollection<AuthenticationMethod> iisAuthMethods, bool useSsl) => this.GetClientUsableAuthMethod(clientAuthMethod, iisAuthMethods, useSsl));
			publicFolderServer = rpcHttpAuthSettingsFromService.RpcHttpServer;
			authPackageString = rpcHttpAuthSettingsFromService.AuthPackageString;
			if (flag)
			{
				UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalRpcHttpServer, rpcHttpAuthSettingsFromService.RpcHttpServer, requestedSettings, settings);
				UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalRpcHttpConnectivityRequiresSsl, rpcHttpAuthSettingsFromService.SslRequired ? "On" : "Off", requestedSettings, settings);
				UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.InternalRpcHttpAuthenticationMethod, rpcHttpAuthSettingsFromService.AuthPackageString, requestedSettings, settings);
				return;
			}
			UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalMailboxServer, rpcHttpAuthSettingsFromService.RpcHttpServer, requestedSettings, settings);
			UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalMailboxServerRequiresSSL, rpcHttpAuthSettingsFromService.SslRequired ? "On" : "Off", requestedSettings, settings);
			UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExternalMailboxServerAuthenticationMethods, rpcHttpAuthSettingsFromService.AuthPackageString, requestedSettings, settings);
			UserSettingsProvider.AddSettingIfRequested(UserConfigurationSettingName.ExchangeRpcUrl, rpcHttpAuthSettingsFromService.XropUrl, requestedSettings, settings);
		}

		private void GetOabSettings(ExchangePrincipal ep, Site userSite, OfflineAddressBookCacheEntry oab, ICollection<UserConfigurationSettingName> requestedSettings, UserConfigurationSettings settings, bool internalProvider)
		{
			string value = null;
			if (oab.WebDistributionEnabled)
			{
				OabService cheapestOabService = this.GetCheapestOabService(ep, userSite, oab, internalProvider);
				if (cheapestOabService != null)
				{
					value = UserSettingsProvider.GetOabServiceUrl(oab, cheapestOabService.Url.ToString(), this.IsClientNegoExCapable());
				}
			}
			if (!string.IsNullOrEmpty(value))
			{
				UserConfigurationSettingName settingName = internalProvider ? UserConfigurationSettingName.InternalOABUrl : UserConfigurationSettingName.ExternalOABUrl;
				UserSettingsProvider.AddSettingIfRequested(settingName, value, requestedSettings, settings);
			}
		}

		private OabService GetCheapestOabService(ExchangePrincipal ep, Site userSite, OfflineAddressBookCacheEntry oab, bool internalProvider)
		{
			OabService result = null;
			if (userSite != null)
			{
				result = this.GetPreferredService<OabService>(ep.MailboxInfo.Location.ServerVersion, userSite, internalProvider ? ClientAccessType.Internal : ClientAccessType.External, (OabService service) => this.OabServiceHasGivenOab(service, oab), new UserSettingsProvider.IsValidCasVersionDelegate(UserSettingsProvider.AnyClientAccessServer));
			}
			return result;
		}

		private bool OabServiceHasGivenOab(OabService service, OfflineAddressBookCacheEntry oab)
		{
			if ((oab.ExchangeVersion.CompareTo(ExchangeObjectVersion.Exchange2012) >= 0 && service.ServerVersionNumber < Server.E15MinVersion) || (oab.ExchangeVersion.CompareTo(ExchangeObjectVersion.Exchange2012) < 0 && service.ServerVersionNumber >= Server.E15MinVersion))
			{
				return false;
			}
			if (oab.GlobalWebDistributionEnabled)
			{
				return true;
			}
			HashSet<string> linkedOfflineAddressBookDistinguishedNames = service.LinkedOfflineAddressBookDistinguishedNames;
			return linkedOfflineAddressBookDistinguishedNames.Contains(oab.DistinguishedName);
		}

		private string GetRpcClientAccessFqdn(ServerId rpcServerId)
		{
			Server configFromSourceObject = this.serverConfigCache.ServerCache.GetConfigFromSourceObject(rpcServerId);
			if (configFromSourceObject != null)
			{
				return configFromSourceObject.Fqdn;
			}
			ClientAccessArray configFromSourceObject2 = this.serverConfigCache.ClientAccessArrayCache.GetConfigFromSourceObject(rpcServerId);
			if (configFromSourceObject2 != null && configFromSourceObject2.IsPriorTo15ExchangeObjectVersion)
			{
				return configFromSourceObject2.Fqdn;
			}
			ExTraceGlobals.FrameworkTracer.TraceDebug((long)this.GetHashCode(), "[GetRpcClientAccessFqdn] ClientAccessArray should not be null.");
			return null;
		}

		private Uri GetUM14Url(Uri ewsUrl)
		{
			string text = ewsUrl.AbsoluteUri;
			int num = text.LastIndexOf(".asmx", StringComparison.OrdinalIgnoreCase);
			if (num < 0)
			{
				return null;
			}
			text = text.Remove(num);
			num = text.LastIndexOf("/");
			if (num < 0)
			{
				return null;
			}
			text = text.Remove(num + 1);
			text += "UM2007Legacy.asmx";
			return new Uri(text);
		}

		internal static ExchangeServerVersion GetServerVersion(int version)
		{
			if (version >= Server.E15MinVersion)
			{
				return ExchangeServerVersion.E15;
			}
			if (version >= 1937866977)
			{
				return ExchangeServerVersion.E14_SP2;
			}
			if (version >= Server.E14SP1MinVersion)
			{
				return ExchangeServerVersion.E14_SP1;
			}
			if (version >= Server.E14MinVersion)
			{
				return ExchangeServerVersion.E14;
			}
			if (version >= Server.E2007MinVersion)
			{
				return ExchangeServerVersion.E12;
			}
			return ExchangeServerVersion.Legacy;
		}

		private T GetVersionMatchedService<T>(IList<T> services, ExchangeServerVersion versionToUse) where T : Service
		{
			if (services != null)
			{
				foreach (T result in services)
				{
					ExchangeServerVersion serverVersion = UserSettingsProvider.GetServerVersion(result.ServerVersionNumber);
					if (UserSettingsProvider.IsServiceVersionAcceptable(versionToUse, serverVersion))
					{
						return result;
					}
				}
			}
			return default(T);
		}

		private void GetServiceUrls<T>(ExchangePrincipal mailboxUser, Site mbxServerSite, out Uri internalUrl, out Uri externalUrl) where T : HttpService
		{
			int num;
			this.GetServiceUrls<T>(mailboxUser, mbxServerSite, null, out internalUrl, out externalUrl, out num, false);
		}

		private void GetServiceUrls<T>(ExchangePrincipal mailboxUser, Site mbxServerSite, ExchangeServerVersion? requestedVersion, out Uri internalUrl, out Uri externalUrl, out int externalVersion, bool interop) where T : HttpService
		{
			ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetServiceUrls", 2784);
			ExchangeServerVersion exchangeServerVersion = UserSettingsProvider.GetServerVersion(mailboxUser.MailboxInfo.Location.ServerVersion);
			if (requestedVersion != null && requestedVersion.Value > exchangeServerVersion)
			{
				exchangeServerVersion = requestedVersion.Value;
			}
			try
			{
				IList<T> services = currentServiceTopology.FindAll<T>(mailboxUser, ClientAccessType.Internal, new Predicate<T>(UserSettingsProvider.IsLiveFrontEndOrDownLevel), "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetServiceUrls", 2798);
				T versionMatchedService = this.GetVersionMatchedService<T>(services, exchangeServerVersion);
				internalUrl = ((versionMatchedService == null) ? null : versionMatchedService.Url);
			}
			catch (ServerNotInSiteException)
			{
				internalUrl = null;
				if (!interop)
				{
					externalUrl = null;
					externalVersion = 0;
					return;
				}
			}
			T t = this.FindCheapest<T>(exchangeServerVersion, mbxServerSite);
			if (t != null)
			{
				externalUrl = t.Url;
				externalVersion = t.ServerVersionNumber;
				return;
			}
			externalUrl = null;
			externalVersion = 0;
		}

		private T FindCheapest<T>(ExchangeServerVersion versionToUse, Site mbxServerSite) where T : HttpService
		{
			ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "FindCheapest", 2845);
			SiteCostComparer<T> comparer = new SiteCostComparer<T>(currentServiceTopology, mbxServerSite);
			int randomCandidateCount = 1;
			T cheapest = default(T);
			currentServiceTopology.ForEach<T>(delegate(T service)
			{
				ExchangeServerVersion serverVersion = UserSettingsProvider.GetServerVersion(service.ServerVersionNumber);
				if (service.ClientAccessType == ClientAccessType.External && UserSettingsProvider.IsLiveFrontEndOrDownLevel(service) && versionToUse <= serverVersion)
				{
					int num = (cheapest == null) ? 1 : comparer.Compare(cheapest, service);
					bool flag = UserSettingsProvider.IsServiceVersionAcceptable(versionToUse, serverVersion);
					if (cheapest == null || (cheapest != null && num > 0) || (cheapest != null && num == 0 && flag && UserSettingsProvider.randomGenerator.Next(++randomCandidateCount) == 0))
					{
						cheapest = service;
					}
				}
			}, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "FindCheapest", 2849);
			return cheapest;
		}

		private T GetPreferredService<T>(int mailboxServerVersion, Site destSite, ClientAccessType caType, UserSettingsProvider.IncludeServiceDelegate<T> includeService, UserSettingsProvider.IsValidCasVersionDelegate isValidCasVersion) where T : HttpService
		{
			T preferredService = default(T);
			ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetPreferredService", 2898);
			SiteCostComparer<T> comparer = new SiteCostComparer<T>(currentServiceTopology, destSite);
			ExchangeServerVersion mailboxVersion = UserSettingsProvider.GetServerVersion(mailboxServerVersion);
			currentServiceTopology.ForEach<T>(delegate(T service)
			{
				if (UserSettingsProvider.IsLiveFrontEndOrDownLevel(service) && caType == service.ClientAccessType && includeService(service))
				{
					ExchangeServerVersion serverVersion = UserSettingsProvider.GetServerVersion(service.ServerVersionNumber);
					if (isValidCasVersion(serverVersion, service.ClientAccessType, mailboxVersion))
					{
						bool flag = UserSettingsProvider.IsServiceVersionAcceptable(mailboxVersion, serverVersion);
						if (preferredService == null)
						{
							preferredService = service;
							return;
						}
						int num = comparer.Compare(preferredService, service);
						if (num > 0)
						{
							preferredService = service;
							return;
						}
						if (num == 0 && flag && UserSettingsProvider.randomGenerator.Next(2) == 0)
						{
							preferredService = service;
						}
					}
				}
			}, "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs", "GetPreferredService", 2903);
			return preferredService;
		}

		private void GetUnifiedMessagingUrls(ExchangePrincipal mailboxUser, Site mbxServerSite, Uri internalE14EwsUrl, Uri externalE14EwsUrl, out Uri internalUrl, out Uri externalUrl)
		{
			internalUrl = null;
			externalUrl = null;
			switch (UserSettingsProvider.GetServerVersion(mailboxUser.MailboxInfo.Location.ServerVersion))
			{
			case ExchangeServerVersion.E12:
				this.GetServiceUrls<E12UnifiedMessagingService>(mailboxUser, mbxServerSite, out internalUrl, out externalUrl);
				return;
			case ExchangeServerVersion.E14:
			case ExchangeServerVersion.E14_SP1:
			case ExchangeServerVersion.E14_SP2:
			case ExchangeServerVersion.E15:
				if (internalE14EwsUrl != null)
				{
					internalUrl = this.GetUM14Url(internalE14EwsUrl);
				}
				if (externalE14EwsUrl != null)
				{
					externalUrl = this.GetUM14Url(externalE14EwsUrl);
				}
				return;
			default:
				return;
			}
		}

		private OfflineAddressBookCacheEntry GetWebDistributedOab(ADUser adu, OabCache oabCache, string oabExtensionAttribute)
		{
			OfflineAddressBookCacheEntry offlineAddressBookCacheEntry = null;
			ADPropertyDefinition propertyDefinition = null;
			string text = null;
			OrganizationId orgId = OrganizationId.ForestWideOrgId;
			if (adu != null && adu.OrganizationId != null)
			{
				orgId = adu.OrganizationId;
			}
			if (UserSettingsProvider.extensionAttributes.TryGetValue(oabExtensionAttribute, out propertyDefinition))
			{
				text = (string)adu[propertyDefinition];
				if (!string.IsNullOrEmpty(text))
				{
					try
					{
						offlineAddressBookCacheEntry = oabCache.GetOabById(orgId, new ADObjectId(text));
						if (offlineAddressBookCacheEntry == null)
						{
							Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrProviderOabNotExist, Common.PeriodicKey, new object[]
							{
								adu.Alias,
								oabExtensionAttribute,
								text
							});
						}
						goto IL_106;
					}
					catch (FormatException ex)
					{
						ExTraceGlobals.FrameworkTracer.TraceError<string>(0L, "Failed to get OAB with exception: {0}", ex.Message);
						Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrProviderFormatException, Common.PeriodicKey, new object[]
						{
							adu.Alias,
							oabExtensionAttribute,
							text,
							ex.Message
						});
						goto IL_106;
					}
				}
				ExTraceGlobals.FrameworkTracer.TraceDebug<string, string>(0L, "{0} does not have OAB configured on attribute {1}.", adu.Alias, oabExtensionAttribute);
				IL_106:
				if (offlineAddressBookCacheEntry != null && !offlineAddressBookCacheEntry.WebDistributionEnabled)
				{
					offlineAddressBookCacheEntry = null;
					Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrProviderOabMisconfiguration, Common.PeriodicKey, new object[]
					{
						adu.Alias,
						oabExtensionAttribute,
						text
					});
				}
			}
			else
			{
				Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrProviderRegistryMisconfiguration, Common.PeriodicKey, new object[]
				{
					oabExtensionAttribute
				});
			}
			return offlineAddressBookCacheEntry;
		}

		internal OfflineAddressBookCacheEntry CalculateOab(ADUser adu, MailboxDatabase mdb)
		{
			OfflineAddressBookCacheEntry offlineAddressBookCacheEntry = null;
			OabCache oabCache = this.serverConfigCache.OabCache;
			OrganizationId organizationId = OrganizationId.ForestWideOrgId;
			if (adu != null && adu.OrganizationId != null)
			{
				organizationId = adu.OrganizationId;
			}
			if (oabCache != null)
			{
				string oabExtensionAttribute = this.serverConfigCache.OabExtensionAttribute;
				if (adu != null && !string.IsNullOrEmpty(oabExtensionAttribute))
				{
					offlineAddressBookCacheEntry = this.GetWebDistributedOab(adu, oabCache, oabExtensionAttribute);
					if (offlineAddressBookCacheEntry != null)
					{
						return offlineAddressBookCacheEntry;
					}
				}
				if (adu != null && adu.OfflineAddressBook != null)
				{
					offlineAddressBookCacheEntry = oabCache.GetOabById(organizationId, adu.OfflineAddressBook);
				}
				else if (adu != null && adu.AddressBookPolicy != null)
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 3151, "CalculateOab", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs");
					AddressBookMailboxPolicy addressBookMailboxPolicy = tenantOrTopologyConfigurationSession.Read<AddressBookMailboxPolicy>(adu.AddressBookPolicy);
					if (addressBookMailboxPolicy != null && addressBookMailboxPolicy.OfflineAddressBook != null)
					{
						offlineAddressBookCacheEntry = oabCache.GetOabById(organizationId, addressBookMailboxPolicy.OfflineAddressBook);
					}
				}
				else if (mdb != null && mdb.OfflineAddressBook != null)
				{
					offlineAddressBookCacheEntry = oabCache.GetOabById(organizationId, mdb.OfflineAddressBook);
				}
				else if (adu != null && adu.OrganizationId != null && adu.OrganizationId.ConfigurationUnit != null)
				{
					offlineAddressBookCacheEntry = oabCache.GetOabByOrganizationId(adu.OrganizationId);
				}
				else
				{
					offlineAddressBookCacheEntry = oabCache.GetOabByOrganizationId(OrganizationId.ForestWideOrgId);
				}
			}
			return offlineAddressBookCacheEntry;
		}

		private AuthenticationMethod GetClientUsableAuthMethod(AuthenticationMethod clientAuthenticationMethod, ICollection<AuthenticationMethod> iisAuthenticationMethods, bool sslRequired)
		{
			if (clientAuthenticationMethod == AuthenticationMethod.NegoEx)
			{
				if (!this.IsClientNegoExCapable())
				{
					if (iisAuthenticationMethods.Contains(AuthenticationMethod.Ntlm))
					{
						clientAuthenticationMethod = AuthenticationMethod.Ntlm;
					}
					else if (sslRequired && (iisAuthenticationMethods.Contains(AuthenticationMethod.Basic) || iisAuthenticationMethods.Contains(AuthenticationMethod.LiveIdBasic)))
					{
						clientAuthenticationMethod = AuthenticationMethod.Basic;
					}
				}
			}
			else if (clientAuthenticationMethod == AuthenticationMethod.Negotiate)
			{
				if (!this.CallerCapabilities.CanHandleNegotiateCorrectly)
				{
					if (iisAuthenticationMethods.Contains(AuthenticationMethod.Ntlm) || iisAuthenticationMethods.Contains(AuthenticationMethod.Negotiate))
					{
						clientAuthenticationMethod = AuthenticationMethod.Ntlm;
					}
					else if (sslRequired && (iisAuthenticationMethods.Contains(AuthenticationMethod.Basic) || iisAuthenticationMethods.Contains(AuthenticationMethod.LiveIdBasic)))
					{
						clientAuthenticationMethod = AuthenticationMethod.Basic;
					}
					else
					{
						clientAuthenticationMethod = AuthenticationMethod.Misconfigured;
					}
				}
				else if (this.CallerCapabilities.CanHandleNegotiateEx)
				{
					clientAuthenticationMethod = AuthenticationMethod.NegoEx;
				}
			}
			return clientAuthenticationMethod;
		}

		private bool IsClientLiveSspCapable()
		{
			string text = HttpContext.Current.Request.Headers["X-Nego-Capability"];
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			string[] array = text.Split(UserSettingsProvider.capabilityDelimiters, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text2 in array)
			{
				string item = text2.Trim();
				if (this.serverConfigCache.NegoExSSPNames.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsTenantNegoEnabled()
		{
			string text = (string)HttpContext.Current.Items["WLID-TenantNegoEnabled"];
			return !string.IsNullOrEmpty(text) && text.Equals("true", StringComparison.OrdinalIgnoreCase);
		}

		private bool IsClientNegoExCapable()
		{
			return this.IsTenantNegoEnabled() && this.IsClientLiveSspCapable() && this.CallerCapabilities.IsWindow7OrNewerClient;
		}

		private ADRecipient GetADRecipientForUser(OrganizationId organizationId, ADObjectId userId)
		{
			ADRecipient adRecipient = null;
			ExTraceGlobals.FrameworkTracer.TraceDebug<ADObjectId>(0L, "GetADRecipientForUser -- Performing global lookup of user \"{0}\"", userId);
			try
			{
				IRecipientSession recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 3340, "GetADRecipientForUser", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs");
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.TrackLatency(ServiceLatencyMetadata.RequestedUserADLatency, delegate()
				{
					adRecipient = recipientSession.FindByObjectGuid(userId.ObjectGuid);
				});
			}
			catch (NonUniqueRecipientException)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<ADObjectId>(0L, "GetADRecipientForUser -- AD lookup returned non-unique error for user \"{0}\"", userId);
			}
			catch (ADTransientException arg)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<ADObjectId, ADTransientException>(0L, "GetADRecipientForUser -- AD lookup returned transient error for user \"{0}\": {1}", userId, arg);
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<ADObjectId>(0L, "GetADRecipientForUser -- AD lookup did not find user \"{0}\"", userId);
			}
			return adRecipient;
		}

		private string GetArchiveSmtpAddress(ADUser user)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}@{1}", new object[]
			{
				user.ArchiveGuid.ToString("D"),
				user.ArchiveDomain.Domain
			});
		}

		private AlternateMailboxCollection GetAlternativeMailboxCollection(ADUser principalUser, string type)
		{
			AlternateMailboxCollection alternateMailboxCollection = new AlternateMailboxCollection();
			if (principalUser.RecipientType == RecipientType.UserMailbox)
			{
				ExchangePrincipal exchangePrincipal = Common.GetExchangePrincipal(principalUser);
				string text;
				string fallbackFqdn;
				if (exchangePrincipal.MailboxInfo.Location.ServerVersion >= Server.E15MinVersion)
				{
					text = ExchangeRpcClientAccess.CreatePersonalizedServer(principalUser.ExchangeGuid, principalUser.PrimarySmtpAddress.Domain);
					fallbackFqdn = ExchangeRpcClientAccess.CreatePersonalizedServer(principalUser.ArchiveGuid, principalUser.PrimarySmtpAddress.Domain);
				}
				else
				{
					Site site;
					LegacyDN legacyDN;
					if (!this.TryGetPreferredSite(exchangePrincipal, out site, out legacyDN))
					{
						legacyDN = LegacyDN.Parse(exchangePrincipal.MailboxInfo.Location.RpcClientAccessServerLegacyDn);
					}
					text = this.GetRpcClientAccessFqdn(new ServerId(legacyDN.ToString()));
					fallbackFqdn = text;
				}
				AlternateMailbox item = new AlternateMailbox
				{
					Type = type,
					DisplayName = principalUser.DisplayName,
					LegacyDN = principalUser.LegacyExchangeDN,
					Server = text,
					SmtpAddress = principalUser.PrimarySmtpAddress.ToString(),
					OwnerSmtpAddress = principalUser.PrimarySmtpAddress.ToString()
				};
				alternateMailboxCollection.Add(item);
				if (principalUser.ArchiveGuid != Guid.Empty && principalUser.ArchiveDomain == null)
				{
					alternateMailboxCollection.Add(this.GetIntraForestArchiveMailbox(principalUser, fallbackFqdn));
				}
			}
			else if (principalUser.RecipientType == RecipientType.MailUser && null != principalUser.ExternalEmailAddress)
			{
				AlternateMailbox item2 = new AlternateMailbox
				{
					Type = type,
					DisplayName = principalUser.DisplayName,
					SmtpAddress = principalUser.ExternalEmailAddress.AddressString,
					OwnerSmtpAddress = principalUser.PrimarySmtpAddress.ToString()
				};
				alternateMailboxCollection.Add(item2);
			}
			else
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string, string>((long)this.GetHashCode(), "Cannot process the alternate mailbox {0} of type {1}", principalUser.DistinguishedName, principalUser.RecipientTypeDetails.ToString());
			}
			return alternateMailboxCollection;
		}

		private bool IsTeamMailboxEnabledConnection(ExchangePrincipal ep)
		{
			return ep.MailboxInfo.Location.ServerVersion >= Server.E15MinVersion && (!(this.CallerCapabilities.OutlookClientVersion != null) || this.CallerCapabilities.OutlookClientVersion.Major >= 15);
		}

		private bool TryGetPreferredSite(ExchangePrincipal exchangePrincipal, out Site preferredSite, out LegacyDN rpcClientAccessServerLegacyDN)
		{
			preferredSite = null;
			rpcClientAccessServerLegacyDN = null;
			if (exchangePrincipal.MailboxInfo.Location.RpcClientAccessServerLegacyDn == null)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string>((long)this.GetHashCode(), "RpcClientAccessServerLegacyDistinguishedName is null. User = {0}", exchangePrincipal.Alias);
				return false;
			}
			ADObjectId adobjectId = null;
			ActiveManager cachingActiveManagerInstance = ActiveManager.GetCachingActiveManagerInstance();
			cachingActiveManagerInstance.CalculatePreferredHomeServer(exchangePrincipal.MailboxInfo.GetDatabaseGuid(), out rpcClientAccessServerLegacyDN, out adobjectId);
			if (adobjectId == null)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string>((long)this.GetHashCode(), "Cannot find RpcClientAccess server or array by {0}.", exchangePrincipal.MailboxInfo.Location.RpcClientAccessServerLegacyDn);
				return false;
			}
			ADSite configFromSourceObject = this.serverConfigCache.SiteCache.GetConfigFromSourceObject(adobjectId);
			if (configFromSourceObject == null)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<ADObjectId>((long)this.GetHashCode(), "Cannot find ADSite from our cache for {0}.", adobjectId);
				return false;
			}
			preferredSite = new Site(new TopologySite(configFromSourceObject));
			return true;
		}

		private AlternateMailbox GetIntraForestArchiveMailbox(ADUser principalUser, string fallbackFqdn)
		{
			string domain = null;
			AlternateMailbox result;
			if (this.ShouldReturnArchiveBeSmtpAddress(principalUser) && this.TryGetArchiveDomain(out domain))
			{
				result = new AlternateMailbox
				{
					Type = "Archive",
					DisplayName = AutodiscoverCommonUserSettings.GetArchiveDisplayName(principalUser),
					SmtpAddress = SmtpProxyAddress.EncapsulateExchangeGuid(domain, principalUser.ArchiveGuid),
					OwnerSmtpAddress = principalUser.PrimarySmtpAddress.ToString()
				};
			}
			else
			{
				result = new AlternateMailbox
				{
					Type = "Archive",
					DisplayName = AutodiscoverCommonUserSettings.GetArchiveDisplayName(principalUser),
					LegacyDN = principalUser.GetAlternateMailboxLegDN(principalUser.ArchiveGuid),
					Server = fallbackFqdn,
					OwnerSmtpAddress = principalUser.PrimarySmtpAddress.ToString()
				};
			}
			return result;
		}

		private bool ShouldReturnArchiveBeSmtpAddress(ADUser principalUser)
		{
			if (!this.doesClientUnderstandMapiHttp)
			{
				return false;
			}
			ExchangePrincipal exchangePrincipal = Common.GetExchangePrincipal(principalUser);
			VariantConfigurationSnapshot configuration = exchangePrincipal.GetConfiguration();
			bool result;
			if (this.serverConfigCache.EnableMapiHttpAutodiscover != null)
			{
				result = this.serverConfigCache.EnableMapiHttpAutodiscover.Value;
			}
			else if (configuration.Autodiscover.UseMapiHttpADSetting.Enabled)
			{
				if (this.recipient.MapiHttpEnabled != null)
				{
					result = this.recipient.MapiHttpEnabled.Value;
				}
				else
				{
					Organization organization = OrganizationCache.Singleton.Get(principalUser.OrganizationId);
					result = organization.MapiHttpEnabled;
				}
			}
			else
			{
				result = configuration.Autodiscover.MapiHttp.Enabled;
			}
			return result;
		}

		private bool TryGetArchiveDomain(out string domain)
		{
			domain = null;
			string[] array = this.emailAddress.Split(new char[]
			{
				'@'
			});
			if (array.Length == 2)
			{
				domain = array[1];
				return true;
			}
			return false;
		}

		private const string EwsSupportedSchemas = "Exchange2007, Exchange2007_SP1, Exchange2010, Exchange2010_SP1, Exchange2010_SP2, Exchange2013, Exchange2013_SP1";

		private const string EcpDeliveryReportFragment = "PersonalSettings/DeliveryReport.aspx?rfr=olk&exsvurl=1&IsOWA=<IsOWA>&MsgID=<MsgID>&Mbx=<Mbx>";

		private const string EcpEmailSubscriptionsFragment = "?rfr=olk&p=personalsettings/EmailSubscriptions.slab&exsvurl=1";

		private const string EcpRetentionPolicyTagsFragment = "?rfr=olk&p=organize/retentionpolicytags.slab&exsvurl=1";

		private const string EcpTextMessagingFragment = "?rfr=olk&p=sms/textmessaging.slab&exsvurl=1";

		private const string EcpVoicemailFragment = "?rfr=olk&p=customize/voicemail.aspx&exsvurl=1";

		private const string EcpPublishingFragment = "customize/calendarpublishing.slab?rfr=olk&exsvurl=1&FldID=<FldID>";

		private const string EcpPhotoFragment = "PersonalSettings/EditAccount.aspx?rfr=olk&chgPhoto=1&exsvurl=1";

		private const string EcpConnectFragment = "Connect/Main.aspx?rfr=olk&exsvurl=1&Provider=<Provider>&Action=<Action>";

		private const string EcpTeamMailboxFragment = "?rfr=olk&ftr=TeamMailbox&exsvurl=1";

		private const string EcpTeamMailboxCreatingFragment = "?rfr=olk&ftr=TeamMailboxCreating&SPUrl=<SPUrl>&Title=<Title>&SPTMAppUrl=<SPTMAppUrl>&exsvurl=1";

		private const string EcpTeamMailboxEditingFragment = "?rfr=olk&ftr=TeamMailboxEditing&Id=<Id>&exsvurl=1";

		private const string EcpExtensionInstallationFragment = "Extension/InstalledExtensions.slab?rfr=olk&exsvurl=1";

		private const string UM12LegacyASMXFile = "UM2007Legacy.asmx";

		private const string TenantNegoEnabled = "WLID-TenantNegoEnabled";

		private const string PathToPhotosEndpointRelativeToEws = "/s";

		private const string ArchiveAlternateMailboxType = "Archive";

		private static readonly ProxyAddressPrefix autodiscoverPrefix = new CustomProxyAddressPrefix("AUTOD");

		private static readonly Random randomGenerator = new Random();

		private static readonly char[] capabilityDelimiters = new char[]
		{
			','
		};

		private static readonly bool isMSOnline = VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.AccountInCloud.Enabled;

		private readonly bool useClientCertificateAuthentication;

		private static int expensiveRequestCount;

		private static IConfigurationSession rootOrgSystemConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 183, ".cctor", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationSettings\\UserSettingsProvider.cs");

		private static string orgGuidString;

		private static string casVersionString;

		private static Dictionary<UserConfigurationSettingName, string> ecpInternalSettings;

		private static Dictionary<UserConfigurationSettingName, string> ecpExternalSettings;

		private static Dictionary<string, ADPropertyDefinition> extensionAttributes;

		private static HashSet<UserConfigurationSettingName> serviceEndpointSettings;

		private static HashSet<UserConfigurationSettingName> serviceEndpointSettingsWithSiteMailboxCreation;

		private ADRecipient recipient;

		private string emailAddress;

		private ExchangeServerVersion? requestedVersion;

		private ServerConfigurationCache serverConfigCache;

		private readonly bool doesClientUnderstandMapiHttp;

		internal delegate bool IncludeServiceDelegate<T>(T service);

		internal delegate bool IsValidCasVersionDelegate(ExchangeServerVersion casVersion, ClientAccessType clientAccessType, ExchangeServerVersion mailboxVersion);
	}
}
