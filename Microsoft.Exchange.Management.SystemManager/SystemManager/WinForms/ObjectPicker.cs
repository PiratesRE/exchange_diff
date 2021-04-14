using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class ObjectPicker : ObjectPickerBase
	{
		public static string ObjectName
		{
			get
			{
				return ADObjectSchema.Name.Name;
			}
		}

		internal static string ObjectClass
		{
			get
			{
				return ADObjectSchema.ObjectClass.Name;
			}
		}

		static ObjectPicker()
		{
			IconLibrary.IconReferenceCollection icons = ObjectPicker.ObjectClassIconLibrary.Icons;
			icons.Add("AddressListPicker", Icons.AddressList);
			icons.Add("AcceptedDomainPicker", Icons.AcceptedDomain);
			icons.Add("RemoteDomainPicker", Icons.RemoteDomain);
			icons.Add("UMAutoAttendantPicker", Icons.AutoAttendant);
			icons.Add("DAGNetworkPicker", Icons.DAGNetwork);
			icons.Add("UMDialPlanPicker", Icons.DialPlan);
			icons.Add("msExchDynamicDistributionList", Icons.DynamicDL);
			icons.Add("ElcMailboxPolicyPicker", Icons.ELCMailboxPolicy);
			icons.Add("ExchangeServerPicker", Icons.ExchangeServer);
			icons.Add("MailboxDatabasePicker", Icons.MailboxDatabase);
			icons.Add("MobileMailboxPolicyPicker", Icons.AirSyncMailboxPolicy);
			icons.Add("OabVirtualDirectoryPicker", Icons.OfflineAddressBookDistributionPoint);
			icons.Add("OfflineAddressBookPicker", Icons.OfflineAddressList);
			icons.Add("PublicFolderDatabasePicker", Icons.PublicFolderDatabase);
			icons.Add("UMMailboxPolicyPicker", Icons.UMMailboxPolicy);
			icons.Add("DatabaseAvailabilityGroupPicker", Icons.DatabaseAvailabilityGroup);
			icons.Add("OwaMailboxPolicyPicker", Icons.OWAMailboxPolicy);
			icons.Add("MailboxServerPicker", Icons.MailboxServer);
			icons.Add(typeof(ADOwaVirtualDirectory).Name, Icons.OWAVirtualDirectory);
			icons.Add(typeof(ADEcpVirtualDirectory).Name, Icons.EcpVirtualDirectory);
			icons.Add(typeof(ADWebServicesVirtualDirectory).Name, Icons.OWAVirtualDirectory);
			icons.Add(typeof(ADMobileVirtualDirectory).Name, Icons.AirSyncVirtualDirectory);
			icons.Add(typeof(ADAutodiscoverVirtualDirectory).Name, Icons.OWAVirtualDirectory);
			icons.Add(typeof(ADOabVirtualDirectory).Name, Icons.OfflineAddressBookDistributionPoint);
			icons.Add(CertificateIconType.ValidCertificate, Icons.CertificateValid);
			icons.Add(CertificateIconType.InvalidCertificate, Icons.CertificateInValid);
			icons.Add(CertificateIconType.CertificateRequest, Icons.CertificateRequest);
			icons.Add(CertificateIconType.AboutToExpiredCertificate, Icons.CertificateAboutToExpire);
			icons.Add(ElcFolderType.All, Icons.ELCFoldersAllMailboxFolders);
			icons.Add(ElcFolderType.Calendar, Icons.ELCFoldersCalendar);
			icons.Add(ElcFolderType.Contacts, Icons.ELCFoldersContacts);
			icons.Add(ElcFolderType.DeletedItems, Icons.ELCFoldersDeletedItems);
			icons.Add(ElcFolderType.Drafts, Icons.ELCFoldersDrafts);
			icons.Add(ElcFolderType.Inbox, Icons.ELCFoldersInbox);
			icons.Add(ElcFolderType.Journal, Icons.ELCFoldersJournal);
			icons.Add(ElcFolderType.JunkEmail, Icons.ELCFoldersJunkEmail);
			icons.Add(ElcFolderType.Notes, Icons.ELCFoldersNotes);
			icons.Add(ElcFolderType.ManagedCustomFolder, Icons.ELCFoldersCustomOrgFolder);
			icons.Add(ElcFolderType.Outbox, Icons.ELCFoldersOutbox);
			icons.Add(ElcFolderType.SentItems, Icons.ELCFoldersSentItems);
			icons.Add(ElcFolderType.Tasks, Icons.ELCFoldersTasks);
			icons.Add(ElcFolderType.RssSubscriptions, Icons.ELCFoldersRSSSubscriptions);
			icons.Add(ElcFolderType.SyncIssues, Icons.ELCFoldersSyncIssues);
			icons.Add(ElcFolderType.ConversationHistory, Icons.ELCFoldersConversationHistory);
			icons.Add(RecipientTypeDetails.UserMailbox, Icons.Mailbox);
			icons.Add(RecipientTypeDetails.LegacyMailbox, Icons.LegacyMailbox);
			icons.Add(RecipientTypeDetails.LinkedMailbox, Icons.LinkedMailbox);
			icons.Add(RecipientTypeDetails.SharedMailbox, Icons.SharedMailbox);
			icons.Add(RecipientTypeDetails.TeamMailbox, Icons.SharedMailbox);
			icons.Add(RecipientTypeDetails.EquipmentMailbox, Icons.EquipmentMailbox);
			icons.Add(RecipientTypeDetails.RoomMailbox, Icons.ConferenceRoomMailbox);
			icons.Add(RecipientTypeDetails.MailForestContact, Icons.MailForestContact);
			icons.Add(RecipientTypeDetails.MailUser, Icons.MailUser);
			icons.Add(RecipientTypeDetails.PublicFolder, Icons.MailEnabledPublicFolder);
			icons.Add(RecipientTypeDetails.MailContact, Icons.MailEnabledContact);
			icons.Add(RecipientTypeDetails.MailUniversalDistributionGroup, Icons.DistributionGroup);
			icons.Add(RecipientTypeDetails.MailUniversalSecurityGroup, Icons.MailEnabledUniversalSecurityGroup);
			icons.Add(RecipientTypeDetails.MailNonUniversalGroup, Icons.MailEnabledNonUniversalGroup);
			icons.Add(RecipientTypeDetails.DynamicDistributionGroup, Icons.DynamicDL);
			icons.Add(RecipientTypeDetails.DiscoveryMailbox, Icons.DiscoveryMailbox);
			icons.Add(RecipientTypeDetails.User, Icons.User);
			icons.Add(RecipientTypeDetails.DisabledUser, Icons.UserDisabled);
			icons.Add(RecipientTypeDetails.Contact, Icons.Contact);
			icons.Add(RecipientTypeDetails.UniversalDistributionGroup, Icons.UniversalDistributionGroup);
			icons.Add(RecipientTypeDetails.UniversalSecurityGroup, Icons.UniversalSecurityGroup);
			icons.Add(RecipientTypeDetails.RoleGroup, Icons.UniversalSecurityGroup);
			icons.Add(RecipientTypeDetails.MicrosoftExchange, Icons.MicrosoftExchange);
			icons.Add((RecipientTypeDetails)((ulong)int.MinValue), Icons.RemoteMailbox);
			icons.Add(RecipientTypeDetails.RemoteRoomMailbox, Icons.RemoteMailbox);
			icons.Add(RecipientTypeDetails.RemoteEquipmentMailbox, Icons.RemoteMailbox);
			icons.Add(RecipientTypeDetails.RemoteSharedMailbox, Icons.RemoteMailbox);
			icons.Add(RecipientTypeDetails.RemoteTeamMailbox, Icons.RemoteMailbox);
			icons.Add(GatewayStatus.Enabled, Icons.IPGateway);
			icons.Add(GatewayStatus.Disabled, Icons.IPGatewayDisabled);
			icons.Add(GatewayStatus.NoNewCalls, Icons.IPGatewayDisabled);
			icons.Add(RecipientTypeDetails.NonUniversalGroup, Icons.NonUniversalGroup);
			icons.Add(RecipientTypeDetails.None, Icons.RecipientTypeDetailsNone);
			icons.Add(SecurityPrincipalType.Group, Icons.UniversalSecurityGroup);
			icons.Add(SecurityPrincipalType.Computer, Icons.ExchangeServer);
			icons.Add(SecurityPrincipalType.WellknownSecurityPrincipal, Icons.SecurityPrincipal);
			icons.Add("organizationalUnit", Icons.OrganizationalUnit);
			icons.Add("container", Icons.OrganizationalUnit);
			icons.Add("builtinDomain", Icons.OrganizationalUnit);
			icons.Add("msExchSystemObjectsContainer", Icons.OrganizationalUnit);
			icons.Add("DeletedUser", Icons.DeletedUser);
			icons.Add("domainDNS", Icons.Domain);
			icons.Add("SharingPolicyPicker", Icons.FederatedSharingMailboxSetting);
			icons.Add("RetentionPolicyPicker", Icons.RetentionPolicy);
			icons.Add("RetentionPolicyTagPicker", Icons.RetentionPolicyTag);
			icons.Add("RoleAssignmentPolicyPicker", Icons.RoleAssignmentPolicy);
			icons.Add(ItemLoadStatus.Loading, Icons.Loading);
			icons.Add(ItemLoadStatus.Failed, Icons.Error);
			icons.Add("ArchivedMailbox", Icons.ArchiveMailbox);
			icons.Add("MailboxPlanPicker", Icons.MailboxPlan);
		}

		internal ResultsLoaderProfile ObjectPickerProfile
		{
			get
			{
				return this.objectPickerProfile;
			}
		}

		protected ObjectPicker() : this(null)
		{
		}

		protected ObjectPicker(ResultsLoaderProfile profile)
		{
			this.ResetUseTreeViewForm();
			this.ResetScopeSupportingLevel();
			if (profile != null)
			{
				this.UpdateResultsLoaderProfile(profile);
				this.objectPickerProfile = profile;
				this.scopeSupportingLevel = this.objectPickerProfile.ScopeSupportingLevel;
				this.useTreeViewForm = this.objectPickerProfile.UseTreeViewForm;
			}
			this.DataTableLoader = this.CreateDataTableLoader();
		}

		private void UpdateResultsLoaderProfile(ResultsLoaderProfile profile)
		{
			ResultsColumnProfile[] displayedColumnCollection = profile.DisplayedColumnCollection;
			for (int i = 0; i < displayedColumnCollection.Length; i++)
			{
				ResultsColumnProfile displayColumnProfile = displayedColumnCollection[i];
				DataColumn dataColumn = profile.DataTable.Columns.OfType<DataColumn>().First((DataColumn column) => column.ColumnName == displayColumnProfile.Name);
				if (dataColumn.DataType != typeof(string))
				{
					string text = dataColumn.ColumnName + "_FilterSupport_";
					DataColumn dataColumn2 = new DataColumn(text, typeof(string));
					dataColumn2.ExtendedProperties["LambdaExpression"] = string.Format("{0}=>WinformsHelper.ConvertValueToString(@0.Table.Columns[\"{0}\"], @0[\"{0}\"])", dataColumn.ColumnName);
					profile.DataTable.Columns.Add(dataColumn2);
					displayColumnProfile.Name = text;
					displayColumnProfile.SortMode = SortMode.DelegateColumn;
					dataColumn2.ExtendedProperties["DelegateColumnName"] = dataColumn.ColumnName;
				}
			}
		}

		protected ObjectPicker(string profileName) : this(ObjectPicker.ProfileLoader.GetProfile(profileName))
		{
		}

		protected ObjectPicker(IResultsLoaderConfiguration config) : this(config.BuildResultsLoaderProfile())
		{
		}

		protected abstract DataTableLoader CreateDataTableLoader();

		public abstract string ObjectClassDisplayName { get; }

		public abstract void PerformQuery(object rootId, string searchText);

		protected virtual bool DefaultUseTreeViewForm
		{
			get
			{
				return false;
			}
		}

		protected virtual ScopeSupportingLevel DefaultScopeSupportingLevel
		{
			get
			{
				return ScopeSupportingLevel.NoScoping;
			}
		}

		protected virtual string DefaultCaption
		{
			get
			{
				return Strings.DefaultCaption(this.ObjectClassDisplayName);
			}
		}

		protected virtual string DefaultNoResultsLabelText
		{
			get
			{
				return Strings.NoItemsToSelect;
			}
		}

		public virtual string IdentityProperty
		{
			get
			{
				return ObjectPicker.ObjectName;
			}
		}

		public virtual string ImageProperty
		{
			get
			{
				return ObjectPicker.ObjectClass;
			}
		}

		public virtual string NameProperty
		{
			get
			{
				return ObjectPicker.ObjectName;
			}
		}

		public virtual string DefaultSortProperty
		{
			get
			{
				return this.NameProperty;
			}
		}

		public virtual ExchangeColumnHeader[] CreateColumnHeaders()
		{
			return new List<ExchangeColumnHeader>
			{
				new ExchangeColumnHeader(ObjectPicker.ObjectName, Strings.Name)
			}.ToArray();
		}

		public override DataTable CreateResultsDataTable()
		{
			DataTable dataTable = base.CreateResultsDataTable();
			dataTable.Columns.Add(ObjectPicker.ObjectClass, typeof(string));
			DataColumn dataColumn = dataTable.Columns.Add(ObjectPicker.ObjectName);
			dataColumn.Unique = true;
			dataTable.PrimaryKey = new DataColumn[]
			{
				dataColumn
			};
			return dataTable;
		}

		protected override DataTable GetSelectedObjects(IntPtr hwndOwner)
		{
			this.ResetScopeSetting();
			DataTable result;
			using (Form form = this.CreateObjectPickerForm())
			{
				if (base.Container != null)
				{
					base.Container.Add(form, form.Name + form.GetHashCode());
				}
				IUIService iuiservice = (IUIService)this.GetService(typeof(IUIService));
				if (iuiservice == null)
				{
					iuiservice = new UIService(new Win32Window(hwndOwner));
				}
				DataTable dataTable = null;
				if (DialogResult.OK == iuiservice.ShowDialog(form))
				{
					DataTable selectedObjects = ((ISelectedObjectsProvider)form).SelectedObjects;
					if (this.ObjectPickerProfile == null)
					{
						dataTable = ObjectPicker.RemoveNonRequiredColumns(selectedObjects);
					}
					else
					{
						dataTable = selectedObjects;
					}
				}
				if (base.Container != null)
				{
					base.Container.Remove(form);
				}
				result = dataTable;
			}
			return result;
		}

		internal virtual DataTableLoader CreateDataTableLoaderForResolver()
		{
			DataTableLoader dataTableLoader = this.CreateDataTableLoader();
			if (dataTableLoader.Table != null && this.ObjectPickerProfile == null)
			{
				this.MarkNonOptionalColumnsAsRequiredColumn(dataTableLoader.Table);
				ObjectPicker.RemoveNonRequiredColumns(dataTableLoader.Table);
			}
			if (dataTableLoader.RefreshArgument != null)
			{
				dataTableLoader.RefreshArgument = (ICloneable)dataTableLoader.ResultsLoaderProfile.CloneWithSharedInputTable();
			}
			return dataTableLoader;
		}

		[Browsable(false)]
		public DataTableLoader DataTableLoader
		{
			get
			{
				return this.dataTableLoader;
			}
			protected set
			{
				if (this.DataTableLoader != value)
				{
					this.dataTableLoader = value;
					if (this.DataTableLoader != null)
					{
						if (this.DataTableLoader.Table == null)
						{
							this.DataTableLoader.Table = this.CreateResultsDataTable();
						}
						if (this.ObjectPickerProfile == null)
						{
							this.MarkNonOptionalColumnsAsRequiredColumn(this.DataTableLoader.Table);
						}
					}
				}
			}
		}

		[DefaultValue(true)]
		public virtual bool ShowListItemIcon
		{
			get
			{
				return this.showListItemIcon;
			}
			set
			{
				this.showListItemIcon = value;
			}
		}

		[DefaultValue(true)]
		public bool SupportSearch
		{
			get
			{
				return this.supportSearch;
			}
			set
			{
				this.supportSearch = value;
			}
		}

		internal bool SupportModifyScope
		{
			get
			{
				return this.scopeSupportingLevel != ScopeSupportingLevel.NoScoping;
			}
		}

		internal bool ShouldScopingWithinDefaultDomainScope
		{
			get
			{
				return this.ScopeSupportingLevel == ScopeSupportingLevel.WithinDefaultScope && !ADServerSettingsSingleton.GetInstance().ADServerSettings.ForestViewEnabled;
			}
		}

		public ScopeSupportingLevel ScopeSupportingLevel
		{
			get
			{
				return this.scopeSupportingLevel;
			}
			set
			{
				this.scopeSupportingLevel = value;
				if (this.ObjectPickerProfile != null)
				{
					this.ObjectPickerProfile.ScopeSupportingLevel = value;
				}
			}
		}

		[DefaultValue(null)]
		public ScopeSettings DefaultScopeSettings
		{
			get
			{
				return this.defaultScopeSettings;
			}
			set
			{
				this.defaultScopeSettings = value;
			}
		}

		[DefaultValue(null)]
		public ScopeSettings ScopeSettings
		{
			get
			{
				if (this.scopeSettings == null)
				{
					if (this.DefaultScopeSettings != null)
					{
						this.scopeSettings = new ScopeSettings();
						this.scopeSettings.CopyFrom(this.DefaultScopeSettings);
					}
					else
					{
						this.scopeSettings = new ScopeSettings(ADServerSettingsSingleton.GetInstance().ADServerSettings);
					}
				}
				return this.scopeSettings;
			}
		}

		public bool UseTreeViewForm
		{
			get
			{
				return this.useTreeViewForm;
			}
			set
			{
				this.useTreeViewForm = value;
			}
		}

		[DefaultValue(true)]
		public bool CanSelectRootObject
		{
			get
			{
				return this.canSelectRootObject;
			}
			set
			{
				this.canSelectRootObject = value;
			}
		}

		public string Caption
		{
			get
			{
				if (string.IsNullOrEmpty(this.caption))
				{
					return this.DefaultCaption;
				}
				return this.caption;
			}
			set
			{
				this.caption = value;
			}
		}

		public string NoResultsLabelText
		{
			get
			{
				if (string.IsNullOrEmpty(this.noResultsLabelText))
				{
					return this.DefaultNoResultsLabelText;
				}
				return this.noResultsLabelText;
			}
			set
			{
				this.noResultsLabelText = value;
			}
		}

		public static IconLibrary ObjectClassIconLibrary
		{
			get
			{
				return ObjectPicker.objectClassIconLibrary;
			}
		}

		public string HelpTopic
		{
			get
			{
				if (this.helpTopic == null)
				{
					this.helpTopic = this.DefaultHelpTopic;
				}
				return this.helpTopic;
			}
			set
			{
				value = (value ?? "");
				this.helpTopic = value;
			}
		}

		private bool ShouldSerializeHelpTopic()
		{
			return this.HelpTopic != this.DefaultHelpTopic;
		}

		private void ResetHelpTopic()
		{
			this.HelpTopic = this.DefaultHelpTopic;
		}

		protected virtual string DefaultHelpTopic
		{
			get
			{
				if (this.ObjectPickerProfile == null)
				{
					return base.GetType().FullName;
				}
				return this.ObjectPickerProfile.HelpTopic;
			}
		}

		private bool ShouldSerializeUseTreeViewForm()
		{
			return this.UseTreeViewForm != this.DefaultUseTreeViewForm;
		}

		private void ResetUseTreeViewForm()
		{
			this.UseTreeViewForm = this.DefaultUseTreeViewForm;
		}

		private bool ShouldSerializeScopeSupportingLevel()
		{
			return this.ScopeSupportingLevel != this.DefaultScopeSupportingLevel;
		}

		private void ResetScopeSupportingLevel()
		{
			this.ScopeSupportingLevel = this.DefaultScopeSupportingLevel;
		}

		private bool ShouldSerializeCaption()
		{
			return this.Caption != this.DefaultCaption;
		}

		private void ResetCaption()
		{
			this.Caption = this.DefaultCaption;
		}

		private bool ShouldSerializeNoResultsLabelText()
		{
			return this.NoResultsLabelText != this.DefaultNoResultsLabelText;
		}

		private void ResetNoResultsLabelText()
		{
			this.NoResultsLabelText = this.DefaultNoResultsLabelText;
		}

		private void ResetScopeSetting()
		{
			this.scopeSettings = null;
		}

		private Form CreateObjectPickerForm()
		{
			Form result;
			if (this.UseTreeViewForm)
			{
				result = new TreeViewObjectPickerForm(this);
			}
			else
			{
				result = new ObjectPickerForm(this);
			}
			return result;
		}

		private void MarkNonOptionalColumnsAsRequiredColumn(DataTable dataTable)
		{
			bool[] array = new bool[dataTable.Columns.Count];
			foreach (ExchangeColumnHeader exchangeColumnHeader in this.CreateColumnHeaders())
			{
				if (!exchangeColumnHeader.Default && dataTable.Columns.Contains(exchangeColumnHeader.Name))
				{
					int ordinal = dataTable.Columns[exchangeColumnHeader.Name].Ordinal;
					array[ordinal] = true;
				}
			}
			foreach (object obj in dataTable.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (!array[dataColumn.Ordinal])
				{
					ObjectPicker.SetIsRequiredDataColumnFlag(dataColumn, true);
				}
			}
			foreach (DataColumn column in dataTable.PrimaryKey)
			{
				ObjectPicker.SetIsRequiredDataColumnFlag(column, true);
			}
		}

		private static DataTable RemoveNonRequiredColumns(DataTable table)
		{
			for (int i = table.Columns.Count - 1; i >= 0; i--)
			{
				DataColumn column = table.Columns[i];
				if (!ObjectPicker.GetIsRequiredDataColumnFlag(column))
				{
					table.Columns.Remove(column);
				}
			}
			return table;
		}

		internal static bool GetIsRequiredDataColumnFlag(DataColumn column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			return column.ExtendedProperties.ContainsKey(ObjectPicker.RequiredDataColumnFlag);
		}

		internal static void SetIsRequiredDataColumnFlag(DataColumn column, bool isRequiredDataColumn)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			if (ObjectPicker.GetIsRequiredDataColumnFlag(column) != isRequiredDataColumn)
			{
				if (isRequiredDataColumn)
				{
					column.ExtendedProperties.Add(ObjectPicker.RequiredDataColumnFlag, null);
					return;
				}
				column.ExtendedProperties.Remove(ObjectPicker.RequiredDataColumnFlag);
			}
		}

		internal const string FilterKeyName = "_FilterSupport_";

		private static readonly string RequiredDataColumnFlag = "required";

		private bool useTreeViewForm;

		private bool showListItemIcon = true;

		private bool supportSearch = true;

		private string caption;

		private string noResultsLabelText;

		private DataTableLoader dataTableLoader;

		private ScopeSupportingLevel scopeSupportingLevel;

		private ScopeSettings scopeSettings;

		private ResultsLoaderProfile objectPickerProfile;

		private static ObjectPickerProfileLoader ProfileLoader = new ObjectPickerProfileLoader();

		private ScopeSettings defaultScopeSettings;

		private bool canSelectRootObject = true;

		private static IconLibrary objectClassIconLibrary = new IconLibrary();

		private string helpTopic;
	}
}
