using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class JET_RECPOS : IContentEquatable<JET_RECPOS>, IDeepCloneable<JET_RECPOS>
	{
		public long centriesLT
		{
			[DebuggerStepThrough]
			get
			{
				return this.entriesBeforeKey;
			}
			set
			{
				this.entriesBeforeKey = value;
			}
		}

		public long centriesTotal
		{
			[DebuggerStepThrough]
			get
			{
				return this.totalEntries;
			}
			set
			{
				this.totalEntries = value;
			}
		}

		public JET_RECPOS DeepClone()
		{
			return (JET_RECPOS)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_RECPOS({0}/{1})", new object[]
			{
				this.entriesBeforeKey,
				this.totalEntries
			});
		}

		public bool ContentEquals(JET_RECPOS other)
		{
			return other != null && this.entriesBeforeKey == other.entriesBeforeKey && this.totalEntries == other.totalEntries;
		}

		internal NATIVE_RECPOS GetNativeRecpos()
		{
			return checked(new NATIVE_RECPOS
			{
				cbStruct = (uint)NATIVE_RECPOS.Size,
				centriesLT = (uint)this.centriesLT,
				centriesTotal = (uint)this.centriesTotal
			});
		}

		internal void SetFromNativeRecpos(NATIVE_RECPOS value)
		{
			this.centriesLT = (long)(checked((int)value.centriesLT));
			this.centriesTotal = (long)(checked((int)value.centriesTotal));
		}

		private long entriesBeforeKey;

		private long totalEntries;
	}
}
