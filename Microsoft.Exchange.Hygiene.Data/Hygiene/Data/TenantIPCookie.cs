using System;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class TenantIPCookie
	{
		public TenantIPCookie()
		{
			this.watermarkTime = SqlDateTime.MinValue.Value;
		}

		public TenantIPCookie(DateTime watermarkTime)
		{
			this.watermarkTime = watermarkTime;
		}

		public DateTime UpdateWatermark()
		{
			DateTime utcNow = DateTime.UtcNow;
			this.oldWatermarkTime = new DateTime?(this.watermarkTime);
			this.watermarkTime = utcNow;
			return this.oldWatermarkTime.Value;
		}

		public void RevertToOldWatermark()
		{
			if (this.oldWatermarkTime != null)
			{
				this.watermarkTime = this.oldWatermarkTime.Value;
				this.oldWatermarkTime = null;
			}
		}

		public void CommitNewWatermark()
		{
			this.oldWatermarkTime = null;
		}

		private DateTime? oldWatermarkTime;

		private DateTime watermarkTime;
	}
}
