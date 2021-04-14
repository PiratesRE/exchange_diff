using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal class EMCRunspaceConfigurationSingleton : RefreshableComponent
	{
		internal void PerformAction(EMCRunspaceConfigurationSingleton.DoActionWhenSucceeded doAction)
		{
			if (this.finished)
			{
				doAction();
			}
			base.RefreshCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
			{
				if (this.finished)
				{
					doAction();
				}
			};
		}

		internal void PerformActionWhenFailed(EMCRunspaceConfigurationSingleton.DoActionWhenFailed doAction)
		{
			if (this.finished && this.exception != null)
			{
				doAction(this.ErrorMessage, this.exception);
			}
			base.RefreshCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
			{
				if (this.finished && this.exception != null)
				{
					doAction(this.ErrorMessage, this.exception);
				}
			};
		}

		internal void PerformActionWhenSucceeded(EMCRunspaceConfigurationSingleton.DoActionWhenSucceeded doAction)
		{
			if (this.finished && this.exception == null)
			{
				doAction();
			}
			base.RefreshCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
			{
				if (this.finished && this.exception == null)
				{
					doAction();
				}
			};
		}

		private void ResetStatus()
		{
			this.finished = false;
			this.exception = null;
		}

		protected override void OnDoRefreshWork(RefreshRequestEventArgs e)
		{
			this.progress = new RefreshRequestEventArgsToIProgressAdapter(e);
			try
			{
				this.ResetStatus();
				PSConnectionInfoSingleton.GetInstance().ReportProgress = this.progress;
				PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo();
				this.progress.ReportProgress(50, 100, Strings.ProgressReportInitializeHelpService, Strings.ProgressReportInitializeHelpServiceErrorText);
				ExchangeHelpService.Initialize();
				this.erc = CmdletBasedRunspaceConfiguration.Create(PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo(), PSConnectionInfoSingleton.GetInstance().UserAccount, this.progress);
				this.TenantDomain = ((this.erc.LogonUserLiveID == SmtpAddress.Empty) ? null : this.erc.LogonUserLiveID.Domain);
			}
			finally
			{
				this.progress.ReportProgress(100, 100, Strings.ProgressReportEnd, string.Empty);
				PSConnectionInfoSingleton.GetInstance().ReportProgress = null;
			}
			base.OnDoRefreshWork(e);
		}

		protected override void OnRefreshCompleted(RunWorkerCompletedEventArgs e)
		{
			this.finished = true;
			if (e.Error != null)
			{
				this.exception = e.Error;
			}
			base.OnRefreshCompleted(e);
		}

		internal string TenantDomain { get; private set; }

		internal string StepDescription
		{
			get
			{
				if (this.progress == null)
				{
					return string.Empty;
				}
				return this.progress.StepDescription;
			}
		}

		internal string ErrorHeader
		{
			get
			{
				if (this.progress == null)
				{
					return string.Empty;
				}
				return this.progress.ErrorHeader;
			}
		}

		[DefaultValue(0)]
		internal int ProgressValue
		{
			get
			{
				if (this.progress == null)
				{
					return 0;
				}
				return this.progress.Value;
			}
		}

		private string ErrorMessage
		{
			get
			{
				if (ExceptionHelper.IsWellknownCommandExecutionException(this.exception))
				{
					return this.exception.InnerException.Message;
				}
				if (this.exception != null)
				{
					return this.exception.Message;
				}
				return null;
			}
		}

		public bool IsCmdletAllowedInScope(string cmdletName, string[] paramNames)
		{
			if (this.erc != null)
			{
				ScopeSet scopeSet = new ScopeSet(new ADScope(null, null), new ADScopeCollection[0], null, null);
				return this.erc.IsCmdletAllowedInScope("Microsoft.Exchange.Management.PowerShell.E2010\\" + cmdletName.Trim(), EMCRunspaceConfigurationSingleton.TrimArray(paramNames), scopeSet);
			}
			return true;
		}

		private static string[] TrimArray(string[] paramNames)
		{
			if (paramNames != null)
			{
				return (from c in paramNames
				select c.Trim()).ToArray<string>();
			}
			return null;
		}

		public ICollection<string> GetAllowedParamsForSetCmdlet(string cmdletName, ADRawEntry adObject)
		{
			if (this.erc != null)
			{
				return this.erc.GetAllowedParamsForSetCmdlet("Microsoft.Exchange.Management.PowerShell.E2010\\" + cmdletName.Trim(), null, ScopeLocation.RecipientWrite);
			}
			return null;
		}

		public bool IsCmdletAllowedInScope(MonadCommand command)
		{
			if (this.erc != null)
			{
				List<string> list = new List<string>(command.Parameters.Count);
				foreach (object obj in command.Parameters)
				{
					MonadParameter monadParameter = (MonadParameter)obj;
					list.Add(monadParameter.ParameterName);
				}
				return this.IsCmdletAllowedInScope(command.CommandText, list.ToArray());
			}
			return true;
		}

		public static EMCRunspaceConfigurationSingleton GetInstance()
		{
			return EMCRunspaceConfigurationSingleton.instance;
		}

		private const string PSSnapInName = "Microsoft.Exchange.Management.PowerShell.E2010";

		private CmdletBasedRunspaceConfiguration erc;

		private bool finished;

		private Exception exception;

		private RefreshRequestEventArgsToIProgressAdapter progress;

		private static EMCRunspaceConfigurationSingleton instance = new EMCRunspaceConfigurationSingleton();

		internal delegate void DoActionWhenSucceeded();

		internal delegate void DoActionWhenFailed(string errorMessage, Exception exception);
	}
}
