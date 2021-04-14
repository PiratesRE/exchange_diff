using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data.Directory.IsMemberOfProvider
{
	internal abstract class IsMemberOfResolverADAdapter<GroupKeyType>
	{
		public IsMemberOfResolverADAdapter(bool disableDynamicGroups)
		{
			this.disableDynamicGroups = disableDynamicGroups;
		}

		public virtual ResolvedGroup ResolveGroup(IRecipientSession session, GroupKeyType group, out int ldapQueries)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			ResolvedGroup resolvedGroup = null;
			int numQueries = 0;
			this.RunADOperation(group, delegate
			{
				Guid groupGuid;
				Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType;
				if (this.TryGetGroupInfo(session, group, out groupGuid, out recipientType, out numQueries) && this.IsGroup(recipientType))
				{
					resolvedGroup = new ResolvedGroup(groupGuid);
				}
			});
			ldapQueries = numQueries;
			return resolvedGroup;
		}

		public virtual ExpandedGroup ExpandGroup(IRecipientSession session, ADObjectId groupId, out int ldapQueries)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			ExpandedGroup expandedGroup = null;
			int numQueries = 0;
			this.RunADOperation(groupId, delegate
			{
				IADDistributionList iaddistributionList = session.Read(groupId) as IADDistributionList;
				numQueries++;
				if (iaddistributionList != null)
				{
					if (this.disableDynamicGroups && iaddistributionList is ADDynamicGroup)
					{
						expandedGroup = null;
						return;
					}
					List<Guid> list = new List<Guid>();
					List<Guid> list2 = new List<Guid>();
					ADPagedReader<ADRawEntry> adpagedReader = iaddistributionList.Expand(1000, IsMemberOfResolverADAdapter<GroupKeyType>.properties);
					numQueries++;
					foreach (ADRawEntry adrawEntry in adpagedReader)
					{
						Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType = (Microsoft.Exchange.Data.Directory.Recipient.RecipientType)adrawEntry[ADRecipientSchema.RecipientType];
						if (this.IsGroup(recipientType))
						{
							list.Add(adrawEntry.Id.ObjectGuid);
						}
						else
						{
							list2.Add(adrawEntry.Id.ObjectGuid);
						}
					}
					expandedGroup = this.CreateExpandedGroup((ADObject)iaddistributionList, list, list2);
				}
			});
			ldapQueries = numQueries;
			return expandedGroup;
		}

		protected abstract bool TryGetGroupInfo(IRecipientSession session, GroupKeyType group, out Guid objectGuid, out Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType, out int ldapQueries);

		protected virtual ExpandedGroup CreateExpandedGroup(ADObject group, List<Guid> memberGroups, List<Guid> memberRecipients)
		{
			return new ExpandedGroup(memberGroups, memberRecipients);
		}

		private bool IsGroup(Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType)
		{
			return recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Group || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup || (!this.disableDynamicGroups && recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.DynamicDistributionGroup);
		}

		private void RunADOperation(object group, ADOperation adOperation)
		{
			try
			{
				ADNotificationAdapter.RunADOperation(adOperation);
			}
			catch (TransientException e)
			{
				this.RethrowException(group, e);
			}
			catch (DataValidationException e2)
			{
				this.RethrowException(group, e2);
			}
		}

		private void RethrowException(object group, Exception e)
		{
			string text = string.Empty;
			if (group != null)
			{
				text = group.ToString();
			}
			IsMemberOfResolver<GroupKeyType>.Tracer.TraceDebug<string, Exception>((long)this.GetHashCode(), "AD query for group {0} failed with exception: {1}", text, e);
			throw new AddressBookTransientException(DirectoryStrings.IsMemberOfQueryFailed(text), e);
		}

		private const int PageSize = 1000;

		private static PropertyDefinition[] properties = new PropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.RecipientType
		};

		protected bool disableDynamicGroups;

		public class RoutingAddressResolver : IsMemberOfResolverADAdapter<RoutingAddress>
		{
			public RoutingAddressResolver(bool disableDynamicGroups) : base(disableDynamicGroups)
			{
			}

			protected override bool TryGetGroupInfo(IRecipientSession session, RoutingAddress group, out Guid objectGuid, out Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType, out int ldapQueries)
			{
				ldapQueries = 0;
				if (group.IsValid)
				{
					ADRawEntry adrawEntry = session.FindByProxyAddress(new SmtpProxyAddress((string)group, false), IsMemberOfResolverADAdapter<RoutingAddress>.properties);
					ldapQueries = 1;
					if (adrawEntry != null)
					{
						objectGuid = adrawEntry.Id.ObjectGuid;
						recipientType = (Microsoft.Exchange.Data.Directory.Recipient.RecipientType)adrawEntry[ADRecipientSchema.RecipientType];
						return true;
					}
				}
				objectGuid = Guid.Empty;
				recipientType = Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Invalid;
				return false;
			}
		}

		public class LegacyDNResolver : IsMemberOfResolverADAdapter<string>
		{
			public LegacyDNResolver(bool disableDynamicGroups) : base(disableDynamicGroups)
			{
			}

			protected override bool TryGetGroupInfo(IRecipientSession session, string group, out Guid objectGuid, out Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType, out int ldapQueries)
			{
				ldapQueries = 0;
				if (!string.IsNullOrEmpty(group))
				{
					Result<ADRawEntry>[] array = session.FindByLegacyExchangeDNs(new string[]
					{
						group
					}, IsMemberOfResolverADAdapter<RoutingAddress>.properties);
					ldapQueries = 1;
					if (array != null && array.Length > 0 && array[0].Data != null)
					{
						ADRawEntry data = array[0].Data;
						objectGuid = data.Id.ObjectGuid;
						recipientType = (Microsoft.Exchange.Data.Directory.Recipient.RecipientType)data[ADRecipientSchema.RecipientType];
						return true;
					}
				}
				objectGuid = Guid.Empty;
				recipientType = Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Invalid;
				return false;
			}
		}
	}
}
