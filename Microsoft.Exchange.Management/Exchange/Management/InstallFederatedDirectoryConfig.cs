using System;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management
{
	[Cmdlet("Install", "FederatedDirectoryConfig")]
	public sealed class InstallFederatedDirectoryConfig : Task
	{
		[Parameter(Mandatory = false)]
		public string Filename { get; set; }

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			string path = Path.Combine(ConfigurationContext.Setup.BinPath, "FederatedDirectory.config");
			File.Delete(path);
			string name = string.IsNullOrEmpty(this.Filename) ? "FederatedDirectory.config" : this.Filename;
			string value;
			using (Stream manifestResourceStream = base.GetType().Assembly.GetManifestResourceStream(name))
			{
				if (manifestResourceStream == null)
				{
					base.WriteError(new LocalizedException(Strings.ErrorObjectNotFound(name)), (ErrorCategory)1003, null);
				}
				using (StreamReader streamReader = new StreamReader(manifestResourceStream))
				{
					value = streamReader.ReadToEnd();
				}
			}
			using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.ASCII))
			{
				streamWriter.Write(value);
			}
			TaskLogger.LogExit();
		}

		private const string DefaultFilename = "FederatedDirectory.config";
	}
}
