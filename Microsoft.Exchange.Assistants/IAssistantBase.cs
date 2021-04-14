using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal interface IAssistantBase
	{
		LocalizedString Name { get; }

		string NonLocalizedName { get; }

		void OnShutdown();
	}
}
