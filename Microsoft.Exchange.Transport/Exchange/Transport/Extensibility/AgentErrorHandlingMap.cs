using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal class AgentErrorHandlingMap
	{
		public static IErrorHandlingAction AllowAction = new AgentErrorHandlingAllowAction();

		public static IErrorHandlingAction DropAction = new AgentErrorHandlingDropAction();

		public static IErrorHandlingAction NdrActionBadContent = new AgentErrorHandlingNdrAction(SmtpResponse.InvalidContent);

		public static IErrorHandlingAction DeferAction30MinConstant = new AgentErrorHandlingDeferAction(TimeSpan.FromMinutes(5.0), false);

		public static IErrorHandlingAction DeferAction5MinProgressive = new AgentErrorHandlingDeferAction(TimeSpan.FromMinutes(5.0), true);

		public static IEnumerable<AgentErrorHandlingDefinition> DefaultMap = new AgentErrorHandlingDefinition[0];
	}
}
