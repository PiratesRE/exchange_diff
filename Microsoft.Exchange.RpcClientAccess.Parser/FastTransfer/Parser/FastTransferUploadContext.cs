using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FastTransferUploadContext : FastTransferContext<IFastTransferReader>
	{
		internal FastTransferUploadContext(Encoding encoding, IResourceTracker resourceTracker, IPropertyFilterFactory propertyFilterFactory, bool isMovingMailbox) : base(resourceTracker, propertyFilterFactory, isMovingMailbox)
		{
			this.encoding = encoding;
		}

		public void PutNextBuffer(IFastTransferReader reader)
		{
			base.CheckDisposed();
			if (this.noMoreBuffers)
			{
				throw new InvalidOperationException();
			}
			this.Process(reader);
		}

		public void PutNextBuffer(ArraySegment<byte> buffer)
		{
			base.CheckDisposed();
			using (IFastTransferReader fastTransferReader = this.CreateReader(buffer))
			{
				this.PutNextBuffer(fastTransferReader);
			}
		}

		public void Flush()
		{
			base.CheckDisposed();
			this.noMoreBuffers = true;
			using (IFastTransferReader fastTransferReader = this.CreateReader(new ArraySegment<byte>(Array<byte>.Empty)))
			{
				try
				{
					this.Process(fastTransferReader);
				}
				catch (BufferParseException)
				{
					throw new BufferParseException(string.Format("Unexpected end of input FastTransfer stream. Final state: {0}", this.ToString()));
				}
			}
		}

		public void PushInitial(IFastTransferProcessor<FastTransferUploadContext> fastTransferObject)
		{
			base.PushInitial(this.CreateStateMachine(fastTransferObject));
		}

		public bool NoMoreData
		{
			get
			{
				base.CheckDisposed();
				return this.noMoreBuffers && !base.DataInterface.IsDataAvailable;
			}
		}

		public Encoding String8Encoding
		{
			get
			{
				return this.encoding;
			}
		}

		protected override void Process(IFastTransferReader dataInterface)
		{
			base.Process(dataInterface);
			this.OnEndOfBuffer();
		}

		internal FastTransferStateMachine CreateStateMachine(IFastTransferProcessor<FastTransferUploadContext> fastTransferObject)
		{
			return FastTransferContext<IFastTransferReader>.CreateStateMachine<FastTransferUploadContext>(this, fastTransferObject);
		}

		internal void SetEndOfBufferAction(Action action)
		{
			this.endOfBufferAction = action;
		}

		internal void AllowEndOfBufferActions(bool allow)
		{
			this.allowEndOfBufferActions = allow;
		}

		protected override bool CanContinue()
		{
			return base.DataInterface.IsDataAvailable || this.noMoreBuffers;
		}

		protected virtual IFastTransferReader CreateReader(ArraySegment<byte> buffer)
		{
			return new FastTransferReader(buffer);
		}

		private void OnEndOfBuffer()
		{
			if (this.allowEndOfBufferActions && this.endOfBufferAction != null)
			{
				this.endOfBufferAction();
			}
		}

		private readonly Encoding encoding;

		private bool noMoreBuffers;

		private Action endOfBufferAction;

		private bool allowEndOfBufferActions = true;
	}
}
