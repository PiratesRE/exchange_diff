using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;
using Microsoft.ManagementGUI.WinForms.Design;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Designer(typeof(AutoHeightControlDesigner))]
	public class AutoHeightRadioButton : RadioButton, IBulkEditor, IButtonBaseBulkEditSupport, IOwnerDrawBulkEditSupport, IBulkEditSupport
	{
		public AutoHeightRadioButton()
		{
			this.TextAlign = ContentAlignment.TopLeft;
			this.CheckAlign = ContentAlignment.TopLeft;
			this.AutoSize = true;
		}

		[DefaultValue(true)]
		public new bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
			}
		}

		[DefaultValue(ContentAlignment.TopLeft)]
		public override ContentAlignment TextAlign
		{
			get
			{
				return base.TextAlign;
			}
			set
			{
				base.TextAlign = value;
			}
		}

		[DefaultValue(ContentAlignment.TopLeft)]
		public new ContentAlignment CheckAlign
		{
			get
			{
				return base.CheckAlign;
			}
			set
			{
				base.CheckAlign = value;
			}
		}

		private void CacheTextSize()
		{
			this.preferredSizeHash.Clear();
			if (string.IsNullOrEmpty(this.Text))
			{
				this.cachedSizeOfOneLineOfText = Size.Empty;
				return;
			}
			this.cachedSizeOfOneLineOfText = TextRenderer.MeasureText(this.Text, this.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			this.CacheTextSize();
			base.OnTextChanged(e);
		}

		protected override void OnFontChanged(EventArgs e)
		{
			this.CacheTextSize();
			base.OnFontChanged(e);
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size = base.GetPreferredSize(proposedSize);
			if (size.Width > proposedSize.Width && !string.IsNullOrEmpty(this.Text) && !proposedSize.Height.Equals(2147483647))
			{
				Size size2 = size - this.cachedSizeOfOneLineOfText;
				Size size3 = proposedSize - size2;
				if (!this.preferredSizeHash.ContainsKey(size3))
				{
					size3.Width = ((size3.Width > 0) ? size3.Width : (base.Size.Width - size2.Width));
					size3.Height = ((size3.Height > 0) ? size3.Height : int.MaxValue);
					size = size2 + TextRenderer.MeasureText(this.Text, this.Font, size3, TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
					this.preferredSizeHash[size3] = size;
				}
				else
				{
					size = this.preferredSizeHash[size3];
				}
			}
			return size;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public event EventHandler Painted;

		private void OnPainted(EventArgs e)
		{
			if (this.Painted != null)
			{
				this.Painted(this, e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public event EventHandler FocusSetted;

		private void OnFocusSetted(EventArgs e)
		{
			if (this.FocusSetted != null)
			{
				this.FocusSetted(this, e);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler FocusKilled;

		private void OnFocusKilled(EventArgs e)
		{
			if (this.FocusKilled != null)
			{
				this.FocusKilled(this, e);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<PropertyChangedEventArgs> UserModified;

		private void OnUserModified(EventArgs e)
		{
			if (this.UserModified != null)
			{
				this.UserModified(this, new PropertyChangedEventArgs("Checked"));
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event HandledEventHandler Entering;

		private void OnEntering(HandledEventArgs e)
		{
			if (this.Entering != null)
			{
				this.Entering(this, e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public event HandledEventHandler CheckedChangedRaising;

		private void OnCheckedChangedRaising(HandledEventArgs e)
		{
			if (this.CheckedChangedRaising != null)
			{
				this.CheckedChangedRaising(this, e);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CheckedValue
		{
			get
			{
				return base.Checked;
			}
			set
			{
				base.Checked = value;
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg == 15)
			{
				this.OnPainted(EventArgs.Empty);
				return;
			}
			if (m.Msg == 7)
			{
				this.OnFocusSetted(EventArgs.Empty);
				return;
			}
			if (m.Msg == 8)
			{
				this.OnFocusKilled(EventArgs.Empty);
			}
		}

		protected override void OnEnter(EventArgs e)
		{
			HandledEventArgs handledEventArgs = new HandledEventArgs();
			this.OnEntering(handledEventArgs);
			if (!handledEventArgs.Handled)
			{
				base.OnEnter(e);
			}
		}

		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessMnemonic(char charCode)
		{
			bool flag = base.ProcessMnemonic(charCode);
			if (flag)
			{
				base.PerformClick();
			}
			return flag;
		}

		protected override void OnClick(EventArgs e)
		{
			this.OnUserModified(EventArgs.Empty);
			base.OnClick(e);
		}

		protected override void OnCheckedChanged(EventArgs e)
		{
			HandledEventArgs handledEventArgs = new HandledEventArgs();
			this.OnCheckedChangedRaising(handledEventArgs);
			if (!handledEventArgs.Handled)
			{
				base.OnCheckedChanged(e);
			}
		}

		BulkEditorAdapter IBulkEditor.BulkEditorAdapter
		{
			get
			{
				if (this.bulkEditorAdapter == null)
				{
					this.bulkEditorAdapter = new RadioButtonBulkEditorAdapter(this);
				}
				return this.bulkEditorAdapter;
			}
		}

		[DefaultValue(false)]
		public bool BulkEditDefaultChecked { get; set; }

		private Size cachedSizeOfOneLineOfText = Size.Empty;

		private Dictionary<Size, Size> preferredSizeHash = new Dictionary<Size, Size>(3);

		private RadioButtonBulkEditorAdapter bulkEditorAdapter;
	}
}
