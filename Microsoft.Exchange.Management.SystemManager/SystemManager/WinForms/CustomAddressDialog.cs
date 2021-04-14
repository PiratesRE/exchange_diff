using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract partial class CustomAddressDialog : ProxyAddressDialog
	{
		public CustomAddressDialog()
		{
			this.InitializeComponent();
			this.Text = Strings.CustomAddress;
			this.addressLabel.Text = Strings.ProxyAddressLabel;
			this.prefixLabel.Text = Strings.ProxyAddressPrefixLabel;
			this.addressTextBox.DataBindings.Add("Text", base.ContentPage.BindingSource, "Address");
			this.prefixTextBox.DataBindings.Add("Text", base.ContentPage.BindingSource, "Prefix");
			this.addressTextBox.TextChanged += delegate(object param0, EventArgs param1)
			{
				int val = ProxyAddressBase.MaxLength - 1 - ((this.addressTextBox.Text.Length == 0) ? 1 : this.addressTextBox.Text.Length);
				this.prefixTextBox.MaxLength = Math.Min(9, val);
			};
			this.prefixTextBox.TextChanged += delegate(object param0, EventArgs param1)
			{
				this.addressTextBox.MaxLength = ProxyAddressBase.MaxLength - 1 - ((this.prefixTextBox.Text.Length == 0) ? 1 : this.prefixTextBox.Text.Length);
			};
		}

		protected override void OnShown(EventArgs e)
		{
			this.addressTextBox.SelectAll();
			base.OnShown(e);
		}

		[DefaultValue(false)]
		public bool IsPrefixTextBoxReadOnly
		{
			get
			{
				return this.prefixTextBox.ReadOnly;
			}
			set
			{
				this.prefixTextBox.ReadOnly = value;
			}
		}
	}
}
