using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants.Diagnostics
{
	internal class DiagnosticsTbaProcessor : DiagnosticsProcessorBase
	{
		public DiagnosticsTbaProcessor(DiagnosticsArgument arguments) : base(arguments)
		{
			this.dbProcessor = new DiagnosticsDatabaseProcessor(base.Arguments);
		}

		public XElement Process(TimeBasedAssistantControllerWrapper[] assistantControllers)
		{
			ArgumentValidator.ThrowIfNull("assistantControllers", assistantControllers);
			IEnumerable<TimeBasedAssistantControllerWrapper> tbasToProcess;
			try
			{
				tbasToProcess = this.GetTbasToProcess(assistantControllers);
			}
			catch (UnknownAssistantException exception)
			{
				return DiagnosticsFormatter.FormatErrorElement(exception);
			}
			if (tbasToProcess == null)
			{
				return DiagnosticsFormatter.FormatErrorElement("Could not find any TBA to provide diagnostics for.");
			}
			XElement xelement = DiagnosticsFormatter.FormatRootElement();
			foreach (TimeBasedAssistantControllerWrapper tba in tbasToProcess)
			{
				List<TimeBasedDatabaseDriver> databaseDriversToProcess;
				try
				{
					databaseDriversToProcess = this.dbProcessor.GetDatabaseDriversToProcess(tba);
				}
				catch (UnknownDatabaseException exception2)
				{
					return DiagnosticsFormatter.FormatErrorElement(exception2);
				}
				if (databaseDriversToProcess == null)
				{
					return DiagnosticsFormatter.FormatErrorElement("Could not retrive the list of database drivers to process (null value).");
				}
				if (databaseDriversToProcess.Count > 0)
				{
					XElement content = this.ProcessDiagnosticsForOneTba(tba, databaseDriversToProcess);
					xelement.Add(content);
				}
			}
			return xelement;
		}

		private IEnumerable<TimeBasedAssistantControllerWrapper> GetTbasToProcess(IEnumerable<TimeBasedAssistantControllerWrapper> assistantControllers)
		{
			ArgumentValidator.ThrowIfNull("assistantControllers", assistantControllers);
			bool flag = base.Arguments.HasArgument("assistant");
			if (flag)
			{
				string argument = base.Arguments.GetArgument<string>("assistant");
				return new TimeBasedAssistantControllerWrapper[]
				{
					this.GetTbaByName(argument, assistantControllers)
				};
			}
			return assistantControllers;
		}

		private TimeBasedAssistantControllerWrapper GetTbaByName(string tbaName, IEnumerable<TimeBasedAssistantControllerWrapper> assistantControllers)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("tbaName", tbaName);
			ArgumentValidator.ThrowIfNull("assistantControllers", assistantControllers);
			foreach (TimeBasedAssistantControllerWrapper timeBasedAssistantControllerWrapper in assistantControllers)
			{
				if (timeBasedAssistantControllerWrapper.Id.Equals(tbaName))
				{
					return timeBasedAssistantControllerWrapper;
				}
			}
			throw new UnknownAssistantException(tbaName);
		}

		private XElement ProcessDiagnosticsForOneTba(TimeBasedAssistantControllerWrapper tba, IEnumerable<TimeBasedDatabaseDriver> drivers)
		{
			ArgumentValidator.ThrowIfNull("tba", tba);
			ArgumentValidator.ThrowIfNull("drivers", drivers);
			string id = tba.Id;
			TimeSpan workCycle = tba.Controller.TimeBasedAssistantType.WorkCycle;
			TimeSpan workCycleCheckpoint = tba.Controller.TimeBasedAssistantType.WorkCycleCheckpoint;
			XElement xelement = DiagnosticsFormatter.FormatAssistantRoot(id);
			XElement content = DiagnosticsFormatter.FormatWorkcycleInfoElement(workCycle);
			XElement content2 = DiagnosticsFormatter.FormatWorkcycleCheckpointInfoElement(workCycleCheckpoint);
			XElement xelement2 = DiagnosticsFormatter.FormatDatabasesRoot();
			foreach (TimeBasedDatabaseDriver driver in drivers)
			{
				XElement content3 = this.dbProcessor.ProcessDiagnosticsForOneDatabase(driver);
				xelement2.Add(content3);
			}
			xelement.Add(content);
			xelement.Add(content2);
			xelement.Add(xelement2);
			return xelement;
		}

		private readonly DiagnosticsDatabaseProcessor dbProcessor;
	}
}
