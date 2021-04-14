using System;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ResultsCommandProfile : CommandProfile
	{
		public ResultsCommandSetting Setting
		{
			get
			{
				return this.setting;
			}
			set
			{
				if (this.Setting != value)
				{
					base.BeginUpdate();
					if (this.Setting != null)
					{
						base.Components.Remove(this.Setting);
					}
					this.setting = value;
					if (this.Setting != null)
					{
						base.Components.Add(this.Setting);
					}
					base.EndUpdate();
				}
			}
		}

		protected override void OnUpdated(EventArgs e)
		{
			if (this.Setting != null)
			{
				this.Setting.Profile = this;
			}
			base.OnUpdated(e);
		}

		public ResultPane ResultPane
		{
			get
			{
				return this.resultPane;
			}
			set
			{
				if (this.ResultPane != value)
				{
					base.BeginUpdate();
					this.resultPane = value;
					base.EndUpdate();
				}
			}
		}

		public static ResultsCommandProfile CreateSeparator()
		{
			return new ResultsCommandProfile
			{
				Command = Command.CreateSeparator()
			};
		}

		private ResultsCommandSetting setting;

		private ResultPane resultPane;
	}
}
