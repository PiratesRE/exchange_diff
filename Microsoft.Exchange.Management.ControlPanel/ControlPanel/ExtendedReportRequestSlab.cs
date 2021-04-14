using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("ExtendedReportRequestSlab", "Microsoft.Exchange.Management.ControlPanel.Client.ExtendedReportRequest.js")]
	public class ExtendedReportRequestSlab : SlabControl, IScriptControl
	{
		public ExtendedReportRequestSlab()
		{
			Util.RequireUpdateProgressPopUp(this);
		}

		public override Control FindControl(string id)
		{
			Control result;
			if ((result = base.FindControl(id)) == null)
			{
				if (this.dockpanel == null)
				{
					return null;
				}
				result = this.dockpanel.FindControl(id);
			}
			return result;
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

		public virtual IEnumerable<ScriptReference> GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(base.GetType());
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<ExtendedReportRequestSlab>(this);
			}
			if (base.FieldValidationAssistantExtender != null)
			{
				this.searchParamsFvaCanvas = (HtmlGenericControl)this.dockpanel.FindControl("searchParamsFvaCanvas");
				base.FieldValidationAssistantExtender.TargetControlID = this.searchParamsFvaCanvas.UniqueID;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (base.FieldValidationAssistantExtender != null)
			{
				base.FieldValidationAssistantExtender.Canvas = this.searchParamsFvaCanvas.ClientID;
			}
			this.AddAttributesToRender(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			base.Render(writer);
			writer.RenderEndTag();
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
		}

		protected void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (this.ID != null)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			}
			writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
			foreach (object obj in base.Attributes.Keys)
			{
				string text = (string)obj;
				writer.AddAttribute(text, base.Attributes[text]);
			}
		}

		private void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddElementProperty("SubmitButton", this.submitButton.ClientID, true);
			descriptor.AddElementProperty("CloseButton", this.closeButton.ClientID, true);
			descriptor.AddComponentProperty("FromEditor", this.senderMatches.ClientID, true);
			descriptor.AddComponentProperty("ToEditor", this.recipientMatches.ClientID, true);
			descriptor.AddElementProperty("MessageIDTextBox", this.messageIDTextBox.ClientID, true);
			descriptor.AddElementProperty("ClientIPTextBox", this.originalClientIPTextbox.ClientID, true);
			descriptor.AddElementProperty("ReportTitleTextBox", this.reportTitleTextbox.ClientID, true);
			string value = ExDateTime.UtcNow.ToUserExDateTime().ToString();
			descriptor.AddProperty("ServerDate", value, true);
			descriptor.AddElementProperty("NotifyAddressTextBox", this.emailTextbox.ClientID, true);
			descriptor.AddElementProperty("DdDeliveryStatusEx", this.ddDeliveryStatusEx.ClientID, true);
			descriptor.AddElementProperty("ReportOptionsPanel", "panelOnDemandOptions", true);
			descriptor.AddElementProperty("DateRangePanel", "dateRangePanel", true);
			descriptor.AddComponentProperty("StartDate", this.startDate.ClientID, true);
			descriptor.AddComponentProperty("EndDate", this.endDate.ClientID, true);
			descriptor.AddComponentProperty("DockPanel", this.dockpanel.ClientID, true);
			WebServiceReference webServiceReference = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=ExtendedReportRequest");
			descriptor.AddProperty("ServiceUrl", EcpUrl.ProcessUrl(webServiceReference.ServiceUrl));
		}

		protected DockPanel2 dockpanel;

		protected RecipientPickerControl senderMatches;

		protected RecipientPickerControl recipientMatches;

		protected DropDownList ddDeliveryStatusEx;

		protected TextBox messageIDTextBox;

		protected TextBox originalClientIPTextbox;

		protected TextBox reportTitleTextbox;

		protected TextBox emailTextbox;

		protected HtmlButton submitButton;

		protected HtmlButton closeButton;

		protected HtmlGenericControl searchParamsFvaCanvas;

		protected DateTimePicker startDate;

		protected DateTimePicker endDate;
	}
}
