using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IUMCAMessage
	{
		UMRecipient CAMessageRecipient { get; }

		bool CollectMessageForAnalysis { get; }
	}
}
