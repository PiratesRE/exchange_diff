using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class JET_CHECKPOINTINFO : IEquatable<JET_CHECKPOINTINFO>
	{
		public int genMin
		{
			[DebuggerStepThrough]
			get
			{
				return this._genMin;
			}
			internal set
			{
				this._genMin = value;
			}
		}

		public int genMax
		{
			[DebuggerStepThrough]
			get
			{
				return this._genMax;
			}
			internal set
			{
				this._genMax = value;
			}
		}

		public JET_LOGTIME logtimeGenMaxCreate
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeGenMaxCreate;
			}
			internal set
			{
				this._logtimeGenMaxCreate = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_CHECKPOINTINFO({0},{1},{2})", new object[]
			{
				this._genMin,
				this._genMax,
				this._logtimeGenMaxCreate
			});
		}

		public override int GetHashCode()
		{
			int[] hashes = new int[]
			{
				this._genMin,
				this._genMax,
				this._logtimeGenMaxCreate.GetHashCode()
			};
			return Util.CalculateHashCode(hashes);
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_CHECKPOINTINFO)obj);
		}

		public bool Equals(JET_CHECKPOINTINFO other)
		{
			return other != null && (this._genMin == other._genMin && this._genMax == other._genMax) && this._logtimeGenMaxCreate == other._logtimeGenMaxCreate;
		}

		internal void SetFromNativeCheckpointInfo(ref NATIVE_CHECKPOINTINFO native)
		{
			this._genMin = (int)native.genMin;
			this._genMax = (int)native.genMax;
			this._logtimeGenMaxCreate = native.logtimeGenMaxCreate;
		}

		internal NATIVE_CHECKPOINTINFO GetNativeCheckpointinfo()
		{
			return new NATIVE_CHECKPOINTINFO
			{
				genMin = (uint)this._genMin,
				genMax = (uint)this._genMax,
				logtimeGenMaxCreate = this._logtimeGenMaxCreate
			};
		}

		private int _genMin;

		private int _genMax;

		private JET_LOGTIME _logtimeGenMaxCreate;
	}
}
