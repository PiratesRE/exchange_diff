using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetTopologySystemConfigurationObjectTask<TIdentity, TPublicObject, TDataObject> : SetSystemConfigurationObjectTask<TIdentity, TPublicObject, TDataObject> where TIdentity : IIdentityParameter, new() where TPublicObject : IConfigurable, new() where TDataObject : ADObject, new()
	{
	}
}
