using System;
using System.Collections.Generic;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PublicFolderMoveJob : MoveBaseJob
	{
		public SourceMailboxWrapper SourceMbxWrapper { get; private set; }

		public MailboxWrapper PrimaryHierarchyMbxWrapper { get; private set; }

		public override bool IsOnlineMove
		{
			get
			{
				return base.IsOnlineMove;
			}
			protected set
			{
				base.IsOnlineMove = value;
			}
		}

		protected override bool ReachedThePointOfNoReturn
		{
			get
			{
				return base.TimeTracker.CurrentState == RequestState.Completed || base.SyncStage >= SyncStage.FinalIncrementalSync;
			}
		}

		public override void Initialize(TransactionalRequestJob moveRequestJob)
		{
			MrsTracer.Service.Function("PublicFolderMoveJob.Initialize: Moving folders from , SourceMailbox=\"{0}\", TargetMailbox=\"{1}\", Flags={2}", new object[]
			{
				moveRequestJob.SourceExchangeGuid,
				moveRequestJob.TargetExchangeGuid,
				moveRequestJob.Flags
			});
			base.Initialize(moveRequestJob);
			TenantPublicFolderConfigurationCache.Instance.RemoveValue(moveRequestJob.OrganizationId);
			this.publicFolderConfiguration = TenantPublicFolderConfigurationCache.Instance.GetValue(moveRequestJob.OrganizationId);
			this.foldersToMove = new List<byte[]>(moveRequestJob.FolderList.Count);
			foreach (MoveFolderInfo moveFolderInfo in moveRequestJob.FolderList)
			{
				this.foldersToMove.Add(HexConverter.HexStringToByteArray(moveFolderInfo.EntryId));
			}
			string orgID = (moveRequestJob.OrganizationId != null && moveRequestJob.OrganizationId.OrganizationalUnit != null) ? (moveRequestJob.OrganizationId.OrganizationalUnit.Name + "\\") : string.Empty;
			LocalizedString sourceTracingID = MrsStrings.PublicFolderMoveTracingId(orgID, moveRequestJob.SourceExchangeGuid);
			LocalizedString targetTracingID = MrsStrings.PublicFolderMoveTracingId(orgID, moveRequestJob.TargetExchangeGuid);
			this.publicFolderMover = new PublicFolderMover(moveRequestJob, this, this.foldersToMove, MailboxCopierFlags.Root, sourceTracingID, targetTracingID);
		}

		public override List<MailboxCopierBase> GetAllCopiers()
		{
			return new List<MailboxCopierBase>
			{
				this.publicFolderMover
			};
		}

		public override void ValidateAndPopulateRequestJob(List<ReportEntry> entries)
		{
			this.ConfigureProviders(false);
			MailboxServerInformation mailboxServerInformation = null;
			MailboxInformation mailboxInformation = null;
			this.ConnectSource(this.publicFolderMover, out mailboxServerInformation, out mailboxInformation);
			if (mailboxInformation != null && mailboxInformation.ServerVersion != 0)
			{
				base.CachedRequestJob.SourceVersion = mailboxInformation.ServerVersion;
				base.CachedRequestJob.SourceServer = ((mailboxServerInformation != null) ? mailboxServerInformation.MailboxServerName : null);
			}
			this.ConnectDestination(this.publicFolderMover, out mailboxServerInformation, out mailboxInformation);
			if (mailboxInformation != null && mailboxInformation.ServerVersion != 0)
			{
				base.CachedRequestJob.TargetVersion = mailboxInformation.ServerVersion;
				base.CachedRequestJob.TargetServer = ((mailboxServerInformation != null) ? mailboxServerInformation.MailboxServerName : null);
			}
		}

		protected override void ConfigureProviders(bool continueAfterConfiguringProviders)
		{
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				ISourceMailbox sourceMailbox = this.ConfigureSourceMailbox();
				disposeGuard.Add<ISourceMailbox>(sourceMailbox);
				IDestinationMailbox destinationMailbox = this.ConfigureDestinationMailbox();
				disposeGuard.Add<IDestinationMailbox>(destinationMailbox);
				this.PrimaryHierarchyMbxWrapper = this.ConfigureHierarchyMailbox(sourceMailbox, destinationMailbox);
				string orgID = (base.CachedRequestJob.OrganizationId != null && base.CachedRequestJob.OrganizationId.OrganizationalUnit != null) ? (base.CachedRequestJob.OrganizationId.OrganizationalUnit.Name + "\\") : string.Empty;
				this.SourceMbxWrapper = new SourceMailboxWrapper(sourceMailbox, MailboxWrapperFlags.Source, MrsStrings.PublicFolderMoveTracingId(orgID, base.CachedRequestJob.SourceExchangeGuid));
				this.publicFolderMover.SetMailboxWrappers(this.SourceMbxWrapper, destinationMailbox);
				disposeGuard.Success();
			}
			base.ConfigureProviders(continueAfterConfiguringProviders);
		}

		protected override void UnconfigureProviders()
		{
			if (this.SourceMbxWrapper != null)
			{
				this.SourceMbxWrapper.Dispose();
				this.SourceMbxWrapper = null;
			}
			if (this.PrimaryHierarchyMbxWrapper != null)
			{
				this.PrimaryHierarchyMbxWrapper.Dispose();
				this.PrimaryHierarchyMbxWrapper = null;
			}
			base.UnconfigureProviders();
		}

		protected override void MigrateSecurityDescriptors()
		{
		}

		protected override void MakeConnections()
		{
			base.MakeConnections();
			if (!this.publicFolderMover.IsSourceConnected)
			{
				this.publicFolderMover.ConnectSourceMailbox(MailboxConnectFlags.None);
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.LastSuccessfulSourceConnection, new DateTime?(DateTime.UtcNow));
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.SourceConnectionFailure, null);
				MailboxServerInformation mailboxServerInformation;
				bool flag;
				this.publicFolderMover.SourceMailboxWrapper.UpdateLastConnectedServerInfo(out mailboxServerInformation, out flag);
			}
			this.publicFolderMover.UpdateSourceDestinationFolderIds();
		}

		protected override MailboxChangesManifest EnumerateSourceHierarchyChanges(MailboxCopierBase mbxCtx, bool catchup, SyncContext syncContext)
		{
			return new MailboxChangesManifest
			{
				ChangedFolders = new List<byte[]>(),
				DeletedFolders = new List<byte[]>()
			};
		}

		protected override void CleanupOrphanedDestinationMailbox()
		{
			MrsTracer.Service.Debug("WorkItem: CleanupOrphanedDestinationMailbox - cleaning up partially created contents if any at the destination mailbox", new object[0]);
			if (this.IsOnlineMove)
			{
				base.CheckServersHealth();
			}
			this.publicFolderMover.ClearSyncState(SyncStateClearReason.CleanupOrphanedMailbox);
			base.Report.Append(MrsStrings.ReportCleanUpFoldersDestination("PreparingToMove"));
			this.CleanUpFoldersAtDestination();
			this.SetDestinationInTransitStatus(this.publicFolderMover);
			this.publicFolderMover.SyncState = new PersistedSyncData(base.RequestJobGuid);
			this.publicFolderMover.ICSSyncState = new MailboxMapiSyncState();
			uint mailboxSignatureVersion = (this.publicFolderMover.DestMailboxWrapper.LastConnectedServerInfo != null) ? this.publicFolderMover.DestMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion : 0U;
			this.publicFolderMover.SyncState.MailboxSignatureVersion = mailboxSignatureVersion;
			base.ScheduleWorkItem(new Action(this.CatchupFolderHierarchy), WorkloadType.Unknown);
		}

		protected override void CatchupFolderHierarchy()
		{
			if (!this.PrimaryHierarchyMbxWrapper.Mailbox.IsConnected())
			{
				this.PrimaryHierarchyMbxWrapper.Mailbox.Connect(MailboxConnectFlags.None);
			}
			bool flag = false;
			bool flag2 = false;
			Guid hierarchyMailboxGuid = this.publicFolderConfiguration.GetHierarchyMailboxInformation().HierarchyMailboxGuid;
			if (hierarchyMailboxGuid == base.CachedRequestJob.SourceExchangeGuid)
			{
				flag = true;
			}
			else if (hierarchyMailboxGuid == base.CachedRequestJob.TargetExchangeGuid)
			{
				flag2 = true;
			}
			foreach (byte[] array in this.foldersToMove)
			{
				if (flag)
				{
					using (IFolder folder = ((ISourceMailbox)this.PrimaryHierarchyMbxWrapper.Mailbox).GetFolder(array))
					{
						using (IDestinationFolder folder2 = this.publicFolderMover.DestMailbox.GetFolder(this.publicFolderMover.DestMailbox.GetSessionSpecificEntryId(array)))
						{
							if (folder2 == null)
							{
								if (folder != null)
								{
									MrsTracer.Service.Error("Inconsistency of hierarchy seen at target mailbox...May be a delay in hierarchy synchronization", new object[0]);
									throw new DestinationFolderHierarchyInconsistentTransientException();
								}
								MrsTracer.Service.Debug("Folder {0} unavailable at both source and destination mailbox during catchup", new object[]
								{
									HexConverter.ByteArrayToHexString(array)
								});
							}
						}
						continue;
					}
				}
				if (flag2)
				{
					using (IFolder folder3 = ((IDestinationMailbox)this.PrimaryHierarchyMbxWrapper.Mailbox).GetFolder(array))
					{
						using (ISourceFolder folder4 = this.publicFolderMover.SourceMailbox.GetFolder(this.publicFolderMover.SourceMailbox.GetSessionSpecificEntryId(array)))
						{
							if (folder4 == null)
							{
								if (folder3 != null)
								{
									MrsTracer.Service.Error("Inconsistency of hierarchy seen at source mailbox...May be a delay in hierarchy synchronization", new object[0]);
									throw new SourceFolderHierarchyInconsistentTransientException();
								}
								MrsTracer.Service.Debug("Folder {0} unavailable at both source and destination mailbox during catchup", new object[]
								{
									HexConverter.ByteArrayToHexString(array)
								});
							}
						}
						continue;
					}
				}
				using (IFolder folder5 = ((IDestinationMailbox)this.PrimaryHierarchyMbxWrapper.Mailbox).GetFolder(array))
				{
					if (folder5 == null)
					{
						MrsTracer.Service.Debug("Folder {0} unavailable at hierarchy mailbox during catchup", new object[]
						{
							HexConverter.ByteArrayToHexString(array)
						});
					}
					else
					{
						using (ISourceFolder folder6 = this.publicFolderMover.SourceMailbox.GetFolder(this.publicFolderMover.SourceMailbox.GetSessionSpecificEntryId(array)))
						{
							if (folder6 == null)
							{
								MrsTracer.Service.Error("Inconsistency of hierarchy seen at source mailbox...May be a delay in hierarchy synchronization", new object[0]);
								throw new SourceFolderHierarchyInconsistentTransientException();
							}
						}
						using (IDestinationFolder folder7 = this.publicFolderMover.DestMailbox.GetFolder(this.publicFolderMover.DestMailbox.GetSessionSpecificEntryId(array)))
						{
							if (folder7 == null)
							{
								MrsTracer.Service.Error("Inconsistency of hierarchy seen at target mailbox...May be a delay in hierarchy synchronization", new object[0]);
								throw new DestinationFolderHierarchyInconsistentTransientException();
							}
						}
					}
				}
			}
			base.SyncStage = SyncStage.CreatingInitialSyncCheckpoint;
			base.OverallProgress = 15;
			base.TimeTracker.CurrentState = RequestState.CreatingInitialSyncCheckpoint;
			base.SaveState(SaveStateFlags.Regular, delegate(TransactionalRequestJob moveRequest)
			{
				moveRequest.RestartFromScratch = false;
			});
			base.ScheduleWorkItem(new Action(base.CatchupFolders), WorkloadType.Unknown);
		}

		protected override void SetSourceInTransitStatus(MailboxCopierBase mbxCtx, InTransitStatus status, out bool sourceSupportsOnlineMove)
		{
			mbxCtx.SourceMailbox.SetInTransitStatus(status | InTransitStatus.ForPublicFolderMove, out sourceSupportsOnlineMove);
			if (!sourceSupportsOnlineMove)
			{
				MrsTracer.Service.Debug("Source does not support online move for public folder move job.", new object[0]);
				throw new OnlineMoveNotSupportedPermanentException(base.CachedRequestJob.SourceExchangeGuid.ToString());
			}
		}

		protected override void SetDestinationInTransitStatus(MailboxCopierBase mbxCtx)
		{
			InTransitStatus inTransitStatus = InTransitStatus.MoveDestination | InTransitStatus.OnlineMove | InTransitStatus.ForPublicFolderMove;
			if (base.CachedRequestJob.AllowLargeItems)
			{
				inTransitStatus |= InTransitStatus.AllowLargeItems;
			}
			bool flag;
			mbxCtx.DestMailbox.SetInTransitStatus(inTransitStatus, out flag);
			if (!flag)
			{
				MrsTracer.Service.Debug("Destination does not support online move for public folder move job.", new object[0]);
				throw new OnlineMoveNotSupportedPermanentException(base.CachedRequestJob.TargetExchangeGuid.ToString());
			}
		}

		protected override void FinalSync()
		{
			base.TestIntegration.Barrier("PostponeFinalSync", new Action(base.RefreshRequestIfNeeded));
			if (!this.PrimaryHierarchyMbxWrapper.Mailbox.IsConnected())
			{
				this.PrimaryHierarchyMbxWrapper.Mailbox.Connect(MailboxConnectFlags.None);
			}
			base.FinalSync();
		}

		protected override bool ValidateTargetMailbox(MailboxInformation mailboxInfo, out LocalizedString moveFinishedReason)
		{
			moveFinishedReason = MrsStrings.ReportTargetPublicFolderContentMailboxGuidUpdated;
			return true;
		}

		protected override void CleanupCanceledJob()
		{
			base.CheckDisposed();
			MrsTracer.Service.Debug("Deleting messages and dumpster folder.", new object[0]);
			using (List<MailboxCopierBase>.Enumerator enumerator = this.GetAllCopiers().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MailboxCopierBase mbxCtx = enumerator.Current;
					CommonUtils.CatchKnownExceptions(delegate
					{
						if (!mbxCtx.IsDestinationConnected)
						{
							mbxCtx.ConnectDestinationMailbox(MailboxConnectFlags.None);
						}
						SyncStateError syncStateError = mbxCtx.LoadSyncState(this.Report);
						if (syncStateError != SyncStateError.None && this.CanBeCanceledOrSuspended())
						{
							MrsTracer.Service.Debug("Deleting folders at destination mailbox {0}", new object[]
							{
								mbxCtx.TargetTracingID
							});
							MrsTracer.Service.Debug(MrsStrings.ReportCleanUpFoldersDestination("CleanupCanceledJob"), new object[0]);
							this.CleanUpFoldersAtDestination();
						}
						mbxCtx.ClearSyncState(SyncStateClearReason.JobCanceled);
						bool flag;
						mbxCtx.DestMailbox.SetInTransitStatus(InTransitStatus.NotInTransit, out flag);
					}, delegate(Exception failure)
					{
						LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
						this.Report.Append(MrsStrings.ReportDestinationMailboxCleanupFailed2(CommonUtils.GetFailureType(failure)), failure, ReportEntryFlags.Cleanup | ReportEntryFlags.Target);
						MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_DestinationMailboxCleanupFailed, new object[]
						{
							this.RequestJobIdentity,
							mbxCtx.TargetTracingID,
							this.CachedRequestJob.TargetMDBName,
							localizedString
						});
					});
				}
			}
			MoveHistoryEntryInternal mhei;
			base.RemoveRequest(true, out mhei);
			using (List<MailboxCopierBase>.Enumerator enumerator2 = this.GetAllCopiers().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					PublicFolderMoveJob.<>c__DisplayClassc CS$<>8__locals3 = new PublicFolderMoveJob.<>c__DisplayClassc();
					CS$<>8__locals3.mbxCtx = enumerator2.Current;
					bool resetInTransitSuccess = false;
					CommonUtils.CatchKnownExceptions(delegate
					{
						if (!CS$<>8__locals3.mbxCtx.IsSourceConnected)
						{
							CS$<>8__locals3.mbxCtx.ConnectSourceMailbox(MailboxConnectFlags.None);
						}
						bool flag;
						CS$<>8__locals3.mbxCtx.SourceMailbox.SetInTransitStatus(InTransitStatus.NotInTransit, out flag);
						resetInTransitSuccess = true;
						if (CS$<>8__locals3.mbxCtx.IsRoot && mhei != null)
						{
							CS$<>8__locals3.mbxCtx.SourceMailbox.AddMoveHistoryEntry(mhei, this.GetConfig<int>("MaxMoveHistoryLength"));
						}
					}, delegate(Exception failure)
					{
						if (!resetInTransitSuccess)
						{
							LocalizedString localizedString = CommonUtils.FullExceptionMessage(failure);
							MailboxReplicationService.LogEvent(MRSEventLogConstants.Tuple_SourceMailboxResetFailed, new object[]
							{
								this.RequestJobIdentity,
								CS$<>8__locals3.mbxCtx.SourceTracingID,
								this.CachedRequestJob.SourceMDBName,
								localizedString
							});
						}
					});
				}
			}
			base.Disconnect();
		}

		protected override void UpdateMovedMailbox()
		{
			if (!this.PrimaryHierarchyMbxWrapper.Mailbox.IsConnected())
			{
				this.PrimaryHierarchyMbxWrapper.Mailbox.Connect(MailboxConnectFlags.None);
			}
			Guid hierarchyMailboxGuid = this.publicFolderConfiguration.GetHierarchyMailboxInformation().HierarchyMailboxGuid;
			IDataConverter<PropValue, PropValueData> dataConverter = new PropValueConverter();
			int num = 0;
			byte[] array = null;
			PropTag propTag = this.PrimaryHierarchyMbxWrapper.Mailbox.GetIDsFromNames(true, new NamedPropData[]
			{
				new NamedPropData(FolderSchema.LastMovedTimeStamp.Guid, FolderSchema.LastMovedTimeStamp.PropertyName)
			})[0];
			propTag = propTag.ChangePropType(PropType.SysTime);
			List<MoveFolderInfo> moveFolderList = base.CachedRequestJob.FolderList;
			foreach (MoveFolderInfo moveFolderInfo in moveFolderList)
			{
				array = HexConverter.HexStringToByteArray(moveFolderInfo.EntryId);
				if (!moveFolderInfo.IsFinalized)
				{
					IFolder folder2;
					if (!(hierarchyMailboxGuid == base.CachedRequestJob.SourceExchangeGuid))
					{
						IFolder folder = ((IDestinationMailbox)this.PrimaryHierarchyMbxWrapper.Mailbox).GetFolder(array);
						folder2 = folder;
					}
					else
					{
						folder2 = ((ISourceMailbox)this.PrimaryHierarchyMbxWrapper.Mailbox).GetFolder(array);
					}
					PropValue nativeRepresentation2;
					using (IFolder folder3 = folder2)
					{
						if (folder3 == null)
						{
							MrsTracer.Service.Debug("Folder {0} unavailable at hierarchy mailbox during finalization", new object[]
							{
								HexConverter.ByteArrayToHexString(array)
							});
							continue;
						}
						List<PropValueData> list = new List<PropValueData>(2);
						PropValueData[] props = folder3.GetProps(new PropTag[]
						{
							PropTag.TimeInServer,
							PropTag.PfProxy
						});
						PropValue nativeRepresentation = dataConverter.GetNativeRepresentation(props[0]);
						list.Add(new PropValueData(propTag, ExDateTime.UtcNow));
						if (!nativeRepresentation.IsNull() && !nativeRepresentation.IsError())
						{
							ELCFolderFlags elcfolderFlags = (ELCFolderFlags)nativeRepresentation.Value;
							if (elcfolderFlags.HasFlag(ELCFolderFlags.DumpsterFolder))
							{
								moveFolderInfo.IsFinalized = true;
								folder3.SetProps(list.ToArray());
								continue;
							}
						}
						list.Add(new PropValueData(PropTag.ReplicaList, ReplicaListProperty.GetBytesFromStringArray(new string[]
						{
							base.CachedRequestJob.TargetExchangeGuid.ToString()
						})));
						folder3.SetProps(list.ToArray());
						nativeRepresentation2 = dataConverter.GetNativeRepresentation(props[1]);
					}
					if (hierarchyMailboxGuid != base.CachedRequestJob.SourceExchangeGuid)
					{
						this.UpdateSourceFolder(array);
					}
					this.UpdateDestinationFolder(array, nativeRepresentation2);
					moveFolderInfo.IsFinalized = true;
					base.Report.AppendDebug(string.Format("Folder {0} has been successfully finalized.", HexConverter.ByteArrayToHexString(array)));
					num++;
					if (num == 100)
					{
						base.SaveRequest(true, delegate(TransactionalRequestJob rj)
						{
							rj.FolderList = moveFolderList;
						});
						num = 0;
					}
				}
			}
			base.SaveRequest(true, delegate(TransactionalRequestJob rj)
			{
				rj.FolderList = base.CachedRequestJob.FolderList;
			});
		}

		protected override void PostMoveUpdateSourceMailbox(MailboxCopierBase mbxCtx)
		{
			mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.UpdateSourceMailbox;
		}

		protected override void PostMoveCleanupSourceMailbox(MailboxCopierBase mbxCtx)
		{
			mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.SourceMailboxCleanup;
		}

		protected override void PostMoveMarkRehomeOnRelatedRequests(MailboxCopierBase mbxCtx)
		{
			mbxCtx.SyncState.CompletedCleanupTasks |= PostMoveCleanupStatusFlags.SetRelatedRequestsRehome;
		}

		protected override void CleanupDestinationMailbox(MailboxCopierBase mbxCtx, bool moveIsSuccessful)
		{
		}

		protected override void ScheduleContentVerification(List<FolderSizeRec> verificationResults)
		{
			FolderMap sourceFolderMap = this.publicFolderMover.GetSourceFolderMap(GetFolderMapFlags.None);
			sourceFolderMap.EnumerateFolderHierarchy(EnumHierarchyFlags.NormalFolders | EnumHierarchyFlags.RootFolder, delegate(FolderRecWrapper folderRec, FolderMap.EnumFolderContext ctx)
			{
				this.ScheduleWorkItem<PublicFolderMover, FolderRecWrapper, List<FolderSizeRec>>(new Action<PublicFolderMover, FolderRecWrapper, List<FolderSizeRec>>(this.VerifyFolderContents), this.publicFolderMover, folderRec, verificationResults, WorkloadType.Unknown);
			});
		}

		protected override void VerifyFolderContents(MailboxCopierBase mbxCtx, FolderRecWrapper folderRecWrapper, List<FolderSizeRec> verificationResults)
		{
			FolderSizeRec folderSizeRec = mbxCtx.VerifyFolderContents(folderRecWrapper, WellKnownFolderType.None, false);
			verificationResults.Add(folderSizeRec);
			mbxCtx.ReportBadItems(folderSizeRec.MissingItems);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.SourceMbxWrapper != null)
				{
					this.SourceMbxWrapper.Dispose();
					this.SourceMbxWrapper = null;
				}
				if (this.PrimaryHierarchyMbxWrapper != null)
				{
					this.PrimaryHierarchyMbxWrapper.Dispose();
					this.PrimaryHierarchyMbxWrapper = null;
				}
				if (this.publicFolderMover != null)
				{
					this.publicFolderMover.UnconfigureProviders();
					this.publicFolderMover = null;
				}
			}
			base.InternalDispose(calledFromDispose);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderMoveJob>(this);
		}

		private static void EmptyContents(IFolder folder)
		{
			List<MessageRec> list = new List<MessageRec>();
			list = folder.EnumerateMessages(EnumerateMessagesFlags.AllMessages, null);
			if (list.Count > 0)
			{
				List<byte[]> list2 = new List<byte[]>();
				foreach (MessageRec messageRec in list)
				{
					list2.Add(messageRec.EntryId);
				}
				folder.DeleteMessages(list2.ToArray());
			}
		}

		private ISourceMailbox ConfigureSourceMailbox()
		{
			PublicFolderRecipient localMailboxRecipient = this.publicFolderConfiguration.GetLocalMailboxRecipient(base.CachedRequestJob.SourceExchangeGuid);
			if (localMailboxRecipient == null)
			{
				throw new RecipientNotFoundPermanentException(base.CachedRequestJob.SourceExchangeGuid);
			}
			List<MRSProxyCapabilities> list = new List<MRSProxyCapabilities>();
			list.Add(MRSProxyCapabilities.PublicFolderMove);
			LocalMailboxFlags sourceMbxFlags = CommonUtils.MapDatabaseToProxyServer(localMailboxRecipient.Database.ObjectGuid).ExtraFlags | LocalMailboxFlags.PublicFolderMove | LocalMailboxFlags.Move;
			ISourceMailbox sourceMailbox = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				sourceMailbox = this.publicFolderMover.GetSourceMailbox(localMailboxRecipient.Database, sourceMbxFlags, list);
				disposeGuard.Add<ISourceMailbox>(sourceMailbox);
				sourceMailbox.Config(base.GetReservation(localMailboxRecipient.Database.ObjectGuid, ReservationFlags.Read), base.CachedRequestJob.SourceExchangeGuid, base.CachedRequestJob.SourceExchangeGuid, CommonUtils.GetPartitionHint(base.CachedRequestJob.OrganizationId), localMailboxRecipient.Database.ObjectGuid, MailboxType.SourceMailbox, null);
				disposeGuard.Success();
			}
			return sourceMailbox;
		}

		private IDestinationMailbox ConfigureDestinationMailbox()
		{
			PublicFolderRecipient localMailboxRecipient = this.publicFolderConfiguration.GetLocalMailboxRecipient(base.CachedRequestJob.TargetExchangeGuid);
			if (localMailboxRecipient == null)
			{
				throw new RecipientNotFoundPermanentException(base.CachedRequestJob.TargetExchangeGuid);
			}
			List<MRSProxyCapabilities> list = new List<MRSProxyCapabilities>();
			list.Add(MRSProxyCapabilities.PublicFolderMove);
			LocalMailboxFlags targetMbxFlags = CommonUtils.MapDatabaseToProxyServer(localMailboxRecipient.Database.ObjectGuid).ExtraFlags | LocalMailboxFlags.PublicFolderMove | LocalMailboxFlags.Move;
			IDestinationMailbox destinationMailbox = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				destinationMailbox = this.publicFolderMover.GetDestinationMailbox(localMailboxRecipient.Database.ObjectGuid, targetMbxFlags, list);
				disposeGuard.Add<IDestinationMailbox>(destinationMailbox);
				destinationMailbox.Config(base.GetReservation(localMailboxRecipient.Database.ObjectGuid, ReservationFlags.Write), base.CachedRequestJob.TargetExchangeGuid, base.CachedRequestJob.TargetExchangeGuid, CommonUtils.GetPartitionHint(base.CachedRequestJob.OrganizationId), localMailboxRecipient.Database.ObjectGuid, MailboxType.DestMailboxIntraOrg, null);
				disposeGuard.Success();
			}
			return destinationMailbox;
		}

		private MailboxWrapper ConfigureHierarchyMailbox(ISourceMailbox sourceMailbox, IDestinationMailbox destinationMailbox)
		{
			MailboxWrapper mailboxWrapper = null;
			Guid hierarchyMailboxGuid = this.publicFolderConfiguration.GetHierarchyMailboxInformation().HierarchyMailboxGuid;
			string orgID = (base.CachedRequestJob.OrganizationId != null && base.CachedRequestJob.OrganizationId.OrganizationalUnit != null) ? (base.CachedRequestJob.OrganizationId.OrganizationalUnit.Name + "\\") : string.Empty;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				if (hierarchyMailboxGuid == base.CachedRequestJob.SourceExchangeGuid)
				{
					mailboxWrapper = new SourceMailboxWrapper(sourceMailbox, MailboxWrapperFlags.Source, MrsStrings.PublicFolderMoveTracingId(orgID, base.CachedRequestJob.SourceExchangeGuid));
					disposeGuard.Add<MailboxWrapper>(mailboxWrapper);
				}
				else if (hierarchyMailboxGuid == base.CachedRequestJob.TargetExchangeGuid)
				{
					mailboxWrapper = new DestinationMailboxWrapper(destinationMailbox, MailboxWrapperFlags.Target, MrsStrings.PublicFolderMoveTracingId(orgID, base.CachedRequestJob.TargetExchangeGuid), new Guid[]
					{
						hierarchyMailboxGuid
					});
					disposeGuard.Add<MailboxWrapper>(mailboxWrapper);
				}
				else
				{
					PublicFolderRecipient localMailboxRecipient = this.publicFolderConfiguration.GetLocalMailboxRecipient(hierarchyMailboxGuid);
					if (localMailboxRecipient == null)
					{
						throw new RecipientNotFoundPermanentException(hierarchyMailboxGuid);
					}
					List<MRSProxyCapabilities> list = new List<MRSProxyCapabilities>();
					list.Add(MRSProxyCapabilities.PublicFolderMove);
					ProxyServerSettings proxyServerSettings = CommonUtils.MapDatabaseToProxyServer(localMailboxRecipient.Database.ObjectGuid);
					LocalMailboxFlags flags = proxyServerSettings.ExtraFlags | LocalMailboxFlags.PublicFolderMove | LocalMailboxFlags.Move;
					RemoteDestinationMailbox remoteDestinationMailbox = new RemoteDestinationMailbox(proxyServerSettings.Fqdn, null, null, MailboxCopierBase.DefaultProxyControlFlags, list, false, flags);
					disposeGuard.Add<RemoteDestinationMailbox>(remoteDestinationMailbox);
					mailboxWrapper = new DestinationMailboxWrapper(remoteDestinationMailbox, MailboxWrapperFlags.Target, MrsStrings.PublicFolderMoveTracingId(orgID, hierarchyMailboxGuid), new Guid[]
					{
						hierarchyMailboxGuid
					});
					disposeGuard.Add<MailboxWrapper>(mailboxWrapper);
					mailboxWrapper.Mailbox.Config(base.GetReservation(localMailboxRecipient.Database.ObjectGuid, ReservationFlags.Write), hierarchyMailboxGuid, hierarchyMailboxGuid, CommonUtils.GetPartitionHint(base.CachedRequestJob.OrganizationId), localMailboxRecipient.Database.ObjectGuid, MailboxType.DestMailboxIntraOrg, null);
				}
				disposeGuard.Success();
			}
			return mailboxWrapper;
		}

		private void ConnectSource(MailboxCopierBase mbxCtx, out MailboxServerInformation sourceMailboxServerInfo, out MailboxInformation sourceMailboxInfo)
		{
			mbxCtx.ConnectSourceMailbox(MailboxConnectFlags.None);
			sourceMailboxServerInfo = mbxCtx.SourceMailbox.GetMailboxServerInformation();
			sourceMailboxInfo = mbxCtx.SourceMailbox.GetMailboxInformation();
		}

		private void ConnectDestination(MailboxCopierBase mbxCtx, out MailboxServerInformation destinationMailboxServerInfo, out MailboxInformation destinationMailboxInfo)
		{
			mbxCtx.ConnectDestinationMailbox(MailboxConnectFlags.None);
			destinationMailboxServerInfo = mbxCtx.DestMailbox.GetMailboxServerInformation();
			destinationMailboxInfo = mbxCtx.DestMailbox.GetMailboxInformation();
		}

		private void CleanUpFoldersAtDestination()
		{
			IDataConverter<PropValue, PropValueData> dataConverter = new PropValueConverter();
			foreach (byte[] array in this.foldersToMove)
			{
				byte[] sessionSpecificEntryId = this.publicFolderMover.DestMailbox.GetSessionSpecificEntryId(array);
				using (IDestinationFolder folder = this.publicFolderMover.DestMailbox.GetFolder(sessionSpecificEntryId))
				{
					if (folder == null)
					{
						MrsTracer.Service.Debug("CleanUpFoldersAtDestination: Folder {0} is not present to cleanup at target mailbox", new object[]
						{
							HexConverter.ByteArrayToHexString(array)
						});
					}
					else
					{
						PropValueData[] props = folder.GetProps(new PropTag[]
						{
							PropTag.ReplicaList
						});
						PropValue nativeRepresentation = dataConverter.GetNativeRepresentation(props[0]);
						byte[] array2 = nativeRepresentation.Value as byte[];
						if (!nativeRepresentation.IsNull() && !nativeRepresentation.IsError() && array2 != null)
						{
							StorePropertyDefinition replicaList = CoreFolderSchema.ReplicaList;
							string[] stringArrayFromBytes = ReplicaListProperty.GetStringArrayFromBytes(array2);
							Guid empty = Guid.Empty;
							if (stringArrayFromBytes.Length > 0 && GuidHelper.TryParseGuid(stringArrayFromBytes[0], out empty) && empty == base.CachedRequestJob.TargetExchangeGuid)
							{
								throw new FolderAlreadyInTargetPermanentException(HexConverter.ByteArrayToHexString(array));
							}
						}
						PublicFolderMoveJob.EmptyContents(folder);
						folder.SetRules(Array<RuleData>.Empty);
					}
				}
			}
		}

		private void UpdateSourceFolder(byte[] hierarchyFolderEntryId)
		{
			byte[] sessionSpecificEntryId = this.publicFolderMover.SourceMailbox.GetSessionSpecificEntryId(hierarchyFolderEntryId);
			using (ISourceFolder folder = this.publicFolderMover.SourceMailbox.GetFolder(sessionSpecificEntryId))
			{
				if (folder == null)
				{
					MrsTracer.Service.Error("UpdateSourceFolder: Folder {0} unavailable at source mailbox", new object[]
					{
						HexConverter.ByteArrayToHexString(hierarchyFolderEntryId)
					});
					throw new UpdateFolderFailedTransientException();
				}
				folder.SetProps(new PropValueData[]
				{
					new PropValueData(PropTag.ReplicaList, ReplicaListProperty.GetBytesFromStringArray(new string[]
					{
						base.CachedRequestJob.TargetExchangeGuid.ToString()
					}))
				});
			}
		}

		private void UpdateDestinationFolder(byte[] hierarchyFolderEntryId, PropValue pfProxyValue)
		{
			byte[] sessionSpecificEntryId = this.publicFolderMover.DestMailbox.GetSessionSpecificEntryId(hierarchyFolderEntryId);
			using (IDestinationFolder folder = this.publicFolderMover.DestMailbox.GetFolder(sessionSpecificEntryId))
			{
				if (folder == null)
				{
					MrsTracer.Service.Error("UpdateDestinationFolder: Folder {0} unavailable at target mailbox", new object[]
					{
						HexConverter.ByteArrayToHexString(hierarchyFolderEntryId)
					});
					throw new UpdateFolderFailedTransientException();
				}
				folder.SetProps(new PropValueData[]
				{
					new PropValueData(PropTag.ReplicaList, ReplicaListProperty.GetBytesFromStringArray(new string[]
					{
						base.CachedRequestJob.TargetExchangeGuid.ToString()
					}))
				});
				if (!pfProxyValue.IsNull() && !pfProxyValue.IsError())
				{
					byte[] bytes = pfProxyValue.GetBytes();
					if (bytes != null && bytes.Length == 16 && new Guid(bytes) != Guid.Empty)
					{
						Guid a = folder.LinkMailPublicFolder(LinkMailPublicFolderFlags.ObjectGuid, bytes);
						if (a == Guid.Empty)
						{
							base.Report.Append(new ReportEntry(MrsStrings.ReportFailedToLinkADPublicFolder(folder.GetFolderRec(null, GetFolderRecFlags.None).FolderName, BitConverter.ToString(bytes), BitConverter.ToString(hierarchyFolderEntryId)), ReportEntryType.Warning));
						}
					}
				}
			}
		}

		private const int DefaultBatchSize = 100;

		private const int SizeOfGuid = 16;

		private PublicFolderMover publicFolderMover;

		private List<byte[]> foldersToMove;

		private TenantPublicFolderConfiguration publicFolderConfiguration;
	}
}
