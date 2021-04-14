using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class TaskDelegateStateConverter : EnumConverter
	{
		public static string ToString(TaskDelegateState taskDelegateState)
		{
			string result = null;
			switch (taskDelegateState)
			{
			case TaskDelegateState.NoMatch:
				result = "NoMatch";
				break;
			case TaskDelegateState.OwnNew:
				result = "OwnNew";
				break;
			case TaskDelegateState.Owned:
				result = "Owned";
				break;
			case TaskDelegateState.Accepted:
				result = "Accepted";
				break;
			case TaskDelegateState.Declined:
				result = "Declined";
				break;
			case TaskDelegateState.Max:
				result = "Max";
				break;
			}
			return result;
		}

		public static TaskDelegateState Parse(string taskDelegateStateString)
		{
			if (taskDelegateStateString != null)
			{
				if (<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x6000cb1-1 == null)
				{
					<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x6000cb1-1 = new Dictionary<string, int>(6)
					{
						{
							"NoMatch",
							0
						},
						{
							"OwnNew",
							1
						},
						{
							"Owned",
							2
						},
						{
							"Accepted",
							3
						},
						{
							"Declined",
							4
						},
						{
							"Max",
							5
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{07C235D2-EA05-4020-8C99-D4258F03250B}.$$method0x6000cb1-1.TryGetValue(taskDelegateStateString, out num))
				{
					TaskDelegateState result;
					switch (num)
					{
					case 0:
						result = TaskDelegateState.NoMatch;
						break;
					case 1:
						result = TaskDelegateState.OwnNew;
						break;
					case 2:
						result = TaskDelegateState.Owned;
						break;
					case 3:
						result = TaskDelegateState.Accepted;
						break;
					case 4:
						result = TaskDelegateState.Declined;
						break;
					case 5:
						result = TaskDelegateState.Max;
						break;
					default:
						goto IL_B1;
					}
					return result;
				}
			}
			IL_B1:
			throw new FormatException("Invalid taskDelegateState string: " + taskDelegateStateString);
		}

		public override object ConvertToObject(string propertyString)
		{
			return TaskDelegateStateConverter.Parse(propertyString);
		}

		public override string ConvertToString(object propertyValue)
		{
			return TaskDelegateStateConverter.ToString((TaskDelegateState)propertyValue);
		}

		private const string NoMatchString = "NoMatch";

		private const string OwnNewString = "OwnNew";

		private const string OwnedString = "Owned";

		private const string AcceptedString = "Accepted";

		private const string DeclinedString = "Declined";

		private const string MaxString = "Max";
	}
}
