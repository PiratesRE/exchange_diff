using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Internal
{
	internal class CtsConfigurationSetting
	{
		internal CtsConfigurationSetting(string name)
		{
			this.name = name;
			this.arguments = new List<CtsConfigurationArgument>();
		}

		internal void AddArgument(string name, string value)
		{
			this.arguments.Add(new CtsConfigurationArgument(name, value));
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public IList<CtsConfigurationArgument> Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		private string name;

		private IList<CtsConfigurationArgument> arguments;
	}
}
