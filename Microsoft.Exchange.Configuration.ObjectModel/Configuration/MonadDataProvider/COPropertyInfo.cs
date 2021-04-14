using System;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class COPropertyInfo
	{
		public COPropertyInfo(string name, Type type)
		{
			this.Name = name;
			this.Type = type;
		}

		public string Name;

		public Type Type;
	}
}
