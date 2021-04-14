using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ExchangePrincipalWrapper
	{
		public ExchangePrincipal ExchangePrincipal { get; private set; }

		public DateTime CreatedOn { get; private set; }

		internal ExchangePrincipalWrapper(ExchangePrincipal exchangePrincipal)
		{
			if (exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			this.ExchangePrincipal = exchangePrincipal;
			this.CreatedOn = DateTime.UtcNow;
		}
	}
}
