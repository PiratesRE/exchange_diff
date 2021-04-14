using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal sealed class DocumentProcessingContext
	{
		internal DocumentProcessingContext(MailboxSession session)
		{
			this.session = session;
		}

		public MailboxSession Session
		{
			get
			{
				return this.session;
			}
		}

		private readonly MailboxSession session;
	}
}
