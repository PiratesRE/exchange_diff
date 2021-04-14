using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class DocumentBreadcrumbBarContextMenu : ContextMenu
	{
		public DocumentBreadcrumbBarContextMenu(UserContext userContext) : base("divDbcbm", userContext, false)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, -1028120515, ThemeFileId.None, "divAFB", "addfavbcb");
			base.RenderMenuItem(output, 636550561, ThemeFileId.None, "divCSB", "copyuribcb");
		}
	}
}
