using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("EllipsisLabel", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class EllipsisLabel : WebControl, IScriptControl, INamingContainer
	{
		public EllipsisLabel() : base(HtmlTextWriterTag.Table)
		{
			this.label = new DivLabel();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.Attributes.Add("cellspacing", "0");
			base.Attributes.Add("cellpadding", "0");
			base.Attributes.Add("border", "0");
			base.Style.Add("table-layout", "fixed");
			base.Style.Add(HtmlTextWriterStyle.Width, "100%");
			if (!Util.IsFirefox() && (!Util.IsSafari() || !RtlUtil.IsRtl))
			{
				this.label.Style.Add(HtmlTextWriterStyle.TextOverflow, "ellipsis");
			}
			base.OnPreRender(e);
			if (this.Page != null)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<EllipsisLabel>(this);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			this.label.ID = "textContainer";
			this.label.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
			this.label.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
			this.label.Height = this.Height;
			tableCell.Controls.Add(this.label);
			tableRow.Controls.Add(tableCell);
			this.Controls.Add(tableRow);
		}

		public WebControl TextContainer
		{
			get
			{
				return this.label;
			}
		}

		public string Text
		{
			get
			{
				return this.label.Text;
			}
			set
			{
				this.label.Text = value;
				this.label.Attributes["aria-label"] = value;
			}
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("EllipsisLabel", this.ClientID);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(typeof(EllipsisLabel));
		}

		private DivLabel label;
	}
}
