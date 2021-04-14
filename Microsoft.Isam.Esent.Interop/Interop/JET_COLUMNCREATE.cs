using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class JET_COLUMNCREATE : IContentEquatable<JET_COLUMNCREATE>, IDeepCloneable<JET_COLUMNCREATE>
	{
		public string szColumnName
		{
			[DebuggerStepThrough]
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

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

		public byte[] pvDefault
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value;
			}
		}

		public int cbDefault
		{
			get
			{
				return this.defaultValueSize;
			}
			set
			{
				this.defaultValueSize = value;
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

		public JET_err err
		{
			[DebuggerStepThrough]
			get
			{
				return this.errorCode;
			}
			set
			{
				this.errorCode = value;
			}
		}

		public bool ContentEquals(JET_COLUMNCREATE other)
		{
			if (other == null)
			{
				return false;
			}
			this.CheckMembersAreValid();
			other.CheckMembersAreValid();
			return this.err == other.err && this.szColumnName == other.szColumnName && this.coltyp == other.coltyp && this.cbMax == other.cbMax && this.grbit == other.grbit && this.cbDefault == other.cbDefault && this.cp == other.cp && this.columnid == other.columnid && Util.ArrayEqual(this.pvDefault, other.pvDefault, 0, other.cbDefault);
		}

		public JET_COLUMNCREATE DeepClone()
		{
			JET_COLUMNCREATE jet_COLUMNCREATE = (JET_COLUMNCREATE)base.MemberwiseClone();
			if (this.pvDefault != null)
			{
				jet_COLUMNCREATE.pvDefault = new byte[this.pvDefault.Length];
				Array.Copy(this.pvDefault, jet_COLUMNCREATE.pvDefault, this.pvDefault.Length);
			}
			return jet_COLUMNCREATE;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_COLUMNCREATE({0},{1},{2})", new object[]
			{
				this.szColumnName,
				this.coltyp,
				this.grbit
			});
		}

		internal void CheckMembersAreValid()
		{
			if (this.szColumnName == null)
			{
				throw new ArgumentNullException("szColumnName");
			}
			if (this.cbDefault < 0)
			{
				throw new ArgumentOutOfRangeException("cbDefault", this.cbDefault, "cannot be negative");
			}
			if (this.pvDefault == null && this.cbDefault != 0)
			{
				throw new ArgumentOutOfRangeException("cbDefault", this.cbDefault, "must be 0");
			}
			if (this.pvDefault != null && this.cbDefault > this.pvDefault.Length)
			{
				throw new ArgumentOutOfRangeException("cbDefault", this.cbDefault, "can't be greater than pvDefault.Length");
			}
		}

		internal NATIVE_COLUMNCREATE GetNativeColumnCreate()
		{
			return checked(new NATIVE_COLUMNCREATE
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_COLUMNCREATE)),
				szColumnName = IntPtr.Zero,
				coltyp = (uint)this.coltyp,
				cbMax = (uint)this.cbMax,
				grbit = (uint)this.grbit,
				pvDefault = IntPtr.Zero,
				cbDefault = (uint)this.cbDefault,
				cp = (uint)this.cp
			});
		}

		internal void SetFromNativeColumnCreate(NATIVE_COLUMNCREATE value)
		{
			this.columnid = new JET_COLUMNID
			{
				Value = value.columnid
			};
			this.err = (JET_err)value.err;
		}

		private string name;

		private JET_coltyp columnType;

		private int maxSize;

		private ColumndefGrbit options;

		private byte[] defaultValue;

		private int defaultValueSize;

		private JET_CP codePage;

		[NonSerialized]
		private JET_COLUMNID id;

		private JET_err errorCode;
	}
}
