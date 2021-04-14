using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class GeneralPageHeader : ExchangeUserControl, IFormatModeProvider, IBindableComponent, IComponent, IDisposable
	{
		public GeneralPageHeader()
		{
			this.InitializeComponent();
			this.displayNameTextBoxBorderStyle = this.displayNameTextBox.BorderStyle;
			this.displayNameTextBoxReadOnly = this.displayNameTextBox.ReadOnly;
			this.displayNameTextBox.TextChanged += delegate(object param0, EventArgs param1)
			{
				this.OnTextChanged(EventArgs.Empty);
			};
			this.displayNameTextBox.FormatModeChanged += delegate(object param0, EventArgs param1)
			{
				this.OnFormatModeChanged(EventArgs.Empty);
			};
			new TextBoxConstraintProvider(this, this.displayNameTextBox);
		}

		[DefaultValue(0)]
		public DisplayFormatMode FormatMode
		{
			get
			{
				return this.displayNameTextBox.FormatMode;
			}
			set
			{
				this.displayNameTextBox.FormatMode = value;
			}
		}

		protected virtual void OnFormatModeChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[GeneralPageHeader.EventFormatModeChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler FormatModeChanged
		{
			add
			{
				base.Events.AddHandler(GeneralPageHeader.EventFormatModeChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(GeneralPageHeader.EventFormatModeChanged, value);
			}
		}

		private void InitializeComponent()
		{
			this.headerNamePanel = new TableLayoutPanel();
			this.objectPictureBox = new ExchangePictureBox();
			this.displayNameTextBox = new ExchangeTextBox();
			this.headerNamePanel.SuspendLayout();
			((ISupportInitialize)this.objectPictureBox).BeginInit();
			base.SuspendLayout();
			this.headerNamePanel.AutoSize = true;
			this.headerNamePanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.headerNamePanel.ColumnCount = 2;
			this.headerNamePanel.ColumnStyles.Add(new ColumnStyle());
			this.headerNamePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.headerNamePanel.Controls.Add(this.objectPictureBox, 0, 0);
			this.headerNamePanel.Controls.Add(this.displayNameTextBox, 1, 0);
			this.headerNamePanel.Dock = DockStyle.Fill;
			this.headerNamePanel.Location = new Point(0, 0);
			this.headerNamePanel.Margin = new Padding(0);
			this.headerNamePanel.Name = "headerNamePanel";
			this.headerNamePanel.RowCount = 1;
			this.headerNamePanel.RowStyles.Add(new RowStyle());
			this.headerNamePanel.Size = new Size(386, 34);
			this.headerNamePanel.TabIndex = 0;
			this.objectPictureBox.Location = new Point(0, 0);
			this.objectPictureBox.Margin = new Padding(0, 0, 0, 2);
			this.objectPictureBox.Name = "objectPictureBox";
			this.objectPictureBox.Size = new Size(32, 32);
			this.objectPictureBox.TabIndex = 1;
			this.objectPictureBox.TabStop = false;
			this.displayNameTextBox.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
			this.displayNameTextBox.Location = new Point(44, 7);
			this.displayNameTextBox.Margin = new Padding(12, 0, 0, 0);
			this.displayNameTextBox.Name = "displayNameTextBox";
			this.displayNameTextBox.Size = new Size(342, 20);
			this.displayNameTextBox.TabIndex = 2;
			base.Controls.Add(this.headerNamePanel);
			this.Margin = new Padding(0);
			base.Name = "GeneralPageHeader";
			base.Size = new Size(386, 34);
			this.headerNamePanel.ResumeLayout(false);
			this.headerNamePanel.PerformLayout();
			((ISupportInitialize)this.objectPictureBox).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public override string Text
		{
			get
			{
				return this.displayNameTextBox.Text;
			}
			set
			{
				this.displayNameTextBox.Text = value;
			}
		}

		[DefaultValue(null)]
		public Icon Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (this.Icon != value)
				{
					Bitmap image = IconLibrary.ToBitmap(value, this.objectPictureBox.Size);
					if (this.objectPictureBox.Image != null)
					{
						this.objectPictureBox.Image.Dispose();
					}
					this.objectPictureBox.Image = image;
					this.icon = value;
				}
			}
		}

		[DefaultValue(true)]
		public bool CanChangeHeaderText
		{
			get
			{
				return this.canChangeHeaderText;
			}
			set
			{
				if (value != this.CanChangeHeaderText)
				{
					this.canChangeHeaderText = value;
					this.displayNameTextBoxReadOnly = !value;
					this.displayNameTextBoxBorderStyle = (value ? BorderStyle.Fixed3D : BorderStyle.None);
					this.RefreshDisplayNameTextBoxStyle();
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Padding Margin
		{
			get
			{
				return base.Margin;
			}
			set
			{
				base.Margin = value;
			}
		}

		private void RefreshDisplayNameTextBoxStyle()
		{
			if (base.IsHandleCreated)
			{
				this.displayNameTextBox.ReadOnly = this.displayNameTextBoxReadOnly;
				this.displayNameTextBox.BorderStyle = this.displayNameTextBoxBorderStyle;
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			this.RefreshDisplayNameTextBoxStyle();
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "Text";
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

		private TableLayoutPanel headerNamePanel;

		private ExchangeTextBox displayNameTextBox;

		private ExchangePictureBox objectPictureBox;

		private bool canChangeHeaderText = true;

		private static readonly object EventFormatModeChanged = new object();

		private Icon icon;

		private BorderStyle displayNameTextBoxBorderStyle;

		private bool displayNameTextBoxReadOnly;
	}
}
