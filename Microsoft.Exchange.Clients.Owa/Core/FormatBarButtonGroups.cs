using System;

namespace Microsoft.Exchange.Clients.Owa.Core
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
		UndoRedo = 1024,
		Hyperlinking = 2048,
		SuperSubScript = 4096,
		Strikethrough = 8192,
		All = 16383
	}
}
