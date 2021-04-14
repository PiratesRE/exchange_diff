using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Designer(typeof(ScrollableControlDesigner))]
	public class CaptionedResultPane : DataListViewResultPane
	{
		public CaptionedResultPane() : this(null, null)
		{
		}

		public CaptionedResultPane(IResultsLoaderConfiguration config) : this((config != null) ? config.BuildResultsLoaderProfile() : null, null)
		{
		}

		public CaptionedResultPane(DataTableLoader loader) : this((loader != null) ? loader.ResultsLoaderProfile : null, loader)
		{
		}

		public CaptionedResultPane(ObjectPickerProfileLoader profileLoader, string profileName) : this(profileLoader.GetProfile(profileName))
		{
		}

		public CaptionedResultPane(ResultsLoaderProfile profile) : this(profile, null)
		{
		}

		protected CaptionedResultPane(ResultsLoaderProfile profile, DataTableLoader loader) : base(profile, loader)
		{
			this.InitializeComponent();
			base.Name = "CaptionedResultPane";
			if (base.ResultsLoaderProfile != null)
			{
				this.CaptionText = base.ResultsLoaderProfile.DisplayName;
			}
		}

		protected override void OnStatusChanged(EventArgs e)
		{
			base.OnStatusChanged(e);
			this.caption.Status = base.Status;
		}

		protected override void OnIconChanged(EventArgs e)
		{
			base.OnIconChanged(e);
			this.caption.Icon = base.Icon;
		}

		[DefaultValue("")]
		[Category("Result Pane")]
		public string CaptionText
		{
			get
			{
				return this.caption.Text;
			}
			set
			{
				this.caption.Text = value;
			}
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			this.caption.SendToBack();
			base.OnLayout(e);
		}

		private void InitializeComponent()
		{
			this.caption = new ResultPaneCaption();
			base.SuspendLayout();
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
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.caption);
			base.Name = "RootResultPane";
			base.Size = new Size(400, 400);
			base.ResumeLayout(false);
		}

		[DefaultValue(true)]
		public bool ShowCaption
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

		private ResultPaneCaption caption;
	}
}
