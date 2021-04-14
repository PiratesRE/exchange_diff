using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.AirSync;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	public class GetMobileDeviceBase<TIdentity, TDataObject> : GetMultitenancySystemConfigurationObjectTask<TIdentity, TDataObject> where TIdentity : IIdentityParameter where TDataObject : ADObject, new()
	{
		[Parameter]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "Mailbox", ValueFromPipeline = true)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields["OrganizationalUnit"];
			}
			set
			{
				base.Fields["OrganizationalUnit"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ActiveSync
		{
			get
			{
				return (SwitchParameter)(base.Fields["ActiveSync"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ActiveSync"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter OWAforDevices
		{
			get
			{
				return (SwitchParameter)(base.Fields["OWAforDevices"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["OWAforDevices"] = value;
			}
		}

		[Parameter]
		public string SortBy
		{
			get
			{
				return (string)base.Fields["SortBy"];
			}
			set
			{
				base.Fields["SortBy"] = (string.IsNullOrEmpty(value) ? null : value);
				this.internalSortBy = QueryHelper.GetSortBy(this.SortBy, GetMobileDeviceBase<TIdentity, TDataObject>.SortPropertiesArray);
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				MonadFilter monadFilter = new MonadFilter(value, this, GetMobileDeviceBase<TIdentity, TDataObject>.FilterableObjectSchema);
				this.inputFilter = monadFilter.InnerFilter;
				if (!this.IsFilteringByDeviceAccess())
				{
					base.OptionalIdentityData.AdditionalFilter = monadFilter.InnerFilter;
				}
				base.Fields["Filter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Monitoring
		{
			get
			{
				return (SwitchParameter)(base.Fields["Monitoring"] ?? false);
			}
			set
			{
				base.Fields["Monitoring"] = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (MobileDeviceTaskHelper.IsRunningUnderMyOptionsRole(this, base.TenantGlobalCatalogSession, base.SessionSettings))
				{
					ADObjectId result;
					if (!base.TryGetExecutingUserId(out result))
					{
						throw new ExecutingUserPropertyNotFoundException("executingUserid");
					}
					return result;
				}
				else
				{
					if (this.user != null)
					{
						return this.user.Id;
					}
					if (this.organizationalUnit != null)
					{
						return this.organizationalUnit.Id;
					}
					if (base.CurrentOrganizationId != null && base.CurrentOrganizationId != OrganizationId.ForestWideOrgId)
					{
						return base.CurrentOrganizationId.OrganizationalUnit;
					}
					return base.RootId;
				}
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return this.ConstructQueryFilterWithCustomFilter(this.IsFilteringByDeviceAccess() ? null : this.inputFilter);
			}
		}

		protected override SortBy InternalSortBy
		{
			get
			{
				return this.internalSortBy;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession;
			if (!MobileDeviceTaskHelper.IsRunningUnderMyOptionsRole(this, base.TenantGlobalCatalogSession, base.SessionSettings))
			{
				configurationSession = (IConfigurationSession)base.CreateSession();
			}
			else
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 296, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\AirSync\\GetMobileDevice.cs");
			}
			configurationSession.UseConfigNC = false;
			configurationSession.UseGlobalCatalog = (base.DomainController == null && base.ServerSettings.ViewEntireForest);
			return configurationSession;
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.FindUser();
			this.FindOrganizationalUnit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			MobileDevice mobileDevice = dataObject as MobileDevice;
			if (!this.ShouldShowMobileDevice(mobileDevice))
			{
				return;
			}
			ADObjectId orgContainerId = this.ConfigurationSession.GetOrgContainerId();
			for (ADObjectId adobjectId = mobileDevice.Id; adobjectId != null; adobjectId = adobjectId.Parent)
			{
				if (ADObjectId.Equals(adobjectId, orgContainerId))
				{
					return;
				}
			}
			bool flag = string.Equals("EASProbeDeviceType", mobileDevice.DeviceType, StringComparison.OrdinalIgnoreCase);
			if (this.Monitoring)
			{
				if (!flag)
				{
					return;
				}
			}
			else
			{
				if (flag)
				{
					return;
				}
				DeviceAccessState deviceAccessState = DeviceAccessState.Unknown;
				DeviceAccessStateReason deviceAccessStateReason = DeviceAccessStateReason.Unknown;
				ADObjectId deviceAccessControlRule = null;
				bool flag2 = false;
				if (mobileDevice != null && mobileDevice.OrganizationId != OrganizationId.ForestWideOrgId && (mobileDevice.DeviceAccessStateReason != DeviceAccessStateReason.Individual || mobileDevice.DeviceAccessState != DeviceAccessState.Blocked) && mobileDevice.DeviceAccessStateReason != DeviceAccessStateReason.Policy && mobileDevice.DeviceAccessStateReason < DeviceAccessStateReason.UserAgentsChanges)
				{
					Command.DetermineDeviceAccessState(this.LoadAbq(OrganizationId.ForestWideOrgId), mobileDevice.DeviceType, mobileDevice.DeviceModel, mobileDevice.DeviceUserAgent, mobileDevice.DeviceOS, out deviceAccessState, out deviceAccessStateReason, out deviceAccessControlRule);
					if (deviceAccessState == DeviceAccessState.Blocked)
					{
						mobileDevice.DeviceAccessState = deviceAccessState;
						mobileDevice.DeviceAccessStateReason = deviceAccessStateReason;
						mobileDevice.DeviceAccessControlRule = deviceAccessControlRule;
						flag2 = true;
						if (this.IsFilteringByDeviceAccess() && !this.IsInFilter(mobileDevice))
						{
							return;
						}
					}
				}
				if (!flag2 && mobileDevice != null && mobileDevice.DeviceAccessState != DeviceAccessState.DeviceDiscovery && mobileDevice.DeviceAccessStateReason != DeviceAccessStateReason.Individual && mobileDevice.DeviceAccessStateReason != DeviceAccessStateReason.Upgrade && mobileDevice.DeviceAccessStateReason != DeviceAccessStateReason.Policy && mobileDevice.DeviceAccessStateReason < DeviceAccessStateReason.UserAgentsChanges)
				{
					Command.DetermineDeviceAccessState(this.LoadAbq(mobileDevice.OrganizationId), mobileDevice.DeviceType, mobileDevice.DeviceModel, mobileDevice.DeviceUserAgent, mobileDevice.DeviceOS, out deviceAccessState, out deviceAccessStateReason, out deviceAccessControlRule);
					mobileDevice.DeviceAccessState = deviceAccessState;
					mobileDevice.DeviceAccessStateReason = deviceAccessStateReason;
					mobileDevice.DeviceAccessControlRule = deviceAccessControlRule;
				}
			}
			if (base.Fields["ActiveSync"] == null && base.Fields["OWAforDevices"] == null)
			{
				base.Fields["ActiveSync"] = new SwitchParameter(true);
				base.Fields["OWAforDevices"] = new SwitchParameter(true);
			}
			else if (base.Fields["ActiveSync"] == null)
			{
				if (this.OWAforDevices == false)
				{
					base.Fields["ActiveSync"] = new SwitchParameter(true);
				}
			}
			else if (base.Fields["OWAforDevices"] == null && this.ActiveSync == false)
			{
				base.Fields["OWAforDevices"] = new SwitchParameter(true);
			}
			if ((!this.ActiveSync || mobileDevice.ClientType != MobileClientType.EAS) && (!this.OWAforDevices || mobileDevice.ClientType != MobileClientType.MOWA))
			{
				return;
			}
			if (mobileDevice.ClientType == MobileClientType.MOWA)
			{
				MobileDevice mobileDevice2 = mobileDevice;
				string format = Strings.MOWADeviceTypePrefix;
				string arg;
				if ((arg = mobileDevice.DeviceType) == null)
				{
					arg = (mobileDevice.DeviceModel ?? mobileDevice.DeviceOS);
				}
				mobileDevice2.DeviceType = string.Format(format, arg);
			}
			if (this.IsFilteringByDeviceAccess() && !this.IsInFilter(mobileDevice))
			{
				return;
			}
			base.WriteResult(dataObject);
		}

		private bool IsInFilter(MobileDevice device)
		{
			if (this.inputFilter == null)
			{
				throw new InvalidOperationException("The inputFilter should not be null in this case");
			}
			if (device != null)
			{
				try
				{
					return OpathFilterEvaluator.FilterMatches(this.inputFilter, device);
				}
				catch (FilterOnlyAttributesException)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		private void FindUser()
		{
			if (this.Mailbox == null)
			{
				return;
			}
			this.Mailbox.SearchWithDisplayName = false;
			IEnumerable<ADRecipient> objects = this.Mailbox.GetObjects<ADRecipient>(null, base.TenantGlobalCatalogSession);
			using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					this.user = (enumerator.Current as ADUser);
					if (enumerator.MoveNext())
					{
						base.WriteError(new RecipientNotUniqueException(this.Mailbox.ToString()), ErrorCategory.InvalidArgument, null);
					}
					if (this.user == null || (this.user.RecipientType != RecipientType.UserMailbox && this.user.RecipientType != RecipientType.MailUser))
					{
						base.WriteError(new RecipientNotValidException(this.Mailbox.ToString()), ErrorCategory.InvalidArgument, null);
					}
					base.CurrentOrganizationId = this.user.OrganizationId;
				}
				else
				{
					base.WriteError(new RecipientNotFoundException(this.Mailbox.ToString()), ErrorCategory.InvalidArgument, null);
				}
			}
			if (this.user == null || (int)this.user.ExchangeVersion.ExchangeBuild.Major > Server.CurrentExchangeMajorVersion || (int)this.user.ExchangeVersion.ExchangeBuild.Major < Server.Exchange2009MajorVersion)
			{
				base.WriteError(new TaskNotSupportedOnVersionException(base.CommandRuntime.ToString(), Server.CurrentExchangeMajorVersion), ErrorCategory.InvalidArgument, null);
			}
		}

		private void FindOrganizationalUnit()
		{
			if (this.OrganizationalUnit == null)
			{
				return;
			}
			bool useConfigNC = this.ConfigurationSession.UseConfigNC;
			bool useGlobalCatalog = this.ConfigurationSession.UseGlobalCatalog;
			this.ConfigurationSession.UseConfigNC = false;
			if (string.IsNullOrEmpty(this.ConfigurationSession.DomainController))
			{
				this.ConfigurationSession.UseGlobalCatalog = true;
			}
			try
			{
				this.organizationalUnit = (ExchangeOrganizationalUnit)base.GetDataObject<ExchangeOrganizationalUnit>(this.OrganizationalUnit, this.ConfigurationSession, (base.CurrentOrganizationId != null) ? base.CurrentOrganizationId.OrganizationalUnit : null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(this.OrganizationalUnit.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(this.OrganizationalUnit.ToString())));
				RecipientTaskHelper.IsOrgnizationalUnitInOrganization(this.ConfigurationSession, base.CurrentOrganizationId, this.organizationalUnit, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
			}
			finally
			{
				this.ConfigurationSession.UseConfigNC = useConfigNC;
				this.ConfigurationSession.UseGlobalCatalog = useGlobalCatalog;
			}
		}

		private OrganizationSettingsData LoadAbq(OrganizationId organizationId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, organizationId, organizationId, false);
			IConfigurationSession configurationSession = this.CreateConfigurationSession(sessionSettings);
			ActiveSyncOrganizationSettings[] array = configurationSession.Find<ActiveSyncOrganizationSettings>(configurationSession.GetOrgContainerId(), QueryScope.SubTree, null, null, 2);
			if (array == null || array.Length == 0)
			{
				base.WriteError(new NoActiveSyncOrganizationSettingsException(organizationId.ToString()), ErrorCategory.InvalidArgument, null);
			}
			OrganizationSettingsData organizationSettingsData = new OrganizationSettingsData(array[0], configurationSession);
			this.organizationSettings[organizationId] = organizationSettingsData;
			return organizationSettingsData;
		}

		private QueryFilter ConstructQueryFilterWithCustomFilter(QueryFilter customFilter)
		{
			List<QueryFilter> list = new List<QueryFilter>(3);
			QueryFilter internalFilter = base.InternalFilter;
			if (internalFilter != null)
			{
				list.Add(internalFilter);
			}
			if (customFilter != null)
			{
				list.Add(customFilter);
			}
			switch (list.Count)
			{
			case 0:
				return null;
			case 1:
				return list[0];
			default:
				return new AndFilter(list.ToArray());
			}
		}

		private bool IsFilteringByDeviceAccess()
		{
			return !string.IsNullOrEmpty(this.Filter) && (this.Filter.IndexOf(MobileDeviceSchema.DeviceAccessState.Name, StringComparison.InvariantCultureIgnoreCase) >= 0 || this.Filter.IndexOf(MobileDeviceSchema.DeviceAccessStateReason.Name, StringComparison.InvariantCultureIgnoreCase) >= 0 || this.Filter.IndexOf(MobileDeviceSchema.DeviceAccessControlRule.Name, StringComparison.InvariantCultureIgnoreCase) >= 0);
		}

		private bool ShouldShowMobileDevice(MobileDevice mobileDevice)
		{
			return mobileDevice != null && !mobileDevice.MaximumSupportedExchangeObjectVersion.IsOlderThan(mobileDevice.ExchangeVersion) && 0 > mobileDevice.Id.DistinguishedName.IndexOf("Soft Deleted Objects", StringComparison.OrdinalIgnoreCase) && 0 > mobileDevice.Id.Rdn.EscapedName.IndexOf("-", StringComparison.OrdinalIgnoreCase) && 0 > mobileDevice.Id.Parent.Rdn.EscapedName.IndexOf("-", StringComparison.OrdinalIgnoreCase);
		}

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			MobileDeviceSchema.FriendlyName,
			MobileDeviceSchema.DeviceId,
			MobileDeviceSchema.DeviceImei,
			MobileDeviceSchema.DeviceMobileOperator,
			MobileDeviceSchema.DeviceOS,
			MobileDeviceSchema.DeviceOSLanguage,
			MobileDeviceSchema.DeviceTelephoneNumber,
			MobileDeviceSchema.DeviceType,
			MobileDeviceSchema.DeviceUserAgent,
			MobileDeviceSchema.DeviceModel,
			MobileDeviceSchema.FirstSyncTime,
			MobileDeviceSchema.UserDisplayName,
			MobileDeviceSchema.DeviceAccessState,
			MobileDeviceSchema.DeviceAccessStateReason,
			MobileDeviceSchema.DeviceAccessControlRule,
			MobileDeviceSchema.ClientVersion
		};

		private static readonly MobileDeviceSchema FilterableObjectSchema = ObjectSchema.GetInstance<MobileDeviceSchema>();

		private ADUser user;

		private ExchangeOrganizationalUnit organizationalUnit;

		private QueryFilter inputFilter;

		private SortBy internalSortBy;

		private Dictionary<OrganizationId, OrganizationSettingsData> organizationSettings = new Dictionary<OrganizationId, OrganizationSettingsData>(2);
	}
}
