using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("BasicPickerForm", "Microsoft.Exchange.Management.ControlPanel.Client.Pickers.js")]
	public class BasicPickerForm : PopupForm
	{
		public BasicPickerForm()
		{
			base.CommitButtonText = Strings.OkButtonText;
			base.ShowHeader = false;
			base.HideFieldValidationAssistant = true;
			base.ShowHelp = false;
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string PickerContentID
		{
			get
			{
				return this.PickerContent.ClientID;
			}
		}

		protected BasicPickerContent PickerContent
		{
			get
			{
				if (this.pickerContent == null)
				{
					foreach (object obj in base.ContentPanel.Controls)
					{
						Control control = (Control)obj;
						BasicPickerContent basicPickerContent = control as BasicPickerContent;
						if (basicPickerContent != null)
						{
							this.pickerContent = basicPickerContent;
							break;
						}
					}
				}
				if (this.pickerContent == null)
				{
					this.pickerContent = this.FindPickerContentRecursive(base.ContentPanel);
				}
				return this.pickerContent;
			}
		}

		private BasicPickerContent FindPickerContentRecursive(Control control)
		{
			if (control != null)
			{
				BasicPickerContent basicPickerContent = control as BasicPickerContent;
				if (basicPickerContent != null)
				{
					return basicPickerContent;
				}
				foreach (object obj in control.Controls)
				{
					Control control2 = (Control)obj;
					basicPickerContent = this.FindPickerContentRecursive(control2);
					if (basicPickerContent != null)
					{
						return basicPickerContent;
					}
				}
			}
			return null;
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("PickerContent", this.PickerContentID, true);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.PickerContent.WrapperControlID = base.ContentPanel.ClientID;
			if (!base.InPagePanel.CssClass.Contains("noHdr") && string.IsNullOrEmpty(base.Caption))
			{
				Panel inPagePanel = base.InPagePanel;
				inPagePanel.CssClass += " noCap";
			}
		}

		private BasicPickerContent pickerContent;
	}
}
