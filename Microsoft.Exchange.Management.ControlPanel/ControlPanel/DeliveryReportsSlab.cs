using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("DeliveryReportsSlab", "Microsoft.Exchange.Management.ControlPanel.Client.DeliveryReports.js")]
	public class DeliveryReportsSlab : SlabControl, IScriptControl
	{
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

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.SetupFilterBindings();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptControl<DeliveryReportsSlab>(this);
			}
			if (base.FieldValidationAssistantExtender != null)
			{
				base.FieldValidationAssistantExtender.Canvas = this.searchParamsFvaCanvas.ClientID;
				base.FieldValidationAssistantExtender.TargetControlID = this.searchParamsFvaCanvas.UniqueID;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
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

		private void SetupFilterBindings()
		{
			BindingCollection filterParameters = this.messageTrackingsearchDataSource.FilterParameters;
			if (this.pickerMailboxToSearch != null)
			{
				filterParameters.Add(new ComponentBinding(this.pickerMailboxToSearch, "value")
				{
					Name = "Identity"
				});
			}
			ClientControlBinding clientControlBinding = new ComponentBinding(this.fromAddress, "value");
			clientControlBinding.Name = "Sender";
			ClientControlBinding clientControlBinding2 = new ComponentBinding(this.toAddress, "value");
			clientControlBinding2.Name = "Recipients";
			ClientControlBinding clientControlBinding3 = new ClientControlBinding(this.subjectTextBox, "value");
			clientControlBinding3.Name = "Subject";
			filterParameters.Add(clientControlBinding);
			filterParameters.Add(clientControlBinding2);
			filterParameters.Add(clientControlBinding3);
		}

		private void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			descriptor.AddElementProperty("SearchButton", this.searchButton.ClientID, true);
			descriptor.AddElementProperty("ClearButton", this.clearButton.ClientID, true);
			descriptor.AddComponentProperty("FromEditor", this.fromAddress.ClientID, true);
			descriptor.AddComponentProperty("ToEditor", this.toAddress.ClientID, true);
			descriptor.AddElementProperty("SubjectTextBox", this.subjectTextBox.ClientID, true);
			descriptor.AddComponentProperty("ListView", this.listViewSearchResults.ClientID, true);
			descriptor.AddComponentProperty("ListViewDataSource", this.messageTrackingsearchDataSource.ClientID, true);
			descriptor.AddComponentProperty("ListViewRefreshMethod", this.messageTrackingsearchDataSource.RefreshWebServiceMethod.ClientID, true);
			descriptor.AddElementProperty("ToAddressRadioButton", this.rbToAddress.ClientID, true);
			descriptor.AddElementProperty("FromAddressRadioButton", this.rbFromAddress.ClientID, true);
			descriptor.AddProperty("FvaResource", base.FVAResource);
			if (this.pickerMailboxToSearch != null)
			{
				descriptor.AddComponentProperty("MailboxPicker", this.pickerMailboxToSearch.ClientID, true);
			}
		}

		protected WebServiceListSource messageTrackingsearchDataSource;

		protected RecipientPickerControl fromAddress;

		protected RecipientPickerControl toAddress;

		protected TextBox subjectTextBox;

		protected HtmlButton searchButton;

		protected HtmlButton clearButton;

		protected LoginView loginView;

		protected RadioButton rbToAddress;

		protected RadioButton rbFromAddress;

		protected Microsoft.Exchange.Management.ControlPanel.WebControls.ListView listViewSearchResults;

		protected EcpSingleSelect pickerMailboxToSearch;

		protected HtmlGenericControl searchParamsFvaCanvas;
	}
}
