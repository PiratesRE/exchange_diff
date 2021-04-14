using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class IdentityExtensions
	{
		public static Identity ToIdentity(this MailEnabledRecipient entry)
		{
			return entry.Id.ToIdentity(entry.DisplayName);
		}

		public static Identity ToIdentity(this ReducedRecipient entry)
		{
			return entry.Id.ToIdentity(entry.DisplayName);
		}

		public static Identity ToIdentity(this CASMailbox entry)
		{
			return entry.Id.ToIdentity(entry.DisplayName);
		}

		public static Identity ToIdentity(this ADObject entry)
		{
			return entry.Id.ToIdentity(entry.Name);
		}

		public static Identity ToIdentity(this ADObjectId identity)
		{
			if (identity != null)
			{
				return identity.ToIdentity(identity.Name);
			}
			return null;
		}

		public static Identity ToIdentity(this ADObjectId identity, string displayName)
		{
			if (identity != null)
			{
				return new Identity(identity, displayName);
			}
			return null;
		}

		public static Identity ToIdentity(this ObjectId identity, string displayName)
		{
			if (identity != null)
			{
				return new Identity(identity.ToString(), displayName);
			}
			return null;
		}

		public static Identity ToIdentity(this IConfigurable entry, string displayName)
		{
			return new Identity(entry.Identity.ToString(), displayName);
		}

		public static Identity ToIdentity(this SupervisionListEntry entry)
		{
			return new Identity(entry.Identity.ToString(), entry.EntryName);
		}

		public static Identity ToIdentity(this MailboxFolder entry)
		{
			string name = entry.Name;
			if (entry.DefaultFolderType == DefaultFolderType.Root)
			{
				Microsoft.Exchange.Data.Storage.Management.MailboxFolderId mailboxFolderId = (Microsoft.Exchange.Data.Storage.Management.MailboxFolderId)entry.Identity;
				if (ADObjectId.Equals(mailboxFolderId.MailboxOwnerId, EacRbacPrincipal.Instance.ExecutingUserId))
				{
					name = EacRbacPrincipal.Instance.Name;
				}
				else
				{
					name = mailboxFolderId.MailboxOwnerId.Name;
				}
			}
			return new Identity(entry.FolderStoreObjectId, name);
		}

		public static Identity ToIdentity(this ExtendedOrganizationalUnit ou)
		{
			return ou.Id.ToIdentity(ou.CanonicalName);
		}

		public static Identity ToIdentity(this InboxRuleId id)
		{
			if (id.StoreObjectId != null && id.MailboxOwnerId != null)
			{
				return new Identity(id.MailboxOwnerId.ObjectGuid.ToString() + '\\' + id.StoreObjectId.ToString(), id.Name);
			}
			return id.ToIdentity(id.Name);
		}

		public static Identity ToIdentity(this UMCallAnsweringRuleId id)
		{
			Guid ruleIdGuid = id.RuleIdGuid;
			if (id.MailboxOwnerId != null)
			{
				return new Identity(id.RuleIdGuid.ToString(), id.RuleIdGuid.ToString());
			}
			return id.ToIdentity(id.RuleIdGuid.ToString());
		}

		public static Identity ToIdentity(this AppId id)
		{
			if (id.AppIdValue != null && id.MailboxOwnerId != null)
			{
				return new Identity(id.MailboxOwnerId.ObjectGuid.ToString() + '\\' + id.AppIdValue, id.DisplayName);
			}
			return id.ToIdentity(id.DisplayName);
		}

		public static Identity ToIdentity(this AggregationSubscriptionIdentity id, string displayName)
		{
			return new Identity(id.GuidIdentityString, displayName);
		}

		public static Identity ToIdentity(this MobileDeviceConfiguration deviceInfo, string displayName)
		{
			return new Identity(deviceInfo.Guid.ToString(), displayName);
		}

		public static IEnumerable<Identity> ToIdentities(this IEnumerable<ADObjectId> identities)
		{
			return from identity in identities
			select identity.ToIdentity();
		}

		public static Identity ToIdentity(this MessageClassification[] classifications)
		{
			if (classifications.IsNullOrEmpty())
			{
				return null;
			}
			string[] value = Array.ConvertAll<MessageClassification, string>(classifications, (MessageClassification x) => x.DisplayName);
			string[] value2 = Array.ConvertAll<MessageClassification, string>(classifications, (MessageClassification x) => x.Guid.ToString());
			return new Identity(string.Join(",", value2), string.Join(",", value));
		}

		public static Identity ToIdentity(this AggregationSubscriptionIdentity[] subscriptionIds, InboxRule ruleObj)
		{
			IList<string> list = null;
			if (!subscriptionIds.IsNullOrEmpty())
			{
				list = ruleObj.GetSubscriptionEmailAddresses((IList<AggregationSubscriptionIdentity>)subscriptionIds);
			}
			if (list != null && list.Count > 0)
			{
				string[] value = Array.ConvertAll<AggregationSubscriptionIdentity, string>(subscriptionIds, (AggregationSubscriptionIdentity x) => x.ToString());
				return new Identity(string.Join(",", value), string.Join(",", list.ToArray<string>()));
			}
			return null;
		}

		public static Identity ToIdentity(this ExDateTime? dateTimeValue)
		{
			if (dateTimeValue == null)
			{
				return null;
			}
			return new Identity(dateTimeValue.ToUserDateTimeGeneralFormatString(), dateTimeValue.ToUserWeekdayDateString());
		}

		public static INamedIdentity ToIdParameter(this Identity identity)
		{
			if (null == identity)
			{
				return null;
			}
			return identity;
		}

		public static INamedIdentity ToMailboxFolderIdParameter(this Identity identity)
		{
			if (null != identity && identity.RawIdentity.IndexOf(':') < 0)
			{
				return new Identity(':' + identity.RawIdentity, identity.DisplayName);
			}
			return identity;
		}

		public static MailboxFolderPermissionIdentity ToMailboxFolderPermissionIdentity(this Identity identity)
		{
			if (null == identity)
			{
				return null;
			}
			string text = HttpUtility.UrlDecode(identity.RawIdentity);
			Identity[] array = Array.ConvertAll<string, Identity>(text.Split(new char[]
			{
				':'
			}), (string x) => Identity.ParseIdentity(x));
			return new MailboxFolderPermissionIdentity(array[0], array[1]);
		}

		[Conditional("DEBUG")]
		private static void EnsureMailboxFolderSeparator(string idString)
		{
			if (idString.IndexOf(':') == -1)
			{
				throw new ArgumentException("Identity must contain the mailbox folder seperator ('" + ':' + "') for conversion to MailboxFolderPermissionIdentity.");
			}
		}

		public static string[] ToTaskIdStringArray(this Identity identity)
		{
			if (identity == null || string.IsNullOrEmpty(identity.RawIdentity))
			{
				return null;
			}
			return new string[]
			{
				identity.RawIdentity
			};
		}

		public static INamedIdentity[] ToIdParameters(this IEnumerable<Identity> identities)
		{
			if (identities == null)
			{
				return null;
			}
			return (from identity in identities
			where identity != null
			select identity).ToArray<Identity>();
		}

		public static void FaultIfNull(this Identity identity)
		{
			if (null == identity)
			{
				throw new FaultException(new ArgumentNullException("identity").Message);
			}
		}

		public static void FaultIfNull(this Identity identity, string errorMessage)
		{
			if (null == identity)
			{
				throw new FaultException(errorMessage);
			}
		}

		public static void FaultIfNullOrEmpty(this Identity[] identities)
		{
			identities.FaultIfNull();
			if (identities.IsNullOrEmpty())
			{
				throw new FaultException(new ArgumentNullException("identities").Message);
			}
		}

		public static void FaultIfNotExactlyOne(this Identity[] identities)
		{
			identities.FaultIfNullOrEmpty();
			if (identities.Length > 1)
			{
				throw new FaultException(new ArgumentException("identities").Message);
			}
		}

		public static void FaultIfNull(this IEnumerable<Identity> identities)
		{
			if (identities != null)
			{
				if (!identities.Any((Identity x) => x == null))
				{
					return;
				}
			}
			throw new FaultException(new ArgumentNullException("identities").Message);
		}

		public static bool IsNullOrEmpty(this Array array)
		{
			return array == null || 0 == array.Length;
		}

		public static Identity ResolveByType(this Identity identity, IdentityType type)
		{
			Identity result = null;
			if (type == IdentityType.MailboxFolder)
			{
				PowerShellResults<MailboxFolder> @object = MailboxFolders.Instance.GetObject(identity);
				if (@object.SucceededWithValue)
				{
					result = @object.Value.Folder.ToIdentity();
				}
				return result;
			}
			throw new NotSupportedException();
		}
	}
}
