using System;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ResultsCommandUpdatingUtil : CommandUpdatingUtil
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

		public OrganizationType[] OrganizationTypes { get; set; }

		protected override void OnProfileUpdated()
		{
			base.OnProfileUpdated();
			base.Command.Visible = WinformsHelper.IsCurrentOrganizationAllowed(this.OrganizationTypes);
		}
	}
}
