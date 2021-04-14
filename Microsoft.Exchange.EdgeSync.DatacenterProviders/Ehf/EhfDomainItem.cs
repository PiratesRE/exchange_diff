using System;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfDomainItem : EhfSyncItem
	{
		protected EhfDomainItem(ExSearchResultEntry entry, EdgeSyncDiag diagSession) : this(entry, 0, diagSession)
		{
			this.domain.CompanyId = this.GetEntryCompanyId();
		}

		protected EhfDomainItem(ExSearchResultEntry entry, int ehfCompanyId, EdgeSyncDiag diagSession) : base(entry, diagSession)
		{
			if (!entry.Attributes.ContainsKey("msEdgeSyncEhfCompanyGuid"))
			{
				throw new InvalidOperationException(string.Format("DomainItem <{0}> does not contain the companyGuid", entry.DistinguishedName));
			}
			if (!entry.IsDeleted)
			{
				this.InitializeDomainType();
			}
			this.domain = new Domain();
			this.domain.CompanyId = ehfCompanyId;
			this.domain.DomainGuid = new Guid?(base.GetObjectGuid());
			this.domain.Name = this.GetDomainName();
			this.domain.InheritFromCompany = (InheritanceSettings.InheritRecipientLevelRoutingConfig | InheritanceSettings.InheritSkipListConfig | InheritanceSettings.InheritDirectoryEdgeBlockModeConfig);
			this.domain.IsEnabled = true;
			this.domain.IsValid = true;
			this.domain.MailServerType = MailServerType.Exchange2007;
			this.domain.SmtpProfileName = "InboundSmtpProfile";
		}

		public AcceptedDomainType AcceptedDomainType
		{
			get
			{
				if (base.ADEntry.IsDeleted)
				{
					throw new InvalidOperationException("Cannot get domain type of a deleted domain");
				}
				return this.acceptedDomainType;
			}
		}

		public bool OutboundOnly
		{
			get
			{
				return this.outboundOnly;
			}
		}

		public Domain Domain
		{
			get
			{
				return this.domain;
			}
		}

		public string Name
		{
			get
			{
				return this.domain.Name;
			}
		}

		public static EhfDomainItem CreateForActive(ExSearchResultEntry entry, EdgeSyncDiag diagSession)
		{
			return new EhfDomainItem(entry, diagSession);
		}

		public static EhfDomainItem CreateForTombstone(ExSearchResultEntry entry, EdgeSyncDiag diagSession)
		{
			return new EhfDomainItem(entry, diagSession);
		}

		public static EhfDomainItem CreateIfAuthoritative(ExSearchResultEntry entry, int ehfCompanyId, EdgeSyncDiag diagSession)
		{
			EhfDomainItem ehfDomainItem = new EhfDomainItem(entry, ehfCompanyId, diagSession);
			if (ehfDomainItem.AcceptedDomainType != AcceptedDomainType.Authoritative)
			{
				return null;
			}
			return ehfDomainItem;
		}

		public static void ClearForceDomainSyncFlagFromPerimeterConfig(EhfADAdapter adAdapter, Guid companyGuid)
		{
			adAdapter.SetAttributeValue(companyGuid, "msExchTransportInboundSettings", 0.ToString());
		}

		public Guid GetDomainGuid()
		{
			return this.domain.DomainGuid.Value;
		}

		public EhfADResultCode TryClearForceDomainSyncFlagFromPerimeterConfig(EhfADAdapter adAdapter)
		{
			if (!this.clearForceDomainSyncFlag)
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.High, "Domain <{0}> does not have [clearForceDomainSyncFlag] set. Not clearing the forceDomainSync flag", new object[]
				{
					base.DistinguishedName
				});
				return EhfADResultCode.Success;
			}
			Guid entryCompanyGuid = this.GetEntryCompanyGuid();
			base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Clearing the force-domainsync flag set on the perimeterconfig with Guid <{0}> for domain <{1}>.", new object[]
			{
				entryCompanyGuid,
				base.DistinguishedName
			});
			try
			{
				EhfDomainItem.ClearForceDomainSyncFlagFromPerimeterConfig(adAdapter, entryCompanyGuid);
			}
			catch (ExDirectoryException ex)
			{
				if (ex.ResultCode == ResultCode.NoSuchObject)
				{
					base.DiagSession.LogAndTraceException(ex, "NoSuchObject error occurred while trying to clear the ForceDomainSync flag in Perimeter Settings for domain <{0}>:<{1}>", new object[]
					{
						base.DistinguishedName,
						entryCompanyGuid
					});
					return EhfADResultCode.NoSuchObject;
				}
				base.DiagSession.LogAndTraceException(ex, "Exception occurred while trying to clear the ForceDomainSync flag in Perimeter Settings for domain <{0}>:<{1}>", new object[]
				{
					base.DistinguishedName,
					entryCompanyGuid
				});
				base.DiagSession.EventLog.LogEvent(EdgeSyncEventLogConstants.Tuple_EhfFailedToClearForceDomainSyncFlagFromDomainSync, null, new object[]
				{
					base.DistinguishedName,
					ex.Message
				});
				return EhfADResultCode.Failure;
			}
			return EhfADResultCode.Success;
		}

		public bool SetForcedDomainSync(Guid ehfCompanyGuid)
		{
			if (this.GetEntryCompanyGuid() == ehfCompanyGuid)
			{
				this.clearForceDomainSyncFlag = true;
				return true;
			}
			return false;
		}

		protected virtual string GetDomainName()
		{
			DirectoryAttribute attribute = base.ADEntry.GetAttribute("msExchAcceptedDomainName");
			if (attribute == null || attribute[0] == null)
			{
				throw new InvalidOperationException("Domain name attribute is not present in accepted domain AD entry");
			}
			return (string)attribute[0];
		}

		private int GetEntryCompanyId()
		{
			DirectoryAttribute attribute = base.ADEntry.GetAttribute("msExchTenantPerimeterSettingsOrgID");
			if (attribute == null || attribute[0] == null)
			{
				throw new InvalidOperationException("EHF company AD attribute is not present in accepted domain AD entry, but it was supposed to be added by PreDecorate delegate");
			}
			return int.Parse((string)attribute[0]);
		}

		private Guid GetEntryCompanyGuid()
		{
			DirectoryAttribute attribute = base.ADEntry.GetAttribute("msEdgeSyncEhfCompanyGuid");
			if (attribute == null || attribute[0] == null)
			{
				throw new InvalidOperationException("EHF companyGuid is not present in accepted domain AD entry, but it was supposed to be added by PreDecorate delegate or during CreateDomainForNewCompany");
			}
			return new Guid((string)attribute[0]);
		}

		private void InitializeDomainType()
		{
			int flagsValue = base.GetFlagsValue("msExchAcceptedDomainFlags");
			this.acceptedDomainType = (AcceptedDomainType)(flagsValue & 3);
			this.outboundOnly = ((flagsValue & 256) != 0);
		}

		private const InheritanceSettings DomainInheritanceSettings = InheritanceSettings.InheritRecipientLevelRoutingConfig | InheritanceSettings.InheritSkipListConfig | InheritanceSettings.InheritDirectoryEdgeBlockModeConfig;

		private Domain domain;

		private AcceptedDomainType acceptedDomainType;

		private bool outboundOnly;

		private bool clearForceDomainSyncFlag;
	}
}
