using System;
using System.Collections.Generic;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class AuxFolderMoveJob : MoveBaseJob
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
			MrsTracer.Service.Function("AuxFolderMoveJob.Initialize: Moving folders from , SourceMailbox=\"{0}\", TargetMailbox=\"{1}\", Flags={2}", new object[]
			{
				moveRequestJob.SourceExchangeGuid,
				moveRequestJob.TargetExchangeGuid,
				moveRequestJob.Flags
			});
			base.Initialize(moveRequestJob);
			this.foldersToMove = new List<byte[]>(moveRequestJob.FolderList.Count);
			foreach (MoveFolderInfo moveFolderInfo in moveRequestJob.FolderList)
			{
				this.foldersToMove.Add(HexConverter.HexStringToByteArray(moveFolderInfo.EntryId));
			}
			string orgID = (moveRequestJob.OrganizationId != null && moveRequestJob.OrganizationId.OrganizationalUnit != null) ? (moveRequestJob.OrganizationId.OrganizationalUnit.Name + "\\") : string.Empty;
			LocalizedString sourceTracingID = MrsStrings.AuxFolderMoveTracingId(orgID, moveRequestJob.SourceExchangeGuid);
			LocalizedString targetTracingID = MrsStrings.AuxFolderMoveTracingId(orgID, moveRequestJob.TargetExchangeGuid);
			this.folderMover = new PublicFolderMover(moveRequestJob, this, this.foldersToMove, MailboxCopierFlags.Root, sourceTracingID, targetTracingID);
		}

		public override List<MailboxCopierBase> GetAllCopiers()
		{
			return new List<MailboxCopierBase>
			{
				this.folderMover
			};
		}

		public override void ValidateAndPopulateRequestJob(List<ReportEntry> entries)
		{
			this.ConfigureProviders(false);
			MailboxServerInformation mailboxServerInformation = null;
			MailboxInformation mailboxInformation = null;
			this.ConnectSource(this.folderMover, out mailboxServerInformation, out mailboxInformation);
			if (mailboxInformation != null && mailboxInformation.ServerVersion != 0)
			{
				base.CachedRequestJob.SourceVersion = mailboxInformation.ServerVersion;
				base.CachedRequestJob.SourceServer = ((mailboxServerInformation != null) ? mailboxServerInformation.MailboxServerName : null);
			}
			this.ConnectDestination(this.folderMover, out mailboxServerInformation, out mailboxInformation);
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
				this.SourceMbxWrapper = new SourceMailboxWrapper(sourceMailbox, MailboxWrapperFlags.Source, MrsStrings.AuxFolderMoveTracingId(orgID, base.CachedRequestJob.SourceExchangeGuid));
				this.folderMover.SetMailboxWrappers(this.SourceMbxWrapper, destinationMailbox);
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
			if (!this.folderMover.IsSourceConnected)
			{
				this.folderMover.ConnectSourceMailbox(MailboxConnectFlags.None);
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.LastSuccessfulSourceConnection, new DateTime?(DateTime.UtcNow));
				base.TimeTracker.SetTimestamp(RequestJobTimestamp.SourceConnectionFailure, null);
				MailboxServerInformation mailboxServerInformation;
				bool flag;
				this.folderMover.SourceMailboxWrapper.UpdateLastConnectedServerInfo(out mailboxServerInformation, out flag);
			}
			this.folderMover.UpdateSourceDestinationFolderIds();
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
			this.folderMover.ClearSyncState(SyncStateClearReason.CleanupOrphanedMailbox);
			base.Report.Append(MrsStrings.ReportCleanUpFoldersDestination("PreparingToMove"));
			this.CleanUpFoldersAtDestination();
			this.SetDestinationInTransitStatus(this.folderMover);
			this.folderMover.SyncState = new PersistedSyncData(base.RequestJobGuid);
			this.folderMover.ICSSyncState = new MailboxMapiSyncState();
			uint mailboxSignatureVersion = (this.folderMover.DestMailboxWrapper.LastConnectedServerInfo != null) ? this.folderMover.DestMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion : 0U;
			this.folderMover.SyncState.MailboxSignatureVersion = mailboxSignatureVersion;
			base.ScheduleWorkItem(new Action(this.CatchupFolderHierarchy), WorkloadType.Unknown);
		}

		protected override void CatchupFolderHierarchy()
		{
			if (!this.PrimaryHierarchyMbxWrapper.Mailbox.IsConnected())
			{
				this.PrimaryHierarchyMbxWrapper.Mailbox.Connect(MailboxConnectFlags.None);
			}
			FolderRecDataFlags folderRecDataFlags = FolderRecDataFlags.SearchCriteria;
			if (!base.CachedRequestJob.SkipFolderPromotedProperties)
			{
				folderRecDataFlags |= FolderRecDataFlags.PromotedProperties;
			}
			if (!base.CachedRequestJob.SkipFolderViews)
			{
				folderRecDataFlags |= FolderRecDataFlags.Views;
			}
			if (!base.CachedRequestJob.SkipFolderRestrictions)
			{
				folderRecDataFlags |= FolderRecDataFlags.Restrictions;
			}
			List<WellKnownFolder> list = this.folderMover.SourceMailbox.DiscoverWellKnownFolders(0);
			List<WellKnownFolder> destinationWellKnownFolders = this.folderMover.DestMailbox.DiscoverWellKnownFolders(0);
			foreach (byte[] array in this.foldersToMove)
			{
				using (IFolder folder = ((ISourceMailbox)this.PrimaryHierarchyMbxWrapper.Mailbox).GetFolder(array))
				{
					using (IDestinationFolder folder2 = this.folderMover.DestMailbox.GetFolder(this.folderMover.DestMailbox.GetSessionSpecificEntryId(array)))
					{
						if (folder2 == null)
						{
							if (folder == null)
							{
								MrsTracer.Service.Debug("Folder {0} unavailable at both source and destination mailbox during catchup", new object[]
								{
									HexConverter.ByteArrayToHexString(array)
								});
							}
							else
							{
								List<FolderRecWrapper> list2 = new List<FolderRecWrapper>();
								byte[] entryId = array;
								for (;;)
								{
									using (ISourceFolder folder3 = this.folderMover.SourceMailbox.GetFolder(entryId))
									{
										if (folder3 != null)
										{
											FolderRecWrapper folderRecWrapper = new FolderRecWrapper(folder3.GetFolderRec(null, GetFolderRecFlags.None));
											if (folderRecWrapper.ParentId != null)
											{
												list2.Insert(0, folderRecWrapper);
												bool flag = false;
												foreach (WellKnownFolder wellKnownFolder in list)
												{
													if (CommonUtils.IsSameEntryId(wellKnownFolder.EntryId, folderRecWrapper.ParentId))
													{
														flag = true;
														break;
													}
												}
												if (!flag)
												{
													entryId = folderRecWrapper.ParentId;
													continue;
												}
											}
										}
									}
									break;
								}
								foreach (FolderRecWrapper folderRecWrapper2 in list2)
								{
									byte[] entryId2 = folderRecWrapper2.FolderRec.EntryId;
									byte[] parentId = folderRecWrapper2.FolderRec.ParentId;
									byte[] sessionSpecificEntryId = this.folderMover.DestMailbox.GetSessionSpecificEntryId(entryId2);
									bool flag2 = false;
									byte[] destinationFolderIdFromSourceFolderId = AuxFolderMoveJob.GetDestinationFolderIdFromSourceFolderId(list, destinationWellKnownFolders, this.folderMover.DestMailbox, parentId, out flag2);
									using (IDestinationFolder folder4 = this.folderMover.DestMailbox.GetFolder(sessionSpecificEntryId))
									{
										if (folder4 == null)
										{
											using (ISourceFolder folder5 = this.folderMover.SourceMailbox.GetFolder(entryId2))
											{
												if (folder5 == null)
												{
													MrsTracer.Service.Warning("Source folder '{0}' {1} disappeared, skipping", new object[]
													{
														folderRecWrapper2.FolderName,
														folderRecWrapper2.EntryId
													});
													break;
												}
												folderRecWrapper2.EnsureDataLoaded(folder5, folderRecDataFlags, this.folderMover);
												this.folderMover.TranslateFolderData(folderRecWrapper2);
												CreateFolderFlags createFolderFlags = CreateFolderFlags.None;
												folderRecWrapper2.FolderRec.EntryId = sessionSpecificEntryId;
												folderRecWrapper2.FolderRec.ParentId = destinationFolderIdFromSourceFolderId;
												byte[] array2;
												this.folderMover.CreateFolder(null, folderRecWrapper2, createFolderFlags, out array2);
												folderRecWrapper2.FolderRec.EntryId = entryId2;
												folderRecWrapper2.FolderRec.ParentId = parentId;
											}
										}
									}
								}
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
				MrsTracer.Service.Debug("Source does not support online move for aux folder move job.", new object[0]);
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
			moveFinishedReason = MrsStrings.ReportTargetAuxFolderContentMailboxGuidUpdated;
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
					AuxFolderMoveJob.<>c__DisplayClass138 CS$<>8__locals3 = new AuxFolderMoveJob.<>c__DisplayClass138();
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
			Guid sourceExchangeGuid = base.CachedRequestJob.SourceExchangeGuid;
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
					PropValue nativeRepresentation2;
					using (IFolder folder = ((ISourceMailbox)this.PrimaryHierarchyMbxWrapper.Mailbox).GetFolder(array))
					{
						if (folder == null)
						{
							MrsTracer.Service.Debug("Folder {0} unavailable at hierarchy mailbox during finalization", new object[]
							{
								HexConverter.ByteArrayToHexString(array)
							});
							continue;
						}
						List<PropValueData> list = new List<PropValueData>(2);
						PropValueData[] props = folder.GetProps(new PropTag[]
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
								folder.SetProps(list.ToArray());
								continue;
							}
						}
						list.Add(new PropValueData(PropTag.ReplicaList, ReplicaListProperty.GetBytesFromStringArray(new string[]
						{
							base.CachedRequestJob.TargetExchangeGuid.ToString()
						})));
						folder.SetProps(list.ToArray());
						nativeRepresentation2 = dataConverter.GetNativeRepresentation(props[1]);
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
			FolderMap sourceFolderMap = this.folderMover.GetSourceFolderMap(GetFolderMapFlags.None);
			sourceFolderMap.EnumerateFolderHierarchy(EnumHierarchyFlags.NormalFolders | EnumHierarchyFlags.RootFolder, delegate(FolderRecWrapper folderRec, FolderMap.EnumFolderContext ctx)
			{
				this.ScheduleWorkItem<PublicFolderMover, FolderRecWrapper, List<FolderSizeRec>>(new Action<PublicFolderMover, FolderRecWrapper, List<FolderSizeRec>>(this.VerifyFolderContents), this.folderMover, folderRec, verificationResults, WorkloadType.Unknown);
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
				if (this.folderMover != null)
				{
					this.folderMover.UnconfigureProviders();
					this.folderMover = null;
				}
			}
			base.InternalDispose(calledFromDispose);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AuxFolderMoveJob>(this);
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

		private static byte[] GetDestinationFolderIdFromSourceFolderId(List<WellKnownFolder> sourceWellKnownFolders, List<WellKnownFolder> destinationWellKnownFolders, IDestinationMailbox destinationMailbox, byte[] sourceFolderId, out bool isWellKnownFolder)
		{
			isWellKnownFolder = false;
			WellKnownFolder wellKnownFolder = null;
			foreach (WellKnownFolder wellKnownFolder2 in sourceWellKnownFolders)
			{
				if (CommonUtils.IsSameEntryId(wellKnownFolder2.EntryId, sourceFolderId))
				{
					wellKnownFolder = wellKnownFolder2;
					break;
				}
			}
			if (wellKnownFolder != null)
			{
				foreach (WellKnownFolder wellKnownFolder3 in destinationWellKnownFolders)
				{
					if (wellKnownFolder3.WKFType == wellKnownFolder.WKFType)
					{
						isWellKnownFolder = true;
						return wellKnownFolder3.EntryId;
					}
				}
				return null;
			}
			return destinationMailbox.GetSessionSpecificEntryId(sourceFolderId);
		}

		private ISourceMailbox ConfigureSourceMailbox()
		{
			List<MRSProxyCapabilities> mrsProxyCaps = new List<MRSProxyCapabilities>();
			Guid sourceMDBGuid = base.CachedRequestJob.SourceMDBGuid;
			LocalMailboxFlags sourceMbxFlags = CommonUtils.MapDatabaseToProxyServer(sourceMDBGuid).ExtraFlags | LocalMailboxFlags.FolderMove | LocalMailboxFlags.Move;
			ISourceMailbox sourceMailbox = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				sourceMailbox = this.folderMover.GetSourceMailbox(new ADObjectId(sourceMDBGuid, PartitionId.LocalForest.ForestFQDN), sourceMbxFlags, mrsProxyCaps);
				disposeGuard.Add<ISourceMailbox>(sourceMailbox);
				sourceMailbox.Config(base.GetReservation(sourceMDBGuid, ReservationFlags.Read), base.CachedRequestJob.SourceExchangeGuid, base.CachedRequestJob.SourceExchangeGuid, CommonUtils.GetPartitionHint(base.CachedRequestJob.OrganizationId), sourceMDBGuid, MailboxType.SourceMailbox, null);
				disposeGuard.Success();
			}
			return sourceMailbox;
		}

		private IDestinationMailbox ConfigureDestinationMailbox()
		{
			List<MRSProxyCapabilities> mrsProxyCaps = new List<MRSProxyCapabilities>();
			Guid targetMDBGuid = base.CachedRequestJob.TargetMDBGuid;
			LocalMailboxFlags targetMbxFlags = CommonUtils.MapDatabaseToProxyServer(targetMDBGuid).ExtraFlags | LocalMailboxFlags.FolderMove | LocalMailboxFlags.Move;
			IDestinationMailbox destinationMailbox = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				destinationMailbox = this.folderMover.GetDestinationMailbox(targetMDBGuid, targetMbxFlags, mrsProxyCaps);
				disposeGuard.Add<IDestinationMailbox>(destinationMailbox);
				destinationMailbox.Config(base.GetReservation(targetMDBGuid, ReservationFlags.Write), base.CachedRequestJob.TargetExchangeGuid, base.CachedRequestJob.TargetExchangeGuid, CommonUtils.GetPartitionHint(base.CachedRequestJob.OrganizationId), targetMDBGuid, MailboxType.DestMailboxIntraOrg, null);
				disposeGuard.Success();
			}
			return destinationMailbox;
		}

		private MailboxWrapper ConfigureHierarchyMailbox(ISourceMailbox sourceMailbox, IDestinationMailbox destinationMailbox)
		{
			MailboxWrapper mailboxWrapper = null;
			string orgID = (base.CachedRequestJob.OrganizationId != null && base.CachedRequestJob.OrganizationId.OrganizationalUnit != null) ? (base.CachedRequestJob.OrganizationId.OrganizationalUnit.Name + "\\") : string.Empty;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				mailboxWrapper = new SourceMailboxWrapper(sourceMailbox, MailboxWrapperFlags.Source, MrsStrings.AuxFolderMoveTracingId(orgID, base.CachedRequestJob.SourceExchangeGuid));
				disposeGuard.Add<MailboxWrapper>(mailboxWrapper);
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
				byte[] sessionSpecificEntryId = this.folderMover.DestMailbox.GetSessionSpecificEntryId(array);
				using (IDestinationFolder folder = this.folderMover.DestMailbox.GetFolder(sessionSpecificEntryId))
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
						AuxFolderMoveJob.EmptyContents(folder);
						folder.SetRules(Array<RuleData>.Empty);
					}
				}
			}
		}

		private void UpdateSourceFolder(byte[] hierarchyFolderEntryId)
		{
			byte[] sessionSpecificEntryId = this.folderMover.SourceMailbox.GetSessionSpecificEntryId(hierarchyFolderEntryId);
			using (ISourceFolder folder = this.folderMover.SourceMailbox.GetFolder(sessionSpecificEntryId))
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
			byte[] sessionSpecificEntryId = this.folderMover.DestMailbox.GetSessionSpecificEntryId(hierarchyFolderEntryId);
			using (IDestinationFolder folder = this.folderMover.DestMailbox.GetFolder(sessionSpecificEntryId))
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
			}
		}

		private const int DefaultBatchSize = 100;

		private const int SizeOfGuid = 16;

		private PublicFolderMover folderMover;

		private List<byte[]> foldersToMove;
	}
}
