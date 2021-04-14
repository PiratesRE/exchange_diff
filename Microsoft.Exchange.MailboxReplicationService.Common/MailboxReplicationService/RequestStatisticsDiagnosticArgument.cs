using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RequestStatisticsDiagnosticArgument : DiagnosableArgument
	{
		public RequestStatisticsDiagnosticArgument(string argument)
		{
			base.Initialize(argument);
		}

		protected override void InitializeSchema(Dictionary<string, Type> schema)
		{
			schema["showtimeslots"] = typeof(bool);
		}

		public const string ShowTimeSlots = "showtimeslots";
	}
}
