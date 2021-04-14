using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SystemManager.WinForms.Design;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Designer(typeof(WorkCenterDesigner))]
	public class WorkCenter : ContainerResultPane
	{
		public WorkCenter()
		{
			this.InitializeComponent();
			this.splitContainer.MinimumSize = new Size(0, this.splitContainer.Panel1MinSize + this.splitContainer.Panel2MinSize + this.splitContainer.SplitterWidth);
			this.bottomResultPane.StatusChanged += this.BottomResultPane_StatusChanged;
			this.BottomResultPane_StatusChanged(this.bottomResultPane, EventArgs.Empty);
		}

		public override bool HasPermission()
		{
			return this.TopPanelResultPane.HasPermission();
		}

		private void InitializeComponent()
		{
			this.splitContainer = new SplitContainer();
			this.bottomResultPane = new TabbedResultPane();
			this.bottomResultPane.IsCaptionVisible = false;
			this.separator = new Panel();
			this.bottomPanelCaption = new ResultPaneCaption();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			base.SuspendLayout();
			this.splitContainer.Dock = DockStyle.Fill;
			this.splitContainer.Location = new Point(0, 22);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = Orientation.Horizontal;
			this.splitContainer.Panel1MinSize = 120;
			this.splitContainer.Panel2MinSize = 120;
			this.splitContainer.Panel2.Controls.Add(this.bottomResultPane);
			this.splitContainer.Panel2.Controls.Add(this.separator);
			this.splitContainer.Panel2.Controls.Add(this.bottomPanelCaption);
			this.splitContainer.Size = new Size(400, 378);
			this.splitContainer.SplitterDistance = 143;
			this.splitContainer.TabIndex = 0;
			this.bottomResultPane.Dock = DockStyle.Fill;
			this.bottomResultPane.Location = new Point(0, 27);
			this.bottomResultPane.Name = "workCenter_TabbedResultPane";
			this.bottomResultPane.TabIndex = 0;
			this.separator.Dock = DockStyle.Top;
			this.separator.Location = new Point(0, 22);
			this.separator.Name = "separator";
			this.separator.Size = new Size(400, 5);
			this.separator.TabIndex = 0;
			this.separator.Paint += this.separator_Paint;
			this.bottomPanelCaption.AutoSize = true;
			this.bottomPanelCaption.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.bottomPanelCaption.BaseFont = new Font("Verdana", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.bottomPanelCaption.Dock = DockStyle.Top;
			this.bottomPanelCaption.Location = new Point(0, 0);
			this.bottomPanelCaption.Name = "bottomPanelCaption";
			this.bottomPanelCaption.TabIndex = 0;
			this.bottomPanelCaption.TabStop = false;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.splitContainer);
			base.Name = "WorkCenter";
			base.Controls.SetChildIndex(this.splitContainer, 0);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.bottomResultPane.StatusChanged -= this.BottomResultPane_StatusChanged;
			}
			base.Dispose(disposing);
		}

		private void separator_Paint(object sender, PaintEventArgs e)
		{
			ControlPaint.DrawBorder3D(e.Graphics, 0, 0, this.separator.Width, this.separator.Height - 2, Border3DStyle.SunkenOuter, Border3DSide.Top | Border3DSide.Bottom);
		}

		private void ResultPane_Enter(object sender, EventArgs e)
		{
			AbstractResultPane selectedResultPane = sender as AbstractResultPane;
			base.SelectedResultPane = selectedResultPane;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Category("Result Pane")]
		public Control TopPanel
		{
			get
			{
				return this.splitContainer.Panel1;
			}
		}

		[Category("ResultPane")]
		[DefaultValue(null)]
		public CaptionedResultPane TopPanelResultPane
		{
			get
			{
				return this.topPanelResultPane;
			}
			set
			{
				if (this.TopPanelResultPane != value)
				{
					if (this.TopPanelResultPane != null)
					{
						this.TopPanelResultPane.Enter -= this.ResultPane_Enter;
						this.BottomPanelResultPane.Enter -= this.ResultPane_Enter;
						this.TopPanel.Controls.Remove(this.TopPanelResultPane);
						base.ResultPanes.Remove(this.BottomPanelResultPane);
						base.ResultPanes.Remove(this.TopPanelResultPane);
						this.TopPanelResultPane.DependentResultPanes.Remove(this.BottomPanelResultPane);
						this.TopPanelResultPane.SelectionChanged -= this.TopPanelResultPane_SelectionChanged;
					}
					this.topPanelResultPane = value;
					if (this.TopPanelResultPane != null)
					{
						this.TopPanelResultPane.SelectionChanged += this.TopPanelResultPane_SelectionChanged;
						this.TopPanelResultPane.DependentResultPanes.Add(this.BottomPanelResultPane);
						base.ResultPanes.Add(this.TopPanelResultPane);
						base.ResultPanes.Add(this.BottomPanelResultPane);
						this.TopPanelResultPane.Dock = DockStyle.Fill;
						this.TopPanel.Controls.Add(this.TopPanelResultPane);
						this.TopPanelResultPane.Enter += this.ResultPane_Enter;
						this.BottomPanelResultPane.Enter += this.ResultPane_Enter;
						if (base.IsActive)
						{
							this.TopPanelResultPane.OnSetActive();
							this.BottomPanelResultPane.OnSetActive();
						}
						else
						{
							base.ResultPanesActiveToContainer.Add(this.TopPanelResultPane);
							base.ResultPanesActiveToContainer.Add(this.BottomPanelResultPane);
						}
						base.SelectedResultPane = this.TopPanelResultPane;
					}
				}
			}
		}

		[Category("Result Pane")]
		[DefaultValue(null)]
		public Icon WorkPaneIcon
		{
			get
			{
				return this.bottomPanelCaption.Icon;
			}
			set
			{
				this.bottomPanelCaption.Icon = value;
			}
		}

		[DefaultValue("")]
		[Category("Result Pane")]
		public string WorkPaneText
		{
			get
			{
				return this.bottomPanelCaption.Text;
			}
			set
			{
				this.bottomPanelCaption.Text = value;
			}
		}

		internal WorkPaneTabs TabControl
		{
			get
			{
				return this.bottomResultPane.TabControl;
			}
		}

		internal TabbedResultPane BottomPanelResultPane
		{
			get
			{
				return this.bottomResultPane;
			}
		}

		private void BottomResultPane_StatusChanged(object sender, EventArgs e)
		{
			this.bottomPanelCaption.Status = this.bottomResultPane.Status;
		}

		[Category("Result Pane")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BindingList<AbstractResultPane> WorkPanePages
		{
			get
			{
				return this.BottomPanelResultPane.ResultPaneTabs;
			}
		}

		protected override void OnSelectedResultPaneChanged(EventArgs e)
		{
			if (base.IsActive && base.SelectedResultPane != null && base.SelectedResultPane.Enabled)
			{
				base.ActiveControl = base.SelectedResultPane;
			}
			base.OnSelectedResultPaneChanged(e);
		}

		protected override void OnSetActive(EventArgs e)
		{
			base.OnSetActive(e);
			if (base.SelectedResultPane != null && base.SelectedResultPane.Enabled)
			{
				base.ActiveControl = base.SelectedResultPane;
			}
		}

		private void TopPanelResultPane_SelectionChanged(object sender, EventArgs e)
		{
			if (this.TopPanelResultPane.HasSelection)
			{
				string text = this.TopPanelResultPane.SelectionDataObject.GetText();
				this.WorkPaneText = text;
				if (this.WorkPaneIcon == null)
				{
					this.WorkPaneIcon = this.oldWorkPaneIcon;
					return;
				}
			}
			else
			{
				if (this.WorkPaneIcon != null)
				{
					this.oldWorkPaneIcon = this.WorkPaneIcon;
				}
				this.WorkPaneIcon = null;
				this.WorkPaneText = string.Empty;
			}
		}

		private Panel separator;

		private TabbedResultPane bottomResultPane;

		private ResultPaneCaption bottomPanelCaption;

		private SplitContainer splitContainer;

		private CaptionedResultPane topPanelResultPane;

		private Icon oldWorkPaneIcon;
	}
}
