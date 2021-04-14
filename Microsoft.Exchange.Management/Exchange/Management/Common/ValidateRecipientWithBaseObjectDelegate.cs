using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.Common
{
	internal delegate void ValidateRecipientWithBaseObjectDelegate<TDataObject>(TDataObject baseObject, ADRecipient recipient, Task.ErrorLoggerDelegate writeError);
}
