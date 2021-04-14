using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal interface IMdbPropertyMappingContext
	{
		MailboxSession MailboxSession { get; }
	}
}
