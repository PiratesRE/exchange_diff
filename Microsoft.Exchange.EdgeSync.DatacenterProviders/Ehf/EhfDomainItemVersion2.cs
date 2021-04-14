using System;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfDomainItemVersion2 : EhfDomainItem
	{
		private EhfDomainItemVersion2(ExSearchResultEntry entry, int[] connectorIds, EdgeSyncDiag diagSession) : base(entry, diagSession)
		{
			this.InitializeDomainSettings(connectorIds);
		}

		private EhfDomainItemVersion2(ExSearchResultEntry entry, int ehfCompanyId, int[] connectorIds, EdgeSyncDiag diagSession) : base(entry, ehfCompanyId, diagSession)
		{
			this.InitializeDomainSettings(connectorIds);
		}

		public static EhfDomainItemVersion2 CreateEhfDomainItem(ExSearchResultEntry entry, int[] connectorIds, EdgeSyncDiag diagSession)
		{
			return new EhfDomainItemVersion2(entry, connectorIds, diagSession);
		}

		public static EhfDomainItemVersion2 CreateEhfDomainItem(ExSearchResultEntry entry, int ehfCompanyId, int[] connectorIds, EdgeSyncDiag diagSession)
		{
			return new EhfDomainItemVersion2(entry, ehfCompanyId, connectorIds, diagSession);
		}

		public static EhfDomainItem CreateForOutboundOnlyTombstone(ExSearchResultEntry entry, EdgeSyncDiag diagSession)
		{
			EhfDomainItem ehfDomainItem = EhfDomainItem.CreateForTombstone(entry, diagSession);
			ehfDomainItem.Domain.Name = AcceptedDomain.FormatEhfOutboundOnlyDomainName(ehfDomainItem.Domain.Name, ehfDomainItem.Domain.DomainGuid.Value);
			return ehfDomainItem;
		}

		public bool TriedToUpdateDomain
		{
			get
			{
				return this.triedToUpdateDomain;
			}
			set
			{
				this.triedToUpdateDomain = value;
			}
		}

		public bool DomainCreatedWithGuid
		{
			get
			{
				return this.domainCreatedWithGuid;
			}
		}

		protected override string GetDomainName()
		{
			string domainName = base.GetDomainName();
			if (base.ADEntry.IsDeleted || !base.OutboundOnly)
			{
				return domainName;
			}
			return AcceptedDomain.FormatEhfOutboundOnlyDomainName(domainName, base.Domain.DomainGuid.Value);
		}

		private void InitializeDomainSettings(int[] connectorIds)
		{
			if (base.Domain.Settings == null)
			{
				base.Domain.Settings = new DomainConfigurationSettings();
			}
			base.Domain.Settings.DomainName = base.Domain.Name;
			base.Domain.Settings.DomainGuid = base.Domain.DomainGuid;
			base.Domain.Settings.CompanyId = base.Domain.CompanyId;
			base.Domain.Settings.MailFlowType = DomainMailFlowType.InboundOutbound;
			if (connectorIds != null)
			{
				base.Domain.Settings.ConnectorId = connectorIds;
			}
			this.domainCreatedWithGuid = (base.OutboundOnly && !base.IsDeleted);
		}

		public void SetAsGuidDomain()
		{
			if (this.domainCreatedWithGuid)
			{
				throw new InvalidOperationException(string.Format("Trying to set a Guid-Domain twice. DomainName: {0}", base.Domain.Name));
			}
			base.Domain.Name = AcceptedDomain.FormatEhfOutboundOnlyDomainName(base.Domain.Name, base.GetDomainGuid());
			base.Domain.Settings.DomainName = base.Domain.Name;
			this.domainCreatedWithGuid = true;
		}

		public EhfADResultCode TrySetDuplicateDomain(EhfADAdapter ehfADAdapter, bool isDuplicate)
		{
			int flagsValue = base.GetFlagsValue("msExchTransportInboundSettings");
			int num;
			if (isDuplicate)
			{
				num = (flagsValue | 1);
			}
			else
			{
				num = (flagsValue & -2);
			}
			if (flagsValue == num)
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "DuplicateDetected field is set to the expected value {0} for domain <{1}>:<{2}>. Not setting the value.", new object[]
				{
					flagsValue,
					base.DistinguishedName,
					base.GetDomainGuid()
				});
				return EhfADResultCode.Success;
			}
			EhfADResultCode result;
			try
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Setting DuplicateDetected field from {0} to {1} for domain <{2}>:<{3}>.", new object[]
				{
					flagsValue,
					num,
					base.DistinguishedName,
					base.GetDomainGuid()
				});
				ehfADAdapter.SetAttributeValue(base.GetDomainGuid(), "msExchTransportInboundSettings", num.ToString());
				result = EhfADResultCode.Success;
			}
			catch (ExDirectoryException ex)
			{
				if (ex.ResultCode == ResultCode.NoSuchObject)
				{
					base.DiagSession.LogAndTraceException(ex, "NoSuchObject error occurred while trying to set the DuplicateDetected flag for domain <{0}>:<{1}>", new object[]
					{
						base.DistinguishedName,
						base.GetDomainGuid()
					});
					result = EhfADResultCode.NoSuchObject;
				}
				else
				{
					base.DiagSession.LogAndTraceException(ex, "Exception occurred while trying to set the DuplicateDetected flag for domain <{0}>:<{1}>", new object[]
					{
						base.DistinguishedName,
						base.GetDomainGuid()
					});
					result = EhfADResultCode.Failure;
				}
			}
			return result;
		}

		private bool triedToUpdateDomain;

		private bool domainCreatedWithGuid;
	}
}
