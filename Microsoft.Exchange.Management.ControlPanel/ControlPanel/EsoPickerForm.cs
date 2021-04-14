using System;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("EsoPickerForm", "Microsoft.Exchange.Management.ControlPanel.Client.Pickers.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class EsoPickerForm : PickerForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			StaticBinding staticBinding = new StaticBinding();
			staticBinding.Name = "IsCrossPremise";
			if (string.IsNullOrEmpty(base.Request.QueryString["xprs"]))
			{
				foreach (ColumnHeader columnHeader in this.pickerContent.Columns)
				{
					if (columnHeader.Name == "RecipientTypeName")
					{
						this.pickerContent.Columns.Remove(columnHeader);
						break;
					}
				}
				staticBinding.Value = "false";
			}
			else
			{
				staticBinding.Value = "true";
			}
			this.pickerContent.FilterParameters.Add(staticBinding);
			if (DDIHelper.IsFFO())
			{
				base.Title = Strings.MasterAccountPickerTitle;
			}
		}

		protected PickerContent pickerContent;
	}
}
