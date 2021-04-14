using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class MessageBoxDialog : Form
	{
		[Obsolete("Use of paramless constructor can cause serious performance problem. Use the other one instead.")]
		public MessageBoxDialog() : this(string.Empty, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
		{
		}

		public MessageBoxDialog(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			this.InitializeComponent();
			this.messageScrollLabelAutoSizePanel.SuspendLayout();
			this.buttonsPanel.SuspendLayout();
			this.messagePanel.SuspendLayout();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.messageLabel.UseCompatibleTextRendering = true;
			this.messageScrollLabel.UseCompatibleTextRendering = true;
			this.captionLabel.UseCompatibleTextRendering = true;
			this.noButton.UseCompatibleTextRendering = true;
			this.yesButton.UseCompatibleTextRendering = true;
			this.cancelButton.UseCompatibleTextRendering = true;
			this.okButton.UseCompatibleTextRendering = true;
			this.okButton.Text = Strings.Ok;
			this.yesButton.Text = Strings.Yes;
			this.noButton.Text = Strings.No;
			this.cancelButton.Text = Strings.Cancel;
			this.ScrollBars = ScrollBars.None;
			this.Message = message;
			this.Text = caption;
			this.Buttons = buttons;
			this.Icon = icon;
			this.DefaultButton = defaultButton;
			this.messageScrollLabelAutoSizePanel.ResumeLayout(false);
			this.messageScrollLabelAutoSizePanel.PerformLayout();
			this.buttonsPanel.ResumeLayout(false);
			this.buttonsPanel.PerformLayout();
			this.messagePanel.ResumeLayout(false);
			this.messagePanel.PerformLayout();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public override RightToLeft RightToLeft
		{
			get
			{
				if (!LayoutHelper.CultureInfoIsRightToLeft)
				{
					return RightToLeft.No;
				}
				return RightToLeft.Yes;
			}
			set
			{
			}
		}

		public override bool RightToLeftLayout
		{
			get
			{
				return LayoutHelper.IsRightToLeft(this);
			}
			set
			{
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.SelectDefaultButton();
		}

		[DefaultValue(ScrollBars.None)]
		public ScrollBars ScrollBars
		{
			get
			{
				return this.scrollBars;
			}
			private set
			{
				switch (value)
				{
				case ScrollBars.None:
					this.autoScrollPanel.Size = Size.Empty;
					this.messagePanel.ColumnStyles[0].Width = 100f;
					this.messageLabel.Visible = true;
					this.messagePanel.ColumnStyles[1].Width = 0f;
					this.autoScrollPanel.Visible = false;
					goto IL_F5;
				case ScrollBars.Vertical:
					this.messagePanel.ColumnStyles[0].Width = 0f;
					this.messageLabel.Visible = false;
					this.messagePanel.ColumnStyles[1].Width = 100f;
					this.autoScrollPanel.Visible = true;
					this.autoScrollPanel.Size = this.messageLabel.MaximumSize;
					goto IL_F5;
				}
				throw new InvalidEnumArgumentException("ScrollBars", (int)value, typeof(ScrollBars));
				IL_F5:
				this.scrollBars = value;
			}
		}

		[DefaultValue("")]
		public string Caption
		{
			get
			{
				return this.Text;
			}
			set
			{
				this.Text = value;
				this.captionLabel.Text = value;
			}
		}

		[DefaultValue("")]
		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
				StringBuilder stringBuilder = new StringBuilder();
				using (StringReader stringReader = new StringReader(this.Message))
				{
					int num = 0;
					string value2;
					while ((value2 = stringReader.ReadLine()) != null)
					{
						num++;
						if (num > 64)
						{
							stringBuilder.AppendLine();
							stringBuilder.Append(Strings.TooManyMessages);
							break;
						}
						if (num > 1)
						{
							stringBuilder.AppendLine();
						}
						stringBuilder.Append(value2);
					}
					this.messageLabel.Text = stringBuilder.ToString();
					this.messageScrollLabel.Text = this.messageLabel.Text;
					if (this.messageLabel.PreferredHeight >= this.messageLabel.MaximumSize.Height && this.ScrollBars != ScrollBars.Vertical)
					{
						this.ScrollBars = ScrollBars.Vertical;
					}
					else if (this.messageLabel.PreferredHeight < this.messageLabel.MaximumSize.Height && this.ScrollBars != ScrollBars.None)
					{
						this.ScrollBars = ScrollBars.None;
					}
				}
			}
		}

		[DefaultValue(MessageBoxDefaultButton.Button1)]
		public MessageBoxDefaultButton DefaultButton
		{
			get
			{
				return this.defaultButton;
			}
			set
			{
				if (!Enum.IsDefined(typeof(MessageBoxDefaultButton), value))
				{
					throw new InvalidEnumArgumentException("DefaultButton", (int)value, typeof(MessageBoxDefaultButton));
				}
				if (value != this.DefaultButton)
				{
					this.defaultButton = value;
					if (base.IsHandleCreated)
					{
						this.SelectDefaultButton();
					}
				}
			}
		}

		[DefaultValue(MessageBoxIcon.None)]
		public new MessageBoxIcon Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (value <= MessageBoxIcon.Hand)
				{
					if (value == MessageBoxIcon.None)
					{
						goto IL_CA;
					}
					if (value == MessageBoxIcon.Hand)
					{
						this.iconPictureBox.Image = IconLibrary.ToBitmap(Icons.Error, this.iconPictureBox.Size);
						goto IL_CA;
					}
				}
				else
				{
					if (value == MessageBoxIcon.Question)
					{
						this.iconPictureBox.Image = IconLibrary.ToBitmap(Icons.Help, this.iconPictureBox.Size);
						goto IL_CA;
					}
					if (value == MessageBoxIcon.Exclamation)
					{
						this.iconPictureBox.Image = IconLibrary.ToBitmap(Icons.Warning, this.iconPictureBox.Size);
						goto IL_CA;
					}
					if (value == MessageBoxIcon.Asterisk)
					{
						this.iconPictureBox.Image = IconLibrary.ToBitmap(Icons.Information, this.iconPictureBox.Size);
						goto IL_CA;
					}
				}
				throw new InvalidEnumArgumentException("Icon", (int)value, typeof(MessageBoxIcon));
				IL_CA:
				this.iconPictureBox.Visible = (value != MessageBoxIcon.None);
				this.icon = value;
			}
		}

		[DefaultValue(MessageBoxButtons.OK)]
		public MessageBoxButtons Buttons
		{
			get
			{
				return this.buttons;
			}
			set
			{
				List<Button> list = new List<Button>();
				switch (value)
				{
				case MessageBoxButtons.OK:
				case MessageBoxButtons.OKCancel:
					list.Add(this.okButton);
					if (MessageBoxButtons.OKCancel == value)
					{
						list.Add(this.cancelButton);
						goto IL_82;
					}
					goto IL_82;
				case MessageBoxButtons.YesNoCancel:
				case MessageBoxButtons.YesNo:
					list.Add(this.yesButton);
					list.Add(this.noButton);
					if (MessageBoxButtons.YesNoCancel == value)
					{
						list.Add(this.cancelButton);
						goto IL_82;
					}
					goto IL_82;
				}
				throw new InvalidEnumArgumentException("Buttons", (int)value, typeof(MessageBoxButtons));
				IL_82:
				this.btnControls = list.ToArray();
				this.SetButtonsAndColumnStyles();
				this.EnableXButton(value != MessageBoxButtons.YesNo);
				this.buttons = value;
			}
		}

		private void SelectDefaultButton()
		{
			int num = (int)(this.DefaultButton / MessageBoxDefaultButton.Button2);
			if (this.btnControls.Length >= num + 1)
			{
				this.btnControls[num].Select();
			}
		}

		private void EnableXButton(bool enabled)
		{
			this.isCloseBoxDisabled = !enabled;
			IntPtr systemMenu = UnsafeNativeMethods.GetSystemMenu(new HandleRef(this, base.Handle), false);
			int num = enabled ? 0 : 1;
			UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, systemMenu), 61536, num);
		}

		private void SetButtonsAndColumnStyles()
		{
			for (int i = 0; i < this.buttonsPanel.ColumnStyles.Count; i++)
			{
				this.buttonsPanel.ColumnStyles[i].SizeType = SizeType.Absolute;
				this.buttonsPanel.ColumnStyles[i].Width = 0f;
				Button button;
				if ((button = (this.buttonsPanel.GetControlFromPosition(i, 0) as Button)) != null)
				{
					button.Visible = false;
				}
			}
			foreach (Button button2 in this.btnControls)
			{
				int column;
				if (-1 != (column = this.buttonsPanel.GetColumn(button2)))
				{
					this.buttonsPanel.ColumnStyles[column].SizeType = SizeType.Percent;
					this.buttonsPanel.ColumnStyles[column].Width = 100f / (float)this.btnControls.Length;
					if (button2 != this.btnControls[this.btnControls.Length - 1])
					{
						this.buttonsPanel.ColumnStyles[column + 1].SizeType = SizeType.Absolute;
						this.buttonsPanel.ColumnStyles[column + 1].Width = 8f;
					}
					button2.Visible = true;
				}
			}
		}

		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (this.Buttons == MessageBoxButtons.OK && (keyData & (Keys.Control | Keys.Alt)) == Keys.None && (keyData & Keys.KeyCode) == Keys.Escape)
			{
				base.DialogResult = DialogResult.Cancel;
				return true;
			}
			return (this.isCloseBoxDisabled && (keyData & Keys.Alt) == Keys.Alt && (keyData & Keys.Control) == Keys.None && (keyData & Keys.KeyCode) == Keys.F4) || base.ProcessDialogKey(keyData);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys)131139)
			{
				WinformsHelper.SetDataObjectToClipboard(this.Content, true);
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		public string Content
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("---------------------------");
				stringBuilder.AppendLine(this.Caption);
				stringBuilder.AppendLine("---------------------------");
				stringBuilder.AppendLine(this.Message);
				stringBuilder.AppendLine("---------------------------");
				foreach (Button button in this.btnControls)
				{
					stringBuilder.Append(button.Text.Replace("&", "") + "   ");
				}
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("---------------------------");
				return stringBuilder.ToString();
			}
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			if (MessageBoxDialog.Test_FormShown != null)
			{
				MessageBoxDialog.Test_FormShown(this, EventArgs.Empty);
			}
		}

		public static event EventHandler Test_FormShown;

		private const int MaximumLinesNumberOfShowedMessage = 64;

		private Button[] btnControls;

		private bool isCloseBoxDisabled;

		private ScrollBars scrollBars;

		private string message = string.Empty;

		private MessageBoxDefaultButton defaultButton;

		private MessageBoxIcon icon;

		private MessageBoxButtons buttons;
	}
}
