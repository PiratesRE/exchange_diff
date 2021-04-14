using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ToolbarButton
	{
		public ToolbarButton(string command, ToolbarButtonFlags flags, Strings.IDs textId, ThemeFileId image, Strings.IDs tooltipId)
		{
			this.command = command;
			this.flags = flags;
			this.textId = textId;
			this.tooltipId = tooltipId;
			this.image = image;
		}

		public ToolbarButton(string command, ToolbarButtonFlags flags, Strings.IDs textId, ThemeFileId image) : this(command, flags, textId, image, textId)
		{
		}

		public ToolbarButton(string command, ToolbarButtonFlags flags, ThemeFileId image) : this(command, flags, -1018465893, image)
		{
		}

		public ToolbarButton(string command, ToolbarButtonFlags flags) : this(command, flags, -1018465893, ThemeFileId.None)
		{
		}

		public ToolbarButton(string command, Strings.IDs textId) : this(command, ToolbarButtonFlags.Text, textId, ThemeFileId.None)
		{
		}

		public ToolbarButtonFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public Strings.IDs TooltipId
		{
			get
			{
				return this.tooltipId;
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

		public ToolbarButton[] ToggleWithButtons
		{
			get
			{
				return this.toggleWithButtons;
			}
		}

		public ToolbarButton[] SwapWithButtons
		{
			get
			{
				return this.swapWithButtons;
			}
		}

		public void SetSwapButtons(params ToolbarButton[] swapWithButtons)
		{
			if (swapWithButtons == null)
			{
				throw new ArgumentNullException("swapWithButtons");
			}
			this.swapWithButtons = swapWithButtons;
		}

		public void SetToggleButtons(params ToolbarButton[] toggleWithButtons)
		{
			if (toggleWithButtons == null)
			{
				throw new ArgumentNullException("toggleWithButtons");
			}
			this.toggleWithButtons = toggleWithButtons;
		}

		public virtual void RenderControl(TextWriter writer)
		{
		}

		private Strings.IDs textId;

		private Strings.IDs tooltipId;

		private ThemeFileId image;

		private string command;

		private ToolbarButtonFlags flags;

		private ToolbarButton[] toggleWithButtons;

		private ToolbarButton[] swapWithButtons;
	}
}
