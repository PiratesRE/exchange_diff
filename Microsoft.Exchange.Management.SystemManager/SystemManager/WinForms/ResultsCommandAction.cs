using System;
using System.Threading;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class ResultsCommandAction : CommandAction
	{
		public ResultsCommandProfile Profile
		{
			get
			{
				return base.Profile as ResultsCommandProfile;
			}
		}

		public ResultPane ResultPane
		{
			get
			{
				if (this.Profile != null)
				{
					return this.Profile.ResultPane;
				}
				return null;
			}
		}

		public ResultsCommandSetting Setting
		{
			get
			{
				if (this.Profile != null)
				{
					return this.Profile.Setting;
				}
				return null;
			}
		}

		protected IRefreshable GetDefaultRefreshObject()
		{
			if (this.Setting == null || this.ResultPane == null)
			{
				return null;
			}
			if (this.Setting.IsSelectionCommand && this.ResultPane.HasSelection && this.Setting.UseSingleRowRefresh)
			{
				return this.ResultPane.GetSelectionRefreshObjects();
			}
			if (this.Setting.UseFullRefresh)
			{
				return this.ResultPane.RefreshableDataSource;
			}
			return null;
		}

		protected void RefreshResultsThreadSafely(DataContext context)
		{
			this.InvokeRefreshResults(context);
		}

		private void InvokeRefreshResults(object obj)
		{
			if (this.ResultPane.IsHandleCreated)
			{
				if (this.ResultPane.InvokeRequired)
				{
					this.ResultPane.BeginInvoke(new SendOrPostCallback(this.InvokeRefreshResults), new object[]
					{
						obj
					});
					return;
				}
				this.RefreshResults((DataContext)obj);
			}
		}

		protected virtual void RefreshResults(DataContext context)
		{
			if (context == null || context.RefreshOnSave == null)
			{
				this.ResultPane.SetRefreshWhenActivated();
				return;
			}
			context.RefreshOnSave.Refresh(this.ResultPane.CreateProgress(this.ResultPane.RefreshCommand.Text));
		}
	}
}
