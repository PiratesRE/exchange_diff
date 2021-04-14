using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class ButtonBaseBulkEditorAdapter : BulkEditorAdapter
	{
		public ButtonBaseBulkEditorAdapter(ButtonBase control) : base(control)
		{
			this.bulkEditSupport = (control as IButtonBaseBulkEditSupport);
			this.bulkEditSupport.Painted += this.OnAppearancePainted;
			this.bulkEditSupport.FocusSetted += this.OnAppearancePainted;
			this.bulkEditSupport.FocusKilled += this.OnAppearancePainted;
			this.bulkEditSupport.CheckedChangedRaising += this.OnCheckedChangedRaising;
		}

		internal void OnCheckedChangedRaising(object sender, HandledEventArgs e)
		{
			e.Handled = (base["Checked"] != null && !this.forceAllowCheckedChangedEvent);
		}

		internal void OnAppearancePainted(object sender, EventArgs e)
		{
			if (base["Checked"] != null)
			{
				using (Graphics graphics = base.HostControl.CreateGraphics())
				{
					this.OnOwnerDraw(graphics);
				}
			}
		}

		protected Rectangle CalculateCheckBounds(ContentAlignment alignment, Size fullCheckSize)
		{
			Rectangle clientRectangle = base.HostControl.ClientRectangle;
			Rectangle rectangle = new Rectangle(clientRectangle.Location, fullCheckSize);
			if (fullCheckSize.Width > 0)
			{
				if ((alignment & (ContentAlignment)1092) != (ContentAlignment)0)
				{
					rectangle.X = clientRectangle.X + clientRectangle.Width - rectangle.Width;
				}
				else if ((alignment & (ContentAlignment)546) != (ContentAlignment)0)
				{
					rectangle.X = clientRectangle.X + (clientRectangle.Width - rectangle.Width) / 2;
				}
				if ((alignment & (ContentAlignment)1792) != (ContentAlignment)0)
				{
					rectangle.Y = clientRectangle.Y + clientRectangle.Height - rectangle.Height;
				}
				else if ((alignment & (ContentAlignment)7) != (ContentAlignment)0)
				{
					rectangle.Y = clientRectangle.Y + 2;
				}
				else
				{
					rectangle.Y = clientRectangle.Y + (clientRectangle.Height - rectangle.Height) / 2;
				}
			}
			return LayoutHelper.MirrorRectangle(rectangle, base.HostControl);
		}

		protected virtual void OnOwnerDraw(Graphics g)
		{
		}

		protected override IList<string> InnerGetManagedProperties()
		{
			IList<string> list = base.InnerGetManagedProperties();
			list.Add("Checked");
			return list;
		}

		protected const string ManagedPropertyName = "Checked";

		protected bool forceAllowCheckedChangedEvent;

		protected IButtonBaseBulkEditSupport bulkEditSupport;
	}
}
