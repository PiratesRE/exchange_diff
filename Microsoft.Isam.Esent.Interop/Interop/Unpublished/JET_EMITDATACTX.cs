using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_EMITDATACTX : IContentEquatable<JET_EMITDATACTX>, IDeepCloneable<JET_EMITDATACTX>
	{
		public int dwVersion { get; set; }

		public ulong qwSequenceNum { get; set; }

		public ShadowLogEmitGrbit grbitOperationalFlags { get; set; }

		public DateTime logtimeEmit { get; set; }

		public JET_LGPOS lgposLogData { get; set; }

		public long cbLogData { get; set; }

		public JET_EMITDATACTX DeepClone()
		{
			return (JET_EMITDATACTX)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_EMITDATACTX({0}:{1}:{2}:{3}:{4})", new object[]
			{
				this.dwVersion,
				this.qwSequenceNum,
				this.grbitOperationalFlags,
				this.lgposLogData,
				this.logtimeEmit
			});
		}

		public bool ContentEquals(JET_EMITDATACTX other)
		{
			return other != null && (this.dwVersion == other.dwVersion && this.qwSequenceNum == other.qwSequenceNum && this.grbitOperationalFlags == other.grbitOperationalFlags && this.logtimeEmit == other.logtimeEmit && this.lgposLogData == other.lgposLogData) && this.cbLogData == other.cbLogData;
		}

		internal NATIVE_EMITDATACTX GetNativeEmitdatactx()
		{
			return checked(new NATIVE_EMITDATACTX
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_EMITDATACTX)),
				dwVersion = (uint)this.dwVersion,
				qwSequenceNum = this.qwSequenceNum,
				grbitOperationalFlags = (uint)this.grbitOperationalFlags,
				logtimeEmit = new JET_LOGTIME(this.logtimeEmit),
				lgposLogData = this.lgposLogData,
				cbLogData = (uint)this.cbLogData
			});
		}

		internal void SetFromNative(ref NATIVE_EMITDATACTX native)
		{
			this.dwVersion = (int)native.dwVersion;
			this.qwSequenceNum = native.qwSequenceNum;
			this.grbitOperationalFlags = (ShadowLogEmitGrbit)native.grbitOperationalFlags;
			this.logtimeEmit = (native.logtimeEmit.ToDateTime() ?? DateTime.MinValue);
			this.lgposLogData = native.lgposLogData;
			this.cbLogData = (long)((ulong)native.cbLogData);
		}
	}
}
