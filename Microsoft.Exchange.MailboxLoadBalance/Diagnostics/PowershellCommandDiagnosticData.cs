using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PowershellCommandDiagnosticData
	{
		protected PowershellCommandDiagnosticData()
		{
		}

		[DataMember]
		public string CommandName { get; set; }

		[DataMember]
		public KeyValuePair<string, object>[] Parameters { get; set; }

		public static PowershellCommandDiagnosticData[] FromPSCommand(PSCommand command, bool includeParameterValues)
		{
			return (from cmd in command.Commands
			select PowershellCommandDiagnosticData.FromCommand(cmd, includeParameterValues)).ToArray<PowershellCommandDiagnosticData>();
		}

		private static PowershellCommandDiagnosticData FromCommand(Command command, bool includeParameterValues)
		{
			PowershellCommandDiagnosticData powershellCommandDiagnosticData = new PowershellCommandDiagnosticData();
			powershellCommandDiagnosticData.CommandName = command.CommandText;
			IEnumerable<KeyValuePair<string, object>> source = includeParameterValues ? command.Parameters.Select(new Func<CommandParameter, KeyValuePair<string, object>>(PowershellCommandDiagnosticData.CommandParametersWithValues)) : command.Parameters.Select(new Func<CommandParameter, KeyValuePair<string, object>>(PowershellCommandDiagnosticData.CommandParametersWithoutValues));
			powershellCommandDiagnosticData.Parameters = source.ToArray<KeyValuePair<string, object>>();
			return powershellCommandDiagnosticData;
		}

		private static KeyValuePair<string, object> CommandParametersWithoutValues(CommandParameter parameter)
		{
			return new KeyValuePair<string, object>(parameter.Name, null);
		}

		private static KeyValuePair<string, object> CommandParametersWithValues(CommandParameter parameter)
		{
			return new KeyValuePair<string, object>(parameter.Name, parameter.Value);
		}
	}
}
