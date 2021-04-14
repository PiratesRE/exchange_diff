using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISubmissionItem : IDisposable
	{
		string SourceServerFqdn { get; }

		IPAddress SourceServerNetworkAddress { get; }

		DateTime OriginalCreateTime { get; }

		void Submit();

		void Submit(ProxyAddress sender, IEnumerable<Participant> recipients);
	}
}
