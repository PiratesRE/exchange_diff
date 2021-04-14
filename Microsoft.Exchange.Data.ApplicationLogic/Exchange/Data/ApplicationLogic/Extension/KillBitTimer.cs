using System;
using System.Configuration;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	public class KillBitTimer
	{
		public static KillBitTimer Singleton
		{
			get
			{
				return KillBitTimer.killBitTimer;
			}
		}

		public bool IsStarted
		{
			get
			{
				return this.isStarted;
			}
		}

		public int DelayedCheckTimeInSeconds
		{
			get
			{
				return this.delayedCheckTimeInSeconds;
			}
			set
			{
				this.delayedCheckTimeInSeconds = value;
			}
		}

		public void Start()
		{
			if (!this.IsStarted)
			{
				this.internalTimer = new Timer(new TimerCallback(KillBitHelper.DownloadKillBitList));
				string text = ConfigurationManager.AppSettings["KillBitRefreshTimeInSeconds"];
				int num = 0;
				int num2;
				DateTime value;
				long num4;
				if (KillBitHelper.TryReadKillBitFile(out num2, out value))
				{
					long num3 = (long)(num2 * this.DelayedCheckTimeInSeconds * 1000);
					KillBitTimer.Tracer.TraceInformation(0, 0L, string.Format("This refresh rate is {0} milliseconds.", num3));
					TimeSpan timeSpan = DateTime.UtcNow.Subtract(value);
					if ((double)num3 < timeSpan.TotalMilliseconds)
					{
						num4 = (long)this.DelayedCheckTimeInSeconds * 1000L;
					}
					else
					{
						num4 = num3;
					}
				}
				else
				{
					num4 = (long)this.DelayedCheckTimeInSeconds * 1000L;
				}
				if (!string.IsNullOrWhiteSpace(text) && int.TryParse(text, out num) && num > 0)
				{
					this.refreshRateInMillionSecondsFromConfig = num * 1000;
					num4 = (long)this.refreshRateInMillionSecondsFromConfig;
					KillBitTimer.Tracer.TraceInformation(0, 0L, string.Format("This test refresh rate setting is {0} seconds.", num));
				}
				KillBitTimer.Tracer.TraceInformation(0, 0L, string.Format("This time to next download is {0} milliseconds.", num4));
				this.internalTimer.Change(num4, num4);
				this.isStarted = true;
			}
		}

		public void UpdateTimerWithRefreshRate(int refreshRate)
		{
			if (refreshRate <= 0)
			{
				throw new ArgumentException("The refreshRate cannot be equal to or less than zero.");
			}
			if (!this.isStarted || this.internalTimer == null)
			{
				return;
			}
			long num = (long)((this.refreshRateInMillionSecondsFromConfig > 0) ? this.refreshRateInMillionSecondsFromConfig : (refreshRate * this.DelayedCheckTimeInSeconds * 1000));
			KillBitTimer.Tracer.TraceInformation(0, 0L, string.Format("This time to next download is {0} milliseconds.", num));
			this.internalTimer.Change(num, num);
		}

		private const string KillBitRefreshTimeInSecondsKey = "KillBitRefreshTimeInSeconds";

		private const int MilliSecondsInSeconds = 1000;

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private int delayedCheckTimeInSeconds = 3600;

		private int refreshRateInMillionSecondsFromConfig;

		private static KillBitTimer killBitTimer = new KillBitTimer();

		private Timer internalTimer;

		private bool isStarted;
	}
}
