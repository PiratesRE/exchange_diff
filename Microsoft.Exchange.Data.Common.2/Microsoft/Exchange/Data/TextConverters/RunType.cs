using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal enum RunType : uint
	{
		Invalid,
		Special = 1073741824U,
		Normal = 2147483648U,
		Literal = 3221225472U,
		Mask = 3221225472U
	}
}
