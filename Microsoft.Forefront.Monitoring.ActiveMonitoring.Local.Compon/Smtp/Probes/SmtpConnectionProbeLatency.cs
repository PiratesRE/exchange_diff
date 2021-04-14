using System;
using System.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class SmtpConnectionProbeLatency
	{
		public SmtpConnectionProbeLatency(string reason, bool startNow)
		{
			this.Reason = reason;
			if (startNow)
			{
				this.StartRecording();
			}
		}

		public ExDateTime StartTime { get; private set; }

		public string Reason { get; private set; }

		public long Latency
		{
			get
			{
				if (this.Stopwatch != null)
				{
					return this.Stopwatch.ElapsedMilliseconds;
				}
				return 0L;
			}
		}

		private Stopwatch Stopwatch { get; set; }

		public void StartRecording()
		{
			this.Stopwatch = new Stopwatch();
			this.StartTime = ExDateTime.Now;
			this.Stopwatch.Start();
		}

		public void StopRecording()
		{
			if (this.Stopwatch != null)
			{
				this.Stopwatch.Stop();
			}
		}

		public override string ToString()
		{
			return string.Format("{0}={1}", this.Reason, this.Stopwatch.ElapsedMilliseconds);
		}
	}
}
