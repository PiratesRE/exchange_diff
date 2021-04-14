using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExtendedDateTimePicker : DateTimePicker, IBulkEditor, IOwnerDrawBulkEditSupport, IBulkEditSupport
	{
		public ExtendedDateTimePicker()
		{
			base.Name = "ExtendedDateTimePicker";
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

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<PropertyChangedEventArgs> UserModified;

		private void OnUserModified(EventArgs e)
		{
			if (this.UserModified != null)
			{
				this.UserModified(this, new PropertyChangedEventArgs("Value"));
			}
		}

		BulkEditorAdapter IBulkEditor.BulkEditorAdapter
		{
			get
			{
				if (this.bulkEditorAdapter == null)
				{
					this.bulkEditorAdapter = new DateTimePickerBulkEditorAdapter(this);
				}
				return this.bulkEditorAdapter;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler Painted;

		private void OnPainted(EventArgs e)
		{
			if (this.Painted != null)
			{
				this.Painted(this, e);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		private DateTimePickerBulkEditorAdapter bulkEditorAdapter;
	}
}
