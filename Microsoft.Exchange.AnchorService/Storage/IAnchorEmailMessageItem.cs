using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAnchorEmailMessageItem : IAnchorAttachmentMessage, IDisposable
	{
		void Send(IEnumerable<SmtpAddress> toAddresses, string subject, string body);
	}
}
