using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ValidationContext
	{
		internal ValidationContext(StoreSession session)
		{
			this.session = session;
		}

		internal StoreSession Session
		{
			get
			{
				return this.session;
			}
		}

		private readonly StoreSession session;
	}
}
