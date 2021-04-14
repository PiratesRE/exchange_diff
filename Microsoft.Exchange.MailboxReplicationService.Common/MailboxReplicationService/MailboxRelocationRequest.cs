using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class MailboxRelocationRequest : RequestBase
	{
		public MailboxRelocationRequest()
		{
		}

		internal MailboxRelocationRequest(IRequestIndexEntry index) : base(index)
		{
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[RequestJobSchema.ExchangeGuid];
			}
		}

		private new bool Protect
		{
			get
			{
				return base.Protect;
			}
		}

		private new RequestStyle RequestStyle
		{
			get
			{
				return base.RequestStyle;
			}
		}
	}
}
