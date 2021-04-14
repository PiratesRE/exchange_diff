using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal struct REG_TIMEZONE_INFO : IEquatable<REG_TIMEZONE_INFO>
	{
		public static bool operator ==(REG_TIMEZONE_INFO v1, REG_TIMEZONE_INFO v2)
		{
			return v1.Equals(v2);
		}

		public static bool operator !=(REG_TIMEZONE_INFO v1, REG_TIMEZONE_INFO v2)
		{
			return !v1.Equals(v2);
		}

		public static REG_TIMEZONE_INFO Parse(ArraySegment<byte> buffer)
		{
			if (buffer.Count < REG_TIMEZONE_INFO.Size)
			{
				throw new ArgumentOutOfRangeException();
			}
			REG_TIMEZONE_INFO result;
			result.Bias = BitConverter.ToInt32(buffer.Array, buffer.Offset + REG_TIMEZONE_INFO.BiasOffset);
			result.StandardBias = BitConverter.ToInt32(buffer.Array, buffer.Offset + REG_TIMEZONE_INFO.StandardBiasOffset);
			result.DaylightBias = BitConverter.ToInt32(buffer.Array, buffer.Offset + REG_TIMEZONE_INFO.DaylightBiasOffset);
			result.StandardDate = NativeMethods.SystemTime.Parse(new ArraySegment<byte>(buffer.Array, buffer.Offset + REG_TIMEZONE_INFO.StandardDateOffset, NativeMethods.SystemTime.Size));
			result.DaylightDate = NativeMethods.SystemTime.Parse(new ArraySegment<byte>(buffer.Array, buffer.Offset + REG_TIMEZONE_INFO.DaylightDateOffset, NativeMethods.SystemTime.Size));
			return result;
		}

		public override bool Equals(object o)
		{
			return o is REG_TIMEZONE_INFO && this.Equals((REG_TIMEZONE_INFO)o);
		}

		public bool Equals(REG_TIMEZONE_INFO v)
		{
			return this.Bias == v.Bias && this.StandardBias == v.StandardBias && this.DaylightBias == v.DaylightBias && this.StandardDate == v.StandardDate && this.DaylightDate == v.DaylightDate;
		}

		public override int GetHashCode()
		{
			return this.StandardDate.GetHashCode() ^ this.DaylightDate.GetHashCode() ^ this.Bias;
		}

		public int Write(ArraySegment<byte> buffer)
		{
			if (buffer.Count < REG_TIMEZONE_INFO.Size)
			{
				throw new ArgumentOutOfRangeException();
			}
			ExBitConverter.Write(this.Bias, buffer.Array, buffer.Offset + REG_TIMEZONE_INFO.BiasOffset);
			ExBitConverter.Write(this.StandardBias, buffer.Array, buffer.Offset + REG_TIMEZONE_INFO.StandardBiasOffset);
			ExBitConverter.Write(this.DaylightBias, buffer.Array, buffer.Offset + REG_TIMEZONE_INFO.DaylightBiasOffset);
			this.StandardDate.Write(new ArraySegment<byte>(buffer.Array, buffer.Offset + REG_TIMEZONE_INFO.StandardDateOffset, NativeMethods.SystemTime.Size));
			this.DaylightDate.Write(new ArraySegment<byte>(buffer.Array, buffer.Offset + REG_TIMEZONE_INFO.DaylightDateOffset, NativeMethods.SystemTime.Size));
			return REG_TIMEZONE_INFO.Size;
		}

		public static readonly int Size = Marshal.SizeOf(typeof(REG_TIMEZONE_INFO));

		public int Bias;

		public int StandardBias;

		public int DaylightBias;

		public NativeMethods.SystemTime StandardDate;

		public NativeMethods.SystemTime DaylightDate;

		private static readonly int BiasOffset = (int)Marshal.OffsetOf(typeof(REG_TIMEZONE_INFO), "Bias");

		private static readonly int StandardBiasOffset = (int)Marshal.OffsetOf(typeof(REG_TIMEZONE_INFO), "StandardBias");

		private static readonly int DaylightBiasOffset = (int)Marshal.OffsetOf(typeof(REG_TIMEZONE_INFO), "DaylightBias");

		private static readonly int StandardDateOffset = (int)Marshal.OffsetOf(typeof(REG_TIMEZONE_INFO), "StandardDate");

		private static readonly int DaylightDateOffset = (int)Marshal.OffsetOf(typeof(REG_TIMEZONE_INFO), "DaylightDate");
	}
}
