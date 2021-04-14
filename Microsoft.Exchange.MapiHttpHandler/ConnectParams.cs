using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ConnectParams : BaseObject
	{
		public ConnectParams(WorkBuffer workBuffer)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				short[] array = new short[3];
				using (BufferReader bufferReader = Reader.CreateBufferReader(workBuffer.ArraySegment))
				{
					this.UserDn = bufferReader.ReadAsciiString(StringFlags.IncludeNull | StringFlags.Sized16);
					this.Flags = (int)bufferReader.ReadUInt32();
					this.ConnectionModulus = (int)bufferReader.ReadUInt32();
					this.CodePage = (int)bufferReader.ReadUInt32();
					this.StringLcid = (int)bufferReader.ReadUInt32();
					this.SortLcid = (int)bufferReader.ReadUInt32();
					array[0] = (short)bufferReader.ReadUInt16();
					array[1] = (short)bufferReader.ReadUInt16();
					array[2] = (short)bufferReader.ReadUInt16();
					short[] array2 = new short[4];
					MapiVersionConversion.Normalize(array, array2);
					this.ClientVersion = array2;
					int num = (int)bufferReader.ReadUInt32();
					if (num > EmsmdbConstants.MaxExtendedAuxBufferSize)
					{
						throw ProtocolException.FromResponseCode((LID)41952, string.Format("Maximum AUX output size too large; maximum={0}.", EmsmdbConstants.MaxExtendedAuxBufferSize), ResponseCode.InvalidPayload, null);
					}
					num = Math.Min(num, EmsmdbConstants.MaxExtendedAuxBufferSize - 4);
					int count = (int)bufferReader.ReadUInt32();
					this.SegmentExtendedAuxIn = bufferReader.ReadArraySegment((uint)count);
					this.responseAuxOutput = new WorkBuffer(num + 4);
					this.SegmentExtendedAuxOut = new ArraySegment<byte>(this.responseAuxOutput.ArraySegment.Array, this.responseAuxOutput.ArraySegment.Offset + 4, this.responseAuxOutput.ArraySegment.Count - 4);
				}
				disposeGuard.Success();
			}
		}

		public string UserDn { get; private set; }

		public int Flags { get; private set; }

		public int ConnectionModulus { get; private set; }

		public int CodePage { get; private set; }

		public int StringLcid { get; private set; }

		public int SortLcid { get; private set; }

		public short[] ClientVersion { get; private set; }

		public ArraySegment<byte> SegmentExtendedAuxIn { get; private set; }

		public ArraySegment<byte> SegmentExtendedAuxOut { get; private set; }

		public uint StatusCode { get; private set; }

		public int ErrorCode { get; private set; }

		public TimeSpan PollsMax { get; private set; }

		public int RetryCount { get; private set; }

		public TimeSpan RetryDelay { get; private set; }

		public string DnPrefix { get; private set; }

		public string DisplayName { get; private set; }

		public short[] ServerVersion { get; private set; }

		public void SetFailedResponse(uint statusCode)
		{
			base.CheckDisposed();
			this.StatusCode = statusCode;
		}

		public void SetSuccessResponse(int ec, TimeSpan pollsMax, int retryCount, TimeSpan retryDelay, string dnPrefix, string displayName, short[] serverVersion, ArraySegment<byte> segmentExtendedAuxOut)
		{
			base.CheckDisposed();
			this.StatusCode = 0U;
			this.ErrorCode = ec;
			this.PollsMax = pollsMax;
			this.RetryCount = retryCount;
			this.RetryDelay = retryDelay;
			this.DnPrefix = dnPrefix;
			this.DisplayName = displayName;
			this.ServerVersion = serverVersion;
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
					short[] array2 = new short[3];
					MapiVersionConversion.Legacy(this.ServerVersion, array2, 4000);
					using (BufferWriter bufferWriter2 = new BufferWriter(this.responseAuxOutput.ArraySegment))
					{
						bufferWriter2.WriteInt32(this.SegmentExtendedAuxOut.Count);
					}
					this.responseAuxOutput.Count = this.SegmentExtendedAuxOut.Count + 4;
					workBuffer = new WorkBuffer((this.DnPrefix.Length + this.DisplayName.Length) * 2 + 256);
					using (BufferWriter bufferWriter3 = new BufferWriter(workBuffer.ArraySegment))
					{
						bufferWriter3.WriteInt32((int)this.StatusCode);
						bufferWriter3.WriteInt32(this.ErrorCode);
						bufferWriter3.WriteInt32((int)this.PollsMax.TotalMilliseconds);
						bufferWriter3.WriteInt32(this.RetryCount);
						bufferWriter3.WriteInt32((int)this.RetryDelay.TotalMilliseconds);
						bufferWriter3.WriteAsciiString(this.DnPrefix, StringFlags.IncludeNull | StringFlags.Sized16);
						bufferWriter3.WriteAsciiString(this.DisplayName, StringFlags.IncludeNull | StringFlags.Sized16);
						bufferWriter3.WriteInt16(array2[0]);
						bufferWriter3.WriteInt16(array2[1]);
						bufferWriter3.WriteInt16(array2[2]);
						workBuffer.Count = (int)bufferWriter3.Position;
					}
					array = new WorkBuffer[]
					{
						workBuffer,
						this.responseAuxOutput
					};
					workBuffer = null;
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
			Util.DisposeIfPresent(this.responseAuxOutput);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ConnectParams>(this);
		}

		private const int BaseResponseSize = 256;

		private WorkBuffer responseAuxOutput;
	}
}
