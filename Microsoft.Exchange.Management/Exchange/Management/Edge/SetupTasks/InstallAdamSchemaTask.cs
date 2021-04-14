using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[Cmdlet("Install", "AdamSchema")]
	[LocDescription(Strings.IDs.InstallAdamSchemaTask)]
	public sealed class InstallAdamSchemaTask : Task
	{
		public InstallAdamSchemaTask()
		{
			this.InstanceName = "MSExchange";
			this.MacroName = "<SchemaContainerDN>";
			this.MacroValue = "#schemaNamingContext";
		}

		[Parameter(Mandatory = false)]
		public string InstanceName
		{
			get
			{
				return (string)base.Fields["InstanceName"];
			}
			set
			{
				base.Fields["InstanceName"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string LdapFileName
		{
			get
			{
				return base.Fields["LdapFileName"] as string;
			}
			set
			{
				base.Fields["LdapFileName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MacroName
		{
			get
			{
				return base.Fields["MacroName"] as string;
			}
			set
			{
				base.Fields["MacroName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MacroValue
		{
			get
			{
				return base.Fields["MacroValue"] as string;
			}
			set
			{
				base.Fields["MacroValue"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!AdamServiceSettings.GetSettingsExist(this.InstanceName))
			{
				base.WriteError(new InvalidAdamInstanceNameException(this.InstanceName), ErrorCategory.InvalidArgument, null);
			}
			if (!File.Exists(this.LdapFileName))
			{
				base.WriteError(new InvalidLdapFileNameException(), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.InstanceName
			});
			try
			{
				ManageAdamService.ImportAdamSchema(this.InstanceName, this.LdapFileName, this.MacroName, this.MacroValue);
			}
			catch (AdamSchemaImportProcessFailureException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		public const string InstanceParamName = "InstanceName";

		public const string LdapSchemaFileNameParamName = "LdapFileName";

		public const string MacroNameParamName = "MacroName";

		public const string MacroValueParamName = "MacroValue";

		public const string MacroNameParamDefaultValue = "<SchemaContainerDN>";

		public const string PredefSchemaNamingContextMacro = "#schemaNamingContext";

		public const string PredefConfigNamingContextMacro = "#configurationNamingContext";

		public static readonly string AdamSchemaImportCumulativeLogFilePath = ManageAdamService.AdamSchemaImportCumulativeLogFilePath;

		public static readonly string AdamSchemaImportCumulativeErrorFilePath = ManageAdamService.AdamSchemaImportCumulativeErrorFilePath;
	}
}
