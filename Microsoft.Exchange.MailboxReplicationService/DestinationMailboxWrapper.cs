using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DestinationMailboxWrapper : MailboxWrapper, IDestinationMailbox, IMailbox, IDisposable
	{
		public DestinationMailboxWrapper(IDestinationMailbox mailbox, MailboxWrapperFlags flags, LocalizedString tracingId, params Guid[] syncStateKeyPrefixGuids) : base(mailbox, flags, tracingId)
		{
			Guid[] array = new Guid[syncStateKeyPrefixGuids.Length + 1];
			syncStateKeyPrefixGuids.CopyTo(array, 0);
			array[syncStateKeyPrefixGuids.Length] = DestinationMailboxWrapper.SyncStateKeySuffix;
			this.syncStateKey = DestinationMailboxWrapper.GetSyncStateKey(array);
			array[syncStateKeyPrefixGuids.Length] = DestinationMailboxWrapper.ICSSyncStateKeySuffix;
			this.icsSyncStateKey = DestinationMailboxWrapper.GetSyncStateKey(array);
			array[syncStateKeyPrefixGuids.Length] = DestinationMailboxWrapper.ReplaySyncStateKeySuffix;
			this.replaySyncStateKey = DestinationMailboxWrapper.GetSyncStateKey(array);
		}

		public IDestinationMailbox DestinationMailbox
		{
			get
			{
				return this;
			}
		}

		public PersistedSyncData SyncState { get; set; }

		public MailboxMapiSyncState ICSSyncState { get; set; }

		public ReplaySyncState ReplaySyncState { get; set; }

		public SyncStateError LoadSyncState(Guid expectedRequestGuid, ReportData report, SyncStateFlags flags)
		{
			MrsTracer.Service.Debug("Attempting to load saved synchronization state.", new object[0]);
			string text = this.DestinationMailbox.LoadSyncState(this.syncStateKey);
			if (new TestIntegration(false).InjectCorruptSyncState)
			{
				text = "Some corrupt sync state";
			}
			PersistedSyncData persistedSyncData;
			try
			{
				persistedSyncData = PersistedSyncData.Deserialize(text);
			}
			catch (UnableToDeserializeXMLException failure)
			{
				DestinationMailboxWrapper.StringSummary stringSummary = new DestinationMailboxWrapper.StringSummary(text);
				report.Append(MrsStrings.ReportSyncStateCorrupt(expectedRequestGuid, stringSummary.Length, stringSummary.Start, stringSummary.End), failure, ReportEntryFlags.Target);
				return SyncStateError.CorruptSyncState;
			}
			if (persistedSyncData == null)
			{
				MrsTracer.Service.Debug("Saved sync state does not exist.", new object[0]);
				report.Append(MrsStrings.ReportSyncStateNull(expectedRequestGuid));
				return SyncStateError.NullSyncState;
			}
			if (persistedSyncData.MoveRequestGuid != expectedRequestGuid)
			{
				MrsTracer.Service.Warning("Saved state message contains invalid data (move request guid does not match). Discarding saved state.", new object[0]);
				report.Append(MrsStrings.ReportSyncStateWrongRequestGuid(expectedRequestGuid, persistedSyncData.MoveRequestGuid));
				return SyncStateError.WrongRequestGuid;
			}
			MrsTracer.Service.Debug("Successfully parsed saved state: stage={0}", new object[]
			{
				persistedSyncData.SyncStage
			});
			this.SyncState = persistedSyncData;
			string text2 = this.DestinationMailbox.LoadSyncState(this.icsSyncStateKey);
			try
			{
				this.ICSSyncState = MailboxMapiSyncState.Deserialize(text2);
			}
			catch (UnableToDeserializeXMLException failure2)
			{
				DestinationMailboxWrapper.StringSummary stringSummary2 = new DestinationMailboxWrapper.StringSummary(text2);
				report.Append(MrsStrings.ReportIcsSyncStateCorrupt(expectedRequestGuid, stringSummary2.Length, stringSummary2.Start, stringSummary2.End), failure2, ReportEntryFlags.Target);
				return SyncStateError.CorruptIcsSyncState;
			}
			if (this.ICSSyncState == null)
			{
				report.Append(MrsStrings.ReportIcsSyncStateNull(expectedRequestGuid));
				return SyncStateError.NullIcsSyncState;
			}
			if (flags.HasFlag(SyncStateFlags.Replay))
			{
				string text3 = this.DestinationMailbox.LoadSyncState(this.replaySyncStateKey);
				try
				{
					this.ReplaySyncState = ReplaySyncState.Deserialize(text3);
				}
				catch (UnableToDeserializeXMLException failure3)
				{
					DestinationMailboxWrapper.StringSummary stringSummary3 = new DestinationMailboxWrapper.StringSummary(text3);
					report.Append(MrsStrings.ReportReplaySyncStateCorrupt(expectedRequestGuid, stringSummary3.Length, stringSummary3.Start, stringSummary3.End), failure3, ReportEntryFlags.Target);
					return SyncStateError.CorruptReplaySyncState;
				}
				if (this.ReplaySyncState == null)
				{
					report.Append(MrsStrings.ReportReplaySyncStateNull(expectedRequestGuid));
					return SyncStateError.NullReplaySyncState;
				}
				report.AppendDebug(MrsStrings.ReportSyncStateLoaded2(expectedRequestGuid, text.Length, text2.Length, text3.Length));
			}
			else
			{
				report.AppendDebug(MrsStrings.ReportSyncStateLoaded(expectedRequestGuid, text.Length, text2.Length));
			}
			return SyncStateError.None;
		}

		public void SaveSyncState()
		{
			this.DestinationMailbox.SaveSyncState(this.syncStateKey, this.SyncState.Serialize(false));
		}

		public void SaveICSSyncState(bool force)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			if (!force && utcNow < this.icsSyncStateUpdateTimestamp + BaseJob.FlushInterval)
			{
				return;
			}
			this.DestinationMailbox.SaveSyncState(this.icsSyncStateKey, this.ICSSyncState.Serialize(false));
			this.icsSyncStateUpdateTimestamp = utcNow;
		}

		public void SaveReplaySyncState()
		{
			this.DestinationMailbox.SaveSyncState(this.replaySyncStateKey, this.ReplaySyncState.Serialize(false));
		}

		public void ClearSyncState()
		{
			this.DestinationMailbox.SaveSyncState(this.syncStateKey, null);
			this.DestinationMailbox.SaveSyncState(this.icsSyncStateKey, null);
			this.DestinationMailbox.SaveSyncState(this.replaySyncStateKey, null);
		}

		protected override OperationSideDataContext SideOperationContext
		{
			get
			{
				return OperationSideDataContext.Target;
			}
		}

		public override IFolder GetFolder(byte[] folderId)
		{
			return this.DestinationMailbox.GetFolder(folderId);
		}

		public override void Clear()
		{
			base.Clear();
			this.SyncState = null;
			this.ICSSyncState = null;
		}

		bool IDestinationMailbox.MailboxExists()
		{
			bool result = false;
			base.CreateContext("IDestinationMailbox.MailboxExists", new DataContext[0]).Execute(delegate
			{
				result = ((IDestinationMailbox)this.WrappedObject).MailboxExists();
			}, true);
			return result;
		}

		CreateMailboxResult IDestinationMailbox.CreateMailbox(byte[] mailboxData, MailboxSignatureFlags sourceSignatureFlags)
		{
			CreateMailboxResult result = CreateMailboxResult.Success;
			base.CreateContext("IDestinationMailbox.CreateMailbox", new DataContext[]
			{
				new SimpleValueDataContext("MailboxData", TraceUtils.DumpBytes(mailboxData)),
				new SimpleValueDataContext("sourceSignatureFlags", sourceSignatureFlags)
			}).Execute(delegate
			{
				result = ((IDestinationMailbox)this.WrappedObject).CreateMailbox(mailboxData, sourceSignatureFlags);
			}, true);
			return result;
		}

		void IDestinationMailbox.ProcessMailboxSignature(byte[] mailboxData)
		{
			base.CreateContext("IDestinationMailbox.ProcessMailbox", new DataContext[]
			{
				new SimpleValueDataContext("MailboxData", TraceUtils.DumpBytes(mailboxData))
			}).Execute(delegate
			{
				((IDestinationMailbox)this.WrappedObject).ProcessMailboxSignature(mailboxData);
			}, true);
		}

		IDestinationFolder IDestinationMailbox.GetFolder(byte[] entryId)
		{
			IDestinationFolder result = null;
			base.CreateContext("IDestinationMailbox.GetFolder", new DataContext[]
			{
				new EntryIDsDataContext(entryId)
			}).Execute(delegate
			{
				result = ((IDestinationMailbox)this.WrappedObject).GetFolder(entryId);
			}, true);
			if (result == null)
			{
				return null;
			}
			return new DestinationFolderWrapper(result, base.CreateContext);
		}

		IFxProxy IDestinationMailbox.GetFxProxy()
		{
			IFxProxy result = null;
			base.CreateContext("IDestinationMailbox.GetFxProxy", new DataContext[0]).Execute(delegate
			{
				result = ((IDestinationMailbox)this.WrappedObject).GetFxProxy();
			}, true);
			if (result == null)
			{
				return null;
			}
			return new FxProxyWrapper(result, base.CreateContext);
		}

		PropProblemData[] IDestinationMailbox.SetProps(PropValueData[] pva)
		{
			PropProblemData[] result = null;
			base.CreateContext("IDestinationMailbox.SetProps", new DataContext[]
			{
				new PropValuesDataContext(pva)
			}).Execute(delegate
			{
				result = ((IDestinationMailbox)this.WrappedObject).SetProps(pva);
			}, true);
			return result;
		}

		IFxProxyPool IDestinationMailbox.GetFxProxyPool(ICollection<byte[]> folderIds)
		{
			IFxProxyPool result = null;
			base.CreateContext("IDestinationMailbox.GetFxProxyPool", new DataContext[]
			{
				new EntryIDsDataContext(new List<byte[]>(folderIds))
			}).Execute(delegate
			{
				result = ((IDestinationMailbox)this.WrappedObject).GetFxProxyPool(folderIds);
			}, true);
			if (result == null)
			{
				return null;
			}
			return new FxProxyPoolWrapper(result, base.CreateContext);
		}

		void IDestinationMailbox.CreateFolder(FolderRec sourceFolder, CreateFolderFlags createFolderFlags, out byte[] newFolderId)
		{
			byte[] newFolderIdInt = null;
			base.CreateContext("IDestinationMailbox.CreateFolder", new DataContext[]
			{
				new FolderRecDataContext(sourceFolder),
				new SimpleValueDataContext("CreateFolderFlags", createFolderFlags)
			}).Execute(delegate
			{
				((IDestinationMailbox)this.WrappedObject).CreateFolder(sourceFolder, createFolderFlags, out newFolderIdInt);
			}, true);
			newFolderId = newFolderIdInt;
		}

		void IDestinationMailbox.MoveFolder(byte[] folderId, byte[] oldParentId, byte[] newParentId)
		{
			base.CreateContext("IDestinationMailbox.MoveFolder", new DataContext[]
			{
				new EntryIDsDataContext(folderId),
				new SimpleValueDataContext("OldParentId", TraceUtils.DumpEntryId(oldParentId)),
				new SimpleValueDataContext("NewParentId", TraceUtils.DumpEntryId(newParentId))
			}).Execute(delegate
			{
				((IDestinationMailbox)this.WrappedObject).MoveFolder(folderId, oldParentId, newParentId);
			}, true);
		}

		void IDestinationMailbox.DeleteFolder(FolderRec folderRec)
		{
			base.CreateContext("IDestinationMailbox.DeleteFolder", new DataContext[]
			{
				new FolderRecDataContext(folderRec)
			}).Execute(delegate
			{
				((IDestinationMailbox)this.WrappedObject).DeleteFolder(folderRec);
			}, true);
		}

		void IDestinationMailbox.SetMailboxSecurityDescriptor(RawSecurityDescriptor sd)
		{
			base.CreateContext("IDestinationMailbox.SetMailboxSecurityDescriptor", new DataContext[]
			{
				new SimpleValueDataContext("SD", CommonUtils.GetSDDLString(sd))
			}).Execute(delegate
			{
				((IDestinationMailbox)this.WrappedObject).SetMailboxSecurityDescriptor(sd);
			}, true);
		}

		void IDestinationMailbox.SetUserSecurityDescriptor(RawSecurityDescriptor sd)
		{
			base.CreateContext("IDestinationMailbox.SetUserSecurityDescriptor", new DataContext[]
			{
				new SimpleValueDataContext("SD", CommonUtils.GetSDDLString(sd))
			}).Execute(delegate
			{
				((IDestinationMailbox)this.WrappedObject).SetUserSecurityDescriptor(sd);
			}, true);
		}

		void IDestinationMailbox.PreFinalSyncDataProcessing(int? sourceMailboxVersion)
		{
			base.CreateContext("IDestinationMailbox.PreFinalSyncDataProcessing", new DataContext[0]).Execute(delegate
			{
				((IDestinationMailbox)this.WrappedObject).PreFinalSyncDataProcessing(sourceMailboxVersion);
			}, true);
		}

		ConstraintCheckResultType IDestinationMailbox.CheckDataGuarantee(DateTime commitTimestamp, out LocalizedString failureReason)
		{
			ConstraintCheckResultType result = ConstraintCheckResultType.Satisfied;
			LocalizedString failureReasonInt = LocalizedString.Empty;
			base.CreateContext("IDestinationMailbox.CheckDataGuarantee", new DataContext[]
			{
				new SimpleValueDataContext("CommitTimestamp", commitTimestamp)
			}).Execute(delegate
			{
				result = ((IDestinationMailbox)this.WrappedObject).CheckDataGuarantee(commitTimestamp, out failureReasonInt);
			}, true);
			failureReason = failureReasonInt;
			return result;
		}

		void IDestinationMailbox.ForceLogRoll()
		{
			base.CreateContext("IDestinationMailbox.ForceLogRoll", new DataContext[0]).Execute(delegate
			{
				((IDestinationMailbox)base.WrappedObject).ForceLogRoll();
			}, true);
		}

		List<ReplayAction> IDestinationMailbox.GetActions(string replaySyncState, int maxNumberOfActions)
		{
			List<ReplayAction> result = null;
			base.CreateContext("IDestinationMailbox.GetActions", new DataContext[]
			{
				new SimpleValueDataContext("ReplaySyncState", replaySyncState),
				new SimpleValueDataContext("MaxNumberOfActions", maxNumberOfActions)
			}).Execute(delegate
			{
				result = ((IDestinationMailbox)this.WrappedObject).GetActions(replaySyncState, maxNumberOfActions);
			}, true);
			return result;
		}

		void IDestinationMailbox.SetMailboxSettings(ItemPropertiesBase item)
		{
			base.CreateContext("IDestinationMailbox.SetMailboxSettings", new DataContext[]
			{
				new SimpleValueDataContext("SetMailboxSettings", item.GetType())
			}).Execute(delegate
			{
				((IDestinationMailbox)this.WrappedObject).SetMailboxSettings(item);
			}, true);
		}

		private static byte[] GetSyncStateKey(Guid[] syncStateKeyGuids)
		{
			byte[] array = new byte[16 * syncStateKeyGuids.Length];
			for (int i = 0; i < syncStateKeyGuids.Length; i++)
			{
				syncStateKeyGuids[i].ToByteArray().CopyTo(array, 16 * i);
			}
			return array;
		}

		private static readonly Guid SyncStateKeySuffix = new Guid("4f91cae0-9566-4c62-9889-2fa527aaa91e");

		private static readonly Guid ICSSyncStateKeySuffix = new Guid("b440559b-375b-4c77-ab84-806c1964b598");

		private static readonly Guid ReplaySyncStateKeySuffix = new Guid("2F9A4D65-3CA2-46CB-BE2C-59190E21D8F1");

		private byte[] syncStateKey;

		private byte[] icsSyncStateKey;

		private byte[] replaySyncStateKey;

		private ExDateTime icsSyncStateUpdateTimestamp;

		private class StringSummary
		{
			public int Length { get; private set; }

			public string Start { get; private set; }

			public string End { get; private set; }

			public StringSummary(string str)
			{
				if (str == null)
				{
					return;
				}
				this.Length = str.Length;
				if (str.Length <= 100)
				{
					this.End = str;
					this.Start = str;
					return;
				}
				this.Start = str.Substring(0, 100);
				this.End = str.Substring(str.Length - 100 - 1, 100);
			}

			private const int StartEndLength = 100;
		}
	}
}
