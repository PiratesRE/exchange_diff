using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "GroupMailbox", DefaultParameterSetName = "Identity")]
	public sealed class GetGroupMailbox : GetMailboxOrSyncMailbox
	{
		private new SwitchParameter Arbitration { get; set; }

		private new SwitchParameter Archive { get; set; }

		private new PSCredential Credential { get; set; }

		private new SwitchParameter InactiveMailboxOnly { get; set; }

		private new SwitchParameter IncludeSoftDeletedMailbox { get; set; }

		private new MailboxPlanIdParameter MailboxPlan { get; set; }

		private new SwitchParameter Monitoring { get; set; }

		private new SwitchParameter PublicFolder { get; set; }

		private new RecipientTypeDetails[] RecipientTypeDetails { get; set; }

		private new SwitchParameter RemoteArchive { get; set; }

		private new SwitchParameter SoftDeletedMailbox { get; set; }

		private new long UsnForReconciliationSearch { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeMembers { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludePermissionsVersion { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeMemberSyncStatus { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeMailboxUrls { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<GroupMailboxSchema>();
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return GetGroupMailbox.AllowedRecipientTypeDetails;
			}
		}

		protected override string SystemAddressListRdn
		{
			get
			{
				return "GroupMailboxes(VLV)";
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			ADUser aduser = (ADUser)dataObject;
			IRecipientSession recipientSession = (IRecipientSession)base.DataSession;
			GroupMailbox groupMailbox = GroupMailbox.FromDataObject(aduser);
			if ((this.IncludeMemberSyncStatus || this.IncludePermissionsVersion) && CmdletProxy.TryToProxyOutputObject(groupMailbox, base.CurrentTaskContext, aduser, false, this.ConfirmationMessage, CmdletProxy.AppendIdentityToProxyCmdlet(aduser)))
			{
				return groupMailbox;
			}
			ExchangePrincipal exchangePrincipal = null;
			if (this.IncludeMailboxUrls)
			{
				try
				{
					exchangePrincipal = ExchangePrincipal.FromADUser(aduser, RemotingOptions.AllowCrossSite);
					MailboxUrls mailboxUrls = new MailboxUrls(exchangePrincipal, false);
					groupMailbox.InboxUrl = this.SuppressPiiDataAsNeeded(mailboxUrls.InboxUrl);
					groupMailbox.CalendarUrl = this.SuppressPiiDataAsNeeded(mailboxUrls.CalendarUrl);
					groupMailbox.PeopleUrl = this.SuppressPiiDataAsNeeded(mailboxUrls.PeopleUrl);
					groupMailbox.PhotoUrl = this.SuppressPiiDataAsNeeded(mailboxUrls.PhotoUrl);
				}
				catch (LocalizedException ex)
				{
					base.WriteWarning("Unable to get mailbox principal due exception: " + ex.Message);
				}
			}
			IdentityDetails[] ownersDetails = this.GetOwnersDetails(recipientSession, aduser);
			if (ownersDetails != null)
			{
				groupMailbox.OwnersDetails = ownersDetails;
			}
			ADRawEntry[] array = null;
			if (this.IncludeMembers)
			{
				array = this.GetMemberRawEntriesFromAD(recipientSession, aduser);
				IdentityDetails[] identityDetails = this.GetIdentityDetails(array);
				groupMailbox.MembersDetails = identityDetails;
				groupMailbox.Members = Array.ConvertAll<IdentityDetails, ADObjectId>(identityDetails, (IdentityDetails member) => member.Identity);
			}
			if (!this.IncludeMemberSyncStatus)
			{
				if (!this.IncludePermissionsVersion)
				{
					return groupMailbox;
				}
			}
			try
			{
				if (exchangePrincipal == null)
				{
					exchangePrincipal = ExchangePrincipal.FromADUser(aduser, RemotingOptions.AllowCrossSite);
				}
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=Get-GroupMailbox"))
				{
					if (this.IncludeMemberSyncStatus)
					{
						if (array == null)
						{
							array = this.GetMemberRawEntriesFromAD(recipientSession, aduser);
						}
						ADObjectId[] membersInAD = Array.ConvertAll<ADRawEntry, ADObjectId>(array, (ADRawEntry member) => member.Id);
						ADObjectId[] membersFromMailbox = this.GetMembersFromMailbox(recipientSession, aduser, mailboxSession);
						groupMailbox.MembersSyncStatus = new GroupMailboxMembersSyncStatus(membersInAD, membersFromMailbox);
						if (base.NeedSuppressingPiiData)
						{
							groupMailbox.MembersSyncStatus.MembersInADOnly = SuppressingPiiData.Redact(groupMailbox.MembersSyncStatus.MembersInADOnly);
							groupMailbox.MembersSyncStatus.MembersInMailboxOnly = SuppressingPiiData.Redact(groupMailbox.MembersSyncStatus.MembersInMailboxOnly);
						}
					}
					if (this.IncludePermissionsVersion)
					{
						groupMailbox.PermissionsVersion = mailboxSession.Mailbox.GetValueOrDefault<int>(MailboxSchema.GroupMailboxPermissionsVersion, 0).ToString();
					}
				}
			}
			catch (LocalizedException ex2)
			{
				base.WriteWarning("Unable to retrieve data from group mailbox due exception: " + ex2.Message);
			}
			return groupMailbox;
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetGroupMailboxTaskModuleFactory();
		}

		private IdentityDetails[] GetOwnersDetails(IRecipientSession recipientSession, ADUser adUser)
		{
			ADObjectId[] array = new List<ADObjectId>((MultiValuedProperty<ADObjectId>)adUser[GroupMailboxSchema.Owners]).ToArray();
			IdentityDetails[] result;
			try
			{
				Result<ADRawEntry>[] queryResults = recipientSession.ReadMultiple(array, IdentityDetails.Properties);
				result = this.GetIdentityDetails(queryResults, array);
			}
			catch (LocalizedException ex)
			{
				base.WriteWarning("Unable to retrieve owners details from directory due exception: " + ex.Message);
				result = null;
			}
			return result;
		}

		private ADRawEntry[] GetMemberRawEntriesFromAD(IRecipientSession recipientSession, ADUser groupAdUser)
		{
			ADRawEntry[] result;
			try
			{
				result = UnifiedGroupADAccessLayer.GetAllGroupMembers(recipientSession, groupAdUser.Id, IdentityDetails.Properties, new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending), null, 0).ReadAllPages();
			}
			catch (LocalizedException ex)
			{
				base.WriteWarning("Unable to retrieve members details from directory due exception: " + ex.Message);
				result = new ADRawEntry[0];
			}
			return result;
		}

		private ADObjectId[] GetMembersFromMailbox(IRecipientSession recipientSession, ADUser groupAdUser, MailboxSession mailboxSession)
		{
			ADRawEntry[] memberRawEntriesFromMailbox = this.GetMemberRawEntriesFromMailbox(recipientSession, groupAdUser, mailboxSession);
			return Array.ConvertAll<ADRawEntry, ADObjectId>(memberRawEntriesFromMailbox, (ADRawEntry member) => member.Id);
		}

		private ADRawEntry[] GetMemberRawEntriesFromMailbox(IRecipientSession recipientSession, ADUser adUser, MailboxSession mailboxSession)
		{
			GroupMailboxLocator groupLocator = GroupMailboxLocator.Instantiate(recipientSession, adUser);
			List<string> exchangeLegacyDNs = new List<string>(10);
			GroupMailboxAccessLayer.Execute("Get-GroupMailbox", recipientSession, mailboxSession, delegate(GroupMailboxAccessLayer accessLayer)
			{
				IEnumerable<UserMailbox> members = accessLayer.GetMembers(groupLocator, false, null);
				foreach (UserMailbox userMailbox in members)
				{
					exchangeLegacyDNs.Add(userMailbox.Locator.LegacyDn);
				}
			});
			string[] array = exchangeLegacyDNs.ToArray();
			List<ADRawEntry> list = new List<ADRawEntry>(array.Length);
			try
			{
				Result<ADRawEntry>[] array2 = recipientSession.FindByExchangeLegacyDNs(array, IdentityDetails.Properties);
				for (int i = 0; i < array2.Length; i++)
				{
					Result<ADRawEntry> result = array2[i];
					if (result.Error != null)
					{
						this.WriteWarning(Strings.WarningUnableToResolveUser(array[i].ToString(), result.Error.ToString()));
					}
					else if (result.Data == null)
					{
						base.WriteVerbose(Strings.WarningUnableToResolveUser(array[i].ToString(), string.Empty));
					}
					else
					{
						list.Add(result.Data);
					}
				}
			}
			catch (LocalizedException ex)
			{
				base.WriteWarning("Unable to retrieve members details from mailbox due exception: " + ex.Message);
			}
			return list.ToArray();
		}

		private IdentityDetails[] GetIdentityDetails(Result<ADRawEntry>[] queryResults, object[] ids)
		{
			List<IdentityDetails> list = new List<IdentityDetails>(queryResults.Length);
			for (int i = 0; i < queryResults.Length; i++)
			{
				Result<ADRawEntry> result = queryResults[i];
				if (result.Error != null)
				{
					this.WriteWarning(Strings.WarningUnableToResolveUser(ids[i].ToString(), result.Error.ToString()));
				}
				else if (result.Data == null)
				{
					this.WriteWarning(Strings.WarningUnableToResolveUser(ids[i].ToString(), string.Empty));
				}
				else
				{
					IdentityDetails identityDetails = new IdentityDetails(result.Data);
					if (base.NeedSuppressingPiiData)
					{
						identityDetails = SuppressingPiiData.Redact(identityDetails);
					}
					list.Add(identityDetails);
				}
			}
			return list.ToArray();
		}

		private IdentityDetails[] GetIdentityDetails(ADRawEntry[] adRawEntries)
		{
			List<IdentityDetails> list = new List<IdentityDetails>(adRawEntries.Length);
			for (int i = 0; i < adRawEntries.Length; i++)
			{
				if (adRawEntries[i] != null)
				{
					IdentityDetails identityDetails = new IdentityDetails(adRawEntries[i]);
					if (base.NeedSuppressingPiiData)
					{
						identityDetails = SuppressingPiiData.Redact(identityDetails);
					}
					list.Add(identityDetails);
				}
			}
			return list.ToArray();
		}

		private string SuppressPiiDataAsNeeded(string value)
		{
			if (base.NeedSuppressingPiiData && value != null)
			{
				return SuppressingPiiData.Redact(value);
			}
			return value;
		}

		private const string ParameterExecutingUser = "ExecutingUser";

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.GroupMailbox
		};
	}
}
