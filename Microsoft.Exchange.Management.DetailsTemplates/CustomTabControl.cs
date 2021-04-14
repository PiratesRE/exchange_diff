using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal sealed class CustomTabControl : TabControl
	{
		internal CustomTabControl()
		{
			this.AllowDrop = true;
		}

		private int GetTabAt(Point location)
		{
			int result = -1;
			for (int i = 0; i < base.TabCount; i++)
			{
				if (base.GetTabRect(i).Contains(location))
				{
					result = i;
					break;
				}
			}
			return result;
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);
			if (!e.Data.GetDataPresent(typeof(CustomTabPage)))
			{
				e.Effect = DragDropEffects.None;
				return;
			}
			e.Effect = DragDropEffects.Move;
			TabPage tabPage = e.Data.GetData(typeof(CustomTabPage)) as CustomTabPage;
			Point point = base.PointToClient(new Point(e.X, e.Y));
			if (this.hoverTabRectangle.Contains(point))
			{
				return;
			}
			int num = base.TabPages.IndexOf(tabPage);
			int tabAt = this.GetTabAt(point);
			if (num != tabAt && tabAt != -1)
			{
				TabPage value = base.TabPages[tabAt];
				base.TabPages[num] = value;
				base.TabPages[tabAt] = tabPage;
				this.Refresh();
			}
			num = this.GetTabAt(point);
			if (num >= 0)
			{
				this.hoverTabRectangle = base.GetTabRect(num);
				return;
			}
			this.hoverTabRectangle = default(Rectangle);
		}

		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			if (!drgevent.Data.GetDataPresent(typeof(CustomTabPage)))
			{
				base.OnDragEnter(drgevent);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left)
			{
				DetailsTemplateTypeService detailsTemplateTypeService = (DetailsTemplateTypeService)this.GetService(typeof(DetailsTemplateTypeService));
				if (detailsTemplateTypeService != null && detailsTemplateTypeService.TemplateType.Equals("Mailbox Agent"))
				{
					return;
				}
				int tabAt = this.GetTabAt(e.Location);
				if (tabAt == -1)
				{
					return;
				}
				TabPage tabPage = base.TabPages[tabAt];
				if (tabPage != null)
				{
					this.hoverTabRectangle = base.GetTabRect(tabAt);
					base.DoDragDrop(tabPage, DragDropEffects.Move);
					if (base.TabPages.IndexOf(tabPage) != tabAt)
					{
						IComponentChangeService componentChangeService = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
						if (componentChangeService != null)
						{
							componentChangeService.OnComponentChanged(tabPage, null, null, null);
						}
					}
				}
			}
		}

		[ReadOnly(true)]
		public new Point Location
		{
			get
			{
				return base.Location;
			}
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			y = (x = 0);
			base.SetBoundsCore(x, y, width, height, specified);
		}

		private Rectangle hoverTabRectangle = default(Rectangle);
	}
}
