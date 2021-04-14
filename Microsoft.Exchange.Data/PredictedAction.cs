using System;

namespace Microsoft.Exchange.Data
{
	public enum PredictedAction : short
	{
		Respond = 10,
		FollowUp = 20,
		MoveToFolder = 30,
		Delete = 40,
		ActionCompleted = 50,
		Read = 60,
		Ignore = 70,
		None = 10000
	}
}
