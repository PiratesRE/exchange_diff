using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class ToolbarButton
	{
		public ToolbarButton(string command, ToolbarButtonFlags flags, Strings.IDs textId, ThemeFileId image)
		{
			this.command = command;
			this.flags = flags;
			this.textId = textId;
			this.image = image;
		}

		public ToolbarButton(ToolbarButtonFlags flags, Strings.IDs textId, ThemeFileId image)
		{
			this.flags = flags;
			this.textId = textId;
			this.image = image;
		}

		public ToolbarButton(string command, ToolbarButtonFlags flags, Strings.IDs textId, ThemeFileId image, string toolTip)
		{
			this.command = command;
			this.flags = flags;
			this.textId = textId;
			this.image = image;
			this.toolTip = toolTip;
		}

		public string ToolTip
		{
			get
			{
				return this.toolTip;
			}
		}

		public ToolbarButtonFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public Strings.IDs TextId
		{
			get
			{
				return this.textId;
			}
		}

		public ThemeFileId Image
		{
			get
			{
				return this.image;
			}
		}

		public string Command
		{
			get
			{
				return this.command;
			}
		}

		private Strings.IDs textId;

		private ThemeFileId image;

		private string command;

		private ToolbarButtonFlags flags;

		private string toolTip;
	}
}
