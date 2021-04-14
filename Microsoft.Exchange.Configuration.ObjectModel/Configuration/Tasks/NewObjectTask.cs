using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class NewObjectTask<TDataObject> : NewTaskBase<TDataObject> where TDataObject : ConfigObject, new()
	{
		protected TDataObject Instance
		{
			get
			{
				return this.DataObject;
			}
			set
			{
				this.DataObject = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			try
			{
				string format = "Identity='{0}'";
				TDataObject dataObject = this.DataObject;
				string searchExpr = string.Format(format, dataObject.Identity);
				ConfigObject[] array = ((DataSourceManager)base.DataSession).Find(typeof(TDataObject), searchExpr, false, null);
				if (array != null)
				{
					base.WriteError(new ManagementObjectAlreadyExistsException(Strings.ExceptionObjectAlreadyExists), (ErrorCategory)1003, null);
				}
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, (ErrorCategory)1002, null);
			}
			if (base.HasErrors)
			{
				return;
			}
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			DataSourceManager[] dataSourceManagers = DataSourceManager.GetDataSourceManagers(typeof(TDataObject), "Identity");
			return dataSourceManagers[0];
		}
	}
}
