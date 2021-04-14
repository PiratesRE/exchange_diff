using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal enum RunTextType : uint
	{
		Unknown,
		Space = 134217728U,
		NewLine = 268435456U,
		Tabulation = 402653184U,
		UnusualWhitespace = 536870912U,
		LastWhitespace = 536870912U,
		Nbsp = 671088640U,
		NonSpace = 805306368U,
		LastText = 805306368U,
		Last = 805306368U,
		Mask = 939524096U
	}
}
