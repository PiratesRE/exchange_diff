using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	[Serializable]
	internal class RetryException : LocalizedException
	{
		public RetryException(MessageStatus status) : base(Strings.RetryException)
		{
			this.status = status;
		}

		public RetryException(MessageStatus status, string storeDriverContext) : this(status)
		{
			this.storeDriverContext = storeDriverContext;
		}

		public MessageStatus MessageStatus
		{
			get
			{
				return this.status;
			}
		}

		public string StoreDriverContext
		{
			get
			{
				return this.storeDriverContext;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Delivery queue needs to be retried later due to {0}", new object[]
			{
				this.status.Exception
			});
		}

		private readonly string storeDriverContext;

		private MessageStatus status;
	}
}
