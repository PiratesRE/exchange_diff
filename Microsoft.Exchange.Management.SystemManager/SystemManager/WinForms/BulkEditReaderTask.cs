using System;
using System.Data;
using System.Reflection;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class BulkEditReaderTask : Reader
	{
		public BulkEditReaderTask(Type configObjectType, IDataObjectCreator creator, string dataObjectName, DataTable table)
		{
			this.type = configObjectType;
			this.creator = creator;
			this.dataObjectName = dataObjectName;
			this.table = table;
		}

		public override object DataObject
		{
			get
			{
				return this.dataObject;
			}
		}

		public override void Run(object interactionHandler, DataRow row, DataObjectStore store)
		{
			if (this.dataObject == null)
			{
				if (this.creator != null)
				{
					this.dataObject = this.creator.Create(this.table);
					return;
				}
				ConstructorInfo constructor = this.type.GetConstructor(new Type[0]);
				if (null != constructor)
				{
					this.dataObject = constructor.Invoke(new object[0]);
					return;
				}
			}
			else
			{
				if (store.GetDataObject(this.dataObjectName) != null)
				{
					this.dataObject = store.GetDataObject(this.dataObjectName);
				}
				IConfigurable configurable = this.DataObject as IConfigurable;
				if (configurable != null)
				{
					try
					{
						configurable.ResetChangeTracking();
					}
					catch (NotImplementedException)
					{
					}
				}
			}
		}

		public override bool IsRunnable(DataRow row, DataObjectStore store)
		{
			return true;
		}

		private object dataObject;

		private string dataObjectName;

		private Type type;

		private IDataObjectCreator creator;

		private DataTable table;
	}
}
