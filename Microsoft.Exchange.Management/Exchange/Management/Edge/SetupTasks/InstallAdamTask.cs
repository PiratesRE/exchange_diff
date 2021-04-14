using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[Cmdlet("Install", "Adam")]
	[LocDescription(Strings.IDs.InstallAdamTask)]
	public sealed class InstallAdamTask : Task
	{
		public InstallAdamTask()
		{
			this.InstanceName = "MSExchange";
		}

		[Parameter(Mandatory = false)]
		public string InstanceName
		{
			get
			{
				return base.Fields["InstanceName"] as string;
			}
			set
			{
				base.Fields["InstanceName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DataFilesPath
		{
			get
			{
				return base.Fields["DataFilesPath"] as string;
			}
			set
			{
				base.Fields["DataFilesPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LogFilesPath
		{
			get
			{
				return base.Fields["LogFilesPath"] as string;
			}
			set
			{
				base.Fields["LogFilesPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Port
		{
			get
			{
				return (int)base.Fields["Port"];
			}
			set
			{
				base.Fields["Port"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SslPort
		{
			get
			{
				return (int)base.Fields["SslPort"];
			}
			set
			{
				base.Fields["SslPort"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!new GatewayRole().IsUnpacked)
			{
				base.WriteError(new TaskException(Strings.GatewayRoleIsNotUnpacked), ErrorCategory.InvalidOperation, null);
			}
			if (this.DataFilesPath == null)
			{
				this.DataFilesPath = ConfigurationContext.Setup.TransportDataPath;
			}
			else
			{
				try
				{
					Utils.ValidateDirectory(this.DataFilesPath, "DataFilesPath");
				}
				catch (InvalidCharsInPathException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				}
				catch (InvalidDriveInPathException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
				}
				catch (NoPermissionsForPathException exception3)
				{
					base.WriteError(exception3, ErrorCategory.InvalidArgument, null);
				}
				catch (PathIsTooLongException exception4)
				{
					base.WriteError(exception4, ErrorCategory.InvalidArgument, null);
				}
				catch (ReadOnlyPathException exception5)
				{
					base.WriteError(exception5, ErrorCategory.InvalidArgument, null);
				}
			}
			if (this.LogFilesPath == null)
			{
				this.LogFilesPath = ConfigurationContext.Setup.TransportDataPath;
			}
			else if (this.LogFilesPath != this.DataFilesPath)
			{
				try
				{
					Utils.ValidateDirectory(this.LogFilesPath, "LogFilesPath");
				}
				catch (InvalidCharsInPathException exception6)
				{
					base.WriteError(exception6, ErrorCategory.InvalidArgument, null);
				}
				catch (InvalidDriveInPathException exception7)
				{
					base.WriteError(exception7, ErrorCategory.InvalidArgument, null);
				}
				catch (NoPermissionsForPathException exception8)
				{
					base.WriteError(exception8, ErrorCategory.InvalidArgument, null);
				}
				catch (PathIsTooLongException exception9)
				{
					base.WriteError(exception9, ErrorCategory.InvalidArgument, null);
				}
				catch (ReadOnlyPathException exception10)
				{
					base.WriteError(exception10, ErrorCategory.InvalidArgument, null);
				}
			}
			this.Port = this.ValidatePort(base.Fields["Port"], "Port", null);
			this.SslPort = this.ValidatePort(base.Fields["SslPort"], "SslPort", base.Fields["Port"]);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ManageAdamService.InstallAdam(this.InstanceName, this.DataFilesPath, this.LogFilesPath, this.Port, this.SslPort, new WriteVerboseDelegate(base.WriteVerbose));
			}
			catch (AdamInstallFailureDataOrLogFolderNotEmptyException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			catch (AdamInstallProcessFailureException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
			}
			catch (AdamInstallGeneralFailureWithResultException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidOperation, null);
			}
			catch (AdamInstallErrorException exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidOperation, null);
			}
			catch (AdamSetAclsProcessFailureException exception5)
			{
				base.WriteError(exception5, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		private int ValidatePort(object portToValidateObj, string parameterName, object ldapPortObj)
		{
			if (Convert.ToInt32(portToValidateObj) == 0)
			{
				return Utils.GetAvailablePort(ldapPortObj);
			}
			int num = (int)portToValidateObj;
			if (!Utils.IsPortValid(num))
			{
				base.WriteError(new InvalidPortNumberException(num), ErrorCategory.InvalidArgument, null);
			}
			if (ldapPortObj != null)
			{
				int num2 = (int)ldapPortObj;
				if (num == num2)
				{
					base.WriteError(new SslPortSameAsLdapPortException(), ErrorCategory.InvalidArgument, null);
				}
			}
			if (!Utils.IsPortAvailable(num))
			{
				base.WriteError(new PortIsBusyException(num), ErrorCategory.InvalidArgument, null);
			}
			return num;
		}

		public const string InstanceParamName = "InstanceName";

		public const string DataFilesPathParamName = "DataFilesPath";

		public const string LogFilesPathParamName = "LogFilesPath";

		public const string LdapPortParamName = "Port";

		public const string SslPortParamName = "SslPort";
	}
}
