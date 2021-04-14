using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class JunkEmailContextMenu : ContextMenu
	{
		public static JunkEmailContextMenu Create(UserContext userContext, JunkEmailContextMenuType junkEmailContextMenuType)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return new JunkEmailContextMenu(userContext, junkEmailContextMenuType);
		}

		public JunkEmailContextMenu(UserContext userContext, JunkEmailContextMenuType junkEmailContextMenuType) : base((junkEmailContextMenuType == JunkEmailContextMenuType.Item) ? "divJnkmItm" : ((junkEmailContextMenuType == JunkEmailContextMenuType.Sender) ? "divJnkmSnd" : "divJnkmRcp"), userContext)
		{
			this.type = junkEmailContextMenuType;
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			switch (this.type)
			{
			case JunkEmailContextMenuType.Item:
				base.RenderMenuItem(output, 460585, "sndbsl");
				base.RenderMenuItem(output, 1616411850, "sndssl");
				base.RenderMenuItem(output, 930405866, "snddmssl");
				ContextMenu.RenderMenuDivider(output, "divS11");
				base.RenderMenuItem(output, 278436626, ThemeFileId.Inbox, "divNtJnk", "ntjnk");
				return;
			case JunkEmailContextMenuType.Sender:
				base.RenderMenuItem(output, 1707311266, "tobsl");
				base.RenderMenuItem(output, -1334943953, "tossl");
				base.RenderMenuItem(output, 527346223, "dmtossl");
				return;
			case JunkEmailContextMenuType.Recipient:
				base.RenderMenuItem(output, 1679197603, "rcvsrl");
				return;
			default:
				throw new OwaInvalidRequestException("Invalid junk mail context menu type");
			}
		}

		private JunkEmailContextMenuType type;
	}
}
