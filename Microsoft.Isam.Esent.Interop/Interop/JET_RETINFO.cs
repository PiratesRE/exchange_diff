using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_RETINFO : IContentEquatable<JET_RETINFO>, IDeepCloneable<JET_RETINFO>
	{
		public int ibLongValue { get; set; }

		public int itagSequence { get; set; }

		public JET_COLUMNID columnidNextTagged { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_RETINFO(ibLongValue={0},itagSequence={1})", new object[]
			{
				this.ibLongValue,
				this.itagSequence
			});
		}

		public bool ContentEquals(JET_RETINFO other)
		{
			return other != null && (this.ibLongValue == other.ibLongValue && this.itagSequence == other.itagSequence) && this.columnidNextTagged == other.columnidNextTagged;
		}

		public JET_RETINFO DeepClone()
		{
			return (JET_RETINFO)base.MemberwiseClone();
		}

		internal NATIVE_RETINFO GetNativeRetinfo()
		{
			return checked(new NATIVE_RETINFO
			{
				cbStruct = (uint)NATIVE_RETINFO.Size,
				ibLongValue = (uint)this.ibLongValue,
				itagSequence = (uint)this.itagSequence
			});
		}

		internal void SetFromNativeRetinfo(NATIVE_RETINFO value)
		{
			checked
			{
				this.ibLongValue = (int)value.ibLongValue;
				this.itagSequence = (int)value.itagSequence;
				JET_COLUMNID columnidNextTagged = new JET_COLUMNID
				{
					Value = value.columnidNextTagged
				};
				this.columnidNextTagged = columnidNextTagged;
			}
		}
	}
}
