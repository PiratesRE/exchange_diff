using System;

namespace Microsoft.Exchange.Data.Internal
{
	internal class CtsConfigurationArgument
	{
		internal CtsConfigurationArgument(string name, string value)
		{
			this.name = name;
			this.value = value;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		private string name;

		private string value;
	}
}
