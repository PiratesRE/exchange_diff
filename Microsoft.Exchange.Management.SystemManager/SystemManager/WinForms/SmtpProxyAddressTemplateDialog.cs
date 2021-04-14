using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class SmtpProxyAddressTemplateDialog : ProxyAddressDialog
	{
		public SmtpProxyAddressTemplateDialog()
		{
			this.InitializeComponent();
			this.Text = Strings.SmtpEmailAddressCaption;
			this.localPartCheckBox.Text = Strings.EmailAddressLocalPart;
			this.aliasRadioButton.Text = Strings.UseAlias;
			this.firstNameLastNameRadioButton.Text = Strings.FirstNameLastName;
			this.firstNameInitialLastNameRadioButton.Text = Strings.FirstNameInitialLastName;
			this.firstNameLastNameInitialRadioButton.Text = Strings.FirstNameLastNameInitial;
			this.lastNameFirstNameRadioButton.Text = Strings.LastNameFirstName;
			this.lastNameInitialFirstNameRadioButton.Text = Strings.LastNameInitialFirstName;
			this.lastNameFirstNameInitialRadioButton.Text = Strings.LastNameFirstNameInitial;
			this.acceptedDomainRadioButton.Text = Strings.AcceptedDomainRadioButtonText;
			this.customDomainRadioButton.Text = Strings.CustomDomainRadioButtonText;
			this.allOptionRadioButtons = new AutoHeightRadioButton[]
			{
				this.aliasRadioButton,
				this.firstNameLastNameRadioButton,
				this.firstNameInitialLastNameRadioButton,
				this.firstNameLastNameInitialRadioButton,
				this.lastNameFirstNameRadioButton,
				this.lastNameInitialFirstNameRadioButton,
				this.lastNameFirstNameInitialRadioButton
			};
			this.currentOption = this.aliasRadioButton;
			this.currentOption.Checked = true;
			foreach (AutoHeightRadioButton autoHeightRadioButton in this.allOptionRadioButtons)
			{
				autoHeightRadioButton.CheckedChanged += this.option_CheckedChanged;
			}
			this.localPartCheckBox.Checked = true;
			this.localPartCheckBox.CheckedChanged += this.localPartCheckBox_CheckedChanged;
			this.acceptedDomainPickerLauncherTextBox.Picker = new AutomatedObjectPicker(new AcceptedDomainConfigurable());
			((AutomatedObjectPicker)this.acceptedDomainPickerLauncherTextBox.Picker).InputValue("ExcludeExternalRelay", true);
			this.acceptedDomainPickerLauncherTextBox.ValueMember = "DomainName";
			this.acceptedDomainPickerLauncherTextBox.ValueMemberConverter = new SmtpDomainWithSubdomainsToDomainNameConverter();
			this.acceptedDomainPickerLauncherTextBox.SelectedValueChanged += delegate(object param0, EventArgs param1)
			{
				base.UpdateError();
				this.MakeDirty();
			};
			this.acceptedDomainPickerLauncherTextBox.DataBindings.Add("Enabled", this.acceptedDomainRadioButton, "Checked", true, DataSourceUpdateMode.OnPropertyChanged);
			this.customDomainTextBox.DataBindings.Add("Enabled", this.customDomainRadioButton, "Checked", true, DataSourceUpdateMode.OnPropertyChanged);
			this.customDomainTextBox.TextChanged += delegate(object param0, EventArgs param1)
			{
				base.UpdateError();
				this.MakeDirty();
			};
			this.customDomainRadioButton.CheckedChanged += delegate(object param0, EventArgs param1)
			{
				this.clearingError = true;
				base.UpdateError();
				this.MakeDirty();
				this.clearingError = false;
			};
			base.DataBindings.Add("TemplateString", base.ContentPage.BindingSource, "Address");
		}

		[DefaultValue("")]
		public string Domain
		{
			get
			{
				if (!this.acceptedDomainRadioButton.Checked)
				{
					return this.customDomainTextBox.Text;
				}
				return this.acceptedDomainPickerLauncherTextBox.Text;
			}
			set
			{
				if (this.acceptedDomainRadioButton.Checked)
				{
					this.acceptedDomainPickerLauncherTextBox.Text = value;
					return;
				}
				this.customDomainTextBox.Text = value;
			}
		}

		[DefaultValue("")]
		public string TemplateString
		{
			get
			{
				return this.templateString;
			}
			set
			{
				if (this.templateString != value)
				{
					this.templateString = value;
					this.UpdateControl();
				}
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (string.IsNullOrEmpty(this.TemplateString))
			{
				this.MakeDirty();
			}
			base.OnClosing(e);
		}

		protected void UpdateControl()
		{
			if (!string.IsNullOrEmpty(this.templateString))
			{
				this.updatingControl = true;
				string text = this.templateString;
				int num = text.IndexOf('@');
				if (num < 0)
				{
					this.Domain = text;
					this.localPartCheckBox.Checked = false;
				}
				else
				{
					this.Domain = text.Substring(num + 1);
					int i;
					for (i = 0; i < this.localParts.Length; i++)
					{
						if (this.localParts[i].Length < text.Length && text.StartsWith(this.localParts[i], StringComparison.InvariantCultureIgnoreCase) && text[this.localParts[i].Length] == '@')
						{
							this.localPartCheckBox.Checked = true;
							this.allOptionRadioButtons[i].Checked = true;
							break;
						}
					}
					if (i == this.localParts.Length)
					{
						this.localPartCheckBox.Checked = false;
						this.customLocalPart = text.Substring(0, num);
					}
				}
				this.updatingControl = false;
			}
		}

		protected void MakeDirty()
		{
			if (!this.updatingControl)
			{
				this.TemplateString = (this.localPartCheckBox.Checked ? this.localParts[this.RadioButtonIndex(this.currentOption)] : this.customLocalPart) + "@" + this.Domain;
			}
		}

		private void option_CheckedChanged(object sender, EventArgs e)
		{
			AutoHeightRadioButton autoHeightRadioButton = (AutoHeightRadioButton)sender;
			if (autoHeightRadioButton.Checked)
			{
				this.currentOption = autoHeightRadioButton;
				this.MakeDirty();
			}
		}

		private int RadioButtonIndex(AutoHeightRadioButton ro)
		{
			return Array.IndexOf<AutoHeightRadioButton>(this.allOptionRadioButtons, ro);
		}

		private void localPartCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			foreach (AutoHeightRadioButton autoHeightRadioButton in this.allOptionRadioButtons)
			{
				autoHeightRadioButton.Enabled = checkBox.Checked;
			}
			this.MakeDirty();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			base.ContentPage.InputValidationProvider.SetUIValidationEnabled(base.ContentPage, true);
		}

		protected override UIValidationError[] GetValidationErrors()
		{
			UIValidationError uivalidationError = null;
			if (!this.clearingError)
			{
				if (this.customDomainRadioButton.Checked)
				{
					if (string.IsNullOrEmpty(this.customDomainTextBox.Text))
					{
						uivalidationError = new UIValidationError(Strings.ErrorDomainPartCannotBeEmpty, this.customDomainTextBox);
						goto IL_8F;
					}
					try
					{
						SmtpDomain.Parse(this.customDomainTextBox.Text);
						goto IL_8F;
					}
					catch (FormatException ex)
					{
						uivalidationError = new UIValidationError(new LocalizedString(ex.Message), this.customDomainTextBox);
						goto IL_8F;
					}
				}
				if (string.IsNullOrEmpty(this.acceptedDomainPickerLauncherTextBox.Text))
				{
					uivalidationError = new UIValidationError(Strings.ErrorDomainPartCannotBeEmpty, this.acceptedDomainPickerLauncherTextBox);
				}
			}
			IL_8F:
			if (uivalidationError != null)
			{
				return new UIValidationError[]
				{
					uivalidationError
				};
			}
			return UIValidationError.None;
		}

		protected override ProxyAddressBaseDataHandler DataHandler
		{
			get
			{
				if (this.dataHandler == null)
				{
					this.dataHandler = new ProxyAddressTemplateDataHandler();
					this.dataHandler.Prefix = ProxyAddressPrefix.Smtp.ToString();
					try
					{
						this.dataHandler.Address = "%m@" + NativeHelpers.GetDomainName();
					}
					catch (CannotGetDomainInfoException)
					{
					}
				}
				return this.dataHandler;
			}
		}

		private AutoHeightRadioButton[] allOptionRadioButtons;

		private AutoHeightRadioButton currentOption;

		private string[] localParts = new string[]
		{
			"%m",
			"%g.%s",
			"%1g%s",
			"%g%1s",
			"%s.%g",
			"%1s%g",
			"%s%1g"
		};

		private string customLocalPart = string.Empty;

		private bool updatingControl;

		private bool clearingError;

		private string templateString = string.Empty;

		private ProxyAddressTemplateDataHandler dataHandler;
	}
}
