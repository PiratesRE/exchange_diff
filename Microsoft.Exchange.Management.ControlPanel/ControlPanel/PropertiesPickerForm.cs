using System;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class PropertiesPickerForm : BaseForm
	{
		public PropertiesPickerForm()
		{
			base.ShowHeader = false;
			base.ShowHelp = false;
			base.HideFieldValidationAssistant = true;
		}

		protected Properties Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = (base.ContentControl as Properties);
				}
				return this.properties;
			}
		}

		protected PickerContent PickerContent
		{
			get
			{
				if (this.pickerContent == null)
				{
					foreach (object obj in this.Properties.ContentContainer.Controls)
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
				return this.pickerContent;
			}
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
			Properties properties = this.Properties;
			properties.CssClass += "PickerPane";
		}

		private Properties properties;

		private PickerContent pickerContent;
	}
}
