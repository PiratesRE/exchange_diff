using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Assistants;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UnifiedGroups;
using Microsoft.WindowsAzure.ActiveDirectory;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("New", "UnifiedGroup")]
	public sealed class NewUnifiedGroup : UnifiedGroupTask
	{
		[Parameter(Mandatory = true, ParameterSetName = "NewUnifiedGroup")]
		public string Alias
		{
			get
			{
				return (string)base.Fields["Alias"];
			}
			set
			{
				base.Fields["Alias"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "NewUnifiedGroup")]
		[ValidateNotNullOrEmpty]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "NewUnifiedGroup", Position = 0)]
		[ValidateNotNullOrEmpty]
		public string DisplayName
		{
			get
			{
				return (string)base.Fields["DisplayName"];
			}
			set
			{
				base.Fields["DisplayName"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "NewUnifiedGroup")]
		[ValidateNotNullOrEmpty]
		public RecipientIdParameter ExecutingUser
		{
			get
			{
				return (RecipientIdParameter)base.Fields["ExecutingUser"];
			}
			set
			{
				base.Fields["ExecutingUser"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "NewUnifiedGroup")]
		public bool? IsPublic
		{
			get
			{
				return (bool?)base.Fields["IsPublic"];
			}
			set
			{
				base.Fields["IsPublic"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "NewUnifiedGroup")]
		public RecipientIdParameter[] Members
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["Members"];
			}
			set
			{
				base.Fields["Members"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "NewUnifiedGroup")]
		[ValidateNotNullOrEmpty]
		public RecipientIdParameter[] Owners
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["Owners"];
			}
			set
			{
				base.Fields["Owners"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "FromGroupMailbox")]
		public MailboxIdParameter FromGroupMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["FromGroupMailbox"];
			}
			set
			{
				base.Fields["FromGroupMailbox"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			this.recipientSession = base.GetRecipientSession();
			this.executingUser = base.ResolveRecipientIdParameter<ADUser>(this.ExecutingUser, this.recipientSession);
			ADRecipient[] array = base.ResolveRecipientIdParameters<ADRecipient>(this.Owners, this.recipientSession);
			if (array != null)
			{
				this.owners = (from recipient in array
				select recipient.ExternalDirectoryObjectId).ToArray<string>();
			}
			array = base.ResolveRecipientIdParameters<ADRecipient>(this.Members, this.recipientSession);
			if (array != null)
			{
				this.members = (from recipient in array
				select recipient.ExternalDirectoryObjectId).ToArray<string>();
			}
		}

		protected override void InternalProcessRecord()
		{
			this.recipientSession = base.GetRecipientSession();
			if (base.Fields.IsModified("FromGroupMailbox"))
			{
				this.CreateFromGroupMailbox();
				return;
			}
			this.CreateUnifiedGroup();
		}

		private void CreateUnifiedGroup()
		{
			AADClient aadclient = this.CreateAADClient(this.executingUser);
			string groupObjectId = this.CreateGroup(this.DisplayName, this.Alias, this.Description, this.IsPublic == null || this.IsPublic.Value, aadclient);
			aadclient.AddMembers(groupObjectId, new string[]
			{
				this.executingUser.ExternalDirectoryObjectId
			});
			base.AddOwnersInAAD(this.owners, aadclient, groupObjectId);
			base.AddMembersInAAD(this.members, aadclient, groupObjectId);
			this.WriteGroup(aadclient, groupObjectId);
		}

		private void CreateFromGroupMailbox()
		{
			ADUser groupMailbox = this.GetGroupMailbox(this.FromGroupMailbox, this.recipientSession);
			ADUser creator = this.GetCreator(groupMailbox, this.recipientSession);
			string[] array = base.GetMembers(groupMailbox, this.recipientSession, "New-UnifiedGroup");
			string[] array2 = base.GetOwners(groupMailbox, creator, this.recipientSession);
			string description = (groupMailbox.Description != null && groupMailbox.Description.Count > 0) ? groupMailbox.Description[0] : string.Empty;
			AADClient aadclient = this.CreateAADClient(creator);
			string text = this.CreateGroup(groupMailbox.DisplayName, groupMailbox.Name, description, groupMailbox.ModernGroupType == ModernGroupObjectType.Public, aadclient);
			base.AddOwnersInAAD(array2, aadclient, text);
			base.AddMembersInAAD(array, aadclient, text);
			try
			{
				base.WriteVerbose("Updating group mailbox with new ExternalDirectoryObjectId", new object[0]);
				groupMailbox.ExternalDirectoryObjectId = text;
				groupMailbox.CustomAttribute13 = "AADGroup";
				this.recipientSession.Save(groupMailbox);
				base.WriteVerbose("Updated group mailbox with new ExternalDirectoryObjectId", new object[0]);
			}
			catch (LocalizedException ex)
			{
				base.WriteVerbose("Failed to update ExternalDirectoryObjectId property on group mailbox due exception: {0}", new object[]
				{
					ex
				});
				this.WriteWarning(Strings.ErrorCannotUpdateExternalDirectoryObjectId(groupMailbox.Id.ToString(), text));
				aadclient.DeleteGroup(text);
				return;
			}
			this.ReplicateMailboxAssociation(groupMailbox);
			this.WriteGroup(aadclient, text);
		}

		private AADClient CreateAADClient(ADUser user)
		{
			AADClient aadclient = AADClientFactory.Create(user);
			if (aadclient == null)
			{
				base.WriteError(new TaskException(Strings.ErrorUnableToSessionWithAAD), ExchangeErrorCategory.Client, null);
			}
			return aadclient;
		}

		private ADUser GetGroupMailbox(MailboxIdParameter mailboxId, IRecipientSession recipientSession)
		{
			LocalizedString? localizedString;
			ADUser objectInOrganization = mailboxId.GetObjectInOrganization<ADUser>(null, recipientSession, null, out localizedString);
			if (objectInOrganization == null)
			{
				base.WriteError(new TaskException(localizedString.Value), (ErrorCategory)1003, null);
			}
			return objectInOrganization;
		}

		private ADUser GetCreator(ADUser groupMailbox, IRecipientSession recipientSession)
		{
			foreach (ADObjectId entryId in ((MultiValuedProperty<ADObjectId>)groupMailbox[GroupMailboxSchema.Owners]))
			{
				ADUser aduser = recipientSession.Read(entryId) as ADUser;
				if (aduser != null)
				{
					base.WriteVerbose("Group creator: {0}", new object[]
					{
						aduser.Id
					});
					return aduser;
				}
			}
			base.WriteError(new TaskException(Strings.ErrorUnableToGetCreatorFromGroupMailbox), ExchangeErrorCategory.Client, null);
			return null;
		}

		private string CreateGroup(string displayName, string alias, string description, bool isPublic, AADClient aadClient)
		{
			string text = null;
			try
			{
				base.WriteVerbose("Creating UnifiedGroup for group {0}", new object[]
				{
					alias
				});
				text = aadClient.CreateGroup(displayName, alias, description, isPublic);
				base.WriteVerbose("UnifiedGroup created successfully, ObjectId={0}", new object[]
				{
					text
				});
			}
			catch (AADException ex)
			{
				base.WriteVerbose("CreateGroup failed with exception: {0}", new object[]
				{
					ex
				});
				base.WriteError(new TaskException(Strings.ErrorUnableToCreateUnifiedGroup), base.GetErrorCategory(ex), null);
			}
			return text;
		}

		private void ReplicateMailboxAssociation(ADUser groupMailbox)
		{
			this.PopulateADCache(groupMailbox);
			this.CallMailboxAssociationReplicationAssistant(groupMailbox);
		}

		private void PopulateADCache(ADUser groupMailbox)
		{
			Uri backEndWebServicesUrl = BackEndLocator.GetBackEndWebServicesUrl(groupMailbox);
			UpdateGroupMailboxBase updateGroupMailboxBase = new UpdateGroupMailboxViaEWS(groupMailbox, null, backEndWebServicesUrl, (GroupMailboxConfigurationActionType)0, null, null, null);
			updateGroupMailboxBase.Execute();
			if (updateGroupMailboxBase.Error != null)
			{
				base.WriteVerbose("UpdateGroupMailbox error: {0}", new object[]
				{
					updateGroupMailboxBase.Error
				});
				this.WriteWarning(Strings.WarningUnableToUpdateUserMailboxes);
			}
		}

		private void CallMailboxAssociationReplicationAssistant(ADUser groupMailbox)
		{
			Exception ex = null;
			try
			{
				ActiveManager activeManagerInstance = ActiveManager.GetActiveManagerInstance();
				DatabaseLocationInfo serverForDatabase = activeManagerInstance.GetServerForDatabase(groupMailbox.Database.ObjectGuid);
				base.WriteVerbose("Group mailbox database is on server {0}", new object[]
				{
					serverForDatabase.ServerFqdn
				});
				using (AssistantsRpcClient assistantsRpcClient = new AssistantsRpcClient(serverForDatabase.ServerFqdn))
				{
					assistantsRpcClient.StartWithParams("MailboxAssociationReplicationAssistant", groupMailbox.ExchangeGuid, groupMailbox.Database.ObjectGuid, string.Empty);
				}
				base.WriteVerbose("Started update of the user mailboxes for the new ExternalDirectoryObjectId", new object[0]);
			}
			catch (RpcException ex2)
			{
				ex = ex2;
			}
			catch (LocalizedException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				base.WriteVerbose("Failed to call RPC to MailboxAssociationReplicationAssistant due the following exception: {0}", new object[]
				{
					ex
				});
				this.WriteWarning(Strings.WarningUnableToUpdateUserMailboxes);
			}
		}

		private void WriteGroup(AADClient aadClient, string groupObjectId)
		{
			try
			{
				Group group = aadClient.GetGroup(groupObjectId, true);
				aadClient.Service.LoadProperty(group, "members");
				aadClient.Service.LoadProperty(group, "owners");
				base.WriteObject(new AADGroupPresentationObject(group));
			}
			catch (AADException ex)
			{
				base.WriteVerbose("GetGroup failed with exception: {0}", new object[]
				{
					ex
				});
				this.WriteWarning(Strings.ErrorUnableToGetUnifiedGroup);
			}
		}

		private IRecipientSession recipientSession;

		private ADUser executingUser;

		private string[] owners;

		private string[] members;
	}
}
