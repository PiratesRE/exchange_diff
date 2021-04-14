using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	internal sealed class UnifiedGroupADAccessLayer
	{
		internal UnifiedGroupADAccessLayer(ADUser groupMailbox, string preferredDC)
		{
			if (groupMailbox.RecipientDisplayType == null || groupMailbox.RecipientDisplayType != RecipientDisplayType.GroupMailboxUser)
			{
				throw new InvalidOperationException("Update membership is only allowed on GroupMailbox. Recipient type:" + groupMailbox.RecipientDisplayType);
			}
			this.groupMailbox = groupMailbox;
			this.preferredDC = preferredDC;
			this.sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.groupMailbox.OrganizationId);
		}

		private IRecipientSession ReadWriteADSession
		{
			get
			{
				if (this.readWriteSession == null)
				{
					this.readWriteSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.preferredDC, false, ConsistencyMode.IgnoreInvalid, this.sessionSettings, 90, "ReadWriteADSession", "f:\\15.00.1497\\sources\\dev\\UnifiedGroups\\src\\UnifiedGroups\\GroupMailboxAccessLayer\\UnifiedGroupADAccessLayer.cs");
				}
				return this.readWriteSession;
			}
		}

		private IRecipientSession ReadOnlyAdSession
		{
			get
			{
				if (this.readOnlySession == null)
				{
					this.readOnlySession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.preferredDC, true, ConsistencyMode.IgnoreInvalid, this.sessionSettings, 111, "ReadOnlyAdSession", "f:\\15.00.1497\\sources\\dev\\UnifiedGroups\\src\\UnifiedGroups\\GroupMailboxAccessLayer\\UnifiedGroupADAccessLayer.cs");
				}
				return this.readOnlySession;
			}
		}

		public static ADPagedReader<ADRawEntry> GetAllGroupMembers(IRecipientSession recipientSession, ADObjectId groupId, IEnumerable<PropertyDefinition> properties, SortBy sortBy, QueryFilter searchFilter = null, int pageSize = 0)
		{
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, IUnifiedGroupMailboxSchema.UnifiedGroupMembersBL, groupId),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADUser.MostDerivedClass)
			});
			if (searchFilter != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					searchFilter
				});
			}
			return recipientSession.FindPagedADRawEntry(null, QueryScope.SubTree, queryFilter, sortBy, pageSize, properties);
		}

		internal void UpdateMembership(ADUser[] addedMembers, ADUser[] removedMembers)
		{
			if (addedMembers.IsNullOrEmpty<ADUser>() && removedMembers.IsNullOrEmpty<ADUser>())
			{
				return;
			}
			ModifyRequest modifyRequest = new ModifyRequest
			{
				DistinguishedName = this.groupMailbox.Id.DistinguishedName
			};
			modifyRequest.Controls.Add(new PermissiveModifyControl());
			this.PopulateModifyRequest(modifyRequest, IUnifiedGroupMailboxSchema.UnifiedGroupMembersLink.LdapDisplayName, DirectoryAttributeOperation.Delete, removedMembers);
			this.PopulateModifyRequest(modifyRequest, IUnifiedGroupMailboxSchema.UnifiedGroupMembersLink.LdapDisplayName, DirectoryAttributeOperation.Add, addedMembers);
			this.ReadWriteADSession.UnsafeExecuteModificationRequest(modifyRequest, this.groupMailbox.Id);
		}

		internal IEnumerable<UnifiedGroupParticipant> GetMembers(bool sortByDisplayName = true, int sizeLimit = 100)
		{
			return this.GetMembersInternal(sortByDisplayName, sizeLimit, null);
		}

		internal IEnumerable<UnifiedGroupParticipant> GetMembersByAnrMatch(string anrToMatch, bool sortByDisplayName = true, int sizeLimit = 100)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("anrToMatch", anrToMatch);
			if (anrToMatch.Length > 255)
			{
				anrToMatch = anrToMatch.Substring(0, 255);
			}
			return this.GetMembersInternal(sortByDisplayName, sizeLimit, anrToMatch);
		}

		private IEnumerable<UnifiedGroupParticipant> GetMembersInternal(bool sortByDisplayName, int sizeLimit, string anrToMatch = null)
		{
			QueryFilter anrFilter = null;
			if (!string.IsNullOrEmpty(anrToMatch))
			{
				anrToMatch = anrToMatch.Trim();
				if (anrToMatch.Length == 1)
				{
					anrFilter = QueryFilter.OrTogether(new QueryFilter[]
					{
						new TextFilter(ADUserSchema.FirstName, anrToMatch, MatchOptions.Prefix, MatchFlags.IgnoreCase),
						new TextFilter(ADUserSchema.LastName, anrToMatch, MatchOptions.Prefix, MatchFlags.IgnoreCase)
					});
				}
				else
				{
					anrFilter = new AmbiguousNameResolutionFilter(anrToMatch);
				}
			}
			ADPagedReader<ADRawEntry> pagedReader = UnifiedGroupADAccessLayer.GetAllGroupMembers(this.ReadOnlyAdSession, this.groupMailbox.Id, UnifiedGroupParticipant.DefaultMemberProperties, sortByDisplayName ? new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending) : null, anrFilter, 0);
			HashSet<Guid> ownerSet = new HashSet<Guid>();
			foreach (ADObjectId adobjectId in this.groupMailbox.Owners)
			{
				ownerSet.Add(adobjectId.ObjectGuid);
			}
			int counter = 0;
			foreach (ADRawEntry item in pagedReader)
			{
				if (sizeLimit > 0)
				{
					counter++;
					if (counter > sizeLimit)
					{
						break;
					}
				}
				yield return this.SetOwnership(UnifiedGroupParticipant.CreateFromADRawEntry(item), ownerSet);
			}
			yield break;
		}

		private UnifiedGroupParticipant SetOwnership(UnifiedGroupParticipant participant, HashSet<Guid> ownerSet)
		{
			if (ownerSet.Contains(participant.Id.ObjectGuid))
			{
				participant.IsOwner = true;
			}
			return participant;
		}

		private void PopulateModifyRequest(ModifyRequest request, string name, DirectoryAttributeOperation operation, IEnumerable<ADUser> members)
		{
			if (members != null)
			{
				DirectoryAttributeModification directoryAttributeModification = new DirectoryAttributeModification
				{
					Name = name,
					Operation = operation
				};
				foreach (ADUser aduser in members)
				{
					directoryAttributeModification.Add(aduser.Id.ToGuidOrDNString());
				}
				if (directoryAttributeModification.Count > 0)
				{
					request.Modifications.Add(directoryAttributeModification);
				}
			}
		}

		private const int DefaultSizeLimit = 100;

		private const int MaxQueryStringLength = 255;

		private readonly ADUser groupMailbox;

		private readonly string preferredDC;

		private readonly ADSessionSettings sessionSettings;

		private IRecipientSession readWriteSession;

		private IRecipientSession readOnlySession;
	}
}
