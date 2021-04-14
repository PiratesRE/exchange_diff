using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class JET_CONDITIONALCOLUMN : IContentEquatable<JET_CONDITIONALCOLUMN>, IDeepCloneable<JET_CONDITIONALCOLUMN>
	{
		public string szColumnName
		{
			[DebuggerStepThrough]
			get
			{
				return this.columnName;
			}
			set
			{
				this.columnName = value;
			}
		}

		public ConditionalColumnGrbit grbit
		{
			[DebuggerStepThrough]
			get
			{
				return this.option;
			}
			set
			{
				this.option = value;
			}
		}

		public JET_CONDITIONALCOLUMN DeepClone()
		{
			return (JET_CONDITIONALCOLUMN)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_CONDITIONALCOLUMN({0}:{1})", new object[]
			{
				this.columnName,
				this.option
			});
		}

		public bool ContentEquals(JET_CONDITIONALCOLUMN other)
		{
			return other != null && this.columnName == other.columnName && this.option == other.option;
		}

		internal NATIVE_CONDITIONALCOLUMN GetNativeConditionalColumn()
		{
			return new NATIVE_CONDITIONALCOLUMN
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_CONDITIONALCOLUMN)),
				grbit = (uint)this.grbit
			};
		}

		private string columnName;

		private ConditionalColumnGrbit option;
	}
}
