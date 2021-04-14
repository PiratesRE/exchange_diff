using System;
using System.Text;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal static class MapiHttpOperationUtilities
	{
		internal static Encoding GetStateEncodingOrDefault(NspiState state)
		{
			Encoding asciiEncoding;
			if (state == null || !String8Encodings.TryGetEncoding(state.CodePage, out asciiEncoding))
			{
				asciiEncoding = CTSGlobals.AsciiEncoding;
			}
			return asciiEncoding;
		}
	}
}
