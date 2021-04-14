using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.OAB;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Assistants;
using Microsoft.Exchange.Rpc.Trigger;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Update", "OfflineAddressBook", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class UpdateOfflineAddressBook : SystemConfigurationObjectActionTask<OfflineAddressBookIdParameter, OfflineAddressBook>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdateOfflineAddressBook(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.DataObject.PublicFolderDatabase != null && this.DataObject.PublicFolderDistributionEnabled)
			{
				base.GetDataObject<PublicFolderDatabase>(new DatabaseIdParameter(this.DataObject.PublicFolderDatabase), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.DataObject.PublicFolderDatabase.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.DataObject.PublicFolderDatabase.ToString())));
			}
			if (!this.DataObject.IsE15OrLater())
			{
				this.ownerServer = (Server)base.GetDataObject<Server>(new ServerIdParameter(this.DataObject.Server), base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.DataObject.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.DataObject.Server.ToString())));
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			if (this.DataObject.IsE15OrLater())
			{
				this.TryCallE15();
			}
			else
			{
				this.TryCallE14();
			}
			TaskLogger.LogExit();
		}

		private void TryCallE14()
		{
			if (this.ownerServer != null && this.ownerServer.VersionNumber < Server.E15MinVersion)
			{
				string name = this.ownerServer.Name;
				string text = null;
				try
				{
					TriggerPrivateRpcClient triggerPrivateRpcClient = new TriggerPrivateRpcClient(name);
					triggerPrivateRpcClient.RunOabGenTask(((ADObjectId)this.DataObject.Identity).DistinguishedName);
				}
				catch (RpcException ex)
				{
					text = RpcUtility.MapRpcErrorCodeToMessage(ex.ErrorCode, name);
				}
				catch (COMException ex2)
				{
					text = ex2.Message;
				}
				if (text != null)
				{
					base.WriteError(new LocalizedException(Strings.ErrorOabGenFailedE14(this.DataObject.Identity.ToString(), name, text)), ErrorCategory.InvalidResult, this.Identity);
				}
			}
		}

		private void TryCallE15()
		{
			ADUser[] array = null;
			if (OABVariantConfigurationSettings.IsLinkedOABGenMailboxesEnabled)
			{
				ADObjectId generatingMailbox = this.DataObject.GeneratingMailbox;
				if (generatingMailbox != null)
				{
					ADUser aduser = (ADUser)base.GetDataObject<ADUser>(new MailboxIdParameter(generatingMailbox), base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(generatingMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotUnique(generatingMailbox.ToString())));
					array = new ADUser[]
					{
						aduser
					};
				}
			}
			if (array == null)
			{
				array = OrganizationMailbox.FindByOrganizationId(this.DataObject.OrganizationId, OrganizationCapability.OABGen);
				if (array.Length == 0)
				{
					if (this.ownerServer != null && this.ownerServer.VersionNumber >= Server.E15MinVersion)
					{
						base.WriteError(new LocalizedException(Strings.ErrorOabGenFailedSinceNoOrgMbxFound(this.DataObject.Identity.ToString())), ErrorCategory.ObjectNotFound, this.Identity);
					}
					return;
				}
			}
			bool flag = false;
			foreach (ADUser aduser2 in array)
			{
				string activeServerFqdn = OrganizationMailbox.GetActiveServerFqdn(aduser2.Id);
				string text = null;
				try
				{
					OABGeneratorAssistantRunNowParameters oabgeneratorAssistantRunNowParameters = new OABGeneratorAssistantRunNowParameters
					{
						PartitionId = this.DataObject.Id.GetPartitionId(),
						ObjectGuid = this.DataObject.Id.ObjectGuid,
						Description = "from Update-OAB on Server:" + Environment.MachineName
					};
					AssistantsRpcClient assistantsRpcClient = new AssistantsRpcClient(activeServerFqdn);
					assistantsRpcClient.StartWithParams("OABGeneratorAssistant", aduser2.ExchangeGuid, aduser2.Database.ObjectGuid, oabgeneratorAssistantRunNowParameters.ToString());
					flag = true;
				}
				catch (RpcException ex)
				{
					text = RpcUtility.MapRpcErrorCodeToMessage(ex.ErrorCode, activeServerFqdn);
				}
				catch (COMException ex2)
				{
					text = ex2.Message;
				}
				if (text != null)
				{
					if (array.Length == 1)
					{
						base.WriteError(new LocalizedException(Strings.ErrorOabGenFailed(this.DataObject.Identity.ToString(), activeServerFqdn, text)), ErrorCategory.InvalidResult, this.Identity);
					}
					else
					{
						this.WriteWarning(Strings.WarningOabGenFailed(this.DataObject.Identity.ToString(), aduser2.Identity.ToString(), activeServerFqdn, text));
					}
				}
			}
			if (array.Length > 1 && !flag)
			{
				base.WriteError(new LocalizedException(Strings.ErrorOabGenFailedForAllOrgMailboxes(this.DataObject.Identity.ToString())), ErrorCategory.InvalidResult, this.Identity);
			}
		}

		private Server ownerServer;
	}
}
