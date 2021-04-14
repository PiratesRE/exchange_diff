using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfCompanyItem : EhfSyncItem
	{
		private EhfCompanyItem(ExSearchResultEntry entry, EdgeSyncDiag diagSession) : base(entry, diagSession)
		{
			this.InitializeEhfCompanyId();
		}

		public int CompanyId
		{
			get
			{
				return this.companyId;
			}
		}

		public Company Company
		{
			get
			{
				return this.company;
			}
		}

		public CompanyConfigurationSettings Settings
		{
			get
			{
				return this.settings;
			}
		}

		public CompanyConfigurationSettings SettingsForCreate
		{
			get
			{
				return this.settingsForCreate;
			}
		}

		public bool PreviouslySynced
		{
			get
			{
				return this.companyId != 0;
			}
		}

		public bool EhfConfigSyncEnabled
		{
			get
			{
				int flagsValue = base.GetFlagsValue("msExchTenantPerimeterSettingsFlags");
				return (flagsValue & 16) == 0;
			}
		}

		public static EhfCompanyItem CreateForActive(ExSearchResultEntry entry, EdgeSyncDiag diagSession, int resellerId)
		{
			EhfCompanyItem ehfCompanyItem = new EhfCompanyItem(entry, diagSession);
			ehfCompanyItem.InitializeForCreateOrUpdate(resellerId);
			return ehfCompanyItem;
		}

		public static EhfCompanyItem CreateForTombstone(ExSearchResultEntry entry, EdgeSyncDiag diagSession)
		{
			return new EhfCompanyItem(entry, diagSession);
		}

		public static bool TryGetEhfCompanyId(ExSearchResultEntry entry, EdgeSyncDiag diagSession, out int ehfCompanyId, out string errorMessage)
		{
			ehfCompanyId = 0;
			errorMessage = null;
			DirectoryAttribute attribute = entry.GetAttribute("msExchTenantPerimeterSettingsOrgID");
			if (attribute == null)
			{
				return false;
			}
			string text = (string)attribute[0];
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			int num;
			if (int.TryParse(text, out num))
			{
				ehfCompanyId = num;
			}
			else
			{
				errorMessage = diagSession.LogAndTraceError("Failed to parse EHF Company ID ({0}) of Perimeter Settings object ({1})", new object[]
				{
					text,
					entry.DistinguishedName
				});
			}
			return ehfCompanyId != 0;
		}

		public Guid GetCompanyGuid()
		{
			if (this.company != null)
			{
				return this.company.CompanyGuid.Value;
			}
			return base.GetObjectGuid();
		}

		public string GetConfigUnitDN()
		{
			if (base.IsDeleted)
			{
				throw new InvalidOperationException("GetConfigUnitDN() cannot be invoked for deleted company");
			}
			ADObjectId adobjectId = new ADObjectId(base.DistinguishedName);
			this.ValidateDNDepth(adobjectId);
			return adobjectId.AncestorDN(2).DistinguishedName;
		}

		public EhfADResultCode TryStoreEhfCompanyId(int ehfCompanyId, EhfADAdapter adapter)
		{
			EhfADResultCode ehfADResultCode = this.TryUpdateAdminSyncEnabledFlag(adapter, true);
			if (ehfADResultCode != EhfADResultCode.Success)
			{
				return EhfADResultCode.Failure;
			}
			this.forceDomainSync = true;
			Guid companyGuid = this.GetCompanyGuid();
			try
			{
				KeyValuePair<string, object> item = new KeyValuePair<string, object>("msExchTransportInboundSettings", 1.ToString());
				KeyValuePair<string, object> item2 = new KeyValuePair<string, object>("msExchTenantPerimeterSettingsOrgID", ehfCompanyId.ToString());
				adapter.SetAttributeValues(companyGuid, new List<KeyValuePair<string, object>>
				{
					item2,
					item
				});
			}
			catch (ExDirectoryException ex)
			{
				if (ex.ResultCode == ResultCode.NoSuchObject)
				{
					base.DiagSession.LogAndTraceException(ex, "NoSuchObject error occurred while trying to save EHF company ID <{0}> in Perimeter Settings <{1}>:<{2}>", new object[]
					{
						ehfCompanyId,
						base.DistinguishedName,
						companyGuid
					});
					return EhfADResultCode.NoSuchObject;
				}
				base.DiagSession.LogAndTraceException(ex, "Exception occurred while trying to save EHF company ID <{0}> in Perimeter Settings <{1}>:<{2}>", new object[]
				{
					ehfCompanyId,
					base.DistinguishedName,
					companyGuid
				});
				base.AddSyncError("Exception occurred while trying to save EHF company ID {0} in Perimeter Settings ({1}):({2}); exception details: {3}", new object[]
				{
					ehfCompanyId,
					base.DistinguishedName,
					companyGuid,
					ex
				});
				return EhfADResultCode.Failure;
			}
			this.SetEhfId(ehfCompanyId);
			base.DiagSession.Tracer.TraceDebug<int, string, Guid>((long)base.DiagSession.GetHashCode(), "Successfully stored company ID <{0}> in Perimeter Settings <{1}>:<{2}>", ehfCompanyId, base.DistinguishedName, companyGuid);
			return EhfADResultCode.Success;
		}

		public EhfADResultCode TryClearEhfCompanyIdAndDisableAdminSync(EhfADAdapter adapter)
		{
			EhfADResultCode ehfADResultCode = this.TryUpdateAdminSyncEnabledFlag(adapter, true);
			if (ehfADResultCode != EhfADResultCode.Success)
			{
				return EhfADResultCode.Failure;
			}
			Guid companyGuid = this.GetCompanyGuid();
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Clearing the company ID stamped on company <{0}>:<{1}>", new object[]
			{
				base.DistinguishedName,
				companyGuid
			});
			try
			{
				adapter.SetAttributeValue(companyGuid, "msExchTenantPerimeterSettingsOrgID", null);
			}
			catch (ExDirectoryException ex)
			{
				if (ex.ResultCode == ResultCode.NoSuchObject)
				{
					base.DiagSession.LogAndTraceException(ex, "NoSuchObject error occurred while trying to clear EHF company ID in Perimeter Settings <{0}>:<{1}>", new object[]
					{
						base.DistinguishedName,
						companyGuid
					});
					return EhfADResultCode.NoSuchObject;
				}
				base.DiagSession.LogAndTraceException(ex, "Exception occurred while trying to clear EHF company ID in Perimeter Settings <{0}>:<{1}>", new object[]
				{
					base.DistinguishedName,
					companyGuid
				});
				base.AddSyncError("Exception occurred while trying to clear EHF company ID in Perimeter Settings ({0}):({1}); exception details: {2}", new object[]
				{
					base.DistinguishedName,
					companyGuid,
					ex
				});
				return EhfADResultCode.Failure;
			}
			base.DiagSession.Tracer.TraceDebug<int, string, Guid>((long)base.DiagSession.GetHashCode(), "Successfully cleared the company ID <{0}> in Perimeter Settings <{1}>:<{2}>", this.companyId, base.DistinguishedName, companyGuid);
			return EhfADResultCode.Success;
		}

		public void RecoverLostCompanyId(Company company)
		{
			if (!base.ADEntry.IsDeleted && this.EhfConfigSyncEnabled)
			{
				throw new InvalidOperationException("RecoverLostCompanyId is invoked for an entry that is not deleted and not disabled.");
			}
			this.SetEhfId(company.CompanyId);
			this.company = company;
		}

		public bool IsForcedDomainSyncRequired()
		{
			if (this.forceDomainSync)
			{
				return true;
			}
			int flagsValue = base.GetFlagsValue("msExchTransportInboundSettings");
			return flagsValue == 1;
		}

		private static IPListConfig CreateEmptyIPList()
		{
			return new IPListConfig
			{
				IPList = new string[0],
				TargetAction = TargetAction.Set
			};
		}

		private static int CalculateProvisioningFlags(OrganizationProvisioningFlags organizationProvisioningFlags, PerimeterFlags perimeterFlags, bool forceDisable)
		{
			if (forceDisable || (perimeterFlags & PerimeterFlags.EhfConfigSyncDisabled) == PerimeterFlags.EhfConfigSyncDisabled || (perimeterFlags & PerimeterFlags.EhfAdminAccountSyncDisabled) == PerimeterFlags.EhfAdminAccountSyncDisabled)
			{
				return (int)(~OrganizationProvisioningFlags.EhfAdminAccountSyncEnabled & organizationProvisioningFlags);
			}
			return (int)(organizationProvisioningFlags | OrganizationProvisioningFlags.EhfAdminAccountSyncEnabled);
		}

		private string GetOuRootDN()
		{
			DirectoryAttribute attribute = base.ADEntry.GetAttribute("msExchOURoot");
			if (attribute == null)
			{
				return null;
			}
			return (string)attribute[0];
		}

		private void InitializeEhfCompanyId()
		{
			string text;
			if (!EhfCompanyItem.TryGetEhfCompanyId(base.ADEntry, base.DiagSession, out this.companyId, out text) && !string.IsNullOrEmpty(text))
			{
				base.AddSyncError("{0}; will try to treat the company as if it has not been previously synced", new object[]
				{
					text
				});
			}
		}

		private void InitializeForCreateOrUpdate(int resellerId)
		{
			this.InitializeSettings();
			if (this.PreviouslySynced)
			{
				this.settings.CompanyId = this.companyId;
				return;
			}
			this.InitializeSettingsForCreate();
			this.InitializeCompany(resellerId);
		}

		private void InitializeSettings()
		{
			this.settings = new CompanyConfigurationSettings();
			this.settings.InheritFromParent = InheritanceSettings.InheritOutboundIPConfig;
			this.settings.PropagationSetting = PropagationSettings.PropagateOutboundIPConfig;
			this.settings.DirectoryEdgeBlockMode = new EdgeBlockMode?(EdgeBlockMode.Disabled);
			this.settings.RecipientLevelRouting = new bool?(false);
			if (this.IpSafelistingSyncEnabled())
			{
				this.settings.SkipList = new bool?(this.GetIPSkiplistingEnabled());
				this.settings.InternalServerIPList = this.GetInternalServerIPAddresses();
				this.settings.OnPremiseGatewayIPList = this.GetGatewayIPAddresses();
				return;
			}
			base.DiagSession.Tracer.TraceDebug<string>((long)this.GetHashCode(), "IPSafelistingSyncEnabled is set to false for {0}. Not syncing the Safelisting information.", base.DistinguishedName);
		}

		public void InitializeSettingsForCreate()
		{
			this.settingsForCreate = new CompanyConfigurationSettings();
			this.settingsForCreate.ServicePlanId = this.GetServicePlanId();
		}

		private bool IpSafelistingSyncEnabled()
		{
			int flagsValue = base.GetFlagsValue("msExchTenantPerimeterSettingsFlags");
			return (flagsValue & 64) != 0;
		}

		private void InitializeCompany(int resellerId)
		{
			this.company = new Company();
			this.company.ActivationComplete = true;
			this.company.InheritFromParent = (InheritanceSettings.InheritOutboundIPConfig | InheritanceSettings.InheritSubscriptions);
			this.company.IsEnabled = true;
			this.company.ParentCompanyId = resellerId;
			this.company.CompanyGuid = new Guid?(base.GetObjectGuid());
			this.company.Name = this.GetEhfCompanyName(resellerId);
		}

		private string GetEhfCompanyName(int resellerId)
		{
			if (base.ADEntry.IsDeleted)
			{
				throw new InvalidOperationException("GetEhfCompanyName() is invoked for a deleted AD object");
			}
			ADObjectId adobjectId = new ADObjectId(base.DistinguishedName);
			this.ValidateDNDepth(adobjectId);
			string text = adobjectId.AncestorDN(3).Name;
			if (string.IsNullOrEmpty(text))
			{
				base.DiagSession.LogAndTraceError("Tenant name in Perimeter Settings DN <{0}> is empty", new object[]
				{
					base.DistinguishedName
				});
				throw new InvalidOperationException("Tenant name in Perimeter Settings DN is empty: " + base.DistinguishedName);
			}
			StringBuilder stringBuilder = null;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] < '\0' || text[i] > '\u007f')
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(text, 0, i, text.Length + 36);
					}
				}
				else if (stringBuilder != null)
				{
					stringBuilder.Append(text[i]);
				}
			}
			if (stringBuilder != null)
			{
				stringBuilder.Append(base.GetObjectGuid());
				text = stringBuilder.ToString();
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}_{2}", new object[]
			{
				"__EXCHANGE__",
				resellerId,
				text
			});
		}

		private bool GetIPSkiplistingEnabled()
		{
			int flagsValue = base.GetFlagsValue("msExchTenantPerimeterSettingsFlags");
			return (flagsValue & 4) != 0;
		}

		private IPListConfig GetInternalServerIPAddresses()
		{
			return this.GetIPAddresses("msExchTenantPerimeterSettingsInternalServerIPAddresses");
		}

		private IPListConfig GetGatewayIPAddresses()
		{
			return this.GetIPAddresses("msExchTenantPerimeterSettingsGatewayIPAddresses");
		}

		private IPListConfig GetIPAddresses(string attrName)
		{
			DirectoryAttribute attribute = base.ADEntry.GetAttribute(attrName);
			if (attribute == null)
			{
				return EhfCompanyItem.EmptyIPList;
			}
			IPListConfig iplistConfig = new IPListConfig();
			iplistConfig.TargetAction = TargetAction.Set;
			iplistConfig.IPList = new string[attribute.Count];
			int num = 0;
			foreach (object obj in attribute)
			{
				if (obj != null)
				{
					string asciiStringValueOfAttribute = ExSearchResultEntry.GetAsciiStringValueOfAttribute(obj, attrName);
					if (!string.IsNullOrEmpty(asciiStringValueOfAttribute))
					{
						IPAddress ipaddress;
						if (!IPAddress.TryParse(asciiStringValueOfAttribute, out ipaddress))
						{
							base.AddSyncError(base.DiagSession.LogAndTraceError("Unable to parse IP address value ({0}) of attribute {1} for AD object ({2}); the attribute will not be pushed to EHF", new object[]
							{
								asciiStringValueOfAttribute,
								attrName,
								base.DistinguishedName
							}));
							return null;
						}
						if (ipaddress.AddressFamily != AddressFamily.InterNetwork)
						{
							base.AddSyncError(base.DiagSession.LogAndTraceError("Address Family {0} of IP address value {1} of attribute {2} for AD object ({3}) is not supported; the attribute will not be pushed to EHF", new object[]
							{
								ipaddress.AddressFamily,
								asciiStringValueOfAttribute,
								attrName,
								base.DistinguishedName
							}));
							return null;
						}
						if (ipaddress == IPAddress.Any || ipaddress == IPAddress.Broadcast || ipaddress == IPAddress.None || IPAddress.IsLoopback(ipaddress))
						{
							base.AddSyncError(base.DiagSession.LogAndTraceError("IP address value {0} of attribute {1} for AD object ({2}) is not allowed for this attribute; the attribute will not be pushed to EHF", new object[]
							{
								asciiStringValueOfAttribute,
								attrName,
								base.DistinguishedName
							}));
							return null;
						}
						iplistConfig.IPList[num++] = ipaddress.ToString();
					}
				}
			}
			if (num < attribute.Count)
			{
				string[] array = new string[num];
				Array.Copy(iplistConfig.IPList, array, num);
				iplistConfig.IPList = array;
			}
			return iplistConfig;
		}

		private ServicePlan GetServicePlanId()
		{
			return ServicePlan.None;
		}

		private void SetEhfId(int companyId)
		{
			if (this.companyId != 0)
			{
				throw new InvalidOperationException("Company already has a valid EHF company ID");
			}
			this.companyId = companyId;
			if (this.company != null)
			{
				this.company.CompanyId = companyId;
			}
			if (this.settings != null)
			{
				this.settings.CompanyId = companyId;
			}
		}

		private void ValidateDNDepth(ADObjectId perimeterSettingsId)
		{
			if (perimeterSettingsId.Depth < 4)
			{
				base.DiagSession.LogAndTraceError("Depth of Perimeter Settings DN <{0}> is less than 4", new object[]
				{
					perimeterSettingsId.DistinguishedName
				});
				throw new InvalidOperationException("Depth of Perimeter Settings DN is less than 4: " + perimeterSettingsId.DistinguishedName);
			}
		}

		public EhfADResultCode TryUpdateAdminSyncEnabledFlag(EhfADAdapter ehfADAdapter, bool forceDisable)
		{
			string ouRootDN = this.GetOuRootDN();
			if (string.IsNullOrEmpty(ouRootDN))
			{
				base.DiagSession.LogAndTraceError("Could not find the OU for company <{0}>. Not trying to update the adminsync flag in OU container.", new object[]
				{
					base.DistinguishedName
				});
				return EhfADResultCode.Success;
			}
			if (ExSearchResultEntry.IsDeletedDN(ouRootDN))
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "OU root DN <{0}> is for a deleted organization. Not updating the OU adminsync flag.", new object[]
				{
					ouRootDN
				});
				return EhfADResultCode.Success;
			}
			ExSearchResultEntry exSearchResultEntry = ehfADAdapter.ReadObjectEntry(ouRootDN, false, EhfCompanyItem.ProvisioningFlagsAttribute);
			if (exSearchResultEntry == null)
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Organization <{0}> is deleted or not yet replicated. Not updating the OU adminsync flag.", new object[]
				{
					base.DistinguishedName
				});
				base.AddSyncError("Failed to get the OrganizationUnit object <{0}>. Not updating the OU adminsync flag.", new object[]
				{
					base.DistinguishedName
				});
				return EhfADResultCode.Failure;
			}
			int flagsValue = EhfSyncItem.GetFlagsValue("msExchProvisioningFlags", exSearchResultEntry, this);
			int flagsValue2 = base.GetFlagsValue("msExchTenantPerimeterSettingsFlags");
			int num = EhfCompanyItem.CalculateProvisioningFlags((OrganizationProvisioningFlags)flagsValue, (PerimeterFlags)flagsValue2, forceDisable);
			if (flagsValue == num)
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Not Updating the OUflag of <{0}> value <{1}> since the flag is already in sync with PerimeterConfig <{2}>.", new object[]
				{
					exSearchResultEntry.DistinguishedName,
					flagsValue,
					flagsValue2
				});
				return EhfADResultCode.Success;
			}
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Updating the OU flag of <{0}> from <{1}> to <{2}>", new object[]
			{
				exSearchResultEntry.DistinguishedName,
				flagsValue,
				num
			});
			if (this.IsCompanyStillAliveInAD(ehfADAdapter))
			{
				try
				{
					ehfADAdapter.SetAttributeValue(exSearchResultEntry.DistinguishedName, "msExchProvisioningFlags", num.ToString());
					return EhfADResultCode.Success;
				}
				catch (ExDirectoryException ex)
				{
					base.DiagSession.LogAndTraceException(ex, "Exception occured while trying to disable the adminsync flag in <{0}>", new object[]
					{
						exSearchResultEntry.DistinguishedName
					});
					base.AddSyncError("Exception occured while trying to disable the adminsync flag in <{0}>; exception details: {1}", new object[]
					{
						exSearchResultEntry.DistinguishedName,
						ex
					});
					return EhfADResultCode.Failure;
				}
			}
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.High, "Organization <{0}> with companyId <{1}> is deleted, not clearing the companyId and not setting the OU flag", new object[]
			{
				base.DistinguishedName,
				this.companyId
			});
			return EhfADResultCode.Success;
		}

		private bool IsCompanyStillAliveInAD(EhfADAdapter adapter)
		{
			ExSearchResultEntry exSearchResultEntry = adapter.ReadObjectEntry(this.GetCompanyGuid(), false, EhfCompanyItem.OrganizationUnitAttribute);
			if (exSearchResultEntry == null)
			{
				return false;
			}
			DirectoryAttribute attribute = exSearchResultEntry.GetAttribute("msExchOURoot");
			return !ExSearchResultEntry.IsDeletedDN((string)attribute[0]);
		}

		public const string CompanyNamePrefix = "__EXCHANGE__";

		public const int InvalidEhfCompanyId = 0;

		public const string EhfCompanyGuid = "msEdgeSyncEhfCompanyGuid";

		private const InheritanceSettings CompanyInheritanceSettings = InheritanceSettings.InheritOutboundIPConfig;

		private const PropagationSettings CompanyPropagationSettings = PropagationSettings.PropagateOutboundIPConfig;

		private static readonly IPListConfig EmptyIPList = EhfCompanyItem.CreateEmptyIPList();

		private static readonly string[] ProvisioningFlagsAttribute = new string[]
		{
			"msExchProvisioningFlags"
		};

		private static readonly string[] OrganizationUnitAttribute = new string[]
		{
			"msExchOURoot"
		};

		private int companyId;

		private Company company;

		private CompanyConfigurationSettings settings;

		private CompanyConfigurationSettings settingsForCreate;

		private bool forceDomainSync;
	}
}
