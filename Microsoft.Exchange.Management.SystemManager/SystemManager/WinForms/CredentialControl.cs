using System;
using System.ComponentModel;
using System.Drawing;
using System.Management.Automation;
using System.Security;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CredentialControl : StrongTypeEditor<PSCredential>
	{
		[DefaultValue(true)]
		public new bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
			}
		}

		[DefaultValue(AutoSizeMode.GrowAndShrink)]
		public new AutoSizeMode AutoSizeMode
		{
			get
			{
				return base.AutoSizeMode;
			}
			set
			{
				base.AutoSizeMode = value;
			}
		}

		private bool ShouldSerializeAccountNameText()
		{
			return !string.Equals(this.AccountNameText, Strings.UserNameText);
		}

		private bool ShouldSerializePasswordText()
		{
			return !string.Equals(this.PasswordText, Strings.PasswordText);
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			proposedSize.Width = base.Width;
			return this.tableLayoutPanel.GetPreferredSize(proposedSize);
		}

		public CredentialControl()
		{
			this.InitializeComponent();
			this.userNameLabel.Text = Strings.UserNameText;
			this.passwordLabel.Text = Strings.PasswordText;
			this.passwordExchangeTextBox.UseSystemPasswordChar = true;
			this.confirmPasswordLabel.Text = Strings.ConfirmationPasswordLabel;
			this.confirmPasswordTextBox.UseSystemPasswordChar = true;
			base.BindingSource.DataSource = typeof(PSCredential);
			this.userNameExchangeTextBox.DataBindings.Add("Text", base.BindingSource, "userName", true, DataSourceUpdateMode.OnValidation);
			this.passwordExchangeTextBox.DataBindings.Add("Text", base.BindingSource, "password", true, DataSourceUpdateMode.OnValidation);
			base.Validator = new CredentialControl.CredentialControlDataHandler(this);
		}

		public string AccountNameText
		{
			get
			{
				return this.userNameLabel.Text;
			}
			set
			{
				this.userNameLabel.Text = value;
			}
		}

		public string PasswordText
		{
			get
			{
				return this.passwordLabel.Text;
			}
			set
			{
				this.passwordLabel.Text = value;
			}
		}

		[DefaultValue(false)]
		public bool AllowPasswordConfirmation
		{
			get
			{
				return this.allowPasswordConfirmation;
			}
			set
			{
				this.allowPasswordConfirmation = value;
				if (!value)
				{
					base.SuspendLayout();
					this.confirmPasswordLabel.Hide();
					this.confirmPasswordTextBox.Hide();
					base.ResumeLayout();
					base.PerformLayout();
				}
			}
		}

		protected override UIValidationError[] GetValidationErrors()
		{
			if (base.Enabled && this.AllowPasswordConfirmation && this.passwordExchangeTextBox.Text != this.confirmPasswordTextBox.Text)
			{
				return new UIValidationError[]
				{
					new UIValidationError(Strings.InvalidPasswordError, this.confirmPasswordTextBox)
				};
			}
			return UIValidationError.None;
		}

		private void InitializeComponent()
		{
			this.passwordLabel = new Label();
			this.userNameLabel = new Label();
			this.passwordExchangeTextBox = new ExchangeTextBox();
			this.userNameExchangeTextBox = new ExchangeTextBox();
			this.confirmPasswordLabel = new Label();
			this.confirmPasswordTextBox = new ExchangeTextBox();
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.passwordLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.passwordLabel.AutoSize = true;
			this.passwordLabel.Location = new Point(0, 48);
			this.passwordLabel.Margin = new Padding(0, 12, 0, 0);
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.Size = new Size(182, 13);
			this.passwordLabel.TabIndex = 2;
			this.passwordLabel.Text = "passwordLabel";
			this.userNameLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.userNameLabel.AutoSize = true;
			this.userNameLabel.Location = new Point(0, 0);
			this.userNameLabel.Margin = new Padding(0);
			this.userNameLabel.Name = "userNameLabel";
			this.userNameLabel.Size = new Size(182, 13);
			this.userNameLabel.TabIndex = 0;
			this.userNameLabel.Text = "userNameLabel";
			this.passwordExchangeTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.passwordExchangeTextBox.Location = new Point(3, 64);
			this.passwordExchangeTextBox.Margin = new Padding(3, 3, 0, 0);
			this.passwordExchangeTextBox.Name = "passwordExchangeTextBox";
			this.passwordExchangeTextBox.Size = new Size(179, 20);
			this.passwordExchangeTextBox.TabIndex = 3;
			this.passwordExchangeTextBox.UseSystemPasswordChar = true;
			this.userNameExchangeTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.userNameExchangeTextBox.Location = new Point(3, 16);
			this.userNameExchangeTextBox.Margin = new Padding(3, 3, 0, 0);
			this.userNameExchangeTextBox.Name = "userNameExchangeTextBox";
			this.userNameExchangeTextBox.Size = new Size(179, 20);
			this.userNameExchangeTextBox.TabIndex = 1;
			this.confirmPasswordLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.confirmPasswordLabel.AutoSize = true;
			this.confirmPasswordLabel.Location = new Point(0, 96);
			this.confirmPasswordLabel.Margin = new Padding(0, 12, 0, 0);
			this.confirmPasswordLabel.Name = "confirmPasswordLabel";
			this.confirmPasswordLabel.Size = new Size(182, 13);
			this.confirmPasswordLabel.TabIndex = 4;
			this.confirmPasswordLabel.Text = "confirmPasswordLabel";
			this.confirmPasswordTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.confirmPasswordTextBox.Location = new Point(3, 112);
			this.confirmPasswordTextBox.Margin = new Padding(3, 3, 0, 0);
			this.confirmPasswordTextBox.Name = "confirmPasswordTextBox";
			this.confirmPasswordTextBox.Size = new Size(179, 20);
			this.confirmPasswordTextBox.TabIndex = 5;
			this.confirmPasswordTextBox.UseSystemPasswordChar = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.userNameLabel, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.confirmPasswordTextBox, 0, 5);
			this.tableLayoutPanel.Controls.Add(this.userNameExchangeTextBox, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.confirmPasswordLabel, 0, 4);
			this.tableLayoutPanel.Controls.Add(this.passwordLabel, 0, 2);
			this.tableLayoutPanel.Controls.Add(this.passwordExchangeTextBox, 0, 3);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Padding = new Padding(0, 0, 16, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 6;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(182, 132);
			this.tableLayoutPanel.TabIndex = 6;
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "CredentialControl";
			base.Size = new Size(182, 197);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private bool allowPasswordConfirmation;

		private Label passwordLabel;

		private Label userNameLabel;

		private ExchangeTextBox passwordExchangeTextBox;

		private ExchangeTextBox userNameExchangeTextBox;

		private Label confirmPasswordLabel;

		private ExchangeTextBox confirmPasswordTextBox;

		private AutoTableLayoutPanel tableLayoutPanel;

		public class CredentialControlDataHandler : StrongTypeEditorDataHandler<PSCredential>
		{
			public CredentialControlDataHandler(CredentialControl control) : base(control, "PSCredential")
			{
			}

			protected override void UpdateStrongType()
			{
				string text = (!DBNull.Value.Equals(base.Table.Rows[0]["userName"])) ? ((string)base.Table.Rows[0]["userName"]) : string.Empty;
				string password = (!DBNull.Value.Equals(base.Table.Rows[0]["password"])) ? ((string)base.Table.Rows[0]["password"]) : string.Empty;
				if (string.IsNullOrEmpty(text))
				{
					throw new StrongTypeFormatException(Strings.MissingUserNameError, "userName");
				}
				if (string.IsNullOrEmpty(text.Trim()))
				{
					throw new StrongTypeFormatException(Strings.SpaceUserNameError, "userName");
				}
				SecureString password2 = password.AsSecureString();
				base.StrongType = new PSCredential(text, password2);
			}

			protected override void UpdateTable()
			{
				base.Table.Rows[0]["userName"] = ((base.StrongType == null) ? string.Empty : base.StrongType.UserName);
				base.Table.Rows[0]["password"] = ((base.StrongType == null) ? string.Empty : base.StrongType.Password.AsUnsecureString());
			}
		}
	}
}
