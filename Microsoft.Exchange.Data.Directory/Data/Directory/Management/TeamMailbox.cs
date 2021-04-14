using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class TeamMailbox : ADPresentationObject
	{
		public TeamMailbox()
		{
		}

		public TeamMailbox(ADUser dataObject) : base(dataObject)
		{
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			internal set
			{
				base.Name = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[TeamMailboxSchema.DisplayName];
			}
			internal set
			{
				this[TeamMailboxSchema.DisplayName] = value;
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[TeamMailboxSchema.PrimarySmtpAddress];
			}
		}

		public MultiValuedProperty<ADObjectId> Owners
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[TeamMailboxSchema.Owners];
			}
		}

		public MultiValuedProperty<ADObjectId> Members
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[TeamMailboxSchema.TeamMailboxMembers];
			}
		}

		public bool ShowInMyClient
		{
			get
			{
				return (bool)this[TeamMailboxSchema.TeamMailboxShowInMyClient];
			}
			internal set
			{
				this[TeamMailboxSchema.TeamMailboxShowInMyClient] = value;
			}
		}

		public bool RemoveDuplicateMessages
		{
			get
			{
				return (bool)this[TeamMailboxSchema.SiteMailboxMessageDedupEnabled];
			}
			internal set
			{
				this[TeamMailboxSchema.SiteMailboxMessageDedupEnabled] = value;
			}
		}

		public string MyRole
		{
			get
			{
				return (string)this[TeamMailboxSchema.TeamMailboxUserMembership];
			}
			internal set
			{
				this[TeamMailboxSchema.TeamMailboxUserMembership] = value;
			}
		}

		public ADObjectId SharePointLinkedBy
		{
			get
			{
				return (ADObjectId)this[TeamMailboxSchema.SharePointLinkedBy];
			}
			internal set
			{
				this[TeamMailboxSchema.SharePointLinkedBy] = value;
			}
		}

		public Uri SharePointUrl
		{
			get
			{
				return (Uri)this[TeamMailboxSchema.SharePointUrl];
			}
			internal set
			{
				this[TeamMailboxSchema.SharePointUrl] = value;
			}
		}

		public bool Active
		{
			get
			{
				return this[TeamMailboxSchema.TeamMailboxClosedTime] == null;
			}
		}

		public DateTime? ClosedTime
		{
			get
			{
				return (DateTime?)this[TeamMailboxSchema.TeamMailboxClosedTime];
			}
			internal set
			{
				this[TeamMailboxSchema.TeamMailboxClosedTime] = value;
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[TeamMailboxSchema.RecipientTypeDetails];
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[TeamMailboxSchema.EmailAddresses];
			}
		}

		public Uri WebCollectionUrl
		{
			get
			{
				return (Uri)this[TeamMailboxSchema.SiteMailboxWebCollectionUrl];
			}
		}

		public Guid WebId
		{
			get
			{
				return (Guid)this[TeamMailboxSchema.SiteMailboxWebId];
			}
		}

		internal MultiValuedProperty<ADObjectId> OwnersAndMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[TeamMailboxSchema.DelegateListLink];
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return TeamMailbox.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ADRecipient.TeamMailboxObjectVersion;
			}
		}

		internal static TeamMailbox FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new TeamMailbox(dataObject);
		}

		internal static bool IsActiveTeamMailbox(ADUser dataObject)
		{
			return dataObject != null && dataObject[TeamMailboxSchema.TeamMailboxClosedTime] == null;
		}

		internal static bool IsPendingDeleteSiteMailbox(ADUser dataObject)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			DateTime? closedTime = TeamMailbox.FromDataObject(dataObject).ClosedTime;
			return ((dataObject.DisplayName != null && dataObject.DisplayName.StartsWith("MDEL:")) || (closedTime != null && closedTime.Value.ToUniversalTime() == TeamMailbox.ClosedTimeOfMarkedForDeletion)) && dataObject.SharePointUrl == null;
		}

		internal static bool IsLocalTeamMailbox(ADUser dataObject)
		{
			return dataObject != null && dataObject.RecipientType == RecipientType.UserMailbox && dataObject.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox;
		}

		internal static bool IsRemoteTeamMailbox(ADUser dataObject)
		{
			return dataObject != null && dataObject.RecipientType == RecipientType.MailUser && (dataObject.RecipientDisplayType == RecipientDisplayType.SyncedTeamMailboxUser || dataObject.RecipientDisplayType == RecipientDisplayType.ACLableSyncedTeamMailboxUser);
		}

		internal static MultiValuedProperty<ADObjectId> MembersGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)propertyBag[TeamMailboxSchema.Owners];
			MultiValuedProperty<ADObjectId> multiValuedProperty2 = (MultiValuedProperty<ADObjectId>)propertyBag[TeamMailboxSchema.DelegateListLink];
			MultiValuedProperty<ADObjectId> multiValuedProperty3 = new MultiValuedProperty<ADObjectId>();
			foreach (ADObjectId item in multiValuedProperty2)
			{
				if (!multiValuedProperty.Contains(item))
				{
					multiValuedProperty3.Add(item);
				}
			}
			return multiValuedProperty3;
		}

		internal static Uri WebCollectionUrlGetter(IPropertyBag propertyBag)
		{
			string sharePointSiteInfo = (string)propertyBag[ADUserSchema.SharePointSiteInfo];
			string urlString;
			string text;
			TeamMailbox.ParseSharePointSiteInfo(sharePointSiteInfo, out urlString, out text);
			return TeamMailbox.GetUrl(urlString);
		}

		internal static object WebIdGetter(IPropertyBag propertyBag)
		{
			string sharePointSiteInfo = (string)propertyBag[ADUserSchema.SharePointSiteInfo];
			string text;
			string guidString;
			TeamMailbox.ParseSharePointSiteInfo(sharePointSiteInfo, out text, out guidString);
			return TeamMailbox.GetGuid(guidString);
		}

		internal static Uri GetUrl(string urlString)
		{
			Uri result = null;
			if (!string.IsNullOrEmpty(urlString))
			{
				try
				{
					result = new Uri(urlString);
				}
				catch (UriFormatException)
				{
				}
			}
			return result;
		}

		internal static Guid GetGuid(string guidString)
		{
			Guid result = Guid.Empty;
			if (!string.IsNullOrEmpty(guidString))
			{
				try
				{
					result = new Guid(guidString);
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
			}
			return result;
		}

		private static void ParseSharePointSiteInfo(string sharePointSiteInfo, out string webCollectionUrl, out string webId)
		{
			webCollectionUrl = string.Empty;
			webId = string.Empty;
			if (!string.IsNullOrEmpty(sharePointSiteInfo))
			{
				foreach (string text in sharePointSiteInfo.Split(new char[]
				{
					';'
				}))
				{
					if (text.StartsWith("WebCollectionUrl", StringComparison.OrdinalIgnoreCase))
					{
						webCollectionUrl = text.Substring("WebCollectionUrl".Length + 1);
					}
					else if (text.StartsWith("WebId", StringComparison.OrdinalIgnoreCase))
					{
						webId = text.Substring("WebId".Length + 1);
					}
				}
			}
		}

		internal static List<ADObjectId> MergeUsers(IList<ADObjectId> userList1, IList<ADObjectId> userList2)
		{
			if (userList1 == null)
			{
				throw new ArgumentNullException("userList1");
			}
			if (userList2 == null)
			{
				throw new ArgumentNullException("userList2");
			}
			List<ADObjectId> list = new List<ADObjectId>(userList1);
			foreach (ADObjectId item in userList2)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		internal static IList<ADObjectId> DiffUsers(IList<ADObjectId> userList1, IList<ADObjectId> userList2)
		{
			if (userList1 == null)
			{
				throw new ArgumentNullException("userList1");
			}
			if (userList2 == null)
			{
				throw new ArgumentNullException("userList2");
			}
			Dictionary<ADObjectId, bool> dictionary = new Dictionary<ADObjectId, bool>();
			foreach (ADObjectId key in userList2)
			{
				dictionary[key] = true;
			}
			Dictionary<ADObjectId, bool> dictionary2 = new Dictionary<ADObjectId, bool>();
			foreach (ADObjectId key2 in userList1)
			{
				if (!dictionary.ContainsKey(key2))
				{
					dictionary2[key2] = true;
				}
			}
			return new List<ADObjectId>(dictionary2.Keys);
		}

		internal void SetPolicy(TeamMailboxProvisioningPolicy policy)
		{
			if (policy == null)
			{
				throw new ArgumentNullException("policy");
			}
			ADUser aduser = (ADUser)base.DataObject;
			aduser.IssueWarningQuota = policy.IssueWarningQuota;
			aduser.MaxReceiveSize = policy.MaxReceiveSize;
			aduser.ProhibitSendReceiveQuota = policy.ProhibitSendReceiveQuota;
			aduser.UseDatabaseQuotaDefaults = new bool?(false);
		}

		internal void SetSharePointSiteInfo(Uri webCollectionUrl, Guid webId)
		{
			this[TeamMailboxSchema.SharePointSiteInfo] = string.Format("{0}:{1};{2}:{3}", new object[]
			{
				"WebCollectionUrl",
				(webCollectionUrl == null) ? string.Empty : webCollectionUrl.AbsoluteUri,
				"WebId",
				(webId == Guid.Empty) ? string.Empty : webId.ToString()
			});
		}

		internal void SetMyRole(ADObjectId executingUserId)
		{
			MyRoleType myRoleType = MyRoleType.NoAccess;
			if (executingUserId != null)
			{
				if (this.Owners.Contains(executingUserId))
				{
					myRoleType = MyRoleType.Owner;
				}
				else if (this.Members.Contains(executingUserId))
				{
					myRoleType = MyRoleType.Member;
				}
			}
			this.MyRole = myRoleType.ToString();
		}

		internal const int TotalOwnersAndMembersLimit = 1800;

		internal const string NamePrefixOfMarkedForDeletion = "MDEL:";

		internal const int CsomRequestTimeoutInMilliSec = 60000;

		internal const int MaxPinnedInClient = 10;

		internal const bool ShowInMyClientDefaultValue = true;

		private const string WebCollectionUrlKey = "WebCollectionUrl";

		private const string WebIdKey = "WebId";

		internal static DateTime? ClosedTimeOfMarkedForDeletion = new DateTime?(new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc));

		private static TeamMailboxSchema schema = ObjectSchema.GetInstance<TeamMailboxSchema>();
	}
}
