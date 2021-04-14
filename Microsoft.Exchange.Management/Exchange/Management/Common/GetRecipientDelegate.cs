using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.Common
{
	internal delegate ADRecipient GetRecipientDelegate<TIdentityParameter>(TIdentityParameter identityParameter, Task.ErrorLoggerDelegate writeError) where TIdentityParameter : IIdentityParameter;
}
