using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public class JET_RSTINFO : IContentEquatable<JET_RSTINFO>, IDeepCloneable<JET_RSTINFO>
	{
		public JET_RSTMAP[] rgrstmap { get; set; }

		public int crstmap { get; set; }

		public JET_LGPOS lgposStop { get; set; }

		public JET_LOGTIME logtimeStop { get; set; }

		public JET_PFNSTATUS pfnStatus { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_RSTINFO(crstmap={0})", new object[]
			{
				this.crstmap
			});
		}

		public bool ContentEquals(JET_RSTINFO other)
		{
			if (other == null)
			{
				return false;
			}
			this.CheckMembersAreValid();
			other.CheckMembersAreValid();
			return this.crstmap == other.crstmap && this.lgposStop == other.lgposStop && this.logtimeStop == other.logtimeStop && this.pfnStatus == other.pfnStatus && Util.ArrayObjectContentEquals<JET_RSTMAP>(this.rgrstmap, other.rgrstmap, this.crstmap);
		}

		public JET_RSTINFO DeepClone()
		{
			JET_RSTINFO jet_RSTINFO = (JET_RSTINFO)base.MemberwiseClone();
			jet_RSTINFO.rgrstmap = Util.DeepCloneArray<JET_RSTMAP>(this.rgrstmap);
			return jet_RSTINFO;
		}

		internal void CheckMembersAreValid()
		{
			if (this.crstmap < 0)
			{
				throw new ArgumentOutOfRangeException("crstmap", this.crstmap, "cannot be negative");
			}
			if (this.rgrstmap == null && this.crstmap > 0)
			{
				throw new ArgumentOutOfRangeException("crstmap", this.crstmap, "must be zero");
			}
			if (this.rgrstmap != null && this.crstmap > this.rgrstmap.Length)
			{
				throw new ArgumentOutOfRangeException("crstmap", this.crstmap, "cannot be greater than the length of rgrstmap");
			}
		}

		internal NATIVE_RSTINFO GetNativeRstinfo()
		{
			this.CheckMembersAreValid();
			return new NATIVE_RSTINFO
			{
				cbStruct = (uint)NATIVE_RSTINFO.SizeOfRstinfo,
				crstmap = checked((uint)this.crstmap),
				lgposStop = this.lgposStop,
				logtimeStop = this.logtimeStop
			};
		}
	}
}
