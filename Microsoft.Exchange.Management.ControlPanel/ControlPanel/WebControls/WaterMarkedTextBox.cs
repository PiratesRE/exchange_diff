using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:WaterMarkedTextBox runat=server></{0}:WaterMarkedTextBox>")]
	[RequiredScript(typeof(ExtenderControlBase))]
	[ClientScriptResource("WaterMarkedTextBox", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	[ControlValueProperty("Value")]
	public class WaterMarkedTextBox : TextBox, IScriptControl
	{
		public WaterMarkedTextBox()
		{
			this.watermarkExtender = new TextBoxWatermarkExtender();
			this.WatermarkExtender.ID = "watermarkBehavior" + Guid.NewGuid();
			this.Controls.Add(this.WatermarkExtender);
		}

		protected override void OnPreRender(EventArgs e)
		{
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<WaterMarkedTextBox>(this);
			this.WatermarkExtender.TargetControlID = this.ID;
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			this.RenderBeginTag(writer);
			if (this.TextMode == TextBoxMode.MultiLine)
			{
				HttpUtility.HtmlEncode(this.Text, writer);
			}
			this.RenderChildren(writer);
			this.RenderEndTag(writer);
		}

		public string WatermarkText
		{
			get
			{
				return this.WatermarkExtender.WatermarkText;
			}
			set
			{
				this.WatermarkExtender.WatermarkText = value;
			}
		}

		public string WaterMarkCssClass
		{
			get
			{
				return this.WatermarkExtender.WatermarkCssClass;
			}
			set
			{
				this.WatermarkExtender.WatermarkCssClass = value;
			}
		}

		private TextBoxWatermarkExtender WatermarkExtender
		{
			get
			{
				return this.watermarkExtender;
			}
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("WaterMarkedTextBox", this.ClientID);
			scriptControlDescriptor.AddComponentProperty("WatermarkBehavior", this.WatermarkExtender.ClientID);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			IEnumerable<ScriptReference> enumerable = new List<ScriptReference>();
			return ScriptObjectBuilder.GetScriptReferences(typeof(WaterMarkedTextBox));
		}

		private TextBoxWatermarkExtender watermarkExtender;
	}
}
