using System;

namespace Microsoft.Exchange.Common
{
	internal class SingletonBuilder<InterfaceOfTypeToBuild, TypeToBuild> : IBuilder<!0> where TypeToBuild : InterfaceOfTypeToBuild, new()
	{
		InterfaceOfTypeToBuild IBuilder<!0>.Build()
		{
			return this.instance;
		}

		private InterfaceOfTypeToBuild instance = (InterfaceOfTypeToBuild)((object)((default(TypeToBuild) == null) ? Activator.CreateInstance<TypeToBuild>() : default(TypeToBuild)));
	}
}
