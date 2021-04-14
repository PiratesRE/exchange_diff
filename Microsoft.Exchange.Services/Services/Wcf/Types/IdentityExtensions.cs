using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	public static class IdentityExtensions
	{
		public static Identity ToIdentity(this ADObjectId source)
		{
			if (source != null)
			{
				return source.ToIdentity(source.Name);
			}
			return null;
		}

		public static Identity ToIdentity(this ADObjectId source, string displayName)
		{
			if (source != null)
			{
				return new Identity(source, displayName);
			}
			return null;
		}

		public static Identity ToIdentity(this AggregationSubscriptionIdentity source, string displayName)
		{
			if (source != null)
			{
				return new Identity(source.GuidIdentityString, displayName);
			}
			return null;
		}

		public static Identity ToIdentity(this MailboxFolder source)
		{
			if (source == null)
			{
				return null;
			}
			string displayName = source.Name;
			if (source.DefaultFolderType == DefaultFolderType.Root)
			{
				MailboxFolderId mailboxFolderId = (MailboxFolderId)source.Identity;
				if (ADObjectId.Equals(mailboxFolderId.MailboxOwnerId, CallContext.Current.AccessingADUser.Id))
				{
					displayName = CallContext.Current.AccessingPrincipal.MailboxInfo.DisplayName;
				}
				else
				{
					displayName = mailboxFolderId.MailboxOwnerId.Name;
				}
			}
			return new Identity(source.Identity.ToString(), displayName);
		}

		public static Identity[] ToIdentityArray(this IEnumerable<ADObjectId> source)
		{
			if (source == null || !source.Any<ADObjectId>())
			{
				return null;
			}
			IEnumerable<Identity> source2 = from e in source
			select e.ToIdentity();
			return source2.ToArray<Identity>();
		}

		public static Identity[] ToIdentityArray(this IEnumerable<MessageClassification> classifications)
		{
			if (classifications == null || !classifications.Any<MessageClassification>())
			{
				return null;
			}
			IEnumerable<Identity> source = from e in classifications
			select new Identity(e.Guid.ToString(), e.DisplayName);
			return source.ToArray<Identity>();
		}

		public static Identity[] ToIdentityArray(this AggregationSubscriptionIdentity[] subscriptionIds, InboxRule ruleObj)
		{
			IList<string> list = null;
			if (!subscriptionIds.IsNullOrEmpty<AggregationSubscriptionIdentity>())
			{
				list = ruleObj.GetSubscriptionEmailAddresses(subscriptionIds);
			}
			if (list == null || list.Count == 0)
			{
				return null;
			}
			List<Identity> list2 = new List<Identity>();
			for (int i = 0; i < subscriptionIds.Length; i++)
			{
				list2.Add(new Identity(subscriptionIds[i].ToString(), list[i]));
			}
			return list2.ToArray();
		}

		public static T ToIdParameter<T>(this Identity identity) where T : ADIdParameter
		{
			if (!(null == identity))
			{
				return (T)((object)Activator.CreateInstance(typeof(T), new object[]
				{
					identity
				}));
			}
			return default(T);
		}

		public static IEnumerable<AggregationSubscriptionIdentity> ToIdParameters(this IEnumerable<Identity> identities)
		{
			if (identities == null || !identities.Any<Identity>())
			{
				return null;
			}
			return from identity in identities
			where identity != null
			select new AggregationSubscriptionIdentity(identity.RawIdentity);
		}

		public static IEnumerable<T> ToIdParameters<T>(this IEnumerable<Identity> identities) where T : ADIdParameter
		{
			if (identities == null || !identities.Any<Identity>())
			{
				return null;
			}
			return from identity in identities
			where identity != null
			select identity.ToIdParameter<T>();
		}

		public static MailboxFolderIdParameter ToMailboxFolderIdParameter(this Identity identity)
		{
			if (!(null != identity))
			{
				return null;
			}
			if (identity.RawIdentity.IndexOf(':') < 0)
			{
				string arg = IdConverter.EwsIdToMessageStoreObjectId(identity.RawIdentity).ToString();
				return new MailboxFolderIdParameter(new Identity(':' + arg, identity.DisplayName));
			}
			return new MailboxFolderIdParameter(identity);
		}
	}
}
