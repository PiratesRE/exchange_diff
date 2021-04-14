using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationEmailMessageItem : IMigrationAttachmentMessage, IDisposable
	{
		void Send(IEnumerable<SmtpAddress> toAddresses, string subject, string body);
	}
}
