using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class JET_COLUMNDEF : IContentEquatable<JET_COLUMNDEF>, IDeepCloneable<JET_COLUMNDEF>
	{
		public JET_coltyp coltyp
		{
			[DebuggerStepThrough]
			get
			{
				return this.columnType;
			}
			set
			{
				this.columnType = value;
			}
		}

		public JET_CP cp
		{
			[DebuggerStepThrough]
			get
			{
				return this.codePage;
			}
			set
			{
				this.codePage = value;
			}
		}

		public int cbMax
		{
			[DebuggerStepThrough]
			get
			{
				return this.maxSize;
			}
			set
			{
				this.maxSize = value;
			}
		}

		public ColumndefGrbit grbit
		{
			[DebuggerStepThrough]
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		public JET_COLUMNID columnid
		{
			[DebuggerStepThrough]
			get
			{
				return this.id;
			}
			internal set
			{
				this.id = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_COLUMNDEF({0},{1})", new object[]
			{
				this.columnType,
				this.options
			});
		}

		public bool ContentEquals(JET_COLUMNDEF other)
		{
			return other != null && (this.columnType == other.columnType && this.codePage == other.codePage && this.maxSize == other.maxSize && this.id == other.id) && this.options == other.options;
		}

		public JET_COLUMNDEF DeepClone()
		{
			return (JET_COLUMNDEF)base.MemberwiseClone();
		}

		internal NATIVE_COLUMNDEF GetNativeColumndef()
		{
			return checked(new NATIVE_COLUMNDEF
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_COLUMNDEF)),
				cp = unchecked((ushort)this.cp),
				cbMax = (uint)this.cbMax,
				grbit = (uint)this.grbit,
				coltyp = (uint)this.coltyp
			});
		}

		internal void SetFromNativeColumndef(NATIVE_COLUMNDEF value)
		{
			this.coltyp = (JET_coltyp)value.coltyp;
			this.cp = (JET_CP)value.cp;
			this.cbMax = checked((int)value.cbMax);
			this.grbit = (ColumndefGrbit)value.grbit;
			this.columnid = new JET_COLUMNID
			{
				Value = value.columnid
			};
		}

		private JET_coltyp columnType;

		private JET_CP codePage;

		private int maxSize;

		[NonSerialized]
		private JET_COLUMNID id;

		private ColumndefGrbit options;
	}
}
