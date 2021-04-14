using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NotificationWaitParams : BaseObject
	{
		public NotificationWaitParams(WorkBuffer workBuffer)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				using (BufferReader bufferReader = Reader.CreateBufferReader(workBuffer.ArraySegment))
				{
					this.Flags = (int)bufferReader.ReadUInt32();
				}
				disposeGuard.Success();
			}
		}

		public int Flags { get; private set; }

		public uint StatusCode { get; private set; }

		public int ErrorCode { get; private set; }

		public void SetFailedResponse(uint statusCode)
		{
			base.CheckDisposed();
			this.StatusCode = statusCode;
		}

		public void SetSuccessResponse(int ec, int flags)
		{
			base.CheckDisposed();
			this.StatusCode = 0U;
			this.ErrorCode = ec;
			this.Flags = flags;
		}

		public WorkBuffer[] Serialize()
		{
			base.CheckDisposed();
			WorkBuffer workBuffer = null;
			WorkBuffer[] result;
			try
			{
				workBuffer = new WorkBuffer(256);
				using (BufferWriter bufferWriter = new BufferWriter(workBuffer.ArraySegment))
				{
					bufferWriter.WriteInt32((int)this.StatusCode);
					if (this.StatusCode == 0U)
					{
						bufferWriter.WriteInt32(this.ErrorCode);
						bufferWriter.WriteInt32(this.Flags);
					}
					workBuffer.Count = (int)bufferWriter.Position;
				}
				WorkBuffer[] array = new WorkBuffer[]
				{
					workBuffer
				};
				workBuffer = null;
				result = array;
			}
			finally
			{
				Util.DisposeIfPresent(workBuffer);
			}
			return result;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationWaitParams>(this);
		}

		private const int BaseResponseSize = 256;
	}
}
