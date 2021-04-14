using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class AsynchronousTransmitter : DisposableWrapper<IDataImport>, IDataImport, IDisposable
	{
		public AsynchronousTransmitter(IDataImport destination, bool ownsDestination) : base(destination, ownsDestination)
		{
			this.currentMessage = null;
			this.replyMessage = null;
			this.lastFailure = null;
			this.eventWakeUpTransmitter = new ManualResetEvent(false);
			this.eventBufferIsAvailableToAccept = new ManualResetEvent(true);
			this.quitting = false;
			this.transmitThread = null;
			this.mainThread = Thread.CurrentThread;
		}

		void IDataImport.SendMessage(IDataMessage message)
		{
			this.SpinUpTransmitterThread();
			this.WaitUntilDataIsProcessed();
			this.currentMessage = message;
			this.eventBufferIsAvailableToAccept.Reset();
			this.eventWakeUpTransmitter.Set();
		}

		IDataMessage IDataImport.SendMessageAndWaitForReply(IDataMessage message)
		{
			((IDataImport)this).SendMessage(new AsynchronousTransmitter.AsyncTransmitterWaitForReplyMessage(message));
			this.WaitUntilDataIsProcessed();
			IDataMessage result = this.replyMessage;
			this.replyMessage = null;
			return result;
		}

		public void Close()
		{
			CommonUtils.CatchKnownExceptions(delegate
			{
				this.SpinUpTransmitterThread();
				CommonUtils.SafeWait(this.eventBufferIsAvailableToAccept, this.transmitThread);
				this.quitting = true;
				this.eventWakeUpTransmitter.Set();
				this.transmitThread.Join();
				this.transmitThread = null;
			}, null);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.Close();
				this.eventWakeUpTransmitter.Close();
				this.eventBufferIsAvailableToAccept.Close();
			}
			base.InternalDispose(calledFromDispose);
		}

		private void SpinUpTransmitterThread()
		{
			if (this.transmitThread != null)
			{
				return;
			}
			this.traceActivityID = MrsTracer.ActivityID;
			this.configContexts = SettingsContextBase.GetCurrentContexts();
			Thread thread = new Thread(new ThreadStart(this.TransmitThread));
			thread.Name = "MRS transmitter thread";
			thread.Start();
			this.transmitThread = thread;
		}

		private void WaitUntilDataIsProcessed()
		{
			CommonUtils.SafeWait(this.eventBufferIsAvailableToAccept, this.transmitThread);
			if (this.lastFailure != null)
			{
				Exception ex = this.lastFailure;
				this.lastFailure = null;
				this.currentMessage = null;
				ExecutionContext.StampCurrentDataContext(ex);
				throw ex;
			}
		}

		private void TransmitThread()
		{
			MrsTracer.ActivityID = this.traceActivityID;
			SettingsContextBase.RunOperationInContext(this.configContexts, delegate
			{
				for (;;)
				{
					CommonUtils.SafeWait(this.eventWakeUpTransmitter, this.mainThread);
					if (this.quitting)
					{
						break;
					}
					CommonUtils.CatchKnownExceptions(delegate
					{
						AsynchronousTransmitter.AsyncTransmitterWaitForReplyMessage asyncTransmitterWaitForReplyMessage = this.currentMessage as AsynchronousTransmitter.AsyncTransmitterWaitForReplyMessage;
						if (asyncTransmitterWaitForReplyMessage != null)
						{
							this.replyMessage = base.WrappedObject.SendMessageAndWaitForReply(asyncTransmitterWaitForReplyMessage.Request);
							return;
						}
						base.WrappedObject.SendMessage(this.currentMessage);
					}, delegate(Exception failure)
					{
						failure.PreserveExceptionStack();
						this.lastFailure = failure;
					});
					this.currentMessage = null;
					this.eventWakeUpTransmitter.Reset();
					this.eventBufferIsAvailableToAccept.Set();
				}
			});
		}

		private IDataMessage currentMessage;

		private IDataMessage replyMessage;

		private Exception lastFailure;

		private ManualResetEvent eventWakeUpTransmitter;

		private ManualResetEvent eventBufferIsAvailableToAccept;

		private bool quitting;

		private Thread transmitThread;

		private Thread mainThread;

		private int traceActivityID;

		private List<SettingsContextBase> configContexts;

		private class AsyncTransmitterWaitForReplyMessage : IDataMessage
		{
			public AsyncTransmitterWaitForReplyMessage(IDataMessage requestMessage)
			{
				this.requestMessage = requestMessage;
			}

			public IDataMessage Request
			{
				get
				{
					return this.requestMessage;
				}
			}

			int IDataMessage.GetSize()
			{
				return this.Request.GetSize();
			}

			void IDataMessage.Serialize(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
			{
				this.Request.Serialize(useCompression, out opcode, out data);
			}

			private IDataMessage requestMessage;
		}
	}
}
