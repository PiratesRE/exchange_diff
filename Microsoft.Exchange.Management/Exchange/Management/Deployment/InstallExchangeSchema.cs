using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "ExchangeSchema")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class InstallExchangeSchema : Task
	{
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

		protected override void InternalBeginProcessing()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 99, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\InstallExchangeSchema.cs");
			try
			{
				this.schemaMasterDC = topologyConfigurationSession.GetSchemaMasterDC();
			}
			catch (SchemaMasterDCNotFoundException exception)
			{
				base.ThrowTerminatingError(exception, ErrorCategory.ResourceUnavailable, null);
			}
			topologyConfigurationSession.DomainController = this.schemaMasterDC;
			this.MacroName = "<SchemaContainerDN>";
			this.MacroValue = topologyConfigurationSession.SchemaNamingContext.DistinguishedName;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				this.ldapFilePath = Path.Combine(ConfigurationContext.Setup.InstallPath, this.LdapFileName);
				if (!File.Exists(this.ldapFilePath))
				{
					base.WriteError(new FileNotFoundException(this.ldapFilePath), ErrorCategory.InvalidData, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.ImportSchemaFile(this.schemaMasterDC, this.ldapFilePath, this.MacroName, this.MacroValue, new WriteVerboseDelegate(base.WriteVerbose));
			TaskLogger.LogExit();
		}

		private void ImportSchemaFile(string schemaMasterServer, string schemaFilePath, string macroName, string macroValue, WriteVerboseDelegate writeVerbose)
		{
			TaskLogger.LogEnter();
			string fileName = Path.Combine(Environment.SystemDirectory, "ldifde.exe");
			string text = Path.GetTempPath();
			if (text[text.Length - 1] == '\\')
			{
				text = text.Substring(0, text.Length - 1);
			}
			string arguments = string.Format("-i -s \"{0}\" -f \"{1}\" -j \"{2}\" -c \"{3}\" \"{4}\"", new object[]
			{
				schemaMasterServer,
				schemaFilePath,
				text,
				macroName,
				macroValue.Replace("\"", "\\\\\"")
			});
			int num = InstallExchangeSchema.RunProcess(fileName, arguments, writeVerbose);
			if (num != 0)
			{
				base.WriteError(new TaskException(Strings.SchemaImportProcessFailure(schemaFilePath, "ldifde.exe", num, Path.Combine(text, "ldif.err"))), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		private static int RunProcess(string fileName, string arguments, WriteVerboseDelegate writeVerbose)
		{
			TaskLogger.LogEnter();
			writeVerbose(Strings.LogRunningCommand(fileName, arguments));
			int exitCode;
			using (Process process = Process.Start(new ProcessStartInfo
			{
				FileName = fileName,
				Arguments = arguments,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false
			}))
			{
				process.WaitForExit();
				writeVerbose(Strings.LogProcessExitCode(fileName, process.ExitCode));
				TaskLogger.LogExit();
				exitCode = process.ExitCode;
			}
			return exitCode;
		}

		private const string MacroNameParamDefaultValue = "<SchemaContainerDN>";

		private const string SchemaImportExportExeFileName = "ldifde.exe";

		private const string SchemaImportExportLogFileName = "ldif.log";

		private const string SchemaImportExportErrorFileName = "ldif.err";

		private string schemaMasterDC;

		private string ldapFilePath;
	}
}
