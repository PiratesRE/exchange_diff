using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Security;

namespace System
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public struct UIntPtr : ISerializable
	{
		[SecuritySafeCritical]
		[NonVersionable]
		[__DynamicallyInvokable]
		public UIntPtr(uint value)
		{
			this.m_value = value;
		}

		[SecuritySafeCritical]
		[NonVersionable]
		[__DynamicallyInvokable]
		public UIntPtr(ulong value)
		{
			this.m_value = value;
		}

		[SecurityCritical]
		[CLSCompliant(false)]
		[NonVersionable]
		public unsafe UIntPtr(void* value)
		{
			this.m_value = value;
		}

		[SecurityCritical]
		private UIntPtr(SerializationInfo info, StreamingContext context)
		{
			ulong @uint = info.GetUInt64("value");
			if (UIntPtr.Size == 4 && @uint > (ulong)-1)
			{
				throw new ArgumentException(Environment.GetResourceString("Serialization_InvalidPtrValue"));
			}
			this.m_value = @uint;
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
			return obj is UIntPtr && this.m_value == ((UIntPtr)obj).m_value;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			return this.m_value & int.MaxValue;
		}

		[SecuritySafeCritical]
		[NonVersionable]
		[__DynamicallyInvokable]
		public uint ToUInt32()
		{
			return this.m_value;
		}

		[SecuritySafeCritical]
		[NonVersionable]
		[__DynamicallyInvokable]
		public ulong ToUInt64()
		{
			return this.m_value;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public override string ToString()
		{
			return this.m_value.ToString(CultureInfo.InvariantCulture);
		}

		[NonVersionable]
		public static explicit operator UIntPtr(uint value)
		{
			return new UIntPtr(value);
		}

		[NonVersionable]
		public static explicit operator UIntPtr(ulong value)
		{
			return new UIntPtr(value);
		}

		[SecuritySafeCritical]
		[NonVersionable]
		public static explicit operator uint(UIntPtr value)
		{
			return value.m_value;
		}

		[SecuritySafeCritical]
		[NonVersionable]
		public static explicit operator ulong(UIntPtr value)
		{
			return value.m_value;
		}

		[SecurityCritical]
		[CLSCompliant(false)]
		[NonVersionable]
		public unsafe static explicit operator UIntPtr(void* value)
		{
			return new UIntPtr(value);
		}

		[SecurityCritical]
		[CLSCompliant(false)]
		[NonVersionable]
		public unsafe static explicit operator void*(UIntPtr value)
		{
			return value.m_value;
		}

		[SecuritySafeCritical]
		[NonVersionable]
		[__DynamicallyInvokable]
		public static bool operator ==(UIntPtr value1, UIntPtr value2)
		{
			return value1.m_value == value2.m_value;
		}

		[SecuritySafeCritical]
		[NonVersionable]
		[__DynamicallyInvokable]
		public static bool operator !=(UIntPtr value1, UIntPtr value2)
		{
			return value1.m_value != value2.m_value;
		}

		[NonVersionable]
		public static UIntPtr Add(UIntPtr pointer, int offset)
		{
			return pointer + offset;
		}

		[NonVersionable]
		public static UIntPtr operator +(UIntPtr pointer, int offset)
		{
			return new UIntPtr(pointer.ToUInt64() + (ulong)((long)offset));
		}

		[NonVersionable]
		public static UIntPtr Subtract(UIntPtr pointer, int offset)
		{
			return pointer - offset;
		}

		[NonVersionable]
		public static UIntPtr operator -(UIntPtr pointer, int offset)
		{
			return new UIntPtr(pointer.ToUInt64() - (ulong)((long)offset));
		}

		[__DynamicallyInvokable]
		public static int Size
		{
			[NonVersionable]
			[__DynamicallyInvokable]
			get
			{
				return 8;
			}
		}

		[SecuritySafeCritical]
		[CLSCompliant(false)]
		[NonVersionable]
		public unsafe void* ToPointer()
		{
			return this.m_value;
		}

		[SecurityCritical]
		private unsafe void* m_value;

		public static readonly UIntPtr Zero;
	}
}
