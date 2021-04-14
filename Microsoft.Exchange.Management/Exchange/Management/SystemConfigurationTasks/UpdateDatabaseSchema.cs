using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Update", "DatabaseSchema", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class UpdateDatabaseSchema : SetTopologySystemConfigurationObjectTask<DatabaseIdParameter, Database>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "Versions", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override DatabaseIdParameter Identity
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Versions")]
		public ushort MajorVersion
		{
			get
			{
				return (ushort)base.Fields["MajorVersion"];
			}
			set
			{
				base.Fields["MajorVersion"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Versions")]
		public ushort MinorVersion
		{
			get
			{
				return (ushort)base.Fields["MinorVersion"];
			}
			set
			{
				base.Fields["MinorVersion"] = value;
			}
		}

		private static int VersionFromComponents(ushort major, ushort minor)
		{
			return (int)major << 16 | (int)minor;
		}

		private static string VersionString(int version)
		{
			return string.Format("{0}.{1}", (ushort)(version >> 16), (ushort)(version & 65535));
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMailboxDatabase(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			Server server = this.DataObject.GetServer();
			bool flag = false;
			if (server == null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorDBOwningServerNotFound(this.DataObject.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			else
			{
				flag = server.IsE15OrLater;
			}
			if (!flag)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorModifyE14DatabaseNotAllowed), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			DatabaseAvailabilityGroup databaseAvailabilityGroup;
			using (IClusterDB clusterDB = DatabaseTasksHelper.OpenClusterDatabase((ITopologyConfigurationSession)base.DataSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError), this.DataObject, false, out databaseAvailabilityGroup))
			{
				if (clusterDB == null || !clusterDB.IsInstalled)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorSchemaVersionDoesntApply(this.DataObject.Name)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
			}
			if (base.Fields.IsModified("MajorVersion"))
			{
				this.version = UpdateDatabaseSchema.VersionFromComponents(this.MajorVersion, this.MinorVersion);
				int num;
				int num2;
				int num3;
				DatabaseTasksHelper.GetSupporableDatabaseSchemaVersionRange((ITopologyConfigurationSession)base.DataSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError), this.DataObject, out num, out num2, out num3);
				if (this.version < num || this.version > num2)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorSchemaVersionOutOfRange(UpdateDatabaseSchema.VersionString(num), UpdateDatabaseSchema.VersionString(num2))), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				if (num3 > this.version)
				{
					this.WriteWarning(Strings.RequestedVersionIsLowerThanCurrentVersion(num3));
				}
			}
			else
			{
				this.version = DatabaseTasksHelper.GetMaximumSupportedDatabaseSchemaVersion((ITopologyConfigurationSession)base.DataSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError), this.DataObject);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			DatabaseTasksHelper.SetRequestedDatabaseSchemaVersion((ITopologyConfigurationSession)base.DataSession, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError), this.DataObject, this.version);
			TaskLogger.LogExit();
		}

		internal const string paramMajorVersion = "MajorVersion";

		internal const string paramMinorVersion = "MinorVersion";

		internal const string versionParameterSetName = "Versions";

		private int version;
	}
}
