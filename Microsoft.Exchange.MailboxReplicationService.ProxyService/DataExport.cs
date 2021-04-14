using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DataExport : DisposeTrackableBase, IDataExport, IDataImport, IDisposable
	{
		public DataExport(IDataMessage getDataResponseMsg, MailboxReplicationProxyService service)
		{
			this.getDataResponseMsg = getDataResponseMsg;
			this.nextOpcode = DataMessageOpcode.None;
			this.nextBuffer = null;
			this.result = DataExport.DataExportResult.Done;
			this.exportFailure = null;
			this.exportThread = null;
			this.eventDataAvailable = new AutoResetEvent(false);
			this.eventDataProcessed = new AutoResetEvent(false);
			this.cancelExport = false;
			this.service = service;
			this.lastReturnWasTimeout = false;
			this.storedOpcode = 0;
			this.storedData = null;
		}

		DataExportBatch IDataExport.ExportData()
		{
			DataExportBatch dataExportBatch = null;
			int opcode;
			byte[] data;
			for (;;)
			{
				DataExport.DataExportResult dataExportResult;
				if (this.storedOpcode != 0)
				{
					dataExportResult = DataExport.DataExportResult.MoreData;
					opcode = this.storedOpcode;
					data = this.storedData;
					this.storedOpcode = 0;
					this.storedData = null;
				}
				else
				{
					dataExportResult = this.GetNextOutput(out opcode, out data);
				}
				bool flag = dataExportBatch == null;
				if (flag)
				{
					dataExportBatch = new DataExportBatch();
				}
				switch (dataExportResult)
				{
				case DataExport.DataExportResult.Done:
					goto IL_B2;
				case DataExport.DataExportResult.MoreData:
					if (!flag)
					{
						goto Block_4;
					}
					dataExportBatch.Opcode = opcode;
					dataExportBatch.Data = data;
					break;
				case DataExport.DataExportResult.Flush:
					dataExportBatch.FlushAfterImport = true;
					break;
				case DataExport.DataExportResult.Timeout:
					goto IL_88;
				}
			}
			Block_4:
			this.storedOpcode = opcode;
			this.storedData = data;
			return dataExportBatch;
			IL_88:
			MrsTracer.ProxyService.Warning("ExportThread appears to be stuck in a call. Returning to prevent WCF call timeout. Will continue waiting on the next call.", new object[0]);
			dataExportBatch.FlushAfterImport = true;
			return dataExportBatch;
			IL_B2:
			dataExportBatch.IsLastBatch = true;
			return dataExportBatch;
		}

		void IDataExport.CancelExport()
		{
			MrsTracer.ProxyService.Function("DataExport.CancelExport", new object[0]);
			if (this.exportThread == null)
			{
				MrsTracer.ProxyService.Warning("CancelExport should not be called after export was finished.", new object[0]);
				return;
			}
			this.cancelExport = true;
			this.eventDataProcessed.Set();
			this.exportThread.Join();
			this.exportThread = null;
		}

		void IDataImport.SendMessage(IDataMessage message)
		{
			MrsTracer.ProxyService.Function("DataExport.SendMessage", new object[0]);
			this.CheckForCancel();
			message.Serialize(this.service.UseCompression, out this.nextOpcode, out this.nextBuffer);
			this.result = DataExport.DataExportResult.MoreData;
			this.eventDataAvailable.Set();
			this.WaitForTheNextCall();
		}

		IDataMessage IDataImport.SendMessageAndWaitForReply(IDataMessage message)
		{
			MrsTracer.ProxyService.Function("DataExport.SendMessageAndWaitForReply", new object[0]);
			if (message is FlushMessage)
			{
				this.CheckForCancel();
				this.result = DataExport.DataExportResult.Flush;
				this.eventDataAvailable.Set();
				this.WaitForTheNextCall();
				return null;
			}
			if (message is FxProxyGetObjectDataRequestMessage && this.getDataResponseMsg is FxProxyGetObjectDataResponseMessage)
			{
				return this.getDataResponseMsg;
			}
			if (message is FxProxyPoolGetFolderDataRequestMessage && this.getDataResponseMsg is FxProxyPoolGetFolderDataResponseMessage)
			{
				return this.getDataResponseMsg;
			}
			throw new UnexpectedErrorPermanentException(-2147024809);
		}

		public void FolderExport(ISourceFolder folder, CopyPropertiesFlags flags, PropTag[] excludeTags)
		{
			this.RunExportThread(delegate
			{
				using (BufferedTransmitter bufferedTransmitter = new BufferedTransmitter(this, this.service.ExportBufferSizeFromMrsKB, false, this.service.UseBufferring, this.service.UseCompression))
				{
					using (AsynchronousTransmitter asynchronousTransmitter = new AsynchronousTransmitter(bufferedTransmitter, false))
					{
						using (FxProxyTransmitter fxProxyTransmitter = new FxProxyTransmitter(asynchronousTransmitter, false))
						{
							folder.CopyTo(fxProxyTransmitter, flags, excludeTags);
						}
					}
				}
			});
		}

		public void FoldersExport(ISourceMailbox mailbox, List<byte[]> folderIds, ExportFoldersDataToCopyFlags exportFoldersDataToCopyFlags, GetFolderRecFlags folderRecFlags, PropTag[] additionalFolderRecProps, CopyPropertiesFlags copyPropertiesFlags, PropTag[] excludeProps, AclFlags extendedAclFlags)
		{
			this.RunExportThread(delegate
			{
				using (BufferedTransmitter bufferedTransmitter = new BufferedTransmitter(this, this.service.ExportBufferSizeFromMrsKB, false, this.service.UseBufferring, this.service.UseCompression))
				{
					using (AsynchronousTransmitter asynchronousTransmitter = new AsynchronousTransmitter(bufferedTransmitter, false))
					{
						using (FxProxyPoolTransmitter fxProxyPoolTransmitter = new FxProxyPoolTransmitter(asynchronousTransmitter, false, this.service.ClientVersion))
						{
							mailbox.ExportFolders(folderIds, fxProxyPoolTransmitter, exportFoldersDataToCopyFlags, folderRecFlags, additionalFolderRecProps, copyPropertiesFlags, excludeProps, extendedAclFlags);
						}
					}
				}
			});
		}

		public void FolderExportMessages(ISourceFolder folder, CopyMessagesFlags flags, byte[][] entryIds)
		{
			this.RunExportThread(delegate
			{
				using (BufferedTransmitter bufferedTransmitter = new BufferedTransmitter(this, this.service.ExportBufferSizeFromMrsKB, false, this.service.UseBufferring, this.service.UseCompression))
				{
					using (AsynchronousTransmitter asynchronousTransmitter = new AsynchronousTransmitter(bufferedTransmitter, false))
					{
						using (FxProxyTransmitter fxProxyTransmitter = new FxProxyTransmitter(asynchronousTransmitter, false))
						{
							folder.ExportMessages(fxProxyTransmitter, flags, entryIds);
						}
					}
				}
			});
		}

		public void MailboxExport(ISourceMailbox mailbox, PropTag[] excludeTags)
		{
			this.RunExportThread(delegate
			{
				using (BufferedTransmitter bufferedTransmitter = new BufferedTransmitter(this, this.service.ExportBufferSizeFromMrsKB, false, this.service.UseBufferring, this.service.UseCompression))
				{
					using (AsynchronousTransmitter asynchronousTransmitter = new AsynchronousTransmitter(bufferedTransmitter, false))
					{
						using (FxProxyTransmitter fxProxyTransmitter = new FxProxyTransmitter(asynchronousTransmitter, false))
						{
							mailbox.CopyTo(fxProxyTransmitter, excludeTags);
						}
					}
				}
			});
		}

		public void MessageExport(ISourceMailbox mailbox, List<MessageRec> messages, ExportMessagesFlags flags, PropTag[] excludeProps)
		{
			this.RunExportThread(delegate
			{
				using (BufferedTransmitter bufferedTransmitter = new BufferedTransmitter(this, this.service.ExportBufferSizeFromMrsKB, false, this.service.UseBufferring, this.service.UseCompression))
				{
					using (AsynchronousTransmitter asynchronousTransmitter = new AsynchronousTransmitter(bufferedTransmitter, false))
					{
						using (FxProxyPoolTransmitter fxProxyPoolTransmitter = new FxProxyPoolTransmitter(asynchronousTransmitter, false, this.service.ClientVersion))
						{
							mailbox.ExportMessages(messages, fxProxyPoolTransmitter, flags, null, excludeProps);
						}
					}
				}
			});
		}

		public void MessageExportWithBadMessageDetection(ISourceMailbox mailbox, List<MessageRec> messages, ExportMessagesFlags flags, PropTag[] excludeProps, bool isDownlevelClient)
		{
			this.RunExportThread(delegate
			{
				List<BadMessageRec> list = new List<BadMessageRec>();
				MapiUtils.ExportMessagesWithBadItemDetection(mailbox, messages, delegate
				{
					BufferedTransmitter destination = new BufferedTransmitter(this, this.service.ExportBufferSizeFromMrsKB, false, this.service.UseBufferring, this.service.UseCompression);
					AsynchronousTransmitter destination2 = new AsynchronousTransmitter(destination, true);
					return new FxProxyPoolTransmitter(destination2, true, this.service.ClientVersion);
				}, flags, null, excludeProps, TestIntegration.Instance, ref list);
				if (list != null && list.Count > 0)
				{
					MessageExportResultTransmitter messageExportResultTransmitter = new MessageExportResultTransmitter(this, isDownlevelClient);
					messageExportResultTransmitter.SendMessageExportResults(list);
					((IDataImport)this).SendMessageAndWaitForReply(FlushMessage.Instance);
				}
			});
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.exportThread != null)
				{
					MrsTracer.ProxyService.Warning("Disposing DataExport while ExportThread is still active.", new object[0]);
					this.cancelExport = true;
					this.eventDataProcessed.Set();
					this.exportThread.Join();
					this.exportThread = null;
				}
				this.eventDataAvailable.Close();
				this.eventDataProcessed.Close();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DataExport>(this);
		}

		private DataExport.DataExportResult GetNextOutput(out int opcode, out byte[] data)
		{
			MrsTracer.ProxyService.Function("DataExport.GetNextOutput", new object[0]);
			opcode = 0;
			data = null;
			if (this.exportThread == null)
			{
				MrsTracer.ProxyService.Error("ExportData should not be called after the export has finished.", new object[0]);
				throw new UnexpectedErrorPermanentException(-2147024809);
			}
			if (!this.lastReturnWasTimeout)
			{
				this.nextOpcode = DataMessageOpcode.None;
				this.nextBuffer = null;
				this.result = DataExport.DataExportResult.Done;
				this.eventDataProcessed.Set();
			}
			if (!CommonUtils.SafeWaitWithTimeout(this.eventDataAvailable, this.exportThread, DataExport.maxCallLength))
			{
				this.lastReturnWasTimeout = true;
				return DataExport.DataExportResult.Timeout;
			}
			this.lastReturnWasTimeout = false;
			if (this.result == DataExport.DataExportResult.MoreData)
			{
				opcode = (int)this.nextOpcode;
				data = this.nextBuffer;
			}
			if (this.result == DataExport.DataExportResult.Done)
			{
				this.exportThread.Join();
				this.exportThread = null;
			}
			if (this.exportFailure != null)
			{
				MrsTracer.ProxyService.Warning("Export failed:\n{0}\n{1}\nContext: {2}", new object[]
				{
					CommonUtils.FullExceptionMessage(this.exportFailure),
					this.exportFailure.StackTrace,
					ExecutionContext.GetDataContext(this.exportFailure)
				});
				ExecutionContext.StampCurrentDataContext(this.exportFailure);
				throw this.exportFailure;
			}
			return this.result;
		}

		private void CheckForCancel()
		{
			if (this.cancelExport)
			{
				MrsTracer.ProxyService.Warning("Export was canceled. Throwing exception to cancel Export routine", new object[0]);
				throw new DataExportPermanentException(new DataExportCanceledPermanentException());
			}
		}

		private void WaitForTheNextCall()
		{
			if (!this.eventDataProcessed.WaitOne(MRSProxyConfiguration.Instance.DataImportTimeout))
			{
				throw new DataExportTransientException(new DataExportTimeoutTransientException());
			}
		}

		private void RunExportThread(Action threadOperation)
		{
			this.traceActivityID = MrsTracer.ActivityID;
			this.configContexts = SettingsContextBase.GetCurrentContexts();
			this.exportThread = new Thread(new ParameterizedThreadStart(this.ExportThread));
			this.exportThread.Name = "DataExport Thread";
			this.exportThread.Start(threadOperation);
		}

		private void ExportThread(object context)
		{
			MrsTracer.ActivityID = this.traceActivityID;
			Action exportOperation = (Action)context;
			CommonUtils.CatchKnownExceptions(delegate
			{
				this.WaitForTheNextCall();
				SettingsContextBase.RunOperationInContext(this.configContexts, exportOperation);
				this.exportFailure = null;
			}, delegate(Exception ex)
			{
				ex.PreserveExceptionStack();
				this.exportFailure = ex;
			});
			this.result = DataExport.DataExportResult.Done;
			this.eventDataAvailable.Set();
		}

		private static readonly TimeSpan maxCallLength = TimeSpan.FromSeconds(30.0);

		private IDataMessage getDataResponseMsg;

		private DataMessageOpcode nextOpcode;

		private byte[] nextBuffer;

		private DataExport.DataExportResult result;

		private Exception exportFailure;

		private Thread exportThread;

		private AutoResetEvent eventDataAvailable;

		private AutoResetEvent eventDataProcessed;

		private bool lastReturnWasTimeout;

		private bool cancelExport;

		private int traceActivityID;

		private List<SettingsContextBase> configContexts;

		private MailboxReplicationProxyService service;

		private int storedOpcode;

		private byte[] storedData;

		private enum DataExportResult
		{
			Done,
			MoreData,
			Flush,
			Timeout
		}
	}
}
