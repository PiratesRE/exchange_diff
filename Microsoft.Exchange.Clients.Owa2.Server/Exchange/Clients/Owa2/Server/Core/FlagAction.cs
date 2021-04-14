using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum FlagAction
	{
		None,
		Default,
		Today,
		Tomorrow,
		ThisWeek,
		NextWeek,
		NoDate,
		SpecificDate,
		DateAndReminder,
		MarkComplete,
		ClearFlag
	}
}
