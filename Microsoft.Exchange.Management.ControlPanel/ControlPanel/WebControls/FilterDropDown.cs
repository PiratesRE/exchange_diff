using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:FilterDropDown runat=server></{0}:FilterDropDown>")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("FilterDropDown", "Microsoft.Exchange.Management.ControlPanel.Client.List.js")]
	public class FilterDropDown : DropDownList, IScriptControl
	{
		public FilterDropDown()
		{
			this.AutoPostBack = false;
			this.selectViewLabel = new Label();
		}

		[DefaultValue(null)]
		public string LabelText { get; set; }

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("FilterDropDown", this.ClientID);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(typeof(FilterDropDown));
		}

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			this.selectViewLabel.Text = (this.LabelText ?? Strings.ViewFilterLabel);
			writer.Write("<table cellspacing='0' cellpadding='0' width='80%' style='margin-top:2px;margin-bottom:1px'>");
			writer.Write("<tr>");
			writer.Write("<td width='20%'>");
			this.selectViewLabel.RenderControl(writer);
			writer.Write("</td>");
			writer.Write("<td width='80%'>");
			base.RenderBeginTag(writer);
		}

		public override void RenderEndTag(HtmlTextWriter writer)
		{
			base.RenderEndTag(writer);
			writer.Write("</td>");
			writer.Write("</tr>");
			writer.Write("</table>");
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<FilterDropDown>(this);
			this.selectViewLabel.ID = this.ClientID + "_label";
			ScriptObjectBuilder.RegisterCssReferences(this);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
		}

		private Label selectViewLabel;
	}
}
