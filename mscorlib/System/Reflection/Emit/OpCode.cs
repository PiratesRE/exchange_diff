using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public struct OpCode
	{
		internal OpCode(OpCodeValues value, int flags)
		{
			this.m_stringname = null;
			this.m_pop = (StackBehaviour)(flags >> 12 & 31);
			this.m_push = (StackBehaviour)(flags >> 17 & 31);
			this.m_operand = (OperandType)(flags & 31);
			this.m_type = (OpCodeType)(flags >> 9 & 7);
			this.m_size = (flags >> 22 & 3);
			this.m_s1 = (byte)(value >> 8);
			this.m_s2 = (byte)value;
			this.m_ctrl = (FlowControl)(flags >> 5 & 15);
			this.m_endsUncondJmpBlk = ((flags & 16777216) != 0);
			this.m_stackChange = flags >> 28;
		}

		internal bool EndsUncondJmpBlk()
		{
			return this.m_endsUncondJmpBlk;
		}

		internal int StackChange()
		{
			return this.m_stackChange;
		}

		[__DynamicallyInvokable]
		public OperandType OperandType
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_operand;
			}
		}

		[__DynamicallyInvokable]
		public FlowControl FlowControl
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_ctrl;
			}
		}

		[__DynamicallyInvokable]
		public OpCodeType OpCodeType
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_type;
			}
		}

		[__DynamicallyInvokable]
		public StackBehaviour StackBehaviourPop
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_pop;
			}
		}

		[__DynamicallyInvokable]
		public StackBehaviour StackBehaviourPush
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_push;
			}
		}

		[__DynamicallyInvokable]
		public int Size
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_size;
			}
		}

		[__DynamicallyInvokable]
		public short Value
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.m_size == 2)
				{
					return (short)((int)this.m_s1 << 8 | (int)this.m_s2);
				}
				return (short)this.m_s2;
			}
		}

		[__DynamicallyInvokable]
		public string Name
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.Size == 0)
				{
					return null;
				}
				string[] array = OpCode.g_nameCache;
				if (array == null)
				{
					array = new string[287];
					OpCode.g_nameCache = array;
				}
				OpCodeValues opCodeValues = (OpCodeValues)((ushort)this.Value);
				int num = (int)opCodeValues;
				if (num > 255)
				{
					if (num < 65024 || num > 65054)
					{
						return null;
					}
					num = 256 + (num - 65024);
				}
				string text = Volatile.Read<string>(ref array[num]);
				if (text != null)
				{
					return text;
				}
				text = Enum.GetName(typeof(OpCodeValues), opCodeValues).ToLowerInvariant().Replace("_", ".");
				Volatile.Write<string>(ref array[num], text);
				return text;
			}
		}

		[__DynamicallyInvokable]
		public override bool Equals(object obj)
		{
			return obj is OpCode && this.Equals((OpCode)obj);
		}

		[__DynamicallyInvokable]
		public bool Equals(OpCode obj)
		{
			return obj.Value == this.Value;
		}

		[__DynamicallyInvokable]
		public static bool operator ==(OpCode a, OpCode b)
		{
			return a.Equals(b);
		}

		[__DynamicallyInvokable]
		public static bool operator !=(OpCode a, OpCode b)
		{
			return !(a == b);
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			return (int)this.Value;
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			return this.Name;
		}

		internal const int OperandTypeMask = 31;

		internal const int FlowControlShift = 5;

		internal const int FlowControlMask = 15;

		internal const int OpCodeTypeShift = 9;

		internal const int OpCodeTypeMask = 7;

		internal const int StackBehaviourPopShift = 12;

		internal const int StackBehaviourPushShift = 17;

		internal const int StackBehaviourMask = 31;

		internal const int SizeShift = 22;

		internal const int SizeMask = 3;

		internal const int EndsUncondJmpBlkFlag = 16777216;

		internal const int StackChangeShift = 28;

		private string m_stringname;

		private StackBehaviour m_pop;

		private StackBehaviour m_push;

		private OperandType m_operand;

		private OpCodeType m_type;

		private int m_size;

		private byte m_s1;

		private byte m_s2;

		private FlowControl m_ctrl;

		private bool m_endsUncondJmpBlk;

		private int m_stackChange;

		private static volatile string[] g_nameCache;
	}
}
