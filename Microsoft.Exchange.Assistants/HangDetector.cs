using System;
using System.Threading;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class HangDetector
	{
		public HangDetector()
		{
			this.MonitoredThread = Thread.CurrentThread;
			this.Timeout = HangDetector.DefaultTimeout;
			this.Period = HangDetector.DefaultPeriod;
		}

		public Thread MonitoredThread { get; private set; }

		public int HangsDetected { get; private set; }

		public TimeSpan Timeout { get; set; }

		public TimeSpan Period { get; set; }

		public DateTime InvokeTime { get; private set; }

		public string AssistantName
		{
			get
			{
				return this.assistantName;
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					this.assistantName = "not set by assistant";
					return;
				}
				this.assistantName = value;
			}
		}

		public string DatabaseName { get; set; }

		public void InvokeUnderHangDetection(HangDetector.InvokeDelegate delegateToBeInvoked)
		{
			this.HangsDetected = 0;
			this.timerIsDisposed = false;
			using (new Timer(new TimerCallback(this.HangDetectionCallback), null, this.Timeout, this.Period))
			{
				this.InvokeTime = DateTime.UtcNow;
				delegateToBeInvoked(this);
				this.timerIsDisposed = true;
			}
		}

		protected abstract void OnHangDetected(int hangsDetected);

		private void HangDetectionCallback(object stateNotUsed)
		{
			if (this.timerIsDisposed)
			{
				return;
			}
			this.OnHangDetected(this.HangsDetected++);
		}

		public const string DefaultAssistantName = "Common Code";

		private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan DefaultPeriod = TimeSpan.FromMinutes(30.0);

		private bool timerIsDisposed = true;

		private string assistantName = "Common Code";

		public delegate void InvokeDelegate(HangDetector hangDetector);
	}
}
