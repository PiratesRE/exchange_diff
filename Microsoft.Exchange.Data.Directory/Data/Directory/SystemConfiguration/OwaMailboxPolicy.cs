using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class OwaMailboxPolicy : MailboxPolicy
	{
		internal static QueryFilter IsDefaultFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(MailboxPolicySchema.MailboxPolicyFlags, 1UL));
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return OwaMailboxPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return OwaMailboxPolicy.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return OwaMailboxPolicy.parentPath;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DirectFileAccessOnPublicComputersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.DirectFileAccessOnPublicComputersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.DirectFileAccessOnPublicComputersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DirectFileAccessOnPrivateComputersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.DirectFileAccessOnPrivateComputersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.DirectFileAccessOnPrivateComputersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WebReadyDocumentViewingOnPublicComputersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.WebReadyDocumentViewingOnPublicComputersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.WebReadyDocumentViewingOnPublicComputersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WebReadyDocumentViewingOnPrivateComputersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.WebReadyDocumentViewingOnPrivateComputersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.WebReadyDocumentViewingOnPrivateComputersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ForceWebReadyDocumentViewingFirstOnPublicComputers
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ForceWebReadyDocumentViewingFirstOnPublicComputers];
			}
			set
			{
				this[OwaMailboxPolicySchema.ForceWebReadyDocumentViewingFirstOnPublicComputers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ForceWebReadyDocumentViewingFirstOnPrivateComputers
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ForceWebReadyDocumentViewingFirstOnPrivateComputers];
			}
			set
			{
				this[OwaMailboxPolicySchema.ForceWebReadyDocumentViewingFirstOnPrivateComputers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WacViewingOnPublicComputersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.WacViewingOnPublicComputersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.WacViewingOnPublicComputersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WacViewingOnPrivateComputersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.WacViewingOnPrivateComputersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.WacViewingOnPrivateComputersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ForceWacViewingFirstOnPublicComputers
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ForceWacViewingFirstOnPublicComputers];
			}
			set
			{
				this[OwaMailboxPolicySchema.ForceWacViewingFirstOnPublicComputers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ForceWacViewingFirstOnPrivateComputers
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ForceWacViewingFirstOnPrivateComputers];
			}
			set
			{
				this[OwaMailboxPolicySchema.ForceWacViewingFirstOnPrivateComputers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AttachmentBlockingActions ActionForUnknownFileAndMIMETypes
		{
			get
			{
				return (AttachmentBlockingActions)this[OwaMailboxPolicySchema.ActionForUnknownFileAndMIMETypes];
			}
			set
			{
				this[OwaMailboxPolicySchema.ActionForUnknownFileAndMIMETypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> WebReadyFileTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[OwaMailboxPolicySchema.WebReadyFileTypes];
			}
			set
			{
				this[OwaMailboxPolicySchema.WebReadyFileTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> WebReadyMimeTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[OwaMailboxPolicySchema.WebReadyMimeTypes];
			}
			set
			{
				this[OwaMailboxPolicySchema.WebReadyMimeTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WebReadyDocumentViewingForAllSupportedTypes
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.WebReadyDocumentViewingForAllSupportedTypes];
			}
			set
			{
				this[OwaMailboxPolicySchema.WebReadyDocumentViewingForAllSupportedTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> WebReadyDocumentViewingSupportedMimeTypes
		{
			get
			{
				return OwaMailboxPolicy.webReadyDocumentViewingSupportedMimeTypes;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> WebReadyDocumentViewingSupportedFileTypes
		{
			get
			{
				return OwaMailboxPolicy.webReadyDocumentViewingSupportedFileTypes;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AllowedFileTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[OwaMailboxPolicySchema.AllowedFileTypes];
			}
			set
			{
				this[OwaMailboxPolicySchema.AllowedFileTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AllowedMimeTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[OwaMailboxPolicySchema.AllowedMimeTypes];
			}
			set
			{
				this[OwaMailboxPolicySchema.AllowedMimeTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ForceSaveFileTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[OwaMailboxPolicySchema.ForceSaveFileTypes];
			}
			set
			{
				this[OwaMailboxPolicySchema.ForceSaveFileTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ForceSaveMimeTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[OwaMailboxPolicySchema.ForceSaveMimeTypes];
			}
			set
			{
				this[OwaMailboxPolicySchema.ForceSaveMimeTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> BlockedFileTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[OwaMailboxPolicySchema.BlockedFileTypes];
			}
			set
			{
				this[OwaMailboxPolicySchema.BlockedFileTypes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> BlockedMimeTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[OwaMailboxPolicySchema.BlockedMimeTypes];
			}
			set
			{
				this[OwaMailboxPolicySchema.BlockedMimeTypes] = value;
			}
		}

		internal static object WebReadyFileTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[OwaMailboxPolicySchema.ADWebReadyFileTypes], OwaMailboxPolicySchema.ADWebReadyFileTypes);
		}

		internal static void WebReadyFileTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OwaMailboxPolicySchema.ADWebReadyFileTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, OwaMailboxPolicySchema.ADWebReadyFileTypes, propertyBag);
		}

		internal static object WebReadyMimeTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[OwaMailboxPolicySchema.ADWebReadyMimeTypes], OwaMailboxPolicySchema.ADWebReadyMimeTypes);
		}

		internal static void WebReadyMimeTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OwaMailboxPolicySchema.ADWebReadyMimeTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, OwaMailboxPolicySchema.ADWebReadyMimeTypes, propertyBag);
		}

		internal static object AllowedFileTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[OwaMailboxPolicySchema.ADAllowedFileTypes], OwaMailboxPolicySchema.ADAllowedFileTypes);
		}

		internal static void AllowedFileTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OwaMailboxPolicySchema.ADAllowedFileTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, OwaMailboxPolicySchema.ADAllowedFileTypes, propertyBag);
		}

		internal static object AllowedMimeTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[OwaMailboxPolicySchema.ADAllowedMimeTypes], OwaMailboxPolicySchema.ADAllowedMimeTypes);
		}

		internal static void AllowedMimeTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OwaMailboxPolicySchema.ADAllowedMimeTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, OwaMailboxPolicySchema.ADAllowedMimeTypes, propertyBag);
		}

		internal static object ForceSaveFileTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[OwaMailboxPolicySchema.ADForceSaveFileTypes], OwaMailboxPolicySchema.ADForceSaveFileTypes);
		}

		internal static void ForceSaveFileTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OwaMailboxPolicySchema.ADForceSaveFileTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, OwaMailboxPolicySchema.ADForceSaveFileTypes, propertyBag);
		}

		internal static object ForceSaveMimeTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[OwaMailboxPolicySchema.ADForceSaveMimeTypes], OwaMailboxPolicySchema.ADForceSaveMimeTypes);
		}

		internal static void ForceSaveMimeTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OwaMailboxPolicySchema.ADForceSaveMimeTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, OwaMailboxPolicySchema.ADForceSaveMimeTypes, propertyBag);
		}

		internal static object BlockedFileTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[OwaMailboxPolicySchema.ADBlockedFileTypes], OwaMailboxPolicySchema.ADBlockedFileTypes);
		}

		internal static void BlockedFileTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OwaMailboxPolicySchema.ADBlockedFileTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, OwaMailboxPolicySchema.ADBlockedFileTypes, propertyBag);
		}

		internal static object BlockedMimeTypesGetter(IPropertyBag propertyBag)
		{
			return ExchangeVirtualDirectory.RemoveDNStringSyntax((MultiValuedProperty<string>)propertyBag[OwaMailboxPolicySchema.ADBlockedMimeTypes], OwaMailboxPolicySchema.ADBlockedMimeTypes);
		}

		internal static void BlockedMimeTypesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[OwaMailboxPolicySchema.ADBlockedMimeTypes] = ExchangeVirtualDirectory.AddDNStringSyntax((MultiValuedProperty<string>)value, OwaMailboxPolicySchema.ADBlockedMimeTypes, propertyBag);
		}

		internal override bool CheckForAssociatedUsers()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.OrganizationId, null, false);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 2539, "CheckForAssociatedUsers", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\OwaMailboxPolicy.cs");
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.OwaMailboxPolicy, base.Id);
			ADUser[] array = tenantOrRootOrgRecipientSession.FindADUser(null, QueryScope.SubTree, filter, null, 1);
			return array != null && array.Length > 0;
		}

		[Parameter(Mandatory = false)]
		public bool PhoneticSupportEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.PhoneticSupportEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.PhoneticSupportEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DefaultTheme
		{
			get
			{
				return (string)this[OwaMailboxPolicySchema.DefaultTheme];
			}
			set
			{
				this[OwaMailboxPolicySchema.DefaultTheme] = value;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.IsDefault];
			}
			set
			{
				this[OwaMailboxPolicySchema.IsDefault] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int DefaultClientLanguage
		{
			get
			{
				return (int)this[OwaMailboxPolicySchema.DefaultClientLanguage];
			}
			set
			{
				this[OwaMailboxPolicySchema.DefaultClientLanguage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int LogonAndErrorLanguage
		{
			get
			{
				return (int)this[OwaMailboxPolicySchema.LogonAndErrorLanguage];
			}
			set
			{
				this[OwaMailboxPolicySchema.LogonAndErrorLanguage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseGB18030
		{
			get
			{
				return (int)this[OwaMailboxPolicySchema.UseGB18030] == 1;
			}
			set
			{
				this[OwaMailboxPolicySchema.UseGB18030] = (value ? 1 : 0);
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseISO885915
		{
			get
			{
				return (int)this[OwaMailboxPolicySchema.UseISO885915] == 1;
			}
			set
			{
				this[OwaMailboxPolicySchema.UseISO885915] = (value ? 1 : 0);
			}
		}

		[Parameter(Mandatory = false)]
		public OutboundCharsetOptions OutboundCharset
		{
			get
			{
				return (OutboundCharsetOptions)this[OwaMailboxPolicySchema.OutboundCharset];
			}
			set
			{
				this[OwaMailboxPolicySchema.OutboundCharset] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool GlobalAddressListEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.GlobalAddressListEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.GlobalAddressListEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OrganizationEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.OrganizationEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.OrganizationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExplicitLogonEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ExplicitLogonEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.ExplicitLogonEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OWALightEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.OWALightEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.OWALightEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DelegateAccessEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.DelegateAccessEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.DelegateAccessEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IRMEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.IRMEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.IRMEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CalendarEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.CalendarEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.CalendarEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ContactsEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ContactsEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.ContactsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TasksEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.TasksEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.TasksEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool JournalEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.JournalEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.JournalEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool NotesEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.NotesEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.NotesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RemindersAndNotificationsEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.RemindersAndNotificationsEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.RemindersAndNotificationsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PremiumClientEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.PremiumClientEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.PremiumClientEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SpellCheckerEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.SpellCheckerEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.SpellCheckerEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SearchFoldersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.SearchFoldersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.SearchFoldersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SignaturesEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.SignaturesEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.SignaturesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ThemeSelectionEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ThemeSelectionEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.ThemeSelectionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool JunkEmailEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.JunkEmailEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.JunkEmailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UMIntegrationEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.UMIntegrationEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.UMIntegrationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WSSAccessOnPublicComputersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.WSSAccessOnPublicComputersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.WSSAccessOnPublicComputersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WSSAccessOnPrivateComputersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.WSSAccessOnPrivateComputersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.WSSAccessOnPrivateComputersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ChangePasswordEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ChangePasswordEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.ChangePasswordEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UNCAccessOnPublicComputersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.UNCAccessOnPublicComputersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.UNCAccessOnPublicComputersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UNCAccessOnPrivateComputersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.UNCAccessOnPrivateComputersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.UNCAccessOnPrivateComputersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ActiveSyncIntegrationEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ActiveSyncIntegrationEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.ActiveSyncIntegrationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllAddressListsEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.AllAddressListsEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.AllAddressListsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RulesEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.RulesEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.RulesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PublicFoldersEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.PublicFoldersEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.PublicFoldersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SMimeEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.SMimeEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.SMimeEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RecoverDeletedItemsEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.RecoverDeletedItemsEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.RecoverDeletedItemsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InstantMessagingEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.InstantMessagingEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.InstantMessagingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TextMessagingEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.TextMessagingEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.TextMessagingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ForceSaveAttachmentFilteringEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ForceSaveAttachmentFilteringEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.ForceSaveAttachmentFilteringEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SilverlightEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.SilverlightEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.SilverlightEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public InstantMessagingTypeOptions? InstantMessagingType
		{
			get
			{
				return new InstantMessagingTypeOptions?((InstantMessagingTypeOptions)this[OwaMailboxPolicySchema.InstantMessagingType]);
			}
			set
			{
				this[OwaMailboxPolicySchema.InstantMessagingType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DisplayPhotosEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.DisplayPhotosEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.DisplayPhotosEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SetPhotoEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.SetPhotoEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.SetPhotoEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AllowOfflineOnEnum AllowOfflineOn
		{
			get
			{
				return (AllowOfflineOnEnum)this[OwaMailboxPolicySchema.AllowOfflineOn];
			}
			set
			{
				this[OwaMailboxPolicySchema.AllowOfflineOn] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SetPhotoURL
		{
			get
			{
				return (string)this[OwaMailboxPolicySchema.SetPhotoURL];
			}
			set
			{
				this[OwaMailboxPolicySchema.SetPhotoURL] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PlacesEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.PlacesEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.PlacesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WeatherEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.WeatherEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.WeatherEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowCopyContactsToDeviceAddressBook
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.AllowCopyContactsToDeviceAddressBook];
			}
			set
			{
				this[OwaMailboxPolicySchema.AllowCopyContactsToDeviceAddressBook] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PredictedActionsEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.PredictedActionsEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.PredictedActionsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UserDiagnosticEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.UserDiagnosticEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.UserDiagnosticEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FacebookEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.FacebookEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.FacebookEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LinkedInEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.LinkedInEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.LinkedInEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WacExternalServicesEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.WacExternalServicesEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.WacExternalServicesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WacOMEXEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.WacOMEXEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.WacOMEXEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ReportJunkEmailEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.ReportJunkEmailEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.ReportJunkEmailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool GroupCreationEnabled
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.GroupCreationEnabled];
			}
			set
			{
				this[OwaMailboxPolicySchema.GroupCreationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SkipCreateUnifiedGroupCustomSharepointClassification
		{
			get
			{
				return (bool)this[OwaMailboxPolicySchema.SkipCreateUnifiedGroupCustomSharepointClassification];
			}
			set
			{
				this[OwaMailboxPolicySchema.SkipCreateUnifiedGroupCustomSharepointClassification] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public WebPartsFrameOptions WebPartsFrameOptionsType
		{
			get
			{
				return (WebPartsFrameOptions)this[ADOwaVirtualDirectorySchema.WebPartsFrameOptionsType];
			}
			set
			{
				this[ADOwaVirtualDirectorySchema.WebPartsFrameOptionsType] = (int)value;
			}
		}

		private static OwaMailboxPolicySchema schema = ObjectSchema.GetInstance<OwaMailboxPolicySchema>();

		private static string mostDerivedClass = "msExchOWAMailboxPolicy";

		private static ADObjectId parentPath = new ADObjectId("CN=OWA Mailbox Policies");

		private static MultiValuedProperty<string> webReadyDocumentViewingSupportedFileTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultWebReadyFileTypes);

		private static MultiValuedProperty<string> webReadyDocumentViewingSupportedMimeTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultWebReadyMimeTypes);
	}
}
