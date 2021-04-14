using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.MailboxSignature;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class MailboxProviderBase : DisposeTrackableBase, IMailbox, IDisposable, ISettingsContextProvider
	{
		public MailboxProviderBase(LocalMailboxFlags flags)
		{
			this.TestIntegration = new TestIntegration(false);
			this.MdbId = null;
			this.ConfiguredMdbName = null;
			this.ConfiguredMdbGuid = Guid.Empty;
			this.MailboxGuid = Guid.Empty;
			this.PrimaryMailboxGuid = Guid.Empty;
			this.MailboxId = null;
			this.DomainControllerName = null;
			this.ConfigDomainControllerName = null;
			this.MailboxDN = null;
			this.TraceMailboxId = null;
			this.Credential = null;
			this.ServerDN = null;
			this.ServerDisplayName = null;
			this.ServerFqdn = null;
			this.ServerGuid = Guid.Empty;
			this.wkpMapper = new WellKnownPrincipalMapper();
			this.publicFoldersToSkip = new EntryIdMap<bool>();
			this.Flags = flags;
			this.Options = MailboxOptions.None;
			this.RestoreFlags = MailboxRestoreType.None;
			this.useMdbQuotaDefaults = null;
			this.recipientType = 0;
			this.recipientDisplayType = 0;
			this.recipientTypeDetails = 0L;
			this.mbxQuota = null;
			this.mbxDumpsterQuota = null;
			this.mbxArchiveQuota = null;
			this.archiveGuid = Guid.Empty;
			this.alternateMailboxes = null;
			this.mbxHomeMdb = null;
			this.preferredDomainControllerName = null;
			this.syncState = null;
			this.configContext = null;
			this.MRSVersion = VersionInformation.MRSProxy;
		}

		protected string ConfiguredMdbName { get; set; }

		protected bool IsE15OrHigher
		{
			get
			{
				return this.ServerVersion >= Server.E15MinVersion;
			}
		}

		public bool IsPublicFolderMailbox
		{
			get
			{
				return this.recipientTypeDetails == 68719476736L || this.IsPublicFolderMailboxRestore;
			}
		}

		public abstract int ServerVersion { get; protected set; }

		public MailboxRelease ServerMailboxRelease { get; protected set; }

		public VersionInformation OtherSideVersion { get; private set; }

		public VersionInformation MRSVersion { get; internal set; }

		public ADObjectId MdbId { get; protected set; }

		public Guid MdbGuid
		{
			get
			{
				if (this.MdbId == null)
				{
					return Guid.Empty;
				}
				return this.MdbId.ObjectGuid;
			}
		}

		public Guid ConfiguredMdbGuid { get; protected set; }

		public Guid MbxHomeMdbGuid
		{
			get
			{
				if (this.mbxHomeMdb == null)
				{
					return Guid.Empty;
				}
				return this.mbxHomeMdb.ObjectGuid;
			}
		}

		public Guid MbxArchiveMdbGuid
		{
			get
			{
				if (this.archiveMdb == null)
				{
					return Guid.Empty;
				}
				return this.archiveMdb.ObjectGuid;
			}
		}

		public Guid? MailboxContainerGuid { get; private set; }

		public Guid MailboxGuid { get; private set; }

		public TenantPartitionHint PartitionHint { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		public string MailboxDN { get; protected set; }

		public NetworkCredential Credential { get; protected set; }

		public string DomainControllerName { get; protected set; }

		public string ConfigDomainControllerName { get; protected set; }

		public ADObjectId MailboxId { get; protected set; }

		public string TraceMailboxId { get; protected set; }

		public string TraceMdbId
		{
			get
			{
				if (this.MdbId == null)
				{
					return "(null)";
				}
				return this.MdbId.ToString();
			}
		}

		public MailboxType MbxType { get; private set; }

		public LocalMailboxFlags Flags { get; protected set; }

		public MailboxOptions Options { get; private set; }

		public Guid PrimaryMailboxGuid { get; private set; }

		public bool IsPrimaryMailbox
		{
			get
			{
				return this.MailboxGuid == this.PrimaryMailboxGuid;
			}
		}

		public bool IsAggregatedMailbox
		{
			get
			{
				return this.Flags.HasFlag(LocalMailboxFlags.AggregatedMailbox);
			}
		}

		public bool IsArchiveMailbox
		{
			get
			{
				return !this.IsPrimaryMailbox && !this.IsAggregatedMailbox;
			}
		}

		public ResourceHealthTracker RHTracker
		{
			get
			{
				if (this.rhTracker == null)
				{
					this.rhTracker = new ResourceHealthTracker(this.reservation);
				}
				return this.rhTracker;
			}
		}

		public TestIntegration TestIntegration { get; private set; }

		public string ServerDisplayName { get; protected set; }

		public string ServerDN { get; protected set; }

		public Guid ServerGuid { get; protected set; }

		public string ServerFqdn { get; protected set; }

		public bool UseHomeMDB
		{
			get
			{
				return this.Flags.HasFlag(LocalMailboxFlags.UseHomeMDB);
			}
		}

		public bool IsFolderMove
		{
			get
			{
				return this.Flags.HasFlag(LocalMailboxFlags.FolderMove);
			}
		}

		public bool IsPublicFolderMove
		{
			get
			{
				return this.Flags.HasFlag(LocalMailboxFlags.PublicFolderMove);
			}
		}

		public bool IsPublicFolderMailboxRestore
		{
			get
			{
				return this.RestoreFlags.HasFlag(MailboxRestoreType.PublicFolderMailbox);
			}
		}

		public bool IsPureMAPI
		{
			get
			{
				return this.Flags.HasFlag(LocalMailboxFlags.PureMAPI);
			}
		}

		public bool IsRestore
		{
			get
			{
				return this.Flags.HasFlag(LocalMailboxFlags.Restore);
			}
		}

		public bool IsMove
		{
			get
			{
				return this.Flags.HasFlag(LocalMailboxFlags.Move);
			}
		}

		public bool IsOlcSync
		{
			get
			{
				return this.Flags.HasFlag(LocalMailboxFlags.OlcSync);
			}
		}

		public bool IsPublicFolderMigrationSource
		{
			get
			{
				return this.Flags.HasFlag(LocalMailboxFlags.LegacyPublicFolders);
			}
		}

		public bool IsTitanium
		{
			get
			{
				return this.ServerVersion < Server.E2007MinVersion;
			}
		}

		public bool IsExchange2007
		{
			get
			{
				return this.ServerVersion >= Server.E2007MinVersion && this.ServerVersion < Server.E14MinVersion;
			}
		}

		private protected MailboxRestoreType RestoreFlags { protected get; private set; }

		protected string EffectiveDomainControllerName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.preferredDomainControllerName))
				{
					return this.preferredDomainControllerName;
				}
				return this.DomainControllerName;
			}
		}

		public virtual MapiSyncState SyncState
		{
			get
			{
				return this.syncState;
			}
			protected set
			{
				this.syncState = value;
			}
		}

		internal virtual bool SupportsSavingSyncState
		{
			get
			{
				return false;
			}
		}

		public abstract SyncProtocol GetSyncProtocol();

		ISettingsContext ISettingsContextProvider.GetSettingsContext()
		{
			return this.configContext;
		}

		LatencyInfo IMailbox.GetLatencyInfo()
		{
			return new LatencyInfo();
		}

		void IMailbox.Config(IReservation reservation, Guid primaryMailboxGuid, Guid physicalMailboxGuid, TenantPartitionHint partitionHint, Guid mdbGuid, MailboxType mbxType, Guid? mailboxContainerGuid)
		{
			this.ConfiguredMdbGuid = mdbGuid;
			this.ConfiguredMdbName = null;
			this.MailboxGuid = physicalMailboxGuid;
			this.PrimaryMailboxGuid = primaryMailboxGuid;
			this.MbxType = mbxType;
			this.MailboxDN = null;
			this.TraceMailboxId = null;
			this.MailboxId = null;
			this.MdbId = null;
			this.PartitionHint = partitionHint;
			this.MailboxContainerGuid = mailboxContainerGuid;
			if (reservation != null)
			{
				this.reservation = (ReservationManager.FindReservation(reservation.Id) as MailboxReservation);
				this.reservation.Activate(this.MailboxGuid);
			}
		}

		void IMailbox.ConfigRestore(MailboxRestoreType restoreFlags)
		{
			this.RestoreFlags = restoreFlags;
		}

		bool IMailbox.IsCapabilitySupported(MRSProxyCapabilities capability)
		{
			return true;
		}

		void IMailbox.UpdateMovedMailbox(UpdateMovedMailboxOperation op, ADUser remoteRecipientData, string domainController, out ReportEntry[] entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, ArchiveStatusFlags archiveStatus, UpdateMovedMailboxFlags updateMovedMailboxFlags, Guid? newMailboxContainerGuid, CrossTenantObjectId newUnifiedMailboxId)
		{
			MrsTracer.Provider.Function("IMailbox.UpdateMovedMailbox(op={0}, remoteRecipientData={1}, dc={2}, newMDB={3}, newArchiveMDB={4}, archiveDomain={5}, archiveStatus={6}, updateMovedMailboxFlags={7}, newMailboxContainerGuid={8}, newUnifiedMailboxId={9})", new object[]
			{
				op,
				remoteRecipientData,
				domainController,
				newDatabaseGuid,
				newArchiveDatabaseGuid,
				archiveDomain,
				archiveStatus,
				updateMovedMailboxFlags,
				newMailboxContainerGuid,
				newUnifiedMailboxId
			});
			if (this.IsPureMAPI || this.IsRestore)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			this.VerifyMailboxConnection(VerifyMailboxConnectionFlags.MailboxSessionNotRequired);
			entries = null;
			List<ReportEntry> list = new List<ReportEntry>();
			try
			{
				using (MrsPSHandler mrsPSHandler = new MrsPSHandler("StorageMailbox.UpdateMovedMailbox Monad"))
				{
					mrsPSHandler.MonadConnection.RunspaceProxy.SetVariable("UMM_UpdateSucceeded", false);
					mrsPSHandler.MonadConnection.RunspaceProxy.SetVariable("UMM_DCName", null);
					mrsPSHandler.MonadConnection.RunspaceProxy.SetVariable("UMM_ReportEntries", list);
					bool flag = updateMovedMailboxFlags.HasFlag(UpdateMovedMailboxFlags.MakeExoPrimary);
					using (MonadCommand command = mrsPSHandler.GetCommand(flag ? MrsCmdlet.SetConsumerMailbox : MrsCmdlet.UpdateMovedMailbox))
					{
						if (flag)
						{
							command.Parameters.Add(new MonadParameter("Identity", new ConsumerMailboxIdParameter(this.MailboxId)));
							command.Parameters.AddSwitch("MakeExoPrimary");
						}
						else
						{
							command.Parameters.Add(new MonadParameter("Identity", new MailboxOrMailUserIdParameter(this.MailboxId)));
							if (this.PartitionHint != null)
							{
								command.Parameters.Add(new MonadParameter("PartitionHint", this.PartitionHint.GetPersistablePartitionHint()));
							}
							if (newDatabaseGuid == Guid.Empty && newArchiveDatabaseGuid == Guid.Empty && (op == UpdateMovedMailboxOperation.UpdateMailbox || op == UpdateMovedMailboxOperation.MorphToMailbox))
							{
								newDatabaseGuid = this.MdbId.ObjectGuid;
								newArchiveDatabaseGuid = this.MdbId.ObjectGuid;
							}
							command.Parameters.Add(new MonadParameter("NewArchiveMDB", newArchiveDatabaseGuid));
							command.Parameters.Add(new MonadParameter("ArchiveDomain", archiveDomain));
							command.Parameters.Add(new MonadParameter("ArchiveStatus", archiveStatus));
							switch (op)
							{
							case UpdateMovedMailboxOperation.UpdateMailbox:
								command.Parameters.AddSwitch("UpdateMailbox");
								command.Parameters.Add(new MonadParameter("NewHomeMDB", newDatabaseGuid));
								command.Parameters.Add(new MonadParameter("NewContainerGuid", newMailboxContainerGuid));
								command.Parameters.Add(new MonadParameter("NewUnifiedMailboxId", newUnifiedMailboxId));
								if ((updateMovedMailboxFlags & UpdateMovedMailboxFlags.SkipMailboxReleaseCheck) != UpdateMovedMailboxFlags.None)
								{
									command.Parameters.AddSwitch("SkipMailboxReleaseCheck");
								}
								if ((updateMovedMailboxFlags & UpdateMovedMailboxFlags.SkipProvisioningCheck) != UpdateMovedMailboxFlags.None)
								{
									command.Parameters.AddSwitch("SkipProvisioningCheck");
								}
								break;
							case UpdateMovedMailboxOperation.MorphToMailbox:
								command.Parameters.AddSwitch("MorphToMailbox");
								command.Parameters.Add(new MonadParameter("NewHomeMDB", newDatabaseGuid));
								command.Parameters.Add(new MonadParameter("RemoteRecipientData", remoteRecipientData));
								if ((updateMovedMailboxFlags & UpdateMovedMailboxFlags.SkipProvisioningCheck) != UpdateMovedMailboxFlags.None)
								{
									command.Parameters.AddSwitch("SkipProvisioningCheck");
								}
								break;
							case UpdateMovedMailboxOperation.MorphToMailUser:
								command.Parameters.AddSwitch("MorphToMailUser");
								command.Parameters.Add(new MonadParameter("RemoteRecipientData", remoteRecipientData));
								break;
							case UpdateMovedMailboxOperation.UpdateArchiveOnly:
								command.Parameters.AddSwitch("UpdateArchiveOnly");
								if (remoteRecipientData != null)
								{
									command.Parameters.Add(new MonadParameter("RemoteRecipientData", remoteRecipientData));
								}
								if ((updateMovedMailboxFlags & UpdateMovedMailboxFlags.SkipMailboxReleaseCheck) != UpdateMovedMailboxFlags.None)
								{
									command.Parameters.AddSwitch("SkipMailboxReleaseCheck");
								}
								break;
							default:
								throw new UpdateMovedMailboxPermanentException();
							}
							if (!string.IsNullOrEmpty(this.DomainControllerName))
							{
								command.Parameters.Add(new MonadParameter("DomainController", this.DomainControllerName));
								PSCredential value = null;
								if (this.Credential != null)
								{
									SecureString secureString = new SecureString();
									foreach (char c in this.Credential.Password)
									{
										secureString.AppendChar(c);
									}
									string text = this.Credential.UserName;
									if (!string.IsNullOrEmpty(this.Credential.Domain))
									{
										text = this.Credential.Domain + "\\" + text;
									}
									value = new PSCredential(text, secureString);
								}
								command.Parameters.Add(new MonadParameter("Credential", value));
								command.Parameters.Add(new MonadParameter("ConfigDomainController", this.ConfigDomainControllerName));
							}
							else if (!string.IsNullOrEmpty(domainController))
							{
								command.Parameters.Add(new MonadParameter("DomainController", domainController));
							}
						}
						bool flag2 = false;
						command.ErrorReport += MailboxProviderBase.ummErrorReportHandler;
						try
						{
							try
							{
								command.Execute();
							}
							finally
							{
								flag2 = (bool)mrsPSHandler.MonadConnection.RunspaceProxy.GetVariable("UMM_UpdateSucceeded");
								if (flag2)
								{
									this.preferredDomainControllerName = (string)mrsPSHandler.MonadConnection.RunspaceProxy.GetVariable("UMM_DCName");
								}
							}
						}
						catch (MonadDataAdapterInvocationException ex)
						{
							LocalizedException ex2 = this.ClassifyWrapAndReturnPowershellException(ex);
							if (!flag2)
							{
								throw ex2;
							}
							list.Add(new ReportEntry(MrsStrings.ReportUpdateMovedMailboxFailureAfterADSwitchover(new LocalizedString(CommonUtils.GetFailureType(ex))), ReportEntryType.Warning, ex2, ReportEntryFlags.Cleanup));
						}
						catch (CmdletInvocationException ex3)
						{
							LocalizedException ex4 = this.ClassifyWrapAndReturnPowershellException(ex3);
							if (!flag2)
							{
								throw ex4;
							}
							list.Add(new ReportEntry(MrsStrings.ReportUpdateMovedMailboxFailureAfterADSwitchover(new LocalizedString(CommonUtils.GetFailureType(ex3))), ReportEntryType.Warning, ex4, ReportEntryFlags.Cleanup));
						}
						if (!flag && !flag2)
						{
							throw this.ClassifyWrapAndReturnPowershellException((mrsPSHandler.ExceptionsReported.Count > 0) ? mrsPSHandler.ExceptionsReported[0] : null);
						}
						foreach (ReportEntry reportEntry in list)
						{
							if (reportEntry.Type == ReportEntryType.Error)
							{
								reportEntry.Type = ReportEntryType.WarningCondition;
							}
						}
						if (this.TestIntegration.UpdateMoveRequestFailsAfterStampingHomeMdb)
						{
							throw new UpdateMovedMailboxPermanentException(new Exception("Failing UpdateMoveRequest due to a test hook"));
						}
					}
					list.AddRange(mrsPSHandler.ReportEntries);
				}
			}
			finally
			{
				entries = list.ToArray();
			}
		}

		MappedPrincipal[] IMailbox.ResolvePrincipals(MappedPrincipal[] principals)
		{
			this.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			if (this.IsPureMAPI)
			{
				return new MappedPrincipal[principals.Length];
			}
			List<MappedPrincipal> result = new List<MappedPrincipal>(principals.Length);
			this.RunADRecipientOperation(true, delegate(IRecipientSession adSession)
			{
				IRecipientSession recipientSession = this.GetRecipientSession(true, true);
				this.wkpMapper.Initialize(recipientSession);
				MappedPrincipal[] principals2 = principals;
				for (int i = 0; i < principals2.Length; i++)
				{
					MappedPrincipal principal = principals2[i];
					ExecutionContext.Create(new DataContext[]
					{
						new SimpleValueDataContext("Principal", principal)
					}).Execute(delegate
					{
						ADRawEntry adrawEntry = null;
						if (principal.HasField(MappedPrincipalFields.MailboxGuid))
						{
							MrsTracer.Provider.Debug("Looking up principal by mailboxGuid {0}", new object[]
							{
								principal.MailboxGuid
							});
							SecurityIdentifier securityIdentifier = this.wkpMapper[principal.MailboxGuid];
							if (securityIdentifier != null)
							{
								MrsTracer.Provider.Debug("Found well-known principal '{0}'", new object[]
								{
									securityIdentifier
								});
								MappedPrincipal mappedPrincipal = new MappedPrincipal();
								mappedPrincipal.MailboxGuid = principal.MailboxGuid;
								mappedPrincipal.ObjectSid = securityIdentifier;
								result.Add(mappedPrincipal);
								return;
							}
							adrawEntry = adSession.FindByExchangeGuid(principal.MailboxGuid, MappedPrincipal.PrincipalProperties);
						}
						if (adrawEntry == null && principal.HasField(MappedPrincipalFields.ObjectGuid))
						{
							MrsTracer.Provider.Debug("Looking up principal by objectGuid {0}", new object[]
							{
								principal.ObjectGuid
							});
							adrawEntry = adSession.ReadADRawEntry(new ADObjectId(principal.ObjectGuid), MappedPrincipal.PrincipalProperties);
						}
						if (adrawEntry == null && principal.HasField(MappedPrincipalFields.ObjectSid))
						{
							MrsTracer.Provider.Debug("Looking up principal by SID {0}", new object[]
							{
								principal.ObjectSid
							});
							Guid guid = this.wkpMapper[principal.ObjectSid];
							if (guid != Guid.Empty)
							{
								MappedPrincipal mappedPrincipal2 = new MappedPrincipal();
								mappedPrincipal2.MailboxGuid = guid;
								mappedPrincipal2.ObjectSid = principal.ObjectSid;
								result.Add(mappedPrincipal2);
								return;
							}
							try
							{
								adrawEntry = adSession.FindADRawEntryBySid(principal.ObjectSid, MappedPrincipal.PrincipalProperties);
							}
							catch (NonUniqueRecipientException)
							{
								MrsTracer.Provider.Debug("More than one recipient found for SID {0}, ignoring.", new object[]
								{
									principal.ObjectSid
								});
							}
						}
						if (adrawEntry == null && principal.HasField(MappedPrincipalFields.LegacyDN))
						{
							MrsTracer.Provider.Debug("Looking up principal by LegDN or X500 proxy '{0}'", new object[]
							{
								principal.LegacyDN
							});
							ProxyAddress proxyAddress = new CustomProxyAddress((CustomProxyAddressPrefix)ProxyAddressPrefix.LegacyDN, principal.LegacyDN, true);
							try
							{
								adrawEntry = adSession.FindByProxyAddress(proxyAddress, MappedPrincipal.PrincipalProperties);
							}
							catch (NonUniqueRecipientException)
							{
								MrsTracer.Provider.Debug("More than one recipient found for LegDN '{0}', ignoring.", new object[]
								{
									principal.LegacyDN
								});
							}
						}
						if (adrawEntry == null && principal.HasField(MappedPrincipalFields.ProxyAddresses))
						{
							MrsTracer.Provider.Debug("Looking up principal by proxies [{0}]", new object[]
							{
								string.Join(",", principal.ProxyAddresses)
							});
							ProxyAddressCollection proxyAddressCollection = new ProxyAddressCollection(principal.ProxyAddresses);
							Result<ADRawEntry>[] array = adSession.FindByProxyAddresses(proxyAddressCollection.ToArray(), MappedPrincipal.PrincipalProperties);
							if (array != null && array.Length == 1)
							{
								adrawEntry = array[0].Data;
							}
						}
						List<ADRawEntry> list = new List<ADRawEntry>();
						if (adrawEntry != null)
						{
							list.Add(adrawEntry);
						}
						if (adrawEntry == null && principal.HasField(MappedPrincipalFields.Alias))
						{
							MrsTracer.Provider.Debug("Looking up principal by ID '{0}'", new object[]
							{
								principal.Alias
							});
							RecipientIdParameter recipientIdParameter = new RecipientIdParameter(principal.Alias);
							IEnumerable<ADRecipient> objects = recipientIdParameter.GetObjects<ADRecipient>(null, adSession);
							if (objects != null)
							{
								foreach (ADRecipient adrecipient in objects)
								{
									if (adrecipient != null)
									{
										list.Add(adrecipient);
									}
								}
							}
						}
						MappedPrincipal mappedPrincipal3 = null;
						foreach (ADRawEntry adrawEntry2 in list)
						{
							MrsTracer.Provider.Debug("Found principal '{0}'", new object[]
							{
								adrawEntry2.Identity
							});
							mappedPrincipal3 = new MappedPrincipal(adrawEntry2)
							{
								NextEntry = mappedPrincipal3
							};
						}
						if (mappedPrincipal3 == null)
						{
							MrsTracer.Provider.Debug("Unable to locate principal", new object[0]);
						}
						result.Add(mappedPrincipal3);
					});
				}
			});
			return result.ToArray();
		}

		Guid[] IMailbox.ResolvePolicyTag(string policyTagStr)
		{
			this.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			if (this.IsPureMAPI)
			{
				return null;
			}
			IEnumerable<RetentionPolicyTag> results = null;
			this.RunADRecipientOperation(true, delegate(IRecipientSession adSession)
			{
				RetentionPolicyTagIdParameter retentionPolicyTagIdParameter = new RetentionPolicyTagIdParameter(policyTagStr);
				results = retentionPolicyTagIdParameter.GetObjects<RetentionPolicyTag>(null, adSession);
			});
			if (results == null)
			{
				return null;
			}
			List<Guid> list = new List<Guid>();
			foreach (RetentionPolicyTag retentionPolicyTag in results)
			{
				list.Add(retentionPolicyTag.Guid);
			}
			return list.ToArray();
		}

		ADUser IMailbox.GetADUser()
		{
			MrsTracer.Provider.Function("IMailbox.GetADUser", new object[0]);
			ADUser adUser = null;
			this.RunADRecipientOperation(false, delegate(IRecipientSession adSession)
			{
				adUser = (adSession.Read(this.MailboxId) as ADUser);
			});
			if (adUser == null)
			{
				throw new RecipientNotFoundPermanentException(this.MailboxGuid);
			}
			return adUser;
		}

		bool IMailbox.UpdateRemoteHostName(string value)
		{
			MrsTracer.Provider.Function("UpdateRemoteHostName({0})", new object[]
			{
				value
			});
			this.RunADRecipientOperation(false, delegate(IRecipientSession adSession)
			{
				ADUser aduser = adSession.Read(this.MailboxId) as ADUser;
				if (aduser == null)
				{
					throw new RecipientNotFoundPermanentException(this.MailboxGuid);
				}
				aduser.MailboxMoveRemoteHostName = value;
				adSession.Save(aduser);
			});
			return true;
		}

		void IMailbox.ConfigMDBByName(string mdbName)
		{
			this.ConfiguredMdbGuid = Guid.Empty;
			this.ConfiguredMdbName = mdbName;
			this.MdbId = null;
		}

		void IMailbox.ConfigADConnection(string domainControllerName, string configDomainControllerName, NetworkCredential cred)
		{
			this.DomainControllerName = domainControllerName;
			this.ConfigDomainControllerName = configDomainControllerName;
			this.Credential = cred;
		}

		void IMailbox.ConfigMailboxOptions(MailboxOptions options)
		{
			this.Options = options;
		}

		void IMailbox.ConfigPreferredADConnection(string preferredDomainControllerName)
		{
			this.preferredDomainControllerName = preferredDomainControllerName;
		}

		void IMailbox.ConfigOlc(OlcMailboxConfiguration config)
		{
			throw new NotImplementedException();
		}

		MailboxServerInformation IMailbox.GetMailboxServerInformation()
		{
			MrsTracer.Provider.Function("MapiMailbox.GetMailboxServerInformation", new object[0]);
			MailboxServerInformation mailboxServerInformation = new MailboxServerInformation();
			mailboxServerInformation.MailboxServerName = this.ServerDisplayName;
			mailboxServerInformation.MailboxServerVersion = this.ServerVersion;
			mailboxServerInformation.MailboxServerGuid = this.ServerGuid;
			mailboxServerInformation.ProxyServerName = null;
			mailboxServerInformation.ProxyServerVersion = null;
			if (!this.IsPureMAPI)
			{
				using (ExRpcAdmin rpcAdmin = this.GetRpcAdmin())
				{
					using (this.RHTracker.Start())
					{
						mailboxServerInformation.MailboxSignatureVersion = rpcAdmin.GetMailboxSignatureServerVersion();
						mailboxServerInformation.DeleteMailboxVersion = rpcAdmin.GetDeleteMailboxServerVersion();
						mailboxServerInformation.InTransitStatusVersion = rpcAdmin.GetInTransitStatusServerVersion();
						mailboxServerInformation.MailboxShapeVersion = rpcAdmin.GetMailboxShapeServerVersion();
					}
				}
			}
			return mailboxServerInformation;
		}

		void IMailbox.DeleteMailbox(int flags)
		{
			this.DeleteMailboxInternal(flags);
		}

		RawSecurityDescriptor IMailbox.GetMailboxSecurityDescriptor()
		{
			MrsTracer.Provider.Function("MapiMailbox.GetMailboxSecurityDescriptor", new object[0]);
			if (!this.IsE15OrHigher)
			{
				RawSecurityDescriptor mailboxSecurityDescriptor;
				using (ExRpcAdmin rpcAdmin = this.GetRpcAdmin())
				{
					using (this.RHTracker.Start())
					{
						mailboxSecurityDescriptor = rpcAdmin.GetMailboxSecurityDescriptor(this.MdbGuid, this.MailboxGuid);
					}
				}
				return mailboxSecurityDescriptor;
			}
			ADUser adUser = null;
			this.RunADRecipientOperation(true, delegate(IRecipientSession adSession)
			{
				adUser = (adSession.Read(this.MailboxId) as ADUser);
			});
			if (adUser == null)
			{
				throw new RecipientNotFoundPermanentException(this.MailboxGuid);
			}
			return adUser.ExchangeSecurityDescriptor;
		}

		RawSecurityDescriptor IMailbox.GetUserSecurityDescriptor()
		{
			MrsTracer.Provider.Function("MapiMailbox.GetUserSecurityDescriptor", new object[0]);
			RawSecurityDescriptor sd = null;
			this.RunADRecipientOperation(true, delegate(IRecipientSession adSession)
			{
				sd = adSession.ReadSecurityDescriptor(this.MailboxId);
			});
			return sd;
		}

		void IMailbox.SeedMBICache()
		{
			if (this.IsTitanium)
			{
				return;
			}
			this.DiscoverUmmDcForTarget();
			this.SeedMBICacheInternal(null);
		}

		ServerHealthStatus IMailbox.CheckServerHealth()
		{
			return this.CheckServerHealthInternal();
		}

		SessionStatistics IMailbox.GetSessionStatistics(SessionStatisticsFlags statisticsTypes)
		{
			return new SessionStatistics();
		}

		Guid IMailbox.StartIsInteg(List<uint> mailboxCorruptionTypes)
		{
			throw new NotImplementedException();
		}

		List<StoreIntegrityCheckJob> IMailbox.QueryIsInteg(Guid isIntegRequestGuid)
		{
			throw new NotImplementedException();
		}

		void IMailbox.Connect(MailboxConnectFlags connectFlags)
		{
			throw new NotImplementedException();
		}

		bool IMailbox.IsConnected()
		{
			throw new NotImplementedException();
		}

		bool IMailbox.IsMailboxCapabilitySupported(MailboxCapabilities capability)
		{
			throw new NotImplementedException();
		}

		MailboxInformation IMailbox.GetMailboxInformation()
		{
			throw new NotImplementedException();
		}

		public virtual void Disconnect()
		{
			if (this.rhTracker != null)
			{
				this.rhTracker.Dispose();
				this.rhTracker = null;
			}
			this.connectedWithoutMailboxSession = false;
		}

		VersionInformation IMailbox.GetVersion()
		{
			return VersionInformation.MRSProxy;
		}

		void IMailbox.SetOtherSideVersion(VersionInformation otherSideVersion)
		{
			this.OtherSideVersion = otherSideVersion;
		}

		void IMailbox.SetInTransitStatus(InTransitStatus status, out bool onlineMoveSupported)
		{
			throw new NotImplementedException();
		}

		List<FolderRec> IMailbox.EnumerateFolderHierarchy(EnumerateFolderHierarchyFlags flags, PropTag[] additionalPtagsToLoad)
		{
			throw new NotImplementedException();
		}

		List<WellKnownFolder> IMailbox.DiscoverWellKnownFolders(int flags)
		{
			return FolderHierarchyUtils.DiscoverWellKnownFolders(this, flags);
		}

		NamedPropData[] IMailbox.GetNamesFromIDs(PropTag[] pta)
		{
			throw new NotImplementedException();
		}

		PropTag[] IMailbox.GetIDsFromNames(bool createIfNotExists, NamedPropData[] npda)
		{
			throw new NotImplementedException();
		}

		void IMailbox.AddMoveHistoryEntry(MoveHistoryEntryInternal mhei, int maxMoveHistoryLength)
		{
			throw new NotImplementedException();
		}

		PropValueData[] IMailbox.GetProps(PropTag[] ptags)
		{
			throw new NotImplementedException();
		}

		byte[] IMailbox.GetReceiveFolderEntryId(string msgClass)
		{
			throw new NotImplementedException();
		}

		string IMailbox.LoadSyncState(byte[] key)
		{
			throw new NotImplementedException();
		}

		MessageRec IMailbox.SaveSyncState(byte[] key, string syncStateStr)
		{
			throw new NotImplementedException();
		}

		byte[] IMailbox.GetSessionSpecificEntryId(byte[] entryId)
		{
			throw new NotImplementedException();
		}

		void IMailbox.ConfigPst(string filePath, int? contentCodePage)
		{
			throw new NotImplementedException();
		}

		void IMailbox.ConfigEas(NetworkCredential userCredential, SmtpAddress smtpAddress, Guid mailboxGuid, string remoteHostName)
		{
			throw new NotImplementedException();
		}

		public ExRpcAdmin GetRpcAdmin()
		{
			MrsTracer.Provider.Function("MapiMailbox.GetRpcAdmin", new object[0]);
			base.CheckDisposed();
			if (this.IsPureMAPI)
			{
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			string user;
			string domain;
			string password;
			this.GetCreds(out user, out domain, out password);
			MrsTracer.Provider.Debug("Opening ExRpcAdmin connection to {0}", new object[]
			{
				this.ServerFqdn
			});
			ExRpcAdmin result;
			using (this.RHTracker.Start())
			{
				result = ExRpcAdmin.Create("Client=MSExchangeMigration", this.ServerFqdn, user, domain, password);
			}
			return result;
		}

		public void GetFolderViewsOrRestrictions(FolderRec folderRec, GetFolderRecFlags flags, byte[] folderId)
		{
			this.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			using (ExRpcAdmin rpcAdmin = this.GetRpcAdmin())
			{
				try
				{
					DateTime t = DateTime.UtcNow - new TimeSpan(7, 0, 0, 0);
					if ((flags & GetFolderRecFlags.Views) != GetFolderRecFlags.None)
					{
						PropTag[] propTagsRequested = new PropTag[]
						{
							PropTag.NextLocalId,
							PropTag.ViewAccessTime,
							PropTag.LCID,
							PropTag.MailboxQuarantined,
							PropTag.ConversationsFilter,
							PropTag.ViewCoveringPropertyTags,
							PropTag.ISCViewFilter
						};
						List<SortOrderData> list = new List<SortOrderData>();
						List<ICSViewData> list2 = new List<ICSViewData>();
						PropValue[][] viewsTable;
						using (this.RHTracker.Start())
						{
							viewsTable = rpcAdmin.GetViewsTable(MdbFlags.Private, this.MdbGuid, this.MailboxGuid, folderId, propTagsRequested);
						}
						foreach (PropValue[] array2 in viewsTable)
						{
							DateTime dateTime = array2[1].GetDateTime();
							if (!(dateTime < t))
							{
								bool flag = array2.Length >= 7 && !array2[6].IsNull() && !array2[6].IsError() && array2[6].GetBoolean();
								if (flag)
								{
									list2.Add(new ICSViewData
									{
										Conversation = (array2.Length >= 5 && !array2[4].IsNull() && !array2[4].IsError() && array2[4].GetBoolean()),
										CoveringPropertyTags = ((array2.Length < 6 || array2[5].IsNull() || array2[5].IsError()) ? Array<int>.Empty : array2[5].GetIntArray())
									});
								}
								else
								{
									SortOrder native = new SortOrder(array2[0].GetBytes());
									SortOrderData data = DataConverter<SortOrderConverter, SortOrder, SortOrderData>.GetData(native);
									data.LCID = ((array2.Length < 3 || array2[2].IsNull() || array2[2].IsError()) ? 0 : array2[2].GetInt());
									data.FAI = (array2.Length >= 4 && !array2[3].IsNull() && !array2[3].IsError() && array2[3].GetBoolean());
									data.Conversation = (array2.Length >= 5 && !array2[4].IsNull() && !array2[4].IsError() && array2[4].GetBoolean());
									list.Add(data);
								}
							}
						}
						MrsTracer.Provider.Debug("Loaded {0} views.", new object[]
						{
							list.Count
						});
						folderRec.Views = list.ToArray();
						MrsTracer.Provider.Debug("Loaded {0} ICS views.", new object[]
						{
							list2.Count
						});
						folderRec.ICSViews = list2.ToArray();
					}
					if ((flags & GetFolderRecFlags.Restrictions) != GetFolderRecFlags.None)
					{
						List<RestrictionData> list3 = new List<RestrictionData>();
						PropTag[] propTagsRequested2 = new PropTag[]
						{
							PropTag.ViewRestriction,
							PropTag.ViewAccessTime,
							PropTag.LCIDRestriction
						};
						PropValue[][] restrictionTable;
						using (this.RHTracker.Start())
						{
							restrictionTable = rpcAdmin.GetRestrictionTable(MdbFlags.Private, this.MdbGuid, this.MailboxGuid, folderId, propTagsRequested2);
						}
						foreach (PropValue[] array4 in restrictionTable)
						{
							DateTime dateTime2 = array4[1].GetDateTime();
							if (!(dateTime2 < t))
							{
								RestrictionData data2 = DataConverter<RestrictionConverter, Restriction, RestrictionData>.GetData((Restriction)array4[0].Value);
								data2.LCID = ((array4.Length < 3 || array4[2].IsNull() || array4[2].IsError()) ? 0 : array4[2].GetInt());
								list3.Add(data2);
							}
						}
						MrsTracer.Provider.Debug("Loaded {0} restrictions.", new object[]
						{
							list3.Count
						});
						folderRec.Restrictions = list3.ToArray();
					}
				}
				catch (MapiExceptionVersion)
				{
					MrsTracer.Provider.Debug("Source server does not support Views/Restrictions tables.", new object[0]);
				}
				catch (MapiExceptionInvalidType ex)
				{
					MrsTracer.Provider.Warning("Loading extended properties failed with error {0}", new object[]
					{
						CommonUtils.FullExceptionMessage(ex)
					});
				}
			}
		}

		public void RunADRecipientOperation(Action del)
		{
			try
			{
				del();
			}
			catch (LocalizedException ex)
			{
				if (CommonUtils.ExceptionIsAny(ex, new WellKnownException[]
				{
					WellKnownException.AD,
					WellKnownException.MapiADUnavailable
				}))
				{
					this.preferredDomainControllerName = null;
				}
				throw;
			}
		}

		public void RunADRecipientOperation(bool readOnly, Action<IRecipientSession> del)
		{
			this.RunADRecipientOperation(delegate()
			{
				IRecipientSession recipientSession = this.GetRecipientSession(readOnly);
				del(recipientSession);
			});
		}

		public void VerifyCapability(MRSProxyCapabilities capability, CapabilityCheck whomToCheck)
		{
			if (whomToCheck.HasFlag(CapabilityCheck.MRS) && !this.MRSVersion[(int)capability])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(this.MRSVersion.ComputerName, this.MRSVersion.ToString(), capability.ToString());
			}
			if (whomToCheck.HasFlag(CapabilityCheck.OtherProvider))
			{
				if (this.OtherSideVersion == null)
				{
					throw new UnsupportedRemoteServerVersionWithOperationPermanentException(this.MRSVersion.ComputerName, this.MRSVersion.ToString(), "SetOtherSideVersion");
				}
				if (!this.OtherSideVersion[(int)capability])
				{
					throw new UnsupportedRemoteServerVersionWithOperationPermanentException(this.OtherSideVersion.ComputerName, this.OtherSideVersion.ToString(), capability.ToString());
				}
			}
		}

		public Guid LinkMailPublicFolder(byte[] folderId, LinkMailPublicFolderFlags flags, byte[] objectId)
		{
			bool flag = !CommonUtils.IsMultiTenantEnabled() || this.PartitionHint == null;
			ADSessionSettings sessionSettings = flag ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromTenantPartitionHint(this.PartitionHint);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 2196, "LinkMailPublicFolder", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\MailboxProviderBase.cs");
			if (flag)
			{
				tenantOrRootOrgRecipientSession.EnforceDefaultScope = false;
				tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			}
			Guid result = Guid.Empty;
			ADUser aduser = tenantOrRootOrgRecipientSession.Read(this.MailboxId) as ADUser;
			if (aduser == null)
			{
				throw new RecipientNotFoundPermanentException(this.MailboxGuid);
			}
			ADPublicFolder adpublicFolder = null;
			switch (flags)
			{
			case LinkMailPublicFolderFlags.ObjectGuid:
				adpublicFolder = (tenantOrRootOrgRecipientSession.Read(new ADObjectId(objectId)) as ADPublicFolder);
				break;
			case LinkMailPublicFolderFlags.EntryId:
			{
				string propertyValue = PublicFolderSession.ConvertToLegacyDN("e71f13d1-0178-42a7-8c47-24206de84a77", HexConverter.ByteArrayToHexString(objectId));
				ADRecipient[] array = tenantOrRootOrgRecipientSession.Find(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, propertyValue), null, 2);
				if (array.Length == 1)
				{
					adpublicFolder = (array[0] as ADPublicFolder);
				}
				break;
			}
			default:
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			if (adpublicFolder != null)
			{
				adpublicFolder.ContentMailbox = aduser.Id;
				adpublicFolder.EntryId = HexConverter.ByteArrayToHexString(folderId);
				tenantOrRootOrgRecipientSession.Save(adpublicFolder);
				result = adpublicFolder.Guid;
			}
			return result;
		}

		protected static void PopulateMailboxInformation(MailboxInformation info, PropTag tag, object value)
		{
			if (tag <= PropTag.AssocContentCount)
			{
				if (tag <= PropTag.MailboxPartitionMailboxGuids)
				{
					if (tag == PropTag.MessageSizeExtended)
					{
						info.RegularItemsSize = MailboxProviderBase.GetULong(value);
						info.MailboxSize += info.RegularItemsSize;
						return;
					}
					if (tag != PropTag.MailboxPartitionMailboxGuids)
					{
						return;
					}
					info.ContainerMailboxGuids = (Guid[])value;
					return;
				}
				else
				{
					if (tag == PropTag.ContentCount)
					{
						info.RegularItemCount = MailboxProviderBase.GetULong(value);
						info.MailboxItemCount += info.RegularItemCount;
						return;
					}
					if (tag != PropTag.AssocContentCount)
					{
						return;
					}
					info.AssociatedItemCount = MailboxProviderBase.GetULong(value);
					info.MailboxItemCount += info.AssociatedItemCount;
					return;
				}
			}
			else if (tag <= PropTag.DeletedAssocMsgCount)
			{
				if (tag == PropTag.DeletedMsgCount)
				{
					info.RegularDeletedItemCount = MailboxProviderBase.GetULong(value);
					info.MailboxItemCount += info.RegularDeletedItemCount;
					return;
				}
				if (tag != PropTag.DeletedAssocMsgCount)
				{
					return;
				}
				info.AssociatedDeletedItemCount = MailboxProviderBase.GetULong(value);
				info.MailboxItemCount += info.AssociatedDeletedItemCount;
				return;
			}
			else
			{
				if (tag == PropTag.DeletedMessageSizeExtended)
				{
					info.RegularDeletedItemsSize = MailboxProviderBase.GetULong(value);
					info.MailboxSize += info.RegularDeletedItemsSize;
					return;
				}
				if (tag == PropTag.DeleteAssocMessageSizeExtended)
				{
					info.AssociatedDeletedItemsSize = MailboxProviderBase.GetULong(value);
					info.MailboxSize += info.AssociatedDeletedItemsSize;
					return;
				}
				if (tag != PropTag.AssocMessageSizeExtended)
				{
					return;
				}
				info.AssociatedItemsSize = MailboxProviderBase.GetULong(value);
				info.MailboxSize += info.AssociatedItemsSize;
				return;
			}
		}

		protected virtual void ValidateRecipient(MiniRecipient recipient)
		{
			if (recipient.RecipientType != RecipientType.UserMailbox && recipient.RecipientType != RecipientType.MailUser)
			{
				throw new UnsupportedRecipientTypePermanentException(recipient.ToString(), recipient.RecipientType.ToString());
			}
			if (string.IsNullOrEmpty(recipient.LegacyExchangeDN))
			{
				throw new RecipientMissingLegDNPermanentException(recipient.ToString());
			}
			if (this.PrimaryMailboxGuid != recipient.ExchangeGuid)
			{
				MrsTracer.Provider.Error("We managed to find the wrong user.", new object[0]);
				throw new UnexpectedErrorPermanentException(-2147221233);
			}
			if (this.IsAggregatedMailbox)
			{
				MultiValuedProperty<Guid> multiValuedProperty = recipient.AggregatedMailboxGuids ?? new MultiValuedProperty<Guid>();
				if (!multiValuedProperty.Contains(this.MailboxGuid))
				{
					MrsTracer.Provider.Error("Unable to locate aggregated mailbox with guid {0}", new object[]
					{
						this.MailboxGuid
					});
					throw new RecipientAggregatedMailboxNotFoundPermanentException(recipient.ToString(), string.Join<Guid>(",", multiValuedProperty.ToArray()), this.MailboxGuid);
				}
			}
			if (this.IsArchiveMailbox && this.MailboxGuid != recipient.ArchiveGuid)
			{
				MrsTracer.Provider.Error("Unable to locate archive mailbox with guid {0}", new object[]
				{
					this.MailboxGuid
				});
				throw new RecipientArchiveGuidMismatchPermanentException(recipient.ToString(), recipient.ArchiveGuid, this.MailboxGuid);
			}
			if (this.IsMove && (this.MbxType == MailboxType.SourceMailbox || this.MbxType == MailboxType.DestMailboxCrossOrg))
			{
				MultiValuedProperty<string> multiValuedProperty2 = recipient[ADRecipientSchema.AllowedAttributesEffective] as MultiValuedProperty<string>;
				foreach (ADPropertyDefinition adpropertyDefinition in MailboxProviderBase.WriteableProperties)
				{
					bool flag = false;
					if (multiValuedProperty2 != null)
					{
						foreach (string x in multiValuedProperty2)
						{
							if (StringComparer.OrdinalIgnoreCase.Equals(x, adpropertyDefinition.LdapDisplayName))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						throw new RecipientPropertyIsNotWriteablePermanentException(recipient.ToString(), adpropertyDefinition.LdapDisplayName);
					}
				}
			}
		}

		protected void CreateStoreSession(MailboxConnectFlags connectFlags, Action createSessionDelegate)
		{
			base.CheckDisposed();
			if (((IMailbox)this).IsConnected())
			{
				return;
			}
			this.LocateAndValidateADUser();
			if ((connectFlags & MailboxConnectFlags.DoNotOpenMapiSession) != MailboxConnectFlags.None)
			{
				this.connectedWithoutMailboxSession = true;
				return;
			}
			createSessionDelegate();
			if (((IMailbox)this).IsConnected())
			{
				this.AfterConnect();
			}
		}

		protected void VerifyMailboxConnection(VerifyMailboxConnectionFlags flags = VerifyMailboxConnectionFlags.None)
		{
			base.CheckDisposed();
			if (this.reservation != null && this.reservation.IsDisposed)
			{
				throw new ExpiredReservationException();
			}
			if (!flags.HasFlag(VerifyMailboxConnectionFlags.MailboxSessionNotRequired) && (this.connectedWithoutMailboxSession || !((IMailbox)this).IsConnected()))
			{
				throw new NotConnectedPermanentException();
			}
		}

		protected virtual void CopyMessagesOneByOne(List<MessageRec> messages, IFxProxyPool proxyPool, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps, Action<MessageRec> changeSourceFolderAction = null)
		{
			MrsTracer.Provider.Function("MailboxProviderBase.CopyMessagesOneByOne({0} messages)", new object[]
			{
				messages.Count
			});
			bool exportCompleted = false;
			CommonUtils.ProcessKnownExceptions(delegate
			{
				this.CopyMessagesIndividually(messages, proxyPool, propsToCopyExplicitly, excludeProps, changeSourceFolderAction);
				exportCompleted = true;
				proxyPool.Flush();
			}, delegate(Exception ex)
			{
				if (!exportCompleted)
				{
					MrsTracer.Provider.Debug("Flushing target proxy after receiving an exception.", new object[0]);
					CommonUtils.CatchKnownExceptions(new Action(proxyPool.Flush), null);
				}
				return false;
			});
		}

		protected virtual void CopySingleMessage(MessageRec curMsg, IFolderProxy folderProxy, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps)
		{
			throw new NotImplementedException();
		}

		protected void DiscoverUmmDcForTarget()
		{
			if (!string.IsNullOrEmpty(this.preferredDomainControllerName))
			{
				return;
			}
			ADUser aduser = ((IMailbox)this).GetADUser();
			Guid mdbGuid = this.MdbGuid;
			ADObjectId adobjectId = this.IsArchiveMailbox ? aduser.ArchiveDatabase : aduser.Database;
			if (adobjectId != null && mdbGuid.Equals(adobjectId.ObjectGuid))
			{
				this.preferredDomainControllerName = aduser.OriginatingServer;
				MrsTracer.Provider.Debug("Updated preferred DC to {0}", new object[]
				{
					this.preferredDomainControllerName
				});
				return;
			}
			throw new CouldNotFindDCHavingUmmUpdateTransientException(mdbGuid, this.MailboxId.ToString());
		}

		protected void VerifyMdbIsOnline(Exception originalException)
		{
			MrsTracer.Provider.Function("MapiMailbox.VerifyMdbIsOnline", new object[0]);
			if (this.IsPureMAPI)
			{
				return;
			}
			MdbStatus[] array;
			using (ExRpcAdmin rpcAdmin = this.GetRpcAdmin())
			{
				using (this.RHTracker.Start())
				{
					array = rpcAdmin.ListMdbStatus(new Guid[]
					{
						this.MdbGuid
					});
				}
			}
			if (array.Length != 1 || (array[0].Status & MdbStatusFlags.Online) == MdbStatusFlags.Offline)
			{
				MrsTracer.Provider.Warning("MDB {0} is offline", new object[]
				{
					this.MdbGuid
				});
				throw new MdbIsOfflineTransientException(this.MdbGuid, originalException);
			}
		}

		protected void LocateAndValidateADUser()
		{
			if (this.IsPureMAPI)
			{
				return;
			}
			if (this.IsPublicFolderMigrationSource)
			{
				PublicFolderDatabase publicFolderDatabase = this.FindDatabaseByGuid<PublicFolderDatabase>(this.ConfiguredMdbGuid);
				this.mbxHomeMdb = publicFolderDatabase.Id;
				this.MailboxId = publicFolderDatabase.Id;
				this.TraceMailboxId = publicFolderDatabase.Identity.ToString();
			}
			else if (this.IsRestore)
			{
				this.TraceMailboxId = string.Format("{0} {1}", this.MailboxGuid, this.MdbGuid);
			}
			else
			{
				MiniRecipient recipient = null;
				this.RunADRecipientOperation(delegate()
				{
					recipient = CommonUtils.FindUserByMailboxGuid(this.PrimaryMailboxGuid, this.PartitionHint, this.Credential, this.EffectiveDomainControllerName, MailboxProviderBase.UserPropertiesToLoad);
				});
				if (recipient == null)
				{
					throw new RecipientNotFoundPermanentException(this.MailboxGuid);
				}
				this.ValidateRecipient(recipient);
				this.MailboxId = recipient.Id;
				this.MailboxDN = recipient.LegacyExchangeDN;
				this.OrganizationId = recipient.OrganizationId;
				this.recipientType = (int)recipient.RecipientType;
				if (!this.IsOlcSync)
				{
					RecipientDisplayType? recipientDisplayType = (RecipientDisplayType?)recipient[ADRecipientSchema.RecipientDisplayType];
					this.recipientDisplayType = (int)((recipientDisplayType != null) ? recipientDisplayType.Value : RecipientDisplayType.MailboxUser);
				}
				this.recipientTypeDetails = (long)recipient.RecipientTypeDetails;
				if (this.IsPrimaryMailbox)
				{
					this.TraceMailboxId = this.MailboxDN;
				}
				else
				{
					this.TraceMailboxId = string.Format("{0}:{1}", this.MailboxDN, this.MailboxGuid);
				}
				if (this.IsOlcSync)
				{
					this.useMdbQuotaDefaults = new bool?(false);
					this.mbxQuota = null;
					this.mbxDumpsterQuota = null;
					this.mbxArchiveQuota = null;
				}
				else
				{
					this.useMdbQuotaDefaults = (bool?)recipient[ADMailboxRecipientSchema.UseDatabaseQuotaDefaults];
					this.mbxQuota = MailboxProviderBase.GetQuotaValue(recipient, ADMailboxRecipientSchema.ProhibitSendReceiveQuota);
					this.mbxDumpsterQuota = MailboxProviderBase.GetQuotaValue(recipient, ADUserSchema.RecoverableItemsQuota);
					this.mbxArchiveQuota = MailboxProviderBase.GetQuotaValue(recipient, ADUserSchema.ArchiveQuota);
				}
				this.mbxHomeMdb = (ADObjectId)recipient[ADMailboxRecipientSchema.Database];
				this.archiveMdb = (((ADObjectId)recipient[ADUserSchema.ArchiveDatabase]) ?? this.mbxHomeMdb);
				this.archiveGuid = recipient.ArchiveGuid;
				MultiValuedProperty<Guid> multiValuedProperty = (MultiValuedProperty<Guid>)recipient[ADUserSchema.AggregatedMailboxGuids];
				if (multiValuedProperty != null && multiValuedProperty.Count > 0)
				{
					this.alternateMailboxes = new Guid[multiValuedProperty.Count];
					int num = 0;
					foreach (Guid guid in multiValuedProperty)
					{
						this.alternateMailboxes[num++] = guid;
					}
				}
			}
			if (this.UseHomeMDB)
			{
				this.MdbId = (this.IsArchiveMailbox ? this.archiveMdb : this.mbxHomeMdb);
				if (this.MdbId == null)
				{
					throw new RecipientIsNotAMailboxPermanentException(this.MailboxId.ToString());
				}
			}
			this.ResolveMDB(false);
		}

		protected void ResolveMDB(bool forceRediscovery)
		{
			if (this.MdbId == null)
			{
				if (this.ConfiguredMdbGuid != Guid.Empty)
				{
					Database database = this.FindDatabaseByGuid<Database>(this.ConfiguredMdbGuid);
					this.MdbId = database.Id;
				}
				else if (!string.IsNullOrEmpty(this.ConfiguredMdbName))
				{
					Database database = CommonUtils.FindMdbByName(this.ConfiguredMdbName, this.Credential, this.ConfigDomainControllerName);
					this.MdbId = database.Id;
				}
				else
				{
					this.MdbId = (this.IsArchiveMailbox ? this.archiveMdb : this.mbxHomeMdb);
					if (this.MdbId == null)
					{
						throw new RecipientIsNotAMailboxPermanentException(this.MailboxId.ToString());
					}
				}
			}
			FindServerFlags findServerFlags = FindServerFlags.None;
			if (forceRediscovery)
			{
				findServerFlags |= FindServerFlags.ForceRediscovery;
			}
			DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(this.MdbGuid, this.ConfigDomainControllerName, this.Credential, findServerFlags);
			this.ServerDN = databaseInformation.ServerDN;
			this.ServerFqdn = databaseInformation.ServerFqdn;
			this.ServerGuid = databaseInformation.ServerGuid;
			this.ServerVersion = databaseInformation.ServerVersion;
			this.ServerMailboxRelease = databaseInformation.MailboxRelease;
			this.ServerDisplayName = this.ServerFqdn;
			if ((this.Flags & LocalMailboxFlags.LocalMachineMapiOnly) != LocalMailboxFlags.None && !databaseInformation.IsOnThisServer)
			{
				throw new MailboxDatabaseNotOnServerTransientException(databaseInformation.DatabaseName, this.MdbGuid, this.ServerFqdn, CommonUtils.LocalComputerName);
			}
			MRSRequestType requestType;
			if (this.Flags.HasFlag(LocalMailboxFlags.Move))
			{
				requestType = MRSRequestType.Move;
			}
			else if (this.Flags.HasFlag(LocalMailboxFlags.AggregatedMailbox) || this.Flags.HasFlag(LocalMailboxFlags.EasSync))
			{
				requestType = MRSRequestType.Sync;
			}
			else if (this.Flags.HasFlag(LocalMailboxFlags.LegacyPublicFolders))
			{
				if (this.Flags.HasFlag(LocalMailboxFlags.ParallelPublicFolderMigration))
				{
					requestType = MRSRequestType.PublicFolderMailboxMigration;
				}
				else
				{
					requestType = MRSRequestType.PublicFolderMigration;
				}
			}
			else if (this.Flags.HasFlag(LocalMailboxFlags.PstImport))
			{
				requestType = MRSRequestType.MailboxImport;
			}
			else if (this.Flags.HasFlag(LocalMailboxFlags.PstExport))
			{
				requestType = MRSRequestType.MailboxExport;
			}
			else if (this.Flags.HasFlag(LocalMailboxFlags.PublicFolderMove))
			{
				requestType = MRSRequestType.PublicFolderMove;
			}
			else if (this.Flags.HasFlag(LocalMailboxFlags.Restore))
			{
				requestType = MRSRequestType.MailboxRestore;
			}
			else if (this.Flags.HasFlag(LocalMailboxFlags.FolderMove))
			{
				requestType = MRSRequestType.FolderMove;
			}
			else
			{
				requestType = MRSRequestType.Merge;
			}
			this.configContext = CommonUtils.CreateConfigContext(this.MailboxGuid, (this.MdbId == null) ? Guid.Empty : this.MdbId.ObjectGuid, this.OrganizationId, RequestWorkloadType.None, requestType, this.GetSyncProtocol());
		}

		protected TDatabase FindDatabaseByGuid<TDatabase>(Guid dbGuid) where TDatabase : Database, new()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.ConfigDomainControllerName, true, ConsistencyMode.PartiallyConsistent, this.Credential, ADSessionSettings.FromRootOrgScopeSet(), 2852, "FindDatabaseByGuid", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\MailboxProviderBase.cs");
			TDatabase tdatabase = topologyConfigurationSession.FindDatabaseByGuid<TDatabase>(dbGuid);
			if (tdatabase == null)
			{
				MrsTracer.Provider.Error("Unable to locate DB by guid {0}", new object[]
				{
					dbGuid
				});
				throw new DatabaseNotFoundByGuidPermanentException(dbGuid);
			}
			return tdatabase;
		}

		protected void GetCreds(out string userName, out string userDomain, out string userPassword)
		{
			userName = ((this.Credential != null) ? this.Credential.UserName : null);
			userDomain = ((this.Credential != null) ? this.Credential.Domain : null);
			userPassword = ((this.Credential != null) ? this.Credential.Password : null);
		}

		protected virtual void DeleteMailboxInternal(int flags)
		{
			MrsTracer.Provider.Function("MapiMailbox.DeleteMailbox({0})", new object[]
			{
				flags
			});
			base.CheckDisposed();
			if (this.IsTitanium)
			{
				flags &= 1;
			}
			using (ExRpcAdmin rpcAdmin = this.GetRpcAdmin())
			{
				MrsTracer.Provider.Debug("Deleting mailbox \"{0}\" in MDB '{1}', flags={2}", new object[]
				{
					this.TraceMailboxId,
					this.TraceMdbId,
					flags
				});
				using (this.RHTracker.Start())
				{
					if (MapiUtils.IsMailboxInDatabase(rpcAdmin, this.MdbGuid, this.MailboxGuid))
					{
						rpcAdmin.DeletePrivateMailbox(this.MdbGuid, this.MailboxGuid, flags);
					}
					else
					{
						MrsTracer.Provider.Debug("Mailbox is already not in MDB, not attempting to delete it.", new object[0]);
					}
					if ((flags & 1) != 0 && (flags & 4) == 0)
					{
						CommonUtils.CatchKnownExceptions(delegate
						{
							this.SeedMBICacheInternal(rpcAdmin);
						}, null);
					}
				}
			}
			((IMailbox)this).Disconnect();
		}

		protected CreateMailboxResult CreateMailbox(byte[] mailboxData, MailboxSignatureFlags sourceSignatureFlags)
		{
			MrsTracer.Provider.Debug("Creating destination mailbox \"{0}\" in MDB {1}{2}", new object[]
			{
				this.TraceMailboxId,
				this.MdbGuid,
				(this.MailboxContainerGuid != null) ? (" in Container " + this.MailboxContainerGuid.Value) : string.Empty
			});
			using (ExRpcAdmin rpcAdmin = this.GetRpcAdmin())
			{
				using (this.RHTracker.Start())
				{
					rpcAdmin.PurgeCachedMailboxObject(this.MailboxGuid);
				}
				uint mailboxSignatureServerVersion = rpcAdmin.GetMailboxSignatureServerVersion();
				if (mailboxSignatureServerVersion >= 102U)
				{
					mailboxData = MailboxSignatureConverter.ConvertTenantHint(mailboxData, sourceSignatureFlags, this.PartitionHint);
				}
				if (this.MailboxContainerGuid != null)
				{
					PartitionInformation.ControlFlags flags = this.Flags.HasFlag(LocalMailboxFlags.CreateNewPartition) ? PartitionInformation.ControlFlags.CreateNewPartition : PartitionInformation.ControlFlags.None;
					PartitionInformation partitionInformation = new PartitionInformation(this.MailboxContainerGuid.Value, flags);
					mailboxData = MailboxSignatureConverter.ConvertPartitionInformation(mailboxData, sourceSignatureFlags, partitionInformation);
				}
				try
				{
					using (this.RHTracker.Start())
					{
						rpcAdmin.SetMailboxBasicInfo(this.MdbGuid, this.MailboxGuid, mailboxData);
					}
				}
				catch (MapiExceptionDuplicateObject ex)
				{
					MrsTracer.Provider.Debug("SetMailboxBasicInfo failed with ecDuplicateObject\n{0}", new object[]
					{
						CommonUtils.FullExceptionMessage(ex)
					});
					return CreateMailboxResult.CleanupNotComplete;
				}
				catch (MapiExceptionFolderNotCleanedUp ex2)
				{
					MrsTracer.Provider.Debug("SetMailboxBasicInfo failed with ecFolderNotCleanedUp\n{0}", new object[]
					{
						CommonUtils.FullExceptionMessage(ex2)
					});
					return CreateMailboxResult.CleanupNotComplete;
				}
				catch (MapiExceptionNotFound ex3)
				{
					MrsTracer.Provider.Debug("SetMailboxBasicInfo failed with ecNotFound\n{0}", new object[]
					{
						CommonUtils.FullExceptionMessage(ex3)
					});
					return CreateMailboxResult.ObjectNotFound;
				}
			}
			return CreateMailboxResult.Success;
		}

		protected void ProcessMailboxSignature(byte[] mailboxData)
		{
			MrsTracer.Provider.Debug("Process destination mailbox signature \"{0}\" in MDB {1}", new object[]
			{
				this.TraceMailboxId,
				this.MdbGuid
			});
			using (ExRpcAdmin rpcAdmin = this.GetRpcAdmin())
			{
				uint mailboxSignatureServerVersion = rpcAdmin.GetMailboxSignatureServerVersion();
				if (mailboxSignatureServerVersion < 103U)
				{
					throw new InputDataIsInvalidPermanentException();
				}
				using (this.RHTracker.Start())
				{
					rpcAdmin.SetMailboxBasicInfo(this.MdbGuid, this.MailboxGuid, mailboxData);
				}
			}
		}

		protected void CleanupAdUserAfterDeleteMailbox()
		{
			MrsTracer.Provider.Function("StorageMailbox.CleanupAdUserAfterDeleteMailbox()", new object[0]);
			this.RunADRecipientOperation(false, delegate(IRecipientSession adSession)
			{
				ADUser aduser = adSession.Read(this.MailboxId) as ADUser;
				aduser.Database = null;
				aduser.ServerLegacyDN = null;
				aduser.SetExchangeVersion(null);
				aduser[ADRecipientSchema.RecipientTypeDetails] = null;
				aduser[ADRecipientSchema.RecipientDisplayType] = null;
				adSession.Save(aduser);
			});
		}

		protected RawSecurityDescriptor GetUserSecurityDescriptor()
		{
			MrsTracer.Provider.Function("IMailbox.GetUserSecurityDescriptor", new object[0]);
			RawSecurityDescriptor sd = null;
			this.RunADRecipientOperation(true, delegate(IRecipientSession adSession)
			{
				sd = adSession.ReadSecurityDescriptor(this.MailboxId);
			});
			return sd;
		}

		protected void SetUserSecurityDescriptor(RawSecurityDescriptor sd)
		{
			MrsTracer.Provider.Function("IMailbox.SetUserSecurityDescriptor", new object[0]);
			this.RunADRecipientOperation(false, delegate(IRecipientSession adSession)
			{
				ADUser aduser = adSession.Read(this.MailboxId) as ADUser;
				if (aduser == null)
				{
					throw new RecipientNotFoundPermanentException(this.MailboxGuid);
				}
				aduser.SaveSecurityDescriptor(sd);
			});
		}

		protected void SetMailboxSecurityDescriptor(RawSecurityDescriptor sd)
		{
			if (this.IsE15OrHigher)
			{
				this.RunADRecipientOperation(false, delegate(IRecipientSession adSession)
				{
					ADUser aduser = adSession.Read(this.MailboxId) as ADUser;
					if (aduser == null)
					{
						throw new RecipientNotFoundPermanentException(this.MailboxGuid);
					}
					aduser.ExchangeSecurityDescriptor = sd;
					adSession.Save(aduser);
				});
				using (ExRpcAdmin rpcAdmin = this.GetRpcAdmin())
				{
					using (this.RHTracker.Start())
					{
						rpcAdmin.PurgeCachedMailboxObject(this.MailboxGuid);
					}
					return;
				}
			}
			using (ExRpcAdmin rpcAdmin2 = this.GetRpcAdmin())
			{
				using (this.RHTracker.Start())
				{
					rpcAdmin2.SetMailboxSecurityDescriptor(this.MdbGuid, this.MailboxGuid, sd);
				}
			}
		}

		private static ulong GetULong(object value)
		{
			if (value is int)
			{
				return (ulong)((long)((int)value));
			}
			return (ulong)((long)value);
		}

		private static ulong? GetQuotaValue(MiniRecipient recipient, ADPropertyDefinition quotaProperty)
		{
			Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)recipient[quotaProperty];
			if (!unlimited.IsUnlimited)
			{
				return new ulong?(unlimited.Value.ToBytes());
			}
			return null;
		}

		private void SeedMBICacheInternal(ExRpcAdmin rpcAdmin)
		{
			if (this.IsTitanium)
			{
				return;
			}
			ExRpcAdmin exRpcAdmin = null;
			if (rpcAdmin == null)
			{
				exRpcAdmin = this.GetRpcAdmin();
				rpcAdmin = exRpcAdmin;
			}
			try
			{
				MrsTracer.Provider.Debug("Clearing MBI cache entry for {0}", new object[]
				{
					this.MailboxGuid
				});
				using (this.RHTracker.Start())
				{
					rpcAdmin.PurgeCachedMailboxObject(this.MailboxGuid);
				}
				if (!string.IsNullOrEmpty(this.EffectiveDomainControllerName))
				{
					try
					{
						using (this.RHTracker.Start())
						{
							this.RunADRecipientOperation(delegate()
							{
								MrsTracer.Provider.Debug("Seeding DSAccess cache for '{0}' from {1}", new object[]
								{
									this.MailboxDN,
									this.EffectiveDomainControllerName
								});
								rpcAdmin.PrePopulateCache(this.MdbGuid, this.MailboxDN, this.MailboxGuid, (this.PartitionHint != null) ? TenantPartitionHint.Serialize(this.PartitionHint) : null, this.EffectiveDomainControllerName);
							});
						}
					}
					catch (MapiExceptionVersion)
					{
						MrsTracer.Provider.Debug("The PrePopulateCache API is supported only by E14 and higher version of Store.", new object[0]);
					}
				}
			}
			finally
			{
				if (exRpcAdmin != null)
				{
					exRpcAdmin.Dispose();
				}
			}
		}

		private LocalizedException ClassifyWrapAndReturnPowershellException(Exception ex)
		{
			if (ex is MonadDataAdapterInvocationException && ((MonadDataAdapterInvocationException)ex).ErrorRecord != null && ((MonadDataAdapterInvocationException)ex).ErrorRecord.Exception != null)
			{
				ex = ((MonadDataAdapterInvocationException)ex).ErrorRecord.Exception;
			}
			else if (ex is CmdletInvocationException && ((CmdletInvocationException)ex).ErrorRecord != null && ((CmdletInvocationException)ex).ErrorRecord.Exception != null)
			{
				ex = ((CmdletInvocationException)ex).ErrorRecord.Exception;
			}
			if (CommonUtils.IsTransientException(ex))
			{
				return new UpdateMovedMailboxTransientException(ex);
			}
			return new UpdateMovedMailboxPermanentException(ex);
		}

		protected IRecipientSession GetRecipientSession(bool readOnly)
		{
			return this.GetRecipientSession(readOnly, false);
		}

		protected void SetMailboxSyncState(string syncStateStr)
		{
			MrsTracer.Provider.Function("MailboxProviderBase.SetMailboxSyncState", new object[0]);
			this.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			this.ValidateIfSourceMailbox("MailboxProviderBase.SetMailboxSyncState");
			MapiSyncState mapiSyncState = MapiSyncState.Deserialize(syncStateStr);
			if (mapiSyncState == null)
			{
				MrsTracer.Provider.Debug("Using empty sync state", new object[0]);
				mapiSyncState = new MapiSyncState();
			}
			this.SyncState = mapiSyncState;
		}

		protected string GetMailboxSyncState()
		{
			MrsTracer.Provider.Function("MailboxProviderBase.SerializeMailboxSyncState", new object[0]);
			this.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			this.ValidateIfSourceMailbox("MailboxProviderBase.SerializeMailboxSyncState");
			if (this.syncState == null)
			{
				return null;
			}
			return this.syncState.Serialize(false);
		}

		protected void VerifyRestoreSource(MailboxConnectFlags mailboxConnectFlags)
		{
			if (!this.IsRestore || mailboxConnectFlags.HasFlag(MailboxConnectFlags.AllowRestoreFromConnectedMailbox))
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			MailboxDatabase mailboxDatabase = CommonUtils.FindMdbByGuid(this.MdbGuid, null, null);
			flag3 = mailboxDatabase.Recovery;
			if (!flag3)
			{
				using (ExRpcAdmin rpcAdmin = this.GetRpcAdmin())
				{
					flag = MapiUtils.IsStoreDisconnectedMailbox(rpcAdmin, this.MdbGuid, this.MailboxGuid);
				}
			}
			if (!flag && !flag3 && this.PartitionHint != null)
			{
				IRecipientSession recipientSession = CommonUtils.CreateRecipientSession(this.PartitionHint.GetExternalDirectoryOrganizationId(), null, null);
				ADRecipient adrecipient = recipientSession.FindByExchangeGuidIncludingArchive(this.MailboxGuid);
				flag2 = (adrecipient != null && adrecipient.IsSoftDeleted);
			}
			if (!flag && !flag2 && !flag3)
			{
				throw new RestoringConnectedMailboxPermanentException(this.MailboxGuid);
			}
		}

		private IRecipientSession GetRecipientSession(bool readOnly, bool rootOrgScoped)
		{
			ADSessionSettings adsessionSettings;
			if (rootOrgScoped || this.PartitionHint == null)
			{
				adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			}
			else
			{
				adsessionSettings = ADSessionSettings.FromTenantPartitionHint(this.PartitionHint);
			}
			adsessionSettings.IncludeSoftDeletedObjects = true;
			adsessionSettings.IncludeInactiveMailbox = true;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.EffectiveDomainControllerName, readOnly, ConsistencyMode.PartiallyConsistent, this.Credential, adsessionSettings, 3435, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\MailboxProviderBase.cs");
			if (rootOrgScoped)
			{
				tenantOrRootOrgRecipientSession.EnforceDefaultScope = false;
			}
			return tenantOrRootOrgRecipientSession;
		}

		private ServerHealthStatus CheckServerHealthInternal()
		{
			ServerHealthStatus serverHealthStatus = new ServerHealthStatus(ServerHealthState.Healthy);
			if (this.MbxType == MailboxType.SourceMailbox)
			{
				return serverHealthStatus;
			}
			ILegacyResourceHealthProvider legacyResourceHealthProvider = ResourceHealthMonitorManager.Singleton.Get(new LegacyResourceHealthMonitorKey(this.MdbGuid)) as ILegacyResourceHealthProvider;
			TimeSpan timeSpan;
			LocalizedString localizedString;
			ConstraintCheckAgent constraintCheckAgent;
			ConstraintCheckResultType constraintCheckResultType = CommonUtils.DumpsterStatus.CheckReplicationHealthConstraint(this.MdbGuid, out timeSpan, out localizedString, out constraintCheckAgent);
			if (constraintCheckResultType != ConstraintCheckResultType.Satisfied)
			{
				MrsTracer.Provider.Warning("Move for mailbox '{0}' is stalled because DataMoveReplicationConstraint is not satisfied for the target database '{1}'. Failure Reason: {2}, agent: {3}", new object[]
				{
					this.TraceMailboxId,
					this.TraceMdbId,
					localizedString,
					constraintCheckAgent
				});
				serverHealthStatus.HealthState = ServerHealthState.NotHealthy;
				serverHealthStatus.Agent = constraintCheckAgent;
				serverHealthStatus.FailureReason = MrsStrings.MoveIsStalled(this.TraceMailboxId, this.TraceMdbId, localizedString, constraintCheckAgent.ToString());
			}
			if (legacyResourceHealthProvider != null)
			{
				legacyResourceHealthProvider.Update(constraintCheckResultType, constraintCheckAgent, localizedString);
			}
			return serverHealthStatus;
		}

		private void CopyMessagesIndividually(IEnumerable<MessageRec> messages, IFxProxyPool proxyPool, PropTag[] propsToCopyExplicitly, PropTag[] excludeProps, Action<MessageRec> changeSourceFolderAction)
		{
			IFolderProxy folderProxy = null;
			byte[] eid = null;
			try
			{
				foreach (MessageRec messageRec in messages)
				{
					if (!CommonUtils.IsSameEntryId(messageRec.FolderId, eid))
					{
						if (folderProxy != null)
						{
							folderProxy.Dispose();
							folderProxy = null;
						}
						folderProxy = proxyPool.GetFolderProxy(messageRec.FolderId);
						eid = messageRec.FolderId;
						if (changeSourceFolderAction != null)
						{
							changeSourceFolderAction(messageRec);
						}
					}
					this.CopySingleMessage(messageRec, folderProxy, propsToCopyExplicitly, excludeProps);
				}
			}
			finally
			{
				if (folderProxy != null)
				{
					folderProxy.Dispose();
				}
			}
		}

		private void ValidateIfSourceMailbox(string methodName)
		{
			this.MbxType.Equals(MailboxType.SourceMailbox);
		}

		private MailboxChangesManifest RunICSManifestSync(bool catchup, SyncHierarchyManifestState hierState, MapiStore mapiStore)
		{
			MrsTracer.Provider.Function("MailboxProviderBase.RunICSManifestSync", new object[0]);
			MailboxChangesManifest mailboxChangesManifest = new MailboxChangesManifest();
			MailboxProviderBase.ManifestHierarchyCallback iMapiManifestCallback = new MailboxProviderBase.ManifestHierarchyCallback(catchup, mailboxChangesManifest, this.publicFoldersToSkip, this.IsPublicFolderMigrationSource);
			using (this.RHTracker.Start())
			{
				using (MapiFolder rootFolder = mapiStore.GetRootFolder())
				{
					SyncConfigFlags syncConfigFlags = SyncConfigFlags.ManifestHierReturnDeletedEntryIds;
					if (((this.ServerVersion >= Server.E14MinVersion && this.ServerVersion < Server.E15MinVersion) || (long)this.ServerVersion >= MailboxProviderBase.E15MinVersionSupportsOnlySpecifiedPropsForHierarchy) && !this.IsPureMAPI)
					{
						syncConfigFlags |= SyncConfigFlags.OnlySpecifiedProps;
					}
					if (catchup && this.isStorageProvider)
					{
						syncConfigFlags |= SyncConfigFlags.Catchup;
					}
					PropTag[] tagsInclude = MailboxProviderBase.PropTagsForRegularMoves;
					if (this.IsPublicFolderMigrationSource)
					{
						if (syncConfigFlags.HasFlag(SyncConfigFlags.OnlySpecifiedProps))
						{
							tagsInclude = MailboxProviderBase.PropTagsForPublicFolderMigration;
						}
						else
						{
							syncConfigFlags |= SyncConfigFlags.NoForeignKeys;
						}
					}
					using (MapiHierarchyManifestEx mapiHierarchyManifestEx = rootFolder.CreateExportHierarchyManifestEx(syncConfigFlags, hierState.IdsetGiven, hierState.CnsetSeen, iMapiManifestCallback, tagsInclude, null))
					{
						while (mapiHierarchyManifestEx.Synchronize() != ManifestStatus.Done)
						{
						}
						byte[] idsetGiven;
						byte[] cnsetSeen;
						mapiHierarchyManifestEx.GetState(out idsetGiven, out cnsetSeen);
						hierState.IdsetGiven = idsetGiven;
						hierState.CnsetSeen = cnsetSeen;
					}
				}
			}
			return mailboxChangesManifest;
		}

		protected virtual MailboxChangesManifest EnumerateHierarchyChanges(bool catchup, Func<SyncHierarchyManifestState, MailboxChangesManifest> hierarchySyncAction)
		{
			MrsTracer.Provider.Function("MailboxProviderBase.EnumerateHierarchyChanges", new object[0]);
			this.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			this.ValidateIfSourceMailbox("MailboxProviderBase.EnumerateHierarchyChanges");
			MailboxChangesManifest mailboxChangesManifest = new MailboxChangesManifest();
			if (hierarchySyncAction != null)
			{
				mailboxChangesManifest = hierarchySyncAction(this.SyncState.HierarchyData);
			}
			if (mailboxChangesManifest.DeletedFolders != null && mailboxChangesManifest.DeletedFolders.Count > 0)
			{
				foreach (byte[] folderId in mailboxChangesManifest.DeletedFolders)
				{
					this.SyncState.RemoveContentsManifestState(folderId);
				}
			}
			if (catchup)
			{
				return null;
			}
			return mailboxChangesManifest;
		}

		protected virtual void AfterConnect()
		{
			if (this.SupportsSavingSyncState)
			{
				this.syncState = new MapiSyncState();
			}
		}

		protected virtual MailboxChangesManifest DoManifestSync(EnumerateHierarchyChangesFlags flags, int maxChanges, SyncHierarchyManifestState hierState, MapiStore mapiStore)
		{
			MrsTracer.Provider.Function("MailboxProviderBase.DoManifestSync", new object[0]);
			this.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			this.ValidateIfSourceMailbox("MailboxProviderBase.DoManifestSync");
			MailboxChangesManifest mailboxChangesManifest = new MailboxChangesManifest();
			bool flag = flags.HasFlag(EnumerateHierarchyChangesFlags.Catchup);
			bool flag2 = flag || hierState.ManualSyncData == null;
			if (flag2)
			{
				try
				{
					bool flag3 = maxChanges != 0;
					if (flag3)
					{
						if (flags.HasFlag(EnumerateHierarchyChangesFlags.FirstPage))
						{
							if (this.hierarchyChangesFetcher != null)
							{
								this.hierarchyChangesFetcher.Dispose();
							}
							this.hierarchyChangesFetcher = new ManifestHierarchyChangesFetcher(mapiStore, this, true);
						}
						mailboxChangesManifest = this.hierarchyChangesFetcher.EnumerateHierarchyChanges(hierState, flags, maxChanges);
					}
					else
					{
						mailboxChangesManifest = this.RunICSManifestSync(flag, hierState, mapiStore);
					}
				}
				catch (MapiExceptionNotFound ex)
				{
					if (!flag)
					{
						throw;
					}
					MrsTracer.Provider.Warning("Got ecNotFound during ICS hierarchy catchup, will try manual. {0}", new object[]
					{
						CommonUtils.FullExceptionMessage(ex, true)
					});
					flag2 = false;
				}
			}
			if (!flag2)
			{
				mailboxChangesManifest = this.RunManualHierarchySync(flag, hierState);
			}
			if (!flag)
			{
				MrsTracer.Provider.Debug("Changes discovered: {0} changed folders, {1} deleted folders.", new object[]
				{
					mailboxChangesManifest.ChangedFolders.Count,
					mailboxChangesManifest.DeletedFolders.Count
				});
			}
			return mailboxChangesManifest;
		}

		protected virtual MailboxChangesManifest RunManualHierarchySync(bool catchup, SyncHierarchyManifestState hierState)
		{
			MrsTracer.Provider.Function("MailboxProviderBase.RunManualHierarchySync", new object[0]);
			this.VerifyMailboxConnection(VerifyMailboxConnectionFlags.None);
			this.ValidateIfSourceMailbox("MailboxProviderBase.RunManualHierarchySync");
			MailboxChangesManifest mailboxChangesManifest = new MailboxChangesManifest();
			List<FolderRec> list = ((IMailbox)this).EnumerateFolderHierarchy(EnumerateFolderHierarchyFlags.None, null);
			EntryIdMap<SyncHierarchyManifestState.FolderData> entryIdMap = new EntryIdMap<SyncHierarchyManifestState.FolderData>();
			List<SyncHierarchyManifestState.FolderData> list2 = new List<SyncHierarchyManifestState.FolderData>();
			foreach (FolderRec folderRec in list)
			{
				if (folderRec.FolderType != FolderType.Search)
				{
					SyncHierarchyManifestState.FolderData folderData = new SyncHierarchyManifestState.FolderData(folderRec);
					entryIdMap[folderData.EntryId] = folderData;
					list2.Add(folderData);
				}
			}
			if (!catchup)
			{
				EntryIdMap<SyncHierarchyManifestState.FolderData> entryIdMap2 = new EntryIdMap<SyncHierarchyManifestState.FolderData>();
				SyncHierarchyManifestState.FolderData[] manualSyncData = hierState.ManualSyncData;
				foreach (SyncHierarchyManifestState.FolderData folderData2 in hierState.ManualSyncData)
				{
					entryIdMap2[folderData2.EntryId] = folderData2;
				}
				mailboxChangesManifest.DeletedFolders = new List<byte[]>();
				foreach (byte[] array in entryIdMap2.Keys)
				{
					if (!entryIdMap.ContainsKey(array))
					{
						mailboxChangesManifest.DeletedFolders.Add(array);
					}
				}
				mailboxChangesManifest.ChangedFolders = new List<byte[]>();
				foreach (FolderRec folderRec2 in list)
				{
					if (folderRec2.FolderType != FolderType.Search)
					{
						SyncHierarchyManifestState.FolderData folderData3;
						if (entryIdMap2.TryGetValue(folderRec2.EntryId, out folderData3))
						{
							if (!CommonUtils.IsSameEntryId(folderData3.ParentId, folderRec2.ParentId) || folderRec2.LastModifyTimestamp > folderData3.LastModifyTimestamp)
							{
								mailboxChangesManifest.ChangedFolders.Add(folderRec2.EntryId);
							}
						}
						else
						{
							mailboxChangesManifest.ChangedFolders.Add(folderRec2.EntryId);
						}
					}
				}
			}
			hierState.ManualSyncData = list2.ToArray();
			return mailboxChangesManifest;
		}

		public virtual List<ItemPropertiesBase> GetMailboxSettings(GetMailboxSettingsFlags flags)
		{
			return null;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.hierarchyChangesFetcher != null)
				{
					this.hierarchyChangesFetcher.Dispose();
					this.hierarchyChangesFetcher = null;
				}
				if (this.rhTracker != null)
				{
					this.rhTracker.Dispose();
					this.rhTracker = null;
				}
				if (this.reservation != null)
				{
					this.reservation.Deactivate(this.MailboxGuid);
					this.reservation = null;
				}
			}
		}

		public const string RootPublicFolderName = "Public Root";

		public const string SyncStateFolderName = "MailboxReplicationService SyncStates";

		public const string UpdateSucceededVar = "UMM_UpdateSucceeded";

		public const string DcNameVar = "UMM_DCName";

		public const string ReportEntriesVar = "UMM_ReportEntries";

		protected const string SyncStateMessageClass = "IPM.MS-Exchange.MailboxSyncState";

		public static readonly StorePropertyDefinition SyncStateStorePropertyDefinition = ItemSchema.TextBody;

		protected static readonly PropTag[] MailboxInformationPropertyTags = new PropTag[]
		{
			PropTag.ContentCount,
			PropTag.DeletedMsgCount,
			PropTag.AssocContentCount,
			PropTag.DeletedAssocMsgCount,
			PropTag.MessageSizeExtended,
			PropTag.DeletedMessageSizeExtended,
			PropTag.AssocMessageSizeExtended,
			PropTag.DeleteAssocMessageSizeExtended,
			PropTag.MailboxPartitionMailboxGuids
		};

		private static readonly long E15MinVersionSupportsOnlySpecifiedPropsForHierarchy = (long)new ServerVersion(15, 0, 922, 0).ToInt();

		protected static readonly byte[] NullFolderKey = Array<byte>.Empty;

		private static readonly ADPropertyDefinition[] UserPropertiesToLoad = new ADPropertyDefinition[]
		{
			ADRecipientSchema.RecipientDisplayType,
			ADMailboxRecipientSchema.UseDatabaseQuotaDefaults,
			ADMailboxRecipientSchema.ProhibitSendReceiveQuota,
			ADUserSchema.RecoverableItemsQuota,
			ADUserSchema.ArchiveQuota,
			ADMailboxRecipientSchema.Database,
			ADUserSchema.ArchiveDatabase,
			ADUserSchema.AggregatedMailboxGuids,
			ADRecipientSchema.AllowedAttributesEffective,
			ADUserSchema.PrimaryMailboxSource
		};

		private static readonly ADPropertyDefinition[] WriteableProperties = new ADPropertyDefinition[]
		{
			ADMailboxRecipientSchema.Database,
			ADMailboxRecipientSchema.ServerLegacyDN
		};

		private static readonly PropTag[] PropTagsForPublicFolderMigration = new PropTag[]
		{
			PropTag.EntryId,
			PropTag.ParentEntryId
		};

		private static readonly PropTag[] PropTagsForRegularMoves = new PropTag[]
		{
			PropTag.EntryId
		};

		private static EventHandler<ErrorReportEventArgs> ummErrorReportHandler = delegate(object sender, ErrorReportEventArgs args)
		{
			args.Handled = true;
		};

		private MailboxReservation reservation;

		private SettingsContextBase configContext;

		private IHierarchyChangesFetcher hierarchyChangesFetcher;

		protected EntryIdMap<bool> publicFoldersToSkip;

		protected WellKnownPrincipalMapper wkpMapper;

		protected int recipientType;

		protected int recipientDisplayType;

		protected long recipientTypeDetails;

		protected bool? useMdbQuotaDefaults;

		protected ulong? mbxQuota;

		protected ulong? mbxDumpsterQuota;

		protected ulong? mbxArchiveQuota;

		protected Guid archiveGuid;

		protected Guid[] alternateMailboxes;

		protected ADObjectId mbxHomeMdb;

		protected ADObjectId archiveMdb;

		protected ResourceHealthTracker rhTracker;

		protected string preferredDomainControllerName;

		protected MapiSyncState syncState;

		protected bool connectedWithoutMailboxSession;

		protected bool isStorageProvider;

		private class ManifestHierarchyCallback : IMapiHierarchyManifestCallback
		{
			public ManifestHierarchyCallback(bool catchup, MailboxChangesManifest changes, EntryIdMap<bool> foldersToSkip, bool isPublicFolderMigration)
			{
				this.catchup = catchup;
				this.changes = changes;
				this.changes.ChangedFolders = new List<byte[]>(0);
				this.changes.DeletedFolders = new List<byte[]>(0);
				this.foldersToSkip = foldersToSkip;
				this.isPublicFolderMigration = isPublicFolderMigration;
			}

			ManifestCallbackStatus IMapiHierarchyManifestCallback.Change(PropValue[] props)
			{
				if (this.catchup)
				{
					return ManifestCallbackStatus.Continue;
				}
				byte[] array = null;
				byte[] parentId = null;
				foreach (PropValue propValue in props)
				{
					if (propValue.PropTag == PropTag.EntryId)
					{
						array = propValue.GetBytes();
					}
					else if (propValue.PropTag == PropTag.ParentEntryId)
					{
						parentId = propValue.GetBytes();
					}
				}
				if (this.ShouldSkipFolder(array, parentId))
				{
					this.foldersToSkip[array] = true;
				}
				else
				{
					this.changes.ChangedFolders.Add(array);
				}
				return ManifestCallbackStatus.Continue;
			}

			ManifestCallbackStatus IMapiHierarchyManifestCallback.Delete(byte[] entryId)
			{
				if (this.catchup)
				{
					return ManifestCallbackStatus.Continue;
				}
				this.changes.DeletedFolders.Add(entryId);
				return ManifestCallbackStatus.Continue;
			}

			private bool ShouldSkipFolder(byte[] entryId, byte[] parentId)
			{
				return this.foldersToSkip.ContainsKey(entryId) || (this.isPublicFolderMigration && parentId != null && this.foldersToSkip.ContainsKey(parentId));
			}

			private readonly bool catchup;

			private readonly bool isPublicFolderMigration;

			private MailboxChangesManifest changes;

			private EntryIdMap<bool> foldersToSkip;
		}
	}
}
