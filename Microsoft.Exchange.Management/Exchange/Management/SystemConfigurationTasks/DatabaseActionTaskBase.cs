using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class DatabaseActionTaskBase<TDataObject> : SystemConfigurationObjectActionTask<DatabaseIdParameter, TDataObject> where TDataObject : Database, new()
	{
		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || SystemConfigurationTasksHelper.IsKnownWmiException(exception) || SystemConfigurationTasksHelper.IsKnownMapiDotNETException(exception) || SystemConfigurationTasksHelper.IsKnownClusterUpdateDatabaseResourceException(exception);
		}

		protected override void TranslateException(ref Exception e, out ErrorCategory category)
		{
			base.TranslateException(ref e, out category);
			category = (ErrorCategory)1001;
			if (SystemConfigurationTasksHelper.IsKnownMapiDotNETException(e))
			{
				TDataObject dataObject = this.DataObject;
				e = new InvalidOperationException(Strings.ErrorFailedToConnectToStore(dataObject.ServerName), e);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			TDataObject dataObject = this.DataObject;
			if (!dataObject.IsExchange2009OrLater)
			{
				Exception exception = new InvalidOperationException(Strings.ErrorModifyE12DatabaseNotAllowed);
				ErrorCategory category = ErrorCategory.InvalidOperation;
				TDataObject dataObject2 = this.DataObject;
				base.WriteError(exception, category, dataObject2.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			TDataObject scratchPad = Activator.CreateInstance<TDataObject>();
			scratchPad.CopyChangesFrom(this.DataObject);
			base.InternalProcessRecord();
			TDataObject dataObject = this.DataObject;
			AmServerName amServerName = new AmServerName(dataObject.ServerName);
			TDataObject dataObject2 = this.DataObject;
			SystemConfigurationTasksHelper.DoubleWrite<TDataObject>(dataObject2.Identity, scratchPad, amServerName.Fqdn, base.DomainController, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			TaskLogger.LogExit();
		}
	}
}
