using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DisconnectParams : BaseObject
	{
		public DisconnectParams(WorkBuffer workBuffer)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (workBuffer.ArraySegment.Count > 0)
				{
					throw ProtocolException.FromResponseCode((LID)42528, "Disconnect body should be empty.", ResponseCode.InvalidPayload, null);
				}
				disposeGuard.Success();
			}
		}

		public uint StatusCode { get; private set; }

		public int ErrorCode { get; private set; }

		public void SetFailedResponse(uint statusCode)
		{
			base.CheckDisposed();
			this.StatusCode = statusCode;
		}

		public void SetSuccessResponse(int ec)
		{
			base.CheckDisposed();
			this.StatusCode = 0U;
			this.ErrorCode = ec;
		}

		public WorkBuffer[] Serialize()
		{
			base.CheckDisposed();
			WorkBuffer workBuffer = null;
			WorkBuffer[] result;
			try
			{
				workBuffer = new WorkBuffer(16);
				using (BufferWriter bufferWriter = new BufferWriter(workBuffer.ArraySegment))
				{
					bufferWriter.WriteInt32((int)this.StatusCode);
					if (this.StatusCode == 0U)
					{
						bufferWriter.WriteInt32(this.ErrorCode);
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
			return DisposeTracker.Get<DisconnectParams>(this);
		}

		private const int BaseResponseSize = 16;
	}
}
