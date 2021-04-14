using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CaptionedTextBox : ExchangeUserControl, IFormatModeProvider, IBindableComponent, IComponent, IDisposable
	{
		public CaptionedTextBox()
		{
			this.InitializeComponent();
			this.exchangeTextBox.TextChanged += delegate(object param0, EventArgs param1)
			{
				this.OnTextChanged(EventArgs.Empty);
			};
			this.exchangeTextBox.FormatModeChanged += delegate(object param0, EventArgs param1)
			{
				this.OnFormatModeChanged(EventArgs.Empty);
			};
			new TextBoxConstraintProvider(this, this.exchangeTextBox);
		}

		[DefaultValue(0)]
		public DisplayFormatMode FormatMode
		{
			get
			{
				return this.exchangeTextBox.FormatMode;
			}
			set
			{
				this.exchangeTextBox.FormatMode = value;
			}
		}

		protected virtual void OnFormatModeChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[CaptionedTextBox.EventFormatModeChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler FormatModeChanged
		{
			add
			{
				base.Events.AddHandler(CaptionedTextBox.EventFormatModeChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(CaptionedTextBox.EventFormatModeChanged, value);
			}
		}

		private void InitializeComponent()
		{
			this.exchangeTextBox = new ExchangeTextBox();
			this.tableLayoutPanel = new TableLayoutPanel();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.exchangeTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.exchangeTextBox.Location = new Point(8, 0);
			this.exchangeTextBox.Margin = new Padding(8, 0, 0, 0);
			this.exchangeTextBox.Name = "exchangeTextBox";
			this.exchangeTextBox.Size = new Size(75, 20);
			this.exchangeTextBox.TabIndex = 1;
			this.tableLayoutPanel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel.Controls.Add(this.exchangeTextBox, 1, 0);
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(83, 20);
			this.tableLayoutPanel.TabIndex = 0;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "CaptionedTextBox";
			base.Size = new Size(83, 20);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override Padding DefaultMargin
		{
			get
			{
				return new Padding(0);
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(366, 20);
			}
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			if (this.tableLayoutPanel.Width > proposedSize.Width)
			{
				proposedSize.Width = this.tableLayoutPanel.Width;
			}
			if (proposedSize != this.lastProposedSize || this.lastTLPSize != this.tableLayoutPanel.Size)
			{
				this.preferredSizeCache = this.tableLayoutPanel.GetPreferredSize(proposedSize);
				this.lastProposedSize = proposedSize;
				this.lastTLPSize = this.tableLayoutPanel.Size;
				this.preferredSizeCache.Width = this.preferredSizeCache.Width + base.Padding.Horizontal;
				this.preferredSizeCache.Height = this.preferredSizeCache.Height + base.Padding.Vertical;
			}
			return this.preferredSizeCache;
		}

		public override string Text
		{
			get
			{
				return this.exchangeTextBox.Text;
			}
			set
			{
				this.exchangeTextBox.Text = value;
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public HorizontalAlignment TextAlign
		{
			get
			{
				return this.exchangeTextBox.TextAlign;
			}
			set
			{
				this.exchangeTextBox.TextAlign = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		private bool ShouldSerializeTextAlign()
		{
			return this.TextAlign != HorizontalAlignment.Left;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetTextAlign()
		{
			this.TextAlign = HorizontalAlignment.Left;
		}

		[DefaultValue(75)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public int TextBoxWidth
		{
			get
			{
				return this.exchangeTextBox.Width;
			}
			set
			{
				this.exchangeTextBox.Width = value;
			}
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

		private Size lastProposedSize = Size.Empty;

		private Size lastTLPSize = Size.Empty;

		private Size preferredSizeCache = Size.Empty;

		private static readonly object EventFormatModeChanged = new object();

		protected ExchangeTextBox exchangeTextBox;

		protected TableLayoutPanel tableLayoutPanel;
	}
}
