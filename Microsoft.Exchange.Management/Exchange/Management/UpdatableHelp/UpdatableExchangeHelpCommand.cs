using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	[Cmdlet("Update", "ExchangeHelp")]
	public sealed class UpdatableExchangeHelpCommand : Task
	{
		[Parameter]
		public SwitchParameter Force { get; set; }

		internal void HandleProgressChanged(object sender, UpdatableExchangeHelpProgressEventArgs e)
		{
			base.WriteProgress(e.Activity, e.ProgressStatus, e.PercentCompleted);
		}

		internal UpdatableExchangeHelpCommand()
		{
			Random random = new Random();
			this.activityId = random.Next();
			this.helpUpdater = new HelpUpdater(this);
		}

		internal bool Abort { get; private set; }

		protected override void InternalStopProcessing()
		{
			TaskLogger.LogEnter();
			this.Abort = true;
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			UpdatableExchangeHelpSystemException ex = null;
			try
			{
				this.helpUpdater.LoadConfiguration();
			}
			catch (Exception ex2)
			{
				if (ex2.GetType() == typeof(UpdatableExchangeHelpSystemException))
				{
					ex = (UpdatableExchangeHelpSystemException)ex2;
				}
				else
				{
					ex = new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateGeneralExceptionErrorID, UpdatableHelpStrings.UpdateGeneralException, ErrorCategory.InvalidOperation, null, ex2);
				}
			}
			if (ex != null)
			{
				this.WriteError(ex, ex.ErrorCategory, ex.TargetObject, false);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			UpdatableExchangeHelpSystemException ex = null;
			try
			{
				ex = this.helpUpdater.UpdateHelp();
			}
			catch (Exception ex2)
			{
				if (ex2.GetType() == typeof(UpdatableExchangeHelpSystemException))
				{
					ex = (UpdatableExchangeHelpSystemException)ex2;
				}
				else
				{
					ex = new UpdatableExchangeHelpSystemException(UpdatableHelpStrings.UpdateGeneralExceptionErrorID, UpdatableHelpStrings.UpdateGeneralException, ErrorCategory.InvalidOperation, null, ex2);
				}
			}
			if (ex != null)
			{
				this.WriteError(ex, ex.ErrorCategory, ex.TargetObject, false);
			}
			TaskLogger.LogExit();
		}

		internal int activityId;

		private HelpUpdater helpUpdater;
	}
}
