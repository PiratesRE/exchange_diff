using System;

namespace Microsoft.Exchange.Diagnostics
{
	public sealed class BoolAppSettingsReader : AppSettingsReader<bool>
	{
		public BoolAppSettingsReader(string name, bool defaultValue) : base(name, defaultValue)
		{
		}

		public static bool TryParseStringValue(string inputValue, out bool outputValue)
		{
			int num;
			if (int.TryParse(inputValue, out num))
			{
				outputValue = (num != 0);
				return true;
			}
			bool flag;
			if (bool.TryParse(inputValue, out flag))
			{
				outputValue = flag;
				return true;
			}
			outputValue = false;
			return false;
		}

		protected override bool TryParseValue(string inputValue, out bool outputValue)
		{
			return BoolAppSettingsReader.TryParseStringValue(inputValue, out outputValue);
		}
	}
}
