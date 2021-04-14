using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class WizardOrientationPanel : Panel
	{
		public WizardOrientationPanel()
		{
			this.InitializeComponent();
			this.DoubleBuffered = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Wizard = null;
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel = new TableLayoutPanel();
			base.SuspendLayout();
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Size = new Size(156, 0);
			this.tableLayoutPanel.TabIndex = 0;
			base.Controls.Add(this.tableLayoutPanel);
			base.ResumeLayout();
		}

		[DefaultValue(null)]
		public Wizard Wizard
		{
			get
			{
				return this.wizard;
			}
			set
			{
				if (this.Wizard != value)
				{
					if (this.Wizard != null)
					{
						this.Wizard.WizardPageAdded -= this.wizard_WizardPageAdded;
						this.Wizard.WizardPageRemoved -= this.wizard_WizardPageRemoved;
						this.Wizard.WizardPageMoved -= new ControlMovedEventHandler(this.wizard_WizardPageMoved);
						this.Wizard.CurrentPageChanged -= this.wizard_CurrentPageChanged;
						this.tableLayoutPanel.SuspendLayout();
						try
						{
							for (int i = this.tableLayoutPanel.RowCount - 1; i >= 0; i--)
							{
								Control controlFromPosition = this.tableLayoutPanel.GetControlFromPosition(0, i);
								if (controlFromPosition != null)
								{
									this.tableLayoutPanel.Controls.Remove(controlFromPosition);
									controlFromPosition.Dispose();
								}
							}
							this.tableLayoutPanel.RowStyles.Clear();
							this.tableLayoutPanel.RowCount = 0;
						}
						finally
						{
							this.tableLayoutPanel.ResumeLayout(false);
						}
					}
					this.wizard = value;
					if (this.Wizard != null)
					{
						this.Wizard.WizardPageAdded += this.wizard_WizardPageAdded;
						this.Wizard.WizardPageRemoved += this.wizard_WizardPageRemoved;
						this.Wizard.WizardPageMoved += new ControlMovedEventHandler(this.wizard_WizardPageMoved);
						this.Wizard.CurrentPageChanged += this.wizard_CurrentPageChanged;
						this.tableLayoutPanel.SuspendLayout();
						try
						{
							foreach (object obj in this.Wizard.WizardPages)
							{
								WizardPage control = (WizardPage)obj;
								this.wizard_WizardPageAdded(this.Wizard, new ControlEventArgs(control));
							}
						}
						finally
						{
							this.tableLayoutPanel.ResumeLayout(false);
						}
						this.ActiveRow = this.Wizard.CurrentPageIndex;
					}
				}
			}
		}

		private void wizard_CurrentPageChanged(object sender, EventArgs e)
		{
			this.ActiveRow = this.Wizard.CurrentPageIndex;
		}

		private void wizard_WizardPageRemoved(object sender, ControlEventArgs e)
		{
			WizardPage wizardPage = (WizardPage)e.Control;
			wizardPage.TextChanged -= this.wizardPage_TextChanged;
			Control labelControl = this.tableLayoutPanel.Controls[wizardPage.Name + "Label"];
			this.RemovePageLabelControl(labelControl);
			this.ActiveRow = this.Wizard.CurrentPageIndex;
		}

		private void wizard_WizardPageAdded(object sender, ControlEventArgs e)
		{
			WizardPage wizardPage = (WizardPage)e.Control;
			wizardPage.TextChanged += this.wizardPage_TextChanged;
			ExchangeLinkLabel exchangeLinkLabel = new ExchangeLinkLabel();
			exchangeLinkLabel.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
			exchangeLinkLabel.AutoEllipsis = true;
			exchangeLinkLabel.AutoSize = true;
			exchangeLinkLabel.ImageAlign = ContentAlignment.MiddleLeft;
			exchangeLinkLabel.Image = WizardOrientationPanel.upcomingPageImage;
			exchangeLinkLabel.LinkArea = new LinkArea(0, 0);
			exchangeLinkLabel.MaximumSize = new Size(0, exchangeLinkLabel.Font.Height * 3 + 4 + 4);
			exchangeLinkLabel.Name = wizardPage.Name + "Label";
			exchangeLinkLabel.Padding = new Padding(20, 0, 0, 0);
			exchangeLinkLabel.Size = new Size(168, 26);
			exchangeLinkLabel.TabIndex = 4;
			exchangeLinkLabel.Text = wizardPage.Text;
			exchangeLinkLabel.TextAlign = ContentAlignment.MiddleLeft;
			exchangeLinkLabel.UseMnemonic = false;
			int index = this.Wizard.WizardPages.IndexOf(wizardPage);
			int num = 0;
			WizardPage wizardPage2 = wizardPage;
			while (wizardPage2.ParentPage != null && wizardPage2.ParentPage != null)
			{
				num++;
				wizardPage2 = wizardPage2.ParentPage;
			}
			exchangeLinkLabel.Padding = new Padding(WizardOrientationPanel.visitedPageImage.Width + 4, 0, 0, 0);
			exchangeLinkLabel.Margin = new Padding(4 + exchangeLinkLabel.Padding.Left * num, 4, 4, 4);
			this.InsertPageLabelControl(exchangeLinkLabel, index);
			this.ActiveRow = this.Wizard.CurrentPageIndex;
		}

		private void wizard_WizardPageMoved(object sender, ControlMovedEventArgs e)
		{
			WizardPage wizardPage = (WizardPage)e.Control;
			Control control = this.tableLayoutPanel.Controls[wizardPage.Name + "Label"];
			if (control != null)
			{
				this.tableLayoutPanel.SuspendLayout();
				try
				{
					this.RemovePageLabelControl(control);
					this.InsertPageLabelControl(control, e.NewIndex);
				}
				finally
				{
					this.tableLayoutPanel.ResumeLayout();
				}
			}
		}

		private void InsertPageLabelControl(Control labelControl, int index)
		{
			this.tableLayoutPanel.SuspendLayout();
			try
			{
				this.tableLayoutPanel.RowCount++;
				this.tableLayoutPanel.RowStyles.Insert(index, new RowStyle());
				for (int i = this.tableLayoutPanel.RowCount - 2; i >= index; i--)
				{
					Control controlFromPosition = this.tableLayoutPanel.GetControlFromPosition(0, i);
					if (controlFromPosition != null)
					{
						this.tableLayoutPanel.SetRow(controlFromPosition, i + 1);
					}
				}
				this.tableLayoutPanel.Controls.Add(labelControl, 0, index);
			}
			finally
			{
				this.tableLayoutPanel.ResumeLayout();
			}
		}

		private void RemovePageLabelControl(Control labelControl)
		{
			int row = this.tableLayoutPanel.GetRow(labelControl);
			this.tableLayoutPanel.SuspendLayout();
			try
			{
				this.tableLayoutPanel.Controls.Remove(labelControl);
				for (int i = row + 1; i < this.tableLayoutPanel.RowCount; i++)
				{
					Control controlFromPosition = this.tableLayoutPanel.GetControlFromPosition(0, i);
					if (controlFromPosition != null)
					{
						this.tableLayoutPanel.SetRow(controlFromPosition, i - 1);
					}
				}
				this.tableLayoutPanel.RowStyles.RemoveAt(this.tableLayoutPanel.RowCount - 1);
				this.tableLayoutPanel.RowCount--;
			}
			finally
			{
				this.tableLayoutPanel.ResumeLayout();
			}
		}

		private void wizardPage_TextChanged(object sender, EventArgs e)
		{
			WizardPage wizardPage = (WizardPage)sender;
			int childIndex = this.Wizard.WizardPages.GetChildIndex(wizardPage);
			Control controlFromPosition = this.tableLayoutPanel.GetControlFromPosition(0, childIndex);
			controlFromPosition.Text = wizardPage.Text;
		}

		[DefaultValue(-1)]
		protected int ActiveRow
		{
			get
			{
				return this.activeRow;
			}
			set
			{
				this.activeRow = value;
				for (int i = 0; i < this.tableLayoutPanel.RowCount; i++)
				{
					ExchangeLinkLabel exchangeLinkLabel = (ExchangeLinkLabel)this.tableLayoutPanel.GetControlFromPosition(0, i);
					if (i < this.activeRow)
					{
						exchangeLinkLabel.Image = WizardOrientationPanel.visitedPageImage;
					}
					else if (i == this.activeRow)
					{
						if (this.ActiveRow == this.tableLayoutPanel.RowCount - 1)
						{
							exchangeLinkLabel.Image = WizardOrientationPanel.visitedPageImage;
						}
						else
						{
							exchangeLinkLabel.Image = WizardOrientationPanel.currentPageImage;
						}
					}
					else
					{
						exchangeLinkLabel.Image = WizardOrientationPanel.upcomingPageImage;
					}
				}
			}
		}

		private const int DefaultLineHorizontalPadding = 4;

		private const int DefaultLineTopPadding = 4;

		private const int DefaultLineBottomPadding = 4;

		private const int LineIndentPadding = 12;

		private TableLayoutPanel tableLayoutPanel;

		private static readonly Bitmap visitedPageImage = Icons.WizardVisitedPage;

		private static readonly Bitmap currentPageImage = Icons.WizardCurrentPage;

		private static readonly Bitmap upcomingPageImage = Icons.WizardUpcomingPage;

		private Wizard wizard;

		private int activeRow = -1;
	}
}
