using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal interface IMailboxInfoList : IList<MailboxInfo>, ICollection<MailboxInfo>, IEnumerable<MailboxInfo>, IEnumerable
	{
	}
}
