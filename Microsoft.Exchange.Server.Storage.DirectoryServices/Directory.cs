using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.DirectoryServices;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public class Directory : DirectoryBase, IRpcProxyDirectory
	{
		static Directory()
		{
			if (Microsoft.Exchange.Data.Directory.Globals.InstanceType == InstanceType.NotInitialized)
			{
				Microsoft.Exchange.Data.Directory.Globals.InitializeSinglePerfCounterInstance();
			}
		}

		private Directory(ICachePerformanceCounters mailboxCacheCounters, ICachePerformanceCounters foreignMailboxCacheCounters, ICachePerformanceCounters addressCacheCounters, ICachePerformanceCounters foreignAddressCacheCounters, ICachePerformanceCounters databaseCacheCounters, ICachePerformanceCounters orgContainerCacheCounters, ICachePerformanceCounters distributionListMembershipCacheCounters)
		{
			EvictionPolicy<Guid> evictionPolicy = new LRU2WithTimeToLiveExpirationPolicy<Guid>(ConfigurationSchema.MailboxInfoCacheSize.Value, ConfigurationSchema.MailboxInfoCacheTTL.Value, true);
			this.oursMailboxInfoCache = new Directory.MailboxInfoCache("MailboxInfoOurs", this.mailboxInfoLock, evictionPolicy, mailboxCacheCounters);
			EvictionPolicy<Guid> evictionPolicy2 = new LRU2WithTimeToLiveExpirationPolicy<Guid>(ConfigurationSchema.ForeignMailboxInfoCacheSize.Value, ConfigurationSchema.ForeignMailboxInfoCacheTTL.Value, true);
			this.foreignMailboxInfoCache = new Directory.MailboxInfoCache("MailboxInfoForeign", this.mailboxInfoLock, evictionPolicy2, foreignMailboxCacheCounters);
			EvictionPolicy<Guid> evictionPolicy3 = new LRU2WithTimeToLiveExpirationPolicy<Guid>(ConfigurationSchema.AddressInfoCacheSize.Value, ConfigurationSchema.AddressInfoCacheTTL.Value, true);
			this.oursAddressInfoCache = new Directory.AddressInfoCache("AddressInfoOurs", this.addressInfoLock, evictionPolicy3, addressCacheCounters);
			EvictionPolicy<Guid> evictionPolicy4 = new LRU2WithTimeToLiveExpirationPolicy<Guid>(ConfigurationSchema.ForeignAddressInfoCacheSize.Value, ConfigurationSchema.ForeignAddressInfoCacheTTL.Value, true);
			this.foreignAddressInfoCache = new Directory.AddressInfoCache("AddressInfoForeign", this.addressInfoLock, evictionPolicy4, foreignAddressCacheCounters);
			EvictionPolicy<Guid> evictionPolicy5 = new LRU2WithTimeToLiveExpirationPolicy<Guid>(ConfigurationSchema.DatabaseInfoCacheSize.Value, ConfigurationSchema.DatabaseInfoCacheTTL.Value, true);
			this.databaseInfoCache = new Directory.DatabaseInfoCache("DatabaseInfo", this.databaseInfoLock, evictionPolicy5, databaseCacheCounters);
			EvictionPolicy<OrganizationId> evictionPolicy6 = new LRU2WithTimeToLiveExpirationPolicy<OrganizationId>(ConfigurationSchema.OrganizationContainerCacheSize.Value, ConfigurationSchema.OrganizationContainerCacheTTL.Value, true);
			this.organizationContainerCache = new Directory.OrganizationContainerCache("OrganizationContainer", this.organizationContainerCacheLock, evictionPolicy6, orgContainerCacheCounters);
			this.serverInfoCache = new Directory.SingletonDirectoryCache<ADObjectWrappers.IADServer, ServerInfo>((DateTime created) => DateTime.UtcNow.Subtract(created) <= ConfigurationSchema.ServerInfoCacheTTL.Value, new Func<IExecutionContext, ADObjectWrappers.IADServer>(this.AdConfigSession.FindLocalServer), (IExecutionContext context, ADObjectWrappers.IADServer adServer) => Directory.CreateServerInfo(context, adServer, (adServer != null) ? this.AdConfigSession.FindLocalInformationStore(context, adServer) : null));
			this.transportInfoCache = new Directory.SingletonDirectoryCache<ADObjectWrappers.IADTransportConfigContainer>((DateTime created) => DateTime.UtcNow.Subtract(created) <= ConfigurationSchema.TransportInfoCacheTTL.Value, new Func<IExecutionContext, ADObjectWrappers.IADTransportConfigContainer>(this.AdConfigSession.FindTransportConfigContainer));
			this.distributionListMembershipCache = new SingleKeyCache<Directory.DistributionListMembershipKey, bool?>(new SimpleTimeToLiveExpirationPolicy<Directory.DistributionListMembershipKey>(500, TimeSpan.FromMinutes(15.0), true), distributionListMembershipCacheCounters);
			StoreQueryTargets.Register<AddressInfo>(this.oursAddressInfoCache, Visibility.Redacted);
			StoreQueryTargets.Register<AddressInfo>(this.foreignAddressInfoCache, Visibility.Redacted);
			StoreQueryTargets.Register<DatabaseInfo>(this.databaseInfoCache, Visibility.Public);
			StoreQueryTargets.Register<MailboxInfo>(this.oursMailboxInfoCache, Visibility.Redacted);
			StoreQueryTargets.Register<MailboxInfo>(this.foreignMailboxInfoCache, Visibility.Redacted);
			StoreQueryTargets.Register<ADObjectWrappers.IADOrganizationContainer>(this.organizationContainerCache, Visibility.Public);
			StoreHelper.SetStoreUserInformationReader(Directory.StoreUserInformationReader.Instance);
		}

		internal Directory.MailboxInfoCache OursMailboxInfoCacheTestOnly
		{
			get
			{
				return this.oursMailboxInfoCache;
			}
		}

		internal Directory.MailboxInfoCache ForeignMailboxInfoCacheTestOnly
		{
			get
			{
				return this.foreignMailboxInfoCache;
			}
		}

		internal Directory.AddressInfoCache OursAddressInfoCacheTestOnly
		{
			get
			{
				return this.oursAddressInfoCache;
			}
		}

		internal Directory.AddressInfoCache ForeignAddressInfoCacheTestOnly
		{
			get
			{
				return this.foreignAddressInfoCache;
			}
		}

		internal Directory.OrganizationContainerCache OrganizationContainerCacheTestOnly
		{
			get
			{
				return this.organizationContainerCache;
			}
		}

		internal Directory.DatabaseInfoCache DatabaseInfoCacheTestOnly
		{
			get
			{
				return this.databaseInfoCache;
			}
		}

		private ADObjectWrappers.IADSystemConfigurationSession AdConfigSession
		{
			get
			{
				if (this.adConfigSession == null)
				{
					this.adConfigSession = ADObjectWrappers.CreateADSystemConfigurationSession(NullExecutionContext.Instance, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), null);
				}
				return this.adConfigSession;
			}
		}

		public static void InitializeConfiguration()
		{
			ConfigurationSchema.Initialize();
			IConfigDriver configDriver = new ADConfigDriver(((StoreConfigProvider)StoreConfigContext.ConfigProvider).Schema);
			configDriver.Initialize();
			StoreConfigProvider.Default.AddConfigDriver(configDriver);
			SettingOverrideSync.Instance.Start(true);
		}

		public static void TerminateConfiguration()
		{
			SettingOverrideSync.Instance.Stop();
		}

		public static IDirectory Create(ICachePerformanceCounters mailboxCacheCounters, ICachePerformanceCounters foreignMailboxCacheCounters, ICachePerformanceCounters addressCacheCounters, ICachePerformanceCounters foreignAddressCacheCounters, ICachePerformanceCounters databaseCacheCounters, ICachePerformanceCounters orgContainerCacheCounters, ICachePerformanceCounters distributionListMembershipCacheCounters)
		{
			return new Directory(mailboxCacheCounters, foreignMailboxCacheCounters, addressCacheCounters, foreignAddressCacheCounters, databaseCacheCounters, orgContainerCacheCounters, distributionListMembershipCacheCounters);
		}

		public static UnlimitedBytes UnlimitedBytesFromByteQuantifiedSize(Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> byteQuantifiedSize)
		{
			if (byteQuantifiedSize.IsUnlimited)
			{
				return UnlimitedBytes.UnlimitedValue;
			}
			return new UnlimitedBytes((long)byteQuantifiedSize.Value.ToBytes());
		}

		SecurityDescriptor IRpcProxyDirectory.GetDatabaseSecurityDescriptor(IExecutionContext context, Guid databaseGuid)
		{
			return this.GetDatabaseInfoImplInternal(context, databaseGuid).NTSecurityDescriptor;
		}

		SecurityDescriptor IRpcProxyDirectory.GetServerSecurityDescriptor(IExecutionContext context)
		{
			return this.GetServerInfoImplInternal(context).NTSecurityDescriptor;
		}

		void IRpcProxyDirectory.RefreshDatabaseInfo(IExecutionContext context, Guid databaseGuid)
		{
			this.RefreshDatabaseInfoImpl(null, databaseGuid);
		}

		void IRpcProxyDirectory.RefreshServerInfo(IExecutionContext context)
		{
			this.RefreshServerInfoImplInternal(context);
		}

		int? IRpcProxyDirectory.GetMaximumRpcThreadCount(IExecutionContext context)
		{
			return this.GetServerInfoImplInternal(context).MaxRpcThreads;
		}

		DatabaseInfo IRpcProxyDirectory.GetDatabaseInfo(IExecutionContext context, Guid databaseGuid)
		{
			return this.GetDatabaseInfoImplInternal(context, databaseGuid);
		}

		internal static void SetGetOrgTestHooks(Action preEnterHook, Action postEnterHook)
		{
			Directory.getOrgContainerAssignmentLockPreEnterTestHook = preEnterHook;
			Directory.getOrgContainerAssignmentLockPostEnterTestHook = postEnterHook;
		}

		internal static DatabaseInfo CreateDatabaseInfo(IExecutionContext context, ADObjectWrappers.IADMailboxDatabase database, ServerInfo serverInfo, int totalDatabases, int? maxActiveDatabases, string forestName)
		{
			string filePath = Directory.GetFilePath(database.EdbFilePath);
			string filePath2 = (filePath == null) ? null : Path.GetDirectoryName(filePath);
			string fileName = (filePath == null) ? null : Path.GetFileName(filePath);
			DatabaseOptions databaseOptions = serverInfo.DatabaseOptions.Clone();
			database.LoadDatabaseOptions(context, databaseOptions);
			databaseOptions.TotalDatabasesOnServer = new int?(totalDatabases);
			databaseOptions.MaxActiveDatabases = maxActiveDatabases;
			SecurityDescriptor securityDescriptor = database.ReadSecurityDescriptor(context);
			if (securityDescriptor == null || securityDescriptor.BinaryForm == null)
			{
				DiagnosticContext.TraceDwordAndString((LID)56448U, 0U, database.Guid.ToString());
				throw new DirectoryInfoCorruptException((LID)44160U, "Database AD object is missing OS SecurityDescriptor");
			}
			QuotaInfo quotaInfo = new QuotaInfo(Directory.UnlimitedBytesFromByteQuantifiedSize(database.IssueWarningQuota), Directory.UnlimitedBytesFromByteQuantifiedSize(database.ProhibitSendQuota), Directory.UnlimitedBytesFromByteQuantifiedSize(database.ProhibitSendReceiveQuota), Directory.UnlimitedBytesFromByteQuantifiedSize(database.RecoverableItemsWarningQuota), Directory.UnlimitedBytesFromByteQuantifiedSize(database.RecoverableItemsQuota));
			return new DatabaseInfo(database.Guid, database.Name, database.DagOrServerGuid, filePath2, fileName, Directory.GetFilePath(database.LogFolderPath), database.ExchangeLegacyDN, database.EventHistoryRetentionPeriod, false, database.Recovery, database.Description, database.ServerName, securityDescriptor, database.CircularLoggingEnabled, database.AllowFileRestore, database.MailboxRetention, database.HostServerNames, databaseOptions, quotaInfo, database.DataMoveReplicationConstraint, forestName);
		}

		internal static ServerInfo CreateServerInfo(IExecutionContext context, ADObjectWrappers.IADServer server, ADObjectWrappers.IADInformationStore informationStore)
		{
			if (server != null)
			{
				Directory.CheckADObjectIsNotCorrupt((LID)33336U, context, server.InstallPath != null, "InstallPath attribute is null for local server object {0}", server.ExchangeLegacyDN);
				Directory.CheckADObjectIsNotCorrupt((LID)58873U, context, informationStore != null, "InformationStore child object does not exist for local server object {0}", server.ExchangeLegacyDN);
				DatabaseOptions databaseOptions = new DatabaseOptions();
				informationStore.LoadDatabaseOptions(context, databaseOptions);
				return new ServerInfo(server.Fqdn, server.Guid, server.ExchangeLegacyDN, server.InstallPath.ToString(), server.ReadSecurityDescriptor(context), server.ServerRole != ServerRole.Mailbox, informationStore.MaxRpcThreads, server.ContinuousReplicationMaxMemoryPerDatabase, server.MaxActiveDatabases, server.Edition, informationStore.MaxRecoveryDatabases, informationStore.MaxTotalDatabases, server.IsDAGMember, server.Forest, databaseOptions);
			}
			return null;
		}

		internal static MailboxInfo CreateMailboxInfo(IExecutionContext context, ADRecipient adRecipient, MailboxLocationType mailboxLocationType, Guid mailboxGuid, ADObjectWrappers.IADTransportConfigContainer transportConfig, ADObjectWrappers.IADOrganizationContainer organizationContainer, bool tenantMailbox, Guid externalDirectoryOrganizationId, out bool ours)
		{
			Guid mdbGuid = Guid.Empty;
			IADSecurityPrincipal iadsecurityPrincipal = adRecipient as IADSecurityPrincipal;
			SecurityIdentifier userSid = null;
			SecurityIdentifier[] userSidHistory = Array<SecurityIdentifier>.Empty;
			if (iadsecurityPrincipal != null)
			{
				userSid = iadsecurityPrincipal.Sid;
				if (iadsecurityPrincipal.SidHistory != null)
				{
					userSidHistory = iadsecurityPrincipal.SidHistory.ToArray();
				}
			}
			string legacyExchangeDN = adRecipient.LegacyExchangeDN;
			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> byteQuantifiedSize = Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> byteQuantifiedSize2 = Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			Guid? partitionGuid = null;
			Guid ownerGuid = Directory.ExtractRecipientObjectId(adRecipient);
			bool flag = mailboxLocationType == MailboxLocationType.MainArchive || mailboxLocationType == MailboxLocationType.AuxArchive;
			QuotaStyle quotaStyle = QuotaStyle.NoQuota;
			int rulesQuota = 0;
			Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType = adRecipient.RecipientType;
			RawSecurityDescriptor exchangeSecurityDescriptor;
			QuotaInfo quotaInfo;
			switch (recipientType)
			{
			case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.User:
			case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox:
			case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUser:
			{
				ADUser aduser = adRecipient as ADUser;
				if (aduser == null)
				{
					throw new DirectoryInfoCorruptException((LID)39324U, string.Format("RecipientType {0} is invalid for MailboxGuid {1}", adRecipient.RecipientType, mailboxGuid));
				}
				exchangeSecurityDescriptor = aduser.ExchangeSecurityDescriptor;
				partitionGuid = aduser.MailboxContainerGuid;
				if (flag)
				{
					quotaStyle = QuotaStyle.UseSpecificValues;
				}
				else if (aduser.UseDatabaseQuotaDefaults != null && aduser.UseDatabaseQuotaDefaults.Value)
				{
					quotaStyle = QuotaStyle.UseDatabaseDefault;
				}
				else
				{
					quotaStyle = QuotaStyle.UseSpecificValues;
				}
				quotaInfo = new QuotaInfo(Directory.UnlimitedBytesFromByteQuantifiedSize(flag ? aduser.ArchiveWarningQuota : aduser.IssueWarningQuota), Directory.UnlimitedBytesFromByteQuantifiedSize(flag ? aduser.ArchiveQuota : aduser.ProhibitSendQuota), Directory.UnlimitedBytesFromByteQuantifiedSize(flag ? aduser.ArchiveQuota : aduser.ProhibitSendReceiveQuota), Directory.UnlimitedBytesFromByteQuantifiedSize(aduser.RecoverableItemsWarningQuota), Directory.UnlimitedBytesFromByteQuantifiedSize(aduser.RecoverableItemsQuota));
				rulesQuota = (int)((double)aduser.RulesQuota);
				IMailboxLocationCollection mailboxLocations = aduser.MailboxLocations;
				IMailboxLocationInfo mailboxLocationInfo = (mailboxLocationType == MailboxLocationType.Aggregated) ? mailboxLocations.GetMailboxLocation(MailboxLocationType.Primary) : mailboxLocations.GetMailboxLocation(mailboxGuid);
				bool flag2 = mailboxLocationInfo != null;
				if (flag2)
				{
					mdbGuid = ((mailboxLocationInfo.DatabaseLocation != null) ? mailboxLocationInfo.DatabaseLocation.ObjectGuid : Guid.Empty);
				}
				else if (mailboxLocationType == MailboxLocationType.Aggregated || mailboxLocationType == MailboxLocationType.Primary || flag)
				{
					if (flag)
					{
						mdbGuid = ((aduser.ArchiveDatabase != null) ? aduser.ArchiveDatabase.ObjectGuid : Guid.Empty);
					}
					else
					{
						mdbGuid = ((aduser.Database != null) ? aduser.Database.ObjectGuid : Guid.Empty);
					}
				}
				byteQuantifiedSize = aduser.MaxSendSize;
				byteQuantifiedSize2 = aduser.MaxReceiveSize;
				if (byteQuantifiedSize.IsUnlimited)
				{
					byteQuantifiedSize = transportConfig.MaxSendSize;
				}
				if (byteQuantifiedSize2.IsUnlimited)
				{
					byteQuantifiedSize2 = transportConfig.MaxReceiveSize;
				}
				break;
			}
			default:
				switch (recipientType)
				{
				case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.SystemAttendantMailbox:
				{
					ADSystemAttendantMailbox adsystemAttendantMailbox = adRecipient as ADSystemAttendantMailbox;
					if (adsystemAttendantMailbox == null)
					{
						throw new DirectoryInfoCorruptException((LID)55708U, string.Format("RecipientType {0} is invalid for MailboxGuid {1}.", adRecipient.RecipientType, mailboxGuid));
					}
					exchangeSecurityDescriptor = adsystemAttendantMailbox.ExchangeSecurityDescriptor;
					mdbGuid = Guid.Empty;
					quotaInfo = QuotaInfo.Unlimited;
					break;
				}
				case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.SystemMailbox:
				{
					ADSystemMailbox adsystemMailbox = adRecipient as ADSystemMailbox;
					if (adsystemMailbox == null)
					{
						throw new DirectoryInfoCorruptException((LID)43420U, string.Format("RecipientType {0} is invalid for MailboxGuid {1}.", adRecipient.RecipientType, mailboxGuid));
					}
					exchangeSecurityDescriptor = adsystemMailbox.ExchangeSecurityDescriptor;
					mdbGuid = adsystemMailbox.Database.ObjectGuid;
					quotaInfo = QuotaInfo.Unlimited;
					break;
				}
				case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MicrosoftExchange:
				{
					ADMicrosoftExchangeRecipient admicrosoftExchangeRecipient = adRecipient as ADMicrosoftExchangeRecipient;
					if (admicrosoftExchangeRecipient == null)
					{
						throw new DirectoryInfoCorruptException((LID)59804U, string.Format("RecipientType {0} is invalid for MailboxGuid {1}.", adRecipient.RecipientType, mailboxGuid));
					}
					exchangeSecurityDescriptor = admicrosoftExchangeRecipient.ExchangeSecurityDescriptor;
					mdbGuid = Guid.Empty;
					quotaInfo = QuotaInfo.Unlimited;
					break;
				}
				default:
					throw Directory.CreateUnsupportedRecipientTypeException((LID)35384U, adRecipient);
				}
				break;
			}
			UnlimitedBytes orgWidePublicFolderWarningQuota = UnlimitedBytes.UnlimitedValue;
			UnlimitedBytes orgWidePublicFolderProhibitPostQuota = UnlimitedBytes.UnlimitedValue;
			UnlimitedBytes orgWidePublicFolderMaxItemSize = UnlimitedBytes.UnlimitedValue;
			if (adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Invalid && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.User && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUser && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Contact && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailContact && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Group && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.DynamicDistributionGroup && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicFolder && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicDatabase && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.SystemAttendantMailbox && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.SystemMailbox && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MicrosoftExchange && adRecipient.RecipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Computer)
			{
				DiagnosticContext.TraceDword((LID)53084U, (uint)adRecipient.RecipientType);
				DiagnosticContext.TraceGuid((LID)47964U, mailboxGuid);
				throw new DirectoryInfoCorruptException((LID)61276U, string.Format("Unsupported RecipientType {0} for MailboxGuid {1}", adRecipient.RecipientType, mailboxGuid));
			}
			MailboxInfo.MailboxType mailboxType = MailboxInfo.MailboxType.Private;
			bool isDiscoveryMailbox = false;
			MailboxInfo.MailboxTypeDetail mailboxTypeDetail = MailboxInfo.MailboxTypeDetail.None;
			if (mailboxType == MailboxInfo.MailboxType.Private)
			{
				RecipientTypeDetails recipientTypeDetails = adRecipient.RecipientTypeDetails;
				if (recipientTypeDetails <= RecipientTypeDetails.DisabledUser)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.DynamicDistributionGroup)
					{
						if (recipientTypeDetails <= RecipientTypeDetails.MailContact)
						{
							if (recipientTypeDetails <= RecipientTypeDetails.RoomMailbox)
							{
								if (recipientTypeDetails <= RecipientTypeDetails.LegacyMailbox)
								{
									if (recipientTypeDetails < RecipientTypeDetails.None)
									{
										goto IL_820;
									}
									switch ((int)recipientTypeDetails)
									{
									case 0:
									case 1:
									case 2:
									case 8:
										goto IL_816;
									case 3:
									case 5:
									case 6:
									case 7:
										goto IL_820;
									case 4:
										mailboxTypeDetail = MailboxInfo.MailboxTypeDetail.SharedMailbox;
										goto IL_870;
									}
								}
								if (recipientTypeDetails != RecipientTypeDetails.RoomMailbox)
								{
									goto IL_820;
								}
							}
							else if (recipientTypeDetails != RecipientTypeDetails.EquipmentMailbox && recipientTypeDetails != RecipientTypeDetails.MailContact)
							{
								goto IL_820;
							}
						}
						else if (recipientTypeDetails <= RecipientTypeDetails.MailUniversalDistributionGroup)
						{
							if (recipientTypeDetails != RecipientTypeDetails.MailUser && recipientTypeDetails != RecipientTypeDetails.MailUniversalDistributionGroup)
							{
								goto IL_820;
							}
						}
						else if (recipientTypeDetails != RecipientTypeDetails.MailNonUniversalGroup && recipientTypeDetails != RecipientTypeDetails.MailUniversalSecurityGroup && recipientTypeDetails != RecipientTypeDetails.DynamicDistributionGroup)
						{
							goto IL_820;
						}
					}
					else if (recipientTypeDetails <= RecipientTypeDetails.User)
					{
						if (recipientTypeDetails <= RecipientTypeDetails.SystemAttendantMailbox)
						{
							if (recipientTypeDetails != RecipientTypeDetails.PublicFolder && recipientTypeDetails != RecipientTypeDetails.SystemAttendantMailbox)
							{
								goto IL_820;
							}
						}
						else if (recipientTypeDetails != RecipientTypeDetails.SystemMailbox && recipientTypeDetails != RecipientTypeDetails.MailForestContact && recipientTypeDetails != RecipientTypeDetails.User)
						{
							goto IL_820;
						}
					}
					else if (recipientTypeDetails <= RecipientTypeDetails.UniversalDistributionGroup)
					{
						if (recipientTypeDetails != RecipientTypeDetails.Contact && recipientTypeDetails != RecipientTypeDetails.UniversalDistributionGroup)
						{
							goto IL_820;
						}
					}
					else if (recipientTypeDetails != RecipientTypeDetails.UniversalSecurityGroup && recipientTypeDetails != RecipientTypeDetails.NonUniversalGroup && recipientTypeDetails != RecipientTypeDetails.DisabledUser)
					{
						goto IL_820;
					}
				}
				else if (recipientTypeDetails <= RecipientTypeDetails.Computer)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.LinkedUser)
					{
						if (recipientTypeDetails <= RecipientTypeDetails.ArbitrationMailbox)
						{
							if (recipientTypeDetails != RecipientTypeDetails.MicrosoftExchange && recipientTypeDetails != RecipientTypeDetails.ArbitrationMailbox)
							{
								goto IL_820;
							}
						}
						else if (recipientTypeDetails != RecipientTypeDetails.MailboxPlan && recipientTypeDetails != RecipientTypeDetails.LinkedUser)
						{
							goto IL_820;
						}
					}
					else if (recipientTypeDetails <= RecipientTypeDetails.DiscoveryMailbox)
					{
						if (recipientTypeDetails != RecipientTypeDetails.RoomList)
						{
							if (recipientTypeDetails != RecipientTypeDetails.DiscoveryMailbox)
							{
								goto IL_820;
							}
							isDiscoveryMailbox = true;
						}
					}
					else if (recipientTypeDetails != RecipientTypeDetails.RoleGroup && recipientTypeDetails != (RecipientTypeDetails)((ulong)-2147483648) && recipientTypeDetails != RecipientTypeDetails.Computer)
					{
						goto IL_820;
					}
				}
				else if (recipientTypeDetails <= RecipientTypeDetails.TeamMailbox)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.RemoteEquipmentMailbox)
					{
						if (recipientTypeDetails != RecipientTypeDetails.RemoteRoomMailbox && recipientTypeDetails != RecipientTypeDetails.RemoteEquipmentMailbox)
						{
							goto IL_820;
						}
					}
					else if (recipientTypeDetails != RecipientTypeDetails.RemoteSharedMailbox)
					{
						if (recipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
						{
							Guid hierarchyMailboxGuid = organizationContainer.HierarchyMailboxGuid;
							mailboxType = ((mailboxGuid == hierarchyMailboxGuid) ? MailboxInfo.MailboxType.PublicFolderPrimary : MailboxInfo.MailboxType.PublicFolderSecondary);
							orgWidePublicFolderWarningQuota = Directory.UnlimitedBytesFromByteQuantifiedSize(organizationContainer.DefaultPublicFolderIssueWarningQuota);
							orgWidePublicFolderProhibitPostQuota = Directory.UnlimitedBytesFromByteQuantifiedSize(organizationContainer.DefaultPublicFolderProhibitPostQuota);
							orgWidePublicFolderMaxItemSize = Directory.UnlimitedBytesFromByteQuantifiedSize(organizationContainer.DefaultPublicFolderMaxItemSize);
							goto IL_870;
						}
						if (recipientTypeDetails != RecipientTypeDetails.TeamMailbox)
						{
							goto IL_820;
						}
						mailboxTypeDetail = MailboxInfo.MailboxTypeDetail.TeamMailbox;
						goto IL_870;
					}
				}
				else if (recipientTypeDetails <= RecipientTypeDetails.MonitoringMailbox)
				{
					if (recipientTypeDetails != RecipientTypeDetails.RemoteTeamMailbox && recipientTypeDetails != RecipientTypeDetails.MonitoringMailbox)
					{
						goto IL_820;
					}
				}
				else
				{
					if (recipientTypeDetails == RecipientTypeDetails.GroupMailbox)
					{
						mailboxTypeDetail = MailboxInfo.MailboxTypeDetail.GroupMailbox;
						goto IL_870;
					}
					if (recipientTypeDetails != RecipientTypeDetails.LinkedRoomMailbox && recipientTypeDetails != RecipientTypeDetails.AuditLogMailbox)
					{
						goto IL_820;
					}
				}
				IL_816:
				mailboxTypeDetail = MailboxInfo.MailboxTypeDetail.UserMailbox;
				goto IL_870;
				IL_820:
				DiagnosticContext.TraceLong((LID)56156U, (ulong)adRecipient.RecipientTypeDetails);
				DiagnosticContext.TraceGuid((LID)39772U, mailboxGuid);
				throw new DirectoryInfoCorruptException((LID)36700U, string.Format("Unsupported RecipientTypeDetails {0} for MailboxGuid {1}", adRecipient.RecipientTypeDetails, mailboxGuid));
			}
			IL_870:
			bool flag3 = false;
			switch (mailboxLocationType)
			{
			case MailboxLocationType.Primary:
			case MailboxLocationType.MainArchive:
			case MailboxLocationType.Aggregated:
				break;
			case MailboxLocationType.AuxArchive:
			case MailboxLocationType.AuxPrimary:
				if (mailboxTypeDetail != MailboxInfo.MailboxTypeDetail.UserMailbox)
				{
					flag3 = true;
				}
				mailboxTypeDetail = MailboxInfo.MailboxTypeDetail.AuxArchiveMailbox;
				break;
			default:
				flag3 = true;
				break;
			}
			if (flag3)
			{
				throw new DirectoryInfoCorruptException((LID)41676U, string.Format("Unsupported mailboxLocationType {0} mapping for recipient type detail {1} for MailboxGuid {2}", mailboxLocationType, mailboxTypeDetail, mailboxGuid));
			}
			MultiValuedProperty<CultureInfo> multiValuedProperty = (MultiValuedProperty<CultureInfo>)adRecipient[ADUserSchema.Languages];
			int lcid = (multiValuedProperty.Count > 0) ? multiValuedProperty[0].LCID : 0;
			ours = (Storage.FindDatabase(mdbGuid) != null);
			return new MailboxInfo(mdbGuid, mailboxGuid, partitionGuid, tenantMailbox, flag, adRecipient.RecipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.SystemMailbox, adRecipient.RecipientTypeDetails == RecipientTypeDetails.MonitoringMailbox, isDiscoveryMailbox, mailboxType, mailboxTypeDetail, ownerGuid, adRecipient.LegacyExchangeDN, Directory.GetDisplayName(adRecipient, flag), adRecipient.SimpleDisplayName, adRecipient.DistinguishedName, adRecipient.RecipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MicrosoftExchange, adRecipient.RecipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.SystemAttendantMailbox, adRecipient.MasterAccountSid, userSid, userSidHistory, SecurityDescriptor.FromRawSecurityDescriptor(exchangeSecurityDescriptor), lcid, rulesQuota, Directory.UnlimitedBytesFromByteQuantifiedSize(byteQuantifiedSize), Directory.UnlimitedBytesFromByteQuantifiedSize(byteQuantifiedSize2), quotaStyle, quotaInfo, orgWidePublicFolderWarningQuota, orgWidePublicFolderProhibitPostQuota, orgWidePublicFolderMaxItemSize, externalDirectoryOrganizationId);
		}

		internal static AddressInfo CreateAddressInfo(IExecutionContext context, ADObjectWrappers.IADRecipientSession adRecipientSession, ADRecipient adRecipient, bool loadPublicDelegates, out bool ours)
		{
			SecurityIdentifier userSid = null;
			SecurityIdentifier[] userSidHistory = Array<SecurityIdentifier>.Empty;
			IADSecurityPrincipal iadsecurityPrincipal = adRecipient as IADSecurityPrincipal;
			if (iadsecurityPrincipal != null)
			{
				userSid = iadsecurityPrincipal.Sid;
				if (iadsecurityPrincipal.SidHistory != null)
				{
					userSidHistory = iadsecurityPrincipal.SidHistory.ToArray();
				}
			}
			else if (adRecipient is ADSystemMailbox)
			{
				userSid = Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.ProcessSecurityContext.UserSid;
			}
			string userOrgEmailAddr = null;
			string userOrgAddrType = null;
			if (adRecipient.OriginalPrimarySmtpAddress.IsValidAddress)
			{
				userOrgEmailAddr = adRecipient.OriginalPrimarySmtpAddress.ToString();
				userOrgAddrType = "SMTP";
			}
			else if (adRecipient.OriginalWindowsEmailAddress.IsValidAddress)
			{
				userOrgEmailAddr = adRecipient.OriginalWindowsEmailAddress.ToString();
				userOrgAddrType = "SMTP";
			}
			string userEmailAddress = null;
			string userAddressType = null;
			if (!string.IsNullOrEmpty(adRecipient.LegacyExchangeDN))
			{
				userEmailAddress = adRecipient.LegacyExchangeDN.ToUpper(CultureHelper.DefaultCultureInfo);
				userAddressType = "EX";
			}
			else
			{
				foreach (ProxyAddress proxyAddress in adRecipient.EmailAddresses)
				{
					if (proxyAddress.IsPrimaryAddress)
					{
						userEmailAddress = proxyAddress.AddressString;
						if (proxyAddress.Prefix.Equals(ProxyAddressPrefix.Smtp))
						{
							userAddressType = "SMTP";
						}
						else if (proxyAddress.Prefix.Equals(ProxyAddressPrefix.LegacyDN))
						{
							userAddressType = "EX";
						}
						else if (proxyAddress.Prefix.Equals(ProxyAddressPrefix.X400))
						{
							userAddressType = "X400";
						}
						else if (proxyAddress.Prefix.Equals(ProxyAddressPrefix.UM))
						{
							userAddressType = "EUM";
						}
					}
				}
			}
			string senderEmailAddress = Directory.GetSenderEmailAddress(adRecipient);
			IList<AddressInfo.PublicDelegate> list = null;
			MultiValuedProperty<ADObjectId> grantSendOnBehalfTo = adRecipient.GrantSendOnBehalfTo;
			if (grantSendOnBehalfTo.Count > 0)
			{
				if (!loadPublicDelegates)
				{
					goto IL_254;
				}
				list = new List<AddressInfo.PublicDelegate>(grantSendOnBehalfTo.Count);
				using (MultiValuedProperty<ADObjectId>.Enumerator enumerator2 = grantSendOnBehalfTo.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ADObjectId adobjectId = enumerator2.Current;
						ADRecipient adrecipient = adRecipientSession.FindByObjectGuid(context, adobjectId.ObjectGuid);
						if (adrecipient != null)
						{
							bool isDistributionList;
							switch (adrecipient.RecipientType)
							{
							case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup:
							case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup:
							case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup:
								isDistributionList = true;
								break;
							default:
								isDistributionList = false;
								break;
							}
							Guid objectId = Directory.ExtractRecipientObjectId(adrecipient);
							list.Add(new AddressInfo.PublicDelegate(adobjectId.DistinguishedName, objectId, isDistributionList));
						}
					}
					goto IL_254;
				}
			}
			list = Array<AddressInfo.PublicDelegate>.Empty;
			IL_254:
			bool isDistributionList2 = false;
			bool isMailPublicFolder = false;
			switch (adRecipient.RecipientType)
			{
			case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup:
			case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup:
			case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup:
			case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.DynamicDistributionGroup:
				isDistributionList2 = true;
				break;
			case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicFolder:
				isMailPublicFolder = true;
				break;
			default:
				isDistributionList2 = false;
				break;
			}
			bool hasMailbox = false;
			Guid cachingUseOnlyMailboxGuid = Guid.Empty;
			IADMailStorage iadmailStorage = adRecipient as IADMailStorage;
			if (iadmailStorage != null)
			{
				hasMailbox = (iadmailStorage.Database != null && string.Compare(iadmailStorage.Database.PartitionFQDN, TopologyProvider.LocalForestFqdn, StringComparison.OrdinalIgnoreCase) == 0);
				cachingUseOnlyMailboxGuid = iadmailStorage.ExchangeGuid;
				ours = (iadmailStorage is ADSystemAttendantMailbox || (iadmailStorage.Database != null && Storage.FindDatabase(iadmailStorage.Database.ObjectGuid) != null));
			}
			else
			{
				ours = false;
			}
			Guid objectId2 = Directory.ExtractRecipientObjectId(adRecipient);
			SecurityDescriptor securityDescriptor = adRecipientSession.ReadSecurityDescriptor(context, adRecipient);
			if (securityDescriptor == null || securityDescriptor.BinaryForm == null)
			{
				DiagnosticContext.TraceDwordAndString((LID)60352U, 0U, adRecipient.LegacyExchangeDN ?? string.Empty);
				throw new DirectoryInfoCorruptException((LID)43440U, "AD object is missing OS SecurityDescriptor");
			}
			SecurityDescriptor securityDescriptor2 = SecurityHelper.StripUnknownObjectAces(securityDescriptor);
			if (securityDescriptor2 == null || securityDescriptor2.BinaryForm == null)
			{
				throw new DirectoryInfoCorruptException((LID)59824U, "StripUnknownObjectAces failed");
			}
			return new AddressInfo(objectId2, cachingUseOnlyMailboxGuid, Directory.GetDisplayName(adRecipient, false), adRecipient.SimpleDisplayName, adRecipient.MasterAccountSid, adRecipient.LegacyExchangeDN, adRecipient.DistinguishedName, userSid, userSidHistory, userOrgEmailAddr, userOrgAddrType, userEmailAddress, userAddressType, senderEmailAddress, isDistributionList2, isMailPublicFolder, securityDescriptor2, list, hasMailbox);
		}

		internal static void CheckADObjectIsNotCorrupt(LID lid, IExecutionContext context, bool assertedCondition, string errorMessageTemplate, object adObjectId)
		{
			if (!assertedCondition)
			{
				Directory.CheckADObjectIsNotCorruptWithArgs(lid, context, assertedCondition, adObjectId, errorMessageTemplate, new object[]
				{
					adObjectId
				});
			}
		}

		internal static void CheckADObjectIsNotCorruptWithArgs(LID lid, IExecutionContext context, bool assertedCondition, object adObjectId, string errorMessageTemplate, params object[] args)
		{
			if (!assertedCondition)
			{
				TraceContentBuilder traceContentBuilder = TraceContentBuilder.Create();
				traceContentBuilder.AppendFormat(errorMessageTemplate, args);
				traceContentBuilder.AppendLine();
				traceContentBuilder.AppendLine("LID: " + lid.Value);
				ExecutionDiagnostics executionDiagnostics = context.Diagnostics as ExecutionDiagnostics;
				if (executionDiagnostics != null)
				{
					executionDiagnostics.FormatCommonInformation(traceContentBuilder, 0, Guid.Empty);
				}
				Context context2 = context as Context;
				if (context2 != null)
				{
					traceContentBuilder.AppendLine("Logged on User Identity: " + context2.UserIdentity);
					traceContentBuilder.AppendLine("Logged on User SID: " + context2.SecurityContext.UserSid.ToString());
				}
				string text = traceContentBuilder.ToString();
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_CorruptDirectoryObjectDetected, new object[]
				{
					adObjectId,
					text
				});
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				ExTraceGlobals.GeneralTracer.TraceError(0L, text);
				throw new DirectoryInfoCorruptException(lid, text);
			}
		}

		internal ADObjectWrappers.IADOrganizationContainer GetOrganizationContainer(IExecutionContext context, TenantHint tenantHint, string domainController)
		{
			ADObjectWrappers.IADOrganizationContainer organizationContainer;
			try
			{
				ADSessionSettings adsessionSettings = tenantHint.IsRootOrg ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromTenantPartitionHint(TenantPartitionHint.FromPersistablePartitionHint(tenantHint.TenantHintBlob));
				organizationContainer = this.GetOrganizationContainer(context, adsessionSettings.CurrentOrganizationId, domainController);
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException ex)
			{
				context.Diagnostics.OnExceptionCatch(ex);
				throw new DirectoryPermanentErrorException((LID)55648U, string.Format("Cannot resolve OrgId for tenant {0}. Domain controller: {1}", tenantHint, domainController), ex);
			}
			catch (DataSourceTransientException ex2)
			{
				context.Diagnostics.OnExceptionCatch(ex2);
				throw new DirectoryTransientErrorException((LID)43360U, string.Format("Unable to retrieve information for tenant {0} from domain controller {1}", tenantHint, domainController), ex2);
			}
			catch (DataSourceOperationException ex3)
			{
				context.Diagnostics.OnExceptionCatch(ex3);
				throw new DirectoryPermanentErrorException((LID)59744U, string.Format("Unable to retrieve information for tenant {0} from domain controller {1}", tenantHint, domainController), ex3);
			}
			return organizationContainer;
		}

		internal ADObjectWrappers.IADOrganizationContainer GetOrganizationContainer(IExecutionContext context, OrganizationId organizationId, string domainController)
		{
			ADObjectWrappers.IADOrganizationContainer iadorganizationContainer = null;
			if (organizationId == null)
			{
				organizationId = OrganizationId.ForestWideOrgId;
			}
			if (domainController == null)
			{
				using (LockManager.Lock(this.organizationContainerCacheLock))
				{
					iadorganizationContainer = this.organizationContainerCache.ByOrganizationId.Find(organizationId);
				}
			}
			if (iadorganizationContainer == null)
			{
				if (domainController != null && ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.GeneralTracer.TraceInformation<OrganizationId, string>(0, 0L, "Directory.GetOrganizationContainer: Loading OrganizationContainer for {0} from specific domain controller {1}", organizationId, domainController);
				}
				iadorganizationContainer = ADObjectWrappers.GetOrganizationContainer(context, organizationId, domainController);
				if (Directory.getOrgContainerAssignmentLockPreEnterTestHook != null)
				{
					Directory.getOrgContainerAssignmentLockPreEnterTestHook();
				}
				using (LockManager.Lock(this.organizationContainerCacheLock))
				{
					if (Directory.getOrgContainerAssignmentLockPreEnterTestHook != null)
					{
						Directory.getOrgContainerAssignmentLockPostEnterTestHook();
					}
					ADObjectWrappers.IADOrganizationContainer iadorganizationContainer2 = this.organizationContainerCache.ByOrganizationId.Find(organizationId);
					if (iadorganizationContainer2 == null || domainController != null)
					{
						this.organizationContainerCache.ByOrganizationId.Insert(iadorganizationContainer, organizationId, organizationId);
						this.organizationContainerCache.ByGuid.Insert(iadorganizationContainer, organizationId, iadorganizationContainer.ObjectGuid);
					}
					else
					{
						iadorganizationContainer = iadorganizationContainer2;
					}
				}
				if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.GeneralTracer.TraceInformation<OrganizationId>(0, 0L, "Directory.GetOrganizationContainer: OrganizationContainer for {0} has been added into the cache", organizationId);
				}
			}
			return iadorganizationContainer;
		}

		protected override ErrorCode PrimeDirectoryCachesImpl(IExecutionContext context)
		{
			return this.PrimeDirectoryCachesImplInternal(context).Propagate((LID)29848U);
		}

		protected ErrorCode PrimeDirectoryCachesImplInternal(IExecutionContext context)
		{
			try
			{
				ServerInfo serverInfo;
				ADObjectWrappers.IADServer values = this.serverInfoCache.GetValues(context, out serverInfo);
				if (values == null || serverInfo == null)
				{
					return ErrorCode.CreateAdUnavailable((LID)29844U);
				}
			}
			catch (DirectoryPermanentErrorException exception)
			{
				context.Diagnostics.OnExceptionCatch(exception);
				return ErrorCode.CreateAdUnavailable((LID)29860U);
			}
			catch (StoreException ex)
			{
				context.Diagnostics.OnExceptionCatch(ex);
				return ErrorCode.CreateWithLid((LID)40992U, ex.Error);
			}
			try
			{
				if (this.transportInfoCache.GetValue(context) == null)
				{
					return ErrorCode.CreateAdUnavailable((LID)29840U);
				}
			}
			catch (DirectoryPermanentErrorException exception2)
			{
				context.Diagnostics.OnExceptionCatch(exception2);
				return ErrorCode.CreateAdUnavailable((LID)29864U);
			}
			catch (StoreException ex2)
			{
				context.Diagnostics.OnExceptionCatch(ex2);
				return ErrorCode.CreateWithLid((LID)57376U, ex2.Error);
			}
			return ErrorCode.NoError;
		}

		protected override ServerInfo GetServerInfoImpl(IExecutionContext context)
		{
			return this.GetServerInfoImplInternal(context);
		}

		protected ServerInfo GetServerInfoImplInternal(IExecutionContext context)
		{
			ServerInfo serverInfo;
			ADObjectWrappers.IADServer values = this.serverInfoCache.GetValues(context, out serverInfo);
			ErrorHelper.AssertRetail(values != null, "Our cache was primed. How'd we get a null server?");
			ErrorHelper.AssertRetail(serverInfo != null, "Our cache was primed. How'd we get a null serverInfo?");
			return serverInfo;
		}

		protected override DatabaseInfo GetDatabaseInfoImpl(IExecutionContext context, Guid databaseGuid)
		{
			return this.GetDatabaseInfoImplInternal(context, databaseGuid);
		}

		protected DatabaseInfo GetDatabaseInfoImplInternal(IExecutionContext context, Guid databaseGuid)
		{
			DatabaseInfo databaseInfo;
			using (LockManager.Lock(this.databaseInfoLock))
			{
				databaseInfo = this.databaseInfoCache.ByGuid.Find(databaseGuid);
			}
			if (databaseInfo == null)
			{
				databaseInfo = this.LoadDatabaseInfoByGuid(context, databaseGuid);
				using (LockManager.Lock(this.databaseInfoLock))
				{
					this.databaseInfoCache.ByGuid.Insert(databaseInfo, databaseInfo.MdbGuid, databaseGuid);
					this.databaseInfoCache.ByGuid.Insert(databaseInfo, databaseInfo.MdbGuid, databaseInfo.MdbGuid);
				}
				if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.GeneralTracer.TraceInformation<Guid>(0, 0L, "Directory.GetDatabaseInfoImplInternal: DatabaseInfo of {0} has been added into the cache", databaseGuid);
				}
			}
			return databaseInfo;
		}

		protected DatabaseInfo LoadDatabaseInfoByGuid(IExecutionContext context, Guid databaseGuid)
		{
			DatabaseInfo result;
			try
			{
				int totalDatabases = 1;
				ServerInfo serverInfo;
				ADObjectWrappers.IADServer values = this.serverInfoCache.GetValues(context, out serverInfo);
				int totalDatabases2 = values.TotalDatabases;
				if (totalDatabases2 > 0)
				{
					totalDatabases = totalDatabases2;
				}
				ADObjectWrappers.IADMailboxDatabase iadmailboxDatabase = this.AdConfigSession.FindDatabaseByGuid(context, databaseGuid);
				if (iadmailboxDatabase == null)
				{
					throw new DatabaseNotFoundException((LID)37432U, databaseGuid);
				}
				result = Directory.CreateDatabaseInfo(context, iadmailboxDatabase, serverInfo, totalDatabases, values.MaxActiveDatabases, values.Forest);
			}
			catch (DataSourceTransientException ex)
			{
				context.Diagnostics.OnExceptionCatch(ex);
				throw new DirectoryTransientErrorException((LID)53816U, string.Format("Unable to retrieve database information for database with guid {0}", databaseGuid), ex);
			}
			catch (DataSourceOperationException ex2)
			{
				context.Diagnostics.OnExceptionCatch(ex2);
				throw new DirectoryPermanentErrorException((LID)41528U, string.Format("Unable to retrieve database information for database with guid {0}", databaseGuid), ex2);
			}
			return result;
		}

		protected override void RefreshServerInfoImpl(IExecutionContext context)
		{
			this.RefreshServerInfoImplInternal(context);
		}

		protected void RefreshServerInfoImplInternal(IExecutionContext context)
		{
			this.serverInfoCache.ForceReload();
		}

		protected override void RefreshDatabaseInfoImpl(IExecutionContext context, Guid databaseGuid)
		{
			if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.GeneralTracer.TraceInformation<Guid>(0, 0L, "Directory.RefreshDatabaseInfoImpl(databaseGuid={0})", databaseGuid);
			}
			using (LockManager.Lock(this.databaseInfoLock))
			{
				this.databaseInfoCache.ByGuid.Remove(databaseGuid);
			}
		}

		protected override void RefreshMailboxInfoImpl(IExecutionContext context, Guid mailboxGuid)
		{
			using (LockManager.Lock(this.addressInfoLock))
			{
				this.oursAddressInfoCache.ByGuid.Remove(mailboxGuid);
				this.foreignAddressInfoCache.ByGuid.Remove(mailboxGuid);
			}
			using (LockManager.Lock(this.mailboxInfoLock))
			{
				this.oursMailboxInfoCache.ByGuid.Remove(mailboxGuid);
				this.foreignMailboxInfoCache.ByGuid.Remove(mailboxGuid);
			}
		}

		protected override void RefreshOrganizationContainerImpl(IExecutionContext context, Guid organizationGuid)
		{
			using (LockManager.Lock(this.organizationContainerCacheLock))
			{
				this.organizationContainerCache.ByGuid.Remove(organizationGuid);
			}
		}

		protected override MailboxInfo GetMailboxInfoImpl(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, GetMailboxInfoFlags flags)
		{
			return this.GetMailboxInfoHelper(context, tenantHint, null, mailboxGuid, flags);
		}

		protected MailboxInfo GetMailboxInfoHelper(IExecutionContext context, TenantHint tenantHint, string domainController, Guid mailboxGuid, GetMailboxInfoFlags flags)
		{
			MailboxInfo mailboxInfo;
			using (LockManager.Lock(this.mailboxInfoLock))
			{
				mailboxInfo = this.oursMailboxInfoCache.ByGuid.Find(mailboxGuid);
				if (mailboxInfo == null && ConfigurationSchema.SeparateDirectoryCaches.Value)
				{
					mailboxInfo = this.foreignMailboxInfoCache.ByGuid.Find(mailboxGuid);
				}
			}
			if (mailboxInfo == null)
			{
				bool flag;
				mailboxInfo = this.LoadMailboxInfoByGuid(context, tenantHint, domainController, mailboxGuid, flags, out flag);
				using (LockManager.Lock(this.mailboxInfoLock))
				{
					Directory.MailboxInfoCache mailboxInfoCache = (flag || !ConfigurationSchema.SeparateDirectoryCaches.Value) ? this.oursMailboxInfoCache : this.foreignMailboxInfoCache;
					mailboxInfoCache.ByGuid.Insert(mailboxInfo, mailboxInfo.MailboxGuid, mailboxGuid);
					mailboxInfoCache.ByGuid.Insert(mailboxInfo, mailboxInfo.MailboxGuid, mailboxInfo.MailboxGuid);
					if (!mailboxInfo.IsArchiveMailbox && !string.IsNullOrEmpty(mailboxInfo.OwnerLegacyDN))
					{
						mailboxInfoCache.ByLegacyDN.Insert(mailboxInfo, mailboxInfo.MailboxGuid, mailboxInfo.OwnerLegacyDN.ToUpper(CultureHelper.DefaultCultureInfo));
					}
				}
			}
			if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.GeneralTracer.TraceInformation(0, 0L, "GetMailboxInfoImpl(lookupGuid={0}), Result=(ObjectGuid={1}, MailboxGuid={2}, MdbGuid={3}, OwnerLegacyDN={4})", new object[]
				{
					mailboxGuid,
					mailboxInfo.OwnerGuid,
					mailboxInfo.MailboxGuid,
					mailboxInfo.MdbGuid,
					mailboxInfo.OwnerLegacyDN
				});
			}
			return mailboxInfo;
		}

		protected override MailboxInfo GetMailboxInfoImpl(IExecutionContext context, TenantHint tenantHint, string legacyDN)
		{
			string text = legacyDN.ToUpper(CultureHelper.DefaultCultureInfo);
			MailboxInfo mailboxInfo;
			using (LockManager.Lock(this.mailboxInfoLock))
			{
				mailboxInfo = this.oursMailboxInfoCache.ByLegacyDN.Find(text);
				if (mailboxInfo == null && ConfigurationSchema.SeparateDirectoryCaches.Value)
				{
					mailboxInfo = this.foreignMailboxInfoCache.ByLegacyDN.Find(text);
				}
			}
			if (mailboxInfo == null)
			{
				bool flag;
				mailboxInfo = this.LoadMailboxInfoByLegacyDn(context, tenantHint, text, out flag);
				using (LockManager.Lock(this.mailboxInfoLock))
				{
					Directory.MailboxInfoCache mailboxInfoCache = (flag || !ConfigurationSchema.SeparateDirectoryCaches.Value) ? this.oursMailboxInfoCache : this.foreignMailboxInfoCache;
					mailboxInfoCache.ByLegacyDN.Insert(mailboxInfo, mailboxInfo.MailboxGuid, text);
					mailboxInfoCache.ByGuid.Insert(mailboxInfo, mailboxInfo.MailboxGuid, mailboxInfo.MailboxGuid);
				}
			}
			if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.GeneralTracer.TraceInformation(0, 0L, "GetMailboxInfoImpl(legacyDN={0}), Result=(ObjectGuid={1}, MailboxGuid={2}, MdbGuid={3}, OwnerLegacyDN={4})", new object[]
				{
					text,
					mailboxInfo.OwnerGuid,
					mailboxInfo.MailboxGuid,
					mailboxInfo.MdbGuid,
					mailboxInfo.OwnerLegacyDN
				});
			}
			return mailboxInfo;
		}

		protected override AddressInfo GetAddressInfoByMailboxGuidImpl(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, GetAddressInfoFlags flags)
		{
			return this.GetAddressInfoHelper(context, tenantHint, null, mailboxGuid, false, flags);
		}

		protected override AddressInfo GetAddressInfoByObjectIdImpl(IExecutionContext context, TenantHint tenantHint, Guid objectId)
		{
			return this.GetAddressInfoHelper(context, tenantHint, null, objectId, true, GetAddressInfoFlags.None);
		}

		protected AddressInfo GetAddressInfoHelper(IExecutionContext context, TenantHint tenantHint, string domainController, Guid lookupGuid, bool lookupByObjectId, GetAddressInfoFlags flags)
		{
			AddressInfo addressInfo;
			using (LockManager.Lock(this.addressInfoLock))
			{
				addressInfo = this.oursAddressInfoCache.ByGuid.Find(lookupGuid);
				if (addressInfo == null && ConfigurationSchema.SeparateDirectoryCaches.Value)
				{
					addressInfo = this.foreignAddressInfoCache.ByGuid.Find(lookupGuid);
				}
			}
			if (addressInfo == null)
			{
				bool flag;
				addressInfo = this.LoadAddressInfoByGuid(context, tenantHint, domainController, lookupGuid, lookupByObjectId, flags, out flag);
				using (LockManager.Lock(this.addressInfoLock))
				{
					Directory.AddressInfoCache addressInfoCache = (flag || !ConfigurationSchema.SeparateDirectoryCaches.Value) ? this.oursAddressInfoCache : this.foreignAddressInfoCache;
					addressInfoCache.ByGuid.Insert(addressInfo, addressInfo.ObjectId, lookupGuid);
					addressInfoCache.ByGuid.Insert(addressInfo, addressInfo.ObjectId, addressInfo.ObjectId);
					if (!addressInfo.ADCachingUseOnlyMailboxGuid.Equals(Guid.Empty))
					{
						addressInfoCache.ByGuid.Insert(addressInfo, addressInfo.ObjectId, addressInfo.ADCachingUseOnlyMailboxGuid);
					}
					if (!string.IsNullOrEmpty(addressInfo.LegacyExchangeDN))
					{
						addressInfoCache.ByLegacyDN.Insert(addressInfo, addressInfo.ObjectId, addressInfo.LegacyExchangeDN.ToUpper(CultureHelper.DefaultCultureInfo));
					}
				}
			}
			if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.GeneralTracer.TraceInformation(0, 0L, "GetAddressInfoHelper(lookupGuid={0}), Result=(ObjectId={1}, MailboxGuid={2}, LegacyExchangeDN={3})", new object[]
				{
					lookupGuid,
					addressInfo.ObjectId,
					addressInfo.ADCachingUseOnlyMailboxGuid,
					addressInfo.LegacyExchangeDN
				});
			}
			return addressInfo;
		}

		protected override AddressInfo GetAddressInfoImpl(IExecutionContext context, TenantHint tenantHint, string legacyDN, bool loadPublicDelegates)
		{
			string text = legacyDN.ToUpper(CultureHelper.DefaultCultureInfo);
			AddressInfo addressInfo;
			using (LockManager.Lock(this.addressInfoLock))
			{
				addressInfo = this.oursAddressInfoCache.ByLegacyDN.Find(text);
				if (addressInfo == null && ConfigurationSchema.SeparateDirectoryCaches.Value)
				{
					addressInfo = this.foreignAddressInfoCache.ByLegacyDN.Find(text);
				}
			}
			if (loadPublicDelegates && addressInfo != null && addressInfo.PublicDelegates == null)
			{
				addressInfo = null;
			}
			if (addressInfo == null)
			{
				bool flag;
				addressInfo = this.LoadAddressInfoByLegacyDn(context, tenantHint, text, loadPublicDelegates, out flag);
				using (LockManager.Lock(this.addressInfoLock))
				{
					Directory.AddressInfoCache addressInfoCache = (flag || !ConfigurationSchema.SeparateDirectoryCaches.Value) ? this.oursAddressInfoCache : this.foreignAddressInfoCache;
					addressInfoCache.ByLegacyDN.Insert(addressInfo, addressInfo.ObjectId, text);
					addressInfoCache.ByGuid.Insert(addressInfo, addressInfo.ObjectId, addressInfo.ObjectId);
					if (!addressInfo.ADCachingUseOnlyMailboxGuid.Equals(Guid.Empty))
					{
						addressInfoCache.ByGuid.Insert(addressInfo, addressInfo.ObjectId, addressInfo.ADCachingUseOnlyMailboxGuid);
					}
				}
			}
			if (ExTraceGlobals.GeneralTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.GeneralTracer.TraceInformation(0, 0L, "GetAddressInfoImpl(legacyDN={0}, loadPublicDelegates={1}), Result=(ObjectId={2}, MailboGuid={3}, LegacyExchangeDN={4})", new object[]
				{
					text,
					loadPublicDelegates,
					addressInfo.ObjectId,
					addressInfo.ADCachingUseOnlyMailboxGuid,
					addressInfo.LegacyExchangeDN
				});
			}
			return addressInfo;
		}

		protected override TenantHint ResolveTenantHintImpl(IExecutionContext context, byte[] tenantHintBlob)
		{
			TenantHint result;
			try
			{
				TenantPartitionHint tenantPartitionHint = TenantPartitionHint.Deserialize(tenantHintBlob);
				result = new TenantHint(tenantPartitionHint.GetPersistablePartitionHint());
			}
			catch (CannotDeserializePartitionHintException ex)
			{
				context.Diagnostics.OnExceptionCatch(ex);
				throw new DirectoryTransientErrorException((LID)55208U, "Unable to deserialize tenant hint", ex);
			}
			catch (CannotResolveTenantNameException ex2)
			{
				context.Diagnostics.OnExceptionCatch(ex2);
				throw new DirectoryTransientErrorException((LID)42920U, "Unable to resolve tenant hint", ex2);
			}
			catch (DataSourceTransientException ex3)
			{
				context.Diagnostics.OnExceptionCatch(ex3);
				throw new DirectoryTransientErrorException((LID)54504U, "Unable to resolve tenant hint", ex3);
			}
			catch (DataSourceOperationException ex4)
			{
				context.Diagnostics.OnExceptionCatch(ex4);
				throw new DirectoryPermanentErrorException((LID)63400U, "Unable to resolve tenant hint", ex4);
			}
			return result;
		}

		protected override void PrePopulateCachesForMailboxImpl(IExecutionContext context, TenantHint tenantHint, Guid mailboxGuid, string domainController)
		{
			this.GetOrganizationContainer(context, tenantHint, domainController);
			base.RefreshMailboxInfo(context, mailboxGuid);
			this.GetMailboxInfoHelper(context, tenantHint, domainController, mailboxGuid, GetMailboxInfoFlags.BypassSharedCache);
			this.GetAddressInfoHelper(context, tenantHint, domainController, mailboxGuid, false, GetAddressInfoFlags.BypassSharedCache);
		}

		protected override bool IsMemberOfDistributionListImpl(IExecutionContext context, TenantHint tenantHint, AddressInfo addressInfo, Guid distributionListObjectId)
		{
			bool result;
			try
			{
				Directory.DistributionListMembershipKey key = new Directory.DistributionListMembershipKey(distributionListObjectId, addressInfo.ObjectId);
				bool? flag;
				using (LockManager.Lock(this.distributionListMembershipLock))
				{
					flag = this.distributionListMembershipCache.Find(key);
				}
				if (flag != null)
				{
					result = flag.Value;
				}
				else
				{
					ADObjectWrappers.IADRecipientSession iadrecipientSession = this.CreateAdRecipientSession(context, tenantHint, null, false);
					ADRecipient adrecipient = iadrecipientSession.FindByObjectId(context, addressInfo.ObjectId);
					if (adrecipient == null)
					{
						throw new UserNotFoundException((LID)33920U, addressInfo.DistinguishedName);
					}
					bool flag2 = false;
					ADRecipient adrecipient2 = iadrecipientSession.FindByObjectId(context, distributionListObjectId);
					if (adrecipient2 != null)
					{
						flag2 = iadrecipientSession.IsMemberOfDistributionList(context, adrecipient, adrecipient2.Guid);
					}
					using (LockManager.Lock(this.distributionListMembershipLock))
					{
						this.distributionListMembershipCache.Insert(key, new bool?(flag2));
					}
					result = flag2;
				}
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException exception)
			{
				context.Diagnostics.OnExceptionCatch(exception);
				throw new UserNotFoundException((LID)50304U, addressInfo.DistinguishedName);
			}
			catch (NonUniqueRecipientException exception2)
			{
				context.Diagnostics.OnExceptionCatch(exception2);
				throw Directory.CreateNonUniqueRecipientException((LID)47232U, exception2);
			}
			catch (DataSourceTransientException ex)
			{
				context.Diagnostics.OnExceptionCatch(ex);
				throw new DirectoryTransientErrorException((LID)63616U, string.Format("Unable to verify if recipient {0} is member in DL {1}", addressInfo.DistinguishedName, distributionListObjectId), ex);
			}
			catch (DataSourceOperationException ex2)
			{
				context.Diagnostics.OnExceptionCatch(ex2);
				throw new DirectoryPermanentErrorException((LID)55424U, string.Format("Unable to verify if recipient {0} is member in DL {1}", addressInfo.DistinguishedName, distributionListObjectId), ex2);
			}
			return result;
		}

		private static StoreException CreateNonUniqueRecipientException(LID lid, NonUniqueRecipientException exception)
		{
			ExTraceGlobals.GeneralTracer.TraceError(0L, "Non unique recipients are not supported; event log should have an event on DirectoryEventLogConstants.Tuple_DSC_EVENT_NON_UNIQUE_RECIPIENT for this exception");
			return new NonUniqueRecipientException(lid, "Non unique recipients are not supported");
		}

		private static StoreException CreateUnsupportedRecipientTypeException(LID lid, ADRecipient adRecipient)
		{
			string text = string.Format("Recipient {0} has an unsupported RecipientType: {1}", adRecipient.DistinguishedName, adRecipient.RecipientType);
			Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_UnsupportedRecipientTypeDetected, new object[]
			{
				adRecipient.RecipientType,
				text
			});
			ExTraceGlobals.GeneralTracer.TraceError(0L, text);
			return new UnsupportedRecipientTypeException(lid, text);
		}

		private static string GetFilePath(LocalLongFullPath fullPath)
		{
			if (!(fullPath == null))
			{
				return fullPath.PathName;
			}
			return null;
		}

		private static Guid ExtractRecipientObjectId(ADRecipient adRecipient)
		{
			return (Guid)adRecipient[ADObjectSchema.ExchangeObjectId];
		}

		private static string GetDisplayName(ADRecipient adRecipient, bool isArchive)
		{
			if (!isArchive)
			{
				return adRecipient.DisplayName;
			}
			ADUser aduser = (ADUser)adRecipient;
			if (aduser.ArchiveName.Count != 0)
			{
				return aduser.ArchiveName[0];
			}
			return null;
		}

		private static bool IsSmtpAddress(ProxyAddress address)
		{
			return address.Prefix.Equals(ProxyAddressPrefix.Smtp);
		}

		private static string GetSenderEmailAddress(ADRecipient adRecipient)
		{
			if (adRecipient == null)
			{
				return string.Empty;
			}
			SmtpAddress primarySmtpAddress = adRecipient.PrimarySmtpAddress;
			if (primarySmtpAddress.IsValidAddress)
			{
				return primarySmtpAddress.ToString();
			}
			ProxyAddressCollection emailAddresses = adRecipient.EmailAddresses;
			if (emailAddresses == null || 0 >= emailAddresses.Count)
			{
				return string.Empty;
			}
			ProxyAddress proxyAddress = emailAddresses.Find(new Predicate<ProxyAddress>(Directory.IsSmtpAddress));
			if (null != proxyAddress)
			{
				return proxyAddress.ToString();
			}
			return emailAddresses[0].ToString();
		}

		private ADObjectWrappers.IADRecipientSession CreateAdRecipientSession(IExecutionContext context, TenantHint tenantHint, string domainController, bool bypassSharedCache)
		{
			return ADObjectWrappers.CreateADRecipientSession(context, ConsistencyMode.PartiallyConsistent, tenantHint, domainController, bypassSharedCache);
		}

		private AddressInfo LoadAddressInfoByLegacyDn(IExecutionContext context, TenantHint tenantHint, string legacyDN, bool loadPublicDelegates, out bool ours)
		{
			AddressInfo result;
			try
			{
				ADObjectWrappers.IADRecipientSession iadrecipientSession = this.CreateAdRecipientSession(context, tenantHint, null, false);
				ADRecipient adrecipient = iadrecipientSession.FindByLegacyExchangeDN(context, legacyDN);
				if (adrecipient == null && !tenantHint.IsRootOrg)
				{
					iadrecipientSession = this.CreateAdRecipientSession(context, TenantHint.RootOrg, null, false);
					adrecipient = iadrecipientSession.FindByLegacyExchangeDN(context, legacyDN);
				}
				if (adrecipient == null)
				{
					throw new UserNotFoundException((LID)64952U, legacyDN);
				}
				result = Directory.CreateAddressInfo(context, iadrecipientSession, adrecipient, loadPublicDelegates, out ours);
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException ex)
			{
				context.Diagnostics.OnExceptionCatch(ex);
				throw new UserNotFoundException((LID)59304U, legacyDN, ex);
			}
			catch (NonUniqueRecipientException exception)
			{
				context.Diagnostics.OnExceptionCatch(exception);
				throw Directory.CreateNonUniqueRecipientException((LID)61736U, exception);
			}
			catch (DataSourceTransientException ex2)
			{
				context.Diagnostics.OnExceptionCatch(ex2);
				throw new DirectoryTransientErrorException((LID)48568U, string.Format("Unable to retrieve address information for addressee with legacy DN {0}", legacyDN), ex2);
			}
			catch (DataSourceOperationException ex3)
			{
				context.Diagnostics.OnExceptionCatch(ex3);
				throw new DirectoryPermanentErrorException((LID)49720U, string.Format("Unable to retrieve address information for addressee with legacy DN {0}", legacyDN), ex3);
			}
			return result;
		}

		private MailboxInfo LoadMailboxInfoByGuid(IExecutionContext context, TenantHint tenantHint, string domainController, Guid mailboxGuid, GetMailboxInfoFlags flags, out bool ours)
		{
			try
			{
				ADRecipient adrecipient = this.CreateAdRecipientSession(context, tenantHint, domainController, (flags & GetMailboxInfoFlags.BypassSharedCache) != GetMailboxInfoFlags.None).FindByExchangeGuidIncludingAlternate(context, mailboxGuid);
				if (adrecipient != null)
				{
					ADUser aduser = adrecipient as ADUser;
					if (aduser != null)
					{
						IMailboxLocationCollection mailboxLocations = aduser.MailboxLocations;
						if (mailboxLocations == null)
						{
							throw new DirectoryInfoCorruptException((LID)62156U, string.Format("MailboxLocations for MailboxGuid {0} is null, ADUser MailboxLocations should never be null.", mailboxGuid));
						}
						IMailboxLocationInfo mailboxLocation = mailboxLocations.GetMailboxLocation(mailboxGuid);
						MailboxLocationType? mailboxLocationType = null;
						if (mailboxLocation != null)
						{
							if (mailboxLocation.MailboxLocationType == MailboxLocationType.Aggregated)
							{
								Directory.CheckADObjectIsNotCorruptWithArgs((LID)53964U, context, false, mailboxGuid, "Found Mailbox Type {0} in mailbox collection and {0} is not supported in store to be in the MailboxLocations for recipient {1}", new object[]
								{
									mailboxLocation.MailboxLocationType,
									mailboxGuid
								});
							}
							else
							{
								mailboxLocationType = new MailboxLocationType?(mailboxLocation.MailboxLocationType);
							}
						}
						else if (aduser.ExchangeGuid == mailboxGuid)
						{
							mailboxLocationType = new MailboxLocationType?(MailboxLocationType.Primary);
						}
						else if (aduser.ArchiveGuid == mailboxGuid)
						{
							mailboxLocationType = new MailboxLocationType?(MailboxLocationType.MainArchive);
						}
						else if (aduser.AggregatedMailboxGuids != null)
						{
							foreach (Guid a in aduser.AggregatedMailboxGuids)
							{
								if (a == mailboxGuid)
								{
									Directory.CheckADObjectIsNotCorrupt((LID)52636U, context, aduser.MailboxContainerGuid != null && aduser.MailboxContainerGuid.Value != Guid.Empty, "Aggregated mailbox is configured on ADUser without ContainerGuid. for recipient {0}", mailboxGuid);
									mailboxLocationType = new MailboxLocationType?(MailboxLocationType.Aggregated);
									break;
								}
							}
						}
						Directory.CheckADObjectIsNotCorrupt((LID)60828U, context, mailboxLocationType != null, "Lookup by ExchangeGuid returns inconsistent result for recipient {0}", mailboxGuid);
						if ((flags & GetMailboxInfoFlags.IgnoreHomeMdb) == GetMailboxInfoFlags.None)
						{
							bool assertedCondition = false;
							if (mailboxLocation != null)
							{
								assertedCondition = (mailboxLocation.DatabaseLocation != null);
							}
							else if (mailboxLocationType == MailboxLocationType.MainArchive)
							{
								assertedCondition = (aduser.ArchiveDatabase != null);
							}
							else if (mailboxLocationType == MailboxLocationType.Primary || mailboxLocationType == MailboxLocationType.Aggregated)
							{
								assertedCondition = (aduser.Database != null);
							}
							Directory.CheckADObjectIsNotCorruptWithArgs((LID)57912U, context, assertedCondition, mailboxGuid, "Mailbox Database attribute is null mailbox location type {0} for recipient {1}", new object[]
							{
								mailboxLocationType,
								mailboxGuid
							});
						}
						ADObjectWrappers.IADTransportConfigContainer value = this.transportInfoCache.GetValue(context);
						ErrorHelper.AssertRetail(value != null, "Our cache was primed. How'd we get a null transportConfig?");
						ADObjectWrappers.IADOrganizationContainer organizationContainer = null;
						if (adrecipient.RecipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox && adrecipient.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
						{
							organizationContainer = this.GetOrganizationContainer(context, adrecipient.OrganizationId, null);
						}
						Guid externalDirectoryOrganizationId = Guid.Empty;
						if (tenantHint.TenantHintBlob != null && tenantHint.TenantHintBlob.Length > 0)
						{
							externalDirectoryOrganizationId = TenantPartitionHint.FromPersistablePartitionHint(tenantHint.TenantHintBlob).GetExternalDirectoryOrganizationId();
						}
						return Directory.CreateMailboxInfo(context, adrecipient, mailboxLocationType.Value, mailboxGuid, value, organizationContainer, !tenantHint.IsRootOrg, externalDirectoryOrganizationId, out ours);
					}
					else
					{
						ADSystemMailbox adsystemMailbox = adrecipient as ADSystemMailbox;
						if (adsystemMailbox != null)
						{
							Directory.CheckADObjectIsNotCorrupt((LID)46492U, context, adsystemMailbox.ExchangeGuid == mailboxGuid, "Lookup by ExchangeGuid returns inconsistent result for recipient {0}", mailboxGuid);
							Directory.CheckADObjectIsNotCorrupt((LID)58808U, context, adsystemMailbox.Database != null, "HomeMDB attribute is null for recipient {0}", mailboxGuid);
							return Directory.CreateMailboxInfo(context, adrecipient, MailboxLocationType.Primary, mailboxGuid, null, null, false, Guid.Empty, out ours);
						}
					}
				}
				string legacyExchangeDN = base.GetServerInfo(context).ExchangeLegacyDN + "/cn=Microsoft System Attendant";
				adrecipient = this.CreateAdRecipientSession(context, TenantHint.RootOrg, domainController, (flags & GetMailboxInfoFlags.BypassSharedCache) != GetMailboxInfoFlags.None).FindByLegacyExchangeDN(context, legacyExchangeDN);
				if (adrecipient != null)
				{
					ADSystemAttendantMailbox adsystemAttendantMailbox = adrecipient as ADSystemAttendantMailbox;
					if (adsystemAttendantMailbox != null && mailboxGuid == adsystemAttendantMailbox.Guid)
					{
						return Directory.CreateMailboxInfo(context, adsystemAttendantMailbox, MailboxLocationType.Primary, mailboxGuid, null, null, false, Guid.Empty, out ours);
					}
				}
				throw new MailboxNotFoundException((LID)54712U, mailboxGuid);
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException ex)
			{
				context.Diagnostics.OnExceptionCatch(ex);
				throw new MailboxNotFoundException((LID)51112U, mailboxGuid, ex);
			}
			catch (NonUniqueRecipientException exception)
			{
				context.Diagnostics.OnExceptionCatch(exception);
				throw Directory.CreateNonUniqueRecipientException((LID)35112U, exception);
			}
			catch (DataSourceTransientException ex2)
			{
				context.Diagnostics.OnExceptionCatch(ex2);
				throw new DirectoryTransientErrorException((LID)52664U, string.Format("Unable to retrieve mailbox information for mailbox with guid {0} ", mailboxGuid), ex2);
			}
			catch (DataSourceOperationException ex3)
			{
				context.Diagnostics.OnExceptionCatch(ex3);
				throw new DirectoryPermanentErrorException((LID)62904U, string.Format("Unable to retrieve mailbox information for mailbox with guid {0}", mailboxGuid), ex3);
			}
			MailboxInfo result;
			return result;
		}

		private MailboxInfo LoadMailboxInfoByLegacyDn(IExecutionContext context, TenantHint tenantHint, string legacyDN, out bool ours)
		{
			TenantHint tenantHint2 = tenantHint;
			try
			{
				ADRecipient adrecipient = this.CreateAdRecipientSession(context, tenantHint2, null, false).FindByLegacyExchangeDN(context, legacyDN);
				if (adrecipient == null && !tenantHint2.IsRootOrg)
				{
					tenantHint2 = TenantHint.RootOrg;
					adrecipient = this.CreateAdRecipientSession(context, tenantHint2, null, false).FindByLegacyExchangeDN(context, legacyDN);
				}
				if (adrecipient != null)
				{
					Guid externalDirectoryOrganizationId = Guid.Empty;
					if (tenantHint.TenantHintBlob != null && tenantHint.TenantHintBlob.Length > 0)
					{
						externalDirectoryOrganizationId = TenantPartitionHint.FromPersistablePartitionHint(tenantHint2.TenantHintBlob).GetExternalDirectoryOrganizationId();
					}
					ADUser aduser = adrecipient as ADUser;
					if (aduser != null)
					{
						Directory.CheckADObjectIsNotCorrupt((LID)50616U, context, aduser.Database != null, "HomeMDB attribute is null for recipient {0}", legacyDN);
						ADObjectWrappers.IADTransportConfigContainer value = this.transportInfoCache.GetValue(context);
						ErrorHelper.AssertRetail(value != null, "Our cache was primed. How'd we get a null transportConfig?");
						ADObjectWrappers.IADOrganizationContainer organizationContainer = null;
						if (adrecipient.RecipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox && adrecipient.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
						{
							organizationContainer = this.GetOrganizationContainer(context, adrecipient.OrganizationId, null);
						}
						return Directory.CreateMailboxInfo(context, adrecipient, MailboxLocationType.Primary, aduser.ExchangeGuid, value, organizationContainer, !tenantHint2.IsRootOrg, externalDirectoryOrganizationId, out ours);
					}
					ADSystemMailbox adsystemMailbox = adrecipient as ADSystemMailbox;
					if (adsystemMailbox != null)
					{
						Directory.CheckADObjectIsNotCorrupt((LID)47544U, context, adsystemMailbox.Database != null, "HomeMDB attribute is null for recipient {0}", legacyDN);
						return Directory.CreateMailboxInfo(context, adrecipient, MailboxLocationType.Primary, adsystemMailbox.ExchangeGuid, null, null, false, Guid.Empty, out ours);
					}
					ADSystemAttendantMailbox adsystemAttendantMailbox = adrecipient as ADSystemAttendantMailbox;
					if (adsystemAttendantMailbox != null)
					{
						return Directory.CreateMailboxInfo(context, adrecipient, MailboxLocationType.Primary, adsystemAttendantMailbox.Guid, null, null, false, Guid.Empty, out ours);
					}
					ADMicrosoftExchangeRecipient admicrosoftExchangeRecipient = adrecipient as ADMicrosoftExchangeRecipient;
					if (admicrosoftExchangeRecipient != null)
					{
						return Directory.CreateMailboxInfo(context, adrecipient, MailboxLocationType.Primary, admicrosoftExchangeRecipient.ExchangeGuid, null, null, !tenantHint2.IsRootOrg, externalDirectoryOrganizationId, out ours);
					}
				}
				throw new MailboxNotFoundException((LID)46520U, legacyDN);
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException ex)
			{
				context.Diagnostics.OnExceptionCatch(ex);
				throw new MailboxNotFoundException((LID)48040U, legacyDN, ex);
			}
			catch (NonUniqueRecipientException exception)
			{
				context.Diagnostics.OnExceptionCatch(exception);
				throw Directory.CreateNonUniqueRecipientException((LID)51496U, exception);
			}
			catch (DataSourceTransientException ex2)
			{
				context.Diagnostics.OnExceptionCatch(ex2);
				throw new DirectoryTransientErrorException((LID)36280U, string.Format("Unable to retrieve mailbox information for mailbox with legacy DN {0}", legacyDN), ex2);
			}
			catch (DataSourceOperationException ex3)
			{
				context.Diagnostics.OnExceptionCatch(ex3);
				throw new DirectoryPermanentErrorException((LID)60856U, string.Format("Unable to retrieve mailbox information for mailbox with legacy DN {0}", legacyDN), ex3);
			}
			MailboxInfo result;
			return result;
		}

		private AddressInfo LoadAddressInfoByGuid(IExecutionContext context, TenantHint tenantHint, string domainController, Guid lookupGuid, bool lookupByObjectId, GetAddressInfoFlags flags, out bool ours)
		{
			AddressInfo result;
			try
			{
				ADObjectWrappers.IADRecipientSession iadrecipientSession = this.CreateAdRecipientSession(context, tenantHint, domainController, (flags & GetAddressInfoFlags.BypassSharedCache) != GetAddressInfoFlags.None);
				ADRecipient adrecipient;
				if (lookupByObjectId)
				{
					adrecipient = iadrecipientSession.FindByObjectId(context, lookupGuid);
				}
				else
				{
					adrecipient = iadrecipientSession.FindByExchangeGuidIncludingAlternate(context, lookupGuid);
				}
				if (adrecipient != null)
				{
					result = Directory.CreateAddressInfo(context, iadrecipientSession, adrecipient, false, out ours);
				}
				else
				{
					string legacyExchangeDN = base.GetServerInfo(context).ExchangeLegacyDN + "/cn=Microsoft System Attendant";
					iadrecipientSession = this.CreateAdRecipientSession(context, TenantHint.RootOrg, domainController, (flags & GetAddressInfoFlags.BypassSharedCache) != GetAddressInfoFlags.None);
					adrecipient = iadrecipientSession.FindByLegacyExchangeDN(context, legacyExchangeDN);
					if (adrecipient == null || !(lookupGuid == adrecipient.Guid))
					{
						throw new UserNotFoundException((LID)44472U, lookupGuid);
					}
					result = Directory.CreateAddressInfo(context, iadrecipientSession, adrecipient, false, out ours);
				}
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException ex)
			{
				context.Diagnostics.OnExceptionCatch(ex);
				throw new UserNotFoundException((LID)39848U, lookupGuid, ex);
			}
			catch (NonUniqueRecipientException exception)
			{
				context.Diagnostics.OnExceptionCatch(exception);
				throw Directory.CreateNonUniqueRecipientException((LID)45352U, exception);
			}
			catch (DataSourceTransientException ex2)
			{
				context.Diagnostics.OnExceptionCatch(ex2);
				throw new DirectoryTransientErrorException((LID)56760U, string.Format("Unable to retrieve address information for addressee with mailbox guid {0}", lookupGuid), ex2);
			}
			catch (DataSourceOperationException ex3)
			{
				context.Diagnostics.OnExceptionCatch(ex3);
				throw new DirectoryPermanentErrorException((LID)40376U, string.Format("Unable to retrieve address information for addressee with mailbox guid {0}", lookupGuid), ex3);
			}
			return result;
		}

		internal const string SystemAttendant = "/cn=Microsoft System Attendant";

		private const string HomeMDBAttributeIsNullTemplate = "HomeMDB attribute is null for recipient {0}";

		private const string MDBAttributeIsNullTemplate = "Mailbox Database attribute is null mailbox location type {0} for recipient {1}";

		private const string LookupByExchangeGuidInconsistentResultTemplate = "Lookup by ExchangeGuid returns inconsistent result for recipient {0}";

		private const string UnsupportedMailboxTypeResultTemplate = "Found Mailbox Type {0} in mailbox collection and {0} is not supported in store to be in the MailboxLocations for recipient {1}";

		private const string AggregatedMailboxConfiguredOnObjectWithoutContainerGuidTemplate = "Aggregated mailbox is configured on ADUser without ContainerGuid. for recipient {0}";

		private static Action getOrgContainerAssignmentLockPreEnterTestHook;

		private static Action getOrgContainerAssignmentLockPostEnterTestHook;

		private ADObjectWrappers.IADSystemConfigurationSession adConfigSession;

		private Directory.SingletonDirectoryCache<ADObjectWrappers.IADServer, ServerInfo> serverInfoCache;

		private Directory.SingletonDirectoryCache<ADObjectWrappers.IADTransportConfigContainer> transportInfoCache;

		private Directory.OrganizationContainerCache organizationContainerCache;

		private Directory.MailboxInfoCache oursMailboxInfoCache;

		private Directory.MailboxInfoCache foreignMailboxInfoCache;

		private Directory.AddressInfoCache oursAddressInfoCache;

		private Directory.AddressInfoCache foreignAddressInfoCache;

		private Directory.DatabaseInfoCache databaseInfoCache;

		private SingleKeyCache<Directory.DistributionListMembershipKey, bool?> distributionListMembershipCache;

		private object databaseInfoLock = new object();

		private object mailboxInfoLock = new object();

		private object addressInfoLock = new object();

		private object distributionListMembershipLock = new object();

		private object organizationContainerCacheLock = new object();

		internal struct DistributionListMembershipKey : IEquatable<Directory.DistributionListMembershipKey>
		{
			internal DistributionListMembershipKey(Guid distributionListObjectId, Guid memberId)
			{
				this.distributionListObjectId = distributionListObjectId;
				this.memberId = memberId;
			}

			public bool Equals(Directory.DistributionListMembershipKey other)
			{
				return other.distributionListObjectId == this.distributionListObjectId && other.memberId == this.memberId;
			}

			public override bool Equals(object obj)
			{
				return obj is Directory.DistributionListMembershipKey && this.Equals((Directory.DistributionListMembershipKey)obj);
			}

			public override int GetHashCode()
			{
				return this.distributionListObjectId.GetHashCode() ^ this.memberId.GetHashCode();
			}

			private Guid distributionListObjectId;

			private Guid memberId;
		}

		internal class SingletonDirectoryCache<TADObject, TTransformed> where TADObject : class where TTransformed : class
		{
			public SingletonDirectoryCache(Func<DateTime, bool> expirationPolicy, Func<IExecutionContext, TADObject> loadADObject, Func<IExecutionContext, TADObject, TTransformed> transformADObject)
			{
				this.expirationPolicy = expirationPolicy;
				this.loadADObject = loadADObject;
				this.transformADObject = transformADObject;
			}

			public void ForceReload()
			{
				using (LockManager.Lock(this.lockInstance))
				{
					this.lastRefreshTime = DateTime.MinValue;
				}
			}

			public TADObject GetValues(IExecutionContext context, out TTransformed transformedObject)
			{
				Exception innerException = null;
				TADObject tadobject;
				using (LockManager.Lock(this.lockInstance))
				{
					tadobject = this.adObject;
					transformedObject = this.transformedObject;
					if (tadobject != null && this.expirationPolicy(this.lastRefreshTime))
					{
						goto IL_111;
					}
				}
				try
				{
					TADObject tadobject2 = this.loadADObject(context);
					TTransformed ttransformed = default(TTransformed);
					if (this.transformADObject != null)
					{
						ttransformed = this.transformADObject(context, tadobject2);
					}
					if (tadobject2 != null)
					{
						tadobject = tadobject2;
						transformedObject = ttransformed;
						using (LockManager.Lock(this.lockInstance))
						{
							this.adObject = tadobject;
							this.transformedObject = transformedObject;
							this.lastRefreshTime = DateTime.UtcNow;
						}
					}
				}
				catch (DataSourceTransientException ex)
				{
					context.Diagnostics.OnExceptionCatch(ex);
					innerException = ex;
				}
				catch (DataSourceOperationException ex2)
				{
					context.Diagnostics.OnExceptionCatch(ex2);
					innerException = ex2;
				}
				if (tadobject == null)
				{
					throw new DirectoryPermanentErrorException((LID)51768U, "Unable to retrieve information from AD!", innerException);
				}
				IL_111:
				ErrorHelper.AssertRetail(null != tadobject, "can't return null adObject");
				ErrorHelper.AssertRetail(this.transformADObject == null || null != transformedObject, "can't return null transformedObject");
				return tadobject;
			}

			private object lockInstance = new object();

			private DateTime lastRefreshTime;

			private TADObject adObject;

			private TTransformed transformedObject;

			private Func<DateTime, bool> expirationPolicy;

			private Func<IExecutionContext, TADObject> loadADObject;

			private Func<IExecutionContext, TADObject, TTransformed> transformADObject;
		}

		internal class SingletonDirectoryCache<TADObject> : Directory.SingletonDirectoryCache<TADObject, TADObject> where TADObject : class
		{
			public SingletonDirectoryCache(Func<DateTime, bool> expirationPolicy, Func<IExecutionContext, TADObject> loadADObject) : base(expirationPolicy, loadADObject, null)
			{
			}

			public TADObject GetValue(IExecutionContext context)
			{
				TADObject tadobject;
				return base.GetValues(context, out tadobject);
			}
		}

		internal class MailboxInfoCache : TypedMultiKeyCache<MailboxInfo, Guid>, IStoreSimpleQueryTarget<MailboxInfo>, IStoreQueryTargetBase<MailboxInfo>
		{
			public MailboxInfoCache(string name, object syncObject, EvictionPolicy<Guid> evictionPolicy, ICachePerformanceCounters perfCounters) : base(evictionPolicy, perfCounters)
			{
				this.name = name;
				this.syncObject = syncObject;
				this.byGuid = new TypedMultiKeyCache<MailboxInfo, Guid>.KeyDefinition<Guid>(this, evictionPolicy.Capacity);
				base.RegisterKeyDefinition(this.byGuid);
				this.byLegacyDN = new TypedMultiKeyCache<MailboxInfo, Guid>.KeyDefinition<string>(this, evictionPolicy.Capacity);
				base.RegisterKeyDefinition(this.byLegacyDN);
			}

			public TypedMultiKeyCache<MailboxInfo, Guid>.KeyDefinition<Guid> ByGuid
			{
				get
				{
					return this.byGuid;
				}
			}

			public TypedMultiKeyCache<MailboxInfo, Guid>.KeyDefinition<string> ByLegacyDN
			{
				get
				{
					return this.byLegacyDN;
				}
			}

			string IStoreQueryTargetBase<MailboxInfo>.Name
			{
				get
				{
					return this.name;
				}
			}

			Type[] IStoreQueryTargetBase<MailboxInfo>.ParameterTypes
			{
				get
				{
					return Array<Type>.Empty;
				}
			}

			IEnumerable<MailboxInfo> IStoreSimpleQueryTarget<MailboxInfo>.GetRows(object[] parameters)
			{
				IEnumerable<Guid> keys;
				using (LockManager.Lock(this.syncObject))
				{
					keys = this.ByGuid.GetKeys();
				}
				foreach (Guid key in keys)
				{
					MailboxInfo info;
					using (LockManager.Lock(this.syncObject))
					{
						info = this.ByGuid.Find(key);
					}
					if (info != null)
					{
						yield return info;
					}
				}
				yield break;
			}

			private readonly string name;

			private readonly object syncObject;

			private readonly TypedMultiKeyCache<MailboxInfo, Guid>.KeyDefinition<Guid> byGuid;

			private readonly TypedMultiKeyCache<MailboxInfo, Guid>.KeyDefinition<string> byLegacyDN;
		}

		internal class AddressInfoCache : TypedMultiKeyCache<AddressInfo, Guid>, IStoreSimpleQueryTarget<AddressInfo>, IStoreQueryTargetBase<AddressInfo>
		{
			public AddressInfoCache(string name, object syncObject, EvictionPolicy<Guid> evictionPolicy, ICachePerformanceCounters perfCounters) : base(evictionPolicy, perfCounters)
			{
				this.name = name;
				this.syncObject = syncObject;
				this.byGuid = new TypedMultiKeyCache<AddressInfo, Guid>.KeyDefinition<Guid>(this, evictionPolicy.Capacity);
				base.RegisterKeyDefinition(this.byGuid);
				this.byLegacyDN = new TypedMultiKeyCache<AddressInfo, Guid>.KeyDefinition<string>(this, evictionPolicy.Capacity);
				base.RegisterKeyDefinition(this.byLegacyDN);
			}

			public TypedMultiKeyCache<AddressInfo, Guid>.KeyDefinition<Guid> ByGuid
			{
				get
				{
					return this.byGuid;
				}
			}

			public TypedMultiKeyCache<AddressInfo, Guid>.KeyDefinition<string> ByLegacyDN
			{
				get
				{
					return this.byLegacyDN;
				}
			}

			string IStoreQueryTargetBase<AddressInfo>.Name
			{
				get
				{
					return this.name;
				}
			}

			Type[] IStoreQueryTargetBase<AddressInfo>.ParameterTypes
			{
				get
				{
					return Array<Type>.Empty;
				}
			}

			IEnumerable<AddressInfo> IStoreSimpleQueryTarget<AddressInfo>.GetRows(object[] parameters)
			{
				IEnumerable<Guid> keys;
				using (LockManager.Lock(this.syncObject))
				{
					keys = this.ByGuid.GetKeys();
				}
				foreach (Guid key in keys)
				{
					AddressInfo info;
					using (LockManager.Lock(this.syncObject))
					{
						info = this.ByGuid.Find(key);
					}
					if (info != null)
					{
						yield return info;
					}
				}
				yield break;
			}

			private readonly string name;

			private readonly object syncObject;

			private readonly TypedMultiKeyCache<AddressInfo, Guid>.KeyDefinition<Guid> byGuid;

			private readonly TypedMultiKeyCache<AddressInfo, Guid>.KeyDefinition<string> byLegacyDN;
		}

		internal class DatabaseInfoCache : TypedMultiKeyCache<DatabaseInfo, Guid>, IStoreSimpleQueryTarget<DatabaseInfo>, IStoreQueryTargetBase<DatabaseInfo>
		{
			public DatabaseInfoCache(string name, object syncObject, EvictionPolicy<Guid> evictionPolicy, ICachePerformanceCounters perfCounters) : base(evictionPolicy, perfCounters)
			{
				this.name = name;
				this.syncObject = syncObject;
				this.byGuid = new TypedMultiKeyCache<DatabaseInfo, Guid>.KeyDefinition<Guid>(this, evictionPolicy.Capacity);
				base.RegisterKeyDefinition(this.byGuid);
			}

			public TypedMultiKeyCache<DatabaseInfo, Guid>.KeyDefinition<Guid> ByGuid
			{
				get
				{
					return this.byGuid;
				}
			}

			string IStoreQueryTargetBase<DatabaseInfo>.Name
			{
				get
				{
					return this.name;
				}
			}

			Type[] IStoreQueryTargetBase<DatabaseInfo>.ParameterTypes
			{
				get
				{
					return Array<Type>.Empty;
				}
			}

			IEnumerable<DatabaseInfo> IStoreSimpleQueryTarget<DatabaseInfo>.GetRows(object[] parameters)
			{
				IEnumerable<Guid> keys;
				using (LockManager.Lock(this.syncObject))
				{
					keys = this.ByGuid.GetKeys();
				}
				foreach (Guid key in keys)
				{
					DatabaseInfo info;
					using (LockManager.Lock(this.syncObject))
					{
						info = this.ByGuid.Find(key);
					}
					if (info != null)
					{
						yield return info;
					}
				}
				yield break;
			}

			private readonly string name;

			private readonly object syncObject;

			private readonly TypedMultiKeyCache<DatabaseInfo, Guid>.KeyDefinition<Guid> byGuid;
		}

		internal class OrganizationContainerCache : TypedMultiKeyCache<ADObjectWrappers.IADOrganizationContainer, OrganizationId>, IStoreSimpleQueryTarget<ADObjectWrappers.IADOrganizationContainer>, IStoreQueryTargetBase<ADObjectWrappers.IADOrganizationContainer>
		{
			public OrganizationContainerCache(string name, object syncObject, EvictionPolicy<OrganizationId> evictionPolicy, ICachePerformanceCounters perfCounters) : base(evictionPolicy, perfCounters)
			{
				this.name = name;
				this.syncObject = syncObject;
				this.byOrganizationId = new TypedMultiKeyCache<ADObjectWrappers.IADOrganizationContainer, OrganizationId>.KeyDefinition<OrganizationId>(this, evictionPolicy.Capacity);
				base.RegisterKeyDefinition(this.byOrganizationId);
				this.byGuid = new TypedMultiKeyCache<ADObjectWrappers.IADOrganizationContainer, OrganizationId>.KeyDefinition<Guid>(this, evictionPolicy.Capacity);
				base.RegisterKeyDefinition(this.byGuid);
			}

			public TypedMultiKeyCache<ADObjectWrappers.IADOrganizationContainer, OrganizationId>.KeyDefinition<OrganizationId> ByOrganizationId
			{
				get
				{
					return this.byOrganizationId;
				}
			}

			public TypedMultiKeyCache<ADObjectWrappers.IADOrganizationContainer, OrganizationId>.KeyDefinition<Guid> ByGuid
			{
				get
				{
					return this.byGuid;
				}
			}

			string IStoreQueryTargetBase<ADObjectWrappers.IADOrganizationContainer>.Name
			{
				get
				{
					return this.name;
				}
			}

			Type[] IStoreQueryTargetBase<ADObjectWrappers.IADOrganizationContainer>.ParameterTypes
			{
				get
				{
					return Array<Type>.Empty;
				}
			}

			IEnumerable<ADObjectWrappers.IADOrganizationContainer> IStoreSimpleQueryTarget<ADObjectWrappers.IADOrganizationContainer>.GetRows(object[] parameters)
			{
				IEnumerable<OrganizationId> keys;
				using (LockManager.Lock(this.syncObject))
				{
					keys = this.ByOrganizationId.GetKeys();
				}
				foreach (OrganizationId key in keys)
				{
					ADObjectWrappers.IADOrganizationContainer info;
					using (LockManager.Lock(this.syncObject))
					{
						info = this.ByOrganizationId.Find(key);
					}
					if (info != null)
					{
						yield return info;
					}
				}
				yield break;
			}

			private readonly string name;

			private readonly object syncObject;

			private readonly TypedMultiKeyCache<ADObjectWrappers.IADOrganizationContainer, OrganizationId>.KeyDefinition<OrganizationId> byOrganizationId;

			private readonly TypedMultiKeyCache<ADObjectWrappers.IADOrganizationContainer, OrganizationId>.KeyDefinition<Guid> byGuid;
		}

		internal class StoreUserInformationReader : IStoreUserInformationReader
		{
			public static Directory.StoreUserInformationReader Instance
			{
				get
				{
					return Directory.StoreUserInformationReader.instance;
				}
			}

			public object[] ReadUserInformation(Guid databaseGuid, Guid userInformationGuid, uint[] propertyTags)
			{
				object[] result;
				using (Context context = Context.Create(new ExecutionDiagnostics(), Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.ProcessSecurityContext, ClientType.ADDriver, CultureHelper.DefaultCultureInfo))
				{
					try
					{
						if (!DefaultSettings.Get.UserInformationIsEnabled)
						{
							throw new StoreException((LID)41548U, ErrorCodeValue.NotSupported);
						}
						StoreDatabase storeDatabase = Storage.FindDatabase(databaseGuid);
						if (storeDatabase == null)
						{
							throw new StoreException((LID)33612U, ErrorCodeValue.MdbNotInitialized);
						}
						using (storeDatabase.IsSharedLockHeld() ? context.AssociateWithDatabaseNoLock(storeDatabase) : context.AssociateWithDatabase(storeDatabase))
						{
							if (!storeDatabase.IsOnlineActive && !storeDatabase.IsOnlinePassiveAttachedReadOnly)
							{
								throw new StoreException((LID)45644U, ErrorCodeValue.MdbNotInitialized);
							}
							if (!UserInfoUpgrader.IsReady(context, context.Database))
							{
								throw new StoreException((LID)60876U, ErrorCodeValue.NotSupported);
							}
							StorePropTag[] propertyTags2 = LegacyHelper.ConvertFromLegacyPropTags(propertyTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.UserInfo, null, false);
							Properties properties = UserInformation.Read(context, userInformationGuid, propertyTags2);
							result = properties.Select(delegate(Property p)
							{
								if (!p.IsError)
								{
									return p.Value;
								}
								return null;
							}).ToArray<object>();
						}
					}
					catch (StoreException ex)
					{
						context.OnExceptionCatch(ex);
						throw new DataSourceOperationException(DirectoryStrings.FailedToReadStoreUserInformation, ex);
					}
				}
				return result;
			}

			private static readonly Directory.StoreUserInformationReader instance = new Directory.StoreUserInformationReader();
		}
	}
}
