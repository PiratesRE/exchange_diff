using System;
using Microsoft.Exchange.Management.IisTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClientAccess
{
	internal struct IisWebServiceExtension
	{
		internal const string GroupID = "MSExchangeClientAccess";

		internal const Strings.IDs DescriptionID = Strings.IDs.ClientAccessIisWebServiceExtensionsDescription;

		internal static readonly IisWebServiceExtension[] AllExtensions = new IisWebServiceExtension[]
		{
			new IisWebServiceExtension("owaauth.dll", "ClientAccess\\owa\\auth", false, true)
		};

		internal enum Index
		{
			First,
			Owa = 0
		}
	}
}
