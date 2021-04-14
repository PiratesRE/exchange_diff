using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Categorizer
{
	[Serializable]
	internal class TransientRoutingException : TransientException
	{
		public TransientRoutingException(LocalizedString localizedString) : base(localizedString)
		{
		}
	}
}
