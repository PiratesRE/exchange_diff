using System;

namespace Microsoft.Exchange.Setup.Common
{
	internal interface IOrganizationName
	{
		string EscapedName { get; }

		string UnescapedName { get; }

		string ToString();
	}
}
