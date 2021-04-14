using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Data.Search
{
	internal delegate uint FillTextBuffer([MarshalAs(UnmanagedType.Struct)] ref TEXT_SOURCE pTextSource);
}
