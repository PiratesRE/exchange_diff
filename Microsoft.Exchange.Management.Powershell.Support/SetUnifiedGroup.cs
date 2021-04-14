using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.UnifiedGroups;
using Microsoft.WindowsAzure.ActiveDirectory;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Set", "UnifiedGroup")]
	public sealed class SetUnifiedGroup : UnifiedGroupTask
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public Guid Identity { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SetUnifiedGroup")]
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

		[Parameter(Mandatory = false, ParameterSetName = "SetUnifiedGroup")]
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

		[Parameter(Mandatory = false, ParameterSetName = "SetUnifiedGroup")]
		public RecipientIdParameter[] AddOwners
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AddOwners"];
			}
			set
			{
				base.Fields["AddOwners"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SetUnifiedGroup")]
		public RecipientIdParameter[] RemoveOwners
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["RemoveOwners"];
			}
			set
			{
				base.Fields["RemoveOwners"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SetUnifiedGroup")]
		public RecipientIdParameter[] AddMembers
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["AddMembers"];
			}
			set
			{
				base.Fields["AddMembers"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SetUnifiedGroup")]
		public RecipientIdParameter[] RemoveMembers
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["RemoveMembers"];
			}
			set
			{
				base.Fields["RemoveMembers"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SetUnifiedGroup")]
		public SwitchParameter PublishExchangeResources
		{
			get
			{
				return (SwitchParameter)(base.Fields["PublishExchangeResources"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["PublishExchangeResources"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SyncGroupMailbox")]
		public SwitchParameter SyncGroupMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["SyncGroupMailbox"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SyncGroupMailbox"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			this.recipientSession = base.GetRecipientSession();
			ADRecipient[] array = base.ResolveRecipientIdParameters<ADRecipient>(this.AddOwners, this.recipientSession);
			if (array != null)
			{
				this.addOwners = (from recipient in array
				select recipient.ExternalDirectoryObjectId).ToArray<string>();
			}
			array = base.ResolveRecipientIdParameters<ADRecipient>(this.RemoveOwners, this.recipientSession);
			if (array != null)
			{
				this.removeOwners = (from recipient in array
				select recipient.ExternalDirectoryObjectId).ToArray<string>();
			}
			array = base.ResolveRecipientIdParameters<ADRecipient>(this.AddMembers, this.recipientSession);
			if (array != null)
			{
				this.addMembers = (from recipient in array
				select recipient.ExternalDirectoryObjectId).ToArray<string>();
			}
			array = base.ResolveRecipientIdParameters<ADRecipient>(this.RemoveMembers, this.recipientSession);
			if (array != null)
			{
				this.removeMembers = (from recipient in array
				select recipient.ExternalDirectoryObjectId).ToArray<string>();
			}
		}

		protected override void InternalProcessRecord()
		{
			if (base.Fields.IsModified("SyncGroupMailbox") && this.SyncGroupMailbox)
			{
				this.UpdateFromGroupMailbox();
				return;
			}
			ADUser groupMailbox = this.recipientSession.FindADUserByExternalDirectoryObjectId(this.Identity.ToString());
			ADUser ownerFromAAD = this.GetOwnerFromAAD(groupMailbox, this.recipientSession);
			string[] exchangeResources = this.GetExchangeResources(groupMailbox);
			AADClient aadclient = AADClientFactory.Create(ownerFromAAD);
			if (aadclient == null)
			{
				base.WriteError(new TaskException(Strings.ErrorUnableToSessionWithAAD), ExchangeErrorCategory.Client, null);
			}
			this.UpdateGroup(this.DisplayName, this.Description, exchangeResources, this.Identity.ToString(), aadclient);
			base.AddOwnersInAAD(this.addOwners, aadclient, this.Identity.ToString());
			base.RemoveOwnersInAAD(this.removeOwners, aadclient, this.Identity.ToString());
			base.AddMembersInAAD(this.addMembers, aadclient, this.Identity.ToString());
			base.RemoveMembersInAAD(this.removeMembers, aadclient, this.Identity.ToString());
		}

		private void UpdateFromGroupMailbox()
		{
			ADUser aduser = this.recipientSession.FindADUserByExternalDirectoryObjectId(this.Identity.ToString());
			ADUser ownerFromAAD = this.GetOwnerFromAAD(aduser, this.recipientSession);
			string[] members = base.GetMembers(aduser, this.recipientSession, "Set-UnifiedGroup");
			string[] owners = base.GetOwners(aduser, null, this.recipientSession);
			string description = (aduser.Description != null && aduser.Description.Count > 0) ? aduser.Description[0] : string.Empty;
			string[] exchangeResources = this.GetExchangeResources(aduser);
			AADClient aadclient = AADClientFactory.Create(ownerFromAAD);
			if (aadclient == null)
			{
				base.WriteError(new TaskException(Strings.ErrorUnableToSessionWithAAD), ExchangeErrorCategory.Client, null);
			}
			this.UpdateGroup(aduser.DisplayName, description, exchangeResources, aduser.ExternalDirectoryObjectId, aadclient);
			base.AddOwnersInAAD(owners, aadclient, aduser.ExternalDirectoryObjectId);
			base.AddMembersInAAD(members, aadclient, aduser.ExternalDirectoryObjectId);
		}

		private ADUser GetOwnerFromAAD(ADUser groupMailbox, IRecipientSession recipientSession)
		{
			Group group = null;
			AADClient aadclient = AADClientFactory.Create(base.OrganizationId, GraphProxyVersions.Version14);
			if (aadclient == null)
			{
				base.WriteError(new TaskException(Strings.ErrorUnableToSessionWithAAD), ExchangeErrorCategory.Client, null);
			}
			try
			{
				group = aadclient.GetGroup(groupMailbox.ExternalDirectoryObjectId, true);
				aadclient.Service.LoadProperty(group, "owners");
			}
			catch (AADException ex)
			{
				base.WriteVerbose("Failed to get group owner from AAD with exception: {0}", new object[]
				{
					ex
				});
				base.WriteError(new TaskException(Strings.ErrorUnableToGetGroupOwners), base.GetErrorCategory(ex), null);
			}
			if (group.owners != null)
			{
				foreach (DirectoryObject directoryObject in group.owners)
				{
					ADUser aduser = recipientSession.FindADUserByExternalDirectoryObjectId(directoryObject.objectId);
					if (aduser != null)
					{
						return aduser;
					}
				}
			}
			base.WriteError(new TaskException(Strings.ErrorUnableToGetGroupOwners), ExchangeErrorCategory.Client, null);
			return null;
		}

		private void UpdateGroup(string displayName, string description, string[] exchangeResources, string groupObjectId, AADClient aadClient)
		{
			try
			{
				base.WriteVerbose("Updating UnifiedGroup for group {0}", new object[]
				{
					groupObjectId
				});
				aadClient.UpdateGroup(groupObjectId, description, exchangeResources, displayName, null);
				base.WriteVerbose("UnifiedGroup updated successfully", new object[0]);
			}
			catch (AADException ex)
			{
				base.WriteVerbose("UpdateGroup failed with exception: {0}", new object[]
				{
					ex
				});
				base.WriteError(new TaskException(Strings.ErrorUnableToUpdateUnifiedGroup), base.GetErrorCategory(ex), null);
			}
		}

		private string[] GetExchangeResources(ADUser groupMailbox)
		{
			try
			{
				MailboxUrls mailboxUrls = new MailboxUrls(ExchangePrincipal.FromADUser(groupMailbox, RemotingOptions.AllowCrossSite), true);
				return mailboxUrls.ToExchangeResources();
			}
			catch (LocalizedException ex)
			{
				base.WriteVerbose("Failed to get MailboxUrls with exception: {0}", new object[]
				{
					ex
				});
				this.WriteWarning(Strings.WarningUnableToUpdateExchangeResources);
			}
			return null;
		}

		private IRecipientSession recipientSession;

		private string[] addOwners;

		private string[] removeOwners;

		private string[] addMembers;

		private string[] removeMembers;
	}
}
