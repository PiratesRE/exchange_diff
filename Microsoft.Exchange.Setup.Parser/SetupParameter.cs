using System;

namespace Microsoft.Exchange.Setup.Parser
{
	internal class SetupParameter
	{
		public SetupParameter(string name, object val)
		{
			this.Name = name;
			this.Value = val;
		}

		public string Name { get; private set; }

		public object Value { get; private set; }
	}
}
