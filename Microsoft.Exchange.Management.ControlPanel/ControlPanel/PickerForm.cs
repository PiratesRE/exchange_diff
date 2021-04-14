using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("PickerForm", "Microsoft.Exchange.Management.ControlPanel.Client.Pickers.js")]
	public class PickerForm : PopupForm
	{
		public PickerForm()
		{
			base.CommitButtonText = Strings.OkButtonText;
			base.ShowHeader = false;
			base.HideFieldValidationAssistant = true;
			base.ShowHelp = false;
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string SelectionMode
		{
			get
			{
				return this.PickerContent.SelectionMode.ToString().ToLower();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string PickerContentID
		{
			get
			{
				return this.PickerContent.ClientID;
			}
		}

		protected PickerContent PickerContent
		{
			get
			{
				if (this.pickerContent == null)
				{
					foreach (object obj in base.ContentPanel.Controls)
					{
						Control control = (Control)obj;
						PickerContent pickerContent = control as PickerContent;
						if (pickerContent != null)
						{
							this.pickerContent = pickerContent;
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

		private PickerContent FindPickerContentRecursive(Control control)
		{
			if (control != null)
			{
				PickerContent pickerContent = control as PickerContent;
				if (pickerContent != null)
				{
					return pickerContent;
				}
				foreach (object obj in control.Controls)
				{
					Control control2 = (Control)obj;
					pickerContent = this.FindPickerContentRecursive(control2);
					if (pickerContent != null)
					{
						return pickerContent;
					}
				}
			}
			return null;
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("SelectionMode", this.SelectionMode, true);
			descriptor.AddComponentProperty("PickerContent", this.PickerContentID, true);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			try
			{
				this.PickerContent.SelectionMode = (PickerSelectionType)Enum.Parse(typeof(PickerSelectionType), base.Request["mode"] ?? string.Empty, true);
			}
			catch (ArgumentException innerException)
			{
				throw new BadQueryParameterException("mode", innerException);
			}
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

		private PickerContent pickerContent;
	}
}
