using System;

namespace Microsoft.Exchange.UM.UMCore
{
	[Serializable]
	internal enum SearchMethod
	{
		None,
		FirstNameLastName,
		LastNameFirstName,
		EmailAlias,
		CompanyName,
		PromptForAlias
	}
}
