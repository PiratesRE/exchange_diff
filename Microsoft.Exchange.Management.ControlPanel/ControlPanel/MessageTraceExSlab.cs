using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("MessageTraceExSlab", "Microsoft.Exchange.Management.ControlPanel.Client.MessageTrace.js")]
	public class MessageTraceExSlab : SlabControl, IScriptControl
	{
		public MessageTraceExSlab()
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
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<MessageTraceExSlab>(this);
			}
			if (base.FieldValidationAssistantExtender != null)
			{
				this.searchParamsFvaCanvas = (HtmlGenericControl)this.dockpanel.FindControl("searchParamsFvaCanvas");
				base.FieldValidationAssistantExtender.TargetControlID = this.searchParamsFvaCanvas.UniqueID;
			}
			ExTimeZone exTimeZone = ExTimeZone.UtcTimeZone;
			if (RbacPrincipal.Current.UserTimeZone != null)
			{
				exTimeZone = RbacPrincipal.Current.UserTimeZone;
			}
			else if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName("UTC", out exTimeZone))
			{
				exTimeZone = ExTimeZone.UtcTimeZone;
			}
			foreach (ExTimeZone exTimeZone2 in ExTimeZoneEnumerator.Instance)
			{
				string text = RtlUtil.ConvertToDecodedBidiString(exTimeZone2.LocalizableDisplayName.ToString(CultureInfo.CurrentCulture), RtlUtil.IsRtl);
				this.ddTimeZone.Items.Add(new ListItem(text, exTimeZone2.Id)
				{
					Selected = (exTimeZone.Id == exTimeZone2.Id)
				});
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
			descriptor.AddElementProperty("SearchButton", this.searchButton.ClientID, true);
			descriptor.AddElementProperty("ClearButton", this.clearButton.ClientID, true);
			descriptor.AddComponentProperty("FromEditor", this.senderMatches.ClientID, true);
			descriptor.AddComponentProperty("ToEditor", this.recipientMatches.ClientID, true);
			descriptor.AddElementProperty("MessageIDTextBox", this.messageIDTextBox.ClientID, true);
			descriptor.AddElementProperty("ClientIPTextBox", this.originalClientIPTextbox.ClientID, true);
			descriptor.AddElementProperty("ReportTitleTextBox", this.reportTitleTextbox.ClientID, true);
			descriptor.AddElementProperty("NotifyAddressTextBox", this.emailTextbox.ClientID, true);
			descriptor.AddElementProperty("DdMessageTimeFrame", this.ddMessageTimeframe.ClientID, true);
			descriptor.AddElementProperty("DdDeliveryStatus", this.ddDeliveryStatus.ClientID, true);
			descriptor.AddElementProperty("DdDeliveryStatusEx", this.ddDeliveryStatusEx.ClientID, true);
			descriptor.AddElementProperty("DirectionDropdown", this.ddDirection.ClientID, true);
			descriptor.AddElementProperty("TimeZoneDropdown", this.ddTimeZone.ClientID, true);
			descriptor.AddElementProperty("ReportOptionsPanel", "panelOnDemandOptions", true);
			descriptor.AddElementProperty("DateRangePanel", "dateRangePanel", true);
			descriptor.AddElementProperty("IncludeReportDetails", this.chkReportDetails.ClientID, true);
			descriptor.AddComponentProperty("StartDate", this.startDate.ClientID, true);
			descriptor.AddComponentProperty("EndDate", this.endDate.ClientID, true);
			descriptor.AddComponentProperty("DockPanel", this.dockpanel.ClientID, true);
			WebServiceReference webServiceReference = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=MessageTraceEx");
			descriptor.AddProperty("ServiceUrl", EcpUrl.ProcessUrl(webServiceReference.ServiceUrl));
		}

		protected DockPanel2 dockpanel;

		protected RecipientPickerControl senderMatches;

		protected RecipientPickerControl recipientMatches;

		protected DropDownList ddMessageTimeframe;

		protected DropDownList ddDeliveryStatus;

		protected DropDownList ddDeliveryStatusEx;

		protected DropDownList ddTimeZone;

		protected DropDownList ddDirection;

		protected TextBox messageIDTextBox;

		protected TextBox originalClientIPTextbox;

		protected TextBox reportTitleTextbox;

		protected TextBox emailTextbox;

		protected HtmlButton searchButton;

		protected HtmlButton clearButton;

		protected HtmlGenericControl searchParamsFvaCanvas;

		protected DateTimePicker startDate;

		protected DateTimePicker endDate;

		protected CheckBox chkReportDetails;
	}
}
