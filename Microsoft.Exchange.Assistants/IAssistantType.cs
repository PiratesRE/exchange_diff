using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal interface IAssistantType
	{
		LocalizedString Name { get; }

		string NonLocalizedName { get; }
	}
}
