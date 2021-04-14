using System;
using Microsoft.Exchange.Configuration.ObjectModel;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetObjectTask<TDataObject> : SetObjectTask<ConfigObjectIdParameter, TDataObject> where TDataObject : ConfigObject, new()
	{
	}
}
