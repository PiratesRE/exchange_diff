using System;
using System.Threading;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ScheduleDataHandler : ExchangeDataHandler
	{
		public ScheduleDataHandler() : this(null)
		{
		}

		public ScheduleDataHandler(DataHandler nestedDataHandler) : this(nestedDataHandler, null, Unlimited<uint>.UnlimitedValue)
		{
		}

		public ScheduleDataHandler(DataHandler nestedDataHandler, DateTime? startTime, Unlimited<uint> intervalHours)
		{
			if (nestedDataHandler != null)
			{
				base.DataHandlers.Add(nestedDataHandler);
			}
			this.StartTaskAtScheduledTime = (startTime != null);
			this.StartTime = ((startTime != null) ? startTime.Value : ((DateTime)ExDateTime.Now));
			this.IntervalHours = intervalHours;
			base.DataSource = this;
		}

		internal override void OnReadData(CommandInteractionHandler interactionHandler, string pageName)
		{
			try
			{
				base.OnReadData(interactionHandler, pageName);
			}
			finally
			{
				base.DataSource = this;
			}
		}

		internal override void OnSaveData(CommandInteractionHandler interactionHandler)
		{
			Timer timer = null;
			Timer timer2 = null;
			try
			{
				if (this.StartTaskAtScheduledTime && this.StartTime > (DateTime)ExDateTime.Now)
				{
					this.startEvent.Reset();
					timer = new Timer(new TimerCallback(this.OnStart), null, 0, 1000);
				}
				if (WaitHandle.WaitAny(new WaitHandle[]
				{
					this.startEvent,
					this.cancelEvent
				}) == 0 && !base.Cancelled)
				{
					if (this.CancelTaskAtScheduledTime)
					{
						this.cancelEvent.Reset();
						timer2 = new Timer(new TimerCallback(this.OnCancel), null, (long)((ulong)this.IntervalHours.Value * 3600UL * 1000UL), -1L);
					}
					base.OnSaveData(interactionHandler);
				}
			}
			finally
			{
				if (timer != null)
				{
					timer.Dispose();
				}
				if (timer2 != null)
				{
					timer2.Dispose();
				}
			}
		}

		public override void Cancel()
		{
			base.Cancel();
			this.cancelEvent.Set();
		}

		private void OnStart(object target)
		{
			this.OnCountDown();
			if ((this.StartTime - (DateTime)ExDateTime.Now).TotalSeconds < 1.0)
			{
				this.startEvent.Set();
			}
		}

		private void OnCancel(object target)
		{
			this.Cancel();
		}

		private void OnCountDown()
		{
			EventHandler eventHandler = this.countDown;
			if (eventHandler != null)
			{
				eventHandler(this, EventArgs.Empty);
			}
		}

		public event EventHandler CountDown
		{
			add
			{
				this.countDown = (EventHandler)SynchronizedDelegate.Combine(this.countDown, value);
			}
			remove
			{
				this.countDown = (EventHandler)SynchronizedDelegate.Remove(this.countDown, value);
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.startTime;
			}
			set
			{
				this.startTime = value;
			}
		}

		public Unlimited<uint> IntervalHours
		{
			get
			{
				return this.intervalHours;
			}
			set
			{
				bool flag = value != Unlimited<uint>.UnlimitedValue;
				if (flag && (value.Value > 48U || value.Value < 1U))
				{
					throw new ArgumentException(Strings.IntervalOutOfRange(1U, 48U));
				}
				this.CancelTaskAtScheduledTime = flag;
				this.intervalHours = value;
			}
		}

		public bool CancelTaskAtScheduledTime
		{
			get
			{
				return this.cancelTaskAtScheduledTime;
			}
			set
			{
				this.cancelTaskAtScheduledTime = value;
			}
		}

		public bool StartTaskAtScheduledTime
		{
			get
			{
				return this.startTaskAtScheduledTime;
			}
			set
			{
				this.startTaskAtScheduledTime = value;
			}
		}

		public bool DoNotApply
		{
			get
			{
				return this.doNotApply;
			}
			set
			{
				this.doNotApply = value;
			}
		}

		public string RemainTimeText
		{
			get
			{
				string result = string.Empty;
				TimeSpan timeSpan = this.StartTime - (DateTime)ExDateTime.Now;
				if (timeSpan.TotalSeconds >= 1.0 && this.StartTaskAtScheduledTime)
				{
					if (timeSpan.Days > 0)
					{
						result = Strings.CountDownDays((long)timeSpan.Days, (long)timeSpan.Hours, (long)timeSpan.Minutes, (long)timeSpan.Seconds);
					}
					else if (timeSpan.Hours > 0)
					{
						result = Strings.CountDownHours((long)timeSpan.Hours, (long)timeSpan.Minutes, (long)timeSpan.Seconds);
					}
					else if (timeSpan.Minutes > 0)
					{
						result = Strings.CountDownMinutes((long)timeSpan.Minutes, (long)timeSpan.Seconds);
					}
					else
					{
						result = Strings.CountDownSeconds((long)timeSpan.Seconds);
					}
				}
				return result;
			}
		}

		internal const uint MaximumIntervalHours = 48U;

		internal const uint MinimumIntervalHours = 1U;

		private ManualResetEvent cancelEvent = new ManualResetEvent(false);

		private ManualResetEvent startEvent = new ManualResetEvent(true);

		private bool cancelTaskAtScheduledTime;

		private bool startTaskAtScheduledTime;

		private DateTime startTime;

		private Unlimited<uint> intervalHours;

		private EventHandler countDown;

		private bool doNotApply;
	}
}
