using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IGroupsLogger
	{
		Enum CurrentAction { get; set; }

		void LogTrace(string formatString, params object[] args);

		void LogException(Exception exception, string formatString, params object[] args);
	}
}
