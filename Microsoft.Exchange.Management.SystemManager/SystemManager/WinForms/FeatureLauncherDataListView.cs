using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class FeatureLauncherDataListView : DataListView
	{
		protected override void DrawLockedState()
		{
			using (Graphics graphics = base.CreateGraphics())
			{
				foreach (object obj in base.Items)
				{
					ListViewItem listViewItem = (ListViewItem)obj;
					if (((FeatureLauncherListViewItem)listViewItem).IsLocked)
					{
						Rectangle targetRect = new Rectangle(listViewItem.Bounds.Right - 8, listViewItem.Bounds.Top, 8, 8);
						if (!base.Enabled)
						{
							Color control = SystemColors.Control;
						}
						else
						{
							Color backColor = this.BackColor;
						}
						using (new SolidBrush(SystemColors.Control))
						{
							graphics.DrawIcon(Icons.LockIcon, targetRect);
						}
					}
				}
			}
		}
	}
}
