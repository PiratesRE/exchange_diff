using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.DirectoryServices;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	internal static class ADObjectWrappers
	{
		public static ADObjectWrappers.IConcreteFactory Factory
		{
			get
			{
				return ADObjectWrappers.hookableFactory.Value;
			}
		}

		public static ADObjectWrappers.IADSystemConfigurationSession CreateADSystemConfigurationSession(IExecutionContext context, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, string domainController)
		{
			return ADObjectWrappers.Factory.CreateADSystemConfigurationSession(context, consistencyMode, sessionSettings, domainController);
		}

		public static ADObjectWrappers.IADRecipientSession CreateADRecipientSession(IExecutionContext context, ConsistencyMode consistencyMode, TenantHint tenantHint, string domainController, bool bypassSharedCache)
		{
			return ADObjectWrappers.Factory.CreateADRecipientSession(context, consistencyMode, tenantHint, domainController, bypassSharedCache);
		}

		public static ADObjectWrappers.IADOrganizationContainer GetOrganizationContainer(IExecutionContext context, OrganizationId organizationId, string domainController)
		{
			return ADObjectWrappers.Factory.GetOrganizationContainer(context, organizationId, domainController);
		}

		internal static IDisposable SetTestHook(ADObjectWrappers.IConcreteFactory factory)
		{
			return ADObjectWrappers.hookableFactory.SetTestHook(factory);
		}

		private static Hookable<ADObjectWrappers.IConcreteFactory> hookableFactory = Hookable<ADObjectWrappers.IConcreteFactory>.Create(true, new ADObjectWrappers.ADObjectWrapperFactory());

		public interface IConcreteFactory
		{
			ADObjectWrappers.IADSystemConfigurationSession CreateADSystemConfigurationSession(IExecutionContext context, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, string domainController);

			ADObjectWrappers.IADRecipientSession CreateADRecipientSession(IExecutionContext context, ConsistencyMode consistencyMode, TenantHint tenantHint, string domainController, bool bypassSharedCache);

			ADObjectWrappers.IADOrganizationContainer GetOrganizationContainer(IExecutionContext context, OrganizationId organizationId, string domainController);
		}

		public interface IADRecipientSession
		{
			ADRecipient FindByObjectGuid(IExecutionContext context, Guid guid);

			ADRecipient FindByObjectId(IExecutionContext context, Guid objectId);

			ADRecipient FindByExchangeGuidIncludingAlternate(IExecutionContext context, Guid exchangeGuid);

			ADRecipient FindByLegacyExchangeDN(IExecutionContext context, string legacyExchangeDN);

			SecurityDescriptor ReadSecurityDescriptor(IExecutionContext context, ADRecipient adRecipient);

			bool IsMemberOfDistributionList(IExecutionContext context, ADRecipient adRecipient, Guid distributionListObjectGuid);
		}

		public interface IADSystemConfigurationSession
		{
			ADObjectWrappers.IADServer FindLocalServer(IExecutionContext context);

			ADObjectWrappers.IADInformationStore FindLocalInformationStore(IExecutionContext context, ADObjectWrappers.IADServer adServer);

			ADObjectWrappers.IADMailboxDatabase FindDatabaseByGuid(IExecutionContext context, Guid databaseGuid);

			ADObjectWrappers.IADTransportConfigContainer FindTransportConfigContainer(IExecutionContext context);

			ADObjectWrappers.IADOrganizationContainer GetOrganizationContainer(IExecutionContext context);
		}

		public interface IADTransportConfigContainer
		{
			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> MaxSendSize { get; }

			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> MaxReceiveSize { get; }
		}

		public interface IADOrganizationContainer
		{
			Guid ObjectGuid { get; }

			Guid HierarchyMailboxGuid { get; }

			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota { get; }

			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota { get; }

			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize { get; }
		}

		public interface IADServer
		{
			ADObjectId Id { get; }

			Guid Guid { get; }

			string ExchangeLegacyDN { get; }

			string Fqdn { get; }

			LocalLongFullPath InstallPath { get; }

			ServerRole ServerRole { get; }

			int TotalDatabases { get; }

			int? MaxActiveDatabases { get; }

			long ContinuousReplicationMaxMemoryPerDatabase { get; }

			Microsoft.Exchange.Server.Storage.Common.ServerEditionType Edition { get; }

			bool IsDAGMember { get; }

			string Forest { get; }

			SecurityDescriptor ReadSecurityDescriptor(IExecutionContext context);
		}

		public interface IADInformationStore
		{
			int? MaxRpcThreads { get; }

			int MaxRecoveryDatabases { get; }

			int MaxTotalDatabases { get; }

			void LoadDatabaseOptions(IExecutionContext context, DatabaseOptions options);
		}

		public interface IADMailboxDatabase
		{
			Guid Guid { get; }

			string Name { get; }

			Guid DagOrServerGuid { get; }

			string ExchangeLegacyDN { get; }

			string Description { get; }

			string ServerName { get; }

			bool Recovery { get; }

			bool CircularLoggingEnabled { get; }

			bool AllowFileRestore { get; }

			EdbFilePath EdbFilePath { get; }

			NonRootLocalLongFullPath LogFolderPath { get; }

			EnhancedTimeSpan EventHistoryRetentionPeriod { get; }

			EnhancedTimeSpan MailboxRetention { get; }

			string[] HostServerNames { get; }

			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> IssueWarningQuota { get; }

			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> ProhibitSendQuota { get; }

			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota { get; }

			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> RecoverableItemsWarningQuota { get; }

			Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> RecoverableItemsQuota { get; }

			int DataMoveReplicationConstraint { get; }

			SecurityDescriptor ReadSecurityDescriptor(IExecutionContext context);

			void LoadDatabaseOptions(IExecutionContext context, DatabaseOptions options);
		}

		internal class ADObjectWrapperFactory : ADObjectWrappers.IConcreteFactory
		{
			public ADObjectWrappers.IADSystemConfigurationSession CreateADSystemConfigurationSession(IExecutionContext context, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, string domainController)
			{
				return new ADObjectWrappers.ADSystemConfigurationSession(context, consistencyMode, sessionSettings, domainController);
			}

			public ADObjectWrappers.IADRecipientSession CreateADRecipientSession(IExecutionContext context, ConsistencyMode consistencyMode, TenantHint tenantHint, string domainController, bool bypassSharedCache)
			{
				return new ADObjectWrappers.ADRecipientSession(context, consistencyMode, tenantHint, domainController, bypassSharedCache);
			}

			public ADObjectWrappers.IADOrganizationContainer GetOrganizationContainer(IExecutionContext context, OrganizationId organizationId, string domainController)
			{
				if (organizationId == null)
				{
					organizationId = OrganizationId.ForestWideOrgId;
				}
				ADObjectId rootOrgContainerIdForLocalForest = Microsoft.Exchange.Data.Directory.SystemConfiguration.ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerIdForLocalForest, organizationId, organizationId, false);
				ADObjectWrappers.IADSystemConfigurationSession iadsystemConfigurationSession = this.CreateADSystemConfigurationSession(context, ConsistencyMode.FullyConsistent, sessionSettings, domainController);
				return iadsystemConfigurationSession.GetOrganizationContainer(context);
			}
		}

		internal class ADSystemConfigurationSession : ADObjectWrappers.IADSystemConfigurationSession
		{
			public ADSystemConfigurationSession(IExecutionContext context, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings, string domainController)
			{
				this.wrappee = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(domainController, true, consistencyMode, sessionSettings, 668, ".ctor", "f:\\15.00.1497\\sources\\dev\\ManagedStore\\src\\DirectoryServices\\ADObjectWrappers.cs");
			}

			public ADObjectWrappers.IADMailboxDatabase FindDatabaseByGuid(IExecutionContext context, Guid databaseGuid)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation<Guid>(0, 0L, "ADSystemConfigurationSession.FindDatabaseByGuid(databaseGuid={0})", databaseGuid);
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				MailboxDatabase mailboxDatabase = null;
				using (ADExecutionTracker.TrackCall(context, "FindDatabaseByGuid"))
				{
					mailboxDatabase = ((ITopologyConfigurationSession)this.wrappee).FindDatabaseByGuid<MailboxDatabase>(databaseGuid);
				}
				if (mailboxDatabase == null)
				{
					return null;
				}
				return new ADObjectWrappers.DDSCMailboxDatabaseWrapper(mailboxDatabase);
			}

			public ADObjectWrappers.IADServer FindLocalServer(IExecutionContext context)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation(0, 0L, "ADSystemConfigurationSession.FindLocalServer");
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				Server server = null;
				using (ADExecutionTracker.TrackCall(context, "FindLocalServer"))
				{
					server = ((ITopologyConfigurationSession)this.wrappee).FindLocalServer();
				}
				return new ADObjectWrappers.DDSCServerWrapper(server);
			}

			public ADObjectWrappers.IADInformationStore FindLocalInformationStore(IExecutionContext context, ADObjectWrappers.IADServer adServer)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation(0, 0L, "ADSystemConfigurationSession.FindLocalInformationStore");
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				ADObjectWrappers.IADServer iadserver = adServer;
				if (iadserver == null)
				{
					iadserver = this.FindLocalServer(context);
				}
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, this.wrappee.SessionSettings, 790, "FindLocalInformationStore", "f:\\15.00.1497\\sources\\dev\\ManagedStore\\src\\DirectoryServices\\ADObjectWrappers.cs");
				InformationStore informationStore = null;
				using (ADExecutionTracker.TrackCall(context, "FindLocalInformationStore"))
				{
					IEnumerable<InformationStore> source = topologyConfigurationSession.FindPaged<InformationStore>(iadserver.Id, QueryScope.SubTree, null, null, 0);
					informationStore = source.FirstOrDefault<InformationStore>();
				}
				if (informationStore != null)
				{
					return new ADObjectWrappers.DDSCInformationStoreWrapper(informationStore);
				}
				return null;
			}

			public ADObjectWrappers.IADTransportConfigContainer FindTransportConfigContainer(IExecutionContext context)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation(0, 0L, "ADSystemConfigurationSession.FindTransportConfigContainer");
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				TransportConfigContainer transportConfigContainer = null;
				using (ADExecutionTracker.TrackCall(context, "FindTransportConfigContainer"))
				{
					transportConfigContainer = this.wrappee.FindSingletonConfigurationObject<TransportConfigContainer>();
				}
				if (transportConfigContainer != null)
				{
					return new ADObjectWrappers.DDSCTransportConfigContainerWrapper(transportConfigContainer);
				}
				return null;
			}

			public ADObjectWrappers.IADOrganizationContainer GetOrganizationContainer(IExecutionContext context)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation(0, 0L, "ADSystemConfigurationSession.GetOrganizationContainer");
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				Organization orgContainer;
				using (ADExecutionTracker.TrackCall(context, "GetOrganizationContainer"))
				{
					orgContainer = this.wrappee.GetOrgContainer();
				}
				if (orgContainer != null)
				{
					return new ADObjectWrappers.DDSCOrganizationContainerWrapper(orgContainer);
				}
				return null;
			}

			private IConfigurationSession wrappee;
		}

		internal class ADRecipientSession : ADObjectWrappers.IADRecipientSession
		{
			public ADRecipientSession(IExecutionContext context, ConsistencyMode consistencyMode, TenantHint tenantHint, string domainController, bool bypassSharedCache)
			{
				using (ADExecutionTracker.TrackCall(context, "SessionSettingsFromScopeSet"))
				{
					ADSessionSettings adsessionSettings = tenantHint.IsRootOrg ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromTenantPartitionHint(TenantPartitionHint.FromPersistablePartitionHint(tenantHint.TenantHintBlob));
					adsessionSettings.IncludeSoftDeletedObjects = true;
					if (bypassSharedCache || !ConfigurationSchema.UseDirectorySharedCache.Value)
					{
						this.wrappee = DirectorySessionFactory.NonCacheSessionFactory.GetTenantOrRootOrgRecipientSession(domainController, true, consistencyMode, adsessionSettings, 930, ".ctor", "f:\\15.00.1497\\sources\\dev\\ManagedStore\\src\\DirectoryServices\\ADObjectWrappers.cs");
					}
					else
					{
						this.wrappee = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, true, consistencyMode, adsessionSettings, 935, ".ctor", "f:\\15.00.1497\\sources\\dev\\ManagedStore\\src\\DirectoryServices\\ADObjectWrappers.cs");
					}
				}
			}

			public ADRecipient FindByObjectGuid(IExecutionContext context, Guid guid)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation<Guid>(0, 0L, "IRecipientSession.FindByObjectGuid(Guid={0})", guid);
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				ADRecipient result = null;
				using (ADExecutionTracker.TrackCall(context, "FindByObjectGuid"))
				{
					result = this.wrappee.FindByObjectGuid(guid);
				}
				return result;
			}

			public ADRecipient FindByObjectId(IExecutionContext context, Guid objectId)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation<Guid>(0, 0L, "IRecipientSession.FindByObjectId(Id={0})", objectId);
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				ADRecipient result = null;
				using (ADExecutionTracker.TrackCall(context, "FindByObjectId"))
				{
					result = this.wrappee.FindByExchangeObjectId(objectId);
				}
				return result;
			}

			public ADRecipient FindByExchangeGuidIncludingAlternate(IExecutionContext context, Guid exchangeGuid)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation<Guid>(0, 0L, "IRecipientSession.FindByExchangeGuidIncludingAlternate(exchangeGuid={0})", exchangeGuid);
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				ADRecipient result = null;
				using (ADExecutionTracker.TrackCall(context, "FindByExchangeGuidIncludingAlternate"))
				{
					result = this.wrappee.FindByExchangeGuidIncludingAlternate(exchangeGuid);
				}
				return result;
			}

			public ADRecipient FindByLegacyExchangeDN(IExecutionContext context, string legacyExchangeDN)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation<string>(0, 0L, "IRecipientSession.FindByLegacyExchangeDN(legacyExchangeDN={0})", legacyExchangeDN);
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				ADRecipient result = null;
				using (ADExecutionTracker.TrackCall(context, "FindByLegacyExchangeDN"))
				{
					result = this.wrappee.FindByLegacyExchangeDN(legacyExchangeDN);
				}
				return result;
			}

			public SecurityDescriptor ReadSecurityDescriptor(IExecutionContext context, ADRecipient adRecipient)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation<string>(0, 0L, "IRecipientSession.ReadSecurityDescriptor(ADRecipient.LegacyExchangeDN={0})", adRecipient.LegacyExchangeDN);
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				SecurityDescriptor result;
				using (ADExecutionTracker.TrackCall(context, "ReadSecurityDescriptor"))
				{
					result = adRecipient.ReadSecurityDescriptorBlob();
				}
				return result;
			}

			public bool IsMemberOfDistributionList(IExecutionContext context, ADRecipient adRecipient, Guid distributionListObjectGuid)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation<string, Guid>(0, 0L, "IRecipientSession.IsMemberOfDistributionList(ADRecipient.LegacyExchangeDN={0}, DistributionListObjectGuid={1})", adRecipient.LegacyExchangeDN, distributionListObjectGuid);
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				bool result;
				using (ADExecutionTracker.TrackCall(context, "IsMemberOfDistributionList"))
				{
					result = adRecipient.IsMemberOf(new ADObjectId(distributionListObjectGuid), false);
				}
				return result;
			}

			private IRecipientSession wrappee;
		}

		internal class DDSCServerWrapper : ADObjectWrappers.IADServer
		{
			public DDSCServerWrapper(Server wrappee)
			{
				this.wrappee = wrappee;
			}

			public ADObjectId Id
			{
				get
				{
					return this.wrappee.Id;
				}
			}

			public Guid Guid
			{
				get
				{
					return this.wrappee.Guid;
				}
			}

			public string ExchangeLegacyDN
			{
				get
				{
					return this.wrappee.ExchangeLegacyDN;
				}
			}

			public string Fqdn
			{
				get
				{
					return this.wrappee.Fqdn;
				}
			}

			public LocalLongFullPath InstallPath
			{
				get
				{
					return this.wrappee.InstallPath;
				}
			}

			public ServerRole ServerRole
			{
				get
				{
					return this.wrappee.CurrentServerRole;
				}
			}

			public Microsoft.Exchange.Server.Storage.Common.ServerEditionType Edition
			{
				get
				{
					return (Microsoft.Exchange.Server.Storage.Common.ServerEditionType)this.wrappee.Edition;
				}
			}

			public int TotalDatabases
			{
				get
				{
					ADObjectId[] databases = this.wrappee.Databases;
					if (databases == null)
					{
						return 0;
					}
					return databases.Length;
				}
			}

			public int? MaxActiveDatabases
			{
				get
				{
					return this.wrappee.MaximumActiveDatabases;
				}
			}

			public long ContinuousReplicationMaxMemoryPerDatabase
			{
				get
				{
					long? continuousReplicationMaxMemoryPerDatabase = this.wrappee.ContinuousReplicationMaxMemoryPerDatabase;
					if (continuousReplicationMaxMemoryPerDatabase != null)
					{
						return continuousReplicationMaxMemoryPerDatabase.Value;
					}
					return 10485760L;
				}
			}

			public bool IsDAGMember
			{
				get
				{
					ADObjectId databaseAvailabilityGroup = this.wrappee.DatabaseAvailabilityGroup;
					return databaseAvailabilityGroup != null;
				}
			}

			public string Forest
			{
				get
				{
					return ADObjectWrappers.DDSCServerWrapper.GetForest(this.wrappee.Fqdn);
				}
			}

			public SecurityDescriptor ReadSecurityDescriptor(IExecutionContext context)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation<string>(0, 0L, "DDSCServerWrapper.ReadSecurityDescriptor(legacyExchangeDN={0})", this.ExchangeLegacyDN);
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				SecurityDescriptor result = null;
				using (ADExecutionTracker.TrackCall(context, "ReadSecurityDescriptor"))
				{
					result = this.wrappee.ReadSecurityDescriptorBlob();
				}
				return result;
			}

			private static string GetForest(string fqdn)
			{
				if (fqdn != null && fqdn.IndexOf('.') >= 0)
				{
					string[] array = fqdn.Split(new char[]
					{
						'.'
					});
					if (array.Length > 1)
					{
						return array[1].ToUpper();
					}
				}
				return string.Empty;
			}

			private Server wrappee;
		}

		internal class DDSCTransportConfigContainerWrapper : ADObjectWrappers.IADTransportConfigContainer
		{
			public DDSCTransportConfigContainerWrapper(TransportConfigContainer wrappee)
			{
				this.wrappee = wrappee;
			}

			public Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> MaxSendSize
			{
				get
				{
					return this.wrappee.MaxSendSize;
				}
			}

			public Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> MaxReceiveSize
			{
				get
				{
					return this.wrappee.MaxReceiveSize;
				}
			}

			private TransportConfigContainer wrappee;
		}

		internal class DDSCInformationStoreWrapper : ADObjectWrappers.IADInformationStore
		{
			public DDSCInformationStoreWrapper(InformationStore wrappee)
			{
				this.wrappee = wrappee;
			}

			public int? MaxRpcThreads
			{
				get
				{
					return this.wrappee.MaxRpcThreads;
				}
			}

			public int MaxRecoveryDatabases
			{
				get
				{
					return this.wrappee.MaxRestoreStorageGroups;
				}
			}

			public int MaxTotalDatabases
			{
				get
				{
					return this.wrappee.MaxStoresTotal;
				}
			}

			public void LoadDatabaseOptions(IExecutionContext context, DatabaseOptions options)
			{
				options.MinCachePages = this.wrappee.MinCachePages;
				options.MaxCachePages = this.wrappee.MaxCachePages;
				options.EnableOnlineDefragmentation = this.wrappee.EnableOnlineDefragmentation;
				if (options.MinCachePages != null && options.MaxCachePages != null)
				{
					Directory.CheckADObjectIsNotCorrupt((LID)36776U, context, options.MinCachePages <= options.MaxCachePages, "MinCachePages is greater than MaxCachePages for object {0}", this.wrappee.Identity);
				}
			}

			private InformationStore wrappee;
		}

		internal class DDSCOrganizationContainerWrapper : ADObjectWrappers.IADOrganizationContainer
		{
			public DDSCOrganizationContainerWrapper(Organization wrappee)
			{
				this.wrappee = wrappee;
			}

			public Guid ObjectGuid
			{
				get
				{
					return this.wrappee.Guid;
				}
			}

			public Guid HierarchyMailboxGuid
			{
				get
				{
					return this.wrappee.DefaultPublicFolderMailbox.HierarchyMailboxGuid;
				}
			}

			public Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				get
				{
					return this.wrappee.DefaultPublicFolderIssueWarningQuota;
				}
			}

			public Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				get
				{
					return this.wrappee.DefaultPublicFolderProhibitPostQuota;
				}
			}

			public Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				get
				{
					return this.wrappee.DefaultPublicFolderMaxItemSize;
				}
			}

			private Organization wrappee;
		}

		internal class DDSCMailboxDatabaseWrapper : ADObjectWrappers.IADMailboxDatabase
		{
			public DDSCMailboxDatabaseWrapper(MailboxDatabase wrappee)
			{
				this.wrappee = wrappee;
			}

			public bool AllowFileRestore
			{
				get
				{
					return this.wrappee.AllowFileRestore;
				}
			}

			public Guid Guid
			{
				get
				{
					return this.wrappee.Guid;
				}
			}

			public string Name
			{
				get
				{
					return this.wrappee.Name;
				}
			}

			public Guid DagOrServerGuid
			{
				get
				{
					if (this.wrappee.MasterServerOrAvailabilityGroup != null)
					{
						return this.wrappee.MasterServerOrAvailabilityGroup.ObjectGuid;
					}
					return Guid.Empty;
				}
			}

			public string ExchangeLegacyDN
			{
				get
				{
					return this.wrappee.ExchangeLegacyDN;
				}
			}

			public string Description
			{
				get
				{
					return this.wrappee.Description;
				}
			}

			public string ServerName
			{
				get
				{
					return this.wrappee.ServerName;
				}
			}

			public bool Recovery
			{
				get
				{
					return this.wrappee.Recovery;
				}
			}

			public bool CircularLoggingEnabled
			{
				get
				{
					return this.wrappee.CircularLoggingEnabled;
				}
			}

			public EdbFilePath EdbFilePath
			{
				get
				{
					return this.wrappee.EdbFilePath;
				}
			}

			public NonRootLocalLongFullPath LogFolderPath
			{
				get
				{
					return this.wrappee.LogFolderPath;
				}
			}

			public EnhancedTimeSpan EventHistoryRetentionPeriod
			{
				get
				{
					return this.wrappee.EventHistoryRetentionPeriod;
				}
			}

			public EnhancedTimeSpan MailboxRetention
			{
				get
				{
					return this.wrappee.MailboxRetention;
				}
			}

			public string[] HostServerNames
			{
				get
				{
					string[] array = Array<string>.Empty;
					DatabaseCopy[] databaseCopies = this.wrappee.DatabaseCopies;
					if (databaseCopies != null && databaseCopies.Length > 0)
					{
						array = new string[databaseCopies.Length];
						for (int i = 0; i < databaseCopies.Length; i++)
						{
							array[i] = databaseCopies[i].HostServerName;
						}
					}
					return array;
				}
			}

			public Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> IssueWarningQuota
			{
				get
				{
					return this.wrappee.IssueWarningQuota;
				}
			}

			public Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> ProhibitSendQuota
			{
				get
				{
					return this.wrappee.ProhibitSendQuota;
				}
			}

			public Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota
			{
				get
				{
					return this.wrappee.ProhibitSendReceiveQuota;
				}
			}

			public Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> RecoverableItemsWarningQuota
			{
				get
				{
					return this.wrappee.RecoverableItemsWarningQuota;
				}
			}

			public Microsoft.Exchange.Data.Unlimited<ByteQuantifiedSize> RecoverableItemsQuota
			{
				get
				{
					return this.wrappee.RecoverableItemsQuota;
				}
			}

			public int DataMoveReplicationConstraint
			{
				get
				{
					return (int)this.wrappee.DataMoveReplicationConstraint;
				}
			}

			public SecurityDescriptor ReadSecurityDescriptor(IExecutionContext context)
			{
				if (ExTraceGlobals.ADCallsTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.ADCallsTracer.TraceInformation<string>(0, 0L, "DDSCMailboxDatabaseWrapper.ReadSecurityDescriptor(legacyExchangeDN={0})", this.ExchangeLegacyDN);
				}
				if (ExTraceGlobals.CallStackTracer.IsTraceEnabled(TraceType.InfoTrace))
				{
					ExTraceGlobals.CallStackTracer.TraceInformation(0, 0L, new StackTrace(true).ToString());
				}
				SecurityDescriptor result = null;
				using (ADExecutionTracker.TrackCall(context, "ReadSecurityDescriptor"))
				{
					result = this.wrappee.ReadSecurityDescriptorBlob();
				}
				return result;
			}

			public void LoadDatabaseOptions(IExecutionContext context, DatabaseOptions options)
			{
				bool? flag = new bool?(this.wrappee.BackgroundDatabaseMaintenance);
				if (flag != null)
				{
					options.BackgroundDatabaseMaintenance = flag.Value;
				}
				flag = this.wrappee.ReplayBackgroundDatabaseMaintenance;
				if (flag != null)
				{
					options.ReplayBackgroundDatabaseMaintenance = flag;
				}
				flag = this.wrappee.BackgroundDatabaseMaintenanceSerialization;
				if (flag != null)
				{
					options.BackgroundDatabaseMaintenanceSerialization = flag;
				}
				int? num = this.wrappee.BackgroundDatabaseMaintenanceDelay;
				if (num != null)
				{
					options.BackgroundDatabaseMaintenanceDelay = num;
				}
				num = this.wrappee.ReplayBackgroundDatabaseMaintenanceDelay;
				if (num != null)
				{
					options.ReplayBackgroundDatabaseMaintenanceDelay = num;
				}
				num = this.wrappee.MimimumBackgroundDatabaseMaintenanceInterval;
				if (num != null)
				{
					options.MimimumBackgroundDatabaseMaintenanceInterval = num;
				}
				num = this.wrappee.MaximumBackgroundDatabaseMaintenanceInterval;
				if (num != null)
				{
					options.MaximumBackgroundDatabaseMaintenanceInterval = num;
				}
				if (this.wrappee.TemporaryDataFolderPath != null)
				{
					string pathName = this.wrappee.TemporaryDataFolderPath.PathName;
					if (!string.IsNullOrEmpty(pathName))
					{
						options.TemporaryDataFolderPath = pathName;
					}
				}
				if (this.wrappee.LogFilePrefix != null)
				{
					string logFilePrefix = this.wrappee.LogFilePrefix;
					if (!string.IsNullOrEmpty(logFilePrefix))
					{
						options.LogFilePrefix = logFilePrefix;
					}
				}
				num = this.wrappee.LogBuffers;
				if (num != null)
				{
					options.LogBuffers = num;
				}
				num = this.wrappee.MaximumOpenTables;
				if (num != null)
				{
					options.MaximumOpenTables = num;
				}
				num = this.wrappee.MaximumTemporaryTables;
				if (num != null)
				{
					options.MaximumTemporaryTables = num;
				}
				num = this.wrappee.MaximumCursors;
				if (num != null)
				{
					options.MaximumCursors = num;
				}
				num = this.wrappee.MaximumSessions;
				if (num != null)
				{
					options.MaximumSessions = num;
				}
				num = this.wrappee.MaximumVersionStorePages;
				if (num != null)
				{
					options.MaximumVersionStorePages = num;
				}
				num = this.wrappee.PreferredVersionStorePages;
				if (num != null)
				{
					options.PreferredVersionStorePages = num;
				}
				num = this.wrappee.DatabaseExtensionSize;
				if (num != null)
				{
					options.DatabaseExtensionSize = num;
				}
				num = this.wrappee.LogCheckpointDepth;
				if (num != null)
				{
					options.LogCheckpointDepth = num;
				}
				num = this.wrappee.ReplayCheckpointDepth;
				if (num != null)
				{
					options.ReplayCheckpointDepth = num;
				}
				num = this.wrappee.CachedClosedTables;
				if (num != null)
				{
					options.CachedClosedTables = num;
				}
				num = this.wrappee.CachePriority;
				if (num != null)
				{
					options.CachePriority = num;
				}
				num = this.wrappee.ReplayCachePriority;
				if (num != null)
				{
					options.ReplayCachePriority = num;
				}
				num = this.wrappee.MaximumPreReadPages;
				if (num != null)
				{
					options.MaximumPreReadPages = num;
				}
				num = this.wrappee.MaximumReplayPreReadPages;
				if (num != null)
				{
					options.MaximumReplayPreReadPages = num;
				}
			}

			private MailboxDatabase wrappee;
		}
	}
}
