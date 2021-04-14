using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	[Serializable]
	public sealed class JET_ERRINFOBASIC : IContentEquatable<JET_ERRINFOBASIC>, IDeepCloneable<JET_ERRINFOBASIC>
	{
		public JET_ERRINFOBASIC()
		{
			this.rgCategoricalHierarchy = new JET_ERRCAT[8];
		}

		public JET_err errValue
		{
			[DebuggerStepThrough]
			get
			{
				return this.errorValue;
			}
			set
			{
				this.errorValue = value;
			}
		}

		public JET_ERRCAT errcat
		{
			[DebuggerStepThrough]
			get
			{
				return this.errorcatMostSpecific;
			}
			set
			{
				this.errorcatMostSpecific = value;
			}
		}

		public JET_ERRCAT[] rgCategoricalHierarchy
		{
			[DebuggerStepThrough]
			get
			{
				return this.arrayCategoricalHierarchy;
			}
			set
			{
				this.arrayCategoricalHierarchy = value;
			}
		}

		public int lSourceLine
		{
			[DebuggerStepThrough]
			get
			{
				return this.sourceLine;
			}
			set
			{
				this.sourceLine = value;
			}
		}

		public string rgszSourceFile
		{
			[DebuggerStepThrough]
			get
			{
				return this.sourceFile;
			}
			set
			{
				this.sourceFile = value;
			}
		}

		public JET_ERRINFOBASIC DeepClone()
		{
			return (JET_ERRINFOBASIC)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_ERRINFOBASIC({0}:{1}:{2}:{3})", new object[]
			{
				this.errValue,
				this.errcat,
				this.rgszSourceFile,
				this.lSourceLine
			});
		}

		public bool ContentEquals(JET_ERRINFOBASIC other)
		{
			return other != null && (this.errValue == other.errValue && this.errcat == other.errcat && this.lSourceLine == other.lSourceLine && this.rgszSourceFile == other.rgszSourceFile) && Util.ArrayStructEquals<JET_ERRCAT>(this.rgCategoricalHierarchy, other.rgCategoricalHierarchy, (this.rgCategoricalHierarchy == null) ? 0 : this.rgCategoricalHierarchy.Length);
		}

		internal NATIVE_ERRINFOBASIC GetNativeErrInfo()
		{
			NATIVE_ERRINFOBASIC result = default(NATIVE_ERRINFOBASIC);
			result.cbStruct = checked((uint)Marshal.SizeOf(typeof(NATIVE_ERRINFOBASIC)));
			result.errValue = this.errValue;
			result.errcatMostSpecific = this.errcat;
			result.rgCategoricalHierarchy = new byte[8];
			if (this.rgCategoricalHierarchy != null)
			{
				for (int i = 0; i < this.rgCategoricalHierarchy.Length; i++)
				{
					result.rgCategoricalHierarchy[i] = (byte)this.rgCategoricalHierarchy[i];
				}
			}
			result.lSourceLine = (uint)this.lSourceLine;
			result.rgszSourceFile = this.rgszSourceFile;
			return result;
		}

		internal void SetFromNative(ref NATIVE_ERRINFOBASIC value)
		{
			this.errValue = value.errValue;
			this.errcat = value.errcatMostSpecific;
			if (value.rgCategoricalHierarchy != null)
			{
				for (int i = 0; i < value.rgCategoricalHierarchy.Length; i++)
				{
					this.rgCategoricalHierarchy[i] = (JET_ERRCAT)value.rgCategoricalHierarchy[i];
				}
			}
			this.lSourceLine = (int)value.lSourceLine;
			this.rgszSourceFile = value.rgszSourceFile;
		}

		private JET_err errorValue;

		private JET_ERRCAT errorcatMostSpecific;

		private JET_ERRCAT[] arrayCategoricalHierarchy;

		private int sourceLine;

		private string sourceFile;
	}
}
