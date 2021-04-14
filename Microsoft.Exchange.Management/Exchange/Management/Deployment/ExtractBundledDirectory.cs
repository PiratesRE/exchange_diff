using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.CabUtility;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("extract", "BundledDirectory", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class ExtractBundledDirectory : Task
	{
		[Parameter(Mandatory = true)]
		public LongPath BundlePath
		{
			get
			{
				return (LongPath)base.Fields["BundlePath"];
			}
			set
			{
				this.bundlePath = value;
				base.Fields["BundlePath"] = this.bundlePath;
			}
		}

		[Parameter(Mandatory = true)]
		public string DirToExtract
		{
			get
			{
				return (string)base.Fields["DirToExtract"];
			}
			set
			{
				string text = value;
				if (text[text.Length - 1] != '\\')
				{
					text += '\\';
				}
				base.Fields["DirToExtract"] = text;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (!File.Exists(this.bundlePath.PathName) || Path.GetFileName(this.bundlePath.PathName).ToLower() != "languagepackbundle.exe")
			{
				base.WriteError(new TaskException(Strings.EBDInvalidBundle(this.bundlePath.PathName)), ErrorCategory.InvalidArgument, 0);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(this.BundlePath.PathName)));
			try
			{
				EmbeddedCabWrapper.ExtractFiles(this.BundlePath.PathName, directoryInfo.FullName, this.DirToExtract);
			}
			catch (CabUtilityWrapperException e)
			{
				TaskLogger.LogError(e);
				TaskLogger.LogExit();
			}
			string sendToPipeline = directoryInfo.FullName + '\\' + this.DirToExtract;
			base.WriteObject(sendToPipeline);
			TaskLogger.LogExit();
		}

		private LongPath bundlePath;
	}
}
