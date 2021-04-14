using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal sealed class EnumAppSettingsEntry<T> : AppSettingsEntry<T> where T : struct
	{
		public EnumAppSettingsEntry(string name, T defaultValue, Trace tracer) : base(name, defaultValue, tracer)
		{
		}

		protected override bool TryParseValue(string inputValue, out T outputValue)
		{
			T t;
			if (Enum.TryParse<T>(inputValue, out t))
			{
				outputValue = t;
				return true;
			}
			outputValue = default(T);
			return false;
		}
	}
}
