using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class WorkPaneTabs : TabControl
	{
		public AbstractResultPane SelectedResultPane
		{
			get
			{
				AbstractResultPane result = null;
				WorkPanePage workPanePage = base.SelectedTab as WorkPanePage;
				if (workPanePage != null)
				{
					result = workPanePage.ResultPane;
				}
				return result;
			}
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			WorkPanePage workPanePage = e.Control as WorkPanePage;
			if (workPanePage.ResultPane != null)
			{
				if (workPanePage.ResultPane.Icon != null)
				{
					workPanePage.ImageIndex = this.AddNewImageIcon(workPanePage.ResultPane.Icon);
				}
				workPanePage.ResultPane.IconChanged += this.ResultPane_IconChanged;
			}
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			WorkPanePage workPanePage = e.Control as WorkPanePage;
			if (workPanePage.ResultPane != null)
			{
				if (workPanePage.ResultPane.Icon != null)
				{
					this.ImageIcons.Icons.Remove(workPanePage.ResultPane.GetHashCode().ToString());
				}
				workPanePage.ResultPane.IconChanged -= this.ResultPane_IconChanged;
			}
			base.OnControlRemoved(e);
		}

		private void ResultPane_IconChanged(object sender, EventArgs e)
		{
			AbstractResultPane abstractResultPane = sender as AbstractResultPane;
			if (abstractResultPane != null)
			{
				if (abstractResultPane.Icon == null)
				{
					this.GetWorkPanePage(abstractResultPane).ImageIndex = -1;
					return;
				}
				this.GetWorkPanePage(abstractResultPane).ImageIndex = this.AddNewImageIcon(abstractResultPane.Icon);
			}
		}

		private WorkPanePage GetWorkPanePage(AbstractResultPane resultPane)
		{
			return resultPane.Parent as WorkPanePage;
		}

		private int AddNewImageIcon(Icon icon)
		{
			int num = this.ImageIcons.Icons.IndexOf(icon);
			if (num == -1)
			{
				this.ImageIcons.Icons.Add(Guid.NewGuid().ToString(), icon);
				num = this.ImageIcons.Icons.IndexOf(icon);
			}
			return num;
		}

		private IconLibrary ImageIcons
		{
			get
			{
				if (this.icons == null)
				{
					this.icons = new IconLibrary();
					base.ImageList = this.icons.SmallImageList;
				}
				return this.icons;
			}
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			try
			{
				base.OnHandleDestroyed(e);
			}
			catch (KeyNotFoundException)
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

		private IconLibrary icons;
	}
}
