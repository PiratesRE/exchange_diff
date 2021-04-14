using System;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal abstract class EncryptedEmailMessageBinding
	{
		public abstract IStorage ConvertToEncryptedStorage(IStream stream, bool create);
	}
}
