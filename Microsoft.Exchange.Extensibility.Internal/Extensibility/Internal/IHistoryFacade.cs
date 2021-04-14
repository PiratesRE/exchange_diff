using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal interface IHistoryFacade
	{
		RecipientP2Type RecipientType { get; }

		List<IHistoryRecordFacade> Records { get; }
	}
}
