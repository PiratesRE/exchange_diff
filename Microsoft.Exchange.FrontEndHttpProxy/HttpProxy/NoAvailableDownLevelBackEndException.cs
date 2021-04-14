using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.HttpProxy
{
	[Serializable]
	internal class NoAvailableDownLevelBackEndException : ServerNotFoundException
	{
		public NoAvailableDownLevelBackEndException(string message) : base(message)
		{
		}
	}
}
