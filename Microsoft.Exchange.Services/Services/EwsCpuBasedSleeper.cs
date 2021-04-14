using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services
{
	internal class EwsCpuBasedSleeper : IDisposable
	{
		public static EwsCpuBasedSleeper Singleton
		{
			get
			{
				if (EwsCpuBasedSleeper.singleton == null)
				{
					lock (EwsCpuBasedSleeper.lockObj)
					{
						if (EwsCpuBasedSleeper.singleton == null)
						{
							if (Global.GetAppSettingAsBool("EwsCpuBasedDelayEnabled", false))
							{
								EwsCpuBasedSleeper.singleton = new EwsCpuBasedSleeper.EwsSleeper();
							}
							else
							{
								EwsCpuBasedSleeper.singleton = new EwsCpuBasedSleeper();
							}
						}
					}
				}
				return EwsCpuBasedSleeper.singleton;
			}
		}

		public virtual double GetDelayTime()
		{
			return 0.0;
		}

		public virtual void Dispose()
		{
		}

		private const string AppSettingEwsCpuBasedDelayEnabled = "EwsCpuBasedDelayEnabled";

		private static EwsCpuBasedSleeper singleton;

		private static object lockObj = new object();

		private class EwsSleeper : EwsCpuBasedSleeper
		{
			internal EwsSleeper()
			{
				this.MaxDelayOnHighSystemCpu = (double)Global.GetAppSettingAsInt("MaxDelayOnHighSystemCpu", 8000);
				this.MaxDelayOnHighProcessCpu = (double)Global.GetAppSettingAsInt("MaxDelayOnHighProcessCpu", 4000);
				this.callsThreshold = Global.GetAppSettingAsInt("CallsThresholdOnHighCpu", 20);
				this.processCpuPerfCounter = new ProcessCPURunningAveragePerfCounterReader();
				this.processCpuThreshold = (float)Global.GetAppSettingAsInt("HighProcessCpuThreshold", 80);
				if (this.processCpuThreshold >= 100f || this.processCpuThreshold <= 0f)
				{
					this.processCpuThreshold = 80f;
				}
				this.systemCpuThreshold = (float)Global.GetAppSettingAsInt("HighSystemCpuThreshold", 80);
				if (this.systemCpuThreshold >= 100f || this.systemCpuThreshold <= 0f)
				{
					this.systemCpuThreshold = 80f;
				}
				this.timer = new System.Timers.Timer(60000.0);
				this.timer.Elapsed += this.UpdateCallHistory;
				this.timer.Start();
				this.newCopy = new Dictionary<string, int>(2000);
			}

			private void UpdateCallHistory(object sender, ElapsedEventArgs e)
			{
				this.oldCopy = this.newCopy;
				Interlocked.Exchange<Dictionary<string, int>>(ref this.newCopy, new Dictionary<string, int>(2000));
				lock (this.oldCopyLock)
				{
					foreach (string key in this.oldCopy.Keys)
					{
						int num;
						if (this.oldCopy.TryGetValue(key, out num) && num > 10)
						{
							int num2 = num / 10;
							int num3 = num2;
							lock (this.newCopyLock)
							{
								int num4 = 0;
								if (this.newCopy.TryGetValue(key, out num4))
								{
									num3 += num4;
								}
								this.newCopy[key] = num3;
							}
						}
					}
				}
			}

			public override void Dispose()
			{
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
			}

			public override double GetDelayTime()
			{
				double num = ServerCPUUsage.GetCurrentUsagePercentage();
				double num2 = (double)(this.processCpuPerfCounter.GetValue() * (float)Environment.ProcessorCount);
				if (HttpContext.Current == null || HttpContext.Current.Request == null)
				{
					return 0.0;
				}
				HttpRequest request = HttpContext.Current.Request;
				if (!request.IsAuthenticated && request.UserAgent != null && !request.UserAgent.StartsWith("ExchangeWebServicesProxy", StringComparison.OrdinalIgnoreCase) && !request.UserAgent.StartsWith("ASProxy", StringComparison.OrdinalIgnoreCase) && !request.UserAgent.StartsWith("OwaProxy", StringComparison.OrdinalIgnoreCase) && num >= (double)this.systemCpuThreshold)
				{
					RequestDetailsLogger.Current.AppendGenericInfo("BoxCpu", num);
					return (num - (double)this.systemCpuThreshold) / (100.0 - (double)this.systemCpuThreshold) * this.MaxDelayOnHighSystemCpu;
				}
				CallContext callContext = CallContext.Current;
				if (callContext == null)
				{
					return 0.0;
				}
				if (callContext.AccessingPrincipal == null)
				{
					RequestDetailsLogger.Current.AppendGenericInfo("BadKey", callContext.OriginalCallerContext.IdentifierString);
					return 0.0;
				}
				string text = string.Format("{0}:{1}", callContext.OriginalCallerContext.IdentifierString, callContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress);
				int num3 = 1;
				lock (this.newCopyLock)
				{
					int num4;
					if (this.newCopy.TryGetValue(text, out num4))
					{
						num3 += num4;
					}
					this.newCopy[text] = num3;
				}
				if (num2 >= (double)this.processCpuThreshold && num3 > this.callsThreshold)
				{
					RequestDetailsLogger.Current.AppendGenericInfo("Key", text);
					RequestDetailsLogger.Current.AppendGenericInfo("BoxCpu", num.ToString("F0"));
					RequestDetailsLogger.Current.AppendGenericInfo("EwsCpu", num2.ToString("F0"));
					RequestDetailsLogger.Current.AppendGenericInfo("Calls", num3);
					double val = (num - (double)this.systemCpuThreshold) / (100.0 - (double)this.systemCpuThreshold) * this.MaxDelayOnHighSystemCpu;
					double val2 = Math.Min((num2 - (double)this.processCpuThreshold) / (100.0 - (double)this.processCpuThreshold), 2.0) * this.MaxDelayOnHighProcessCpu;
					return Math.Max(val2, val);
				}
				return 0.0;
			}

			private const int DefaultHistorySize = 2000;

			private const int RefreshInterval = 60000;

			private readonly double MaxDelayOnHighSystemCpu;

			private readonly double MaxDelayOnHighProcessCpu;

			private readonly ProcessCPURunningAveragePerfCounterReader processCpuPerfCounter;

			private readonly int callsThreshold;

			private readonly float systemCpuThreshold;

			private readonly float processCpuThreshold;

			private readonly object oldCopyLock = new object();

			private readonly object newCopyLock = new object();

			private Dictionary<string, int> oldCopy;

			private Dictionary<string, int> newCopy;

			private System.Timers.Timer timer;
		}
	}
}
