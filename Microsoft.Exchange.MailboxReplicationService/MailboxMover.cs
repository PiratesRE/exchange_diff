using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MailboxMover : MailboxCopierBase
	{
		public MailboxMover(TransactionalRequestJob requestJob, BaseJob mrsJob, MailboxCopierFlags mbxCopierFlags, LocalizedString tracingID, Guid mailboxGuid) : this(requestJob, mrsJob, mbxCopierFlags, tracingID, mailboxGuid, CommonUtils.GetPartitionHint(requestJob.OrganizationId))
		{
		}

		public MailboxMover(TransactionalRequestJob requestJob, BaseJob mrsJob, MailboxCopierFlags mbxCopierFlags, LocalizedString tracingID, Guid mailboxGuid, TenantPartitionHint partitionHintToUse) : base(mailboxGuid, mailboxGuid, requestJob, mrsJob, mbxCopierFlags, tracingID, tracingID)
		{
			this.partitionHintToUse = partitionHintToUse;
		}

		public override void ConfigureProviders()
		{
			base.ConfigureProviders();
			RequestStatisticsBase cachedRequestJob = base.MRSJob.CachedRequestJob;
			LocalMailboxFlags localMailboxFlags = LocalMailboxFlags.Move;
			List<MRSProxyCapabilities> list = new List<MRSProxyCapabilities>();
			if (cachedRequestJob.IgnoreRuleLimitErrors)
			{
				localMailboxFlags |= LocalMailboxFlags.StripLargeRulesForDownlevelTargets;
			}
			if (cachedRequestJob.IsSplitPrimaryAndArchive)
			{
				list.Add(MRSProxyCapabilities.ArchiveSeparation);
			}
			if (cachedRequestJob.TargetContainerGuid != null)
			{
				list.Add(MRSProxyCapabilities.ContainerOperations);
			}
			LocalMailboxFlags localMailboxFlags2 = LocalMailboxFlags.Move;
			if (base.Flags.HasFlag(MailboxCopierFlags.ContainerAggregated))
			{
				localMailboxFlags |= LocalMailboxFlags.AggregatedMailbox;
				localMailboxFlags2 |= LocalMailboxFlags.AggregatedMailbox;
			}
			if (cachedRequestJob.RequestType == MRSRequestType.Move && base.IsRoot && base.IsPrimary && base.TargetMailboxContainerGuid != null)
			{
				localMailboxFlags2 |= LocalMailboxFlags.CreateNewPartition;
			}
			ADObjectId adobjectId = base.IsRoot ? cachedRequestJob.SourceDatabase : cachedRequestJob.SourceArchiveDatabase;
			if (base.Flags.HasFlag(MailboxCopierFlags.SourceIsArchive) && cachedRequestJob.JobType >= MRSJobType.RequestJobE14R5_PrimaryOrArchiveExclusiveMoves)
			{
				MrsTracer.Service.Debug("Overwriting archive guid in Archive MailboxMover...", new object[0]);
				ADObjectId adobjectId2;
				if ((adobjectId2 = cachedRequestJob.SourceArchiveDatabase) == null)
				{
					adobjectId2 = new ADObjectId(cachedRequestJob.RemoteArchiveDatabaseGuid ?? Guid.Empty);
				}
				adobjectId = adobjectId2;
				base.SourceMdbGuid = adobjectId.ObjectGuid;
				base.DestMdbGuid = ((cachedRequestJob.TargetArchiveDatabase != null) ? cachedRequestJob.TargetArchiveDatabase.ObjectGuid : (cachedRequestJob.RemoteArchiveDatabaseGuid ?? Guid.Empty));
			}
			ISourceMailbox sourceMailbox = this.GetSourceMailbox(adobjectId, localMailboxFlags, list);
			IDestinationMailbox destinationMailbox = this.GetDestinationMailbox(base.DestMdbGuid, localMailboxFlags2, list);
			if (cachedRequestJob.RequestStyle == RequestStyle.CrossOrg && (cachedRequestJob.Flags & RequestFlags.RemoteLegacy) != RequestFlags.None)
			{
				if (cachedRequestJob.Direction == RequestDirection.Push)
				{
					destinationMailbox.ConfigADConnection(cachedRequestJob.TargetDCName, cachedRequestJob.TargetDCName, cachedRequestJob.TargetCredential);
				}
				else
				{
					sourceMailbox.ConfigADConnection(cachedRequestJob.SourceDCName, cachedRequestJob.SourceDCName, cachedRequestJob.SourceCredential);
				}
			}
			TenantPartitionHint partitionHint = (cachedRequestJob.SourceIsLocal || cachedRequestJob.CrossResourceForest) ? this.partitionHintToUse : null;
			TenantPartitionHint partitionHint2 = (cachedRequestJob.TargetIsLocal || cachedRequestJob.CrossResourceForest) ? this.partitionHintToUse : null;
			if (base.MRSJob.TestIntegration.RemoteExchangeGuidOverride != Guid.Empty)
			{
				if (!cachedRequestJob.SourceIsLocal)
				{
					partitionHint = this.partitionHintToUse;
				}
				if (!cachedRequestJob.TargetIsLocal)
				{
					partitionHint2 = this.partitionHintToUse;
				}
			}
			Guid primaryMailboxGuid = base.Flags.HasFlag(MailboxCopierFlags.ContainerOrg) ? base.SourceMailboxGuid : cachedRequestJob.ExchangeGuid;
			sourceMailbox.Config(base.MRSJob.GetReservation(base.SourceMdbGuid, ReservationFlags.Read), primaryMailboxGuid, base.SourceMailboxGuid, partitionHint, base.SourceMdbGuid, MailboxType.SourceMailbox, null);
			destinationMailbox.Config(base.MRSJob.GetReservation(base.DestMdbGuid, ReservationFlags.Write), primaryMailboxGuid, base.TargetMailboxGuid, partitionHint2, base.DestMdbGuid, (cachedRequestJob.RequestStyle == RequestStyle.CrossOrg) ? MailboxType.DestMailboxCrossOrg : MailboxType.DestMailboxIntraOrg, base.TargetMailboxContainerGuid);
			base.Config(sourceMailbox, destinationMailbox);
		}

		public void FinalSyncCopyMailboxData()
		{
			if (base.MRSJob.TestIntegration.RemoteExchangeGuidOverride == Guid.Empty)
			{
				using (IFxProxy fxProxy = base.DestMailbox.GetFxProxy())
				{
					using (IFxProxy fxProxy2 = base.CreateFxProxyTransmissionPipeline(fxProxy))
					{
						base.SourceMailbox.CopyTo(fxProxy2, new PropTag[]
						{
							PropTag.ContainerHierarchy,
							PropTag.ContainerContents
						});
					}
				}
				if (this.ServerSupportsInferencePropertiesMove(base.SourceMailboxWrapper.MailboxVersion) && this.ServerSupportsInferencePropertiesMove(base.DestMailboxWrapper.MailboxVersion) && base.MRSJob.GetConfig<bool>("CopyInferenceProperties"))
				{
					byte[] badItemId = BitConverter.GetBytes(base.SourceMailbox.GetHashCode());
					CommonUtils.ProcessKnownExceptions(delegate
					{
						List<PropValueData> list = new List<PropValueData>(2);
						foreach (PropValueData propValueData in this.SourceMailbox.GetProps(MailboxMover.inferencePropertiesToMove))
						{
							if (((PropTag)propValueData.PropTag).ValueType() != PropType.Error)
							{
								list.Add(propValueData);
							}
						}
						badItemId = BadMessageRec.ComputeKey(list.ToArray());
						if (!this.SyncState.BadItems.ContainsKey(badItemId))
						{
							this.DestMailbox.SetProps(list.ToArray());
						}
					}, delegate(Exception failure)
					{
						if (MapiUtils.IsBadItemIndicator(failure))
						{
							List<BadMessageRec> list = new List<BadMessageRec>(1);
							list.Add(BadMessageRec.InferenceData(failure, badItemId));
							this.ReportBadItems(list);
							return true;
						}
						return false;
					});
				}
			}
			if (base.SupportsPerUserReadUnreadDataTransfer)
			{
				base.Report.Append(MrsStrings.ReportCopyPerUserReadUnreadDataStarted);
				using (ISourceFolder folder = base.SourceMailbox.GetFolder(null))
				{
					using (IDestinationFolder folder2 = base.DestMailbox.GetFolder(null))
					{
						using (IFxProxy fxProxy3 = folder2.GetFxProxy(FastTransferFlags.PassThrough))
						{
							using (IFxProxy fxProxy4 = base.CreateFxProxyTransmissionPipeline(fxProxy3))
							{
								folder.CopyTo(fxProxy4, CopyPropertiesFlags.CopyMailboxPerUserData, Array<PropTag>.Empty);
							}
						}
					}
				}
				base.Report.Append(MrsStrings.ReportCopyPerUserReadUnreadDataCompleted);
			}
		}

		public bool CompareRootFolders()
		{
			ISourceFolder folder = base.SourceMailbox.GetFolder(null);
			if (folder == null)
			{
				throw new RootFolderNotFoundPermananentException();
			}
			FolderRec folderRec;
			using (folder)
			{
				folderRec = folder.GetFolderRec(null, GetFolderRecFlags.None);
			}
			IDestinationFolder folder2 = base.DestMailbox.GetFolder(null);
			if (folder2 == null)
			{
				throw new RootFolderNotFoundPermananentException();
			}
			FolderRec folderRec2;
			using (folder2)
			{
				folderRec2 = folder2.GetFolderRec(null, GetFolderRecFlags.None);
			}
			if (!CommonUtils.IsSameEntryId(folderRec.EntryId, folderRec2.EntryId))
			{
				MrsTracer.Service.Warning("Root folder IDs don't match on source and destination mailboxes. Restarting the move.", new object[0]);
				return false;
			}
			return true;
		}

		public void GetSignatureModeInfo(bool jobIsPreservingMailboxSignature, out bool serverSupportsSignaturePreservationCheck, out bool serverIsInSignaturePreservingMode, out bool mrsIsInSignaturePreservingMode)
		{
			MrsTracer.Service.Debug("GetSignatureModeInfo().", new object[0]);
			serverSupportsSignaturePreservationCheck = false;
			serverIsInSignaturePreservingMode = jobIsPreservingMailboxSignature;
			mrsIsInSignaturePreservingMode = jobIsPreservingMailboxSignature;
			PropValue[] native = DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(base.DestMailbox.GetProps(new PropTag[]
			{
				PropTag.PreservingMailboxSignature,
				PropTag.MRSPreservingMailboxSignature
			}));
			if (native != null)
			{
				for (int i = 0; i < 2; i++)
				{
					if (!native[i].IsNull())
					{
						if (PropTag.PreservingMailboxSignature == native[i].PropTag && native[i].Value != null && native[i].Value is bool)
						{
							MrsTracer.Service.Debug("Retrieved StorePreservingSignature PropTag.", new object[0]);
							serverIsInSignaturePreservingMode = (bool)native[i].Value;
							serverSupportsSignaturePreservationCheck = true;
						}
						if (PropTag.MRSPreservingMailboxSignature == native[i].PropTag && native[i].Value != null && native[i].Value is bool)
						{
							MrsTracer.Service.Debug("Retrieved MrsPreservingSignature PropTag.", new object[0]);
							mrsIsInSignaturePreservingMode = (bool)native[i].Value;
							jobIsPreservingMailboxSignature &= (!base.Flags.HasFlag(MailboxCopierFlags.ContainerAggregated) && !base.Flags.HasFlag(MailboxCopierFlags.ContainerOrg));
						}
					}
				}
			}
		}

		public void UpdateMappingMetadata(bool jobIsPreservingMailboxSignature)
		{
			MrsTracer.Service.Debug("UpdateMappingMetadata().", new object[0]);
			bool flag;
			bool flag2;
			bool flag3;
			this.GetSignatureModeInfo(jobIsPreservingMailboxSignature, out flag, out flag2, out flag3);
			if (flag3 && !flag2)
			{
				MrsTracer.Service.Debug("Attempting incremental move restart for: {0}", new object[]
				{
					base.MRSJob.RequestJobIdentity
				});
				if (base.DestMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion >= 104U && base.SourceMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion == base.DestMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion)
				{
					byte[] mailboxBasicInfo = base.SourceMailbox.GetMailboxBasicInfo(MailboxSignatureFlags.GetMappingMetadata);
					MrsTracer.Service.Debug("SourceMailbox.GetMailboxBasicInfo({0}) succeeded for {1}.", new object[]
					{
						MailboxSignatureFlags.GetMappingMetadata,
						base.MRSJob.RequestJobIdentity
					});
					base.DestMailbox.ProcessMailboxSignature(mailboxBasicInfo);
					MrsTracer.Service.Debug("DestMailbox.ProcessMailboxSignature() succeeded for {0}.", new object[]
					{
						base.MRSJob.RequestJobIdentity
					});
				}
			}
		}

		public void VerifyAndUpdateDestinationMailboxSignature(bool jobIsPreservingMailboxSignature)
		{
			bool flag;
			bool serverIsInSignaturePreservingMode;
			bool mrsIsInSignaturePreservingMode;
			this.GetSignatureModeInfo(jobIsPreservingMailboxSignature, out flag, out serverIsInSignaturePreservingMode, out mrsIsInSignaturePreservingMode);
			this.VerifyAndUpdateDestinationMailboxSignature(serverIsInSignaturePreservingMode, mrsIsInSignaturePreservingMode);
		}

		public void FinalizeStoreSignaturePreservingMailboxMove(bool jobIsPreservingMailboxSignature)
		{
			bool serverSupportsMailboxSignatureProperty;
			bool serverIsInSignaturePreservingMode;
			bool mrsIsInSignaturePreservingMode;
			this.GetSignatureModeInfo(jobIsPreservingMailboxSignature, out serverSupportsMailboxSignatureProperty, out serverIsInSignaturePreservingMode, out mrsIsInSignaturePreservingMode);
			this.FinalizeStoreSignaturePreservingMailboxMove(serverIsInSignaturePreservingMode, mrsIsInSignaturePreservingMode, serverSupportsMailboxSignatureProperty);
		}

		public void CopyHighWatermark(bool jobIsPreservingMailboxSignature)
		{
			MrsTracer.Service.Debug("Copying HighWatermark value for the logon", new object[0]);
			bool serverSupportsMailboxSignatureProperty;
			bool serverIsInSignaturePreservingMode;
			bool mrsIsInSignaturePreservingMode;
			this.GetSignatureModeInfo(jobIsPreservingMailboxSignature, out serverSupportsMailboxSignatureProperty, out serverIsInSignaturePreservingMode, out mrsIsInSignaturePreservingMode);
			this.VerifyAndUpdateDestinationMailboxSignature(serverIsInSignaturePreservingMode, mrsIsInSignaturePreservingMode);
			this.FinalizeStoreSignaturePreservingMailboxMove(serverIsInSignaturePreservingMode, mrsIsInSignaturePreservingMode, serverSupportsMailboxSignatureProperty);
		}

		private void FinalizeStoreSignaturePreservingMailboxMove(bool serverIsInSignaturePreservingMode, bool mrsIsInSignaturePreservingMode, bool serverSupportsMailboxSignatureProperty)
		{
			if (serverIsInSignaturePreservingMode)
			{
				MrsTracer.Service.Debug("Finalize Store mailbox signature preserving mailbox move.", new object[0]);
				PropValueData[] props = base.SourceMailbox.GetProps(new PropTag[]
				{
					PropTag.NextLocalId
				});
				PropProblemData[] array = base.DestMailbox.SetProps(props);
				if (array != null)
				{
					throw new MailboxSignatureChangedTransientException();
				}
				MrsTracer.Service.Debug("Store mailbox signature preserving mailbox move has been finalized.", new object[0]);
			}
			if (serverSupportsMailboxSignatureProperty)
			{
				PropValueData[] props2 = base.DestMailbox.GetProps(new PropTag[]
				{
					PropTag.PreservingMailboxSignature
				});
				if (1722286091 != props2[0].PropTag || (bool)props2[0].Value)
				{
					throw new UnexpectedErrorPermanentException(44);
				}
				base.MRSJob.Report.Append(MrsStrings.ReportStoreMailboxHasFinalized);
				MrsTracer.Service.Debug("Store mailbox signature preserving mailbox move was finalized for: {0}.", new object[]
				{
					base.MRSJob.RequestJobIdentity
				});
			}
		}

		private void VerifyAndUpdateDestinationMailboxSignature(bool serverIsInSignaturePreservingMode, bool mrsIsInSignaturePreservingMode)
		{
			if (serverIsInSignaturePreservingMode && base.DestMailboxWrapper.LastConnectedServerInfo.MailboxSignatureVersion >= 103U)
			{
				byte[] mailboxBasicInfo = base.SourceMailbox.GetMailboxBasicInfo(MailboxSignatureFlags.GetNamedPropertyMapping | MailboxSignatureFlags.GetReplidGuidMapping);
				MrsTracer.Service.Debug("SourceMailbox.GetMailboxBasicInfo({0}) succeeded for {1}.", new object[]
				{
					MailboxSignatureFlags.GetNamedPropertyMapping | MailboxSignatureFlags.GetReplidGuidMapping,
					base.MRSJob.RequestJobIdentity
				});
				base.DestMailbox.ProcessMailboxSignature(mailboxBasicInfo);
				MrsTracer.Service.Debug("DestMailbox.ProcessMailboxSignature() succeeded for {0}.", new object[]
				{
					base.MRSJob.RequestJobIdentity
				});
			}
		}

		private bool ServerSupportsInferencePropertiesMove(int? serverVersion)
		{
			return serverVersion != null && serverVersion.Value >= MailboxMover.minimumSupportedVersionForInferenceProperties.ToInt();
		}

		private static readonly ServerVersion minimumSupportedVersionForInferenceProperties = new ServerVersion(15, 0, 745, 0);

		private static readonly PropTag[] inferencePropertiesToMove = new PropTag[]
		{
			PropTag.InferenceTrainedModelVersionBreadCrumb,
			PropTag.InferenceUserCapabilityFlags
		};

		private readonly TenantPartitionHint partitionHintToUse;
	}
}
