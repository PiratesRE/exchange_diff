using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[RequiredScript(typeof(ScriptControlBase))]
	public abstract class ScriptComponent : ScriptControlBase
	{
		protected ScriptComponent() : base(false)
		{
		}

		[Browsable(false)]
		public string ComponentID
		{
			get
			{
				return this.ClientID;
			}
		}

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
		}

		public override void RenderEndTag(HtmlTextWriter writer)
		{
		}

		protected override List<ScriptDescriptor> CreateScriptDescriptors()
		{
			return new List<ScriptDescriptor>(1)
			{
				new ScriptComponentDescriptor(this.ClientControlType)
			};
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddProperty("id", this.ClientID);
		}
	}
}
