using System;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Logging;
using Microsoft.Exchange.Net.Mserve;
using Microsoft.Exchange.Net.Mserve.ProvisionRequest;
using Microsoft.Exchange.Net.Mserve.ProvisionResponse;

namespace Microsoft.Exchange.EdgeSync.Mserve
{
	internal class MserveEdgeSyncService : MserveWebService
	{
		public MserveEdgeSyncService(string provisionUrl, string settingsUrl, string remoteCertSubject, string clientToken, EdgeSyncLogSession logSession, bool batchMode, bool trackDuplicatedAddEntries) : base(provisionUrl, settingsUrl, remoteCertSubject, clientToken, batchMode)
		{
			base.TrackDuplicatedAddEntries = trackDuplicatedAddEntries;
			this.logSession = logSession;
		}

		protected override void LogProvisionAccountDetail(string type, Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType account)
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.AppendFormat("Type:{0} Name:{1} PartnerId:{2}", type, account.Name, account.PartnerID);
			this.logSession.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.TargetConnection, stringBuilder.ToString(), "ProvisionRequest");
		}

		protected override void LogResponseAccountDetail(OperationType type, Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType account)
		{
			StringBuilder stringBuilder;
			if (account.Fault != null)
			{
				stringBuilder = new StringBuilder(300);
				stringBuilder.AppendFormat("Type:{0} Name:{1} PartnerId:{2} Status:{3} Fault:{4} FaultCode:{5} FaultDetail:{6}", new object[]
				{
					type,
					account.Name,
					account.PartnerID,
					account.Status,
					account.Fault.Faultstring,
					account.Fault.Faultcode,
					account.Fault.Detail
				});
			}
			else
			{
				stringBuilder = new StringBuilder(200);
				stringBuilder.AppendFormat("Type:{0} Name:{1} PartnerId:{2} Status:{3}", new object[]
				{
					type,
					account.Name,
					account.PartnerID,
					account.Status
				});
			}
			this.logSession.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.TargetConnection, stringBuilder.ToString(), "ProvisionResponse");
		}

		protected override void LogProvisionResponse(Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision response)
		{
			StringBuilder stringBuilder;
			if (response.Fault != null)
			{
				stringBuilder = new StringBuilder(200);
				stringBuilder.AppendFormat("Status:{0} Fault:{1} FaultCode:{2} FaultDetail:{3}", new object[]
				{
					response.Status,
					response.Fault.Faultstring,
					response.Fault.Faultcode,
					response.Fault.Detail
				});
			}
			else
			{
				stringBuilder = new StringBuilder(20);
				stringBuilder.AppendFormat("Status:{0}", response.Status);
			}
			this.logSession.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.TargetConnection, stringBuilder.ToString(), "ProvisionRootResponse");
		}

		protected override void LogProvisionRequest(string url)
		{
			this.logSession.LogEvent(EdgeSyncLoggingLevel.Medium, EdgeSyncEvent.TargetConnection, url, "Sending a new batch of provision requests");
		}

		private readonly EdgeSyncLogSession logSession;
	}
}
