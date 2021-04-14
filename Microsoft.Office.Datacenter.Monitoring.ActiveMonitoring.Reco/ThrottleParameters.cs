using System;
using System.Collections.Generic;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class ThrottleParameters
	{
		public ThrottleParameters(bool isEnabled, int localMinimumMinutesBetweenAttempts, int localMaximumAllowedAttemptsInOneHour, int localMaximumAllowedAttemptsInADay, int groupMinimumMinutesBetweenAttempts, int groupMaximumAllowedAttemptsInADay)
		{
			this.IsEnabled = isEnabled;
			this.LocalMinimumMinutesBetweenAttempts = localMinimumMinutesBetweenAttempts;
			this.LocalMaximumAllowedAttemptsInOneHour = localMaximumAllowedAttemptsInOneHour;
			this.LocalMaximumAllowedAttemptsInADay = localMaximumAllowedAttemptsInADay;
			this.GroupMinimumMinutesBetweenAttempts = groupMinimumMinutesBetweenAttempts;
			this.GroupMaximumAllowedAttemptsInADay = groupMaximumAllowedAttemptsInADay;
			this.PropertyBag = this.ToDictionary();
		}

		public bool IsEnabled { get; private set; }

		public int LocalMinimumMinutesBetweenAttempts { get; private set; }

		public int LocalMaximumAllowedAttemptsInOneHour { get; private set; }

		public int LocalMaximumAllowedAttemptsInADay { get; private set; }

		public int GroupMinimumMinutesBetweenAttempts { get; private set; }

		public int GroupMaximumAllowedAttemptsInADay { get; private set; }

		internal Dictionary<string, string> PropertyBag { get; private set; }

		internal static bool GetBoolProperty(Dictionary<string, string> propertyBag, string propertyName, bool defaultValue)
		{
			bool result = defaultValue;
			bool flag = false;
			string text;
			if (propertyBag.TryGetValue(propertyName, out text) && !string.IsNullOrWhiteSpace(text))
			{
				flag = bool.TryParse(text, out result);
				if (!flag)
				{
					int num;
					flag = int.TryParse(text, out num);
					if (flag)
					{
						if (num == 0)
						{
							result = false;
						}
						else if (num == 1)
						{
							result = true;
						}
						else
						{
							flag = false;
						}
					}
				}
			}
			if (!flag)
			{
				result = defaultValue;
			}
			return result;
		}

		internal static int GetIntProperty(Dictionary<string, string> propertyBag, string propertyName, int defaultValue)
		{
			int result = defaultValue;
			string text;
			if (propertyBag.TryGetValue(propertyName, out text) && !string.IsNullOrWhiteSpace(text) && !int.TryParse(text, out result))
			{
				result = defaultValue;
			}
			return result;
		}

		internal Dictionary<string, string> ToDictionary()
		{
			return new Dictionary<string, string>
			{
				{
					"Enabled",
					this.IsEnabled.ToString()
				},
				{
					"LocalMinimumMinutesBetweenAttempts",
					this.LocalMinimumMinutesBetweenAttempts.ToString()
				},
				{
					"LocalMaximumAllowedAttemptsInOneHour",
					this.LocalMaximumAllowedAttemptsInOneHour.ToString()
				},
				{
					"LocalMaximumAllowedAttemptsInADay",
					this.LocalMaximumAllowedAttemptsInADay.ToString()
				},
				{
					"GroupMinimumMinutesBetweenAttempts",
					this.GroupMinimumMinutesBetweenAttempts.ToString()
				},
				{
					"GroupMaximumAllowedAttemptsInADay",
					this.GroupMaximumAllowedAttemptsInADay.ToString()
				}
			};
		}

		internal void ProcessProperty(Dictionary<string, string> propertyBag, string propertyName, string propertyNameShort, Action<string> action)
		{
			string text = null;
			if (propertyBag.ContainsKey(propertyName))
			{
				text = propertyName;
			}
			if (!string.IsNullOrWhiteSpace(propertyNameShort) && propertyBag.ContainsKey(propertyNameShort))
			{
				text = propertyNameShort;
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				action(text);
			}
		}

		internal void ApplyPropertyOverrides(Dictionary<string, string> propertyBag)
		{
			this.ProcessProperty(propertyBag, "Enabled", null, delegate(string propertyName)
			{
				this.IsEnabled = ThrottleParameters.GetBoolProperty(propertyBag, propertyName, this.IsEnabled);
			});
			this.ProcessProperty(propertyBag, "LocalMinimumMinutesBetweenAttempts", "LocalMinutesMinimum", delegate(string propertyName)
			{
				this.LocalMinimumMinutesBetweenAttempts = ThrottleParameters.GetIntProperty(propertyBag, propertyName, this.LocalMinimumMinutesBetweenAttempts);
			});
			this.ProcessProperty(propertyBag, "LocalMaximumAllowedAttemptsInOneHour", "LocalMaxInHour", delegate(string propertyName)
			{
				this.LocalMaximumAllowedAttemptsInOneHour = ThrottleParameters.GetIntProperty(propertyBag, propertyName, this.LocalMaximumAllowedAttemptsInOneHour);
			});
			this.ProcessProperty(propertyBag, "LocalMaximumAllowedAttemptsInADay", "LocalMaxInDay", delegate(string propertyName)
			{
				this.LocalMaximumAllowedAttemptsInADay = ThrottleParameters.GetIntProperty(propertyBag, propertyName, this.LocalMaximumAllowedAttemptsInADay);
			});
			this.ProcessProperty(propertyBag, "GroupMinimumMinutesBetweenAttempts", "GroupMinutesMinimum", delegate(string propertyName)
			{
				this.GroupMinimumMinutesBetweenAttempts = ThrottleParameters.GetIntProperty(propertyBag, propertyName, this.GroupMinimumMinutesBetweenAttempts);
			});
			this.ProcessProperty(propertyBag, "GroupMaximumAllowedAttemptsInADay", "GroupMaxInDay", delegate(string propertyName)
			{
				this.GroupMaximumAllowedAttemptsInADay = ThrottleParameters.GetIntProperty(propertyBag, propertyName, this.GroupMaximumAllowedAttemptsInADay);
			});
		}

		internal ThrottleParameters Clone()
		{
			return (ThrottleParameters)base.MemberwiseClone();
		}

		public const string EnabledProperty = "Enabled";

		public const string LocalMinMinutesGapProperty = "LocalMinimumMinutesBetweenAttempts";

		public const string LocalMaxAllowedInHourProperty = "LocalMaximumAllowedAttemptsInOneHour";

		public const string LocalMaxAllowedInDayProperty = "LocalMaximumAllowedAttemptsInADay";

		public const string GroupMinMinutesGapProperty = "GroupMinimumMinutesBetweenAttempts";

		public const string GroupMaxAllowedInDayProperty = "GroupMaximumAllowedAttemptsInADay";

		public const string LocalMinMinutesGapPropertyShort = "LocalMinutesMinimum";

		public const string LocalMaxAllowedInHourPropertyShort = "LocalMaxInHour";

		public const string LocalMaxAllowedInDayPropertyShort = "LocalMaxInDay";

		public const string GroupMinMinutesGapPropertyShort = "GroupMinutesMinimum";

		public const string GroupMaxAllowedInDayPropertyShort = "GroupMaxInDay";

		public const int CheckNotApplicable = -1;

		internal static readonly ThrottleParameters Default = new ThrottleParameters(true, 60, 1, 1, 60, 1);
	}
}
