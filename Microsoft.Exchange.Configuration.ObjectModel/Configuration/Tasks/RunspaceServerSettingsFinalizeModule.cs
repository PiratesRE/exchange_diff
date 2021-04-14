using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.CmdletInfra;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class RunspaceServerSettingsFinalizeModule : ITaskModule, ICriticalFeature
	{
		public RunspaceServerSettingsFinalizeModule(TaskContext context)
		{
			this.context = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		public void Init(ITaskEvent task)
		{
			task.Error += this.OnError;
			task.PreRelease += this.PreRelease;
			task.Release += this.Finalize;
			task.Stop += this.Finalize;
		}

		public void Dispose()
		{
		}

		private void OnError(object sender, GenericEventArg<TaskErrorEventArg> e)
		{
			if (e.Data.ExceptionHandled)
			{
				return;
			}
			this.MarkDcDownIfNecessary(e);
			this.TryFailOver();
		}

		private void Finalize(object sender, EventArgs e)
		{
			this.TryFailOver();
			ADSessionSettings.ClearThreadADContext();
		}

		private void PreRelease(object sender, EventArgs e)
		{
			ADDriverContext threadADContext = ADSessionSettings.GetThreadADContext();
			ADServerSettings serverSettings = (threadADContext != null) ? threadADContext.ServerSettings : null;
			CmdletLogHelper.LogADServerSettingsAfterCmdExecuted(this.context.UniqueId, serverSettings);
		}

		private void MarkDcDownIfNecessary(GenericEventArg<TaskErrorEventArg> e)
		{
			if (e.Data.ExceptionHandled)
			{
				return;
			}
			ADDriverContext threadADContext = ADSessionSettings.GetThreadADContext();
			ADServerSettings adserverSettings = (threadADContext != null) ? threadADContext.ServerSettings : null;
			if (this.context == null || adserverSettings == null)
			{
				return;
			}
			string text = null;
			for (Exception ex = e.Data.Exception; ex != null; ex = ex.InnerException)
			{
				if (ex is SuitabilityDirectoryException)
				{
					text = ((SuitabilityDirectoryException)ex).Fqdn;
					break;
				}
				if (ex is ServerInMMException)
				{
					text = ((ServerInMMException)ex).Fqdn;
					break;
				}
				if (ex is ADServerSettingsChangedException)
				{
					ADServerSettings serverSettings = ((ADServerSettingsChangedException)ex).ServerSettings;
					this.PersistNewServerSettings(serverSettings);
					break;
				}
				if (ex == ex.InnerException)
				{
					break;
				}
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			Fqdn fqdn;
			if (Fqdn.TryParse(text, out fqdn))
			{
				try
				{
					adserverSettings.MarkDcDown(fqdn);
					CmdletLogger.SafeAppendGenericInfo(this.context.UniqueId, "MarkDcDown", fqdn);
					return;
				}
				catch (NotSupportedException)
				{
					CmdletLogger.SafeAppendGenericInfo(this.context.UniqueId, "MarkDcDown-NotSupportedException", fqdn);
					return;
				}
			}
			CmdletLogger.SafeAppendGenericInfo(this.context.UniqueId, "MarkDcDown-InvalidFqdn", fqdn);
		}

		private void TryFailOver()
		{
			ADDriverContext threadADContext = ADSessionSettings.GetThreadADContext();
			ADServerSettings adserverSettings = (threadADContext != null) ? threadADContext.ServerSettings : null;
			if (adserverSettings != null && adserverSettings.IsFailoverRequired())
			{
				ADServerSettings newServerSettings;
				string str;
				if (adserverSettings.TryFailover(out newServerSettings, out str, false))
				{
					this.PersistNewServerSettings(newServerSettings);
					return;
				}
				CmdletLogger.SafeAppendGenericInfo(this.context.UniqueId, "ADServerSettings-TryFailOver", "Failed - " + str);
			}
		}

		private void PersistNewServerSettings(ADServerSettings newServerSettings)
		{
			if (newServerSettings != null)
			{
				ADSessionSettings.ClearThreadADContext();
				LocalizedString adserverSettings;
				if (CmdletLogHelper.NeedConvertLogMessageToUS)
				{
					adserverSettings = TaskVerboseStringHelper.GetADServerSettings(this.context.InvocationInfo.CommandName, newServerSettings, CmdletLogHelper.DefaultLoggingCulture);
				}
				else
				{
					adserverSettings = TaskVerboseStringHelper.GetADServerSettings(this.context.InvocationInfo.CommandName, newServerSettings);
				}
				if (this.context.SessionState != null)
				{
					CmdletLogger.SafeAppendGenericInfo(this.context.UniqueId, "ADServerSettings-FailOver", adserverSettings);
					ExchangePropertyContainer.SetServerSettings(this.context.SessionState, newServerSettings);
				}
				else
				{
					CmdletLogger.SafeAppendGenericInfo(this.context.UniqueId, "ADServerSettings-NOTFailOver-SessionStateNull", adserverSettings);
				}
				ADSessionSettings.SetThreadADContext(new ADDriverContext(newServerSettings, ContextMode.Cmdlet));
				this.context.ServerSettingsAfterFailOver = newServerSettings;
				this.context.CommandShell.WriteWarning(DirectoryStrings.RunspaceServerSettingsChanged);
				return;
			}
			CmdletLogger.SafeAppendGenericInfo(this.context.UniqueId, "ADServerSettings-NOTFailOver-ServerSettingsNull", "NULL");
		}

		private readonly TaskContext context;
	}
}
