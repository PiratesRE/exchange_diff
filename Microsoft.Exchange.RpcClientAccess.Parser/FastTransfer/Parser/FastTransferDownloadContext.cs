using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferDownloadContext : FastTransferContext<FastTransferWriter>
	{
		private FastTransferDownloadContext(bool isIcs, FastTransferSendOption options, uint steps, Encoding encoding, IResourceTracker resourceTracker, IPropertyFilterFactory propertyFilterFactory, bool isMovingMailbox) : base(resourceTracker, propertyFilterFactory, isMovingMailbox)
		{
			this.isIcs = isIcs;
			this.options = options;
			this.steps = steps;
			this.encoding = encoding;
		}

		public static FastTransferDownloadContext CreateForIcs(FastTransferSendOption options, Encoding encoding, IResourceTracker resourceTracker, IPropertyFilterFactory propertyFilterFactory, bool isMovingMailbox)
		{
			return new FastTransferDownloadContext(true, options, 1U, encoding, resourceTracker, propertyFilterFactory, isMovingMailbox);
		}

		public static FastTransferDownloadContext CreateForDownload(FastTransferSendOption options, uint steps, Encoding encoding, IResourceTracker resourceTracker, IPropertyFilterFactory propertyFilterFactory, bool isMovingMailbox)
		{
			return new FastTransferDownloadContext(false, options, steps, encoding, resourceTracker, propertyFilterFactory, isMovingMailbox);
		}

		public int GetNextBuffer(ArraySegment<byte> buffer)
		{
			base.CheckDisposed();
			int position;
			using (FastTransferWriter fastTransferWriter = new FastTransferWriter(buffer))
			{
				this.Process(fastTransferWriter);
				position = fastTransferWriter.Position;
			}
			return position;
		}

		public void PushInitial(IFastTransferProcessor<FastTransferDownloadContext> fastTransferObject)
		{
			base.PushInitial(this.CreateStateMachine(fastTransferObject));
		}

		public uint Progress
		{
			get
			{
				base.CheckDisposed();
				return this.progress;
			}
		}

		public uint Steps
		{
			get
			{
				base.CheckDisposed();
				return this.steps;
			}
		}

		public void IncrementProgress()
		{
			base.CheckDisposed();
			this.progress += 1U;
		}

		internal bool IsIcs
		{
			get
			{
				return this.isIcs;
			}
		}

		internal bool UseCpidOrUnicode
		{
			get
			{
				return this.options.UseCpidOrUnicode();
			}
		}

		internal bool UseCpid
		{
			get
			{
				return this.options.UseCpid();
			}
		}

		internal bool SendPropertyErrors
		{
			get
			{
				return (byte)(this.options & FastTransferSendOption.SendPropErrors) != 0;
			}
		}

		internal FastTransferStateMachine CreateStateMachine(IFastTransferProcessor<FastTransferDownloadContext> fastTransferObject)
		{
			return FastTransferContext<FastTransferWriter>.CreateStateMachine<FastTransferDownloadContext>(this, fastTransferObject);
		}

		public Encoding String8Encoding
		{
			get
			{
				return this.encoding;
			}
		}

		protected override void Process(FastTransferWriter dataInterface)
		{
			if (dataInterface.TryWriteOverflow(ref this.overflowBytes))
			{
				base.Process(dataInterface);
				this.overflowBytes = dataInterface.GetOverflowBytes();
			}
			if (base.State != FastTransferState.Error && this.overflowBytes.Count > 0)
			{
				base.State = FastTransferState.Partial;
			}
		}

		[DebuggerNonUserCode]
		protected override bool CanContinue()
		{
			return !base.DataInterface.IsBufferFull;
		}

		private readonly bool isIcs;

		private readonly FastTransferSendOption options;

		private readonly Encoding encoding;

		private readonly uint steps;

		private uint progress;

		private ArraySegment<byte> overflowBytes;
	}
}
