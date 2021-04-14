using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Imap;
using Microsoft.Exchange.Connections.Pop;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxCopierBase
	{
		public MailboxCopierBase(Guid sourceMailboxGuid, Guid targetMailboxGuid, TransactionalRequestJob requestJob, BaseJob mrsJob, MailboxCopierFlags flags, LocalizedString sourceTracingID, LocalizedString targetTracingID)
		{
			this.SourceMailboxGuid = sourceMailboxGuid;
			this.TargetMailboxGuid = targetMailboxGuid;
			this.MRSJob = mrsJob;
			this.Report = mrsJob.Report;
			this.SourceTracingID = sourceTracingID;
			this.TargetTracingID = targetTracingID;
			this.MailboxSizeTracker = new MailboxSizeTracker();
			this.SourceMdbGuid = ((requestJob.SourceDatabase != null) ? requestJob.SourceDatabase.ObjectGuid : (requestJob.RemoteDatabaseGuid ?? Guid.Empty));
			this.DestMdbGuid = ((requestJob.TargetDatabase != null) ? requestJob.TargetDatabase.ObjectGuid : (requestJob.RemoteDatabaseGuid ?? Guid.Empty));
			this.TargetMailboxContainerGuid = requestJob.TargetContainerGuid;
			this.Direction = requestJob.Direction;
			this.Flags = flags;
		}

		protected bool SourceSupportsPagedEnumerateChanges
		{
			get
			{
				return this.SourceMailbox.IsMailboxCapabilitySupported(MailboxCapabilities.PagedEnumerateChanges);
			}
		}

		protected int MaxIncrementalChanges
		{
			get
			{
				return this.GetConfig<int>("MaxIncrementalChanges");
			}
		}

		public static ProxyControlFlags DefaultProxyControlFlags
		{
			get
			{
				ProxyControlFlags proxyControlFlags = ProxyControlFlags.DoNotCompress;
				if (ConfigBase<MRSConfigSchema>.GetConfig<bool>("DisableMrsProxyBuffering"))
				{
					proxyControlFlags |= ProxyControlFlags.DoNotBuffer;
				}
				return proxyControlFlags;
			}
		}

		public Guid SourceMailboxGuid { get; private set; }

		public Guid TargetMailboxGuid { get; private set; }

		public Guid? TargetMailboxContainerGuid { get; private set; }

		public LocalizedString SourceTracingID { get; private set; }

		public LocalizedString TargetTracingID { get; private set; }

		public ReportData Report { get; private set; }

		public BaseJob MRSJob { get; private set; }

		public Guid SourceMdbGuid { get; protected set; }

		public Guid DestMdbGuid { get; protected set; }

		public RequestDirection Direction { get; private set; }

		public MailboxCopierFlags Flags { get; private set; }

		public bool CopyMessagesCompleted { get; internal set; }

		public Guid IsIntegRequestGuid { get; private set; }

		public bool IsIntegDone { get; private set; }

		public bool IsSourceConnected
		{
			get
			{
				return this.SourceMailboxWrapper != null && this.SourceMailbox.IsConnected();
			}
		}

		public bool IsDestinationConnected
		{
			get
			{
				return this.DestMailboxWrapper != null && this.DestMailbox.IsConnected();
			}
		}

		public ISourceMailbox SourceMailbox
		{
			get
			{
				if (this.SourceMailboxWrapper != null)
				{
					return this.SourceMailboxWrapper.SourceMailbox;
				}
				return null;
			}
		}

		public IDestinationMailbox DestMailbox
		{
			get
			{
				if (this.DestMailboxWrapper != null)
				{
					return this.DestMailboxWrapper.DestinationMailbox;
				}
				return null;
			}
		}

		public SourceMailboxWrapper SourceMailboxWrapper { get; protected set; }

		public DestinationMailboxWrapper DestMailboxWrapper { get; protected set; }

		public NamedPropTranslator NamedPropTranslator { get; private set; }

		public PrincipalTranslator PrincipalTranslator { get; private set; }

		public FolderIdTranslator FolderIdTranslator { get; private set; }

		public PersistedSyncData SyncState
		{
			get
			{
				if (this.DestMailboxWrapper == null)
				{
					return null;
				}
				return this.DestMailboxWrapper.SyncState;
			}
			set
			{
				this.DestMailboxWrapper.SyncState = value;
			}
		}

		public MailboxMapiSyncState ICSSyncState
		{
			get
			{
				if (this.DestMailboxWrapper == null)
				{
					return null;
				}
				return this.DestMailboxWrapper.ICSSyncState;
			}
			set
			{
				this.DestMailboxWrapper.ICSSyncState = value;
			}
		}

		public DateTime TimestampWhenPersistentProgressWasMade { get; private set; }

		public MailboxSizeTracker MailboxSizeTracker { get; private set; }

		public bool SupportsRuleAPIs
		{
			get
			{
				return this.SourceMailbox.IsMailboxCapabilitySupported(MailboxCapabilities.FolderRules) && this.DestMailbox.IsMailboxCapabilitySupported(MailboxCapabilities.FolderRules) && !this.IsPst && this.SourceMailbox.IsCapabilitySupported(MRSProxyCapabilities.MergeMailbox) && this.SourceMailbox.IsCapabilitySupported(MRSProxyCapabilities.MailboxOptions) && this.DestMailbox.IsCapabilitySupported(MRSProxyCapabilities.MergeMailbox);
			}
		}

		public bool SupportsAcls
		{
			get
			{
				return !this.MRSJob.CachedRequestJob.SkipFolderACLs && !this.IsPst && this.SourceMailbox.IsMailboxCapabilitySupported(MailboxCapabilities.FolderAcls);
			}
		}

		public bool SourceIsE15OrHigher
		{
			get
			{
				return this.SourceMailboxWrapper.MailboxVersion != null && this.SourceMailboxWrapper.MailboxVersion.Value >= Server.E15MinVersion;
			}
		}

		public bool DestinationIsE15OrHigher
		{
			get
			{
				return this.DestMailboxWrapper.MailboxVersion != null && this.DestMailboxWrapper.MailboxVersion.Value >= Server.E15MinVersion;
			}
		}

		public bool IsUpgradeToE15OrHigher
		{
			get
			{
				return !this.SourceIsE15OrHigher && this.DestinationIsE15OrHigher;
			}
		}

		public bool SupportsPerUserReadUnreadDataTransfer
		{
			get
			{
				return this.SourceMailbox.IsCapabilitySupported(MRSProxyCapabilities.CopyToWithFlags) && this.DestMailbox.IsCapabilitySupported(MRSProxyCapabilities.CopyToWithFlags) && this.SourceMailboxWrapper.MailboxVersion != null && this.SourceMailboxWrapper.MailboxVersion.Value >= MailboxCopierBase.MinExchangeVersionForPerUserReadUnreadDataTransfer && this.DestMailboxWrapper.MailboxVersion != null && this.DestMailboxWrapper.MailboxVersion.Value >= MailboxCopierBase.MinExchangeVersionForPerUserReadUnreadDataTransfer;
			}
		}

		public bool IsPublicFolder
		{
			get
			{
				return this.MRSJob.CachedRequestJob.IsPublicFolderMailboxRestore || this.MRSJob.CachedRequestJob.RequestType == MRSRequestType.PublicFolderMove || this.MRSJob.CachedRequestJob.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox;
			}
		}

		public bool IsPublicFolderMigration
		{
			get
			{
				return this.MRSJob.CachedRequestJob.RequestType == MRSRequestType.PublicFolderMigration || this.MRSJob.CachedRequestJob.RequestType == MRSRequestType.PublicFolderMailboxMigration;
			}
		}

		public bool IsPrimary
		{
			get
			{
				return !this.IsArchive;
			}
		}

		public bool IsArchive
		{
			get
			{
				return this.Flags.HasFlag(MailboxCopierFlags.SourceIsArchive) || this.Flags.HasFlag(MailboxCopierFlags.TargetIsArchive);
			}
		}

		public bool IsPst
		{
			get
			{
				return this.Flags.HasFlag(MailboxCopierFlags.SourceIsPST) || this.Flags.HasFlag(MailboxCopierFlags.TargetIsPST);
			}
		}

		public bool IsOlcSync
		{
			get
			{
				return this.Flags.HasFlag(MailboxCopierFlags.Olc);
			}
		}

		public bool IsRoot
		{
			get
			{
				return this.Flags.HasFlag(MailboxCopierFlags.Root);
			}
		}

		public bool IsIncrementalSyncPaged
		{
			get
			{
				return this.SourceSupportsPagedEnumerateChanges && this.MaxIncrementalChanges != 0;
			}
		}

		public MailboxServerInformation SourceServerInfo { get; set; }

		public MailboxServerInformation TargetServerInfo { get; set; }

		public void ConfigDestinationMailbox(IDestinationMailbox destMailbox)
		{
			this.DestMailboxWrapper = new DestinationMailboxWrapper(destMailbox, MailboxWrapperFlags.Target, this.TargetTracingID, new Guid[]
			{
				this.MRSJob.RequestJobGuid,
				this.TargetMailboxGuid
			});
			this.NamedPropTranslator = new NamedPropTranslator(new Action<List<BadMessageRec>>(this.ReportBadItems), this.SourceMailboxWrapper.NamedPropMapper, this.DestMailboxWrapper.NamedPropMapper);
		}

		public void Config(ISourceMailbox sourceMailbox, IDestinationMailbox destMailbox)
		{
			MailboxWrapperFlags mailboxWrapperFlags = MailboxWrapperFlags.Source;
			MailboxWrapperFlags mailboxWrapperFlags2 = MailboxWrapperFlags.Target;
			if ((this.Flags & MailboxCopierFlags.SourceIsPST) != MailboxCopierFlags.None)
			{
				mailboxWrapperFlags |= MailboxWrapperFlags.PST;
			}
			if ((this.Flags & MailboxCopierFlags.TargetIsPST) != MailboxCopierFlags.None)
			{
				mailboxWrapperFlags2 |= MailboxWrapperFlags.PST;
			}
			if ((this.Flags & MailboxCopierFlags.SourceIsArchive) != MailboxCopierFlags.None)
			{
				mailboxWrapperFlags |= MailboxWrapperFlags.Archive;
			}
			if ((this.Flags & MailboxCopierFlags.TargetIsArchive) != MailboxCopierFlags.None)
			{
				mailboxWrapperFlags2 |= MailboxWrapperFlags.Archive;
			}
			this.SourceMailboxWrapper = new SourceMailboxWrapper(sourceMailbox, mailboxWrapperFlags, this.SourceTracingID);
			this.DestMailboxWrapper = new DestinationMailboxWrapper(destMailbox, mailboxWrapperFlags2, this.TargetTracingID, new Guid[]
			{
				this.MRSJob.RequestJobGuid,
				this.TargetMailboxGuid
			});
			this.NamedPropTranslator = new NamedPropTranslator(new Action<List<BadMessageRec>>(this.ReportBadItems), this.SourceMailboxWrapper.NamedPropMapper, this.DestMailboxWrapper.NamedPropMapper);
		}

		public void ConfigTranslators(PrincipalTranslator principalTranslator, FolderIdTranslator folderIdTranslator)
		{
			this.PrincipalTranslator = principalTranslator;
			this.FolderIdTranslator = folderIdTranslator;
		}

		public virtual void ClearCachedData()
		{
			if (this.NamedPropTranslator != null)
			{
				this.NamedPropTranslator.Clear();
			}
			if (this.PrincipalTranslator != null)
			{
				this.PrincipalTranslator.Clear();
			}
			if (this.SourceMailboxWrapper != null)
			{
				this.SourceMailboxWrapper.Clear();
			}
			if (this.DestMailboxWrapper != null)
			{
				this.DestMailboxWrapper.Clear();
			}
		}

		public SyncStateError LoadSyncState(ReportData report)
		{
			SyncStateFlags flags = (this.MRSJob.CachedRequestJob.RequestType == MRSRequestType.Sync) ? SyncStateFlags.Replay : SyncStateFlags.Default;
			return this.DestMailboxWrapper.LoadSyncState(this.MRSJob.CachedRequestJob.RequestGuid, report, flags);
		}

		public void SaveSyncState()
		{
			MrsTracer.Service.Debug("Saving state message changes", new object[0]);
			this.DestMailboxWrapper.SaveSyncState();
		}

		public void ClearSyncState(SyncStateClearReason reason)
		{
			this.DestMailboxWrapper.ClearSyncState();
			this.UpdateTimestampWhenPersistentProgressWasMade();
			this.MRSJob.Report.Append(MrsStrings.ReportSyncStateCleared(this.SourceMailboxGuid, reason.ToString()));
		}

		public T GetConfig<T>(string settingName)
		{
			return this.MRSJob.GetConfig<T>(settingName);
		}

		public virtual FolderHierarchy GetSourceFolderHierarchy()
		{
			FolderHierarchyFlags folderHierarchyFlags = FolderHierarchyFlags.None;
			if (this.IsPublicFolder)
			{
				folderHierarchyFlags |= FolderHierarchyFlags.PublicFolderMailbox;
			}
			FolderHierarchy folderHierarchy = new FolderHierarchy(folderHierarchyFlags, this.SourceMailboxWrapper);
			folderHierarchy.LoadHierarchy(EnumerateFolderHierarchyFlags.None, null, false, this.GetAdditionalFolderPtags());
			this.cachedSourceHierarchy = folderHierarchy;
			return folderHierarchy;
		}

		public virtual void ConfigureProviders()
		{
		}

		public virtual void UnconfigureProviders()
		{
			this.ClearCachedData();
			if (this.SourceMailboxWrapper != null)
			{
				this.SourceMailboxWrapper.Dispose();
				this.SourceMailboxWrapper = null;
			}
			if (this.DestMailboxWrapper != null)
			{
				this.DestMailboxWrapper.Dispose();
				this.DestMailboxWrapper = null;
			}
		}

		public virtual void ConnectSourceMailbox(MailboxConnectFlags flags)
		{
			this.SourceMailbox.Connect(this.MRSJob.GetConnectFlags(flags));
		}

		public virtual void ConnectDestinationMailbox(MailboxConnectFlags flags)
		{
			this.DestMailbox.Connect(this.MRSJob.GetConnectFlags(flags));
		}

		public virtual void PostCreateDestinationMailbox()
		{
		}

		public virtual void Disconnect()
		{
			if (this.SourceMailboxWrapper != null)
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					this.SourceMailbox.Disconnect();
				}, delegate(Exception failure)
				{
					this.Report.Append(MrsStrings.ReportFailedToDisconnectFromSource2(CommonUtils.GetFailureType(failure)), failure, ReportEntryFlags.Cleanup | ReportEntryFlags.Source);
				});
			}
			if (this.DestMailboxWrapper != null)
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					this.DestMailbox.Disconnect();
				}, delegate(Exception failure)
				{
					this.Report.Append(MrsStrings.ReportFailedToDisconnectFromDestination2(CommonUtils.GetFailureType(failure)), failure, ReportEntryFlags.Cleanup | ReportEntryFlags.Target);
				});
			}
		}

		public virtual FolderMap GetSourceFolderMap(GetFolderMapFlags flags)
		{
			this.SourceMailboxWrapper.LoadFolderMap(flags, () => new FolderMap(this.SourceMailboxWrapper.LoadFolders<FolderRecWrapper>(EnumerateFolderHierarchyFlags.None, this.GetAdditionalFolderPtags())));
			return this.SourceMailboxWrapper.FolderMap;
		}

		public virtual FolderMap GetDestinationFolderMap(GetFolderMapFlags flags)
		{
			this.DestMailboxWrapper.LoadFolderMap(flags, () => new FolderMap(this.DestMailboxWrapper.LoadFolders<FolderRecWrapper>(EnumerateFolderHierarchyFlags.None, null)));
			return this.DestMailboxWrapper.FolderMap;
		}

		protected virtual bool ShouldCompareParentIDs()
		{
			return true;
		}

		protected virtual EnumerateMessagesFlags GetAdditionalEnumerateMessagesFlagsForContentVerification()
		{
			return (EnumerateMessagesFlags)0;
		}

		protected virtual byte[] GetMessageKey(MessageRec messageRec, MailboxWrapperFlags flags)
		{
			return messageRec.EntryId;
		}

		protected virtual PropTag[] GetEnumerateMessagesPropsForContentVerification(MailboxWrapperFlags flags)
		{
			return null;
		}

		protected virtual bool IsIgnorableItem(MessageRec msg)
		{
			if (msg.IsFAI && this.GetConfig<bool>("ContentVerificationIgnoreFAI"))
			{
				MrsTracer.Service.Debug("Ignoring missing FAI item", new object[0]);
				return true;
			}
			string text = msg[PropTag.MessageClass] as string;
			if (text != null && CommonUtils.IsValueInWildcardedList(text, this.GetConfig<string>("ContentVerificationIgnorableMsgClasses")))
			{
				MrsTracer.Service.Debug("Ignoring missing {0} item", new object[]
				{
					text
				});
				return true;
			}
			return false;
		}

		protected virtual RestrictionData GetContentRestriction()
		{
			return null;
		}

		public FolderSizeRec VerifyFolderContents(FolderRecWrapper folderRecWrapper, WellKnownFolderType wellKnownFolderType, bool verifyInboxProperties = false)
		{
			MrsTracer.Service.Debug("Verifying folder '{0}'", new object[]
			{
				folderRecWrapper.FullFolderName
			});
			FolderSizeRec folderSizeRec = new FolderSizeRec();
			folderSizeRec.FolderID = folderRecWrapper.EntryId;
			folderSizeRec.ParentID = folderRecWrapper.ParentId;
			folderSizeRec.FolderPath = folderRecWrapper.FullFolderName;
			folderSizeRec.WKFType = wellKnownFolderType;
			folderSizeRec.MailboxGuid = this.TargetMailboxGuid;
			byte[] destinationFolderEntryId = this.GetDestinationFolderEntryId(folderRecWrapper.EntryId);
			PropValueData[] array = null;
			PropValueData[] array2 = null;
			if (wellKnownFolderType != WellKnownFolderType.Inbox)
			{
				verifyInboxProperties = false;
			}
			FolderMap destinationFolderMap = this.GetDestinationFolderMap(GetFolderMapFlags.None);
			FolderRecWrapper folderRecWrapper2 = destinationFolderMap[destinationFolderEntryId];
			if (folderRecWrapper2 == null)
			{
				if (folderSizeRec.MissingItems == null)
				{
					folderSizeRec.MissingItems = new List<BadMessageRec>();
				}
				MrsTracer.Service.Error("Target folder is missing.", new object[0]);
				folderSizeRec.MissingItems.Add(BadMessageRec.MissingFolder(folderRecWrapper.FolderRec, folderRecWrapper.FullFolderName, wellKnownFolderType));
			}
			else if (this.ShouldCompareParentIDs())
			{
				byte[] destinationFolderEntryId2 = this.GetDestinationFolderEntryId(folderRecWrapper.ParentId);
				if (!CommonUtils.IsSameEntryId(destinationFolderEntryId2, folderRecWrapper2.ParentId))
				{
					if (folderSizeRec.MissingItems == null)
					{
						folderSizeRec.MissingItems = new List<BadMessageRec>();
					}
					MrsTracer.Service.Error("Target folder is misplaced.", new object[0]);
					folderSizeRec.MissingItems.Add(BadMessageRec.MisplacedFolder(folderRecWrapper.FolderRec, folderRecWrapper.FullFolderName, wellKnownFolderType, folderRecWrapper2.ParentId));
				}
			}
			if (this.SourceIsE15OrHigher && !verifyInboxProperties && (int)(folderRecWrapper.FolderRec[PropTag.ContentCount] ?? -1) == 0 && (int)(folderRecWrapper.FolderRec[PropTag.AssocContentCount] ?? -1) == 0)
			{
				return folderSizeRec;
			}
			EnumerateMessagesFlags emFlags = EnumerateMessagesFlags.RegularMessages | EnumerateMessagesFlags.IncludeExtendedData | this.GetAdditionalEnumerateMessagesFlagsForContentVerification();
			List<MessageRec> list = null;
			EntryIdMap<MessageRec> entryIdMap = new EntryIdMap<MessageRec>();
			StringBuilder stringBuilder = null;
			using (ISourceFolder folder = this.SourceMailbox.GetFolder(folderRecWrapper.EntryId))
			{
				if (folder == null)
				{
					MrsTracer.Service.Error("Something deleted source folder from under us?", new object[0]);
					if ((this.Flags & MailboxCopierFlags.Merge) != MailboxCopierFlags.None)
					{
						return null;
					}
					throw new FolderIsMissingPermanentException(folderRecWrapper.FullFolderName);
				}
				else
				{
					RestrictionData contentRestriction = this.GetContentRestriction();
					if (contentRestriction != null)
					{
						folder.SetContentsRestriction(contentRestriction);
					}
					MrsTracer.Service.Debug("Enumerating messages in source folder", new object[0]);
					list = folder.EnumerateMessages(emFlags, this.GetEnumerateMessagesPropsForContentVerification(MailboxWrapperFlags.Source));
					MrsTracer.Service.Debug("{0} message(s) found.", new object[]
					{
						list.Count
					});
					foreach (MessageRec messageRec in list)
					{
						if (messageRec.IsFAI)
						{
							folderSizeRec.SourceFAI.Add(messageRec);
						}
						else
						{
							folderSizeRec.Source.Add(messageRec);
						}
					}
					if (verifyInboxProperties)
					{
						MrsTracer.Service.Debug("Verifying default folder references on inbox", new object[0]);
						array = folder.GetProps(MailboxCopierBase.InboxPropertiesToValidate);
					}
				}
			}
			if (folderRecWrapper2 != null)
			{
				using (IDestinationFolder folder2 = this.DestMailbox.GetFolder(destinationFolderEntryId))
				{
					if (folder2 != null)
					{
						MrsTracer.Service.Debug("Enumerating messages in dest folder", new object[0]);
						List<MessageRec> list2 = folder2.EnumerateMessages(emFlags, this.GetEnumerateMessagesPropsForContentVerification(MailboxWrapperFlags.Target));
						MrsTracer.Service.Debug("{0} message(s) found.", new object[]
						{
							list2.Count
						});
						if (this.MRSJob.TestIntegration.LogContentDetails)
						{
							stringBuilder = new StringBuilder();
							stringBuilder.AppendLine(string.Format("Target folder contents: {0}", folderRecWrapper.FullFolderName));
						}
						foreach (MessageRec messageRec2 in list2)
						{
							entryIdMap[this.GetMessageKey(messageRec2, MailboxWrapperFlags.Target)] = messageRec2;
							if (this.MRSJob.TestIntegration.LogContentDetails)
							{
								stringBuilder.AppendLine(string.Format("TargetItem: {0}{1}", TraceUtils.DumpEntryId(this.GetMessageKey(messageRec2, MailboxWrapperFlags.Target)), messageRec2.IsFAI ? ", FAI" : string.Empty));
							}
						}
						if (this.MRSJob.TestIntegration.LogContentDetails)
						{
							this.MRSJob.Report.AppendDebug(stringBuilder.ToString());
						}
						if (verifyInboxProperties)
						{
							MrsTracer.Service.Debug("Verifying default folder references on inbox", new object[0]);
							array2 = folder2.GetProps(MailboxCopierBase.InboxPropertiesToValidate);
						}
					}
					else
					{
						MrsTracer.Service.Error("Something deleted target folder from under us.", new object[0]);
					}
				}
			}
			List<MessageRec> list3 = null;
			if (this.MRSJob.TestIntegration.LogContentDetails)
			{
				stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(string.Format("Source folder verification: {0}", folderRecWrapper.FullFolderName));
			}
			foreach (MessageRec messageRec3 in list)
			{
				if (this.MRSJob.TestIntegration.LogContentDetails)
				{
					stringBuilder.AppendLine(string.Format("SourceItem: {0}{1}", TraceUtils.DumpEntryId(this.GetMessageKey(messageRec3, MailboxWrapperFlags.Source)), messageRec3.IsFAI ? ", FAI" : string.Empty));
				}
				MessageRec messageRec4;
				if (entryIdMap.TryGetValue(this.GetMessageKey(messageRec3, MailboxWrapperFlags.Source), out messageRec4))
				{
					if (messageRec4.IsFAI)
					{
						folderSizeRec.TargetFAI.Add(messageRec4);
					}
					else
					{
						folderSizeRec.Target.Add(messageRec4);
					}
				}
				else
				{
					BadItemMarker badItemMarker;
					this.SyncState.BadItems.TryGetValue(messageRec3.EntryId, out badItemMarker);
					if (badItemMarker != null && badItemMarker.Kind == BadItemKind.CorruptItem)
					{
						folderSizeRec.Corrupt.Add(messageRec3);
					}
					else if (badItemMarker != null && badItemMarker.Kind == BadItemKind.LargeItem)
					{
						folderSizeRec.Large.Add(messageRec3);
					}
					else
					{
						if (list3 == null)
						{
							list3 = new List<MessageRec>();
						}
						list3.Add(messageRec3);
						if (this.MRSJob.TestIntegration.LogContentDetails)
						{
							stringBuilder.AppendLine("The above item is suspect.");
						}
					}
				}
			}
			if (this.MRSJob.TestIntegration.LogContentDetails)
			{
				this.MRSJob.Report.AppendDebug(stringBuilder.ToString());
			}
			if (list3 != null)
			{
				List<MessageRec> lookedUpItems = new List<MessageRec>(list3.Count);
				CommonUtils.ProcessInBatches<MessageRec>(list3.ToArray(), 1000, delegate(MessageRec[] batch)
				{
					EntryIdMap<MessageRec> entryIdMap2;
					EntryIdMap<FolderRec> entryIdMap3;
					MapiUtils.LookupBadMessagesInMailbox(this.SourceMailbox, new List<MessageRec>(batch), out entryIdMap2, out entryIdMap3);
					lookedUpItems.AddRange(entryIdMap2.Values);
				});
				foreach (MessageRec messageRec5 in lookedUpItems)
				{
					if (this.IsIgnorableItem(messageRec5))
					{
						folderSizeRec.Skipped.Add(messageRec5);
					}
					else
					{
						folderSizeRec.Missing.Add(messageRec5);
						if (folderSizeRec.MissingItems == null)
						{
							folderSizeRec.MissingItems = new List<BadMessageRec>();
						}
						BadMessageRec badMessageRec = BadMessageRec.Item(messageRec5, folderRecWrapper.FolderRec, null);
						MrsTracer.Service.Error("Missing item found: {0}", new object[]
						{
							badMessageRec.ToString()
						});
						folderSizeRec.MissingItems.Add(badMessageRec);
					}
				}
			}
			if (verifyInboxProperties)
			{
				if (array == null || array2 == null || array.Length != array2.Length)
				{
					this.ReportBadItem(BadMessageRec.FolderProperty(folderRecWrapper.FolderRec, PropTag.Unresolved, "Unknown", "Unknown"));
				}
				for (int i = 0; i < array.Length; i++)
				{
					PropValueData propValueData = array[i];
					PropValueData propValueData2 = array2[i];
					PropTag propTag = (PropTag)propValueData.PropTag;
					PropTag propTag2 = (PropTag)propValueData2.PropTag;
					if (propTag.ValueType() == PropType.Error || propValueData.Value == null)
					{
						MrsTracer.Service.Debug("Property '{0}' does not exist on source.", new object[]
						{
							propTag
						});
					}
					else if (propTag != propTag2)
					{
						this.ReportBadItem(BadMessageRec.FolderProperty(folderRecWrapper.FolderRec, propTag, propTag.ToString(), propTag2.ToString()));
					}
					else
					{
						PropType propType = propTag.ValueType();
						if (propType == PropType.Binary && !ArrayComparer<byte>.EqualityComparer.Equals(propValueData.Value as byte[], propValueData2.Value as byte[]))
						{
							this.ReportBadItem(BadMessageRec.FolderProperty(folderRecWrapper.FolderRec, propTag, TraceUtils.DumpEntryId(propValueData.Value as byte[]), TraceUtils.DumpEntryId(propValueData2.Value as byte[])));
						}
					}
				}
			}
			return folderSizeRec;
		}

		private void ReportBadItem(BadMessageRec record)
		{
			this.ReportBadItems(new List<BadMessageRec>(1)
			{
				record
			});
		}

		public virtual void PreProcessHierarchy()
		{
		}

		public virtual PropTag[] GetAdditionalFolderPtags()
		{
			return MailboxCopierBase.AdditionalFolderPtags;
		}

		protected virtual PropTag[] GetAdditionalExcludedFolderPtags()
		{
			return null;
		}

		public virtual void CopyFolderProperties(FolderRecWrapper folderRec, ISourceFolder sourceFolder, IDestinationFolder destFolder, FolderRecDataFlags dataToCopy, out bool wasPropertyCopyingSkipped)
		{
			if (destFolder == null)
			{
				throw new FolderCopyFailedPermanentException(folderRec.FullFolderName);
			}
			if (this.MRSJob.CachedRequestJob.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox || this.IsPublicFolderMigration)
			{
				destFolder.SetProps(CommonUtils.PropertiesToDelete);
			}
			wasPropertyCopyingSkipped = false;
			MrsTracer.Service.Debug("Copying folder properties: \"{0}\"", new object[]
			{
				folderRec.FullFolderName
			});
			folderRec.EnsureDataLoaded(sourceFolder, dataToCopy, this);
			this.TranslateFolderData(folderRec);
			if (folderRec.FolderType == FolderType.Search && (dataToCopy & FolderRecDataFlags.SearchCriteria) != FolderRecDataFlags.None)
			{
				this.CopySearchFolderCriteria(folderRec, destFolder);
			}
			HashSet<PropTag> ptagsToExclude = new HashSet<PropTag>(MailboxCopierBase.AlwaysExcludedFolderPtags);
			if (folderRec.FolderType == FolderType.Root)
			{
				ptagsToExclude.UnionWith(MailboxCopierBase.ExcludedRootFolderPtags);
			}
			FolderMapping folderMapping = folderRec as FolderMapping;
			if (folderMapping != null && folderMapping.TargetFolder != null)
			{
				if (folderMapping.TargetFolder.Flags.HasFlag(FolderMappingFlags.Root))
				{
					ptagsToExclude.UnionWith(MailboxCopierBase.ExcludedRootFolderPtags);
					ptagsToExclude.UnionWith(FolderHierarchyUtils.GetInboxReferencePtags(this.SourceMailboxWrapper.NamedPropMapper, this.SourceMailboxWrapper.MailboxVersion));
				}
				if (folderMapping.TargetFolder.WKFType != WellKnownFolderType.None || StringComparer.OrdinalIgnoreCase.Equals(folderMapping.FolderName, folderMapping.TargetFolder.FolderName))
				{
					ptagsToExclude.Add(PropTag.DisplayName);
				}
				if (folderMapping.TargetFolder.WKFType == WellKnownFolderType.Inbox)
				{
					ptagsToExclude.UnionWith(FolderHierarchyUtils.GetInboxReferencePtags(this.SourceMailboxWrapper.NamedPropMapper, this.SourceMailboxWrapper.MailboxVersion));
				}
			}
			if (this.MRSJob.CachedRequestJob.SkipFolderRules || this.SupportsRuleAPIs)
			{
				ptagsToExclude.UnionWith(MailboxCopierBase.RuleFolderPtags);
			}
			if (this.GetAdditionalExcludedFolderPtags() != null)
			{
				ptagsToExclude.UnionWith(this.GetAdditionalExcludedFolderPtags());
			}
			bool propertiesSkippedAndUnreported = false;
			CommonUtils.ProcessKnownExceptions(delegate
			{
				using (IFxProxy fxProxy = destFolder.GetFxProxy(FastTransferFlags.None))
				{
					using (IFxProxy fxProxy2 = this.CreateFxProxyTransmissionPipeline(fxProxy))
					{
						sourceFolder.CopyTo(fxProxy2, CopyPropertiesFlags.None, ptagsToExclude.ToArray<PropTag>());
					}
				}
			}, delegate(Exception failure)
			{
				if (MapiUtils.IsBadItemIndicator(failure))
				{
					if (!this.GetConfig<MRSConfigurableFeatures>("DisabledFeatures").HasFlag(MRSConfigurableFeatures.SkipCopyFolderPropertyCheck) && CommonUtils.ExceptionIsAny(failure, new WellKnownException[]
					{
						WellKnownException.MapiCorruptMidsetDeleted
					}))
					{
						this.MRSJob.Report.AppendDebug("Encountered midset deleted exception while copying properties, will skip next time");
						propertiesSkippedAndUnreported = true;
					}
					else
					{
						List<BadMessageRec> list = new List<BadMessageRec>(1);
						list.Add(BadMessageRec.Folder(folderRec.FolderRec, BadItemKind.CorruptFolderProperty, failure));
						this.ReportBadItems(list);
					}
					return true;
				}
				return false;
			});
			wasPropertyCopyingSkipped = propertiesSkippedAndUnreported;
			if (this.IsOlcSync && this.DestMailbox.IsMailboxCapabilitySupported(MailboxCapabilities.ExportFolders) && this.SourceMailbox.IsMailboxCapabilitySupported(MailboxCapabilities.ExportFolders))
			{
				EntryIdMap<byte[]> entryIdMap = new EntryIdMap<byte[]>();
				byte[] destinationFolderEntryId = this.GetDestinationFolderEntryId(folderRec.EntryId);
				entryIdMap[folderRec.EntryId] = destinationFolderEntryId;
				CommonUtils.ProcessKnownExceptions(delegate
				{
					using (IFxProxyPool fxProxyPoolTransmissionPipeline = this.GetFxProxyPoolTransmissionPipeline(entryIdMap))
					{
						this.SourceMailbox.ExportFolders(new List<byte[]>(entryIdMap.Keys), fxProxyPoolTransmissionPipeline, ExportFoldersDataToCopyFlags.None, GetFolderRecFlags.None, null, CopyPropertiesFlags.None, null, AclFlags.None);
					}
				}, delegate(Exception failure)
				{
					if (MapiUtils.IsBadItemIndicator(failure))
					{
						List<BadMessageRec> list = new List<BadMessageRec>(1);
						list.Add(BadMessageRec.Folder(folderRec.FolderRec, BadItemKind.CorruptFolderProperty, failure));
						this.ReportBadItems(list);
						return true;
					}
					return false;
				});
			}
		}

		public void CopyFolderRules(FolderRecWrapper folderRec, ISourceFolder sourceFolder, IDestinationFolder destFolder)
		{
			if (destFolder == null)
			{
				throw new FolderCopyFailedPermanentException(folderRec.FullFolderName);
			}
			if (this.MRSJob.CachedRequestJob.SkipFolderRules || !this.SupportsRuleAPIs || !folderRec.AreRulesSupported())
			{
				return;
			}
			MrsTracer.Service.Debug("Copying folder rules: \"{0}\"", new object[]
			{
				folderRec.FullFolderName
			});
			folderRec.EnsureDataLoaded(sourceFolder, FolderRecDataFlags.Rules, this);
			this.TranslateFolderData(folderRec);
			folderRec.WriteRules(destFolder, new Action<List<BadMessageRec>>(this.ReportBadItems));
		}

		public void CopyFolderAcl(FolderRecWrapper folderRec, ISourceFolder sourceFolder, IDestinationFolder destFolder)
		{
			if (folderRec.FolderType == FolderType.Search || !this.SupportsAcls)
			{
				return;
			}
			FolderRecDataFlags dataToCopy;
			if (CommonUtils.ShouldUseExtendedAclInformation(this.SourceMailbox, this.DestMailbox))
			{
				dataToCopy = FolderRecDataFlags.ExtendedAclInformation;
			}
			else if (this.Flags.HasFlag(MailboxCopierFlags.CrossOrg) && (this.Flags.HasFlag(MailboxCopierFlags.Merge) || this.Flags.HasFlag(MailboxCopierFlags.PublicFolderMigration)))
			{
				dataToCopy = FolderRecDataFlags.FolderAcls;
			}
			else
			{
				dataToCopy = FolderRecDataFlags.SecurityDescriptors;
			}
			folderRec.EnsureDataLoaded(sourceFolder, dataToCopy, this);
			this.TranslateFolderData(folderRec);
			bool sourceIsTitanium = this.SourceMailboxWrapper.MailboxVersion < Server.E2007MinVersion;
			bool targetIsTitanium = this.DestMailboxWrapper.MailboxVersion < Server.E2007MinVersion;
			CommonUtils.ProcessKnownExceptions(delegate
			{
				if (dataToCopy.HasFlag(FolderRecDataFlags.FolderAcls))
				{
					destFolder.SetACL(SecurityProp.NTSD, folderRec.FolderACL);
					if (!sourceIsTitanium && !targetIsTitanium)
					{
						destFolder.SetACL(SecurityProp.FreeBusyNTSD, folderRec.FolderFreeBusyACL);
						return;
					}
				}
				else
				{
					if (dataToCopy.HasFlag(FolderRecDataFlags.ExtendedAclInformation))
					{
						destFolder.SetExtendedAcl(AclFlags.FolderAcl, folderRec.FolderACL);
						destFolder.SetExtendedAcl(AclFlags.FreeBusyAcl, folderRec.FolderFreeBusyACL);
						return;
					}
					if (dataToCopy.HasFlag(FolderRecDataFlags.SecurityDescriptors))
					{
						destFolder.SetSecurityDescriptor(SecurityProp.NTSD, folderRec.FolderNTSD);
						if (!sourceIsTitanium && !targetIsTitanium)
						{
							destFolder.SetSecurityDescriptor(SecurityProp.FreeBusyNTSD, folderRec.FolderFreeBusyNTSD);
						}
					}
				}
			}, delegate(Exception failure)
			{
				if (MapiUtils.IsBadItemIndicator(failure))
				{
					List<BadMessageRec> list = new List<BadMessageRec>(1);
					list.Add(BadMessageRec.Folder(folderRec.FolderRec, BadItemKind.CorruptFolderACL, failure));
					this.ReportBadItems(list);
					return true;
				}
				return false;
			});
		}

		public void CopyLocalDirectoryEntryId()
		{
			if (this.IsPublicFolderMigration || this.MRSJob.CachedRequestJob.RequestType == MRSRequestType.PublicFolderMove || this.MRSJob.CachedRequestJob.RequestType == MRSRequestType.FolderMove)
			{
				return;
			}
			CommonUtils.ProcessKnownExceptions(delegate
			{
				PropValueData[] props = this.DestMailbox.GetProps(MailboxCopierBase.LocalDirectoryEntryIdArray);
				if (props == null || props.Length != 1)
				{
					MrsTracer.Service.Warning("Failed to read local directory entry id from destination.", new object[0]);
					return;
				}
				int propTag = props[0].PropTag;
				byte[] entryId = props[0].Value as byte[];
				if (IdConverter.IsValidMessageEntryId(entryId))
				{
					return;
				}
				props = this.SourceMailbox.GetProps(MailboxCopierBase.LocalDirectoryEntryIdArray);
				if (props != null && props.Length == 1)
				{
					byte[] entryId2 = props[0].Value as byte[];
					if (IdConverter.IsValidMessageEntryId(entryId2))
					{
						this.DestMailbox.SetProps(props);
					}
				}
			}, delegate(Exception failure)
			{
				if (CommonUtils.ExceptionIsAny(failure, new WellKnownException[]
				{
					WellKnownException.MRSPermanent,
					WellKnownException.DataProviderPermanent,
					WellKnownException.CorruptData
				}))
				{
					MrsTracer.Service.Warning("Failed to update local directory entry id on mailbox. Error {0}", new object[]
					{
						failure.ToString()
					});
					CommonUtils.FullExceptionMessage(failure);
					ExecutionContext.GetDataContext(failure);
					this.ReportBadItems(new List<BadMessageRec>(1)
					{
						BadMessageRec.Folder(new FolderRec
						{
							EntryId = new byte[1]
						}, BadItemKind.CorruptFolderProperty, failure as LocalizedException)
					});
					return true;
				}
				return false;
			});
		}

		public void TranslateFolderData(FolderRecWrapper folderRec)
		{
			folderRec.EnumerateMappableData(this);
			folderRec.TranslateMappableData(this);
		}

		public virtual bool ShouldCreateFolder(FolderMap.EnumFolderContext context, FolderRecWrapper sourceFolderRecWrapper)
		{
			return true;
		}

		public void ExchangeSourceAndTargetVersions()
		{
			if (!this.IsOlcSync)
			{
				this.SourceMailbox.SetOtherSideVersion(this.DestMailbox.GetVersion());
			}
			this.DestMailbox.SetOtherSideVersion(this.SourceMailbox.GetVersion());
		}

		protected virtual bool HasSourceFolderContents(FolderRecWrapper sourceFolderRec)
		{
			return true;
		}

		protected virtual bool ShouldCopyFolderProperties(FolderRecWrapper sourceFolderRec)
		{
			return true;
		}

		protected virtual IFxProxyPool GetFxProxyPoolTransmissionPipeline(EntryIdMap<byte[]> sourceMap)
		{
			IFxProxyPool fxProxyPool = this.DestMailbox.GetFxProxyPool(sourceMap.Keys);
			return this.CreateFxProxyPoolTransmissionPipeline(fxProxyPool);
		}

		public virtual void CreateFolder(FolderMap.EnumFolderContext context, FolderRecWrapper sourceFolderRecWrapper, CreateFolderFlags createFolderFlags, out byte[] newFolderEntryId)
		{
			byte[] entryId = null;
			if (sourceFolderRecWrapper.IsInternalAccess)
			{
				if (!this.DestMailbox.IsCapabilitySupported(MRSProxyCapabilities.InternalAccessFolderCreation))
				{
					throw new InternalAccessFolderCreationIsNotSupportedException();
				}
				createFolderFlags |= CreateFolderFlags.InternalAccess;
			}
			CommonUtils.TreatMissingFolderAsTransient(delegate
			{
				this.DestMailbox.CreateFolder(sourceFolderRecWrapper.FolderRec, createFolderFlags, out entryId);
			}, sourceFolderRecWrapper.FolderRec.ParentId, new Func<byte[], IFolder>(this.DestMailboxWrapper.GetFolder));
			newFolderEntryId = entryId;
			if (this.MRSJob.TestIntegration.LogContentDetails)
			{
				this.MRSJob.Report.AppendDebug(string.Format("Folder created: Name '{0}', FolderID {1}, ParentID {2}", sourceFolderRecWrapper.FolderName, TraceUtils.DumpEntryId(newFolderEntryId), TraceUtils.DumpEntryId(sourceFolderRecWrapper.ParentId)));
			}
		}

		public virtual byte[] GetSourceFolderEntryId(FolderRecWrapper destinationFolderRec)
		{
			return destinationFolderRec.EntryId;
		}

		public virtual byte[] GetDestinationFolderEntryId(byte[] srcFolderEntryId)
		{
			return srcFolderEntryId;
		}

		public virtual IFxProxyPool GetDestinationFxProxyPool(ICollection<byte[]> folderIds)
		{
			return this.DestMailbox.GetFxProxyPool(folderIds);
		}

		public virtual bool IsContentAvailableInTargetMailbox(FolderRecWrapper destinationFolderRec)
		{
			return true;
		}

		public void CatchupFolder(FolderRec folderRec, ISourceFolder srcFolder)
		{
			FolderStateSnapshot folderStateSnapshot = this.ICSSyncState[folderRec.EntryId];
			if (folderStateSnapshot.LocalCommitTimeMax != DateTime.MinValue)
			{
				MrsTracer.Service.Debug("Folder contents catchup was already run.", new object[0]);
				return;
			}
			MrsTracer.Service.Debug("Catching up folder contents.", new object[0]);
			if (!folderRec.IsGhosted)
			{
				srcFolder.EnumerateChanges(EnumerateContentChangesFlags.Catchup, 0);
			}
			folderStateSnapshot.UpdateContentsCopied(folderRec);
		}

		public void CopyMessages(List<MessageRec> batch)
		{
			this.CopyMessageBatch(batch, null);
		}

		public ServerHealthStatus CheckServersHealth()
		{
			ServerHealthStatus serverHealthStatus = new ServerHealthStatus(ServerHealthState.Healthy);
			if (this.MRSJob.TestIntegration.UseLegacyCheckForHaCiHealthQuery || (this.DestMailboxWrapper.MailboxVersion != null && this.DestMailboxWrapper.MailboxVersion.Value < Server.E15MinVersion))
			{
				serverHealthStatus = this.DestMailbox.CheckServerHealth();
			}
			if (serverHealthStatus.HealthState == ServerHealthState.Healthy)
			{
				if ((this.MRSJob.TestIntegration.UseLegacyCheckForHaCiHealthQuery || (this.SourceMailboxWrapper.MailboxVersion != null && this.SourceMailboxWrapper.MailboxVersion.Value < Server.E15MinVersion)) && !this.IsOlcSync)
				{
					serverHealthStatus = this.SourceMailbox.CheckServerHealth();
				}
			}
			else
			{
				this.SourceMailboxWrapper.Ping();
			}
			return serverHealthStatus;
		}

		public virtual void ReportBadItems(List<BadMessageRec> badItems)
		{
			if (badItems != null && badItems.Count > 0)
			{
				this.DetermineWellKnownFolders(badItems);
				this.MRSJob.ReportBadItems(this, badItems);
				this.MRSJob.CheckBadItemCount(false);
			}
		}

		public void ReportSourceMailboxSize()
		{
			MailboxInformation mailboxInformation = this.SourceMailbox.GetMailboxInformation();
			if (mailboxInformation != null)
			{
				MailboxSizeRec mailboxSizeRec = new MailboxSizeRec(mailboxInformation);
				if ((this.Flags & MailboxCopierFlags.SourceIsArchive) == MailboxCopierFlags.None)
				{
					this.Report.Append(MrsStrings.ReportMailboxInfoBeforeMoveLoc(mailboxInformation.GetItemCountsAndSizesString()), mailboxSizeRec, ReportEntryFlags.Primary | ReportEntryFlags.Source);
					return;
				}
				this.Report.Append(MrsStrings.ReportMailboxArchiveInfoBeforeMoveLoc(mailboxInformation.GetItemCountsAndSizesString()), mailboxSizeRec, ReportEntryFlags.Archive | ReportEntryFlags.Source);
			}
		}

		public void ReportTargetMailboxSize()
		{
			MailboxInformation mailboxInformation = this.DestMailbox.GetMailboxInformation();
			if (mailboxInformation != null)
			{
				MailboxSizeRec mailboxSizeRec = new MailboxSizeRec(mailboxInformation);
				if ((this.Flags & MailboxCopierFlags.TargetIsArchive) == MailboxCopierFlags.None)
				{
					this.MRSJob.Report.Append(MrsStrings.ReportMailboxInfoAfterMoveLoc(mailboxInformation.GetItemCountsAndSizesString()), mailboxSizeRec, ReportEntryFlags.Primary | ReportEntryFlags.Target);
				}
				else
				{
					this.MRSJob.Report.Append(MrsStrings.ReportMailboxArchiveInfoAfterMoveLoc(mailboxInformation.GetItemCountsAndSizesString()), mailboxSizeRec, ReportEntryFlags.Archive | ReportEntryFlags.Target);
				}
				int num = 0;
				int num2 = 0;
				ulong num3 = 0UL;
				ulong num4 = 0UL;
				foreach (BadItemMarker badItemMarker in this.SyncState.BadItems.Values)
				{
					if (badItemMarker.Kind == BadItemKind.LargeItem)
					{
						num2++;
						num4 += (ulong)((long)(badItemMarker.MessageSize ?? 0));
					}
					else
					{
						num++;
						num3 += (ulong)((long)(badItemMarker.MessageSize ?? 0));
					}
				}
				if (num > 0)
				{
					this.MRSJob.Report.Append(MrsStrings.ReportCorruptItemsSkipped(num, new ByteQuantifiedSize(num3).ToString()));
				}
				if (num2 > 0)
				{
					this.MRSJob.Report.Append(MrsStrings.ReportLargeItemsSkipped(num2, new ByteQuantifiedSize(num4).ToString()));
				}
			}
		}

		public virtual bool ShouldReportEntry(ReportEntryKind reportEntryKind)
		{
			return reportEntryKind != ReportEntryKind.AggregatedSoftDeletedMessages;
		}

		public void CopyChangedFoldersData()
		{
			this.GetSourceFolderHierarchy().ResetFolderHierarchyEnumerator();
			IEnumerator<FolderRecWrapper> sourceHierarchyEnumeratorForChangedFolders = this.GetSourceHierarchyEnumeratorForChangedFolders();
			ExDateTime exDateTime = ExDateTime.UtcNow + MailboxCopierBase.CopyFolderPropertyReportingFrequency;
			ulong num = 0UL;
			while (sourceHierarchyEnumeratorForChangedFolders.MoveNext())
			{
				num += 1UL;
				ExDateTime utcNow = ExDateTime.UtcNow;
				if (utcNow > exDateTime)
				{
					this.MRSJob.Report.Append(MrsStrings.ReportCopyFolderPropertyProgress(num - 1UL));
					exDateTime += MailboxCopierBase.CopyFolderPropertyReportingFrequency;
				}
				FolderRecWrapper folderRec = sourceHierarchyEnumeratorForChangedFolders.Current;
				ExecutionContext.Create(new DataContext[]
				{
					new FolderRecWrapperDataContext(folderRec)
				}).Execute(delegate
				{
					if (!this.ShouldCopyFolderProperties(folderRec))
					{
						MrsTracer.Service.Debug("Ignoring folder '{0}' since its contents do not reside in the mailbox '{1}'", new object[]
						{
							folderRec.FullFolderName,
							this.TargetMailboxGuid
						});
						return;
					}
					byte[] entryId = folderRec.EntryId;
					FolderStateSnapshot folderStateSnapshot = this.ICSSyncState[entryId];
					if (!folderStateSnapshot.IsFolderDataChanged(folderRec.FolderRec))
					{
						MrsTracer.Service.Debug("CopyChangedFoldersData: Skipping unchanged folder '{0}'.", new object[]
						{
							folderRec.FullFolderName
						});
						return;
					}
					if (this.Flags.HasFlag(MailboxCopierFlags.Merge))
					{
						FolderMapping folderMapping = folderRec as FolderMapping;
						if (folderMapping != null && !folderMapping.IsIncluded)
						{
							MrsTracer.Service.Debug("Changed source folder '{0}' is excluded, skipping.", new object[]
							{
								folderMapping.FullFolderName
							});
							return;
						}
					}
					if (folderStateSnapshot.State.HasFlag(FolderState.PropertiesNotCopied) && folderStateSnapshot.CopyPropertiesTimestamp < folderStateSnapshot.LocalCommitTimeMax)
					{
						this.MRSJob.Report.AppendDebug("Properties weren't copied over in an earlier pass but content has since copied over. Clearing PropertiesNotCopied flag");
						folderStateSnapshot.State &= ~FolderState.PropertiesNotCopied;
					}
					bool flag = false;
					using (ISourceFolder folder = this.SourceMailbox.GetFolder(entryId))
					{
						if (folder == null)
						{
							MrsTracer.Service.Debug("Source folder '{0}' disappeared.", new object[]
							{
								folderRec
							});
							return;
						}
						byte[] destinationFolderEntryId = this.GetDestinationFolderEntryId(entryId);
						if (destinationFolderEntryId == null)
						{
							MrsTracer.Service.Debug("Folder does not map to destination: '{0}'.", new object[]
							{
								folderRec
							});
							return;
						}
						using (IDestinationFolder folder2 = this.DestMailbox.GetFolder(destinationFolderEntryId))
						{
							if (folder2 == null)
							{
								MrsTracer.Service.Debug("Destination folder '{0}' disappeared.", new object[]
								{
									folderRec
								});
								if (!this.Flags.HasFlag(MailboxCopierFlags.Merge) && !this.Flags.HasFlag(MailboxCopierFlags.PublicFolderMigration) && this.SyncState != null && this.SyncState.SyncStage == SyncStage.FinalIncrementalSync)
								{
									throw new FolderIsMissingPermanentException(folderRec.FullFolderName);
								}
								return;
							}
							else
							{
								this.CopyFolderRules(folderRec, folder, folder2);
								this.CopyFolderAcl(folderRec, folder, folder2);
								if (!this.Flags.HasFlag(MailboxCopierFlags.Merge) && !folderStateSnapshot.State.HasFlag(FolderState.PropertiesNotCopied))
								{
									this.CopyFolderProperties(folderRec, folder, folder2, FolderRecDataFlags.None, out flag);
								}
							}
						}
					}
					if (!flag && !folderStateSnapshot.State.HasFlag(FolderState.PropertiesNotCopied))
					{
						folderStateSnapshot.UpdateFolderDataCopied(folderRec.FolderRec);
					}
					else
					{
						folderStateSnapshot.State |= FolderState.PropertiesNotCopied;
					}
					MrsTracer.Service.Debug("CopyChangedFoldersData: Copied folder '{0}'.", new object[]
					{
						folderRec.FullFolderName
					});
					if (this.MRSJob != null && this.MRSJob.TestIntegration.InjectTransientExceptionAfterFolderDataCopy && folderRec.FolderName == "FolderToInjectTransientException")
					{
						this.SaveICSSyncState(true);
						throw new CommunicationErrorTransientException("Test hook exception", new LocalizedString("Exception due to test hook after copying folder data"));
					}
				});
				this.SaveICSSyncState(false);
			}
			this.GetSourceFolderHierarchy().ResetFolderHierarchyEnumerator();
		}

		public virtual IEnumerator<FolderRecWrapper> GetSourceHierarchyEnumeratorForChangedFolders()
		{
			return this.GetSourceFolderHierarchy().GetFolderHierarchyEnumerator(EnumHierarchyFlags.AllFolders);
		}

		public virtual SyncContext CreateSyncContext()
		{
			return new SyncContext(this.GetSourceFolderMap(GetFolderMapFlags.ForceRefresh), this.GetDestinationFolderMap(GetFolderMapFlags.ForceRefresh));
		}

		public virtual IDestinationMailbox GetDestinationMailbox(Guid mdbGuid, LocalMailboxFlags targetMbxFlags, IEnumerable<MRSProxyCapabilities> mrsProxyCaps)
		{
			RequestStatisticsBase cachedRequestJob = this.MRSJob.CachedRequestJob;
			string serverName;
			string remoteOrgName;
			NetworkCredential remoteCred;
			LocalMailboxFlags localMailboxFlags;
			MailboxCopierBase.ProviderType mailboxHelper = this.GetMailboxHelper(new ADObjectId(mdbGuid, PartitionId.LocalForest.ForestFQDN), targetMbxFlags, true, out serverName, out remoteOrgName, out remoteCred, out localMailboxFlags);
			targetMbxFlags |= localMailboxFlags;
			switch (mailboxHelper)
			{
			case MailboxCopierBase.ProviderType.MAPI:
				return new MapiDestinationMailbox(targetMbxFlags);
			case MailboxCopierBase.ProviderType.Storage:
				return new StorageDestinationMailbox(targetMbxFlags);
			case MailboxCopierBase.ProviderType.TcpRemote:
			case MailboxCopierBase.ProviderType.HttpsRemote:
				return new RemoteDestinationMailbox(serverName, remoteOrgName, remoteCred, this.MRSJob.CachedRequestJob.GetProxyControlFlags() | MailboxCopierBase.DefaultProxyControlFlags, mrsProxyCaps, mailboxHelper == MailboxCopierBase.ProviderType.HttpsRemote, targetMbxFlags);
			case MailboxCopierBase.ProviderType.PST:
			{
				if (cachedRequestJob.RemoteHostName == null && !this.MRSJob.TestIntegration.UseRemoteForDestination)
				{
					return new PstDestinationMailbox();
				}
				string text = cachedRequestJob.RemoteHostName;
				NetworkCredential remoteCred2 = cachedRequestJob.RemoteCredential;
				bool useHttps = true;
				if (this.MRSJob.TestIntegration.UseRemoteForDestination && string.IsNullOrEmpty(text))
				{
					text = CommonUtils.MapDatabaseToProxyServer(this.MRSJob.CachedRequestJob.SourceMDBGuid).Fqdn;
					remoteCred2 = null;
					useHttps = false;
				}
				return new RemoteDestinationMailbox(text, null, remoteCred2, this.MRSJob.CachedRequestJob.GetProxyControlFlags() | MailboxCopierBase.DefaultProxyControlFlags, new MRSProxyCapabilities[]
				{
					MRSProxyCapabilities.RemotePstExport
				}, useHttps, LocalMailboxFlags.PstExport);
			}
			default:
				return null;
			}
		}

		public virtual ISourceMailbox GetSourceMailbox(ADObjectId database, LocalMailboxFlags sourceMbxFlags, IEnumerable<MRSProxyCapabilities> mrsProxyCaps)
		{
			RequestStatisticsBase cachedRequestJob = this.MRSJob.CachedRequestJob;
			string serverName;
			string remoteOrgName;
			NetworkCredential remoteCred;
			LocalMailboxFlags localMailboxFlags;
			MailboxCopierBase.ProviderType mailboxHelper = this.GetMailboxHelper(database, sourceMbxFlags, false, out serverName, out remoteOrgName, out remoteCred, out localMailboxFlags);
			sourceMbxFlags |= localMailboxFlags;
			switch (mailboxHelper)
			{
			case MailboxCopierBase.ProviderType.MAPI:
				return new MapiSourceMailbox(sourceMbxFlags);
			case MailboxCopierBase.ProviderType.Storage:
				return new StorageSourceMailbox(sourceMbxFlags);
			case MailboxCopierBase.ProviderType.TcpRemote:
			case MailboxCopierBase.ProviderType.HttpsRemote:
				return new RemoteSourceMailbox(serverName, remoteOrgName, remoteCred, this.MRSJob.CachedRequestJob.GetProxyControlFlags() | MailboxCopierBase.DefaultProxyControlFlags, mrsProxyCaps, mailboxHelper == MailboxCopierBase.ProviderType.HttpsRemote, sourceMbxFlags);
			case MailboxCopierBase.ProviderType.PST:
			{
				if (cachedRequestJob.RemoteHostName == null && !this.MRSJob.TestIntegration.UseRemoteForSource)
				{
					return new PstSourceMailbox();
				}
				string text = cachedRequestJob.RemoteHostName;
				NetworkCredential remoteCred2 = cachedRequestJob.RemoteCredential;
				bool useHttps = true;
				if (this.MRSJob.TestIntegration.UseRemoteForSource && string.IsNullOrEmpty(text))
				{
					text = CommonUtils.MapDatabaseToProxyServer(new ADObjectId(cachedRequestJob.TargetMDBGuid, PartitionId.LocalForest.ForestFQDN)).Fqdn;
					remoteCred2 = null;
					useHttps = false;
				}
				MRSProxyCapabilities mrsproxyCapabilities = (cachedRequestJob.ContentCodePage != null) ? MRSProxyCapabilities.ConfigPst : MRSProxyCapabilities.Pst;
				return new RemoteSourceMailbox(text, null, remoteCred2, this.MRSJob.CachedRequestJob.GetProxyControlFlags() | MailboxCopierBase.DefaultProxyControlFlags, new MRSProxyCapabilities[]
				{
					mrsproxyCapabilities
				}, useHttps, LocalMailboxFlags.PstImport);
			}
			case MailboxCopierBase.ProviderType.IMAP:
			{
				ConnectionParameters connectionParameters = new ConnectionParameters(new UniquelyNamedObject(), new NullLog(), long.MaxValue, ImapMailbox.ImapTimeout);
				AuthenticationMethod authenticationMethod = cachedRequestJob.AuthenticationMethod ?? AuthenticationMethod.Basic;
				ImapAuthenticationMechanism imapAuthenticationMechanism = (authenticationMethod == AuthenticationMethod.Ntlm) ? ImapAuthenticationMechanism.Ntlm : ImapAuthenticationMechanism.Basic;
				ImapSecurityMechanism securityMechanism = (ImapSecurityMechanism)cachedRequestJob.SecurityMechanism;
				ImapAuthenticationParameters authenticationParameters = new ImapAuthenticationParameters(cachedRequestJob.RemoteCredential, imapAuthenticationMechanism, securityMechanism);
				ImapServerParameters serverParameters = new ImapServerParameters(cachedRequestJob.RemoteHostName, cachedRequestJob.RemoteHostPort);
				SmtpServerParameters smtpParameters = new SmtpServerParameters(cachedRequestJob.SmtpServerName, cachedRequestJob.SmtpServerPort);
				return new ImapSourceMailbox(connectionParameters, authenticationParameters, serverParameters, smtpParameters);
			}
			case MailboxCopierBase.ProviderType.EAS:
				if (!this.MRSJob.TestIntegration.UseRemoteForSource)
				{
					return new EasSourceMailbox();
				}
				return new RemoteSourceMailbox(CommonUtils.MapDatabaseToProxyServer(cachedRequestJob.TargetMDBGuid).Fqdn, null, null, this.MRSJob.CachedRequestJob.GetProxyControlFlags() | MailboxCopierBase.DefaultProxyControlFlags, new MRSProxyCapabilities[]
				{
					MRSProxyCapabilities.Eas
				}, false, LocalMailboxFlags.EasSync);
			case MailboxCopierBase.ProviderType.POP:
			{
				ConnectionParameters connectionParameters2 = new ConnectionParameters(new UniquelyNamedObject(), new NullLog(), long.MaxValue, (int)Pop3Constants.PopConnectionTimeout.TotalMilliseconds);
				AuthenticationMethod authenticationMethod2 = cachedRequestJob.AuthenticationMethod ?? AuthenticationMethod.Basic;
				Pop3AuthenticationMechanism pop3AuthenticationMechanism = (authenticationMethod2 == AuthenticationMethod.Basic) ? Pop3AuthenticationMechanism.Basic : Pop3AuthenticationMechanism.Spa;
				Pop3SecurityMechanism securityMechanism2 = (Pop3SecurityMechanism)cachedRequestJob.SecurityMechanism;
				Pop3AuthenticationParameters authenticationParameters2 = new Pop3AuthenticationParameters(cachedRequestJob.RemoteCredential, pop3AuthenticationMechanism, securityMechanism2);
				Pop3ServerParameters serverParameters2 = new Pop3ServerParameters(cachedRequestJob.RemoteHostName, cachedRequestJob.RemoteHostPort);
				SmtpServerParameters smtpParameters2 = new SmtpServerParameters(cachedRequestJob.SmtpServerName, cachedRequestJob.SmtpServerPort);
				return new PopSourceMailbox(connectionParameters2, authenticationParameters2, serverParameters2, smtpParameters2);
			}
			default:
				return null;
			}
		}

		public MailboxChangesManifest EnumerateHierarchyChanges(SyncContext syncContext)
		{
			MrsTracer.Service.Debug("Enumerating source hierarchy changes.", new object[0]);
			MailboxChangesManifest changes = this.SourceMailbox.EnumerateHierarchyChanges(EnumerateHierarchyChangesFlags.None, 0);
			syncContext.SourceFolderMap.EnumerateFolderHierarchy(EnumHierarchyFlags.SearchFolders, delegate(FolderRecWrapper srcFolder, FolderMap.EnumFolderContext context)
			{
				if ((this.Flags & MailboxCopierFlags.Merge) != MailboxCopierFlags.None)
				{
					FolderMapping folderMapping = srcFolder as FolderMapping;
					if (folderMapping != null && !folderMapping.IsIncluded)
					{
						return;
					}
				}
				FolderRecWrapper targetFolderBySourceId = syncContext.GetTargetFolderBySourceId(srcFolder.EntryId);
				if (targetFolderBySourceId == null || srcFolder.FolderRec.LastModifyTimestamp > targetFolderBySourceId.FolderRec.LastModifyTimestamp)
				{
					changes.ChangedFolders.Add(srcFolder.EntryId);
				}
			});
			syncContext.TargetFolderMap.EnumerateFolderHierarchy(EnumHierarchyFlags.SearchFolders, delegate(FolderRecWrapper destFolder, FolderMap.EnumFolderContext context)
			{
				byte[] sourceEntryIdFromTargetFolder = syncContext.GetSourceEntryIdFromTargetFolder(destFolder);
				if (sourceEntryIdFromTargetFolder != null && syncContext.SourceFolderMap[sourceEntryIdFromTargetFolder] == null)
				{
					changes.DeletedFolders.Add(sourceEntryIdFromTargetFolder);
				}
			});
			List<byte[]> list = new List<byte[]>(changes.ChangedFolders.Count);
			List<byte[]> list2 = new List<byte[]>(changes.DeletedFolders.Count);
			EntryIdMap<byte[]> entryIdMap = new EntryIdMap<byte[]>();
			foreach (byte[] array in changes.DeletedFolders)
			{
				if (!entryIdMap.ContainsKey(array))
				{
					entryIdMap.Add(array, array);
					list2.Add(array);
				}
			}
			foreach (byte[] array2 in changes.ChangedFolders)
			{
				if (!entryIdMap.ContainsKey(array2))
				{
					list.Add(array2);
				}
			}
			changes.ChangedFolders = list;
			changes.DeletedFolders = list2;
			return changes;
		}

		public IEnumerable<MailboxChanges> EnumerateContentChanges(SyncContext syncContext, MailboxChangesManifest hierarchyChanges)
		{
			EntryIdMap<byte[]> deletedSourceIDs = new EntryIdMap<byte[]>();
			EntryIdMap<byte[]> enumeratedSourceIDs = new EntryIdMap<byte[]>();
			EntryIdMap<FolderChangesManifest> contentChanges = new EntryIdMap<FolderChangesManifest>();
			foreach (byte[] array in hierarchyChanges.DeletedFolders)
			{
				deletedSourceIDs[array] = array;
			}
			syncContext.TargetFolderMap.ResetFolderHierarchyEnumerator();
			IEnumerator<FolderRecWrapper> folderHierarchyEnumerator = syncContext.TargetFolderMap.GetFolderHierarchyEnumerator(EnumHierarchyFlags.NormalFolders | EnumHierarchyFlags.RootFolder);
			while (folderHierarchyEnumerator.MoveNext())
			{
				FolderRecWrapper destFolder = folderHierarchyEnumerator.Current;
				if (!this.IsContentAvailableInTargetMailbox(destFolder))
				{
					MrsTracer.Service.Debug("Ignoring folder '{0}' since it's contents do not reside in this mailbox", new object[]
					{
						destFolder.FullFolderName
					});
				}
				else
				{
					byte[] sourceFolderId = syncContext.GetSourceEntryIdFromTargetFolder(destFolder);
					if (sourceFolderId != null && !deletedSourceIDs.ContainsKey(sourceFolderId))
					{
						enumeratedSourceIDs[sourceFolderId] = sourceFolderId;
						FolderRecWrapper srcFolderRec = syncContext.SourceFolderMap[sourceFolderId];
						if (srcFolderRec == null)
						{
							MrsTracer.Service.Debug("Folder {0} is not present in source, not syncing", new object[]
							{
								TraceUtils.DumpEntryId(sourceFolderId)
							});
						}
						else if (srcFolderRec.FolderRec.IsGhosted)
						{
							MrsTracer.Service.Debug("Source folder '{0}' is Ghosted, skipping", new object[]
							{
								srcFolderRec.FullFolderName
							});
						}
						else
						{
							foreach (MailboxChanges mailboxChanges in this.EnumerateFolderContentChanges(srcFolderRec, contentChanges))
							{
								yield return mailboxChanges;
							}
						}
					}
				}
			}
			syncContext.TargetFolderMap.ResetFolderHierarchyEnumerator();
			foreach (byte[] sourceFolderId2 in hierarchyChanges.ChangedFolders)
			{
				if (!enumeratedSourceIDs.ContainsKey(sourceFolderId2))
				{
					FolderRecWrapper srcFolderRec2 = syncContext.SourceFolderMap[sourceFolderId2];
					if (srcFolderRec2 == null)
					{
						MrsTracer.Service.Debug("Folder {0} is not present in source, not syncing", new object[]
						{
							TraceUtils.DumpEntryId(sourceFolderId2)
						});
					}
					else if (srcFolderRec2.FolderRec.IsGhosted)
					{
						MrsTracer.Service.Debug("Source folder '{0}' is Ghosted, skipping", new object[]
						{
							srcFolderRec2.FullFolderName
						});
					}
					else if (!this.HasSourceFolderContents(srcFolderRec2))
					{
						MrsTracer.Service.Debug("Ignoring folder '{0}' since its contents do not reside in this mailbox", new object[]
						{
							srcFolderRec2.FullFolderName
						});
					}
					else
					{
						foreach (MailboxChanges mailboxChanges2 in this.EnumerateFolderContentChanges(srcFolderRec2, contentChanges))
						{
							yield return mailboxChanges2;
						}
					}
				}
			}
			MailboxChanges lastMailboxChanges = new MailboxChanges(contentChanges);
			if (lastMailboxChanges.HasChanges)
			{
				yield return lastMailboxChanges;
			}
			yield break;
		}

		public void AddResources(HashSet<ResourceKey> addTo)
		{
			this.AddResources(this.SourceMdbGuid, addTo, ResourceDirection.Source);
			this.AddResources(this.DestMdbGuid, addTo, ResourceDirection.Target);
		}

		public void SaveICSSyncState(bool force)
		{
			this.DestMailboxWrapper.SaveICSSyncState(force);
			this.UpdateTimestampWhenPersistentProgressWasMade();
		}

		public void UpdateTimestampWhenPersistentProgressWasMade()
		{
			MrsTracer.Service.Debug("Job {0} {1} made progress", new object[]
			{
				this.MRSJob.RequestJobGuid,
				this.MRSJob.RequestJobIdentity
			});
			this.TimestampWhenPersistentProgressWasMade = DateTime.UtcNow;
		}

		public int GetBadItemsCountForCounter(BadItemCounter counter)
		{
			int num = 0;
			if (this.SyncState != null)
			{
				foreach (BadItemMarker badItemMarker in this.SyncState.BadItems.Values)
				{
					switch (badItemMarker.Kind)
					{
					case BadItemKind.CorruptItem:
					case BadItemKind.CorruptSearchFolderCriteria:
					case BadItemKind.CorruptFolderACL:
					case BadItemKind.CorruptFolderRule:
					case BadItemKind.CorruptFolderProperty:
					case BadItemKind.CorruptInferenceProperties:
					case BadItemKind.CorruptMailboxSetting:
					case BadItemKind.FolderPropertyMismatch:
						num += (counter.Count(badItemMarker.Category) ? 1 : 0);
						break;
					}
				}
			}
			return num;
		}

		public int GetMissingItemsCount(BadItemCounter counter)
		{
			int num = 0;
			if (this.SyncState != null)
			{
				foreach (BadItemMarker badItemMarker in this.SyncState.BadItems.Values)
				{
					BadItemKind kind = badItemMarker.Kind;
					if (kind != BadItemKind.MissingItem)
					{
						switch (kind)
						{
						case BadItemKind.MissingFolder:
						case BadItemKind.MisplacedFolder:
							break;
						default:
							continue;
						}
					}
					num += (counter.Count(badItemMarker.Category) ? 1 : 0);
				}
			}
			return num;
		}

		public int GetLargeItemsCount(BadItemCounter counter)
		{
			int num = 0;
			if (this.SyncState != null)
			{
				foreach (BadItemMarker badItemMarker in this.SyncState.BadItems.Values)
				{
					BadItemKind kind = badItemMarker.Kind;
					if (kind == BadItemKind.LargeItem)
					{
						num += (counter.Count(badItemMarker.Category) ? 1 : 0);
					}
				}
			}
			return num;
		}

		public bool HasMissingItems()
		{
			BadItemCounter counter = new BadItemCounter(this.MRSJob.CachedRequestJob.SkipKnownCorruptions);
			return this.SyncState != null && this.GetMissingItemsCount(counter) > 0;
		}

		public void StartIsInteg()
		{
			if (this.IsIntegRequestGuid == Guid.Empty)
			{
				this.IsIntegRequestGuid = this.SourceMailbox.StartIsInteg(IsInteg.CorruptionTypesToFix.ToList<uint>());
				this.Report.Append(MrsStrings.ReportStartedIsInteg(this.SourceMailboxGuid, this.IsIntegRequestGuid));
			}
		}

		public bool QueryIsInteg()
		{
			List<StoreIntegrityCheckJob> list = this.SourceMailbox.QueryIsInteg(this.IsIntegRequestGuid);
			bool isIntegComplete = true;
			if (list == null || list.Count == 0)
			{
				throw new StoreIntegFailedTransientException(999);
			}
			string percentages = string.Empty;
			list.ForEach(delegate(StoreIntegrityCheckJob job)
			{
				if (job.JobState == 4)
				{
					throw new StoreIntegFailedTransientException(job.ErrorCode.Value);
				}
				if (job.JobState != 3)
				{
					isIntegComplete = false;
					MrsTracer.Service.Debug("Waiting Store IsInteg task for Mailbox {0}: {1}", new object[]
					{
						this.SourceMailboxGuid,
						job.ToString()
					});
				}
				percentages = percentages + job.Progress + " ";
			});
			if (isIntegComplete)
			{
				this.Report.Append(MrsStrings.ReportCompletedIsInteg(this.SourceMailboxGuid, this.IsIntegRequestGuid));
				this.IsIntegRequestGuid = Guid.Empty;
				this.IsIntegDone = true;
			}
			else
			{
				this.Report.Append(MrsStrings.ReportWaitingIsInteg(this.SourceMailboxGuid, this.IsIntegRequestGuid, percentages));
			}
			return isIntegComplete;
		}

		protected IFxProxy CreateFxProxyTransmissionPipeline(IFxProxy destinationProxy)
		{
			FxProxyReceiver destination = new FxProxyReceiver(destinationProxy, false);
			ProgressTrackerTransmitter destination2 = new ProgressTrackerTransmitter(destination, this.MRSJob);
			return new FxProxyTransmitter(destination2, true);
		}

		protected IFxProxyPool CreateFxProxyPoolTransmissionPipeline(IFxProxyPool destinationProxy)
		{
			FxProxyPoolReceiver destination = new FxProxyPoolReceiver(destinationProxy, true);
			ProgressTrackerTransmitter destination2 = new ProgressTrackerTransmitter(destination, this.MRSJob);
			return new FxProxyPoolTransmitter(destination2, true, this.DestMailbox.GetVersion());
		}

		public virtual void ApplyContentsChanges(SyncContext ctx, MailboxChanges changes)
		{
			CopyMessagesCount right = this.CopyMessageBatch(null, changes);
			ctx.CopyMessagesCount += right;
			this.ReportContentChangesSynced(ctx);
			this.ICSSyncState.ProviderState = this.SourceMailbox.GetMailboxSyncState();
			this.SaveICSSyncState(false);
		}

		[Conditional("DEBUG")]
		public void ValidateContentChanges(MailboxChanges changes)
		{
			bool isIncrementalSyncPaged = this.IsIncrementalSyncPaged;
		}

		protected void ReportContentChangesSynced(SyncContext syncContext)
		{
			if (!this.Flags.HasFlag(MailboxCopierFlags.Merge) && !this.Flags.HasFlag(MailboxCopierFlags.Imap) && !this.Flags.HasFlag(MailboxCopierFlags.Eas) && !this.Flags.HasFlag(MailboxCopierFlags.Pop))
			{
				this.Flags.HasFlag(MailboxCopierFlags.Olc);
			}
			this.Report.Append(MrsStrings.ReportIncrementalSyncContentChangesSynced2(this.TargetTracingID, syncContext.CopyMessagesCount.NewMessages, syncContext.CopyMessagesCount.Changed, syncContext.CopyMessagesCount.Deleted, syncContext.CopyMessagesCount.Read, syncContext.CopyMessagesCount.Unread, syncContext.CopyMessagesCount.Skipped, syncContext.CopyMessagesCount.TotalContentCopied));
		}

		private void AddResources(Guid mdbGuid, HashSet<ResourceKey> addTo, ResourceDirection direction)
		{
			if (mdbGuid == Guid.Empty)
			{
				return;
			}
			Guid a = Guid.Empty;
			if (direction == ResourceDirection.Source && this.SourceServerInfo != null)
			{
				a = this.SourceServerInfo.MailboxServerGuid;
			}
			if (direction == ResourceDirection.Target && this.TargetServerInfo != null)
			{
				a = this.TargetServerInfo.MailboxServerGuid;
			}
			if (a != CommonUtils.LocalServerGuid)
			{
				return;
			}
			DatabaseResource instance;
			if (direction == ResourceDirection.Source)
			{
				instance = DatabaseReadResource.Cache.GetInstance(mdbGuid, WorkloadType.MailboxReplicationService);
			}
			else
			{
				instance = DatabaseWriteResource.Cache.GetInstance(mdbGuid, WorkloadType.MailboxReplicationService);
			}
			if (instance != null)
			{
				foreach (WlmResourceHealthMonitor wlmResourceHealthMonitor in instance.GetWlmResources())
				{
					addTo.Add(wlmResourceHealthMonitor.WlmResourceKey);
				}
			}
		}

		private void CopySearchFolderCriteria(FolderRecWrapper folderRec, IDestinationFolder destFolder)
		{
			if (folderRec.SearchFolderRestriction == null || folderRec.SearchFolderScopeIDs == null || folderRec.SearchFolderScopeIDs.Length == 0)
			{
				return;
			}
			if (folderRec.IsSpoolerQueue)
			{
				return;
			}
			SearchCriteriaFlags flags = SearchCriteriaFlags.None;
			if ((folderRec.SearchFolderState & SearchState.TwirTotally) != SearchState.None)
			{
				flags |= SearchCriteriaFlags.NonContentIndexed;
			}
			if ((folderRec.SearchFolderState & SearchState.Recursive) != SearchState.None)
			{
				flags |= SearchCriteriaFlags.Recursive;
			}
			if ((folderRec.SearchFolderState & SearchState.Foreground) != SearchState.None)
			{
				flags |= SearchCriteriaFlags.Foreground;
			}
			flags |= SearchCriteriaFlags.FailOnForeignEID;
			CommonUtils.ProcessKnownExceptions(delegate
			{
				destFolder.SetSearchCriteria(folderRec.SearchFolderRestriction, folderRec.SearchFolderScopeIDs, flags);
			}, delegate(Exception failure)
			{
				if (CommonUtils.ExceptionIsAny(failure, new WellKnownException[]
				{
					WellKnownException.MRSPermanent,
					WellKnownException.DataProviderPermanent,
					WellKnownException.CorruptData
				}))
				{
					MrsTracer.Service.Warning("Failed to update search criteria on folder \"{0}\", ignoring. Error {1}", new object[]
					{
						folderRec.FullFolderName,
						failure.ToString()
					});
					LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
					string dataContext = ExecutionContext.GetDataContext(failure);
					MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_MoveUnableToApplySearchCriteria, new object[]
					{
						this.TargetTracingID,
						folderRec.FullFolderName,
						localizedString,
						dataContext
					});
					List<BadMessageRec> list = new List<BadMessageRec>(1);
					list.Add(BadMessageRec.Folder(folderRec.FolderRec, BadItemKind.CorruptSearchFolderCriteria, failure as LocalizedException));
					this.ReportBadItems(list);
					return true;
				}
				return false;
			});
		}

		private IEnumerable<MailboxChanges> EnumerateFolderContentChanges(FolderRecWrapper srcFolderRec, EntryIdMap<FolderChangesManifest> contentChanges)
		{
			IEnumerable<FolderChangesManifest> folderChangesManifests = null;
			ExecutionContext.Create(new DataContext[]
			{
				new FolderRecWrapperDataContext(srcFolderRec)
			}).Execute(delegate
			{
				folderChangesManifests = this.EnumerateChanges(srcFolderRec, contentChanges);
			});
			foreach (FolderChangesManifest folderChanges in folderChangesManifests)
			{
				if (folderChanges != null && folderChanges.HasChanges)
				{
					contentChanges[srcFolderRec.EntryId] = folderChanges;
					this.Report.AppendDebug(string.Format("{0}: {1}", srcFolderRec.FullFolderName, folderChanges));
					if (this.IsPageFull(srcFolderRec, folderChanges, contentChanges))
					{
						yield return new MailboxChanges(contentChanges);
						contentChanges.Clear();
					}
				}
			}
			yield break;
		}

		private bool IsPageFull(FolderRecWrapper srcFolderRec, FolderChangesManifest folderChanges, EntryIdMap<FolderChangesManifest> contentChanges)
		{
			if (!this.SourceSupportsPagedEnumerateChanges)
			{
				bool hasMoreChanges = folderChanges.HasMoreChanges;
			}
			if (!this.IsIncrementalSyncPaged)
			{
				bool hasMoreChanges2 = folderChanges.HasMoreChanges;
			}
			return this.IsIncrementalSyncPaged && folderChanges.HasMoreChanges;
		}

		private IEnumerable<FolderChangesManifest> EnumerateChanges(FolderRecWrapper srcFolderRec, EntryIdMap<FolderChangesManifest> contentChanges)
		{
			byte[] sourceFolderId = srcFolderRec.EntryId;
			if (srcFolderRec.FolderType != FolderType.Search)
			{
				if (this.Flags.HasFlag(MailboxCopierFlags.Merge))
				{
					FolderMapping folderMapping = (FolderMapping)srcFolderRec;
					if (!folderMapping.IsIncluded)
					{
						MrsTracer.Service.Debug("Source folder '{0}' is excluded, skipping.", new object[]
						{
							folderMapping.FullFolderName
						});
						goto IL_391;
					}
				}
				FolderStateSnapshot folderSnapshot = this.ICSSyncState[sourceFolderId];
				bool checkIfGhostedFolderContentHasChanged = false;
				ContentChangeResult contentChangeResult = folderSnapshot.VerifyContentsChanged(srcFolderRec.FolderRec);
				if (contentChangeResult != ContentChangeResult.Changed)
				{
					if (!this.IsPublicFolderMigration || contentChangeResult != ContentChangeResult.Ghosted)
					{
						MrsTracer.Service.Debug("Folder '{0}' appears unchanged, will not run ICS sync.", new object[]
						{
							srcFolderRec.FullFolderName
						});
						goto IL_391;
					}
					checkIfGhostedFolderContentHasChanged = true;
					MrsTracer.Service.Debug("Folder '{0}' is ghosted, will check again from the right DB", new object[]
					{
						srcFolderRec.FullFolderName
					});
				}
				MrsTracer.Service.Debug("Folder '{0}' appears changed, running ICS sync.", new object[]
				{
					srcFolderRec.FullFolderName
				});
				FolderRec frec = srcFolderRec.FolderRec;
				using (ISourceFolder srcFolder = this.SourceMailbox.GetFolder(sourceFolderId))
				{
					if (srcFolder == null)
					{
						MrsTracer.Service.Debug("Folder '{0}' disappeared from source, will sync deletion later", new object[]
						{
							srcFolderRec.FullFolderName
						});
						yield break;
					}
					if (checkIfGhostedFolderContentHasChanged)
					{
						frec = srcFolder.GetFolderRec(this.GetAdditionalFolderPtags(), GetFolderRecFlags.None);
						if (!folderSnapshot.IsFolderChanged(frec))
						{
							MrsTracer.Service.Debug("Ghosted Folder '{0}' appears unchanged, will not run ICS sync.", new object[]
							{
								srcFolderRec.FullFolderName
							});
							yield break;
						}
					}
					FolderChangesManifest folderChanges = null;
					bool firstPage = true;
					do
					{
						CommonUtils.TreatMissingFolderAsTransient(delegate
						{
							EnumerateContentChangesFlags enumerateContentChangesFlags = EnumerateContentChangesFlags.None;
							int maxChanges = 0;
							if (this.IsIncrementalSyncPaged)
							{
								maxChanges = this.MaxIncrementalChanges - CommonUtils.CountNewOrUpdatedMessages(contentChanges);
								if (firstPage)
								{
									enumerateContentChangesFlags |= EnumerateContentChangesFlags.FirstPage;
									firstPage = false;
								}
							}
							folderChanges = srcFolder.EnumerateChanges(enumerateContentChangesFlags, maxChanges);
						}, sourceFolderId, new Func<byte[], IFolder>(this.SourceMailboxWrapper.GetFolder));
						yield return folderChanges;
					}
					while (folderChanges != null && folderChanges.HasMoreChanges);
				}
				folderSnapshot.UpdateContentsCopied(frec);
			}
			IL_391:
			yield break;
		}

		public void ApplyHierarchyChanges(SyncContext ctx, MailboxChangesManifest changes)
		{
			foreach (byte[] array in changes.ChangedFolders)
			{
				MailboxCopierBase.<>c__DisplayClass60 CS$<>8__locals2 = new MailboxCopierBase.<>c__DisplayClass60();
				CS$<>8__locals2.srcFolderRec = ctx.SourceFolderMap[array];
				if ((this.Flags & MailboxCopierFlags.Merge) != MailboxCopierFlags.None && CS$<>8__locals2.srcFolderRec != null)
				{
					FolderMapping folderMapping = (FolderMapping)CS$<>8__locals2.srcFolderRec;
					if (!folderMapping.IsIncluded)
					{
						MrsTracer.Service.Debug("Changed source folder '{0}' is excluded, skipping.", new object[]
						{
							folderMapping.FullFolderName
						});
						continue;
					}
				}
				using (ISourceFolder srcFolder = this.SourceMailbox.GetFolder(array))
				{
					if (srcFolder == null)
					{
						MrsTracer.Service.Debug("Changed source folder {0} was recently deleted. Will pick up deletion on the next iteration.", new object[]
						{
							TraceUtils.DumpEntryId(array)
						});
					}
					else
					{
						FolderRec folderRec = srcFolder.GetFolderRec(this.GetAdditionalFolderPtags(), GetFolderRecFlags.None);
						if (CS$<>8__locals2.srcFolderRec == null)
						{
							CS$<>8__locals2.srcFolderRec = ctx.CreateSourceFolderRec(folderRec);
							ctx.SourceFolderMap.InsertFolder(CS$<>8__locals2.srcFolderRec);
						}
						else
						{
							ctx.SourceFolderMap.UpdateFolder(folderRec);
						}
						ExecutionContext.Create(new DataContext[]
						{
							new FolderRecWrapperDataContext(CS$<>8__locals2.srcFolderRec)
						}).Execute(delegate
						{
							this.UpdateFolderAfterHierarchyChange(ctx, CS$<>8__locals2.srcFolderRec, srcFolder);
						});
					}
				}
			}
			foreach (byte[] array2 in changes.DeletedFolders)
			{
				FolderRecWrapper deletedFolderRec = ctx.GetTargetFolderBySourceId(array2);
				if (deletedFolderRec != null)
				{
					ExecutionContext.Create(new DataContext[]
					{
						new FolderRecWrapperDataContext(deletedFolderRec)
					}).Execute(delegate
					{
						MrsTracer.Service.Debug("Removing destination folder \"{0}\".", new object[]
						{
							deletedFolderRec.FolderName
						});
						TimeSpan timeout = TimeSpan.FromSeconds(1.0);
						int num = 0;
						try
						{
							IL_36:
							this.DestMailbox.DeleteFolder(deletedFolderRec.FolderRec);
						}
						catch (Exception ex)
						{
							if (CommonUtils.ExceptionIs(ex, new WellKnownException[]
							{
								WellKnownException.MapiPartialCompletion
							}) && num < 10)
							{
								num++;
								MrsTracer.Service.Warning("Got PartialCompletion during DeleteFolder(), will retry ({0}/{1})", new object[]
								{
									num,
									10
								});
								Thread.Sleep(timeout);
								goto IL_36;
							}
							throw;
						}
						if (this.MRSJob.TestIntegration.LogContentDetails)
						{
							this.MRSJob.Report.AppendDebug(string.Format("Folder deleted: Name '{0}', FolderID {1}, ParentID {2}", deletedFolderRec.FolderName, TraceUtils.DumpEntryId(deletedFolderRec.EntryId), TraceUtils.DumpEntryId(deletedFolderRec.ParentId)));
						}
						ctx.TargetFolderMap.RemoveFolder(deletedFolderRec);
						ctx.NumberOfHierarchyUpdates++;
					});
				}
				else
				{
					MrsTracer.Service.Debug("Deleted folder {0} is already not present in dest, skipping.", new object[]
					{
						TraceUtils.DumpEntryId(array2)
					});
				}
			}
		}

		private void UpdateFolderAfterHierarchyChange(SyncContext ctx, FolderRecWrapper srcFolderRec, ISourceFolder srcFolder)
		{
			MrsTracer.Service.Function("MailboxCopier.UpdateFolderAfterHierarchyChange(\"{0}\")", new object[]
			{
				srcFolderRec.FullFolderName
			});
			FolderRecWrapper folderRecWrapper = ctx.GetTargetFolderBySourceId(srcFolderRec.EntryId);
			FolderRecWrapper targetParentFolderBySourceParentId = ctx.GetTargetParentFolderBySourceParentId(srcFolderRec.ParentId);
			if (targetParentFolderBySourceParentId == null)
			{
				MrsTracer.Service.Warning("Destination parent folder does not exist. Will pick up this change on the next iteration.", new object[0]);
				return;
			}
			if (folderRecWrapper == null || !CommonUtils.IsSameEntryId(targetParentFolderBySourceParentId.EntryId, folderRecWrapper.ParentId))
			{
				if (folderRecWrapper == null)
				{
					MrsTracer.Service.Debug("Destination folder \"{0}\" does not exist. Creating it.", new object[]
					{
						srcFolderRec.FullFolderName
					});
					if (this.ShouldCreateFolder(new FolderMap.EnumFolderContext(), srcFolderRec))
					{
						folderRecWrapper = this.CreateDestinationFolder(ctx, srcFolderRec, targetParentFolderBySourceParentId);
					}
					if (folderRecWrapper == null)
					{
						return;
					}
					ctx.TargetFolderMap.InsertFolder(folderRecWrapper);
					if (this.MRSJob.TestIntegration.LogContentDetails)
					{
						this.MRSJob.Report.AppendDebug(string.Format("Folder created: Name '{0}', FolderID {1}, ParentID {2}", folderRecWrapper.FolderName, TraceUtils.DumpEntryId(folderRecWrapper.FolderRec.EntryId), TraceUtils.DumpEntryId(folderRecWrapper.ParentId)));
					}
				}
				else
				{
					MrsTracer.Service.Debug("Destination folder \"{0}\" needs to be moved to a new parent. Moving it.", new object[]
					{
						folderRecWrapper.FullFolderName
					});
					this.DestMailbox.MoveFolder(folderRecWrapper.EntryId, folderRecWrapper.ParentId, targetParentFolderBySourceParentId.EntryId);
					byte[] parentId = folderRecWrapper.ParentId;
					FolderRec folderRec = new FolderRec(folderRecWrapper.FolderRec);
					folderRec.ParentId = targetParentFolderBySourceParentId.EntryId;
					ctx.TargetFolderMap.UpdateFolder(folderRec);
					if (this.MRSJob.TestIntegration.LogContentDetails)
					{
						this.MRSJob.Report.AppendDebug(string.Format("Folder moved: Name '{0}', FolderID {1}, OldParentID {2}, NewParentID {3}", new object[]
						{
							folderRecWrapper.FolderName,
							TraceUtils.DumpEntryId(folderRecWrapper.EntryId),
							TraceUtils.DumpEntryId(parentId),
							TraceUtils.DumpEntryId(targetParentFolderBySourceParentId.EntryId)
						}));
					}
				}
			}
			FolderRecDataFlags dataToCopy = FolderRecDataFlags.SearchCriteria;
			using (IDestinationFolder folder = this.DestMailbox.GetFolder(folderRecWrapper.EntryId))
			{
				if (folder == null)
				{
					MrsTracer.Service.Error("Something deleted destination folder from under us", new object[0]);
					throw new FolderIsMissingTransientException();
				}
				bool flag;
				this.CopyFolderProperties(srcFolderRec, srcFolder, folder, dataToCopy, out flag);
				if (flag)
				{
					FolderStateSnapshot folderStateSnapshot = this.ICSSyncState[srcFolderRec.EntryId];
					folderStateSnapshot.State |= FolderState.PropertiesNotCopied;
					this.SaveICSSyncState(false);
				}
			}
			folderRecWrapper.FolderRec.FolderName = srcFolderRec.FolderRec.FolderName;
			folderRecWrapper.FolderRec.LastModifyTimestamp = srcFolderRec.FolderRec.LastModifyTimestamp;
			ctx.NumberOfHierarchyUpdates++;
		}

		protected virtual FolderRecWrapper CreateDestinationFolder(SyncContext syncContext, FolderRecWrapper srcFolderRec, FolderRecWrapper destParentRec)
		{
			CreateFolderFlags createFolderFlags = CreateFolderFlags.FailIfExists;
			if (srcFolderRec.IsInternalAccess)
			{
				if (!this.DestMailbox.IsCapabilitySupported(MRSProxyCapabilities.InternalAccessFolderCreation))
				{
					throw new InternalAccessFolderCreationIsNotSupportedException();
				}
				createFolderFlags |= CreateFolderFlags.InternalAccess;
			}
			byte[] entryId = this.Flags.HasFlag(MailboxCopierFlags.Merge) ? null : srcFolderRec.EntryId;
			FolderRecWrapper folderRecWrapper = syncContext.CreateTargetFolderRec(srcFolderRec);
			folderRecWrapper.FolderRec.EntryId = entryId;
			folderRecWrapper.FolderRec.ParentId = destParentRec.EntryId;
			byte[] entryId2;
			this.DestMailbox.CreateFolder(folderRecWrapper.FolderRec, createFolderFlags, out entryId2);
			folderRecWrapper.FolderRec.EntryId = entryId2;
			return folderRecWrapper;
		}

		private CopyMessagesCount CopyMessageBatch(List<MessageRec> batch, MailboxChanges mailboxChanges)
		{
			int newMessages = 0;
			int changed = 0;
			int deleted = 0;
			int read = 0;
			int unread = 0;
			ExDateTime dtLastMessage = ExDateTime.UtcNow;
			MailboxUpdates mailboxUpdates = new MailboxUpdates();
			int batchSize;
			if (batch == null)
			{
				batch = new List<MessageRec>();
				if (mailboxChanges.FolderChanges != null)
				{
					foreach (FolderChangesManifest folderChangesManifest in mailboxChanges.FolderChanges.Values)
					{
						if (folderChangesManifest.ChangedMessages != null)
						{
							foreach (MessageRec messageRec in folderChangesManifest.ChangedMessages)
							{
								if (messageRec.IsDeleted)
								{
									mailboxUpdates.AddMessage(messageRec.FolderId, messageRec.EntryId, MessageUpdateType.Delete);
								}
								else
								{
									batch.Add(messageRec);
								}
							}
						}
						mailboxUpdates.AddReadUnread(folderChangesManifest.FolderId, folderChangesManifest.ReadMessages, folderChangesManifest.UnreadMessages);
					}
				}
				batchSize = this.GetConfig<int>("MinBatchSize");
			}
			else
			{
				batchSize = batch.Count;
			}
			int itemsCopied = 0;
			if (batch.Count > 0)
			{
				CommonUtils.ProcessInBatches<MessageRec>(batch.ToArray(), batchSize, delegate(MessageRec[] subBatch)
				{
					MrsTracer.Service.Debug("Copying {0} messages", new object[]
					{
						subBatch.Length
					});
					this.CheckHealthCallback();
					EntryIdMap<byte[]> folders = new EntryIdMap<byte[]>();
					foreach (MessageRec messageRec2 in subBatch)
					{
						if (messageRec2.IsNew)
						{
							newMessages++;
						}
						else
						{
							changed++;
						}
						byte[] destinationFolderEntryId = this.GetDestinationFolderEntryId(messageRec2.FolderId);
						if (!folders.ContainsKey(destinationFolderEntryId))
						{
							folders.Add(destinationFolderEntryId, null);
						}
					}
					List<BadMessageRec> list = new List<BadMessageRec>();
					MapiUtils.ExportMessagesWithBadItemDetection(this.SourceMailbox, new List<MessageRec>(subBatch), delegate
					{
						IFxProxyPool destinationFxProxyPool = this.GetDestinationFxProxyPool(folders.EntryIds);
						return this.CreateFxProxyPoolTransmissionPipeline(destinationFxProxyPool);
					}, ExportMessagesFlags.None, null, null, this.MRSJob.TestIntegration, ref list);
					MrsTracer.Service.Debug("Message copy is successful.", new object[0]);
					itemsCopied += subBatch.Length;
					if (this.MRSJob.TestIntegration.LogContentDetails)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine(string.Format("CopyMessageBatch: {0} items copied", subBatch.Length));
						foreach (MessageRec messageRec3 in subBatch)
						{
							stringBuilder.AppendLine(string.Format("ItemID {0}, FolderID {1}{2}", TraceUtils.DumpEntryId(messageRec3.EntryId), TraceUtils.DumpEntryId(messageRec3.FolderId), messageRec3.IsFAI ? ", FAI" : string.Empty));
						}
						this.MRSJob.Report.AppendDebug(stringBuilder.ToString());
					}
					if (list != null && list.Count > 0)
					{
						MrsTracer.Service.Debug("Message copy returned {0} bad/missing messages", new object[]
						{
							list.Count
						});
						List<BadMessageRec> list2 = new List<BadMessageRec>();
						foreach (BadMessageRec badMessageRec in list)
						{
							if (badMessageRec.Kind == BadItemKind.MissingItem)
							{
								mailboxUpdates.AddMessage(badMessageRec.FolderId, badMessageRec.EntryId, MessageUpdateType.Delete);
							}
							else
							{
								list2.Add(badMessageRec);
							}
						}
						if (this.MRSJob.TestIntegration.LogContentDetails && list2.Count > 0)
						{
							StringBuilder stringBuilder2 = new StringBuilder();
							stringBuilder2.AppendLine(string.Format("Bad items reported: {0} items", list2.Count));
							foreach (BadMessageRec badMessageRec2 in list2)
							{
								stringBuilder2.AppendLine(string.Format("ItemID {0}, {1}", TraceUtils.DumpEntryId(badMessageRec2.EntryId), badMessageRec2.ToString()));
							}
							this.MRSJob.Report.AppendDebug(stringBuilder2.ToString());
						}
						this.ReportBadItems(list2);
					}
					ExDateTime utcNow = ExDateTime.UtcNow;
					if (mailboxChanges != null && utcNow - dtLastMessage > BaseJob.FlushInterval)
					{
						this.MRSJob.Report.Append(MrsStrings.ReportIncrementalSyncProgress(this.TargetTracingID, itemsCopied, batch.Count));
						this.MRSJob.FlushReport(null);
						dtLastMessage = utcNow;
					}
				});
			}
			if (!mailboxUpdates.IsEmpty())
			{
				MrsTracer.Service.Debug("Applying destination updates: {0} deleted, {1} read, {2} unread", new object[]
				{
					mailboxUpdates.GetUpdateCount(MessageUpdateType.Delete),
					mailboxUpdates.GetUpdateCount(MessageUpdateType.SetRead),
					mailboxUpdates.GetUpdateCount(MessageUpdateType.SetUnread)
				});
				using (Dictionary<byte[], FolderUpdates>.ValueCollection.Enumerator enumerator3 = mailboxUpdates.FolderData.Values.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						FolderUpdates fu = enumerator3.Current;
						ExecutionContext.Create(new DataContext[]
						{
							new OperationDataContext("Applying dest folder changes", OperationType.None),
							new FolderIdDataContext(fu.FolderId)
						}).Execute(delegate
						{
							using (IDestinationFolder folder = this.DestMailbox.GetFolder(fu.FolderId))
							{
								if (folder == null)
								{
									MrsTracer.Service.Error("Destination folder {0} does not exist, unable to apply updates.", new object[]
									{
										TraceUtils.DumpEntryId(fu.FolderId)
									});
								}
								else
								{
									if (fu.DeletedMessages != null && fu.DeletedMessages.Count > 0)
									{
										folder.DeleteMessages(fu.DeletedMessages.ToArray());
										deleted += fu.DeletedMessages.Count;
										if (this.MRSJob.TestIntegration.LogContentDetails)
										{
											this.LogMessageIDs(fu.DeletedMessages, "Delete", fu.FolderId);
										}
									}
									if (fu.ReadMessages != null && fu.ReadMessages.Count > 0)
									{
										folder.SetReadFlagsOnMessages(SetReadFlags.None, fu.ReadMessages.ToArray());
										read += fu.ReadMessages.Count;
										if (this.MRSJob.TestIntegration.LogContentDetails)
										{
											this.LogMessageIDs(fu.ReadMessages, "MarkRead", fu.FolderId);
										}
									}
									if (fu.UnreadMessages != null && fu.UnreadMessages.Count > 0)
									{
										folder.SetReadFlagsOnMessages(SetReadFlags.ClearRead, fu.UnreadMessages.ToArray());
										unread += fu.UnreadMessages.Count;
										if (this.MRSJob.TestIntegration.LogContentDetails)
										{
											this.LogMessageIDs(fu.UnreadMessages, "MarkUnread", fu.FolderId);
										}
									}
								}
							}
						});
					}
				}
			}
			this.MRSJob.ProgressTracker.AddItems((uint)(newMessages + changed + deleted + read + unread));
			return new CopyMessagesCount(newMessages, changed, deleted, read, unread, 0);
		}

		private void LogMessageIDs(List<byte[]> list, string operationName, byte[] folderID)
		{
			if (list == null || list.Count == 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("{0} {1} items, FolderID {2}", operationName, list.Count, TraceUtils.DumpEntryId(folderID)));
			foreach (byte[] entryId in list)
			{
				stringBuilder.AppendLine(string.Format("ItemID {0}", TraceUtils.DumpEntryId(entryId)));
			}
			this.MRSJob.Report.AppendDebug(stringBuilder.ToString());
		}

		private void DetermineWellKnownFolders(List<BadMessageRec> badItems)
		{
			if (this.cachedSourceHierarchy == null)
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					this.GetSourceFolderHierarchy();
				}, null);
			}
			if (this.cachedSourceHierarchy == null)
			{
				MrsTracer.Service.Warning("Unable to load source hierarchy, will not classify bad item folders.", new object[0]);
				return;
			}
			foreach (BadMessageRec badMessageRec in badItems)
			{
				FolderMapping folderMapping = this.cachedSourceHierarchy[badMessageRec.FolderId] as FolderMapping;
				if (folderMapping != null)
				{
					badMessageRec.WellKnownFolderType = folderMapping.WKFType;
				}
			}
		}

		private void CheckHealthCallback()
		{
			MoveBaseJob moveBaseJob = this.MRSJob as MoveBaseJob;
			if (moveBaseJob != null && moveBaseJob.IsOnlineMove && moveBaseJob.SyncStage == SyncStage.IncrementalSync)
			{
				moveBaseJob.CheckServersHealth();
			}
		}

		private MailboxCopierBase.ProviderType GetMailboxHelper(ADObjectId database, LocalMailboxFlags mbxFlags, bool isDestinationMailbox, out string mrsProxyFqdn, out string remoteOrgName, out NetworkCredential remoteCredential, out LocalMailboxFlags extraMbxFlags)
		{
			RequestStatisticsBase cachedRequestJob = this.MRSJob.CachedRequestJob;
			bool flag = cachedRequestJob.Direction == RequestDirection.Push;
			bool flag2 = cachedRequestJob.RequestStyle == RequestStyle.IntraOrg || (!flag && isDestinationMailbox) || (flag && !isDestinationMailbox);
			extraMbxFlags = LocalMailboxFlags.None;
			mrsProxyFqdn = null;
			remoteOrgName = null;
			remoteCredential = null;
			if (cachedRequestJob.RequestType == MRSRequestType.Sync && !isDestinationMailbox)
			{
				switch (cachedRequestJob.SyncProtocol)
				{
				case SyncProtocol.Imap:
					return MailboxCopierBase.ProviderType.IMAP;
				case SyncProtocol.Eas:
					return MailboxCopierBase.ProviderType.EAS;
				case SyncProtocol.Pop:
					return MailboxCopierBase.ProviderType.POP;
				case SyncProtocol.Olc:
					mrsProxyFqdn = cachedRequestJob.RemoteHostName;
					return MailboxCopierBase.ProviderType.HttpsRemote;
				default:
					throw new UnexpectedErrorPermanentException((int)(100 + cachedRequestJob.SyncProtocol));
				}
			}
			else
			{
				if ((cachedRequestJob.RequestType == MRSRequestType.MailboxExport && isDestinationMailbox) || (cachedRequestJob.RequestType == MRSRequestType.MailboxImport && !isDestinationMailbox))
				{
					return MailboxCopierBase.ProviderType.PST;
				}
				if (cachedRequestJob.RequestType == MRSRequestType.Merge && !flag2)
				{
					return MailboxCopierBase.ProviderType.MAPI;
				}
				bool flag3 = !isDestinationMailbox && (cachedRequestJob.SkipStorageProviderForSource || mbxFlags.HasFlag(LocalMailboxFlags.UseMapiProvider));
				if (flag3)
				{
					extraMbxFlags |= LocalMailboxFlags.UseMapiProvider;
				}
				if (!cachedRequestJob.SkipWordBreaking && !this.GetConfig<bool>("SkipWordBreaking") && !this.MRSJob.TestIntegration.SkipWordBreaking)
				{
					extraMbxFlags |= LocalMailboxFlags.WordBreak;
					if (cachedRequestJob.InvalidateContentIndexAnnotations)
					{
						extraMbxFlags |= LocalMailboxFlags.InvalidateContentIndexAnnotations;
					}
				}
				if (!flag2)
				{
					if (cachedRequestJob.Flags.HasFlag(RequestFlags.RemoteLegacy))
					{
						return MailboxCopierBase.ProviderType.MAPI;
					}
					mrsProxyFqdn = cachedRequestJob.RemoteHostName;
					remoteOrgName = cachedRequestJob.RemoteOrgName;
					remoteCredential = cachedRequestJob.RemoteCredential;
					return MailboxCopierBase.ProviderType.HttpsRemote;
				}
				else
				{
					ProxyServerSettings proxyServerSettings = CommonUtils.MapDatabaseToProxyServer(database);
					extraMbxFlags |= proxyServerSettings.ExtraFlags;
					mrsProxyFqdn = proxyServerSettings.Fqdn;
					if (!isDestinationMailbox && this.MRSJob.TestIntegration.UseRemoteForSource && cachedRequestJob.RequestType != MRSRequestType.MailboxExport)
					{
						return MailboxCopierBase.ProviderType.TcpRemote;
					}
					if (isDestinationMailbox && this.MRSJob.TestIntegration.UseRemoteForDestination)
					{
						return MailboxCopierBase.ProviderType.TcpRemote;
					}
					if (!extraMbxFlags.HasFlag(LocalMailboxFlags.LocalMachineMapiOnly))
					{
						return MailboxCopierBase.ProviderType.MAPI;
					}
					if (!proxyServerSettings.IsProxyLocal)
					{
						return MailboxCopierBase.ProviderType.TcpRemote;
					}
					if (flag3)
					{
						return MailboxCopierBase.ProviderType.MAPI;
					}
					return MailboxCopierBase.ProviderType.Storage;
				}
			}
		}

		protected static readonly PropTag[] MidsetDeletedPropTags = new PropTag[]
		{
			PropTag.MidsetDeletedExport
		};

		private static readonly PropTag[] LocalDirectoryEntryIdArray = new PropTag[]
		{
			PropTag.LocalDirectoryEntryId
		};

		public static readonly int MinExchangeVersionForPerUserReadUnreadDataTransfer = new ServerVersion(Server.Exchange2011MajorVersion, 0, 444, 0).ToInt();

		public static readonly PropTag[] AlwaysExcludedFolderPtags = new PropTag[]
		{
			PropTag.ContainerContents,
			PropTag.FolderAssociatedContents,
			PropTag.ContainerHierarchy,
			(PropTag)1638859010U,
			PropTag.NTSD,
			PropTag.FreeBusyNTSD,
			(PropTag)1071644930U,
			PropTag.AclTable,
			(PropTag)1073611010U
		};

		private static readonly PropTag[] InboxPropertiesToValidate = new PropTag[]
		{
			PropTag.ContactsFolderEntryId,
			PropTag.CalendarFolderEntryId
		};

		public static readonly PropTag[] PSTIncludeMessagePtags = new PropTag[]
		{
			PropTag.DisplayCc,
			PropTag.DisplayTo,
			PropTag.DisplayBcc
		};

		public static readonly PropTag[] RuleFolderPtags = new PropTag[]
		{
			PropTag.RulesTable,
			(PropTag)1071710466U
		};

		public static readonly PropTag[] ExcludedRootFolderPtags = new PropTag[]
		{
			PropTag.DisplayName,
			PropTag.Comment
		};

		private static readonly PropTag[] AdditionalFolderPtags = new PropTag[]
		{
			PropTag.LocalCommitTimeMax,
			PropTag.DeletedCountTotal,
			PropTag.ChangeKey,
			PropTag.ContentCount,
			PropTag.MessageSizeExtended,
			PropTag.AssocContentCount,
			PropTag.AssocMessageSizeExtended,
			PropTag.ReplicaList,
			PropTag.InternalAccess
		};

		private FolderHierarchy cachedSourceHierarchy;

		private static readonly TimeSpan CopyFolderPropertyReportingFrequency = TimeSpan.FromMinutes(5.0);

		protected enum ProviderType
		{
			MAPI = 1,
			Storage,
			TcpRemote,
			HttpsRemote,
			PST,
			IMAP,
			EAS,
			POP,
			ContactSync
		}
	}
}
