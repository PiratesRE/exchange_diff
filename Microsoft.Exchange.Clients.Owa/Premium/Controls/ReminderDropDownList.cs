using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ReminderDropDownList : DropDownList
	{
		public ReminderDropDownList(string id, double reminderTime) : base(id, reminderTime.ToString(), null)
		{
			this.reminderTime = reminderTime;
		}

		public ReminderDropDownList(string id, double reminderTime, bool isSnooze) : base(id, reminderTime.ToString(), null)
		{
			this.reminderTime = reminderTime;
			this.isSnooze = isSnooze;
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			for (int i = 0; i < ReminderDropDownList.reminderTimes.Length; i++)
			{
				ReminderDropDownList.ReminderTime reminderTime = ReminderDropDownList.reminderTimes[i];
				if (this.reminderTime == ReminderDropDownList.reminderTimes[i].Time)
				{
					writer.Write(SanitizedHtmlString.FromStringId(ReminderDropDownList.reminderTimes[i].Display));
					return;
				}
			}
			this.isStockTime = false;
			writer.Write(DateTimeUtilities.FormatDuration((int)this.reminderTime));
		}

		protected override DropDownListItem[] CreateListItems()
		{
			List<DropDownListItem> list = new List<DropDownListItem>();
			int i = 0;
			if (this.isSnooze)
			{
				while (i < ReminderDropDownList.snoozeTimes.Length)
				{
					list.Add(new DropDownListItem(ReminderDropDownList.snoozeTimes[i].Time, ReminderDropDownList.snoozeTimes[i].Display));
					i++;
				}
				i = 1;
			}
			while (i < ReminderDropDownList.reminderTimes.Length)
			{
				list.Add(new DropDownListItem(ReminderDropDownList.reminderTimes[i].Time, ReminderDropDownList.reminderTimes[i].Display));
				if (!this.isStockTime && (i == ReminderDropDownList.reminderTimes.Length - 1 || (ReminderDropDownList.reminderTimes[i].Time < this.reminderTime && this.reminderTime < ReminderDropDownList.reminderTimes[i + 1].Time)))
				{
					list.Add(new DropDownListItem(this.reminderTime, DateTimeUtilities.FormatDuration((int)this.reminderTime), false));
				}
				i++;
			}
			return list.ToArray();
		}

		private static readonly ReminderDropDownList.ReminderTime[] reminderTimes = new ReminderDropDownList.ReminderTime[]
		{
			new ReminderDropDownList.ReminderTime(0.0, -1884236483),
			new ReminderDropDownList.ReminderTime(5.0, 2007446286),
			new ReminderDropDownList.ReminderTime(10.0, 2098102450),
			new ReminderDropDownList.ReminderTime(15.0, 2098102453),
			new ReminderDropDownList.ReminderTime(30.0, 935303036),
			new ReminderDropDownList.ReminderTime(60.0, 104450136),
			new ReminderDropDownList.ReminderTime(120.0, 1670534077),
			new ReminderDropDownList.ReminderTime(180.0, -1058349278),
			new ReminderDropDownList.ReminderTime(240.0, 507734663),
			new ReminderDropDownList.ReminderTime(480.0, -1105403445),
			new ReminderDropDownList.ReminderTime(720.0, 2000165286),
			new ReminderDropDownList.ReminderTime(1440.0, -1685054980),
			new ReminderDropDownList.ReminderTime(2880.0, -1685054979),
			new ReminderDropDownList.ReminderTime(4320.0, -1685054978),
			new ReminderDropDownList.ReminderTime(10080.0, 636052262),
			new ReminderDropDownList.ReminderTime(20160.0, -930031679)
		};

		private static readonly ReminderDropDownList.ReminderTime[] snoozeTimes = new ReminderDropDownList.ReminderTime[]
		{
			new ReminderDropDownList.ReminderTime(-15.0, 84937006),
			new ReminderDropDownList.ReminderTime(-10.0, 844451893),
			new ReminderDropDownList.ReminderTime(-5.0, 1844088193),
			new ReminderDropDownList.ReminderTime(0.0, 1844088196)
		};

		private static readonly double MinutesInHour = 60.0;

		private static readonly double MinutesInDay = ReminderDropDownList.MinutesInHour * 24.0;

		private double reminderTime;

		private bool isStockTime = true;

		private bool isSnooze;

		private struct ReminderTime
		{
			public ReminderTime(double time, Strings.IDs display)
			{
				this.time = time;
				this.display = display;
			}

			public double Time
			{
				get
				{
					return this.time;
				}
			}

			public Strings.IDs Display
			{
				get
				{
					return this.display;
				}
			}

			private double time;

			private Strings.IDs display;
		}
	}
}
