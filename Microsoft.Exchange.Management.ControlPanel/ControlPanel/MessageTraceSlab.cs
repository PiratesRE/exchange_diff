using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	[ClientScriptResource("MessageTraceSlab", "Microsoft.Exchange.Management.ControlPanel.Client.MessageTrace.js")]
	public class MessageTraceSlab : SlabControl, IScriptControl
	{
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
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<MessageTraceSlab>(this);
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
			descriptor.AddElementProperty("SearchButton", this.searchButton.ClientID, true);
			descriptor.AddElementProperty("ClearButton", this.clearButton.ClientID, true);
			descriptor.AddComponentProperty("FromEditor", this.senderMatches.ClientID, true);
			descriptor.AddComponentProperty("ToEditor", this.recipientMatches.ClientID, true);
			descriptor.AddElementProperty("MessageIDTextBox", this.messageIDTextBox.ClientID, true);
			descriptor.AddComponentProperty("ListView", this.listViewSearchResults.ClientID, true);
			descriptor.AddComponentProperty("ListViewDataSource", this.messageTraceDataSource.ClientID, true);
			descriptor.AddComponentProperty("ListViewRefreshMethod", this.messageTraceDataSource.RefreshWebServiceMethod.ClientID, true);
			descriptor.AddElementProperty("DdMessageTimeFrame", this.ddMessageTimeframe.ClientID, true);
			descriptor.AddElementProperty("DdDeliveryStatus", this.ddDeliveryStatus.ClientID, true);
			descriptor.AddElementProperty("CustomLink", this.customLink.ClientID, true);
			ExDateTime dateTimeValue = ExDateTime.UtcNow.ToUserExDateTime();
			dateTimeValue = dateTimeValue.AddMinutes((double)((60 - dateTimeValue.Minute) % 30));
			descriptor.AddProperty("StartDate", dateTimeValue.AddHours(-48.0).ToUserDateTimeGeneralFormatString(), true);
			descriptor.AddProperty("EndDate", dateTimeValue.ToUserDateTimeGeneralFormatString(), true);
			ExTimeZone exTimeZone;
			if (RbacPrincipal.Current.UserTimeZone != null)
			{
				exTimeZone = RbacPrincipal.Current.UserTimeZone;
			}
			else if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName("UTC", out exTimeZone))
			{
				exTimeZone = ExTimeZone.UtcTimeZone;
			}
			descriptor.AddProperty("TimeZone", exTimeZone.Id, true);
			descriptor.AddUrlProperty("PickerFormUrl", "~/Pickers/CustomDateRangePicker.aspx", this);
		}

		protected DockPanel2 dockpanel;

		protected WebServiceListSource messageTraceDataSource;

		protected RecipientPickerControl senderMatches;

		protected RecipientPickerControl recipientMatches;

		protected DropDownList ddMessageTimeframe;

		protected DropDownList ddDeliveryStatus;

		protected TextBox messageIDTextBox;

		protected HtmlButton searchButton;

		protected HtmlButton clearButton;

		protected HyperLink customLink;

		protected Microsoft.Exchange.Management.ControlPanel.WebControls.ListView listViewSearchResults;

		protected HtmlGenericControl searchParamsFvaCanvas;
	}
}
