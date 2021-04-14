using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[DefaultProperty("Text")]
	[Designer(typeof(ScrollableControlDesigner))]
	[DefaultEvent("StatusClick")]
	public class CollapsiblePanel : AutoSizePanel
	{
		public CollapsiblePanel()
		{
			this.InitializeComponent();
			this.BackColor = CollapsiblePanel.DefaultExpandedBackColor;
			this.chevron.Image = Icons.Collapse;
			this.UpdateBackground();
			Theme.UseVisualEffectsChanged += this.Theme_UseVisualEffectsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Theme.UseVisualEffectsChanged -= this.Theme_UseVisualEffectsChanged;
			}
			base.Dispose(disposing);
		}

		private void Theme_UseVisualEffectsChanged(object sender, EventArgs e)
		{
			base.Invalidate(true);
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(150, 31);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor
		{
			get
			{
				if (Theme.UseVisualEffects)
				{
					return base.BackColor;
				}
				return SystemColors.Window;
			}
			set
			{
				if (this.UseDefaultExpandedBackColor)
				{
					base.BackColor = value;
				}
			}
		}

		[DefaultValue(true)]
		[Browsable(false)]
		public bool UseDefaultExpandedBackColor
		{
			get
			{
				return this.useDefaultExpandedBackColor;
			}
			set
			{
				if (this.useDefaultExpandedBackColor != value)
				{
					this.useDefaultExpandedBackColor = value;
					base.BackColor = (this.useDefaultExpandedBackColor ? CollapsiblePanel.DefaultExpandedBackColor : Color.Transparent);
				}
			}
		}

		private bool ShouldSerializeBackColor()
		{
			return false;
		}

		public override void ResetBackColor()
		{
			this.BackColor = (this.IsMinimized ? CollapsiblePanel.DefaultMinimizedBackColor : CollapsiblePanel.DefaultExpandedBackColor);
		}

		protected override Padding DefaultMargin
		{
			get
			{
				return CollapsiblePanel.defaultMargin;
			}
		}

		protected override Size DefaultMinimumSize
		{
			get
			{
				return CollapsiblePanel.defaultMinimumSize;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override Size MinimumSize
		{
			get
			{
				return base.MinimumSize;
			}
			set
			{
				base.MinimumSize = value;
			}
		}

		[DefaultValue(null)]
		[Category("Appearance")]
		public Icon Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (value != this.Icon)
				{
					Bitmap bitmap = IconLibrary.ToSmallBitmap(value);
					if (this.image.Image != null)
					{
						this.image.Image.Dispose();
					}
					this.image.Image = bitmap;
					this.icon = value;
				}
			}
		}

		[DefaultValue("")]
		[Category("Appearance")]
		public string Status
		{
			get
			{
				return this.status.Text;
			}
			set
			{
				value = (value ?? "");
				if (this.status.Text != value)
				{
					this.status.LinkVisited = false;
					this.status.Text = value;
					this.PerformAlign();
				}
			}
		}

		[Category("Appearance")]
		[DefaultValue(true)]
		public bool StatusVisible
		{
			get
			{
				return this.status.Visible;
			}
			set
			{
				if (this.status.Visible != value)
				{
					this.status.Visible = value;
					this.PerformAlign();
				}
			}
		}

		private void PerformAlign()
		{
			if (base.Parent != null)
			{
				base.SuspendLayout();
				base.Parent.PerformLayout(null, CollapsiblePanel.AlignLayout);
				base.ResumeLayout();
			}
		}

		protected ToolStrip CaptionStrip
		{
			get
			{
				return this.captionStrip;
			}
		}

		internal int GetStatusWidth()
		{
			if (!this.StatusVisible)
			{
				return 0;
			}
			return this.status.GetPreferredSize(Size.Empty).Width;
		}

		internal void SetStatusWidth(int width)
		{
			if (width != this.status.Width)
			{
				this.CaptionStrip.SuspendLayout();
				this.status.Width = width;
				this.HideOverflowButtonOfCaptionStrip();
				this.CaptionStrip.ResumeLayout(true);
			}
		}

		[DefaultValue(null)]
		[Category("Appearance")]
		public Image StatusImage
		{
			get
			{
				return this.status.Image;
			}
			set
			{
				if (this.StatusImage != value)
				{
					this.status.LinkVisited = false;
					this.status.Image = value;
					this.PerformAlign();
				}
			}
		}

		[DefaultValue(false)]
		[Category("Appearance")]
		public bool StatusIsLink
		{
			get
			{
				return this.status.IsLink;
			}
			set
			{
				this.status.LinkVisited = false;
				this.status.IsLink = value;
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		protected override void OnTextChanged(EventArgs e)
		{
			this.caption.Text = this.Text.Replace("&", "&&");
			base.OnTextChanged(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[RefreshProperties(RefreshProperties.All)]
		[Browsable(false)]
		public int ExpandedHeight
		{
			get
			{
				if (this.IsMinimized)
				{
					return this.expandedHeight;
				}
				return base.Height;
			}
			set
			{
				this.expandedHeight = value;
				if (!this.IsMinimized)
				{
					base.Height = value;
				}
			}
		}

		private bool ShouldSerializeExpandedHeight()
		{
			return this.IsMinimized;
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(false)]
		[Category("Appearance")]
		public bool IsMinimized
		{
			get
			{
				return this.isMinimized;
			}
			set
			{
				if (!this.smoothSizing && this.IsMinimized != value)
				{
					if (value)
					{
						this.Collapse();
					}
					else
					{
						this.Expand();
					}
					this.isMinimized = value;
					this.OnIsMinimizedChanged(EventArgs.Empty);
				}
			}
		}

		internal void FastSetIsMinimized(bool collapse)
		{
			if (CollapsiblePanel.Animate)
			{
				CollapsiblePanel.Animate = false;
				try
				{
					this.IsMinimized = collapse;
					return;
				}
				finally
				{
					CollapsiblePanel.Animate = true;
				}
			}
			this.IsMinimized = collapse;
		}

		private void Collapse()
		{
			this.oldAutoSize = this.AutoSize;
			this.AutoSize = false;
			this.expandedHeight = base.Height;
			this.chevron.Image = Icons.CollapseToExpand;
			this.SmoothHeightChange(this.captionStrip.Height);
			this.chevron.Image = Icons.Expand;
			this.captionStrip.BackgroundImage = null;
			this.BackColor = CollapsiblePanel.DefaultMinimizedBackColor;
		}

		private void Expand()
		{
			this.captionStrip.BackgroundImage = this.captionBackground;
			this.chevron.Image = Icons.ExpandToCollapse;
			this.SmoothHeightChange(this.expandedHeight);
			this.chevron.Image = Icons.Collapse;
			this.AutoSize = this.oldAutoSize;
			if (CollapsiblePanel.Animate && base.Parent is ScrollableControl)
			{
				(base.Parent as ScrollableControl).ScrollControlIntoView(this);
			}
			this.BackColor = CollapsiblePanel.DefaultExpandedBackColor;
		}

		protected virtual void OnIsMinimizedChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[CollapsiblePanel.EventIsMinimizedChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		[Category("Appearance")]
		public event EventHandler IsMinimizedChanged
		{
			add
			{
				base.Events.AddHandler(CollapsiblePanel.EventIsMinimizedChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(CollapsiblePanel.EventIsMinimizedChanged, value);
			}
		}

		private void SmoothHeightChange(int endHeight)
		{
			base.SuspendLayout();
			this.smoothSizing = true;
			try
			{
				bool flag = Theme.UseVisualEffects && CollapsiblePanel.Animate && base.IsHandleCreated && base.Parent != null && base.Parent.Controls.Count <= 100;
				if (flag)
				{
					int height = base.Height;
					int num = endHeight - height;
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					do
					{
						this.smoothSizeProgress = Math.Max(0f, Math.Min((float)stopwatch.ElapsedMilliseconds / 200f, 1f));
						base.Height = height + (int)((float)num * this.smoothSizeProgress);
						this.UpdateBackground();
						base.Parent.Update();
					}
					while ((float)stopwatch.ElapsedMilliseconds < 200f);
				}
				this.smoothSizeProgress = 1f;
				base.Height = endHeight;
				this.UpdateBackground();
			}
			finally
			{
				this.smoothSizeProgress = 0f;
				this.smoothSizing = false;
				base.ResumeLayout();
			}
		}

		protected override void Select(bool directed, bool forward)
		{
			if (this.IsMinimized)
			{
				base.ActiveControl = this.captionStrip;
				this.chevron.Select();
				return;
			}
			base.Select(directed, forward);
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			this.captionStrip.SendToBack();
			base.OnLayout(e);
			this.HideOverflowButtonOfCaptionStrip();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (!this.IsMinimized && !this.smoothSizing)
			{
				this.expandedHeight = base.Height;
			}
		}

		private void UpdateBackground()
		{
			float num = (this.IsMinimized && this.smoothSizing) ? this.smoothSizeProgress : (1f - this.smoothSizeProgress);
			int opacityLevel = (int)(255f * num);
			this.captionBackground = CollapsiblePanel.BackgroundCache.GetImage(opacityLevel);
			int num2 = (int)(this.IsMinimized ? CollapsiblePanel.DefaultMinimizedBackColor.R : CollapsiblePanel.DefaultExpandedBackColor.R);
			int num3 = (int)(this.IsMinimized ? CollapsiblePanel.DefaultExpandedBackColor.R : CollapsiblePanel.DefaultMinimizedBackColor.R);
			int num4 = (int)(this.smoothSizeProgress * (float)num3 + (1f - this.smoothSizeProgress) * (float)num2);
			this.BackColor = Color.FromArgb(num4, num4, num4);
			this.captionStrip.BackgroundImage = this.captionBackground;
		}

		private void chevron_Click(object sender, EventArgs e)
		{
			this.IsMinimized = !this.IsMinimized;
		}

		private void captionStrip_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == ' ')
			{
				this.chevron_Click(sender, e);
				e.Handled = true;
			}
		}

		private void captionStrip_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ToolStripItem itemAt = this.captionStrip.GetItemAt(e.Location);
				if (itemAt != this.chevron && itemAt != this.status)
				{
					this.chevron.PerformClick();
				}
			}
		}

		private void status_Click(object sender, EventArgs e)
		{
			this.status.LinkVisited = true;
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			this.OnStatusClick(cancelEventArgs);
			if (!cancelEventArgs.Cancel)
			{
				this.chevron.PerformClick();
			}
		}

		protected virtual void OnStatusClick(CancelEventArgs e)
		{
			CancelEventHandler cancelEventHandler = (CancelEventHandler)base.Events[CollapsiblePanel.EventStatusClick];
			if (cancelEventHandler != null)
			{
				cancelEventHandler(this, e);
			}
		}

		[Category("Appearance")]
		public event CancelEventHandler StatusClick
		{
			add
			{
				base.Events.AddHandler(CollapsiblePanel.EventStatusClick, value);
			}
			remove
			{
				base.Events.RemoveHandler(CollapsiblePanel.EventStatusClick, value);
			}
		}

		private void InitializeComponent()
		{
			this.captionStrip = new TabbableToolStrip();
			this.image = new ToolStripLabel();
			this.caption = new ToolStripLabel();
			this.chevron = new ToolStripButton();
			this.status = new ToolStripLabel();
			this.captionStrip.SuspendLayout();
			base.SuspendLayout();
			this.captionStrip.BackColor = Color.Transparent;
			this.captionStrip.BackgroundImageLayout = ImageLayout.Stretch;
			this.captionStrip.GripStyle = ToolStripGripStyle.Hidden;
			this.captionStrip.Items.AddRange(new ToolStripItem[]
			{
				this.image,
				this.caption,
				this.chevron,
				this.status
			});
			this.captionStrip.Location = new Point(0, 0);
			this.captionStrip.Name = "captionStrip";
			this.captionStrip.Size = new Size(150, 25);
			this.captionStrip.Stretch = true;
			this.captionStrip.TabIndex = 0;
			this.captionStrip.TabStop = true;
			this.captionStrip.KeyPress += this.captionStrip_KeyPress;
			this.captionStrip.MouseClick += this.captionStrip_MouseClick;
			this.image.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.image.Name = "image";
			this.image.Overflow = ToolStripItemOverflow.Never;
			this.image.Size = new Size(0, 22);
			this.caption.AutoSize = false;
			this.caption.BackColor = Color.Transparent;
			this.caption.ImageAlign = ContentAlignment.MiddleLeft;
			this.caption.Name = "caption";
			this.caption.Size = new Size(4, 22);
			this.caption.TextAlign = ContentAlignment.MiddleLeft;
			this.chevron.Alignment = ToolStripItemAlignment.Right;
			this.chevron.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.chevron.ImageScaling = ToolStripItemImageScaling.None;
			this.chevron.Margin = new Padding(4, 1, 1, 2);
			this.chevron.Name = "chevron";
			this.chevron.Overflow = ToolStripItemOverflow.Never;
			this.chevron.Size = new Size(23, 22);
			this.chevron.Click += this.chevron_Click;
			this.status.ActiveLinkColor = Color.FromArgb(153, 153, 153);
			this.status.Alignment = ToolStripItemAlignment.Right;
			this.status.AutoSize = false;
			this.status.BackColor = Color.Transparent;
			this.status.ForeColor = Color.FromArgb(153, 153, 153);
			this.status.ImageAlign = ContentAlignment.MiddleLeft;
			this.status.LinkColor = Color.FromArgb(153, 153, 153);
			this.status.Name = "status";
			this.status.Overflow = ToolStripItemOverflow.Never;
			this.status.Size = new Size(0, 22);
			this.status.TextAlign = ContentAlignment.MiddleLeft;
			this.status.VisitedLinkColor = Color.FromArgb(153, 153, 153);
			this.status.Click += this.status_Click;
			base.Controls.Add(this.captionStrip);
			base.Name = "CollapsiblePanel";
			base.Size = new Size(150, 25);
			this.captionStrip.ResumeLayout(false);
			this.captionStrip.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			this.captionStrip.Padding = (LayoutHelper.IsRightToLeft(this) ? new Padding(0, 0, 5, 0) : new Padding(5, 0, 0, 0));
			this.caption.Padding = (LayoutHelper.IsRightToLeft(this) ? new Padding(0, 0, 4, 0) : new Padding(4, 0, 0, 0));
			base.OnRightToLeftChanged(e);
		}

		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessTabKey(bool forward)
		{
			if (this.IsMinimized && base.Parent != null)
			{
				return base.Parent.SelectNextControl(this, forward, true, false, false);
			}
			return base.ProcessTabKey(forward);
		}

		private void HideOverflowButtonOfCaptionStrip()
		{
			int num = 0;
			int num2 = this.CaptionStrip.DisplayRectangle.Width;
			foreach (object obj in this.CaptionStrip.Items)
			{
				ToolStripItem toolStripItem = (ToolStripItem)obj;
				if (toolStripItem != this.caption && toolStripItem.Placement == ToolStripItemPlacement.Main)
				{
					num += toolStripItem.Width + toolStripItem.Margin.Horizontal;
				}
			}
			num2 -= num;
			num2 -= this.caption.Margin.Horizontal;
			this.caption.Width = num2;
			if (this.CaptionStrip.OverflowButton.Visible)
			{
				this.CaptionStrip.PerformLayout();
			}
		}

		private const string category = "Appearance";

		private Bitmap captionBackground;

		private ToolStripLabel image;

		internal static readonly string AlignLayout = "AlignStatus";

		private bool useDefaultExpandedBackColor = true;

		public static readonly Color DefaultMinimizedBackColor = Color.FromArgb(238, 238, 238);

		public static readonly Color DefaultExpandedBackColor = Color.FromArgb(247, 247, 247);

		private static readonly Padding defaultMargin = new Padding(0, 0, 0, 1);

		private static readonly Size defaultMinimumSize = new Size(0, 25);

		private Icon icon;

		private int expandedHeight;

		private bool isMinimized;

		private bool oldAutoSize;

		private static readonly object EventIsMinimizedChanged = new object();

		internal static bool Animate = true;

		private bool smoothSizing;

		private float smoothSizeProgress;

		private static readonly object EventStatusClick = new object();

		private TabbableToolStrip captionStrip;

		private ToolStripLabel caption;

		private ToolStripLabel status;

		private ToolStripButton chevron;

		private static class BackgroundCache
		{
			static BackgroundCache()
			{
				Size size = new Size(1, 25);
				Rectangle rect = new Rectangle(Point.Empty, size);
				for (int i = 0; i < 16; i++)
				{
					CollapsiblePanel.BackgroundCache.bitmaps[i] = new Bitmap(size.Width, size.Height);
					using (Graphics graphics = Graphics.FromImage(CollapsiblePanel.BackgroundCache.bitmaps[i]))
					{
						int alpha = (i + 1) * 16 - 1;
						Color color = Color.FromArgb(alpha, 255, 255, 255);
						Color color2 = Color.FromArgb(alpha, 204, 204, 204);
						using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rect, color, color2, LinearGradientMode.Vertical))
						{
							graphics.FillRectangle(linearGradientBrush, rect);
						}
					}
				}
			}

			public static Bitmap GetImage(int opacityLevel)
			{
				return CollapsiblePanel.BackgroundCache.bitmaps[opacityLevel / 16];
			}

			private const int imageCount = 16;

			private const int opacityFactorsPerImage = 16;

			private static Bitmap[] bitmaps = new Bitmap[16];
		}
	}
}
