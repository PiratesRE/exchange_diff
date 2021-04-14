using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Diagnostics.Tracing
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	internal struct EventDescriptor
	{
		public EventDescriptor(int traceloggingId, byte level, byte opcode, long keywords)
		{
			this.m_id = 0;
			this.m_version = 0;
			this.m_channel = 0;
			this.m_traceloggingId = traceloggingId;
			this.m_level = level;
			this.m_opcode = opcode;
			this.m_task = 0;
			this.m_keywords = keywords;
		}

		public EventDescriptor(int id, byte version, byte channel, byte level, byte opcode, int task, long keywords)
		{
			if (id < 0)
			{
				throw new ArgumentOutOfRangeException("id", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (id > 65535)
			{
				throw new ArgumentOutOfRangeException("id", Environment.GetResourceString("ArgumentOutOfRange_NeedValidId", new object[]
				{
					1,
					ushort.MaxValue
				}));
			}
			this.m_traceloggingId = 0;
			this.m_id = (ushort)id;
			this.m_version = version;
			this.m_channel = channel;
			this.m_level = level;
			this.m_opcode = opcode;
			this.m_keywords = keywords;
			if (task < 0)
			{
				throw new ArgumentOutOfRangeException("task", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (task > 65535)
			{
				throw new ArgumentOutOfRangeException("task", Environment.GetResourceString("ArgumentOutOfRange_NeedValidId", new object[]
				{
					1,
					ushort.MaxValue
				}));
			}
			this.m_task = (ushort)task;
		}

		public int EventId
		{
			get
			{
				return (int)this.m_id;
			}
		}

		public byte Version
		{
			get
			{
				return this.m_version;
			}
		}

		public byte Channel
		{
			get
			{
				return this.m_channel;
			}
		}

		public byte Level
		{
			get
			{
				return this.m_level;
			}
		}

		public byte Opcode
		{
			get
			{
				return this.m_opcode;
			}
		}

		public int Task
		{
			get
			{
				return (int)this.m_task;
			}
		}

		public long Keywords
		{
			get
			{
				return this.m_keywords;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is EventDescriptor && this.Equals((EventDescriptor)obj);
		}

		public override int GetHashCode()
		{
			return (int)(this.m_id ^ (ushort)this.m_version ^ (ushort)this.m_channel ^ (ushort)this.m_level ^ (ushort)this.m_opcode ^ this.m_task) ^ (int)this.m_keywords;
		}

		public bool Equals(EventDescriptor other)
		{
			return this.m_id == other.m_id && this.m_version == other.m_version && this.m_channel == other.m_channel && this.m_level == other.m_level && this.m_opcode == other.m_opcode && this.m_task == other.m_task && this.m_keywords == other.m_keywords;
		}

		public static bool operator ==(EventDescriptor event1, EventDescriptor event2)
		{
			return event1.Equals(event2);
		}

		public static bool operator !=(EventDescriptor event1, EventDescriptor event2)
		{
			return !event1.Equals(event2);
		}

		[FieldOffset(0)]
		private int m_traceloggingId;

		[FieldOffset(0)]
		private ushort m_id;

		[FieldOffset(2)]
		private byte m_version;

		[FieldOffset(3)]
		private byte m_channel;

		[FieldOffset(4)]
		private byte m_level;

		[FieldOffset(5)]
		private byte m_opcode;

		[FieldOffset(6)]
		private ushort m_task;

		[FieldOffset(8)]
		private long m_keywords;
	}
}
