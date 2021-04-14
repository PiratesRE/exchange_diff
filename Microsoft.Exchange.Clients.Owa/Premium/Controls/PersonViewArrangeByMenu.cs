using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class PersonViewArrangeByMenu : ArrangeByMenu
	{
		protected override void RenderMenuItems(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			ArrangeByMenu.RenderMenuItem(output, -1876431821, null, ColumnId.GivenName);
			ArrangeByMenu.RenderMenuItem(output, 1759499200, null, ColumnId.Surname);
			ArrangeByMenu.RenderMenuItem(output, -826838917, null, ColumnId.CompanyName);
			ArrangeByMenu.RenderMenuItem(output, 13085779, null, ColumnId.FileAs);
			ArrangeByMenu.RenderMenuItem(output, -611050349, null, ColumnId.Department);
			ArrangeByMenu.RenderMenuItem(output, 1587370059, null, ColumnId.ContactFlagDueDate);
			ArrangeByMenu.RenderMenuItem(output, 1580556595, null, ColumnId.ContactFlagStartDate);
		}
	}
}
