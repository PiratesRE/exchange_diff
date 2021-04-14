using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class JET_TABLECREATE : IContentEquatable<JET_TABLECREATE>, IDeepCloneable<JET_TABLECREATE>
	{
		public string szTableName
		{
			[DebuggerStepThrough]
			get
			{
				return this.tableName;
			}
			set
			{
				this.tableName = value;
			}
		}

		public string szTemplateTableName
		{
			[DebuggerStepThrough]
			get
			{
				return this.templateTableName;
			}
			set
			{
				this.templateTableName = value;
			}
		}

		public int ulPages
		{
			[DebuggerStepThrough]
			get
			{
				return this.initialPageAllocation;
			}
			set
			{
				this.initialPageAllocation = value;
			}
		}

		public int ulDensity
		{
			[DebuggerStepThrough]
			get
			{
				return this.tableDensity;
			}
			set
			{
				this.tableDensity = value;
			}
		}

		public JET_COLUMNCREATE[] rgcolumncreate
		{
			[DebuggerStepThrough]
			get
			{
				return this.columnCreates;
			}
			set
			{
				this.columnCreates = value;
			}
		}

		public int cColumns
		{
			[DebuggerStepThrough]
			get
			{
				return this.columnCreateCount;
			}
			set
			{
				this.columnCreateCount = value;
			}
		}

		public JET_INDEXCREATE[] rgindexcreate
		{
			[DebuggerStepThrough]
			get
			{
				return this.indexCreates;
			}
			set
			{
				this.indexCreates = value;
			}
		}

		public int cIndexes
		{
			[DebuggerStepThrough]
			get
			{
				return this.indexCreateCount;
			}
			set
			{
				this.indexCreateCount = value;
			}
		}

		public string szCallback
		{
			[DebuggerStepThrough]
			get
			{
				return this.callbackFunction;
			}
			set
			{
				this.callbackFunction = value;
			}
		}

		public JET_cbtyp cbtyp
		{
			[DebuggerStepThrough]
			get
			{
				return this.callbackType;
			}
			set
			{
				this.callbackType = value;
			}
		}

		public CreateTableColumnIndexGrbit grbit
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

		public JET_SPACEHINTS pSeqSpacehints
		{
			[DebuggerStepThrough]
			get
			{
				return this.seqSpacehints;
			}
			set
			{
				this.seqSpacehints = value;
			}
		}

		public JET_SPACEHINTS pLVSpacehints
		{
			[DebuggerStepThrough]
			get
			{
				return this.longValueSpacehints;
			}
			set
			{
				this.longValueSpacehints = value;
			}
		}

		public int cbSeparateLV
		{
			[DebuggerStepThrough]
			get
			{
				return this.separateLvThresholdHint;
			}
			set
			{
				this.separateLvThresholdHint = value;
			}
		}

		public JET_TABLEID tableid
		{
			[DebuggerStepThrough]
			get
			{
				return this.tableIdentifier;
			}
			set
			{
				this.tableIdentifier = value;
			}
		}

		public int cCreated
		{
			[DebuggerStepThrough]
			get
			{
				return this.objectsCreated;
			}
			set
			{
				this.objectsCreated = value;
			}
		}

		public bool ContentEquals(JET_TABLECREATE other)
		{
			if (other == null)
			{
				return false;
			}
			this.CheckMembersAreValid();
			other.CheckMembersAreValid();
			return this.szTableName == other.szTableName && this.szTemplateTableName == other.szTemplateTableName && this.ulPages == other.ulPages && this.ulDensity == other.ulDensity && this.cColumns == other.cColumns && this.cIndexes == other.cIndexes && this.szCallback == other.szCallback && this.cbtyp == other.cbtyp && this.grbit == other.grbit && this.cbSeparateLV == other.cbSeparateLV && Util.ObjectContentEquals<JET_SPACEHINTS>(this.pSeqSpacehints, other.pSeqSpacehints) && Util.ObjectContentEquals<JET_SPACEHINTS>(this.pLVSpacehints, other.pLVSpacehints) && this.tableid == other.tableid && this.cCreated == other.cCreated && Util.ArrayObjectContentEquals<JET_COLUMNCREATE>(this.rgcolumncreate, other.rgcolumncreate, this.cColumns) && Util.ArrayObjectContentEquals<JET_INDEXCREATE>(this.rgindexcreate, other.rgindexcreate, this.cIndexes);
		}

		public JET_TABLECREATE DeepClone()
		{
			JET_TABLECREATE jet_TABLECREATE = (JET_TABLECREATE)base.MemberwiseClone();
			jet_TABLECREATE.rgcolumncreate = Util.DeepCloneArray<JET_COLUMNCREATE>(this.rgcolumncreate);
			jet_TABLECREATE.rgindexcreate = Util.DeepCloneArray<JET_INDEXCREATE>(this.rgindexcreate);
			jet_TABLECREATE.seqSpacehints = ((this.seqSpacehints == null) ? null : this.seqSpacehints.DeepClone());
			jet_TABLECREATE.pLVSpacehints = ((this.pLVSpacehints == null) ? null : this.pLVSpacehints.DeepClone());
			return jet_TABLECREATE;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_TABLECREATE({0}:{1} columns:{2} indices)", new object[]
			{
				this.szTableName,
				this.cColumns,
				this.cIndexes
			});
		}

		internal void CheckMembersAreValid()
		{
			if (this.cColumns < 0)
			{
				throw new ArgumentOutOfRangeException("cColumns", this.cColumns, "cannot be negative");
			}
			if (this.rgcolumncreate != null && this.cColumns > this.rgcolumncreate.Length)
			{
				throw new ArgumentOutOfRangeException("cColumns", this.cColumns, "cannot be greater than rgcolumncreate.Length");
			}
			if (this.rgcolumncreate == null && this.cColumns != 0)
			{
				throw new ArgumentOutOfRangeException("cColumns", this.cColumns, "must be zero when rgcolumncreate is null");
			}
			if (this.cIndexes < 0)
			{
				throw new ArgumentOutOfRangeException("cIndexes", this.cIndexes, "cannot be negative");
			}
			if (this.rgindexcreate != null && this.cIndexes > this.rgindexcreate.Length)
			{
				throw new ArgumentOutOfRangeException("cIndexes", this.cIndexes, "cannot be greater than rgindexcreate.Length");
			}
			if (this.rgindexcreate == null && this.cIndexes != 0)
			{
				throw new ArgumentOutOfRangeException("cIndexes", this.cIndexes, "must be zero when rgindexcreate is null");
			}
		}

		internal NATIVE_TABLECREATE2 GetNativeTableCreate2()
		{
			this.CheckMembersAreValid();
			return checked(new NATIVE_TABLECREATE2
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_TABLECREATE2)),
				szTableName = this.szTableName,
				szTemplateTableName = this.szTemplateTableName,
				ulPages = (uint)this.ulPages,
				ulDensity = (uint)this.ulDensity,
				cColumns = (uint)this.cColumns,
				cIndexes = (uint)this.cIndexes,
				szCallback = this.szCallback,
				cbtyp = this.cbtyp,
				grbit = (uint)this.grbit,
				tableid = this.tableid.Value,
				cCreated = (uint)this.cCreated
			});
		}

		internal NATIVE_TABLECREATE3 GetNativeTableCreate3()
		{
			this.CheckMembersAreValid();
			return checked(new NATIVE_TABLECREATE3
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_TABLECREATE3)),
				szTableName = this.szTableName,
				szTemplateTableName = this.szTemplateTableName,
				ulPages = (uint)this.ulPages,
				ulDensity = (uint)this.ulDensity,
				cColumns = (uint)this.cColumns,
				cIndexes = (uint)this.cIndexes,
				szCallback = this.szCallback,
				cbtyp = this.cbtyp,
				grbit = (uint)this.grbit,
				cbSeparateLV = (uint)this.cbSeparateLV,
				tableid = this.tableid.Value,
				cCreated = (uint)this.cCreated
			});
		}

		internal NATIVE_TABLECREATE4 GetNativeTableCreate4()
		{
			this.CheckMembersAreValid();
			return checked(new NATIVE_TABLECREATE4
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_TABLECREATE4)),
				szTableName = this.szTableName,
				szTemplateTableName = this.szTemplateTableName,
				ulPages = (uint)this.ulPages,
				ulDensity = (uint)this.ulDensity,
				cColumns = (uint)this.cColumns,
				cIndexes = (uint)this.cIndexes,
				szCallback = this.szCallback,
				cbtyp = this.cbtyp,
				grbit = (uint)this.grbit,
				cbSeparateLV = (uint)this.cbSeparateLV,
				tableid = this.tableid.Value,
				cCreated = (uint)this.cCreated
			});
		}

		private string tableName;

		private string templateTableName;

		private int initialPageAllocation;

		private int tableDensity;

		private JET_COLUMNCREATE[] columnCreates;

		private int columnCreateCount;

		private JET_INDEXCREATE[] indexCreates;

		private int indexCreateCount;

		private string callbackFunction;

		private JET_cbtyp callbackType;

		private CreateTableColumnIndexGrbit options;

		private JET_SPACEHINTS seqSpacehints;

		private JET_SPACEHINTS longValueSpacehints;

		private int separateLvThresholdHint;

		[NonSerialized]
		private JET_TABLEID tableIdentifier;

		private int objectsCreated;
	}
}
