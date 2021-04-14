using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.AAD;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	public abstract class UnifiedGroupTask : Task
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		protected OrganizationId OrganizationId
		{
			get
			{
				if (this.organizationId == null)
				{
					try
					{
						this.organizationId = MapiTaskHelper.ResolveTargetOrganization(null, this.Organization, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, OrganizationId.ForestWideOrgId);
					}
					catch (LocalizedException ex)
					{
						this.WriteVerbose("Unable to resolve Organization parameter due exception: {0}", new object[]
						{
							ex
						});
					}
					if (this.organizationId == null)
					{
						base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), ExchangeErrorCategory.Client, null);
					}
				}
				return this.organizationId;
			}
		}

		protected void WriteVerbose(string format, params object[] args)
		{
			base.WriteVerbose(new LocalizedString(string.Format(format, args)));
		}

		internal ExchangeErrorCategory GetErrorCategory(AADException exception)
		{
			if (exception is AADDataException)
			{
				return ExchangeErrorCategory.Client;
			}
			return ExchangeErrorCategory.ServerOperation;
		}

		internal IRecipientSession GetRecipientSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.OrganizationId);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, CultureInfo.CurrentCulture.LCID, false, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 115, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Support\\FederatedDirectory\\UnifiedGroupTask.cs");
		}

		internal string[] GetOwners(ADUser groupMailbox, ADUser creator, IRecipientSession recipientSession)
		{
			ADObjectId[] array = ((MultiValuedProperty<ADObjectId>)groupMailbox[GroupMailboxSchema.Owners]).ToArray();
			this.WriteVerbose("Getting owners from group mailbox: {0}", new object[]
			{
				groupMailbox.Id
			});
			Result<ADRawEntry>[] array2 = recipientSession.ReadMultiple(array, new PropertyDefinition[]
			{
				ADRecipientSchema.ExternalDirectoryObjectId
			});
			List<string> list = new List<string>(array2.Length);
			for (int i = 0; i < array2.Length; i++)
			{
				Result<ADRawEntry> result = array2[i];
				if (result.Error != null)
				{
					this.WriteWarning(Strings.WarningUnableToResolveUser(array[i].ToString(), result.Error.ToString()));
				}
				else if (result.Data == null)
				{
					this.WriteWarning(Strings.WarningUnableToResolveUser(array[i].ToString(), string.Empty));
				}
				else
				{
					string text = result.Data[ADRecipientSchema.ExternalDirectoryObjectId] as string;
					if (string.IsNullOrEmpty(text))
					{
						this.WriteWarning(Strings.WarningUnableToResolveUser(array[i].ToString(), string.Empty));
					}
					else if (creator == null || !StringComparer.OrdinalIgnoreCase.Equals(text, creator.ExternalDirectoryObjectId))
					{
						list.Add(text);
						this.WriteVerbose("Group owner: {0}, ExternalDirectoryObjectId={1}", new object[]
						{
							result.Data.Id,
							text
						});
					}
				}
			}
			return list.ToArray();
		}

		internal string[] GetMembers(ADUser groupMailbox, IRecipientSession recipientSession, string operation)
		{
			List<string> externalDirectoryObjectIds = new List<string>(10);
			this.WriteVerbose("Getting members from group mailbox: {0}", new object[]
			{
				groupMailbox.Id
			});
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(ExchangePrincipal.FromADUser(groupMailbox, RemotingOptions.AllowCrossSite), CultureInfo.InvariantCulture, "Client=Management;Action=" + operation))
			{
				GroupMailboxAccessLayer.Execute(operation, recipientSession, mailboxSession, delegate(GroupMailboxAccessLayer accessLayer)
				{
					GroupMailboxLocator group = GroupMailboxLocator.Instantiate(recipientSession, groupMailbox);
					IEnumerable<UserMailbox> members = accessLayer.GetMembers(group, false, null);
					foreach (UserMailbox userMailbox in members)
					{
						if (string.IsNullOrEmpty(userMailbox.Locator.ExternalId))
						{
							this.WriteVerbose("Group member is missing ExternalId: {0}", new object[]
							{
								userMailbox.Locator.LegacyDn
							});
						}
						else
						{
							externalDirectoryObjectIds.Add(userMailbox.Locator.ExternalId);
							this.WriteVerbose("Group member: {0}, ExternalDirectoryObjectId={1}", new object[]
							{
								userMailbox.Locator.LegacyDn,
								userMailbox.Locator.ExternalId
							});
						}
					}
				});
			}
			return externalDirectoryObjectIds.ToArray();
		}

		internal void AddMembersInAAD(string[] members, AADClient aadClient, string groupObjectId)
		{
			AADClient.LinkResult[] array = aadClient.AddMembers(groupObjectId, members);
			if (array != null)
			{
				this.WriteWarning(Strings.WarningUnableToAddMembers(string.Join(",", (from linkResult in array
				select linkResult.FailedLink).ToArray<string>())));
			}
		}

		internal void RemoveMembersInAAD(string[] members, AADClient aadClient, string groupObjectId)
		{
			AADClient.LinkResult[] array = aadClient.RemoveMembers(groupObjectId, members);
			if (array != null)
			{
				this.WriteWarning(Strings.WarningUnableToRemoveMembers(string.Join(",", (from linkResult in array
				select linkResult.FailedLink).ToArray<string>())));
			}
		}

		internal void AddOwnersInAAD(string[] owners, AADClient aadClient, string groupObjectId)
		{
			AADClient.LinkResult[] array = aadClient.AddOwners(groupObjectId, owners);
			if (array != null)
			{
				this.WriteWarning(Strings.WarningUnableToAddOwners(string.Join(",", (from linkResult in array
				select linkResult.FailedLink).ToArray<string>())));
			}
		}

		internal void RemoveOwnersInAAD(string[] owners, AADClient aadClient, string groupObjectId)
		{
			AADClient.LinkResult[] array = aadClient.RemoveOwners(groupObjectId, owners);
			if (array != null)
			{
				this.WriteWarning(Strings.WarningUnableToRemoveOwners(string.Join(",", (from linkResult in array
				select linkResult.FailedLink).ToArray<string>())));
			}
		}

		internal T[] ResolveRecipientIdParameters<T>(RecipientIdParameter[] recipientIdParameters, IRecipientSession recipientSession) where T : ADRecipient, new()
		{
			if (recipientIdParameters == null)
			{
				return null;
			}
			T[] array = new T[recipientIdParameters.Length];
			for (int i = 0; i < recipientIdParameters.Length; i++)
			{
				array[i] = this.ResolveRecipientIdParameter<T>(recipientIdParameters[i], recipientSession);
			}
			return array;
		}

		internal T ResolveRecipientIdParameter<T>(RecipientIdParameter recipientIdParameter, IRecipientSession recipientSession) where T : ADRecipient, new()
		{
			if (recipientIdParameter == null)
			{
				return default(T);
			}
			IEnumerable<T> objects = recipientIdParameter.GetObjects<T>(null, recipientSession);
			T result = default(T);
			using (IEnumerator<T> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					base.WriteError(new TaskException(Strings.NoRecipientsForRecipientId(recipientIdParameter.ToString())), ExchangeErrorCategory.Client, null);
				}
				result = enumerator.Current;
				if (enumerator.MoveNext())
				{
					base.WriteError(new TaskException(Strings.MoreThanOneRecipientForRecipientId(recipientIdParameter.ToString())), ExchangeErrorCategory.Client, null);
				}
			}
			if (string.IsNullOrEmpty(result.ExternalDirectoryObjectId))
			{
				base.WriteError(new TaskException(Strings.NoExternalDirectoryObjectIdForRecipientId(recipientIdParameter.ToString())), ExchangeErrorCategory.Client, null);
			}
			return result;
		}

		private OrganizationId organizationId;
	}
}
