using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Setup.Bootstrapper.Common;

namespace Microsoft.Exchange.Bootstrapper.Setup
{
	public partial class DialogBoxForm : Form
	{
		public DialogBoxForm(string localizedString)
		{
			this.InitializeComponent();
			this.okButton.Text = Strings.ButtonTexkOk;
			this.Text = Strings.MessageHeaderText;
			this.statusText.Text = localizedString;
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			base.Close();
		}
	}
}
