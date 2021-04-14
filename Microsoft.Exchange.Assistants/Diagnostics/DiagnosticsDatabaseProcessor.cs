using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants.Diagnostics
{
	internal class DiagnosticsDatabaseProcessor : DiagnosticsProcessorBase
	{
		public DiagnosticsDatabaseProcessor(DiagnosticsArgument arguments) : base(arguments)
		{
		}

		public List<TimeBasedDatabaseDriver> GetDatabaseDriversToProcess(TimeBasedAssistantControllerWrapper tba)
		{
			ArgumentValidator.ThrowIfNull("tba", tba);
			bool flag = base.Arguments.HasArgument("database");
			if (flag)
			{
				string argument = base.Arguments.GetArgument<string>("database");
				return new List<TimeBasedDatabaseDriver>
				{
					this.GetDatabaseDriverByName(tba, argument)
				};
			}
			TimeBasedDatabaseDriver[] currentAssistantDrivers = tba.Controller.GetCurrentAssistantDrivers();
			if (currentAssistantDrivers != null)
			{
				return currentAssistantDrivers.ToList<TimeBasedDatabaseDriver>();
			}
			return new List<TimeBasedDatabaseDriver>();
		}

		public XElement ProcessDiagnosticsForOneDatabase(TimeBasedDatabaseDriver driver)
		{
			ArgumentValidator.ThrowIfNull("driver", driver);
			if (base.Arguments.HasArgument("summary"))
			{
				return DiagnosticsFormatter.FormatTimeBasedJobDatabaseStats(driver.DatabaseInfo.DatabaseName, driver.DatabaseInfo.Guid, driver.GetDatabaseDiagnosticsSummary());
			}
			if (base.Arguments.HasArgument("running"))
			{
				return DiagnosticsFormatter.FormatTimeBasedMailboxes(true, driver.DatabaseInfo.DatabaseName, driver.DatabaseInfo.Guid, driver.GetDatabaseDiagnosticsSummary(), driver.GetMailboxGuidList(true));
			}
			if (base.Arguments.HasArgument("queued"))
			{
				return DiagnosticsFormatter.FormatTimeBasedMailboxes(false, driver.DatabaseInfo.DatabaseName, driver.DatabaseInfo.Guid, driver.GetDatabaseDiagnosticsSummary(), driver.GetMailboxGuidList(false));
			}
			if (base.Arguments.HasArgument("history"))
			{
				return DiagnosticsFormatter.FormatWindowJobHistory(driver.DatabaseInfo.DatabaseName, driver.DatabaseInfo.Guid, driver.GetDatabaseDiagnosticsSummary(), driver.GetWindowJobHistory());
			}
			return DiagnosticsFormatter.FormatTimeBasedJobDatabaseStatsCommon(driver.DatabaseInfo.DatabaseName, driver.DatabaseInfo.Guid, driver.GetDatabaseDiagnosticsSummary());
		}

		private TimeBasedDatabaseDriver GetDatabaseDriverByName(TimeBasedAssistantControllerWrapper tba, string databaseName)
		{
			ArgumentValidator.ThrowIfNull("tba", tba);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("databaseName", databaseName);
			foreach (TimeBasedDatabaseDriver timeBasedDatabaseDriver in tba.Controller.GetCurrentAssistantDrivers())
			{
				if (timeBasedDatabaseDriver.DatabaseInfo.DatabaseName.Equals(databaseName))
				{
					return timeBasedDatabaseDriver;
				}
			}
			throw new UnknownDatabaseException(databaseName);
		}
	}
}
