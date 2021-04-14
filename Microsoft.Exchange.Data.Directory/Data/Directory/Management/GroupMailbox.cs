using System;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class GroupMailbox : ADPresentationObject
	{
		public GroupMailbox()
		{
		}

		public GroupMailbox(ADUser dataObject) : base(dataObject)
		{
		}

		public string Alias
		{
			get
			{
				return (string)this[GroupMailboxSchema.Alias];
			}
		}

		public string CalendarUrl { get; internal set; }

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[GroupMailboxSchema.Database];
			}
		}

		public string Description
		{
			get
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)this[GroupMailboxSchema.Description];
				if (multiValuedProperty != null && multiValuedProperty.Count > 0)
				{
					return multiValuedProperty[0];
				}
				return string.Empty;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[GroupMailboxSchema.DisplayName];
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[GroupMailboxSchema.EmailAddresses];
			}
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[GroupMailboxSchema.ExchangeGuid];
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this[GroupMailboxSchema.ExternalDirectoryObjectId];
			}
		}

		public string InboxUrl { get; internal set; }

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[GroupMailboxSchema.LegacyExchangeDN];
			}
		}

		public CultureInfo Language
		{
			get
			{
				MultiValuedProperty<CultureInfo> source = (MultiValuedProperty<CultureInfo>)this[GroupMailboxSchema.Languages];
				return source.FirstOrDefault<CultureInfo>();
			}
		}

		public ADObjectId[] Members { get; internal set; }

		public IdentityDetails[] MembersDetails { get; internal set; }

		public GroupMailboxMembersSyncStatus MembersSyncStatus { get; internal set; }

		public ModernGroupObjectType ModernGroupType
		{
			get
			{
				return (ModernGroupObjectType)this[GroupMailboxSchema.ModernGroupType];
			}
		}

		internal MultiValuedProperty<SecurityIdentifier> PublicToGroupSids
		{
			get
			{
				return (MultiValuedProperty<SecurityIdentifier>)this[GroupMailboxSchema.PublicToGroupSids];
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		public MultiValuedProperty<ADObjectId> Owners
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[GroupMailboxSchema.Owners];
			}
		}

		public IdentityDetails[] OwnersDetails { get; internal set; }

		public string PeopleUrl { get; internal set; }

		public string PhotoUrl { get; internal set; }

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[GroupMailboxSchema.PrimarySmtpAddress];
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[GroupMailboxSchema.RecipientTypeDetails];
			}
		}

		public bool RequireSenderAuthenticationEnabled
		{
			get
			{
				return (bool)this[GroupMailboxSchema.RequireSenderAuthenticationEnabled];
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[GroupMailboxSchema.ServerName];
			}
		}

		public Uri SharePointUrl
		{
			get
			{
				return (Uri)this[GroupMailboxSchema.SharePointUrl];
			}
		}

		public string SharePointSiteUrl
		{
			get
			{
				return (string)this[GroupMailboxSchema.SharePointSiteUrl];
			}
		}

		public string SharePointDocumentsUrl
		{
			get
			{
				return (string)this[GroupMailboxSchema.SharePointDocumentsUrl];
			}
		}

		public bool IsMailboxConfigured
		{
			get
			{
				return (bool)this[GroupMailboxSchema.IsMailboxConfigured];
			}
		}

		public bool IsExternalResourcesPublished
		{
			get
			{
				return (bool)this[GroupMailboxSchema.IsExternalResourcesPublished];
			}
		}

		public string YammerGroupEmailAddress
		{
			get
			{
				return (string)this[GroupMailboxSchema.YammerGroupEmailAddress];
			}
		}

		public string PermissionsVersion { get; internal set; }

		public bool AutoSubscribeNewGroupMembers
		{
			get
			{
				return (bool)this[GroupMailboxSchema.AutoSubscribeNewGroupMembers];
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return GroupMailbox.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ADRecipient.TeamMailboxObjectVersion;
			}
		}

		internal static GroupMailbox FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new GroupMailbox(dataObject);
		}

		internal static bool IsLocalGroupMailbox(ADUser dataObject)
		{
			return dataObject.RecipientType == RecipientType.UserMailbox && dataObject.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox;
		}

		internal static MultiValuedProperty<SecurityIdentifier> PublicToGroupSidsGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)propertyBag[GroupMailboxSchema.DelegateListLink];
			ModernGroupObjectType modernGroupObjectType = (ModernGroupObjectType)propertyBag[GroupMailboxSchema.ModernGroupType];
			MultiValuedProperty<SecurityIdentifier> multiValuedProperty2 = new MultiValuedProperty<SecurityIdentifier>();
			if (multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				using (MultiValuedProperty<ADObjectId>.Enumerator enumerator = multiValuedProperty.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ADObjectId instance = enumerator.Current;
						SecurityIdentifier securityIdentifier = ADObjectId.GetSecurityIdentifier(instance);
						if (securityIdentifier != null)
						{
							multiValuedProperty2.Add(securityIdentifier);
						}
					}
					return multiValuedProperty2;
				}
			}
			if (modernGroupObjectType == ModernGroupObjectType.Public)
			{
				SecurityIdentifier item = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
				multiValuedProperty2.Add(item);
			}
			return multiValuedProperty2;
		}

		internal static string SharePointSiteUrlGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> sharePointResources = (MultiValuedProperty<string>)propertyBag[ADMailboxRecipientSchema.SharePointResources];
			return GroupMailbox.ExtractSharePointResource(sharePointResources, "SiteUrl");
		}

		internal static string SharePointDocumentsUrlGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> sharePointResources = (MultiValuedProperty<string>)propertyBag[ADMailboxRecipientSchema.SharePointResources];
			return GroupMailbox.ExtractSharePointResource(sharePointResources, "DocumentsUrl");
		}

		private static string ExtractSharePointResource(MultiValuedProperty<string> sharePointResources, string resourceKey)
		{
			string result = null;
			if (sharePointResources != null)
			{
				foreach (string text in sharePointResources)
				{
					if (text.StartsWith(resourceKey + "=", StringComparison.OrdinalIgnoreCase))
					{
						result = text.Substring(resourceKey.Length + 1);
						break;
					}
				}
			}
			return result;
		}

		public const string SiteUrl = "SiteUrl";

		public const string DocumentsUrl = "DocumentsUrl";

		public const string ResourceSeparator = "=";

		private static GroupMailboxSchema schema = ObjectSchema.GetInstance<GroupMailboxSchema>();

		public class GroupIdentityOwnerConstants
		{
			public const string Exchange = "Exchange";

			public const string AADCollabSpace = "AADCollabspace";

			public const string AADGroup = "AADGroup";
		}
	}
}
