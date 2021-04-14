using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("DockElement", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class DockElement : ScriptControlBase
	{
		public DockElement() : base(HtmlTextWriterTag.Div)
		{
			this.Dock = DockStyle.Fill;
		}

		[DefaultValue(DockStyle.Fill)]
		public DockStyle Dock { get; set; }

		[DefaultValue(null)]
		[TemplateInstance(TemplateInstance.Single)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate Content { get; set; }

		[DefaultValue(null)]
		public string Borders { get; set; }

		[DefaultValue(false)]
		public bool AutoSize { get; set; }

		internal DockPanelBorderWidths BorderWidths
		{
			get
			{
				if (this.borderWidth == null)
				{
					this.borderWidth = new DockPanelBorderWidths(this.Borders);
				}
				return this.borderWidth;
			}
		}

		internal DockElement FillPanel { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			DockElement dockElement = (this.Controls.Count > 0) ? (this.Controls[0] as DockElement) : null;
			if (dockElement != null)
			{
				descriptor.AddComponentProperty("ChildDockPanel", dockElement);
			}
			else if (this.FillPanel != null)
			{
				descriptor.AddComponentProperty("FillPanel", this.FillPanel);
			}
			if (this.Dock != DockStyle.Fill)
			{
				descriptor.AddProperty("Dock", this.Dock);
			}
			descriptor.AddProperty("AutoSize", this.AutoSize, true);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.Content != null)
			{
				this.Content.InstantiateIn(this);
			}
			string text = "DockElement abs0 ";
			switch (this.Dock)
			{
			case DockStyle.Fill:
				this.Width = Unit.Empty;
				this.Height = Unit.Empty;
				break;
			case DockStyle.Top:
				text += "dpTop";
				this.Width = Unit.Empty;
				break;
			case DockStyle.Left:
				text += "dpLeft";
				this.Height = Unit.Empty;
				break;
			case DockStyle.Right:
				text += "dpRight";
				this.Height = Unit.Empty;
				break;
			case DockStyle.Bottom:
				text += "dpBottom";
				this.Width = Unit.Empty;
				break;
			}
			this.CssClass = this.CssClass + " " + text;
			base.Style.Add(HtmlTextWriterStyle.BorderWidth, this.BorderWidths.FormatCssString(RtlUtil.IsRtl));
		}

		public Control InternalFindControl(string id)
		{
			return this.FindControl(id, 0);
		}

		private DockPanelBorderWidths borderWidth;
	}
}
