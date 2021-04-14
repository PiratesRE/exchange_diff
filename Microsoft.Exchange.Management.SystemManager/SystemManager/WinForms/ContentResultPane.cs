using System;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class ContentResultPane : ResultPane
	{
		public ContentResultPane()
		{
			base.Name = "ContentResultPane";
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys)131139)
			{
				this.CopyContentToClipBoard();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void CopyContentToClipBoard()
		{
			using (new ControlWaitCursor(this))
			{
				WinformsHelper.SetDataObjectToClipboard(this.GetContent(), true);
			}
		}

		protected virtual string GetContent()
		{
			return string.Empty;
		}
	}
}
