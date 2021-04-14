using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Flags]
	public enum FormatBarButtonGroups
	{
		None = 0,
		BoldItalicUnderline = 1,
		Justification = 2,
		Lists = 4,
		Indenting = 8,
		Direction = 16,
		ForegroundColor = 32,
		BackgroundColor = 64,
		RemoveFormatting = 128,
		HorizontalRule = 256,
		UndoRedo = 512,
		Hyperlinking = 1024,
		SuperSubScript = 2048,
		Strikethrough = 4096,
		Customize = 8192,
		All = 16383
	}
}
