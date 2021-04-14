using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class TabbedResultPane : ContainerResultPane
	{
		public TabbedResultPane()
		{
			this.InitializeComponent();
			this.tabControl.Multiline = true;
			this.tabControl.SizeMode = TabSizeMode.FillToRight;
			this.ResultPaneTabs.ListChanged += this.ResultPaneTabs_ListChanged;
			this.tabControl.SelectedIndexChanged += this.TabControl_SelectedIndexChanged;
		}

		private void InitializeComponent()
		{
			this.tabControl = new WorkPaneTabs();
			this.caption = new ResultPaneCaption();
			base.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabControl.Dock = DockStyle.Fill;
			this.tabControl.Name = "tabControl";
			this.tabControl.Enabled = true;
			this.caption.AutoSize = true;
			this.caption.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.caption.BackColor = SystemColors.ControlDark;
			this.caption.BaseFont = new Font("Verdana", 9.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.caption.Dock = DockStyle.Top;
			this.caption.ForeColor = SystemColors.ControlLightLight;
			this.caption.Location = new Point(0, 0);
			this.caption.Name = "caption";
			this.caption.TabIndex = 0;
			this.caption.TabStop = false;
			base.Controls.Add(this.caption);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.tabControl);
			base.Name = "TabbedResultPane";
			base.Controls.SetChildIndex(this.tabControl, 0);
			this.tabControl.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.ResultPaneTabs.ListChanged -= this.ResultPaneTabs_ListChanged;
				this.tabControl.SelectedIndexChanged -= this.TabControl_SelectedIndexChanged;
			}
			base.Dispose(disposing);
		}

		[DefaultValue(true)]
		public bool IsCaptionVisible
		{
			get
			{
				return this.caption.Visible;
			}
			set
			{
				this.caption.Visible = value;
			}
		}

		[Category("Result Pane")]
		[DefaultValue("")]
		public string CaptionText
		{
			get
			{
				return this.caption.Text;
			}
			set
			{
				if (this.CaptionText != value)
				{
					this.caption.Text = value;
					this.OnCaptionTextChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnCaptionTextChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[TabbedResultPane.EventCaptionTextChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler CaptionTextChanged
		{
			add
			{
				base.Events.AddHandler(TabbedResultPane.EventCaptionTextChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(TabbedResultPane.EventCaptionTextChanged, value);
			}
		}

		protected override void OnStatusChanged(EventArgs e)
		{
			this.caption.Status = base.Status;
			base.OnStatusChanged(e);
		}

		protected override void OnIconChanged(EventArgs e)
		{
			base.OnIconChanged(e);
			this.caption.Icon = base.Icon;
		}

		internal WorkPaneTabs TabControl
		{
			get
			{
				return this.tabControl;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Result Pane")]
		public BindingList<AbstractResultPane> ResultPaneTabs
		{
			get
			{
				return this.resultPaneTabs;
			}
		}

		private void ResultPaneTabs_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				this.ResultPaneTabs_ResultPaneAdded(e.NewIndex);
				return;
			}
			if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				this.ResultPaneTabs_ResultPaneRemoved(e.NewIndex);
				return;
			}
			if (e.ListChangedType == ListChangedType.Reset)
			{
				this.ResultPaneTabs_ResultPaneResetted();
			}
		}

		private void ResultPaneTabs_ResultPaneAdded(int index)
		{
			base.ResultPanes.Insert(index, this.ResultPaneTabs[index]);
			WorkPanePage workPanePage = new WorkPanePage();
			workPanePage.ResultPane = this.ResultPaneTabs[index];
			workPanePage.Name = "WorkPane" + this.ResultPaneTabs[index].Name;
			if (index == this.tabControl.TabPages.Count)
			{
				this.tabControl.TabPages.Add(workPanePage);
			}
			else
			{
				this.tabControl.TabPages.Insert(index, workPanePage);
			}
			if (this.tabControl.SelectedIndex == -1)
			{
				this.tabControl.SelectedIndex = 0;
			}
			this.TabControl_SelectedIndexChanged(this.tabControl, EventArgs.Empty);
		}

		private void ResultPaneTabs_ResultPaneRemoved(int index)
		{
			if (index == this.tabControl.SelectedIndex)
			{
				if (this.tabControl.TabPages.Count != 1)
				{
					this.tabControl.SelectedIndex = ((index == 0) ? 1 : (index - 1));
				}
				else
				{
					this.tabControl.SelectedIndex = -1;
				}
				this.TabControl_SelectedIndexChanged(this.tabControl, EventArgs.Empty);
			}
			this.tabControl.TabPages.RemoveAt(index);
			base.ResultPanes.RemoveAt(index);
		}

		private void ResultPaneTabs_ResultPaneResetted()
		{
			while (base.ResultPanes.Count > 0)
			{
				this.ResultPaneTabs_ResultPaneRemoved(base.ResultPanes.Count - 1);
			}
			for (int i = 0; i < this.ResultPaneTabs.Count; i++)
			{
				this.ResultPaneTabs_ResultPaneAdded(i);
			}
		}

		protected override void OnSelectedResultPaneChanged(EventArgs e)
		{
			base.SuspendLayout();
			if (this.tabControl.SelectedResultPane != base.SelectedResultPane)
			{
				AbstractResultPane selectedResultPane = this.tabControl.SelectedResultPane;
				int selectedIndex = base.ResultPanes.IndexOf(base.SelectedResultPane);
				this.tabControl.SelectedIndex = selectedIndex;
				if (selectedResultPane != null)
				{
					if (base.IsActive)
					{
						selectedResultPane.OnKillActive();
					}
					else
					{
						base.ResultPanesActiveToContainer.Remove(selectedResultPane);
					}
				}
			}
			base.ResumeLayout(true);
			base.OnSelectedResultPaneChanged(e);
		}

		private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.tabControl.SelectedResultPane != base.SelectedResultPane)
			{
				AbstractResultPane selectedResultPane = base.SelectedResultPane;
				base.SelectedResultPane = this.tabControl.SelectedResultPane;
				if (selectedResultPane != null)
				{
					if (base.IsActive)
					{
						selectedResultPane.OnKillActive();
						return;
					}
					base.ResultPanesActiveToContainer.Remove(selectedResultPane);
				}
			}
		}

		private ResultPaneCaption caption;

		private WorkPaneTabs tabControl;

		private static readonly object EventCaptionTextChanged = new object();

		private BindingList<AbstractResultPane> resultPaneTabs = new BindingList<AbstractResultPane>();
	}
}
