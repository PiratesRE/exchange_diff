using System;
using Microsoft.Exchange.Management.IisTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClientAccess
{
	internal struct CafeIisWebServiceExtension
	{
		internal const string GroupID = "MSExchangeCafe";

		internal const Strings.IDs DescriptionID = Strings.IDs.CafeIisWebServiceExtensionsDescription;

		internal static readonly IisWebServiceExtension[] AllExtensions = new IisWebServiceExtension[]
		{
			new IisWebServiceExtension("owaauth.dll", "FrontEnd\\HttpProxy\\bin", false, true)
		};

		internal enum Index
		{
			First,
			Cafe = 0
		}
	}
}
