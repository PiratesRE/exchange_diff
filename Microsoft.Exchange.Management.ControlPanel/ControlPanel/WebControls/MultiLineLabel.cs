using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("MultiLineLabel", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[RequiredScript(typeof(ExtenderControlBase))]
	public class MultiLineLabel : WebControl, IScriptControl, INamingContainer
	{
		public MultiLineLabel() : base(HtmlTextWriterTag.Table)
		{
			this.label = new DivLabel();
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
			}
		}

		public Unit MaxHeight { get; set; }

		public Unit SafariMaxHeight { get; set; }

		public Unit MinHeight { get; set; }

		public bool ReserveSpace { get; set; }

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("MultiLineLabel", this.ClientID);
			if (Util.IsSafari())
			{
				Unit safariMaxHeight = this.SafariMaxHeight;
				scriptControlDescriptor.AddProperty("SafariMaxHeight", (this.SafariMaxHeight.IsEmpty ? MultiLineLabel.defaultSafariMaxHeight : this.SafariMaxHeight).ToString());
			}
			scriptControlDescriptor.AddProperty("ReserveSpace", this.ReserveSpace, true);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(typeof(MultiLineLabel));
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.Attributes.Add("cellspacing", "0");
			base.Attributes.Add("cellpadding", "0");
			this.CssClass = "multiLineLabel";
			base.OnPreRender(e);
			if (this.Page != null)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<MultiLineLabel>(this);
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
			this.label.Style.Add("max-height", (this.MaxHeight.IsEmpty ? MultiLineLabel.defaultMaxHeight : this.MaxHeight).ToString());
			this.label.Attributes.Add("tabindex", "0");
			this.label.Height = this.Height;
			if (!this.MinHeight.IsEmpty)
			{
				this.label.Style.Add("min-height", this.MinHeight.ToString());
			}
			tableCell.Controls.Add(this.label);
			tableRow.Controls.Add(tableCell);
			this.Controls.Add(tableRow);
		}

		private static Unit defaultMaxHeight = new Unit(7.0, UnitType.Em);

		private static Unit defaultSafariMaxHeight = new Unit(6.5, UnitType.Em);

		private DivLabel label;
	}
}
