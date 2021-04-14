using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public struct JET_LGPOS : IEquatable<JET_LGPOS>, IComparable<JET_LGPOS>, INullableJetStruct
	{
		public int ib
		{
			[DebuggerStepThrough]
			get
			{
				return (int)this.offset;
			}
			set
			{
				this.offset = checked((ushort)value);
			}
		}

		public int isec
		{
			[DebuggerStepThrough]
			get
			{
				return (int)this.sector;
			}
			set
			{
				this.sector = checked((ushort)value);
			}
		}

		public int lGeneration
		{
			[DebuggerStepThrough]
			get
			{
				return this.generation;
			}
			set
			{
				this.generation = value;
			}
		}

		public bool HasValue
		{
			get
			{
				return 0 != this.lGeneration;
			}
		}

		public static bool operator ==(JET_LGPOS lhs, JET_LGPOS rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(JET_LGPOS lhs, JET_LGPOS rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator <(JET_LGPOS lhs, JET_LGPOS rhs)
		{
			return lhs.CompareTo(rhs) < 0;
		}

		public static bool operator >(JET_LGPOS lhs, JET_LGPOS rhs)
		{
			return lhs.CompareTo(rhs) > 0;
		}

		public static bool operator <=(JET_LGPOS lhs, JET_LGPOS rhs)
		{
			return lhs.CompareTo(rhs) <= 0;
		}

		public static bool operator >=(JET_LGPOS lhs, JET_LGPOS rhs)
		{
			return lhs.CompareTo(rhs) >= 0;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_LGPOS(0x{0:X},{1:X},{2:X})", new object[]
			{
				this.lGeneration,
				this.isec,
				this.ib
			});
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_LGPOS)obj);
		}

		public override int GetHashCode()
		{
			return this.generation ^ (int)this.sector << 16 ^ (int)this.offset;
		}

		public bool Equals(JET_LGPOS other)
		{
			return this.generation == other.generation && this.sector == other.sector && this.offset == other.offset;
		}

		public int CompareTo(JET_LGPOS other)
		{
			int num = this.generation.CompareTo(other.generation);
			if (num == 0)
			{
				num = this.sector.CompareTo(other.sector);
			}
			if (num == 0)
			{
				num = this.offset.CompareTo(other.offset);
			}
			return num;
		}

		private ushort offset;

		private ushort sector;

		private int generation;
	}
}
