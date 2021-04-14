using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Isam.Esent.Interop
{
	public sealed class ColumnInfo
	{
		internal ColumnInfo(string name, JET_COLUMNID columnid, JET_coltyp coltyp, JET_CP cp, int maxLength, byte[] defaultValue, ColumndefGrbit grbit)
		{
			this.Name = name;
			this.Columnid = columnid;
			this.Coltyp = coltyp;
			this.Cp = cp;
			this.MaxLength = maxLength;
			this.defaultValue = ((defaultValue == null) ? null : new ReadOnlyCollection<byte>(defaultValue));
			this.Grbit = grbit;
		}

		public string Name { get; private set; }

		public JET_COLUMNID Columnid { get; private set; }

		public JET_coltyp Coltyp { get; private set; }

		public JET_CP Cp { get; private set; }

		public int MaxLength { get; private set; }

		public IList<byte> DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		public ColumndefGrbit Grbit { get; private set; }

		public override string ToString()
		{
			return this.Name;
		}

		private readonly ReadOnlyCollection<byte> defaultValue;
	}
}
