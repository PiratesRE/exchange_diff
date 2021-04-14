using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("WaterMarkedTextArea", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ToolboxData("<{0}:WaterMarkedTextArea runat=server></{0}:WaterMarkedTextArea>")]
	[ControlValueProperty("Value")]
	[RequiredScript(typeof(ExtenderControlBase))]
	public class WaterMarkedTextArea : TextArea, IScriptControl
	{
		public WaterMarkedTextArea()
		{
			this.watermarkExtender = new TextBoxWatermarkExtender();
			this.WatermarkExtender.ID = "watermarkBehavior";
			this.Controls.Add(this.WatermarkExtender);
		}

		protected override void OnPreRender(EventArgs e)
		{
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<WaterMarkedTextArea>(this);
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
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("WaterMarkedTextArea", this.ClientID);
			scriptControlDescriptor.AddComponentProperty("WatermarkBehavior", this.WatermarkExtender.ClientID);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			IEnumerable<ScriptReference> enumerable = new List<ScriptReference>();
			return ScriptObjectBuilder.GetScriptReferences(typeof(WaterMarkedTextArea));
		}

		private TextBoxWatermarkExtender watermarkExtender;
	}
}
