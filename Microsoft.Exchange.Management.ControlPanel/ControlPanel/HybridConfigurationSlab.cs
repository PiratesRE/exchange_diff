using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("HybridConfigurationSlab", "Microsoft.Exchange.Management.ControlPanel.Client.OrgSettings.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class HybridConfigurationSlab : SlabControl, IScriptControl
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<HybridConfigurationSlab>(this);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.ID != null)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			base.Render(writer);
			writer.RenderEndTag();
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
		}

		public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			ClientScriptResourceAttribute clientScriptResourceAttribute = (ClientScriptResourceAttribute)TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor(clientScriptResourceAttribute.ComponentType, this.ClientID);
			this.BuildScriptDescriptor(scriptControlDescriptor);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		public IEnumerable<ScriptReference> GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(base.GetType());
		}

		private void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddElementProperty("EnableButton", this.btnEnable.ClientID, true);
			descriptor.AddElementProperty("EditButton", this.btnEdit.ClientID, true);
			descriptor.AddElementProperty("IsGallatinCheckbox", this.chkIsHostedOnGallatin.ClientID, true);
			descriptor.AddProperty("LinkToCrossPremiseWorldWide", CrossPremiseUtil.OnPremiseLinkToOffice365WorldWide);
			descriptor.AddProperty("LinkToCrossPremiseGallatin", CrossPremiseUtil.OnPremiseLinkToOffice365Gallatin);
		}

		protected HtmlInputButton btnEnable;

		protected HtmlInputButton btnEdit;

		protected CheckBox chkIsHostedOnGallatin;
	}
}
