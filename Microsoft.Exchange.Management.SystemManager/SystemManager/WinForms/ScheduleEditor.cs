using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ScheduleEditor : ExchangeUserControl
	{
		static ScheduleEditor()
		{
			ScheduleEditor.dayTexts = new string[]
			{
				Strings.Sunday,
				Strings.Monday,
				Strings.Tuesday,
				Strings.Wednesday,
				Strings.Thursday,
				Strings.Friday,
				Strings.Saturday
			};
		}

		public ScheduleEditor()
		{
			this.InitializeComponent();
			this.gridControl.SupportDoubleBuffer = true;
			this.gridHeaderControl.SupportDoubleBuffer = true;
			this.sunMoonControl.SupportDoubleBuffer = true;
			this.detailViewGroupBox.Text = Strings.ScheduleDetailView;
			this.hourRadioButton.Text = Strings.OneHourDetails;
			this.intervalRadioButton.Text = Strings.FifteenMinutesDetailsView;
			this.sunMoonControl.Paint += this.sunMoonControl_Paint;
			this.gridHeaderControl.Paint += this.gridHeaderControl_Paint;
			this.gridControl.MouseLeave += this.gridControl_MouseLeave;
			this.gridControl.MouseDown += this.gridControl_MouseDown;
			this.gridControl.MouseUp += this.gridControl_MouseUp;
			this.gridControl.MouseMove += this.gridControl_MouseMove;
			this.gridControl.Paint += this.gridControl_Paint;
			this.gridControl.PreviewKeyDown += this.gridControl_PreviewKeyDown;
			this.gridControl.KeyDown += this.gridControl_KeyDown;
			this.gridControl.LostFocus += delegate(object param0, EventArgs param1)
			{
				this.gridControl.Invalidate();
			};
			this.scrollPanel.Scroll += this.scrollPanel_Scroll;
			this.hourRadioButton.Click += this.hour_Click;
			this.intervalRadioButton.Click += this.interval_Click;
			this.hintLabel.Paint += this.hintLabel_Paint;
			this.hintLabel.TextChanged += this.hintLabel_TextChanged;
			this.Schedule = Schedule.Never;
		}

		private void InitializeComponent()
		{
			this.contentPanel = new AutoTableLayoutPanel();
			this.hintLabel = new OwnerDrawControl();
			this.scrollPanel = new Panel();
			this.gridControl = new OwnerDrawControl();
			this.intervalButtonsPanel = new FlowLayoutPanel();
			this.dayButtonsPanel = new TableLayoutPanel();
			this.gridHeaderControl = new OwnerDrawControl();
			this.sunMoonControl = new OwnerDrawControl();
			this.detailViewPanel = new Panel();
			this.detailViewGroupBox = new GroupBox();
			this.detailViewTableLayoutPanel = new TableLayoutPanel();
			this.hourRadioButton = new AutoHeightRadioButton();
			this.intervalRadioButton = new AutoHeightRadioButton();
			this.contentPanel.SuspendLayout();
			this.scrollPanel.SuspendLayout();
			this.detailViewPanel.SuspendLayout();
			this.detailViewGroupBox.SuspendLayout();
			this.detailViewTableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.contentPanel.AutoSize = true;
			this.contentPanel.ColumnCount = 3;
			this.contentPanel.ColumnStyles.Add(new ColumnStyle());
			this.contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16f));
			this.contentPanel.Controls.Add(this.hintLabel, 1, 4);
			this.contentPanel.Controls.Add(this.scrollPanel, 1, 3);
			this.contentPanel.Controls.Add(this.dayButtonsPanel, 0, 3);
			this.contentPanel.Controls.Add(this.gridHeaderControl, 0, 2);
			this.contentPanel.Controls.Add(this.sunMoonControl, 0, 1);
			this.contentPanel.Controls.Add(this.detailViewPanel, 1, 0);
			this.contentPanel.Dock = DockStyle.Top;
			this.contentPanel.Location = new Point(6, 0);
			this.contentPanel.Margin = new Padding(0);
			this.contentPanel.Name = "contentPanel";
			this.contentPanel.Padding = new Padding(0);
			this.contentPanel.RowCount = 5;
			this.contentPanel.RowStyles.Add(new RowStyle());
			this.contentPanel.RowStyles.Add(new RowStyle());
			this.contentPanel.RowStyles.Add(new RowStyle());
			this.contentPanel.RowStyles.Add(new RowStyle());
			this.contentPanel.RowStyles.Add(new RowStyle());
			this.contentPanel.Size = new Size(353, 306);
			this.contentPanel.TabIndex = 13;
			this.hintLabel.Dock = DockStyle.Top;
			this.hintLabel.Location = new Point(0, 283);
			this.hintLabel.Margin = new Padding(0);
			this.hintLabel.Name = "hintLabel";
			this.hintLabel.Size = new Size(353, 16);
			this.hintLabel.TabIndex = 35;
			this.scrollPanel.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.scrollPanel.AutoScroll = true;
			this.scrollPanel.Controls.Add(this.gridControl);
			this.scrollPanel.Controls.Add(this.intervalButtonsPanel);
			this.scrollPanel.Location = new Point(0, 97);
			this.scrollPanel.Margin = new Padding(0);
			this.scrollPanel.Name = "scrollPanel";
			this.scrollPanel.Size = new Size(353, 186);
			this.scrollPanel.TabIndex = 34;
			this.gridControl.BackColor = SystemColors.Window;
			this.gridControl.Location = new Point(0, 19);
			this.gridControl.Margin = new Padding(0);
			this.gridControl.Name = "gridControl";
			this.gridControl.Size = new Size(353, 167);
			this.gridControl.TabIndex = 2;
			this.gridControl.TabStop = false;
			this.intervalButtonsPanel.Location = new Point(0, 0);
			this.intervalButtonsPanel.Margin = new Padding(0);
			this.intervalButtonsPanel.Name = "intervalButtonsPanel";
			this.intervalButtonsPanel.Size = new Size(353, 19);
			this.intervalButtonsPanel.TabIndex = 1;
			this.dayButtonsPanel.AutoSize = true;
			this.dayButtonsPanel.ColumnCount = 1;
			this.dayButtonsPanel.ColumnStyles.Add(new ColumnStyle());
			this.dayButtonsPanel.Dock = DockStyle.Left;
			this.dayButtonsPanel.Location = new Point(0, 97);
			this.dayButtonsPanel.Margin = new Padding(0);
			this.dayButtonsPanel.Name = "dayButtonsPanel";
			this.dayButtonsPanel.Size = new Size(0, 209);
			this.dayButtonsPanel.TabIndex = 33;
			this.contentPanel.SetColumnSpan(this.gridHeaderControl, 3);
			this.gridHeaderControl.Dock = DockStyle.Top;
			this.gridHeaderControl.Location = new Point(0, 76);
			this.gridHeaderControl.Margin = new Padding(0);
			this.gridHeaderControl.Name = "gridHeaderControl";
			this.gridHeaderControl.Size = new Size(353, 21);
			this.gridHeaderControl.TabIndex = 31;
			this.gridHeaderControl.TabStop = false;
			this.contentPanel.SetColumnSpan(this.sunMoonControl, 3);
			this.sunMoonControl.Dock = DockStyle.Top;
			this.sunMoonControl.Location = new Point(0, 51);
			this.sunMoonControl.Margin = new Padding(0);
			this.sunMoonControl.Name = "sunMoonControl";
			this.sunMoonControl.Size = new Size(353, 25);
			this.sunMoonControl.TabIndex = 30;
			this.sunMoonControl.TabStop = false;
			this.detailViewPanel.AutoSize = true;
			this.detailViewPanel.Controls.Add(this.detailViewGroupBox);
			this.detailViewPanel.Dock = DockStyle.Top;
			this.detailViewPanel.Location = new Point(0, 0);
			this.detailViewPanel.Margin = new Padding(0, 0, 0, 8);
			this.detailViewPanel.Name = "detailViewPanel";
			this.detailViewPanel.Size = new Size(353, 51);
			this.detailViewPanel.TabIndex = 28;
			this.detailViewPanel.Text = "detailViewPanel";
			this.detailViewGroupBox.Controls.Add(this.detailViewTableLayoutPanel);
			this.detailViewGroupBox.Dock = DockStyle.Top;
			this.detailViewGroupBox.Location = new Point(0, 0);
			this.detailViewGroupBox.Margin = new Padding(0);
			this.detailViewGroupBox.Name = "detailViewGroupBox";
			this.detailViewGroupBox.Size = new Size(353, 43);
			this.detailViewGroupBox.TabIndex = 36;
			this.detailViewGroupBox.TabStop = false;
			this.detailViewGroupBox.Text = "detailViewGroupBox";
			this.detailViewTableLayoutPanel.AutoSize = true;
			this.detailViewTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.detailViewTableLayoutPanel.ColumnCount = 2;
			this.detailViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
			this.detailViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.detailViewTableLayoutPanel.Controls.Add(this.hourRadioButton, 0, 0);
			this.detailViewTableLayoutPanel.Controls.Add(this.intervalRadioButton, 1, 0);
			this.detailViewTableLayoutPanel.Dock = DockStyle.Top;
			this.detailViewTableLayoutPanel.Location = new Point(3, 16);
			this.detailViewTableLayoutPanel.Margin = new Padding(0);
			this.detailViewTableLayoutPanel.Name = "detailViewTableLayoutPanel";
			this.detailViewTableLayoutPanel.RowCount = 1;
			this.detailViewTableLayoutPanel.RowStyles.Add(new RowStyle());
			this.detailViewTableLayoutPanel.Size = new Size(347, 23);
			this.detailViewTableLayoutPanel.TabIndex = 0;
			this.hourRadioButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.hourRadioButton.CheckAlign = ContentAlignment.MiddleLeft;
			this.hourRadioButton.Location = new Point(3, 3);
			this.hourRadioButton.Name = "hourRadioButton";
			this.hourRadioButton.Size = new Size(108, 17);
			this.hourRadioButton.TabIndex = 30;
			this.hourRadioButton.TabStop = true;
			this.hourRadioButton.Text = "hourRadioButton";
			this.hourRadioButton.TextAlign = ContentAlignment.MiddleLeft;
			this.hourRadioButton.UseVisualStyleBackColor = true;
			this.intervalRadioButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.intervalRadioButton.CheckAlign = ContentAlignment.MiddleLeft;
			this.intervalRadioButton.Location = new Point(117, 3);
			this.intervalRadioButton.Name = "intervalRadioButton";
			this.intervalRadioButton.Size = new Size(227, 17);
			this.intervalRadioButton.TabIndex = 32;
			this.intervalRadioButton.TabStop = true;
			this.intervalRadioButton.Text = "intervalRadioButton";
			this.intervalRadioButton.TextAlign = ContentAlignment.MiddleLeft;
			this.intervalRadioButton.UseVisualStyleBackColor = true;
			base.Controls.Add(this.contentPanel);
			base.Name = "ScheduleEditor";
			base.Size = new Size(364, 307);
			this.contentPanel.ResumeLayout(false);
			this.contentPanel.PerformLayout();
			this.scrollPanel.ResumeLayout(false);
			this.detailViewPanel.ResumeLayout(false);
			this.detailViewGroupBox.ResumeLayout(false);
			this.detailViewGroupBox.PerformLayout();
			this.detailViewTableLayoutPanel.ResumeLayout(false);
			this.detailViewTableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.hourIndicatorBrush == null)
			{
				this.hourIndicatorBrush = new SolidBrush(this.hintLabel.ForeColor);
			}
			if (this.hourIndicatorPen == null)
			{
				this.hourIndicatorPen = new Pen(this.hourIndicatorBrush);
			}
			if (this.gridBrush == null)
			{
				this.gridBrush = new SolidBrush(ControlPaint.Light(SystemColors.Highlight));
			}
			this.InitializeContextMenu();
			this.InitializeDayButtons();
			this.InitializeCellSize();
			this.LayoutIntervalButtons();
			this.AdjustScrollpanel();
			this.gridHeaderControl.Invalidate(true);
			this.sunMoonControl.Invalidate(true);
			this.gridControl.Invalidate(true);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.hourIndicatorPen != null)
				{
					this.hourIndicatorPen.Dispose();
				}
				if (this.hourIndicatorBrush != null)
				{
					this.hourIndicatorBrush.Dispose();
				}
				if (this.gridBrush != null)
				{
					this.gridBrush.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeContextMenu()
		{
			this.hourMenuItem = new MenuItem();
			this.hourMenuItem.Checked = true;
			this.hourMenuItem.Index = 0;
			this.hourMenuItem.Text = Strings.OneHourDetails;
			this.hourMenuItem.Click += this.hour_Click;
			this.intervalMenuItem = new MenuItem();
			this.intervalMenuItem.Index = 1;
			this.intervalMenuItem.Text = Strings.FifteenMinutesDetailsView;
			this.intervalMenuItem.Click += this.interval_Click;
			this.contextMenu = new ContextMenu();
			this.contextMenu.MenuItems.AddRange(new MenuItem[]
			{
				this.hourMenuItem,
				this.intervalMenuItem
			});
			this.ContextMenu = this.contextMenu;
		}

		private void hour_Click(object sender, EventArgs e)
		{
			this.hourRadioButton.Checked = true;
			this.ShowIntervals = false;
		}

		private void interval_Click(object sender, EventArgs e)
		{
			this.intervalRadioButton.Checked = true;
			this.ShowIntervals = true;
		}

		private void InitializeDayButtons()
		{
			base.SuspendLayout();
			this.dayButtonsPanel.SuspendLayout();
			string[] array = new string[8];
			array[0] = Strings.All;
			ScheduleEditor.dayTexts.CopyTo(array, 1);
			for (int i = 0; i < array.Length; i++)
			{
				Button button = new ExchangeButton();
				button.Margin = new Padding(0);
				button.Padding = new Padding(0);
				button.FlatStyle = FlatStyle.System;
				button.Text = array[i];
				button.AutoSize = true;
				button.TabIndex = i;
				button.TabStop = (i == 0);
				button.Dock = DockStyle.Top;
				button.Tag = i - 1;
				this.dayButtonsPanel.RowCount++;
				this.dayButtonsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				this.dayButtonsPanel.Controls.Add(button, 0, i);
				button.MouseEnter += this.dayButton_Enter;
				button.MouseLeave += this.dayButton_Leave;
				button.Enter += this.dayButton_Enter;
				button.Leave += this.dayButton_Leave;
				button.Click += ((i == 0) ? new EventHandler(this.all_Click) : new EventHandler(this.dayButton_Click));
				button.PreviewKeyDown += this.Button_PreviewKeyDown;
				button.KeyDown += this.DayButton_KeyDown;
				if (i == 0)
				{
					button.GotFocus += delegate(object param0, EventArgs param1)
					{
						this.SetFocus();
					};
				}
			}
			this.dayButtonsPanel.ResumeLayout(false);
			this.dayButtonsPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void dayButton_Enter(object sender, EventArgs e)
		{
			Button button = sender as Button;
			this.UpdateHintText(new Point(-1, (int)button.Tag));
		}

		private void dayButton_Leave(object sender, EventArgs e)
		{
			Button button = sender as Button;
			if (((Point)this.hintLabel.Tag).Equals(new Point(-1, (int)button.Tag)))
			{
				this.ResetHintText(sender, e);
			}
		}

		private void all_Click(object sender, EventArgs e)
		{
			if (Schedule.Never == this.scheduleBuilder.Schedule)
			{
				this.scheduleBuilder = new ScheduleBuilder(Schedule.Always);
			}
			else
			{
				this.scheduleBuilder = new ScheduleBuilder(Schedule.Never);
			}
			this.selectionEnd = (this.selectionStart = new Point(-1, -1));
			this.gridControl.Invalidate(true);
		}

		private void dayButton_Click(object sender, EventArgs e)
		{
			DayOfWeek dayOfWeek = (DayOfWeek)((Button)sender).Tag;
			this.scheduleBuilder.SetStateOfDay(dayOfWeek, !this.scheduleBuilder.GetStateOfDay(dayOfWeek));
			this.selectionEnd = (this.selectionStart = new Point(-1, (int)dayOfWeek));
			this.gridControl.Invalidate(true);
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			base.ScaleControl(factor, specified);
			this.cellSize = new Size((int)((float)this.cellSize.Width * factor.Width), (int)((float)this.cellSize.Height * factor.Height));
		}

		private void InitializeCellSize()
		{
			this.cellSize.Height = this.dayButtonsPanel.Controls[0].Height;
			Size proposedSize = new Size(int.MaxValue, int.MaxValue);
			for (int i = 0; i < 24; i++)
			{
				this.textSizes[i.ToString()] = TextRenderer.MeasureText(i.ToString(), this.hintLabel.Font, proposedSize, TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
				this.cellSize.Width = Math.Max((this.textSizes[i.ToString()].Width + this.hourIndicatorSize.Width) / 2 + 1, this.cellSize.Width);
			}
			int num = this.cellSize.Width % 4;
			if (num == 0)
			{
				this.cellSize.Width = this.cellSize.Width + 1;
				return;
			}
			if (num != 1)
			{
				this.cellSize.Width = this.cellSize.Width + (4 - num + 1);
			}
		}

		private int GetMaxTextHeight()
		{
			int num = int.MinValue;
			foreach (Size size in this.textSizes.Values)
			{
				num = Math.Max(num, size.Height);
			}
			return num;
		}

		private void sunMoonControl_Paint(object sender, PaintEventArgs e)
		{
			for (int i = this.GetStartVisibleFullHCell(); i <= this.GetEndVisibleFullHCell() + 1; i++)
			{
				if (!this.ShowIntervals || i % 4 == 0)
				{
					int hour = (this.ShowIntervals ? (i / 4) : i) % 24;
					Icon hourIcon = this.GetHourIcon(hour);
					if (hourIcon != null)
					{
						Rectangle rectangle = new Rectangle(this.GetLeftPaddingOfSunMoonControl() + (i - this.GetStartVisibleFullHCell()) * this.cellSize.Width, 0, ScheduleEditor.iconSize.Width, ScheduleEditor.iconSize.Height);
						e.Graphics.DrawIcon(hourIcon, LayoutHelper.MirrorRectangle(rectangle, this.sunMoonControl));
					}
				}
			}
		}

		private int GetLeftPaddingOfSunMoonControl()
		{
			return this.dayButtonsPanel.Width - ScheduleEditor.iconSize.Width / 2 - 1;
		}

		private int GetRightOffSetOfSunMoonControl()
		{
			return ScheduleEditor.iconSize.Width - ScheduleEditor.iconSize.Width / 2 - 1;
		}

		private Icon GetHourIcon(int hour)
		{
			if (hour == 0)
			{
				return Icons.Moon;
			}
			if (hour == 12)
			{
				return Icons.Sun;
			}
			return null;
		}

		private void gridHeaderControl_Paint(object sender, PaintEventArgs e)
		{
			for (int i = this.GetStartVisibleFullHCell(); i <= this.GetEndVisibleFullHCell() + 1; i++)
			{
				if (!this.ShowIntervals || i % 4 == 0)
				{
					int hour = (this.ShowIntervals ? (i / 4) : i) % 24;
					string hourText = this.GetHourText(hour);
					if (!string.IsNullOrEmpty(hourText))
					{
						Rectangle rectangle = new Rectangle(this.GetLeftPaddingOfGridHeaderControl() + (i - this.GetStartVisibleFullHCell()) * this.cellSize.Width, 0, this.cellSize.Width, this.GetMaxTextHeight());
						TextRenderer.DrawText(e.Graphics, hourText, this.hintLabel.Font, LayoutHelper.MirrorRectangle(rectangle, this.gridHeaderControl), this.hintLabel.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
					}
					else
					{
						Rectangle rectangle2 = new Rectangle(this.dayButtonsPanel.Width + (i - this.GetStartVisibleFullHCell()) * this.cellSize.Width - 1 - this.hourIndicatorSize.Width / 2 - 1, this.gridHeaderControl.Height / 2 - this.hourIndicatorSize.Height / 2 - 1, this.hourIndicatorSize.Width, this.hourIndicatorSize.Height);
						e.Graphics.DrawEllipse(this.hourIndicatorPen, LayoutHelper.MirrorRectangle(rectangle2, this.gridHeaderControl));
						e.Graphics.FillEllipse(this.hourIndicatorBrush, LayoutHelper.MirrorRectangle(rectangle2, this.gridHeaderControl));
					}
				}
			}
		}

		private int GetStartVisibleFullHCell()
		{
			int num = this.scrollPanel.HorizontalScroll.Value / this.cellSize.Width;
			if (LayoutHelper.IsRightToLeft(this))
			{
				int num2 = this.ShowIntervals ? 96 : 24;
				num = ((num2 <= this.GetMaxVisibleFullHCellsNumber()) ? 0 : (num2 - (num + this.GetMaxVisibleFullHCellsNumber())));
			}
			return num;
		}

		private int GetEndVisibleFullHCell()
		{
			return this.GetStartVisibleFullHCell() + this.GetMaxVisibleFullHCellsNumber() - 1;
		}

		private int GetLeftPaddingOfGridHeaderControl()
		{
			return this.dayButtonsPanel.Width - this.cellSize.Width / 2 - 1;
		}

		private int GetRightOffSetOfGridHeaderControl()
		{
			return this.cellSize.Width - this.cellSize.Width / 2 - 1;
		}

		private string GetHourText(int hour)
		{
			if (this.ShowIntervals)
			{
				return hour.ToString();
			}
			if (hour % 2 == 0)
			{
				return hour.ToString();
			}
			return null;
		}

		private void LayoutIntervalButtons()
		{
			base.SuspendLayout();
			this.scrollPanel.SuspendLayout();
			this.intervalButtonsPanel.SuspendLayout();
			try
			{
				if (this.intervalButtonsPanel.Controls.Count == 0)
				{
					for (int i = 0; i < 96; i++)
					{
						Button button = new ExchangeButton();
						button.Margin = new Padding(0);
						button.Padding = new Padding(0);
						button.FlatStyle = FlatStyle.System;
						button.Size = this.cellSize;
						button.TabStop = false;
						button.TabIndex = i;
						button.Tag = i;
						button.Dock = DockStyle.Top;
						button.Click += this.intervalButton_Click;
						button.MouseMove += this.intervalButton_MouseMove;
						button.MouseLeave += this.intervalButton_Leave;
						button.Enter += this.intervalButton_Enter;
						button.Leave += this.intervalButton_Leave;
						button.PreviewKeyDown += this.Button_PreviewKeyDown;
						button.KeyDown += this.intervalButton_KeyDown;
						this.intervalButtonsPanel.Controls.Add(button);
					}
				}
				for (int j = 0; j < 96; j++)
				{
					if (j < this.NumberOfHCells)
					{
						this.intervalButtonsPanel.Controls[j].Visible = true;
					}
					else
					{
						this.intervalButtonsPanel.Controls[j].Visible = false;
					}
				}
			}
			finally
			{
				this.intervalButtonsPanel.ResumeLayout(false);
				this.intervalButtonsPanel.PerformLayout();
				this.scrollPanel.ResumeLayout(false);
				this.scrollPanel.PerformLayout();
				base.ResumeLayout(false);
				base.PerformLayout();
			}
		}

		private Button GetFocusedIntervalButton()
		{
			foreach (object obj in this.intervalButtonsPanel.Controls)
			{
				Control control = (Control)obj;
				if (control.Visible && control.Focused)
				{
					return control as Button;
				}
			}
			return null;
		}

		private void intervalButton_MouseMove(object sender, MouseEventArgs e)
		{
			Button button = sender as Button;
			this.newMousePositionInScrollPanel.X = button.Left + e.X - this.scrollPanel.HorizontalScroll.Value;
			this.newMousePositionInScrollPanel.Y = button.Top + e.Y - this.scrollPanel.VerticalScroll.Value;
			if (!this.newMousePositionInScrollPanel.Equals(this.oldMousePositionInScrollPanel))
			{
				this.oldMousePositionInScrollPanel = this.newMousePositionInScrollPanel;
				this.UpdateHintText(new Point((int)button.Tag, -1));
			}
		}

		private void intervalButton_Enter(object sender, EventArgs e)
		{
			Button button = sender as Button;
			this.UpdateHintText(new Point((int)button.Tag, -1));
		}

		private void intervalButton_Leave(object sender, EventArgs e)
		{
			Button button = sender as Button;
			if (((Point)this.hintLabel.Tag).Equals(new Point((int)button.Tag, -1)))
			{
				this.ResetHintText(sender, e);
			}
		}

		private void intervalButton_Click(object sender, EventArgs e)
		{
			int num = (int)((Button)sender).Tag;
			if (this.ShowIntervals)
			{
				this.scheduleBuilder.SetStateOfInterval(num, !this.scheduleBuilder.GetStateOfInterval(num));
			}
			else
			{
				this.scheduleBuilder.SetStateOfHour(num, !this.scheduleBuilder.GetStateOfHour(num));
			}
			this.selectionEnd = (this.selectionStart = new Point(num, -1));
			this.gridControl.Invalidate(true);
		}

		private void Button_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			e.IsInputKey = (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || (sender.Equals(this.gridControl) && e.KeyCode == Keys.Space));
		}

		private void intervalButton_KeyDown(object sender, KeyEventArgs e)
		{
			Control control = sender as Control;
			if (((e.KeyCode == Keys.Left && !LayoutHelper.IsRightToLeft(this)) || (e.KeyCode == Keys.Right && LayoutHelper.IsRightToLeft(this))) && this.selectionEnd.X >= 0)
			{
				this.selectionEnd.X = this.selectionEnd.X - 1;
				this.selectionStart = this.selectionEnd;
				if (this.GetControlIndex(control) == 0)
				{
					this.SelectFirstChild(this.dayButtonsPanel);
					return;
				}
				this.InternalSelectNextControl(control, false);
				this.scrollPanel_Scroll(this.scrollPanel, new ScrollEventArgs(ScrollEventType.SmallIncrement, this.scrollPanel.HorizontalScroll.Value, ScrollOrientation.HorizontalScroll));
				return;
			}
			else
			{
				if (((e.KeyCode == Keys.Right && !LayoutHelper.IsRightToLeft(this)) || (e.KeyCode == Keys.Left && LayoutHelper.IsRightToLeft(this))) && this.selectionEnd.X < this.NumberOfHCells - 1)
				{
					this.selectionEnd.X = this.selectionEnd.X + 1;
					this.selectionStart = this.selectionEnd;
					this.InternalSelectNextControl(control, true);
					this.scrollPanel_Scroll(this.scrollPanel, new ScrollEventArgs(ScrollEventType.SmallIncrement, this.scrollPanel.HorizontalScroll.Value, ScrollOrientation.HorizontalScroll));
					return;
				}
				if (e.KeyCode == Keys.Down)
				{
					this.gridControl.Focus();
					this.selectionEnd.Y = this.selectionEnd.Y + 1;
					this.selectionStart = this.selectionEnd;
					this.EnsureCellVisible(this.selectionEnd);
					this.gridControl.Invalidate();
				}
				return;
			}
		}

		private void DayButton_KeyDown(object sender, KeyEventArgs e)
		{
			Control control = sender as Control;
			if ((e.KeyCode == Keys.Right && !LayoutHelper.IsRightToLeft(this)) || (e.KeyCode == Keys.Left && LayoutHelper.IsRightToLeft(this)))
			{
				if (this.GetControlIndex(control) == 0)
				{
					this.SelectFirstChild(this.intervalButtonsPanel);
				}
				else
				{
					this.gridControl.Focus();
				}
				this.selectionEnd.X = this.selectionEnd.X + 1;
				this.selectionStart = this.selectionEnd;
				this.EnsureCellVisible(this.selectionEnd);
				this.gridControl.Invalidate();
				return;
			}
			if (e.KeyCode == Keys.Up && this.selectionEnd.Y >= 0)
			{
				this.selectionEnd.Y = this.selectionEnd.Y - 1;
				this.selectionStart = this.selectionEnd;
				this.InternalSelectNextControl(control, false);
				return;
			}
			if (e.KeyCode == Keys.Down && this.selectionEnd.Y < this.NumberOfVCells - 1)
			{
				this.selectionEnd.Y = this.selectionEnd.Y + 1;
				this.selectionStart = this.selectionEnd;
				this.InternalSelectNextControl(control, true);
			}
		}

		private void InternalSelectNextControl(Control activeControl, bool forward)
		{
			if (activeControl.Parent != null)
			{
				int num = activeControl.Parent.Controls.IndexOf(activeControl);
				if (forward && num < activeControl.Parent.Controls.Count - 1)
				{
					num++;
				}
				else if (!forward && num > 0)
				{
					num--;
				}
				Control control = activeControl.Parent.Controls[num];
				if (control.Visible)
				{
					control.Focus();
				}
			}
		}

		private void SelectFirstChild(Control control)
		{
			for (int i = 0; i < control.Controls.Count; i++)
			{
				Control control2 = control.Controls[i];
				if (control2.Visible)
				{
					control2.Focus();
					return;
				}
			}
		}

		private int GetControlIndex(Control control)
		{
			if (control.Parent == null)
			{
				return -1;
			}
			return control.Parent.Controls.IndexOf(control);
		}

		private void gridControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (!this.gridControl.Focused)
			{
				this.gridControl.Select();
			}
			if (e.Button == MouseButtons.Left)
			{
				this.isKeyboardMoveCellFocus = false;
				this.gridControl.Capture = true;
				this.selectionStart = this.CellFromGridXY(e.X, e.Y);
				this.selectionEnd = this.selectionStart;
			}
		}

		private void gridControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.gridControl.Capture)
			{
				this.gridControl.Capture = false;
				this.selectionEnd = this.CellFromGridXY(e.X, e.Y);
				bool selectedCellsState = !this.GetCellState(this.selectionStart);
				this.SetSelectedCellsState(selectedCellsState);
				this.gridControl.Invalidate();
			}
		}

		private void gridControl_MouseLeave(object sender, EventArgs e)
		{
			Point point = (Point)this.hintLabel.Tag;
			if (point.X >= 0 && point.Y >= 0)
			{
				this.ResetHintText(sender, e);
			}
		}

		private void gridControl_MouseMove(object sender, MouseEventArgs e)
		{
			this.newMousePositionInScrollPanel.X = e.X - this.scrollPanel.HorizontalScroll.Value;
			this.newMousePositionInScrollPanel.Y = e.Y - this.scrollPanel.VerticalScroll.Value;
			if (!this.newMousePositionInScrollPanel.Equals(this.oldMousePositionInScrollPanel))
			{
				this.oldMousePositionInScrollPanel = this.newMousePositionInScrollPanel;
				if (this.gridControl.Capture)
				{
					this.selectionEnd = this.CellFromGridXY(e.X, e.Y);
					this.EnsureCellVisible(this.selectionEnd);
					this.gridControl.Invalidate(true);
				}
				this.UpdateHintText(this.CellFromGridXY(e.X, e.Y));
			}
		}

		private void SetCellRowStates(int startX, int endX, int y, bool state)
		{
			int num = Math.Min(startX, endX);
			int num2 = Math.Max(startX, endX);
			for (int i = num; i <= num2; i++)
			{
				this.SetCellState(new Point(i, y), state);
			}
		}

		private void SetCellColumnStates(int startY, int endY, int x, bool state)
		{
			int num = Math.Min(startY, endY);
			int num2 = Math.Max(startY, endY);
			for (int i = num; i <= num2; i++)
			{
				this.SetCellState(new Point(x, i), state);
			}
		}

		private void gridControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			e.IsInputKey = (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right);
		}

		private void gridControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up || e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
			{
				bool flag = false;
				if (e.Shift)
				{
					flag = this.GetCellState(this.selectionStart);
					if (this.selectionEnd == this.selectionStart && this.isKeyboardMoveCellFocus)
					{
						flag = !flag;
						this.SetCellState(this.selectionStart, flag);
					}
				}
				if (e.KeyCode == Keys.Up)
				{
					if (this.selectionEnd.Y >= 0 && !e.Shift)
					{
						this.selectionEnd.Y = this.selectionEnd.Y - 1;
						if (this.selectionEnd.Y == -1)
						{
							this.intervalButtonsPanel.Controls[this.selectionEnd.X].Focus();
						}
					}
					else if (e.Shift && this.selectionEnd.Y > 0)
					{
						this.selectionEnd.Y = this.selectionEnd.Y - 1;
						if (this.selectionEnd.Y >= this.selectionStart.Y)
						{
							flag = !flag;
							this.SetCellRowStates(this.selectionStart.X, this.selectionEnd.X, this.selectionEnd.Y + 1, flag);
						}
						else
						{
							this.SetCellRowStates(this.selectionStart.X, this.selectionEnd.X, this.selectionEnd.Y, flag);
						}
					}
				}
				else if (e.KeyCode == Keys.Down && this.selectionEnd.Y < this.NumberOfVCells - 1)
				{
					this.selectionEnd.Y = this.selectionEnd.Y + 1;
					if (e.Shift)
					{
						if (this.selectionStart.Y >= this.selectionEnd.Y)
						{
							flag = !flag;
							this.SetCellRowStates(this.selectionStart.X, this.selectionEnd.X, this.selectionEnd.Y - 1, flag);
						}
						else
						{
							this.SetCellRowStates(this.selectionStart.X, this.selectionEnd.X, this.selectionEnd.Y, flag);
						}
					}
				}
				else if ((e.KeyCode == Keys.Left && !LayoutHelper.IsRightToLeft(this)) || (e.KeyCode == Keys.Right && LayoutHelper.IsRightToLeft(this)))
				{
					if (this.selectionEnd.X >= 0 && !e.Shift)
					{
						this.selectionEnd.X = this.selectionEnd.X - 1;
						if (this.selectionEnd.X == -1)
						{
							this.dayButtonsPanel.Controls[this.selectionEnd.Y + 1].Focus();
						}
					}
					if (e.Shift && this.selectionEnd.X > 0)
					{
						this.selectionEnd.X = this.selectionEnd.X - 1;
						if (this.selectionEnd.X >= this.selectionStart.X)
						{
							flag = !flag;
							this.SetCellColumnStates(this.selectionStart.Y, this.selectionEnd.Y, this.selectionEnd.X + 1, flag);
						}
						else
						{
							this.SetCellColumnStates(this.selectionStart.Y, this.selectionEnd.Y, this.selectionEnd.X, flag);
						}
					}
				}
				else if (((e.KeyCode == Keys.Right && !LayoutHelper.IsRightToLeft(this)) || (e.KeyCode == Keys.Left && LayoutHelper.IsRightToLeft(this))) && this.selectionEnd.X < this.NumberOfHCells - 1)
				{
					this.selectionEnd.X = this.selectionEnd.X + 1;
					if (e.Shift)
					{
						if (this.selectionStart.X >= this.selectionEnd.X)
						{
							flag = !flag;
							this.SetCellColumnStates(this.selectionStart.Y, this.selectionEnd.Y, this.selectionEnd.X - 1, flag);
						}
						else
						{
							this.SetCellColumnStates(this.selectionStart.Y, this.selectionEnd.Y, this.selectionEnd.X, flag);
						}
					}
				}
				if (!e.Shift)
				{
					this.selectionStart = this.selectionEnd;
					this.isKeyboardMoveCellFocus = true;
				}
				this.EnsureCellVisible(this.selectionEnd);
				this.UpdateHintText(this.selectionEnd);
				this.gridControl.Invalidate();
				e.Handled = true;
				return;
			}
			if (e.KeyCode == Keys.Space)
			{
				this.SetSelectedCellsState(!this.GetCellState(this.selectionEnd));
				this.gridControl.Invalidate();
				e.Handled = true;
			}
		}

		private void SetFocus()
		{
			if (this.dayButtonsPanel.Controls[0].Capture)
			{
				return;
			}
			if (this.selectionEnd.X == -1)
			{
				this.dayButtonsPanel.Controls[this.selectionEnd.Y + 1].Focus();
				return;
			}
			if (this.selectionEnd.Y == -1 && this.selectionEnd.X != -1)
			{
				this.intervalButtonsPanel.Controls[this.selectionEnd.X].Focus();
				return;
			}
			this.gridControl.Focus();
			this.gridControl.Invalidate();
		}

		private void gridControl_Paint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			int num = this.ShowIntervals ? 1 : 4;
			for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
			{
				for (int i = 0; i < 96; i++)
				{
					if (this.scheduleBuilder.GetStateOfInterval(dayOfWeek, i))
					{
						Rectangle rectangle = new Rectangle(i * this.cellSize.Width / num, (int)(dayOfWeek * (DayOfWeek)this.cellSize.Height), this.cellSize.Width / num, this.cellSize.Height);
						graphics.FillRectangle(this.gridBrush, LayoutHelper.MirrorRectangle(rectangle, this.gridControl));
					}
				}
			}
			int x = this.gridControl.Width - 1;
			for (int j = 1; j <= 7; j++)
			{
				int num2 = j * this.cellSize.Height - 1;
				graphics.DrawLine(SystemPens.ControlDarkDark, 0, num2, x, num2);
			}
			int y = this.gridControl.Height - 1;
			for (int k = 1; k < 97; k++)
			{
				int num3 = LayoutHelper.MirrorPosition(k * this.cellSize.Width - 1, this.gridControl);
				if (LayoutHelper.IsRightToLeft(this))
				{
					num3--;
				}
				graphics.DrawLine(SystemPens.ControlDarkDark, num3, 0, num3, y);
			}
			if (this.gridControl.Focused)
			{
				Point selectionTopLeft = this.SelectionTopLeft;
				Point selectionRightBottom = this.SelectionRightBottom;
				Rectangle rectangle2 = Rectangle.FromLTRB(selectionTopLeft.X * this.cellSize.Width, selectionTopLeft.Y * this.cellSize.Height, (selectionRightBottom.X + 1) * this.cellSize.Width - 1, (selectionRightBottom.Y + 1) * this.cellSize.Height - 1);
				ControlPaint.DrawFocusRectangle(graphics, LayoutHelper.MirrorRectangle(rectangle2, this.gridControl));
			}
		}

		private void hintLabel_TextChanged(object sender, EventArgs e)
		{
			this.hintLabel.Invalidate();
		}

		private void hintLabel_Paint(object sender, PaintEventArgs e)
		{
			TextRenderer.DrawText(e.Graphics, this.hintLabel.Text, this.hintLabel.Font, this.hintLabel.ClientRectangle, this.hintLabel.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
		}

		private void scrollPanel_Scroll(object sender, ScrollEventArgs e)
		{
			if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
			{
				int num = e.NewValue / this.cellSize.Width + ((e.NewValue % this.cellSize.Width == 0) ? 0 : 1);
				this.scrollPanel.AutoScrollPosition = new Point(num * this.cellSize.Width, this.scrollPanel.AutoScrollPosition.Y);
				Button focusedIntervalButton = this.GetFocusedIntervalButton();
				if (focusedIntervalButton != null)
				{
					Button button = focusedIntervalButton;
					if (this.GetStartVisibleFullHCell() > (int)focusedIntervalButton.Tag)
					{
						button = (this.intervalButtonsPanel.Controls[this.GetStartVisibleFullHCell()] as Button);
					}
					else if (this.GetEndVisibleFullHCell() < (int)focusedIntervalButton.Tag)
					{
						button = (this.intervalButtonsPanel.Controls[this.GetEndVisibleFullHCell()] as Button);
					}
					button.Focus();
				}
				this.gridHeaderControl.Invalidate(true);
				this.sunMoonControl.Invalidate(true);
			}
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			this.sunMoonControl.Height = ScheduleEditor.iconSize.Height;
			this.gridHeaderControl.Height = this.GetMaxTextHeight();
			this.intervalButtonsPanel.Top = 0;
			this.intervalButtonsPanel.Height = this.cellSize.Height;
			this.intervalButtonsPanel.Width = this.cellSize.Width * this.NumberOfHCells;
			this.gridControl.Top = this.intervalButtonsPanel.Bottom;
			this.gridControl.Height = this.cellSize.Height * this.NumberOfVCells;
			this.gridControl.Width = this.intervalButtonsPanel.Width;
			this.scrollPanel.Height = this.intervalButtonsPanel.Height + this.gridControl.Height + SystemInformation.HorizontalScrollBarHeight;
			this.scrollPanel.HorizontalScroll.SmallChange = this.cellSize.Width;
			this.hintLabel.Height = this.cellSize.Height;
			base.OnLayout(e);
			this.gridHeaderControl.Invalidate(true);
			this.sunMoonControl.Invalidate(true);
			this.gridControl.Invalidate(true);
		}

		private void AdjustScrollpanel()
		{
			int right = (this.scrollPanel.Width + this.scrollPanel.Margin.Right) % this.cellSize.Width;
			this.scrollPanel.Margin = new Padding(0, 0, right, 0);
		}

		private int GetMaxVisibleFullHCellsNumber()
		{
			return Math.Min(this.scrollPanel.Width / this.cellSize.Width, this.ShowIntervals ? 96 : 24);
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			return this.contentPanel.GetPreferredSize(proposedSize);
		}

		private void UpdateHintText(Point cell)
		{
			bool flag = -1 != cell.Y;
			bool flag2 = -1 != cell.X;
			StringBuilder stringBuilder = new StringBuilder();
			if (flag)
			{
				stringBuilder.Append(ExchangeUserControl.RemoveAccelerator(ScheduleEditor.dayTexts[cell.Y]));
				if (flag2)
				{
					stringBuilder.Append(" ");
				}
			}
			if (flag2)
			{
				DateTime dateTime;
				if (this.ShowIntervals)
				{
					dateTime = new DateTime(1, 1, 1, cell.X / 4, 15 * (cell.X % 4), 0);
				}
				else
				{
					dateTime = new DateTime(1, 1, 1, cell.X, 0, 0);
				}
				stringBuilder.Append(dateTime.ToShortTimeString());
			}
			this.hintLabel.Text = stringBuilder.ToString();
			this.hintLabel.Tag = cell;
		}

		private void ResetHintText(object sender, EventArgs e)
		{
			this.hintLabel.Text = string.Empty;
			this.hintLabel.Tag = new Point(-1, -1);
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Schedule Schedule
		{
			get
			{
				return this.scheduleBuilder.Schedule;
			}
			set
			{
				if (this.scheduleBuilder == null || this.scheduleBuilder.Schedule != value)
				{
					if (value != null)
					{
						this.scheduleBuilder = new ScheduleBuilder(value);
					}
					else
					{
						this.scheduleBuilder = new ScheduleBuilder(Schedule.Never);
					}
					this.gridControl.Invalidate(true);
				}
			}
		}

		[DefaultValue(false)]
		public bool ShowIntervals
		{
			get
			{
				return this.showIntervals;
			}
			set
			{
				if (this.ShowIntervals != value)
				{
					this.showIntervals = value;
					this.hourMenuItem.Checked = !this.ShowIntervals;
					this.intervalMenuItem.Checked = this.ShowIntervals;
					this.LayoutIntervalButtons();
					this.AdjustScrollpanel();
					if (this.ShowIntervals)
					{
						this.selectionStart = new Point(this.selectionStart.X * 4, this.selectionStart.Y);
						this.selectionEnd = new Point(this.selectionEnd.X * 4 + 4 - 1, this.selectionEnd.Y);
					}
					else
					{
						this.selectionStart = new Point(this.selectionStart.X / 4, this.selectionStart.Y);
						this.selectionEnd = new Point(this.selectionEnd.X / 4, this.selectionEnd.Y);
					}
					this.gridHeaderControl.Invalidate(true);
					this.sunMoonControl.Invalidate(true);
					this.gridControl.Invalidate(true);
					this.EnsureCellVisible(this.selectionEnd);
				}
			}
		}

		private int NumberOfHCells
		{
			get
			{
				if (!this.ShowIntervals)
				{
					return 24;
				}
				return 96;
			}
		}

		private int NumberOfVCells
		{
			get
			{
				return 7;
			}
		}

		private Point CellFromGridXY(int x, int y)
		{
			return new Point(Math.Max(0, Math.Min(LayoutHelper.MirrorPosition(x, this.gridControl) / this.cellSize.Width, this.NumberOfHCells - 1)), Math.Max(0, Math.Min(y / this.cellSize.Height, this.NumberOfVCells - 1)));
		}

		private void EnsureCellVisible(Point cell)
		{
			if (cell.X >= 0 && cell.X < this.intervalButtonsPanel.Controls.Count)
			{
				this.scrollPanel.ScrollControlIntoView(this.intervalButtonsPanel.Controls[cell.X]);
				this.scrollPanel_Scroll(this.scrollPanel, new ScrollEventArgs(ScrollEventType.SmallIncrement, this.scrollPanel.HorizontalScroll.Value, ScrollOrientation.HorizontalScroll));
			}
		}

		private Point SelectionTopLeft
		{
			get
			{
				return new Point(Math.Min(this.selectionStart.X, this.selectionEnd.X), Math.Min(this.selectionStart.Y, this.selectionEnd.Y));
			}
		}

		private Point SelectionRightBottom
		{
			get
			{
				return new Point(Math.Max(this.selectionStart.X, this.selectionEnd.X), Math.Max(this.selectionStart.Y, this.selectionEnd.Y));
			}
		}

		private IEnumerable SelectedCells
		{
			get
			{
				Point selectionTopLeft = this.SelectionTopLeft;
				Point selectionRightBottom = this.SelectionRightBottom;
				for (int day = selectionTopLeft.Y; day <= selectionRightBottom.Y; day++)
				{
					for (int interval = selectionTopLeft.X; interval <= selectionRightBottom.X; interval++)
					{
						yield return new Point(interval, day);
					}
				}
				yield break;
			}
		}

		private bool GetCellState(Point cell)
		{
			if (!this.ShowIntervals)
			{
				return this.scheduleBuilder.GetStateOfHour((DayOfWeek)cell.Y, cell.X);
			}
			return this.scheduleBuilder.GetStateOfInterval((DayOfWeek)cell.Y, cell.X);
		}

		private void SetCellState(Point cell, bool state)
		{
			if (this.ShowIntervals)
			{
				this.scheduleBuilder.SetStateOfInterval((DayOfWeek)cell.Y, cell.X, state);
				return;
			}
			this.scheduleBuilder.SetStateOfHour((DayOfWeek)cell.Y, cell.X, state);
		}

		private void SetSelectedCellsState(bool cellState)
		{
			foreach (object obj in this.SelectedCells)
			{
				Point cell = (Point)obj;
				this.SetCellState(cell, cellState);
			}
		}

		private Point selectionStart;

		private Point selectionEnd;

		private bool isKeyboardMoveCellFocus;

		private AutoTableLayoutPanel contentPanel;

		private OwnerDrawControl hintLabel;

		private Panel scrollPanel;

		private OwnerDrawControl gridControl;

		private FlowLayoutPanel intervalButtonsPanel;

		private TableLayoutPanel dayButtonsPanel;

		private OwnerDrawControl gridHeaderControl;

		private OwnerDrawControl sunMoonControl;

		private Panel detailViewPanel;

		private TableLayoutPanel detailViewTableLayoutPanel;

		private AutoHeightRadioButton intervalRadioButton;

		private AutoHeightRadioButton hourRadioButton;

		private GroupBox detailViewGroupBox;

		private static string[] dayTexts;

		private MenuItem hourMenuItem;

		private MenuItem intervalMenuItem;

		private ContextMenu contextMenu;

		private Size cellSize = new Size(13, 0);

		private Dictionary<string, Size> textSizes = new Dictionary<string, Size>();

		private Size hourIndicatorSize = new Size(3, 3);

		private static Size iconSize = new Size(16, 16);

		private SolidBrush hourIndicatorBrush;

		private Pen hourIndicatorPen;

		private SolidBrush gridBrush;

		private Point newMousePositionInScrollPanel = new Point(-1, -1);

		private Point oldMousePositionInScrollPanel = new Point(-1, -1);

		private ScheduleBuilder scheduleBuilder;

		private bool showIntervals;
	}
}
