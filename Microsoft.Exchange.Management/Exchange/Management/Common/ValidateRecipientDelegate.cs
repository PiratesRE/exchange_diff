using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.Common
{
	internal delegate void ValidateRecipientDelegate(ADRecipient recipient, string recipientId, Task.ErrorLoggerDelegate writeError);
}
