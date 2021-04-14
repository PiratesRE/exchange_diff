using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class SelectionCommandUpdatingUtil : ResultsCommandUpdatingUtil
	{
		protected override void OnProfileUpdating()
		{
			if (base.ResultPane != null && base.Command != null)
			{
				base.ResultPane.SelectionChanged -= this.ResultPane_SelectionChanged;
			}
			base.OnProfileUpdating();
		}

		protected override void OnProfileUpdated()
		{
			base.OnProfileUpdated();
			if (base.ResultPane != null && base.Command != null && WinformsHelper.IsCurrentOrganizationAllowed(base.OrganizationTypes))
			{
				base.ResultPane.SelectionChanged += this.ResultPane_SelectionChanged;
				this.ResultPane_SelectionChanged(base.ResultPane, EventArgs.Empty);
			}
		}

		private void ResultPane_SelectionChanged(object sender, EventArgs e)
		{
			if (base.ResultPane.HasSelection)
			{
				base.Command.Visible = true;
				this.UpdateCommand();
				if (base.Setting.RequiresSingleSelection)
				{
					base.Command.Visible = (base.Command.Visible && base.ResultPane.HasSingleSelection);
					return;
				}
				if (base.Setting.RequiresMultiSelection)
				{
					base.Command.Visible = (base.Command.Visible && base.ResultPane.HasMultiSelection);
					return;
				}
			}
			else
			{
				base.Command.Visible = false;
			}
		}

		protected virtual void UpdateCommand()
		{
		}
	}
}
