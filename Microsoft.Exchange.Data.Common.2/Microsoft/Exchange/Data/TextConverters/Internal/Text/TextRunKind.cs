using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
	internal enum TextRunKind : uint
	{
		Invalid,
		Text = 67108864U,
		QuotingLevel = 167772160U
	}
}
