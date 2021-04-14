using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal class AcquireResult
	{
		public AcquireResult(IMessageDepotItemWrapper itemWrapper, AcquireToken token)
		{
			if (itemWrapper == null)
			{
				throw new ArgumentNullException("itemWrapper");
			}
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			this.itemWrapper = itemWrapper;
			this.token = token;
		}

		public IMessageDepotItemWrapper ItemWrapper
		{
			get
			{
				return this.itemWrapper;
			}
		}

		public AcquireToken Token
		{
			get
			{
				return this.token;
			}
		}

		private readonly IMessageDepotItemWrapper itemWrapper;

		private readonly AcquireToken token;
	}
}
