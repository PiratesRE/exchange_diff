using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.TenantMonitoring;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ProvisioningMonitoring;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class CmdletHealthCountersModule : ITaskModule, ICriticalFeature
	{
		private protected TaskContext CurrentTaskContext { protected get; private set; }

		public CmdletHealthCountersModule(TaskContext context)
		{
			this.CurrentTaskContext = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return false;
		}

		public void Init(ITaskEvent task)
		{
			task.IterateCompleted += this.Task_IterateCompleted;
			task.Error += this.Task_Error;
			task.Release += this.Task_Release;
		}

		public void Dispose()
		{
		}

		protected virtual CounterType CounterTypeForAttempts
		{
			get
			{
				return CounterType.CmdletAttempts;
			}
		}

		protected virtual CounterType CounterTypeForSuccesses
		{
			get
			{
				return CounterType.CmdletSuccesses;
			}
		}

		protected virtual CounterType CounterTypeForIterationAttempts
		{
			get
			{
				return CounterType.CmdletIterationAttempts;
			}
		}

		protected virtual CounterType CounterTypeForIterationSuccesses
		{
			get
			{
				return CounterType.CmdletIterationSuccesses;
			}
		}

		protected virtual string TenantNameForMonitoringCounters
		{
			get
			{
				if (this.CurrentTaskContext.UserInfo == null)
				{
					return string.Empty;
				}
				OrganizationId currentOrganizationId = this.CurrentTaskContext.UserInfo.CurrentOrganizationId;
				if (!(currentOrganizationId == null))
				{
					return currentOrganizationId.GetFriendlyName();
				}
				return string.Empty;
			}
		}

		private CmdletHealthCounters CmdletHealthCounters
		{
			get
			{
				if (this.cmdletHealthCounters == null)
				{
					if (ProvisioningMonitoringConfig.IsCmdletMonitoringEnabled && this.CurrentTaskContext.InvocationInfo != null && this.CurrentTaskContext.ExchangeRunspaceConfig != null && ProvisioningMonitoringConfig.IsCmdletMonitored(this.CurrentTaskContext.InvocationInfo.CommandName) && ProvisioningMonitoringConfig.IsHostMonitored(this.HostApplicationForMonitoring) && (this.CurrentTaskContext.ExchangeRunspaceConfig == null || this.CurrentTaskContext.ExchangeRunspaceConfig.ConfigurationSettings == null || ProvisioningMonitoringConfig.IsClientApplicationMonitored(this.CurrentTaskContext.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication)))
					{
						this.cmdletHealthCounters = this.GetCmdletHealthCounters();
					}
					else
					{
						this.cmdletHealthCounters = ProvisioningMonitoringConfig.NullCmdletHealthCounters;
					}
				}
				return this.cmdletHealthCounters;
			}
		}

		private string HostApplicationForMonitoring
		{
			get
			{
				if (this.CurrentTaskContext.ExchangeRunspaceConfig != null && this.CurrentTaskContext.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication == ExchangeRunspaceConfigurationSettings.ExchangeApplication.SimpleDataMigration)
				{
					return this.CurrentTaskContext.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication.ToString();
				}
				string result = "";
				if (this.CurrentTaskContext.InvocationInfo != null)
				{
					result = this.CurrentTaskContext.InvocationInfo.ShellHostName;
				}
				return result;
			}
		}

		private void Task_Release(object sender, EventArgs e)
		{
			this.IncrementSuccessCount(null);
			this.IncrementInvocationCount();
		}

		private void Task_Error(object sender, GenericEventArg<TaskErrorEventArg> e)
		{
			if (e.Data.ExceptionHandled)
			{
				return;
			}
			ErrorRecord errorRecord = Task.CreateErrorRecord(this.CurrentTaskContext.ErrorInfo);
			if (this.CurrentTaskContext.Stage == TaskStage.ProcessRecord)
			{
				this.CmdletHealthCounters.UpdateIterationSuccessCount(errorRecord);
				return;
			}
			this.IncrementSuccessCount(errorRecord);
			this.IncrementInvocationCount();
		}

		private void Task_IterateCompleted(object sender, EventArgs e)
		{
			this.CmdletHealthCounters.IncrementIterationInvocationCount();
			if (this.CurrentTaskContext.ErrorInfo != null && !this.CurrentTaskContext.ErrorInfo.HasErrors)
			{
				this.CmdletHealthCounters.UpdateIterationSuccessCount(null);
			}
		}

		private CmdletHealthCounters GetCmdletHealthCounters()
		{
			return new PerTenantCmdletHealthCounters(this.CurrentTaskContext.InvocationInfo.CommandName, this.TenantNameForMonitoringCounters, this.HostApplicationForMonitoring, this.CounterTypeForAttempts, this.CounterTypeForSuccesses, this.CounterTypeForIterationAttempts, this.CounterTypeForIterationSuccesses);
		}

		private void IncrementInvocationCount()
		{
			if (!this.invocationCountIncremented)
			{
				this.CmdletHealthCounters.IncrementInvocationCount();
				this.invocationCountIncremented = true;
			}
		}

		private void IncrementSuccessCount(ErrorRecord errorRecord)
		{
			if (!this.successCountIncremented)
			{
				this.CmdletHealthCounters.UpdateSuccessCount(errorRecord);
				this.successCountIncremented = true;
			}
		}

		private CmdletHealthCounters cmdletHealthCounters;

		private bool invocationCountIncremented;

		private bool successCountIncremented;
	}
}
