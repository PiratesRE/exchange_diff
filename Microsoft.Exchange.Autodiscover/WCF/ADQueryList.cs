using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class ADQueryList : QueryListBase<ADQueryResult>
	{
		internal ADQueryList(OrganizationId organizationId, ADObjectId searchRoot, IBudget budget)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<OrganizationId, ADObjectId>((long)this.GetHashCode(), "Constructing ADQueryList for organizationId {0} searchRoot '{1}'.", organizationId, searchRoot);
			this.organizationId = organizationId;
			this.searchRoot = searchRoot;
			this.budget = budget;
		}

		protected override ADQueryResult CreateResult(UserResultMapping userResultMapping)
		{
			return new ADQueryResult(userResultMapping);
		}

		public override void Execute()
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<ADQueryList, int>((long)this.GetHashCode(), "{0} Execute() called for {1} addresses.", this, this.resultDictionary.Values.Count);
			this.budget.CheckOverBudget();
			IRecipientSession adRecipientSession = this.GetRecipientSessionForOrganization(this.searchRoot, this.organizationId);
			Result<ADRecipient>[] adRecipientQueryResults = null;
			Guid[] archiveGuid;
			if (this.TryCreateExchangeGuidArray(out archiveGuid))
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.TrackLatency(ServiceLatencyMetadata.RequestedUserADLatency, delegate()
				{
					adRecipientQueryResults = this.FindByExchangeGuidsIncludingArchive(adRecipientSession, archiveGuid);
				});
			}
			else
			{
				SmtpProxyAddress[] smtpProxyAddresses = this.CreateSmtpProxyAddressArray();
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.TrackLatency(ServiceLatencyMetadata.RequestedUserADLatency, delegate()
				{
					adRecipientQueryResults = adRecipientSession.FindByProxyAddresses(smtpProxyAddresses);
				});
			}
			this.SetQueryResults(adRecipientQueryResults);
		}

		private bool TryCreateExchangeGuidArray(out Guid[] exchangeGuid)
		{
			bool result = true;
			exchangeGuid = null;
			int num = 0;
			foreach (ADQueryResult adqueryResult in this.resultDictionary.Values)
			{
				string smtpAddress = adqueryResult.UserResultMapping.SmtpProxyAddress.SmtpAddress;
				Guid guid;
				if (!SmtpProxyAddress.TryDeencapsulateExchangeGuid(smtpAddress, out guid))
				{
					result = false;
					exchangeGuid = null;
					break;
				}
				if (exchangeGuid == null)
				{
					exchangeGuid = new Guid[this.resultDictionary.Count];
				}
				exchangeGuid[num] = guid;
				num++;
			}
			return result;
		}

		private SmtpProxyAddress[] CreateSmtpProxyAddressArray()
		{
			SmtpProxyAddress[] array = new SmtpProxyAddress[this.resultDictionary.Count];
			int num = 0;
			foreach (ADQueryResult adqueryResult in this.resultDictionary.Values)
			{
				array[num] = adqueryResult.UserResultMapping.SmtpProxyAddress;
				num++;
			}
			return array;
		}

		private Result<ADRecipient>[] FindByExchangeGuidsIncludingArchive(IRecipientSession adRecipientSession, Guid[] archiveGuids)
		{
			Result<ADRecipient>[] array = new Result<ADRecipient>[archiveGuids.Length];
			for (int i = 0; i < archiveGuids.Length; i++)
			{
				Result<ADRecipient>[] array2 = adRecipientSession.FindByExchangeGuidsIncludingArchive(new Guid[]
				{
					archiveGuids[i]
				});
				if (array2.Length > 0)
				{
					array[i] = array2[0];
				}
				else
				{
					array[i] = new Result<ADRecipient>(null, ProviderError.NotFound);
				}
			}
			return array;
		}

		private void SetQueryResults(Result<ADRecipient>[] adRecipientQueryResults)
		{
			int num = 0;
			foreach (ADQueryResult adqueryResult in this.resultDictionary.Values)
			{
				adqueryResult.Result = adRecipientQueryResults[num];
				num++;
			}
		}

		public override string ToString()
		{
			return string.Format(base.ToString() + "-OrganizationId:{0}-SearchRoot:{1}", this.organizationId, (this.searchRoot != null) ? this.searchRoot.ToString() : "<Null>");
		}

		private IRecipientSession GetRecipientSessionForOrganization(ADObjectId searchRoot, OrganizationId organizationId)
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<OrganizationId, ADObjectId>((long)this.GetHashCode(), "GetRecipientSessionForOrganization() called for organizationId {0} searchRoot '{1}'.", organizationId, searchRoot);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, searchRoot, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 268, "GetRecipientSessionForOrganization", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\WCF\\Commands\\ADQueryList.cs");
			tenantOrRootOrgRecipientSession.ServerTimeout = new TimeSpan?(ADQueryList.RecipientSessionServerTimeout);
			return tenantOrRootOrgRecipientSession;
		}

		internal static TimeSpan RecipientSessionServerTimeout = TimeSpan.FromSeconds(30.0);

		private OrganizationId organizationId;

		private ADObjectId searchRoot;

		private IBudget budget;
	}
}
