using System;
using System.Data;
using System.Diagnostics;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class MonadDataAdapterExecutionContext : DataAdapterExecutionContext
	{
		public MonadConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		public override void Open(IUIService service, WorkUnitCollection workUnits, bool enforceViewEntireForest, ResultsLoaderProfile profile)
		{
			this.isResultPane = !enforceViewEntireForest;
			this.workUnits = workUnits;
			this.commandInteractionHandler = ((service != null) ? new WinFormsCommandInteractionHandler(service) : new CommandInteractionHandler());
			RunspaceServerSettingsPresentationObject runspaceServerSettingsPresentationObject = ADServerSettingsSingleton.GetInstance().CreateRunspaceServerSettingsObject();
			if (enforceViewEntireForest && runspaceServerSettingsPresentationObject != null)
			{
				runspaceServerSettingsPresentationObject.ViewEntireForest = true;
			}
			this.connection = new MonadConnection(PSConnectionInfoSingleton.GetInstance().GetConnectionStringForScript(), this.commandInteractionHandler, runspaceServerSettingsPresentationObject, PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo(profile.SerializationLevel));
			this.connection.Open();
		}

		private void BeginExecute(AbstractDataTableFiller filler, ResultsLoaderProfile profile)
		{
			MonadAdapterFiller monadAdapterFiller = filler as MonadAdapterFiller;
			if (monadAdapterFiller != null)
			{
				this.AttachCommandToMonitorWarnings(monadAdapterFiller.Command);
				monadAdapterFiller.Command.PreservedObjectProperty = profile.WholeObjectProperty;
			}
		}

		public override void Execute(AbstractDataTableFiller filler, DataTable table, ResultsLoaderProfile profile)
		{
			MonadAdapterFiller monadAdapterFiller = filler as MonadAdapterFiller;
			if (monadAdapterFiller != null && !monadAdapterFiller.HasPermission() && (this.isResultPane || monadAdapterFiller.IsResolving))
			{
				return;
			}
			Stopwatch stopwatch = new Stopwatch();
			this.BeginExecute(filler, profile);
			MonadCommand monadCommand = null;
			if (monadAdapterFiller != null)
			{
				monadCommand = monadAdapterFiller.Command;
			}
			else if (filler is SupervisionListFiller)
			{
				monadCommand = ((SupervisionListFiller)filler).Command;
			}
			if (monadCommand != null)
			{
				monadCommand.Connection = this.connection;
			}
			try
			{
				if (monadAdapterFiller != null)
				{
					ExTraceGlobals.DataFlowTracer.TracePerformance<string, Guid, RunspaceState>((long)Thread.CurrentThread.ManagedThreadId, "MonadScriptExecutionContext.Execute: In runspace {1}[State:{2}], before executing command '{0}' .", monadAdapterFiller.Command.CommandText, monadAdapterFiller.Command.Connection.RunspaceProxy.InstanceId, monadAdapterFiller.Command.Connection.RunspaceProxy.State);
					stopwatch.Start();
				}
				filler.Fill(table);
			}
			catch (MonadDataAdapterInvocationException ex)
			{
				if (!(ex.InnerException is ManagementObjectNotFoundException) && !(ex.InnerException is MapiObjectNotFoundException))
				{
					throw;
				}
			}
			finally
			{
				if (monadAdapterFiller != null)
				{
					stopwatch.Stop();
					ExTraceGlobals.DataFlowTracer.TracePerformance((long)Thread.CurrentThread.ManagedThreadId, "MonadScriptExecutionContext.Execute: In the runspace {1}[State:{2}], {3} Milliseconds are taken to finish the command {0}.", new object[]
					{
						monadAdapterFiller.Command.CommandText,
						monadAdapterFiller.Command.Connection.RunspaceProxy.InstanceId,
						monadAdapterFiller.Command.Connection.RunspaceProxy.State,
						stopwatch.ElapsedMilliseconds
					});
				}
				this.EndExecute(filler, profile);
			}
		}

		private void EndExecute(AbstractDataTableFiller filler, ResultsLoaderProfile profile)
		{
			MonadAdapterFiller monadAdapterFiller = filler as MonadAdapterFiller;
			if (monadAdapterFiller != null)
			{
				this.DetachCommandFromMonitorWarnings(monadAdapterFiller.Command);
			}
		}

		public override void Close()
		{
			if (this.connection != null)
			{
				this.connection.Close();
			}
		}

		private void AttachCommandToMonitorWarnings(MonadCommand command)
		{
			lock (this.workUnits)
			{
				WorkUnit workUnit;
				if (!this.TryGetWorkUnit(command.CommandText, out workUnit))
				{
					workUnit = new WorkUnit();
					workUnit.Target = command;
					workUnit.Text = command.CommandText;
					this.workUnits.Add(workUnit);
					command.WarningReport += this.command_WarningReport;
				}
			}
		}

		private void DetachCommandFromMonitorWarnings(MonadCommand command)
		{
			lock (this.workUnits)
			{
				WorkUnit workUnit;
				if (this.TryGetWorkUnit(command.CommandText, out workUnit))
				{
					workUnit.Status = WorkUnitStatus.Completed;
					command.WarningReport -= this.command_WarningReport;
				}
			}
		}

		private bool TryGetWorkUnit(string text, out WorkUnit workUnit)
		{
			workUnit = null;
			for (int i = 0; i < this.workUnits.Count; i++)
			{
				if (this.workUnits[i].Text == text)
				{
					workUnit = this.workUnits[i];
					break;
				}
			}
			return null != workUnit;
		}

		private void command_WarningReport(object sender, WarningReportEventArgs e)
		{
			lock (this.workUnits)
			{
				WorkUnit workUnit;
				if (this.TryGetWorkUnit(e.Command.CommandText, out workUnit) && workUnit.Target == e.Command)
				{
					workUnit.Warnings.Add(e.WarningMessage);
				}
			}
		}

		private CommandInteractionHandler commandInteractionHandler;

		private MonadConnection connection;

		private WorkUnitCollection workUnits;

		private bool isResultPane;
	}
}
