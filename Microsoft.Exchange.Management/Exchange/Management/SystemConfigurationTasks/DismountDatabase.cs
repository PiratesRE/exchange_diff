using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("dismount", "Database", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class DismountDatabase : DatabaseActionTaskBase<Database>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDismountDatabase(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			MapiTaskHelper.VerifyDatabaseAndItsOwningServerInScope(base.SessionSettings, this.DataObject, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			try
			{
				Server server = this.DataObject.GetServer();
				if (server == null)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorDBOwningServerNotFound(this.DataObject.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				base.WriteVerbose(Strings.VerboseConnectionAdminRpcInterface(server.Fqdn));
				base.WriteVerbose(Strings.VerboseUnmountDatabase(this.DataObject.Identity.ToString()));
				AmRpcClientHelper.DismountDatabase(ADObjectWrapperFactory.CreateWrapper(this.DataObject), 0);
			}
			catch (AmServerException ex)
			{
				Exception ex2;
				if (ex.TryGetInnerExceptionOfType(out ex2))
				{
					TaskLogger.Trace("DismountDatabase.InternalProcessRecord ignoring exception while unmounting database: {0}", new object[]
					{
						ex2.Message
					});
				}
				else if (ex.TryGetInnerExceptionOfType(out ex2))
				{
					TaskLogger.Trace("DismountDatabase.InternalProcessRecord ignoring exception while unmounting database: {0}", new object[]
					{
						ex2.Message
					});
				}
				else if (ex.TryGetInnerExceptionOfType(out ex2))
				{
					TaskLogger.Trace("DismountDatabase.InternalProcessRecord ignoring exception while unmounting database: {0}", new object[]
					{
						ex2.Message
					});
				}
				else if (ex is AmDatabaseNeverMountedException)
				{
					TaskLogger.Trace("DismountDatabase.InternalProcessRecord ignoring exception while unmounting database: {0}", new object[]
					{
						ex.Message
					});
				}
				else
				{
					TaskLogger.Trace("DismountDatabase.InternalProcessRecord raises exception while dismounting database: {0}", new object[]
					{
						ex.Message
					});
					base.WriteError(new InvalidOperationException(Strings.ErrorFailedToUnmountDatabase(this.Identity.ToString(), ex.Message), ex), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
			}
			catch (AmServerTransientException ex3)
			{
				TaskLogger.Trace("DismountDatabase.InternalProcessRecord raises exception while dismounting database: {0}", new object[]
				{
					ex3.Message
				});
				base.WriteError(new InvalidOperationException(Strings.ErrorFailedToUnmountDatabase(this.Identity.ToString(), ex3.Message), ex3), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}
	}
}
