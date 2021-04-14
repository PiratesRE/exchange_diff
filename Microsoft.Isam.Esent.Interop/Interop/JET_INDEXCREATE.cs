using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class JET_INDEXCREATE : IContentEquatable<JET_INDEXCREATE>, IDeepCloneable<JET_INDEXCREATE>
	{
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

		public string szIndexName
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

		public string szKey
		{
			[DebuggerStepThrough]
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		public int cbKey
		{
			[DebuggerStepThrough]
			get
			{
				return this.keyLength;
			}
			set
			{
				this.keyLength = value;
			}
		}

		public CreateIndexGrbit grbit
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

		public int ulDensity
		{
			[DebuggerStepThrough]
			get
			{
				return this.density;
			}
			set
			{
				this.density = value;
			}
		}

		public JET_UNICODEINDEX pidxUnicode
		{
			[DebuggerStepThrough]
			get
			{
				return this.unicodeOptions;
			}
			set
			{
				this.unicodeOptions = value;
			}
		}

		public int cbVarSegMac
		{
			[DebuggerStepThrough]
			get
			{
				return this.maxSegmentLength;
			}
			set
			{
				this.maxSegmentLength = value;
			}
		}

		public JET_CONDITIONALCOLUMN[] rgconditionalcolumn
		{
			[DebuggerStepThrough]
			get
			{
				return this.conditionalColumns;
			}
			set
			{
				this.conditionalColumns = value;
			}
		}

		public int cConditionalColumn
		{
			[DebuggerStepThrough]
			get
			{
				return this.numConditionalColumns;
			}
			set
			{
				this.numConditionalColumns = value;
			}
		}

		public int cbKeyMost
		{
			[DebuggerStepThrough]
			get
			{
				return this.maximumKeyLength;
			}
			set
			{
				this.maximumKeyLength = value;
			}
		}

		public JET_SPACEHINTS pSpaceHints
		{
			[DebuggerStepThrough]
			get
			{
				return this.spaceHints;
			}
			set
			{
				this.spaceHints = value;
			}
		}

		public JET_INDEXCREATE DeepClone()
		{
			JET_INDEXCREATE jet_INDEXCREATE = (JET_INDEXCREATE)base.MemberwiseClone();
			jet_INDEXCREATE.pidxUnicode = ((this.pidxUnicode == null) ? null : this.pidxUnicode.DeepClone());
			this.conditionalColumns = Util.DeepCloneArray<JET_CONDITIONALCOLUMN>(this.conditionalColumns);
			return jet_INDEXCREATE;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_INDEXCREATE({0}:{1})", new object[]
			{
				this.szIndexName,
				this.szKey
			});
		}

		public bool ContentEquals(JET_INDEXCREATE other)
		{
			if (other == null)
			{
				return false;
			}
			this.CheckMembersAreValid();
			other.CheckMembersAreValid();
			return this.err == other.err && this.szIndexName == other.szIndexName && this.szKey == other.szKey && this.cbKey == other.cbKey && this.grbit == other.grbit && this.ulDensity == other.ulDensity && this.cbVarSegMac == other.cbVarSegMac && this.cbKeyMost == other.cbKeyMost && this.IsUnicodeIndexEqual(other) && this.AreConditionalColumnsEqual(other);
		}

		internal void CheckMembersAreValid()
		{
			if (this.szIndexName == null)
			{
				throw new ArgumentNullException("szIndexName");
			}
			if (this.szKey == null)
			{
				throw new ArgumentNullException("szKey");
			}
			if (this.cbKey > checked(this.szKey.Length + 1))
			{
				throw new ArgumentOutOfRangeException("cbKey", this.cbKey, "cannot be greater than the length of szKey");
			}
			if (this.cbKey < 0)
			{
				throw new ArgumentOutOfRangeException("cbKey", this.cbKey, "cannot be negative");
			}
			if (this.ulDensity < 0)
			{
				throw new ArgumentOutOfRangeException("ulDensity", this.ulDensity, "cannot be negative");
			}
			if (this.cbKeyMost < 0)
			{
				throw new ArgumentOutOfRangeException("cbKeyMost", this.cbKeyMost, "cannot be negative");
			}
			if (this.cbVarSegMac < 0)
			{
				throw new ArgumentOutOfRangeException("cbVarSegMac", this.cbVarSegMac, "cannot be negative");
			}
			if ((this.cConditionalColumn > 0 && this.rgconditionalcolumn == null) || (this.cConditionalColumn > 0 && this.cConditionalColumn > this.rgconditionalcolumn.Length))
			{
				throw new ArgumentOutOfRangeException("cConditionalColumn", this.cConditionalColumn, "cannot be greater than the length of rgconditionalcolumn");
			}
			if (this.cConditionalColumn < 0)
			{
				throw new ArgumentOutOfRangeException("cConditionalColumn", this.cConditionalColumn, "cannot be negative");
			}
		}

		internal NATIVE_INDEXCREATE GetNativeIndexcreate()
		{
			this.CheckMembersAreValid();
			return checked(new NATIVE_INDEXCREATE
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_INDEXCREATE)),
				cbKey = (uint)this.cbKey,
				grbit = (uint)this.grbit,
				ulDensity = (uint)this.ulDensity,
				cbVarSegMac = new IntPtr(this.cbVarSegMac),
				cConditionalColumn = (uint)this.cConditionalColumn
			});
		}

		internal NATIVE_INDEXCREATE1 GetNativeIndexcreate1()
		{
			NATIVE_INDEXCREATE1 result = default(NATIVE_INDEXCREATE1);
			result.indexcreate = this.GetNativeIndexcreate();
			checked
			{
				result.indexcreate.cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_INDEXCREATE1));
				if (this.cbKeyMost != 0)
				{
					result.cbKeyMost = (uint)this.cbKeyMost;
					result.indexcreate.grbit = (result.indexcreate.grbit | 32768U);
				}
				return result;
			}
		}

		internal NATIVE_INDEXCREATE2 GetNativeIndexcreate2()
		{
			NATIVE_INDEXCREATE2 result = default(NATIVE_INDEXCREATE2);
			result.indexcreate1 = this.GetNativeIndexcreate1();
			result.indexcreate1.indexcreate.cbStruct = checked((uint)Marshal.SizeOf(typeof(NATIVE_INDEXCREATE2)));
			return result;
		}

		internal void SetFromNativeIndexCreate(NATIVE_INDEXCREATE2 value)
		{
			this.SetFromNativeIndexCreate(value.indexcreate1);
		}

		internal void SetFromNativeIndexCreate(NATIVE_INDEXCREATE1 value)
		{
			this.SetFromNativeIndexCreate(value.indexcreate);
		}

		internal void SetFromNativeIndexCreate(NATIVE_INDEXCREATE value)
		{
			this.err = (JET_err)value.err;
		}

		private bool IsUnicodeIndexEqual(JET_INDEXCREATE other)
		{
			if (this.pidxUnicode != null)
			{
				return this.pidxUnicode.ContentEquals(other.pidxUnicode);
			}
			return null == other.pidxUnicode;
		}

		private bool AreConditionalColumnsEqual(JET_INDEXCREATE other)
		{
			if (this.cConditionalColumn != other.cConditionalColumn)
			{
				return false;
			}
			for (int i = 0; i < this.cConditionalColumn; i++)
			{
				if (!this.rgconditionalcolumn[i].ContentEquals(other.rgconditionalcolumn[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal NATIVE_INDEXCREATE3 GetNativeIndexcreate3()
		{
			this.CheckMembersAreValid();
			NATIVE_INDEXCREATE3 result = default(NATIVE_INDEXCREATE3);
			checked
			{
				result.cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_INDEXCREATE3));
				result.cbKey = (uint)this.cbKey * 2U;
				result.grbit = (uint)this.grbit;
				result.ulDensity = (uint)this.ulDensity;
				result.cbVarSegMac = new IntPtr(this.cbVarSegMac);
				result.cConditionalColumn = (uint)this.cConditionalColumn;
				if (this.cbKeyMost != 0)
				{
					result.cbKeyMost = (uint)this.cbKeyMost;
					result.grbit |= 32768U;
				}
				return result;
			}
		}

		internal void SetFromNativeIndexCreate(NATIVE_INDEXCREATE3 value)
		{
			this.err = (JET_err)value.err;
		}

		private string name;

		private string key;

		private int keyLength;

		private CreateIndexGrbit options;

		private int density;

		private JET_UNICODEINDEX unicodeOptions;

		private int maxSegmentLength;

		private JET_CONDITIONALCOLUMN[] conditionalColumns;

		private int numConditionalColumns;

		private JET_err errorCode;

		private int maximumKeyLength;

		private JET_SPACEHINTS spaceHints;
	}
}
