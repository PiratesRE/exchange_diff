using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class UserLogonNameControl : ExchangeUserControl, IFormatModeProvider, IBindableComponent, IComponent, IDisposable
	{
		public UserLogonNameControl()
		{
			this.InitializeComponent();
			if (PSConnectionInfoSingleton.GetInstance().Type == OrganizationType.Cloud)
			{
				this.domainComboBoxPicker.Picker = new AutomatedObjectPicker(new AcceptedDomainUPNSuffixesConfigurable());
				this.domainComboBoxPicker.ValueMember = "SmtpDomainToString";
			}
			else
			{
				this.domainComboBoxPicker.Picker = new AutomatedObjectPicker(new UPNSuffixesConfigurable());
				this.domainComboBoxPicker.ValueMember = "CanonicalName";
			}
			this.domainComboBoxPicker.Picker.DataTableLoader.RefreshCompleted += this.DataTableLoader_RefreshCompleted;
			this.domainComboBoxPicker.ValueMemberConverter = new DomainNameConverter();
			this.userNameTextBox.Leave += this.Focus_Leave;
			this.userNameTextBox.TextChanged += this.userNameTextBox_TextChanged;
			this.userNameTextBox.FormatModeChanged += delegate(object param0, EventArgs param1)
			{
				this.OnFormatModeChanged(EventArgs.Empty);
			};
			this.domainComboBoxPicker.Leave += this.Focus_Leave;
			this.domainComboBoxPicker.SelectedValueChanged += this.domainComboBoxPicker_SelectedValueChanged;
			new TextBoxConstraintProvider(this, "UserLogonName", this.userNameTextBox);
		}

		private void DataTableLoader_RefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error == null)
			{
				DataTable table = (sender as DataTableLoader).Table;
				if (PSConnectionInfoSingleton.GetInstance().Type == OrganizationType.Cloud)
				{
					DataRow dataRow = table.Rows.OfType<DataRow>().First((DataRow row) => (bool)row["Default"]);
					if (dataRow != null)
					{
						this.DefaultDomain = dataRow["SmtpDomainToString"].ToString();
					}
				}
				else if (table.Rows.Count > 0)
				{
					this.DefaultDomain = table.Rows[0]["CanonicalName"].ToString();
				}
				if (this.domainComboBoxPicker.SelectedValue == null)
				{
					this.domainComboBoxPicker.SelectedValue = this.DefaultDomain;
				}
			}
		}

		private void domainComboBoxPicker_SelectedValueChanged(object sender, EventArgs e)
		{
			this.OnUserLogonNameChanged(EventArgs.Empty);
		}

		private void userNameTextBox_TextChanged(object sender, EventArgs e)
		{
			this.OnUserLogonNameChanged(EventArgs.Empty);
		}

		private void Focus_Leave(object sender, EventArgs e)
		{
			base.UpdateError();
		}

		private void InitializeComponent()
		{
			this.userNameTextBox = new ExchangeTextBox();
			this.domainComboBoxPicker = new ComboBoxPicker();
			this.tableLayoutPanel = new TableLayoutPanel();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.userNameTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.userNameTextBox.Location = new Point(0, 0);
			this.userNameTextBox.Margin = new Padding(3, 0, 0, 1);
			this.userNameTextBox.Name = "userNameTextBox";
			this.userNameTextBox.Size = new Size(123, 20);
			this.userNameTextBox.TabIndex = 2;
			this.domainComboBoxPicker.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.domainComboBoxPicker.Location = new Point(136, 0);
			this.domainComboBoxPicker.Margin = new Padding(0);
			this.domainComboBoxPicker.Name = "domainComboBoxPicker";
			this.domainComboBoxPicker.Size = new Size(124, 21);
			this.domainComboBoxPicker.TabIndex = 3;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 13f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
			this.tableLayoutPanel.Controls.Add(this.userNameTextBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.domainComboBoxPicker, 2, 0);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Size = new Size(260, 21);
			this.tableLayoutPanel.TabIndex = 4;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "UserLogonNameControl";
			base.Size = new Size(260, 21);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DefaultValue(null)]
		public ADObjectId OrganizationalUnit
		{
			get
			{
				return this.organizationalUnit;
			}
			set
			{
				if (this.OrganizationalUnit != value)
				{
					this.organizationalUnit = value;
					if (OrganizationType.Cloud != PSConnectionInfoSingleton.GetInstance().Type)
					{
						(this.domainComboBoxPicker.Picker as AutomatedObjectPicker).InputValue("OrganizationalUnit", value);
						this.domainComboBoxPicker.Picker.PerformQuery(null, string.Empty);
					}
				}
			}
		}

		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DefaultDomain
		{
			get
			{
				return this.defaultDomain;
			}
			set
			{
				this.defaultDomain = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public string UserLogonName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(this.userNameTextBox.Text);
				if (!string.IsNullOrEmpty(this.userNameTextBox.Text) && this.domainComboBoxPicker.SelectedValue != null)
				{
					stringBuilder.Append("@" + this.domainComboBoxPicker.SelectedValue.ToString());
				}
				return stringBuilder.ToString();
			}
			set
			{
				if (value != this.UserLogonName)
				{
					this.suspendChangeNotification = true;
					int num = (value != null) ? value.LastIndexOf("@") : -1;
					if (num >= 0)
					{
						this.userNameTextBox.Text = value.Substring(0, num);
						string text = value.Substring(num + 1);
						if (!string.IsNullOrEmpty(text))
						{
							(this.domainComboBoxPicker.Picker as AutomatedObjectPicker).InputValue("OtherSuffix", text);
							this.domainComboBoxPicker.Picker.PerformQuery(null, string.Empty);
						}
						this.domainComboBoxPicker.SelectedValue = text;
					}
					else
					{
						this.userNameTextBox.Text = value;
						this.domainComboBoxPicker.SelectedValue = this.DefaultDomain;
					}
					this.suspendChangeNotification = false;
					this.OnUserLogonNameChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnUserLogonNameChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[UserLogonNameControl.EventUserLogonNameChanged];
			if (eventHandler != null && !this.suspendChangeNotification)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler UserLogonNameChanged
		{
			add
			{
				base.Events.AddHandler(UserLogonNameControl.EventUserLogonNameChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(UserLogonNameControl.EventUserLogonNameChanged, value);
			}
		}

		[DefaultValue(0)]
		public DisplayFormatMode FormatMode
		{
			get
			{
				return this.userNameTextBox.FormatMode;
			}
			set
			{
				this.userNameTextBox.FormatMode = value;
			}
		}

		protected virtual void OnFormatModeChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[UserLogonNameControl.EventFormatModeChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler FormatModeChanged
		{
			add
			{
				base.Events.AddHandler(UserLogonNameControl.EventFormatModeChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(UserLogonNameControl.EventFormatModeChanged, value);
			}
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "UserLogonName";
			}
		}

		void IFormatModeProvider.add_BindingContextChanged(EventHandler A_1)
		{
			base.BindingContextChanged += A_1;
		}

		void IFormatModeProvider.remove_BindingContextChanged(EventHandler A_1)
		{
			base.BindingContextChanged -= A_1;
		}

		private ExchangeTextBox userNameTextBox;

		private ComboBoxPicker domainComboBoxPicker;

		private TableLayoutPanel tableLayoutPanel;

		private bool suspendChangeNotification;

		private ADObjectId organizationalUnit;

		private string defaultDomain;

		private static readonly object EventUserLogonNameChanged = new object();

		private static readonly object EventFormatModeChanged = new object();
	}
}
