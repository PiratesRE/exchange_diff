using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_PAGEINFO : IContentEquatable<JET_PAGEINFO>, IDeepCloneable<JET_PAGEINFO>
	{
		public int pgno { [DebuggerStepThrough] get; set; }

		public bool fPageIsInitialized { [DebuggerStepThrough] get; internal set; }

		public bool fCorrectableError { [DebuggerStepThrough] get; internal set; }

		public long checksumActual { [DebuggerStepThrough] get; internal set; }

		public long checksumExpected { [DebuggerStepThrough] get; internal set; }

		public long dbtime { [DebuggerStepThrough] get; internal set; }

		public long structureChecksum { [DebuggerStepThrough] get; internal set; }

		public long flags { [DebuggerStepThrough] get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_PAGEINFO({0}:{1})", new object[]
			{
				this.pgno,
				this.dbtime
			});
		}

		public bool ContentEquals(JET_PAGEINFO other)
		{
			if (other == null)
			{
				return false;
			}
			this.CheckMembersAreValid();
			other.CheckMembersAreValid();
			return this.pgno == other.pgno && this.fPageIsInitialized == other.fPageIsInitialized && this.fCorrectableError == other.fCorrectableError && this.checksumActual == other.checksumActual && this.checksumExpected == other.checksumExpected && this.dbtime == other.dbtime && this.structureChecksum == other.structureChecksum && this.flags == other.flags;
		}

		public JET_PAGEINFO DeepClone()
		{
			return (JET_PAGEINFO)base.MemberwiseClone();
		}

		internal void CheckMembersAreValid()
		{
			if (this.pgno < 0)
			{
				throw new ArgumentOutOfRangeException("pgno", this.pgno, "cannot be negative");
			}
			if (this.dbtime < 0L)
			{
				throw new ArgumentOutOfRangeException("dbtime", this.dbtime, "cannot be negative");
			}
		}

		internal NATIVE_PAGEINFO GetNativePageinfo()
		{
			this.CheckMembersAreValid();
			NATIVE_PAGEINFO result = default(NATIVE_PAGEINFO);
			BitVector32 bitVector = new BitVector32(0);
			int num = BitVector32.CreateMask();
			int bit = BitVector32.CreateMask(num);
			bitVector[num] = this.fPageIsInitialized;
			bitVector[bit] = this.fCorrectableError;
			checked
			{
				result.bitField = (uint)bitVector.Data;
				result.pgno = (uint)this.pgno;
				result.checksumActual = (ulong)this.checksumActual;
				result.checksumExpected = (ulong)this.checksumExpected;
				result.dbtime = (ulong)this.dbtime;
				result.structureChecksum = (ulong)this.structureChecksum;
				result.flags = (ulong)this.flags;
				return result;
			}
		}

		internal void SetFromNativePageInfo(ref NATIVE_PAGEINFO native)
		{
			int num = BitVector32.CreateMask();
			int bit = BitVector32.CreateMask(num);
			checked
			{
				BitVector32 bitVector = new BitVector32((int)native.bitField);
				this.fPageIsInitialized = bitVector[num];
				this.fCorrectableError = bitVector[bit];
				this.pgno = (int)native.pgno;
				this.checksumActual = (long)native.checksumActual;
				this.checksumExpected = (long)native.checksumExpected;
				this.dbtime = (long)native.dbtime;
				this.structureChecksum = (long)native.structureChecksum;
				this.flags = (long)native.flags;
			}
		}
	}
}
