using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class ScheduledCheckTaskConfiguration
	{
		private ScheduledCheckTaskConfiguration()
		{
			this.isEnabled = false;
			this.taskIdsDetectOnly = new List<TaskId>(0);
			this.taskIdsDetectAndFix = new List<TaskId>(0);
			this.taskNamesDetectOnly = string.Empty;
			this.taskNamesDetectAndFix = string.Empty;
		}

		private ScheduledCheckTaskConfiguration(ScheduledCheckTaskConfiguration cachedConfig)
		{
			this.isEnabled = ConfigurationSchema.ScheduledISIntegEnabled.Value;
			this.taskNamesDetectOnly = cachedConfig.taskNamesDetectOnly;
			this.taskIdsDetectOnly = cachedConfig.taskIdsDetectOnly;
			this.taskNamesDetectAndFix = cachedConfig.taskNamesDetectAndFix;
			this.taskIdsDetectAndFix = cachedConfig.taskIdsDetectAndFix;
			if (this.isEnabled)
			{
				ScheduledCheckTaskConfiguration.ParseConfigurationString(ConfigurationSchema.ScheduledISIntegDetectOnly.Value, ref this.taskNamesDetectOnly, ref this.taskIdsDetectOnly);
				ScheduledCheckTaskConfiguration.ParseConfigurationString(ConfigurationSchema.ScheduledISIntegDetectAndFix.Value, ref this.taskNamesDetectAndFix, ref this.taskIdsDetectAndFix);
				return;
			}
			if (cachedConfig.isEnabled)
			{
				this.taskIdsDetectOnly = new List<TaskId>(0);
				this.taskIdsDetectAndFix = new List<TaskId>(0);
				this.taskNamesDetectOnly = string.Empty;
				this.taskNamesDetectAndFix = string.Empty;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
		}

		public List<TaskId> TaskIdsDetectOnly
		{
			get
			{
				return this.taskIdsDetectOnly;
			}
		}

		public List<TaskId> TaskIdsDetectAndFix
		{
			get
			{
				return this.taskIdsDetectAndFix;
			}
		}

		public static ScheduledCheckTaskConfiguration GetConfiguration()
		{
			ScheduledCheckTaskConfiguration result;
			using (LockManager.Lock(ScheduledCheckTaskConfiguration.configLock))
			{
				ScheduledCheckTaskConfiguration.instance = new ScheduledCheckTaskConfiguration(ScheduledCheckTaskConfiguration.instance);
				result = ScheduledCheckTaskConfiguration.instance;
			}
			return result;
		}

		internal static void ParseConfigurationString(string taskNamesParseNext, ref string taskNamesParseCached, ref List<TaskId> taskIds)
		{
			if (string.IsNullOrEmpty(taskNamesParseNext))
			{
				if (!string.IsNullOrEmpty(taskNamesParseCached))
				{
					taskIds = new List<TaskId>(0);
				}
			}
			else if (taskNamesParseCached != taskNamesParseNext || string.Compare(taskNamesParseCached, taskNamesParseNext, StringComparison.OrdinalIgnoreCase) != 0)
			{
				taskIds = new List<TaskId>(0);
				string[] array = taskNamesParseNext.Split(ScheduledCheckTaskConfiguration.taskListSeparator, StringSplitOptions.RemoveEmptyEntries);
				string text = string.Empty;
				foreach (string text2 in array)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						TaskId taskId;
						bool flag = Enum.TryParse<TaskId>(text2, out taskId);
						if (flag && taskId != TaskId.ScheduledCheck)
						{
							taskIds.Add(taskId);
						}
						else
						{
							text += (string.IsNullOrEmpty(text) ? string.Empty : ScheduledCheckTaskConfiguration.taskListSeparator.ToString());
							text += text2;
						}
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_IntegrityCheckConfigurationSkippedEntry, new object[]
					{
						text,
						taskNamesParseNext
					});
				}
			}
			taskNamesParseCached = taskNamesParseNext;
		}

		private static char[] taskListSeparator = new char[]
		{
			',',
			';'
		};

		private static object configLock = new object();

		private static ScheduledCheckTaskConfiguration instance = new ScheduledCheckTaskConfiguration();

		private readonly bool isEnabled;

		private readonly string taskNamesDetectOnly;

		private readonly List<TaskId> taskIdsDetectOnly;

		private readonly string taskNamesDetectAndFix;

		private readonly List<TaskId> taskIdsDetectAndFix;
	}
}
