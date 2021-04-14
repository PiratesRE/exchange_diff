using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	[Serializable]
	internal class DatabaseInitializationException : ComponentFailedException
	{
		public DatabaseInitializationException(LocalizedString message) : base(message)
		{
		}

		internal override void RethrowNewInstance()
		{
			throw new ComponentFailedTransientException(this);
		}
	}
}
