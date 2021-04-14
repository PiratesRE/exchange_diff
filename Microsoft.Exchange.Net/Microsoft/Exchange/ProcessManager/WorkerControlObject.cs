using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.ProcessManager
{
	internal abstract class WorkerControlObject
	{
		public WorkerControlObject(PipeStream readPipeStream, IWorkerProcess workerArg, Trace tracer)
		{
			this.readPipeStream = readPipeStream;
			this.workerProcess = workerArg;
			this.tracer = tracer;
			this.messageMemoryStream = new MemoryStream(this.pipeStreamMessageBuffer, false);
		}

		public bool Initialize()
		{
			bool result;
			try
			{
				this.readPipeStream.BeginRead(this.pipeStreamMessageBuffer, 0, this.pipeStreamMessageBuffer.Length, WorkerControlObject.readMessageComplete, this);
				result = true;
			}
			catch (IOException arg)
			{
				this.Disconnect();
				this.tracer.TraceDebug<IOException>(0L, "WorkerControlObject.Initialize: IOException on BeginRead {0}", arg);
				result = false;
			}
			return result;
		}

		public bool SeenRetireCommand
		{
			get
			{
				return this.seenRetireCommand;
			}
		}

		private static void ReadMessageComplete(IAsyncResult asyncResult)
		{
			WorkerControlObject workerControlObject = (WorkerControlObject)asyncResult.AsyncState;
			workerControlObject.ReadMessageCompleteInternal(asyncResult);
		}

		private void ReadMessageCompleteInternal(IAsyncResult asyncResult)
		{
			try
			{
				int num = this.readPipeStream.EndRead(asyncResult);
				if (num == 0)
				{
					this.tracer.TraceDebug(0L, "WorkerControlObject.ReadMessageCompleteInternal: reading from control pipe returned 0 bytes");
					this.Disconnect();
					this.workerProcess.Stop();
				}
				else
				{
					this.CommandReceived(num);
					if (this.readPipeStream != null)
					{
						this.readPipeStream.BeginRead(this.pipeStreamMessageBuffer, 0, this.pipeStreamMessageBuffer.Length, WorkerControlObject.readMessageComplete, this);
					}
				}
			}
			catch (IOException arg)
			{
				this.tracer.TraceDebug<IOException>(0L, "WorkerControlObject.ReadMessageCompleteInternal: IOException {0}", arg);
				this.Disconnect();
				this.workerProcess.Stop();
			}
			catch (ObjectDisposedException arg2)
			{
				this.tracer.TraceDebug<ObjectDisposedException>(0L, "WorkerControlObject.ReadMessageCompleteInternal: ObjectDisposedException {0}", arg2);
			}
			catch (OperationCanceledException arg3)
			{
				this.tracer.TraceDebug<OperationCanceledException>(0L, "WorkerControlObject.ReadMessageCompleteInternal: OperationCanceledException {0}", arg3);
			}
		}

		private void Disconnect()
		{
			if (this.readPipeStream != null)
			{
				try
				{
					this.readPipeStream.Flush();
					this.readPipeStream.Dispose();
				}
				catch (IOException arg)
				{
					this.tracer.TraceDebug<IOException>(0L, "WorkerControlObject.Disconnect: IOException {0}", arg);
				}
				catch (ObjectDisposedException arg2)
				{
					this.tracer.TraceDebug<ObjectDisposedException>(0L, "WorkerControlObject.Disconnect on ObjectDisposedException {0}", arg2);
				}
				this.readPipeStream = null;
			}
		}

		private void CommandReceived(int size)
		{
			char c = (char)this.pipeStreamMessageBuffer[0];
			if (c != 'C')
			{
				switch (c)
				{
				case 'P':
					this.workerProcess.Pause();
					return;
				case 'R':
					this.seenRetireCommand = true;
					this.Disconnect();
					this.workerProcess.Retire();
					return;
				}
				this.messageMemoryStream.Position = 1L;
				if (!this.ProcessCommand((char)this.pipeStreamMessageBuffer[0], size - 1, (size > 1) ? this.messageMemoryStream : null))
				{
					this.tracer.TraceError(0L, "Unknown message received by worker, forcing Retire to cause worker exit");
					this.Disconnect();
					this.workerProcess.Retire();
				}
				return;
			}
			this.workerProcess.Continue();
		}

		protected abstract bool ProcessCommand(char command, int size, Stream data);

		private readonly IWorkerProcess workerProcess;

		private static readonly AsyncCallback readMessageComplete = new AsyncCallback(WorkerControlObject.ReadMessageComplete);

		private PipeStream readPipeStream;

		private bool seenRetireCommand;

		private byte[] pipeStreamMessageBuffer = new byte[2048];

		private MemoryStream messageMemoryStream;

		private readonly Trace tracer;

		public static class StandardWorkerCommands
		{
			public const char Retire = 'R';

			public const char Pause = 'P';

			public const char Continue = 'C';
		}
	}
}
