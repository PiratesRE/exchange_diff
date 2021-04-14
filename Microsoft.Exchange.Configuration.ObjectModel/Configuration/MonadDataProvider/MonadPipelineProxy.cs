using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Monad;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	public sealed class MonadPipelineProxy : PowerShellProxy, IDisposable
	{
		internal event EventHandler<ProgressReportEventArgs> ProgressReport;

		internal event EventHandler<ErrorReportEventArgs> ErrorReport;

		internal event EventHandler<WarningReportEventArgs> WarningReport;

		public MonadPipelineProxy(RunspaceProxy proxy, PSCommand commands) : base(proxy, commands)
		{
		}

		public MonadPipelineProxy(RunspaceProxy proxy, IEnumerable input, PSCommand commands) : this(proxy, commands)
		{
			this.InitInput(input);
		}

		public MonadPipelineProxy(RunspaceProxy proxy, IEnumerable input, PSCommand commands, WorkUnit[] workUnits) : base(proxy, workUnits, commands)
		{
			this.InitInput(input);
		}

		private void InitInput(IEnumerable input)
		{
			if (input == null)
			{
				return;
			}
			if (input.GetType().IsAssignableFrom(typeof(PSDataCollection<object>)))
			{
				this.input = (PSDataCollection<object>)input;
				if (this.input.IsOpen)
				{
					this.input.Complete();
					return;
				}
			}
			else
			{
				this.input = new PSDataCollection<object>();
				foreach (object item in input)
				{
					this.input.Add(item);
				}
				this.input.Complete();
				using (input as IDisposable)
				{
				}
			}
		}

		internal CommandInteractionHandler InteractionHandler
		{
			get
			{
				return this.commandInteractionHandler;
			}
			set
			{
				this.commandInteractionHandler = value;
			}
		}

		internal MonadCommand Command
		{
			get
			{
				return this.command;
			}
			set
			{
				this.command = value;
				if (this.command != null)
				{
					MonadHost.InitializeMonadHostConnection(base.GetRunspaceHost(), this.Command.Connection);
				}
			}
		}

		internal IEnumerable Input
		{
			get
			{
				return this.input;
			}
		}

		protected override bool RegisterHostListener
		{
			get
			{
				return base.RegisterHostListener || this.commandInteractionHandler != null;
			}
		}

		public ErrorRecord LastUnhandledError
		{
			get
			{
				return this.lastUnhandledError;
			}
		}

		public void Dispose()
		{
			if (this.input != null)
			{
				this.input.Dispose();
				this.input = null;
			}
		}

		protected override IAsyncResult InternalBeginInvoke(AsyncCallback asyncCallback, object asyncState)
		{
			PSDataCollection<PSObject> output = new PSDataCollection<PSObject>();
			IAsyncResult psAsyncResult = base.PowerShell.BeginInvoke<object, PSObject>(this.input, output);
			return new MonadAsyncResult(this.Command, asyncCallback, asyncState, psAsyncResult, output);
		}

		protected override Collection<PSObject> InternalEndInvoke(IAsyncResult results)
		{
			MonadAsyncResult monadAsyncResult = results as MonadAsyncResult;
			if (monadAsyncResult == null)
			{
				throw new ArgumentException("results");
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadCommand.EndExecute()");
			if (monadAsyncResult.RunningCommand != this.Command)
			{
				throw new ArgumentException("Parameter does not correspond to this command.", "asyncResult");
			}
			Collection<PSObject> result = null;
			ErrorRecord errorRecord = null;
			try
			{
				result = this.ClosePipeline(monadAsyncResult);
				if (this.pipelineStateAtClose == PSInvocationState.Stopped)
				{
					errorRecord = new ErrorRecord(new PipelineStoppedException(), string.Empty, ErrorCategory.OperationStopped, null);
				}
			}
			catch (CmdletInvocationException ex)
			{
				errorRecord = ex.ErrorRecord;
				throw;
			}
			catch (CommandExecutionException ex2)
			{
				errorRecord = new ErrorRecord(ex2.InnerException, string.Empty, ErrorCategory.InvalidOperation, null);
				throw;
			}
			catch (Exception exception)
			{
				errorRecord = new ErrorRecord(exception, string.Empty, ErrorCategory.InvalidOperation, null);
				throw;
			}
			finally
			{
				if (base.WorkUnits != null)
				{
					foreach (WorkUnit workUnit in base.WorkUnits)
					{
						workUnit.ExecutedCommandText = this.Command.ToString();
						if (workUnit.CurrentStatus == 1)
						{
							if (errorRecord != null)
							{
								workUnit.Errors.Add(errorRecord);
								workUnit.CurrentStatus = 3;
							}
							else if (workUnit.Errors.Count > 0)
							{
								workUnit.CurrentStatus = 3;
							}
							else
							{
								workUnit.CurrentStatus = 2;
							}
						}
						if (workUnit.CurrentStatus == null && workUnit.Errors.Count > 0)
						{
							workUnit.CurrentStatus = 3;
						}
					}
				}
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.EndExecute()");
			return result;
		}

		private Collection<PSObject> ClosePipeline(MonadAsyncResult asyncResult)
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "-->MonadCommand.ClosePipeline()");
			if (base.PowerShell == null)
			{
				throw new InvalidOperationException("The command is not currently executing.");
			}
			Exception ex = null;
			Collection<PSObject> result = new Collection<PSObject>();
			ExTraceGlobals.VerboseTracer.Information((long)this.GetHashCode(), "\tWaiting for the pipeline to finish.");
			try
			{
				base.PowerShell.EndInvoke(asyncResult.PowerShellIAsyncResult);
			}
			catch (Exception)
			{
				ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "\tPipeline End Invoke Fired an Exception.");
				if (base.PowerShell.InvocationStateInfo.Reason == null)
				{
					throw;
				}
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "\tPipeline finished.");
			if (base.PowerShell.InvocationStateInfo.State == PSInvocationState.Completed && base.PowerShell.Streams.Error.Count == 0)
			{
				result = asyncResult.Output.ReadAll();
			}
			else if (base.PowerShell.InvocationStateInfo.State == PSInvocationState.Stopped || base.PowerShell.InvocationStateInfo.State == PSInvocationState.Failed || base.PowerShell.Streams.Error.Count > 0)
			{
				ex = MonadCommand.DeserializeException(base.PowerShell.InvocationStateInfo.Reason);
				if (ex != null && (this.IsHandledException(ex) || base.PowerShell.InvocationStateInfo.State == PSInvocationState.Stopped))
				{
					ThrowTerminatingErrorException ex2 = ex as ThrowTerminatingErrorException;
					ErrorRecord errorRecord;
					if (ex2 != null)
					{
						errorRecord = ex2.ErrorRecord;
					}
					else
					{
						errorRecord = new ErrorRecord(ex, LocalizedException.GenerateErrorCode(ex).ToString("X"), ErrorCategory.InvalidOperation, null);
					}
					if (base.WorkUnits != null)
					{
						for (int i = 0; i < base.WorkUnits.Length; i++)
						{
							if (base.WorkUnits[i].CurrentStatus == 2)
							{
								if (base.PowerShell.InvocationStateInfo.State != PSInvocationState.Stopped)
								{
									this.lastUnhandledError = errorRecord;
									break;
								}
							}
							else
							{
								base.ReportError(errorRecord, i);
								base.WorkUnits[i].CurrentStatus = 3;
							}
						}
					}
					else
					{
						base.ReportError(errorRecord, -1);
					}
					ex = null;
				}
				if (ex == null)
				{
					result = asyncResult.Output.ReadAll();
					base.DrainErrorStream(-1);
				}
				asyncResult.Output.Complete();
				base.PowerShell.Streams.Error.Complete();
			}
			this.pipelineStateAtClose = base.PowerShell.InvocationStateInfo.State;
			if (ex != null && !(ex is PipelineStoppedException))
			{
				ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), ex.ToString());
				if (!(ex is CmdletInvocationException))
				{
					int innerErrorCode = LocalizedException.GenerateErrorCode(ex);
					ex = new CommandExecutionException(innerErrorCode, this.Command.ToString(), ex);
				}
				this.InteractionHandler.ReportException(ex);
				throw ex;
			}
			if (this.LastUnhandledError != null)
			{
				throw new MonadDataAdapterInvocationException(this.LastUnhandledError, this.Command.ToString());
			}
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "<--MonadCommand.ClosePipeline()");
			return result;
		}

		private bool IsHandledException(Exception e)
		{
			if (this.Command != null && this.Command.Connection.IsRemote)
			{
				return e is ThrowTerminatingErrorException || typeof(LocalizedException).IsInstanceOfType(e);
			}
			return e is ThrowTerminatingErrorException;
		}

		protected override Collection<ErrorRecord> RetrieveCurrentErrors()
		{
			if (this.Command != null && this.Command.Connection.IsRemote)
			{
				Collection<ErrorRecord> collection = new Collection<ErrorRecord>();
				foreach (ErrorRecord errorRecord in base.PowerShell.Streams.Error.ReadAll())
				{
					collection.Add(this.ResolveErrorRecord(errorRecord));
				}
				return collection;
			}
			return base.RetrieveCurrentErrors();
		}

		private ErrorRecord ResolveErrorRecord(ErrorRecord errorRecord)
		{
			RemoteException ex = errorRecord.Exception as RemoteException;
			if (ex != null)
			{
				return new ErrorRecord(MonadCommand.DeserializeException(ex), errorRecord.FullyQualifiedErrorId, errorRecord.CategoryInfo.Category, errorRecord.TargetObject);
			}
			return errorRecord;
		}

		protected override void OnWorkUnitReportProgress(ProgressRecord progressRecord)
		{
			ProgressReportEventArgs e = new ProgressReportEventArgs(progressRecord, this.Command);
			EventHandler<ProgressReportEventArgs> progressReport = this.ProgressReport;
			if (progressReport != null)
			{
				progressReport(this, e);
			}
			this.InteractionHandler.ReportProgress(e);
		}

		protected override void OnWorkUnitReportWarning(string warning, int currentIndex)
		{
			WarningReportEventArgs warningReportEventArgs = new WarningReportEventArgs(this.Command.CommandGuid, warning, currentIndex, this.Command);
			EventHandler<WarningReportEventArgs> warningReport = this.WarningReport;
			this.InteractionHandler.ReportWarning(warningReportEventArgs);
			if (warningReport != null)
			{
				warningReport(this, warningReportEventArgs);
				return;
			}
			warningReportEventArgs.Dispose();
		}

		protected override void OnWorkUnitReportError(ErrorRecord error)
		{
			ErrorReportEventArgs errorReportEventArgs;
			if (base.WorkUnits == null)
			{
				errorReportEventArgs = new ErrorReportEventArgs(this.Command.CommandGuid, error, base.CurrentWorkUnit, this.Command);
			}
			else
			{
				errorReportEventArgs = new ErrorReportEventArgs(this.Command.CommandGuid, error, base.CurrentWorkUnit, this.Command);
				errorReportEventArgs.Handled = true;
			}
			EventHandler<ErrorReportEventArgs> errorReport = this.ErrorReport;
			if (errorReport != null)
			{
				errorReport(this, errorReportEventArgs);
			}
			this.InteractionHandler.ReportErrors(errorReportEventArgs);
			if (!errorReportEventArgs.Handled)
			{
				this.lastUnhandledError = error;
			}
		}

		private CommandInteractionHandler commandInteractionHandler;

		private MonadCommand command;

		private PSInvocationState pipelineStateAtClose;

		private PSDataCollection<object> input;

		private ErrorRecord lastUnhandledError;
	}
}
