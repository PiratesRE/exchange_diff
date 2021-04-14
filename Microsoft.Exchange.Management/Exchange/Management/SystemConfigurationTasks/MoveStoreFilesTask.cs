using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class MoveStoreFilesTask<TIdentity, TDataObject> : SystemConfigurationObjectActionTask<TIdentity, TDataObject> where TIdentity : IIdentityParameter, new() where TDataObject : ADConfigurationObject, new()
	{
		[Parameter]
		public SwitchParameter ConfigurationOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["ConfigurationOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ConfigurationOnly"] = value;
			}
		}

		protected string OwnerServerName
		{
			get
			{
				return this.OwnerServer.Name;
			}
		}

		protected abstract Server OwnerServer { get; }

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
				TIdentity identity = this.Identity;
				e = new InvalidOperationException(Strings.ErrorFailedToGetDatabaseStatus(identity.ToString()), e);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!this.ConfigurationOnly && !Cluster.StringIEquals(this.OwnerServerName, Environment.MachineName))
			{
				Exception exception = new ArgumentException(Strings.ErrorConfigurationOnly, "ConfigurationOnly");
				ErrorCategory category = ErrorCategory.InvalidArgument;
				TDataObject dataObject = this.DataObject;
				base.WriteError(exception, category, dataObject.Identity);
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
			AmServerName amServerName = new AmServerName(this.OwnerServerName);
			TDataObject dataObject = this.DataObject;
			SystemConfigurationTasksHelper.DoubleWrite<TDataObject>(dataObject.Identity, scratchPad, amServerName.Fqdn, base.DomainController, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			TaskLogger.LogExit();
		}

		internal const string paramConfigurationOnly = "ConfigurationOnly";
	}
}
