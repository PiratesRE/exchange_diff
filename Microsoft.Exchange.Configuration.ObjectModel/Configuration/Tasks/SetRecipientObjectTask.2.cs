using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetRecipientObjectTask<TIdentity, TDataObject> : SetRecipientObjectTask<TIdentity, TDataObject, TDataObject> where TIdentity : IIdentityParameter, new() where TDataObject : ADRecipient, new()
	{
	}
}
