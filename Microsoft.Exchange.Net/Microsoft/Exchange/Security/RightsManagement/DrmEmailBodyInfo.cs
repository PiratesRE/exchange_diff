using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	public struct DrmEmailBodyInfo
	{
		public DrmEmailBodyInfo(BodyFormat bodyFormat, int cpId)
		{
			this.bodyFormat = bodyFormat;
			this.cpId = cpId;
		}

		public BodyFormat BodyFormat
		{
			get
			{
				return this.bodyFormat;
			}
		}

		public int CodePage
		{
			get
			{
				return this.cpId;
			}
		}

		public static bool operator ==(DrmEmailBodyInfo drmEmailBodyInfo1, DrmEmailBodyInfo drmEmailBodyInfo2)
		{
			return drmEmailBodyInfo1.Equals(drmEmailBodyInfo2);
		}

		public static bool operator !=(DrmEmailBodyInfo drmEmailBodyInfo1, DrmEmailBodyInfo drmEmailBodyInfo2)
		{
			return !drmEmailBodyInfo1.Equals(drmEmailBodyInfo2);
		}

		public override int GetHashCode()
		{
			return this.cpId.GetHashCode() ^ this.bodyFormat.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is DrmEmailBodyInfo && this.Equals((DrmEmailBodyInfo)obj);
		}

		public bool Equals(DrmEmailBodyInfo other)
		{
			return this.bodyFormat == other.bodyFormat && this.cpId == other.cpId;
		}

		private BodyFormat bodyFormat;

		private int cpId;
	}
}
