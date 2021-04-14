using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:IncidentReportContentEditControl runat=server></{0}:IncidentReportContentEditControl>")]
	[ClientScriptResource("IncidentReportContentEditControl", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	public class IncidentReportContentEditControl : ScriptControlBase
	{
		public IncidentReportContentEditControl() : base(HtmlTextWriterTag.Div)
		{
		}

		public string CheckboxListControlID
		{
			get
			{
				this.EnsureChildControls();
				return this.checkBoxList.ClientID;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("CheckboxListID", this.CheckboxListControlID);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.checkBoxList = new EnumCheckBoxList();
			this.checkBoxList.ID = "checklist";
			this.checkBoxList.Items.AddRange((from e in Enum.GetNames(typeof(IncidentReportContent))
			select new ListItem(LocalizedDescriptionAttribute.FromEnum(typeof(IncidentReportContent), Enum.Parse(typeof(IncidentReportContent), e)), e.ToString())).ToArray<ListItem>());
			this.checkBoxList.CellSpacing = 2;
			this.Controls.Add(this.checkBoxList);
		}

		private EnumCheckBoxList checkBoxList;
	}
}
