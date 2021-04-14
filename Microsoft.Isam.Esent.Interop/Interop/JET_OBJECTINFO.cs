using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_OBJECTINFO
	{
		public JET_objtyp objtyp { get; private set; }

		public ObjectInfoGrbit grbit { get; private set; }

		public ObjectInfoFlags flags { get; private set; }

		public int cRecord { get; private set; }

		public int cPage { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_OBJECTINFO({0})", new object[]
			{
				this.flags
			});
		}

		internal void SetFromNativeObjectinfo(ref NATIVE_OBJECTINFO value)
		{
			this.objtyp = (JET_objtyp)value.objtyp;
			this.grbit = (ObjectInfoGrbit)value.grbit;
			this.flags = (ObjectInfoFlags)value.flags;
			this.cRecord = (int)value.cRecord;
			this.cPage = (int)value.cPage;
		}
	}
}
