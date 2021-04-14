using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetTopologySystemConfigurationObjectTask<TIdentity, TDataObject> : SetTopologySystemConfigurationObjectTask<TIdentity, TDataObject, TDataObject> where TIdentity : IIdentityParameter, new() where TDataObject : ADObject, new()
	{
	}
}
