using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class Reader : RunnerBase
	{
		public abstract object DataObject { get; }

		public sealed override void Cancel()
		{
		}

		public virtual Reader CreateBulkReader(string dataObjectName, DataObjectStore store, DataTable table)
		{
			return new BulkEditReaderTask(store.GetDataObjectType(dataObjectName), store.GetDataObjectCreator(dataObjectName), dataObjectName, table);
		}

		public virtual bool HasPermission(IList<ParameterProfile> paramInfos)
		{
			return true;
		}
	}
}
