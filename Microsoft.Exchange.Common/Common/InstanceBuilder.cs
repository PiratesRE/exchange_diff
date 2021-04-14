using System;

namespace Microsoft.Exchange.Common
{
	internal class InstanceBuilder<InterfaceOfTypeToBuild, TypeToBuild> : IBuilder<InterfaceOfTypeToBuild> where TypeToBuild : InterfaceOfTypeToBuild, new()
	{
		InterfaceOfTypeToBuild IBuilder<!0>.Build()
		{
			return (InterfaceOfTypeToBuild)((object)((default(TypeToBuild) == null) ? Activator.CreateInstance<TypeToBuild>() : default(TypeToBuild)));
		}
	}
}
