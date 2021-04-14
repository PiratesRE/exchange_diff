using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class AtsException : LocalizedException
	{
		public AtsException(LocalizedString errorMessage) : base(errorMessage)
		{
		}
	}
}
