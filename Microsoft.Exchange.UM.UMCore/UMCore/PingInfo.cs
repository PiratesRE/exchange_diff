using System;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PingInfo
	{
		internal PingInfo(UMSipPeer peer)
		{
			this.Peer = peer;
			this.ResponseText = string.Empty;
			this.Diagnostics = string.Empty;
		}

		internal UMSipPeer Peer { get; private set; }

		internal string TargetUri
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "sip:{0}:{1}", new object[]
				{
					this.Peer.Address,
					this.Peer.Port
				});
			}
		}

		internal int ResponseCode { get; set; }

		internal string ResponseText
		{
			get
			{
				return this.responseText;
			}
			set
			{
				if (value != null)
				{
					this.responseText = value;
					return;
				}
				this.responseText = string.Empty;
			}
		}

		internal string Diagnostics { get; set; }

		internal Exception Error { get; set; }

		private ExDateTime? StartTime { get; set; }

		private ExDateTime? EndTime { get; set; }

		internal void RecordStartTime()
		{
			this.StartTime = new ExDateTime?(ExDateTime.UtcNow);
			this.EndTime = null;
		}

		internal void RecordStopTime()
		{
			this.EndTime = new ExDateTime?(ExDateTime.UtcNow);
		}

		internal TimeSpan GetElaspedTime()
		{
			if (this.EndTime == null || this.StartTime == null)
			{
				throw new InvalidOperationException("PingInfo.RecordStartTime and RecordStopTime must be called before GetElaspedTime");
			}
			return this.EndTime.Value.Subtract(this.StartTime.Value);
		}

		private string responseText;
	}
}
