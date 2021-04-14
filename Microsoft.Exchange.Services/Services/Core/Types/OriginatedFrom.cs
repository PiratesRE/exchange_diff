using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal enum OriginatedFrom
	{
		DirectoryRecipient,
		AccessToken = 2,
		ExternalIdentity = 5,
		Unknown
	}
}
