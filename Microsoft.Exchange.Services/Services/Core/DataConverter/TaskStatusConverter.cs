using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class TaskStatusConverter : EnumConverter
	{
		public static string ToString(TaskStatus taskStatus)
		{
			string result = null;
			switch (taskStatus)
			{
			case TaskStatus.NotStarted:
				result = "NotStarted";
				break;
			case TaskStatus.InProgress:
				result = "InProgress";
				break;
			case TaskStatus.Completed:
				result = "Completed";
				break;
			case TaskStatus.WaitingOnOthers:
				result = "WaitingOnOthers";
				break;
			case TaskStatus.Deferred:
				result = "Deferred";
				break;
			}
			return result;
		}

		public static TaskStatus Parse(string taskStatusString)
		{
			if (taskStatusString != null)
			{
				TaskStatus result;
				if (!(taskStatusString == "NotStarted"))
				{
					if (!(taskStatusString == "InProgress"))
					{
						if (!(taskStatusString == "Completed"))
						{
							if (!(taskStatusString == "WaitingOnOthers"))
							{
								if (!(taskStatusString == "Deferred"))
								{
									goto IL_5E;
								}
								result = TaskStatus.Deferred;
							}
							else
							{
								result = TaskStatus.WaitingOnOthers;
							}
						}
						else
						{
							result = TaskStatus.Completed;
						}
					}
					else
					{
						result = TaskStatus.InProgress;
					}
				}
				else
				{
					result = TaskStatus.NotStarted;
				}
				return result;
			}
			IL_5E:
			throw new FormatException("Invalid taskStatus string: " + taskStatusString);
		}

		public override object ConvertToObject(string propertyString)
		{
			return TaskStatusConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return TaskStatusConverter.ToString((TaskStatus)propertyValue);
		}

		private const string NotStartedString = "NotStarted";

		private const string InProgressString = "InProgress";

		private const string CompletedString = "Completed";

		private const string WaitingOnOthersString = "WaitingOnOthers";

		private const string DeferredString = "Deferred";
	}
}
