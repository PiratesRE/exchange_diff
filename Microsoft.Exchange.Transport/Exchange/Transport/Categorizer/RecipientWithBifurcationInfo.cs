using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal struct RecipientWithBifurcationInfo<T> where T : IEquatable<T>, IComparable<T>, new()
	{
		public RecipientWithBifurcationInfo(MailRecipient recipient, T bifurcationInfo)
		{
			this.recipient = recipient;
			this.bifurcationInfo = bifurcationInfo;
		}

		public MailRecipient Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		public T BifurcationInfo
		{
			get
			{
				return this.bifurcationInfo;
			}
			set
			{
				this.bifurcationInfo = value;
			}
		}

		private MailRecipient recipient;

		private T bifurcationInfo;
	}
}
