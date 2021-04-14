using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.AirSync;

namespace Microsoft.Exchange.Management.Tasks
{
	public class GetMobileDeviceStatisticsBase<TIdentity, TDataObject> : SystemConfigurationObjectActionTask<TIdentity, TDataObject> where TIdentity : MobileDeviceIdParameter, new() where TDataObject : MobileDevice, new()
	{
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
		public SwitchParameter GetMailboxLog
		{
			get
			{
				return (SwitchParameter)(base.Fields["GetMailboxLog"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["GetMailboxLog"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> NotificationEmailAddresses
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["NotificationEmailAddresses"];
			}
			set
			{
				base.Fields["NotificationEmailAddresses"] = value;
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
		public SwitchParameter ShowRecoveryPassword
		{
			get
			{
				return (SwitchParameter)(base.Fields["ShowRecoveryPassword"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ShowRecoveryPassword"] = value;
			}
		}

		public MobileDeviceConfiguration DeviceConfiguration { get; set; }

		private string UserName
		{
			get
			{
				if (this.userName == null && this.mailboxSession != null)
				{
					this.userName = this.mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
					if (this.userName != null)
					{
						int num = this.userName.IndexOf('@');
						if (num >= 0)
						{
							this.userName = this.userName.Substring(0, num);
						}
					}
					else
					{
						this.userName = this.mailboxSession.MailboxOwner.MailboxInfo.DisplayName;
					}
				}
				return this.userName;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, (this.principal != null) ? this.principal.MailboxInfo.OrganizationId : base.CurrentOrganizationId, (this.principal != null) ? this.principal.MailboxInfo.OrganizationId : base.ExecutingUserOrganizationId, false);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 193, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\AirSync\\GetMobileDeviceStatistics.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			tenantOrTopologyConfigurationSession.UseGlobalCatalog = (base.DomainController == null && base.ServerSettings.ViewEntireForest);
			return tenantOrTopologyConfigurationSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (base.ParameterSetName == "Identity")
				{
					base.InternalValidate();
					if (base.HasErrors)
					{
						return;
					}
					if (MobileDeviceTaskHelper.IsRunningUnderMyOptionsRole(this, base.TenantGlobalCatalogSession, base.SessionSettings))
					{
						ADObjectId id;
						if (!base.TryGetExecutingUserId(out id))
						{
							throw new ExecutingUserPropertyNotFoundException("executingUserid");
						}
						TDataObject dataObject = this.DataObject;
						if (!dataObject.Id.Parent.Parent.Equals(id))
						{
							TIdentity identity = this.Identity;
							base.WriteError(new LocalizedException(Strings.ErrorObjectNotFound(identity.ToString())), ErrorCategory.InvalidArgument, null);
						}
					}
				}
				IRecipientSession recipientSession = this.CreateTenantGlobalCatalogSession(base.SessionSettings);
				this.GetExchangePrincipal(recipientSession);
				if (MobileDeviceTaskHelper.IsRunningUnderMyOptionsRole(this, base.TenantGlobalCatalogSession, base.SessionSettings))
				{
					ADObjectId id2;
					if (!base.TryGetExecutingUserId(out id2))
					{
						throw new ExecutingUserPropertyNotFoundException("executingUserid");
					}
					if (!this.principal.ObjectId.Equals(id2))
					{
						base.WriteError(new RecipientNotFoundException(this.Mailbox.ToString()), ErrorCategory.InvalidArgument, null);
					}
				}
				if (this.GetMailboxLog)
				{
					IList<LocalizedString> list = null;
					ADObjectId executingUserId = null;
					base.TryGetExecutingUserId(out executingUserId);
					this.validatedAddresses = MobileDeviceTaskHelper.ValidateAddresses(recipientSession, executingUserId, this.NotificationEmailAddresses, out list);
					if (list != null)
					{
						foreach (LocalizedString text in list)
						{
							this.WriteWarning(text);
						}
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = this.principal.RecipientTypeDetails == RecipientTypeDetails.MonitoringMailbox;
			try
			{
				this.mailboxSession = MailboxSession.OpenAsAdmin(this.principal, CultureInfo.InvariantCulture, "Client=Management;Action=Get-MobileDeviceStatistics");
				List<DeviceInfo> list = new List<DeviceInfo>();
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
				if (this.ActiveSync == true)
				{
					DeviceInfo[] allDeviceInfo = DeviceInfo.GetAllDeviceInfo(this.mailboxSession, MobileClientType.EAS);
					if (allDeviceInfo != null)
					{
						list.AddRange(allDeviceInfo);
					}
				}
				if (this.OWAforDevices == true)
				{
					DeviceInfo[] allDeviceInfo2 = DeviceInfo.GetAllDeviceInfo(this.mailboxSession, MobileClientType.MOWA);
					if (allDeviceInfo2 != null)
					{
						list.AddRange(allDeviceInfo2);
					}
				}
				if (list != null)
				{
					List<MobileDevice> allMobileDevices = this.GetAllMobileDevices();
					int num = 0;
					using (List<DeviceInfo>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							DeviceInfo deviceInfo = enumerator.Current;
							if (deviceInfo.DeviceADObjectId != null && ADObjectId.Equals(deviceInfo.UserADObjectId, this.principal.ObjectId))
							{
								MobileDevice mobileDevice = allMobileDevices.Find((MobileDevice currentDevice) => currentDevice.DeviceId.Equals(deviceInfo.DeviceIdentity.DeviceId));
								if (mobileDevice != null)
								{
									num++;
									this.deviceConfiguration = this.CreateDeviceConfiguration(deviceInfo);
									if (this.Identity != null)
									{
										TIdentity identity = this.Identity;
										if (!identity.InternalADObjectId.Equals(this.deviceConfiguration.Identity))
										{
											continue;
										}
									}
									if (mobileDevice != null && this.deviceConfiguration.DeviceAccessStateReason < DeviceAccessStateReason.UserAgentsChanges && mobileDevice.ClientType == MobileClientType.EAS)
									{
										if (!flag)
										{
											DeviceAccessState deviceAccessState = DeviceAccessState.Unknown;
											DeviceAccessStateReason deviceAccessStateReason = DeviceAccessStateReason.Unknown;
											ADObjectId deviceAccessControlRule = null;
											bool flag2 = false;
											if (mobileDevice.OrganizationId != OrganizationId.ForestWideOrgId && (mobileDevice.DeviceAccessStateReason != DeviceAccessStateReason.Individual || mobileDevice.DeviceAccessState != DeviceAccessState.Blocked) && mobileDevice.DeviceAccessStateReason != DeviceAccessStateReason.Policy)
											{
												Command.DetermineDeviceAccessState(this.LoadAbq(OrganizationId.ForestWideOrgId), mobileDevice.DeviceType, mobileDevice.DeviceModel, mobileDevice.DeviceUserAgent, mobileDevice.DeviceOS, out deviceAccessState, out deviceAccessStateReason, out deviceAccessControlRule);
												if (deviceAccessState == DeviceAccessState.Blocked)
												{
													mobileDevice.DeviceAccessState = deviceAccessState;
													mobileDevice.DeviceAccessStateReason = deviceAccessStateReason;
													mobileDevice.DeviceAccessControlRule = deviceAccessControlRule;
													flag2 = true;
												}
											}
											if (!flag2 && mobileDevice.DeviceAccessState != DeviceAccessState.DeviceDiscovery && mobileDevice.DeviceAccessStateReason != DeviceAccessStateReason.Individual && mobileDevice.DeviceAccessStateReason != DeviceAccessStateReason.Upgrade && mobileDevice.DeviceAccessStateReason != DeviceAccessStateReason.Policy)
											{
												Command.DetermineDeviceAccessState(this.LoadAbq(mobileDevice.OrganizationId), mobileDevice.DeviceType, mobileDevice.DeviceModel, mobileDevice.DeviceUserAgent, mobileDevice.DeviceOS, out deviceAccessState, out deviceAccessStateReason, out deviceAccessControlRule);
												mobileDevice.DeviceAccessState = deviceAccessState;
												mobileDevice.DeviceAccessStateReason = deviceAccessStateReason;
												mobileDevice.DeviceAccessControlRule = deviceAccessControlRule;
											}
										}
										this.deviceConfiguration.DeviceAccessState = mobileDevice.DeviceAccessState;
										this.deviceConfiguration.DeviceAccessStateReason = mobileDevice.DeviceAccessStateReason;
										this.deviceConfiguration.DeviceAccessControlRule = mobileDevice.DeviceAccessControlRule;
									}
									if (this.ShowRecoveryPassword == false)
									{
										this.deviceConfiguration.RecoveryPassword = "********";
									}
									if (this.GetMailboxLog)
									{
										this.deviceConfiguration.MailboxLogReport = deviceInfo.GetOrCreateMailboxLogReport(this.mailboxSession);
									}
									base.WriteObject(this.deviceConfiguration);
									if (this.Identity != null)
									{
										this.ProcessMailboxLogger(new DeviceInfo[]
										{
											deviceInfo
										});
									}
								}
							}
						}
					}
					if (this.Identity == null)
					{
						this.ProcessMailboxLogger(list.ToArray());
					}
					if (num > 0)
					{
						using (IBudget budget = StandardBudget.Acquire(this.mailboxSession.MailboxOwner.Sid, BudgetType.Eas, this.mailboxSession.GetADSessionSettings()))
						{
							if (budget != null)
							{
								IThrottlingPolicy throttlingPolicy = budget.ThrottlingPolicy;
								if (throttlingPolicy != null && !throttlingPolicy.EasMaxDevices.IsUnlimited && (long)num >= (long)((ulong)throttlingPolicy.EasMaxDevices.Value))
								{
									this.WriteWarning((num == 1) ? Strings.MaxDevicesReachedSingular(throttlingPolicy.EasMaxDevices.Value) : Strings.MaxDevicesReached(num, throttlingPolicy.EasMaxDevices.Value));
								}
							}
						}
					}
				}
			}
			catch (StorageTransientException ex)
			{
				TaskLogger.LogError(ex);
				base.WriteError(ex, ErrorCategory.ReadError, this.principal);
			}
			catch (StoragePermanentException ex2)
			{
				TaskLogger.LogError(ex2);
				base.WriteError(ex2, ErrorCategory.InvalidOperation, this.principal);
			}
			finally
			{
				if (this.mailboxSession != null)
				{
					this.mailboxSession.Dispose();
				}
				TaskLogger.LogExit();
			}
		}

		protected virtual MobileDeviceConfiguration CreateDeviceConfiguration(DeviceInfo deviceInfo)
		{
			return new MobileDeviceConfiguration(deviceInfo);
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

		private List<MobileDevice> GetAllMobileDevices()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)this.CreateSession();
			ADPagedReader<MobileDevice> adpagedReader = configurationSession.FindPaged<MobileDevice>(this.principal.ObjectId, QueryScope.SubTree, null, null, 0);
			List<MobileDevice> list = new List<MobileDevice>();
			foreach (MobileDevice mobileDevice in adpagedReader)
			{
				if (!mobileDevice.MaximumSupportedExchangeObjectVersion.IsOlderThan(mobileDevice.ExchangeVersion) && 0 > mobileDevice.Id.DistinguishedName.IndexOf("Soft Deleted Objects", StringComparison.OrdinalIgnoreCase) && 0 > mobileDevice.Id.Rdn.EscapedName.IndexOf("-", StringComparison.OrdinalIgnoreCase) && 0 > mobileDevice.Id.Parent.Rdn.EscapedName.IndexOf("-", StringComparison.OrdinalIgnoreCase))
				{
					list.Add(mobileDevice);
				}
			}
			return list;
		}

		private void GetExchangePrincipal(IRecipientSession recipientSession)
		{
			Exception ex = null;
			if (this.Mailbox != null)
			{
				this.principal = MobileDeviceTaskHelper.GetExchangePrincipal(base.SessionSettings, recipientSession, this.Mailbox, "Get-MobileDeviceStatistics", out ex);
			}
			else if (this.Identity != null)
			{
				TIdentity identity = this.Identity;
				MailboxIdParameter mailboxId = identity.GetMailboxId();
				if (mailboxId == null && this.DataObject != null)
				{
					this.Identity = (TIdentity)((object)this.CreateIdentityObject());
					TIdentity identity2 = this.Identity;
					mailboxId = identity2.GetMailboxId();
				}
				if (mailboxId == null)
				{
					TIdentity identity3 = this.Identity;
					base.WriteError(new LocalizedException(Strings.ErrorObjectNotFound(identity3.ToString())), ErrorCategory.InvalidArgument, null);
				}
				this.principal = MobileDeviceTaskHelper.GetExchangePrincipal(base.SessionSettings, recipientSession, mailboxId, base.CommandRuntime.ToString(), out ex);
			}
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidArgument, null);
			}
		}

		protected virtual MobileDeviceIdParameter CreateIdentityObject()
		{
			return new MobileDeviceIdParameter(this.DataObject);
		}

		private void ProcessMailboxLogger(params DeviceInfo[] deviceInfos)
		{
			if (!this.GetMailboxLog)
			{
				if (base.Fields["GetMailboxLog"] != null)
				{
					this.WriteWarning(Strings.MobileDeviceLogNotRetrieved);
				}
				return;
			}
			try
			{
				bool flag;
				if (!DeviceInfo.SendMailboxLog(this.mailboxSession, this.UserName, deviceInfos, this.validatedAddresses, out flag))
				{
					if (flag)
					{
						base.WriteError(new MobileDeviceLogException(Strings.MobileDeviceLogEMailFailure), ErrorCategory.NotSpecified, this.mailboxSession.MailboxOwner);
					}
					else
					{
						base.WriteError(new MobileDeviceLogException(Strings.MobileDeviceLogNoLogsExist), ErrorCategory.NotSpecified, this.mailboxSession.MailboxOwner);
					}
				}
			}
			catch (IOException ex)
			{
				base.WriteError(new MobileDeviceLogException(ex.Message), ErrorCategory.NotSpecified, this.mailboxSession.MailboxOwner);
			}
			catch (StoragePermanentException ex2)
			{
				base.WriteError(new MobileDeviceLogException(ex2.Message), ErrorCategory.NotSpecified, this.mailboxSession.MailboxOwner);
			}
			catch (StorageTransientException ex3)
			{
				base.WriteError(new MobileDeviceLogException(ex3.Message), ErrorCategory.NotSpecified, this.mailboxSession.MailboxOwner);
			}
		}

		private string userName;

		private ExchangePrincipal principal;

		private MailboxSession mailboxSession;

		private List<string> validatedAddresses;

		private Dictionary<OrganizationId, OrganizationSettingsData> organizationSettings = new Dictionary<OrganizationId, OrganizationSettingsData>(2);

		private MobileDeviceConfiguration deviceConfiguration;
	}
}
