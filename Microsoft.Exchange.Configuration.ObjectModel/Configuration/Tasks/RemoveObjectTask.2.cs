using System;
using Microsoft.Exchange.Configuration.ObjectModel;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class RemoveObjectTask<TDataObject> : RemoveObjectTask<ConfigObjectIdParameter, TDataObject> where TDataObject : ConfigObject, new()
	{
	}
}
