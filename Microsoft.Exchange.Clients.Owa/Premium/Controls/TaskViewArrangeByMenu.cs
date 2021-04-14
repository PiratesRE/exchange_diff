using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class TaskViewArrangeByMenu : ArrangeByMenu
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
			ArrangeByMenu.RenderMenuItem(output, 785343019, "divTy", ColumnId.TaskIcon);
			ArrangeByMenu.RenderMenuItem(output, -153493007, "divCmp", ColumnId.MarkCompleteCheckbox);
			ArrangeByMenu.RenderMenuItem(output, 1569168155, "divIm", ColumnId.Importance);
			ArrangeByMenu.RenderMenuItem(output, 1072079569, "divAt", ColumnId.HasAttachment);
			ArrangeByMenu.RenderMenuItem(output, 2014646167, "divSbj", ColumnId.Subject);
			ArrangeByMenu.RenderMenuItem(output, -66960209, "divDt", ColumnId.DueDate);
			ArrangeByMenu.RenderMenuItem(output, 503275475, "divFlg", ColumnId.TaskFlag);
		}
	}
}
