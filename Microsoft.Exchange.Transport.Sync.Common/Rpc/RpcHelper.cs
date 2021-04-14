using System;
using System.Globalization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RpcHelper
	{
		public static bool TryGetProperty<TOutputType>(MdbefPropertyCollection args, uint property, out TOutputType output)
		{
			string text;
			return RpcHelper.TryGetProperty<TOutputType>(args, property, out output, out text);
		}

		public static bool TryGetProperty<TOutputType>(MdbefPropertyCollection args, uint property, out TOutputType output, out string errorString)
		{
			object obj;
			if (args.TryGetValue(property, out obj) && obj is TOutputType)
			{
				output = (TOutputType)((object)obj);
				errorString = null;
				return true;
			}
			output = default(TOutputType);
			errorString = string.Format(CultureInfo.InvariantCulture, "Could not read property {0}. Found {1}.", new object[]
			{
				property,
				obj
			});
			return false;
		}

		public static byte[] CreateResponsePropertyCollection(uint returnValuePropTag, object value)
		{
			return new MdbefPropertyCollection
			{
				{
					returnValuePropTag,
					value
				}
			}.GetBytes();
		}
	}
}
