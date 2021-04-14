using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class ComboBoxPickerDialog : ExchangeDialog
	{
		public ComboBoxPickerDialog()
		{
			this.InitializeComponent();
		}

		[DefaultValue("")]
		public string Label
		{
			get
			{
				return this.label.Text;
			}
			set
			{
				this.label.Text = value;
			}
		}

		[DefaultValue(null)]
		public object DataSource
		{
			get
			{
				return this.comboBox.DataSource;
			}
			set
			{
				this.comboBox.DataSource = value;
			}
		}

		[DefaultValue("")]
		public string DisplayMember
		{
			get
			{
				return this.comboBox.DisplayMember;
			}
			set
			{
				this.comboBox.DisplayMember = value;
			}
		}

		[DefaultValue("")]
		public string ValueMember
		{
			get
			{
				return this.comboBox.ValueMember;
			}
			set
			{
				this.comboBox.ValueMember = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public object SelectedValue
		{
			get
			{
				return this.comboBox.SelectedValue;
			}
			set
			{
				this.comboBox.SelectedValue = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int SelectedIndex
		{
			get
			{
				return this.comboBox.SelectedIndex;
			}
			set
			{
				this.comboBox.SelectedIndex = value;
			}
		}
	}
}
