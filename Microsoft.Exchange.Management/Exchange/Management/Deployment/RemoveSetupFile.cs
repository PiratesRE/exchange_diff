using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Remove", "SetupFile")]
	public sealed class RemoveSetupFile : Task
	{
		[Parameter(Mandatory = true)]
		public LongPath FilePath
		{
			get
			{
				return (LongPath)base.Fields["FilePath"];
			}
			set
			{
				base.Fields["FilePath"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			TaskLogger.Log(Strings.RemovingFile(this.FilePath.PathName));
			try
			{
				File.Delete(this.FilePath.PathName);
			}
			catch (SecurityException exception)
			{
				base.WriteError(exception, ErrorCategory.SecurityError, null);
			}
			catch (UnauthorizedAccessException exception2)
			{
				base.WriteError(exception2, ErrorCategory.PermissionDenied, null);
			}
			catch (IOException exception3)
			{
				base.WriteError(exception3, ErrorCategory.ResourceUnavailable, null);
			}
			catch (Win32Exception exception4)
			{
				base.WriteError(exception4, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}
	}
}
