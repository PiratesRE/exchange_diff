using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExecuteParams : BaseObject
	{
		public ExecuteParams(WorkBuffer workBuffer)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				using (BufferReader bufferReader = Reader.CreateBufferReader(workBuffer.ArraySegment))
				{
					this.Flags = (int)bufferReader.ReadUInt32();
					int num = (int)bufferReader.ReadUInt32();
					if (num > EmsmdbConstants.MaxChainedExtendedRopBufferSize)
					{
						throw ProtocolException.FromResponseCode((LID)34336, "Maximum ROP output size too large.", ResponseCode.InvalidPayload, null);
					}
					num = Math.Min(num, EmsmdbConstants.MaxChainedExtendedRopBufferSize - 4);
					int count = (int)bufferReader.ReadUInt32();
					this.SegmentExtendedRopIn = bufferReader.ReadArraySegment((uint)count);
					int num2 = (int)bufferReader.ReadUInt32();
					if (num2 > EmsmdbConstants.MaxExtendedAuxBufferSize)
					{
						throw ProtocolException.FromResponseCode((LID)41952, "Maximum AUX output size too large.", ResponseCode.InvalidPayload, null);
					}
					num2 = Math.Min(num2, EmsmdbConstants.MaxExtendedAuxBufferSize - 4);
					int count2 = (int)bufferReader.ReadUInt32();
					this.SegmentExtendedAuxIn = bufferReader.ReadArraySegment((uint)count2);
					this.responseRopOutput = new WorkBuffer(num + 4);
					this.SegmentExtendedRopOut = new ArraySegment<byte>(this.responseRopOutput.ArraySegment.Array, this.responseRopOutput.ArraySegment.Offset + 4, this.responseRopOutput.ArraySegment.Count - 4);
					this.responseAuxOutput = new WorkBuffer(num2 + 4);
					this.SegmentExtendedAuxOut = new ArraySegment<byte>(this.responseAuxOutput.ArraySegment.Array, this.responseAuxOutput.ArraySegment.Offset + 4, this.responseAuxOutput.ArraySegment.Count - 4);
				}
				disposeGuard.Success();
			}
		}

		public int Flags { get; private set; }

		public ArraySegment<byte> SegmentExtendedRopIn { get; private set; }

		public ArraySegment<byte> SegmentExtendedAuxIn { get; private set; }

		public ArraySegment<byte> SegmentExtendedRopOut { get; private set; }

		public ArraySegment<byte> SegmentExtendedAuxOut { get; private set; }

		public uint StatusCode { get; private set; }

		public int ErrorCode { get; private set; }

		public void SetFailedResponse(uint statusCode)
		{
			base.CheckDisposed();
			this.StatusCode = statusCode;
		}

		public void SetSuccessResponse(int ec, ArraySegment<byte> segmentExtendedRopOut, ArraySegment<byte> segmentExtendedAuxOut)
		{
			base.CheckDisposed();
			this.StatusCode = 0U;
			this.ErrorCode = ec;
			this.SegmentExtendedRopOut = segmentExtendedRopOut;
			this.SegmentExtendedAuxOut = segmentExtendedAuxOut;
		}

		public WorkBuffer[] Serialize()
		{
			base.CheckDisposed();
			WorkBuffer workBuffer = null;
			WorkBuffer[] result;
			try
			{
				WorkBuffer[] array;
				if (this.StatusCode != 0U)
				{
					workBuffer = new WorkBuffer(256);
					using (BufferWriter bufferWriter = new BufferWriter(workBuffer.ArraySegment))
					{
						bufferWriter.WriteInt32((int)this.StatusCode);
						workBuffer.Count = (int)bufferWriter.Position;
					}
					array = new WorkBuffer[]
					{
						workBuffer
					};
					workBuffer = null;
				}
				else
				{
					using (BufferWriter bufferWriter2 = new BufferWriter(this.responseRopOutput.ArraySegment))
					{
						bufferWriter2.WriteInt32(this.SegmentExtendedRopOut.Count);
					}
					this.responseRopOutput.Count = this.SegmentExtendedRopOut.Count + 4;
					using (BufferWriter bufferWriter3 = new BufferWriter(this.responseAuxOutput.ArraySegment))
					{
						bufferWriter3.WriteInt32(this.SegmentExtendedAuxOut.Count);
					}
					this.responseAuxOutput.Count = this.SegmentExtendedAuxOut.Count + 4;
					workBuffer = new WorkBuffer(256);
					using (BufferWriter bufferWriter4 = new BufferWriter(workBuffer.ArraySegment))
					{
						bufferWriter4.WriteInt32((int)this.StatusCode);
						bufferWriter4.WriteInt32(this.ErrorCode);
						bufferWriter4.WriteInt32(0);
						bufferWriter4.WriteInt32(Environment.TickCount - this.startTick);
						workBuffer.Count = (int)bufferWriter4.Position;
					}
					array = new WorkBuffer[]
					{
						workBuffer,
						this.responseRopOutput,
						this.responseAuxOutput
					};
					workBuffer = null;
					this.responseRopOutput = null;
					this.responseAuxOutput = null;
				}
				result = array;
			}
			finally
			{
				Util.DisposeIfPresent(workBuffer);
			}
			return result;
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.responseRopOutput);
			Util.DisposeIfPresent(this.responseAuxOutput);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ExecuteParams>(this);
		}

		private const int BaseResponseSize = 256;

		private readonly int startTick = Environment.TickCount;

		private WorkBuffer responseRopOutput;

		private WorkBuffer responseAuxOutput;
	}
}
