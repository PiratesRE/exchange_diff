using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class ObjectNotFoundException : StoreException
	{
		public ObjectNotFoundException(LID lid, Guid mailboxGuid, string message) : base(lid, ErrorCodeValue.NotFound, message)
		{
			this.mailboxGuid = mailboxGuid;
		}

		public ObjectNotFoundException(LID lid, Guid mailboxGuid, string message, Exception innerException) : base(lid, ErrorCodeValue.NotFound, message, innerException)
		{
			this.mailboxGuid = mailboxGuid;
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		private Guid mailboxGuid;
	}
}
