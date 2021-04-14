using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal sealed class CalendarRepairPolicy
	{
		private CalendarRepairPolicy()
		{
		}

		internal static CalendarRepairPolicy CreateInstance(string name)
		{
			return new CalendarRepairPolicy
			{
				repairFlags = new HashSet<CalendarInconsistencyFlag>(),
				RepairMode = CalendarRepairType.ValidateOnly,
				name = name
			};
		}

		private void AddRepairFlags(CalendarInconsistencyFlag flag)
		{
			if (!this.ContainsRepairFlag(flag))
			{
				this.repairFlags.Add(flag);
				this.RepairMode = CalendarRepairType.RepairAndValidate;
			}
		}

		public void RemoveRepairFlags(params CalendarInconsistencyFlag[] flags)
		{
			if (flags != null)
			{
				bool flag = false;
				foreach (CalendarInconsistencyFlag calendarInconsistencyFlag in flags)
				{
					if (calendarInconsistencyFlag != CalendarInconsistencyFlag.None)
					{
						flag |= this.repairFlags.Remove(calendarInconsistencyFlag);
					}
				}
				if (flag)
				{
					this.RepairMode = ((this.repairFlags.Count > 1) ? CalendarRepairType.RepairAndValidate : CalendarRepairType.ValidateOnly);
				}
			}
		}

		internal bool ContainsRepairFlag(CalendarInconsistencyFlag flag)
		{
			return flag == CalendarInconsistencyFlag.None || this.repairFlags.Contains(flag);
		}

		public void InitializeWithDefaults()
		{
			this.daysInWindowForward = 45;
			this.daysInWindowBackward = 45;
			foreach (object obj in Enum.GetValues(typeof(CalendarInconsistencyFlag)))
			{
				CalendarInconsistencyFlag flag = (CalendarInconsistencyFlag)obj;
				this.AddRepairFlags(flag);
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal int DaysInWindowForward
		{
			get
			{
				return this.daysInWindowForward;
			}
			set
			{
				if (value >= 0)
				{
					this.daysInWindowForward = value;
					return;
				}
				throw new ArgumentOutOfRangeException("DaysInWindowForward");
			}
		}

		internal int DaysInWindowBackward
		{
			get
			{
				return this.daysInWindowBackward;
			}
			set
			{
				if (value >= 0)
				{
					this.daysInWindowBackward = value;
					return;
				}
				throw new ArgumentOutOfRangeException("DaysInWindowBackward");
			}
		}

		internal CalendarRepairType RepairMode { get; set; }

		internal int MaxThreads
		{
			get
			{
				return this.maxThreads;
			}
			set
			{
				this.maxThreads = value;
			}
		}

		internal bool CopyRumsToSentItems { get; set; }

		public const int DefaultDaysForward = 45;

		private string name;

		private int daysInWindowForward;

		private int daysInWindowBackward;

		private int maxThreads;

		private HashSet<CalendarInconsistencyFlag> repairFlags;
	}
}
