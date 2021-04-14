using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	internal delegate void AutoProvisionProgress(LocalizedString activity, LocalizedString statusDescription);
}
