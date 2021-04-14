using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class WizardStepBase : Panel, IScriptControl
	{
		public WizardStepBase()
		{
			this.FvaEnabled = true;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			base.Style[HtmlTextWriterStyle.Display] = "none";
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
			ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
		}

		public string ClientClassName { get; set; }

		public string NextStepID { get; set; }

		[DefaultValue(true)]
		public bool FvaEnabled { get; set; }

		public string Title { get; set; }

		public virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			return new ScriptDescriptor[]
			{
				this.GetScriptDescriptor()
			};
		}

		protected virtual ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor(this.ClientClassName, this.ClientID);
			scriptControlDescriptor.AddProperty("FvaEnabled", this.FvaEnabled, true);
			scriptControlDescriptor.AddProperty("Title", this.Title, true);
			scriptControlDescriptor.AddComponentProperty("Form", "aspnetForm");
			return scriptControlDescriptor;
		}

		public virtual IEnumerable<ScriptReference> GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(base.GetType());
		}
	}
}
