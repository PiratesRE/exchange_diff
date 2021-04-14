using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class JET_SETINFO : IContentEquatable<JET_SETINFO>, IDeepCloneable<JET_SETINFO>
	{
		public int ibLongValue
		{
			get
			{
				return this.longValueOffset;
			}
			set
			{
				this.longValueOffset = value;
			}
		}

		public int itagSequence
		{
			get
			{
				return this.itag;
			}
			set
			{
				this.itag = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SETINFO(ibLongValue={0},itagSequence={1})", new object[]
			{
				this.ibLongValue,
				this.itagSequence
			});
		}

		public bool ContentEquals(JET_SETINFO other)
		{
			return other != null && this.ibLongValue == other.ibLongValue && this.itagSequence == other.itagSequence;
		}

		public JET_SETINFO DeepClone()
		{
			return (JET_SETINFO)base.MemberwiseClone();
		}

		internal NATIVE_SETINFO GetNativeSetinfo()
		{
			return checked(new NATIVE_SETINFO
			{
				cbStruct = (uint)NATIVE_SETINFO.Size,
				ibLongValue = (uint)this.ibLongValue,
				itagSequence = (uint)this.itagSequence
			});
		}

		private int longValueOffset;

		private int itag;
	}
}
