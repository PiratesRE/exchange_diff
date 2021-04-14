using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Etw
{
	public abstract class TraceEvent
	{
		internal unsafe TraceEvent(Guid providerGuid, string providerName, EtwTraceNativeComponents.EVENT_RECORD* rawData)
		{
			this.providerGuid = providerGuid;
			this.providerName = providerName;
			this.eventRecord = rawData;
			this.userData = rawData->UserData;
		}

		public unsafe ushort EventID
		{
			get
			{
				return this.eventRecord->EventHeader.Id;
			}
		}

		public Guid ProviderGuid
		{
			get
			{
				return this.providerGuid;
			}
		}

		public string ProviderName
		{
			get
			{
				if (this.providerName == null)
				{
					this.providerName = "UnknownProvider";
				}
				return this.providerName;
			}
		}

		public unsafe ushort ID
		{
			get
			{
				return this.eventRecord->EventHeader.Id;
			}
		}

		public unsafe byte Opcode
		{
			get
			{
				return this.eventRecord->EventHeader.Opcode;
			}
		}

		public unsafe int Version
		{
			get
			{
				return (int)this.eventRecord->EventHeader.Version;
			}
		}

		public unsafe int ThreadID
		{
			get
			{
				return this.eventRecord->EventHeader.ThreadId;
			}
		}

		public unsafe virtual int ProcessID
		{
			get
			{
				return this.eventRecord->EventHeader.ProcessId;
			}
		}

		public unsafe int PointerSize
		{
			get
			{
				if ((this.eventRecord->EventHeader.Flags & 64) == 0)
				{
					return 4;
				}
				return 8;
			}
		}

		public unsafe Guid ActivityID
		{
			get
			{
				return this.eventRecord->EventHeader.ActivityId;
			}
		}

		public unsafe int EventDataLength
		{
			get
			{
				return (int)this.eventRecord->UserDataLength;
			}
		}

		internal IntPtr DataStart
		{
			get
			{
				return this.userData;
			}
		}

		internal unsafe bool IsClassicProvider
		{
			get
			{
				return (this.eventRecord->EventHeader.Flags & 256) != 0;
			}
		}

		internal byte[] EventData()
		{
			return this.EventData(null, 0, 0, this.EventDataLength);
		}

		internal unsafe byte[] EventData(byte[] targetBuffer, int targetStartIndex, int sourceStartIndex, int length)
		{
			if (targetBuffer == null)
			{
				targetBuffer = new byte[length + targetStartIndex];
			}
			if (sourceStartIndex + length > this.EventDataLength)
			{
				throw new IndexOutOfRangeException();
			}
			IntPtr source = (IntPtr)((void*)((byte*)this.DataStart.ToPointer() + sourceStartIndex));
			if (length > 0)
			{
				Marshal.Copy(source, targetBuffer, targetStartIndex, length);
			}
			return targetBuffer;
		}

		internal int HostSizePtr(int numPointers)
		{
			return this.PointerSize * numPointers;
		}

		internal int GetByteAt(int offset)
		{
			return (int)TraceEvent.RawReaderUtils.ReadByte(this.DataStart, offset);
		}

		internal int GetInt32At(int offset)
		{
			return TraceEvent.RawReaderUtils.ReadInt32(this.DataStart, offset);
		}

		internal long GetInt64At(int offset)
		{
			return TraceEvent.RawReaderUtils.ReadInt64(this.DataStart, offset);
		}

		internal long GetIntPtrAt(int offset)
		{
			if (this.PointerSize == 4)
			{
				return (long)((ulong)this.GetInt32At(offset));
			}
			return this.GetInt64At(offset);
		}

		internal virtual void Parse()
		{
		}

		private readonly Guid providerGuid;

		private unsafe readonly EtwTraceNativeComponents.EVENT_RECORD* eventRecord;

		private readonly IntPtr userData;

		private string providerName;

		internal sealed class RawReaderUtils
		{
			internal unsafe static long ReadInt64(IntPtr pointer, int offset)
			{
				return *(long*)((byte*)pointer.ToPointer() + offset);
			}

			internal unsafe static int ReadInt32(IntPtr pointer, int offset)
			{
				return *(int*)((byte*)pointer.ToPointer() + offset);
			}

			internal unsafe static short ReadInt16(IntPtr pointer, int offset)
			{
				return *(short*)((byte*)pointer.ToPointer() + offset);
			}

			internal unsafe static byte ReadByte(IntPtr pointer, int offset)
			{
				return ((byte*)pointer.ToPointer())[offset];
			}
		}
	}
}
