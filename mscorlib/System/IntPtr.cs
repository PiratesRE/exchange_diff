using System;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public struct IntPtr : ISerializable
	{
		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal bool IsNull()
		{
			return this.m_value == null;
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		[NonVersionable]
		[__DynamicallyInvokable]
		public IntPtr(int value)
		{
			this.m_value = value;
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		[NonVersionable]
		[__DynamicallyInvokable]
		public IntPtr(long value)
		{
			this.m_value = value;
		}

		[SecurityCritical]
		[CLSCompliant(false)]
		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		[NonVersionable]
		public unsafe IntPtr(void* value)
		{
			this.m_value = value;
		}

		[SecurityCritical]
		private IntPtr(SerializationInfo info, StreamingContext context)
		{
			long @int = info.GetInt64("value");
			if (IntPtr.Size == 4 && (@int > 2147483647L || @int < -2147483648L))
			{
				throw new ArgumentException(Environment.GetResourceString("Serialization_InvalidPtrValue"));
			}
			this.m_value = @int;
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("value", this.m_value);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public override bool Equals(object obj)
		{
			return obj is IntPtr && this.m_value == ((IntPtr)obj).m_value;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			return this.m_value;
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[NonVersionable]
		[__DynamicallyInvokable]
		public int ToInt32()
		{
			long num = this.m_value;
			return checked((int)num);
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[NonVersionable]
		[__DynamicallyInvokable]
		public long ToInt64()
		{
			return this.m_value;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public override string ToString()
		{
			return this.m_value.ToString(CultureInfo.InvariantCulture);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public string ToString(string format)
		{
			return this.m_value.ToString(format, CultureInfo.InvariantCulture);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		[NonVersionable]
		public static explicit operator IntPtr(int value)
		{
			return new IntPtr(value);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		[NonVersionable]
		public static explicit operator IntPtr(long value)
		{
			return new IntPtr(value);
		}

		[SecurityCritical]
		[CLSCompliant(false)]
		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		[NonVersionable]
		public unsafe static explicit operator IntPtr(void* value)
		{
			return new IntPtr(value);
		}

		[SecuritySafeCritical]
		[CLSCompliant(false)]
		[NonVersionable]
		public unsafe static explicit operator void*(IntPtr value)
		{
			return value.m_value;
		}

		[SecuritySafeCritical]
		[NonVersionable]
		public static explicit operator int(IntPtr value)
		{
			long num = value.m_value;
			return checked((int)num);
		}

		[SecuritySafeCritical]
		[NonVersionable]
		public static explicit operator long(IntPtr value)
		{
			return value.m_value;
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[NonVersionable]
		[__DynamicallyInvokable]
		public static bool operator ==(IntPtr value1, IntPtr value2)
		{
			return value1.m_value == value2.m_value;
		}

		[SecuritySafeCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[NonVersionable]
		[__DynamicallyInvokable]
		public static bool operator !=(IntPtr value1, IntPtr value2)
		{
			return value1.m_value != value2.m_value;
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		[NonVersionable]
		public static IntPtr Add(IntPtr pointer, int offset)
		{
			return pointer + offset;
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		[NonVersionable]
		public static IntPtr operator +(IntPtr pointer, int offset)
		{
			return new IntPtr(pointer.ToInt64() + (long)offset);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		[NonVersionable]
		public static IntPtr Subtract(IntPtr pointer, int offset)
		{
			return pointer - offset;
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		[NonVersionable]
		public static IntPtr operator -(IntPtr pointer, int offset)
		{
			return new IntPtr(pointer.ToInt64() - (long)offset);
		}

		[__DynamicallyInvokable]
		public static int Size
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[NonVersionable]
			[__DynamicallyInvokable]
			get
			{
				return 8;
			}
		}

		[SecuritySafeCritical]
		[CLSCompliant(false)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[NonVersionable]
		public unsafe void* ToPointer()
		{
			return this.m_value;
		}

		[SecurityCritical]
		private unsafe void* m_value;

		public static readonly IntPtr Zero;
	}
}
