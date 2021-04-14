using System;

namespace Microsoft.Exchange.Data.Internal
{
	internal enum TagClass : byte
	{
		Universal,
		Application = 64,
		Context = 128,
		Private = 192
	}
}
