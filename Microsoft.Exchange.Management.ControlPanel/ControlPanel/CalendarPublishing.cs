using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(Properties))]
	[ClientScriptResource("CalendarPublishing", "Microsoft.Exchange.Management.ControlPanel.Client.Calendar.js")]
	public class CalendarPublishing : SlabControl, IScriptControl
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			string displayName = CalendarSharingsSlab.GetDisplayName(this.Context.Request, "fldID");
			base.Title = string.Format("{0} - {1}", base.Title, displayName);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<CalendarPublishing>(this);
			}
			if (base.FieldValidationAssistantExtender != null)
			{
				base.FieldValidationAssistantExtender.Canvas = this.calendarPublishingPropertiesWrapper.ClientID;
				base.FieldValidationAssistantExtender.TargetControlID = this.calendarPublishingPropertiesWrapper.UniqueID;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.ID != null)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
			}
			base.Render(writer);
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
			descriptor.AddElementProperty("SubscriptionUrlLabel", this.txtSubscriptionUrl_label.ClientID, true);
			descriptor.AddElementProperty("SubscriptionUrl", this.txtSubscriptionUrl.ClientID, true);
			descriptor.AddElementProperty("ViewUrlLabel", this.txtViewUrl_label.ClientID, true);
			descriptor.AddElementProperty("ViewUrl", this.txtViewUrl.ClientID, true);
		}

		protected PropertiesWrapper calendarPublishingPropertiesWrapper;

		protected Label txtSubscriptionUrl_label;

		protected TextBox txtSubscriptionUrl;

		protected Label txtViewUrl_label;

		protected TextBox txtViewUrl;
	}
}
