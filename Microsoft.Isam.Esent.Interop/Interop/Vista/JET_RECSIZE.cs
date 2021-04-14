using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Vista
{
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public struct JET_RECSIZE : IEquatable<JET_RECSIZE>
	{
		public long cbData
		{
			[DebuggerStepThrough]
			get
			{
				return this.userData;
			}
			internal set
			{
				this.userData = value;
			}
		}

		public long cbLongValueData
		{
			[DebuggerStepThrough]
			get
			{
				return this.userLongValueData;
			}
			internal set
			{
				this.userLongValueData = value;
			}
		}

		public long cbOverhead
		{
			[DebuggerStepThrough]
			get
			{
				return this.overhead;
			}
			internal set
			{
				this.overhead = value;
			}
		}

		public long cbLongValueOverhead
		{
			[DebuggerStepThrough]
			get
			{
				return this.longValueOverhead;
			}
			internal set
			{
				this.longValueOverhead = value;
			}
		}

		public long cNonTaggedColumns
		{
			[DebuggerStepThrough]
			get
			{
				return this.numNonTaggedColumns;
			}
			internal set
			{
				this.numNonTaggedColumns = value;
			}
		}

		public long cTaggedColumns
		{
			[DebuggerStepThrough]
			get
			{
				return this.numTaggedColumns;
			}
			internal set
			{
				this.numTaggedColumns = value;
			}
		}

		public long cLongValues
		{
			[DebuggerStepThrough]
			get
			{
				return this.numLongValues;
			}
			internal set
			{
				this.numLongValues = value;
			}
		}

		public long cMultiValues
		{
			[DebuggerStepThrough]
			get
			{
				return this.numMultiValues;
			}
			internal set
			{
				this.numMultiValues = value;
			}
		}

		public long cCompressedColumns
		{
			[DebuggerStepThrough]
			get
			{
				return this.numCompressedColumns;
			}
			internal set
			{
				this.numCompressedColumns = value;
			}
		}

		public long cbDataCompressed
		{
			[DebuggerStepThrough]
			get
			{
				return this.userDataAfterCompression;
			}
			internal set
			{
				this.userDataAfterCompression = value;
			}
		}

		public long cbLongValueDataCompressed
		{
			[DebuggerStepThrough]
			get
			{
				return this.userLongValueDataCompressed;
			}
			internal set
			{
				this.userLongValueDataCompressed = value;
			}
		}

		public static JET_RECSIZE Add(JET_RECSIZE s1, JET_RECSIZE s2)
		{
			return checked(new JET_RECSIZE
			{
				cbData = s1.cbData + s2.cbData,
				cbDataCompressed = s1.cbDataCompressed + s2.cbDataCompressed,
				cbLongValueData = s1.cbLongValueData + s2.cbLongValueData,
				cbLongValueDataCompressed = s1.cbLongValueDataCompressed + s2.cbLongValueDataCompressed,
				cbLongValueOverhead = s1.cbLongValueOverhead + s2.cbLongValueOverhead,
				cbOverhead = s1.cbOverhead + s2.cbOverhead,
				cCompressedColumns = s1.cCompressedColumns + s2.cCompressedColumns,
				cLongValues = s1.cLongValues + s2.cLongValues,
				cMultiValues = s1.cMultiValues + s2.cMultiValues,
				cNonTaggedColumns = s1.cNonTaggedColumns + s2.cNonTaggedColumns,
				cTaggedColumns = s1.cTaggedColumns + s2.cTaggedColumns
			});
		}

		public static JET_RECSIZE operator +(JET_RECSIZE left, JET_RECSIZE right)
		{
			return JET_RECSIZE.Add(left, right);
		}

		public static JET_RECSIZE Subtract(JET_RECSIZE s1, JET_RECSIZE s2)
		{
			return checked(new JET_RECSIZE
			{
				cbData = s1.cbData - s2.cbData,
				cbDataCompressed = s1.cbDataCompressed - s2.cbDataCompressed,
				cbLongValueData = s1.cbLongValueData - s2.cbLongValueData,
				cbLongValueDataCompressed = s1.cbLongValueDataCompressed - s2.cbLongValueDataCompressed,
				cbLongValueOverhead = s1.cbLongValueOverhead - s2.cbLongValueOverhead,
				cbOverhead = s1.cbOverhead - s2.cbOverhead,
				cCompressedColumns = s1.cCompressedColumns - s2.cCompressedColumns,
				cLongValues = s1.cLongValues - s2.cLongValues,
				cMultiValues = s1.cMultiValues - s2.cMultiValues,
				cNonTaggedColumns = s1.cNonTaggedColumns - s2.cNonTaggedColumns,
				cTaggedColumns = s1.cTaggedColumns - s2.cTaggedColumns
			});
		}

		public static JET_RECSIZE operator -(JET_RECSIZE left, JET_RECSIZE right)
		{
			return JET_RECSIZE.Subtract(left, right);
		}

		public static bool operator ==(JET_RECSIZE lhs, JET_RECSIZE rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(JET_RECSIZE lhs, JET_RECSIZE rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_RECSIZE)obj);
		}

		public override int GetHashCode()
		{
			long num = this.cbData ^ this.cbDataCompressed << 1 ^ this.cbLongValueData << 2 ^ this.cbDataCompressed << 3 ^ this.cbLongValueDataCompressed << 4 ^ this.cbOverhead << 5 ^ this.cbLongValueOverhead << 6 ^ this.cNonTaggedColumns << 7 ^ this.cTaggedColumns << 8 ^ this.cLongValues << 9 ^ this.cMultiValues << 10 ^ this.cCompressedColumns << 11;
			return (int)(num & (long)((ulong)-1)) ^ (int)(num >> 32);
		}

		public bool Equals(JET_RECSIZE other)
		{
			return this.cbData == other.cbData && this.cbLongValueData == other.cbLongValueData && this.cbOverhead == other.cbOverhead && this.cbLongValueOverhead == other.cbLongValueOverhead && this.cNonTaggedColumns == other.cNonTaggedColumns && this.cTaggedColumns == other.cTaggedColumns && this.cLongValues == other.cLongValues && this.cMultiValues == other.cMultiValues && this.cCompressedColumns == other.cCompressedColumns && this.cbDataCompressed == other.cbDataCompressed && this.cbLongValueDataCompressed == other.cbLongValueDataCompressed;
		}

		internal void SetFromNativeRecsize(NATIVE_RECSIZE value)
		{
			checked
			{
				this.cbData = (long)value.cbData;
				this.cbDataCompressed = (long)value.cbData;
				this.cbLongValueData = (long)value.cbLongValueData;
				this.cbLongValueDataCompressed = (long)value.cbLongValueData;
				this.cbLongValueOverhead = (long)value.cbLongValueOverhead;
				this.cbOverhead = (long)value.cbOverhead;
				this.cCompressedColumns = 0L;
				this.cLongValues = (long)value.cLongValues;
				this.cMultiValues = (long)value.cMultiValues;
				this.cNonTaggedColumns = (long)value.cNonTaggedColumns;
				this.cTaggedColumns = (long)value.cTaggedColumns;
			}
		}

		internal void SetFromNativeRecsize(NATIVE_RECSIZE2 value)
		{
			checked
			{
				this.cbData = (long)value.cbData;
				this.cbDataCompressed = (long)value.cbDataCompressed;
				this.cbLongValueData = (long)value.cbLongValueData;
				this.cbLongValueDataCompressed = (long)value.cbLongValueDataCompressed;
				this.cbLongValueOverhead = (long)value.cbLongValueOverhead;
				this.cbOverhead = (long)value.cbOverhead;
				this.cCompressedColumns = (long)value.cCompressedColumns;
				this.cLongValues = (long)value.cLongValues;
				this.cMultiValues = (long)value.cMultiValues;
				this.cNonTaggedColumns = (long)value.cNonTaggedColumns;
				this.cTaggedColumns = (long)value.cTaggedColumns;
			}
		}

		internal NATIVE_RECSIZE GetNativeRecsize()
		{
			return new NATIVE_RECSIZE
			{
				cbData = (ulong)this.cbData,
				cbLongValueData = (ulong)this.cbLongValueData,
				cbLongValueOverhead = (ulong)this.cbLongValueOverhead,
				cbOverhead = (ulong)this.cbOverhead,
				cLongValues = (ulong)this.cLongValues,
				cMultiValues = (ulong)this.cMultiValues,
				cNonTaggedColumns = (ulong)this.cNonTaggedColumns,
				cTaggedColumns = (ulong)this.cTaggedColumns
			};
		}

		internal NATIVE_RECSIZE2 GetNativeRecsize2()
		{
			return new NATIVE_RECSIZE2
			{
				cbData = (ulong)this.cbData,
				cbDataCompressed = (ulong)this.cbDataCompressed,
				cbLongValueData = (ulong)this.cbLongValueData,
				cbLongValueDataCompressed = (ulong)this.cbLongValueDataCompressed,
				cbLongValueOverhead = (ulong)this.cbLongValueOverhead,
				cbOverhead = (ulong)this.cbOverhead,
				cCompressedColumns = (ulong)this.cCompressedColumns,
				cLongValues = (ulong)this.cLongValues,
				cMultiValues = (ulong)this.cMultiValues,
				cNonTaggedColumns = (ulong)this.cNonTaggedColumns,
				cTaggedColumns = (ulong)this.cTaggedColumns
			};
		}

		private long userData;

		private long userLongValueData;

		private long overhead;

		private long longValueOverhead;

		private long numNonTaggedColumns;

		private long numTaggedColumns;

		private long numLongValues;

		private long numMultiValues;

		private long numCompressedColumns;

		private long userDataAfterCompression;

		private long userLongValueDataCompressed;
	}
}
