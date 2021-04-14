using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class EumProxyAddressDialog : ProxyAddressDialog
	{
		public EumProxyAddressDialog()
		{
			this.InitializeComponent();
			this.Text = Strings.EumProxyAddress;
			this.addressLabel.Text = Strings.AddressOrExtension;
			this.dialplanLabel.Text = Strings.DialPlanPhoneContext;
			base.Prefix = ProxyAddressPrefix.UM.PrimaryPrefix;
			base.ContentPage.BindingSource.DataSource = typeof(EumProxyAddressDataHandler);
			this.addressTextBox.DataBindings.Add("Text", base.ContentPage.BindingSource, "Extension");
			this.addressTextBox.TextChanged += delegate(object param0, EventArgs param1)
			{
				base.UpdateError();
			};
			this.dialplanPickerLauncherTextBox.Picker = new AutomatedObjectPicker("DialPlanConfigurable");
			this.dialplanPickerLauncherTextBox.ValueMember = "PhoneContext";
			this.dialplanPickerLauncherTextBox.DataBindings.Add("SelectedValue", base.ContentPage.BindingSource, "PhoneContext", true, DataSourceUpdateMode.OnPropertyChanged);
			this.dialplanPickerLauncherTextBox.SelectedValueChanged += delegate(object param0, EventArgs param1)
			{
				base.UpdateError();
			};
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			base.ContentPage.InputValidationProvider.SetUIValidationEnabled(base.ContentPage, true);
		}

		protected override UIValidationError[] GetValidationErrors()
		{
			List<UIValidationError> list = new List<UIValidationError>(UIValidationError.None);
			if (this.addressTextBox.TextLength + this.dialplanPickerLauncherTextBox.SelectedValue.ToString().Length > ProxyAddressBase.MaxLength - EumProxyAddressDataHandlerSchema.FixedLength)
			{
				list.Add(new UIValidationError(Strings.ExceedMaxLimit, this.addressTextBox));
			}
			if (string.IsNullOrEmpty(this.dialplanPickerLauncherTextBox.Text))
			{
				list.Add(new UIValidationError(Strings.SelectValueErrorMessage, this.dialplanPickerLauncherTextBox));
			}
			return list.ToArray();
		}

		protected override ProxyAddressBaseDataHandler DataHandler
		{
			get
			{
				if (this.dataHandler == null)
				{
					this.dataHandler = new EumProxyAddressDataHandler();
				}
				return this.dataHandler;
			}
		}

		private ProxyAddressDataHandler dataHandler;
	}
}
