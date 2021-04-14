using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetSystemConfigurationObjectTask<TIdentity, TDataObject> : SetSystemConfigurationObjectTask<TIdentity, TDataObject, TDataObject> where TIdentity : IIdentityParameter, new() where TDataObject : ADObject, new()
	{
	}
}
