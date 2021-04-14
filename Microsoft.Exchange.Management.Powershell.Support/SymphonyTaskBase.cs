using System;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	public class SymphonyTaskBase : Task
	{
		[Parameter(Mandatory = false)]
		public string Endpoint { get; set; }

		[Parameter(Mandatory = false)]
		public string CertSubject { get; set; }

		internal Uri WorkloadUri { get; set; }

		internal X509Certificate2 Certificate { get; set; }

		internal WorkItemInfo GetWorkItemById(string workItemId)
		{
			WorkItemStatus[] array = (WorkItemStatus[])Enum.GetValues(typeof(WorkItemStatus));
			using (ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler> workloadClient = new ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler>(this.WorkloadUri, this.Certificate))
			{
				WorkItemStatus[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					SymphonyTaskBase.<>c__DisplayClass7 CS$<>8__locals3 = new SymphonyTaskBase.<>c__DisplayClass7();
					CS$<>8__locals3.status = array2[i];
					WorkItemQueryResult result = new WorkItemQueryResult();
					WorkItemInfo workItemInfo;
					do
					{
						workloadClient.CallSymphony(delegate
						{
							result = workloadClient.Proxy.QueryWorkItems(null, null, null, CS$<>8__locals3.status, 1000, result.Bookmark);
						}, this.WorkloadUri.ToString());
						workItemInfo = result.WorkItems.SingleOrDefault((WorkItemInfo w) => string.Equals(w.WorkItemId, workItemId));
					}
					while (workItemInfo == null && result.HasMoreResults);
					if (workItemInfo != null)
					{
						return workItemInfo;
					}
				}
				throw new WorkItemNotFoundException(workItemId);
			}
			WorkItemInfo result2;
			return result2;
		}

		internal WorkItemInfo GetWorkitemByIdAndTenantId(string workItemId, Guid tenantId)
		{
			WorkItemInfo retrieved2;
			using (ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler> workloadClient = new ProxyWrapper<UpgradeHandlerClient, IUpgradeHandler>(this.WorkloadUri, this.Certificate))
			{
				WorkItemInfo retrieved = null;
				workloadClient.CallSymphony(delegate
				{
					retrieved = workloadClient.Proxy.QueryTenantWorkItems(tenantId).SingleOrDefault((WorkItemInfo w) => string.Equals(w.WorkItemId, workItemId));
				}, this.WorkloadUri.ToString());
				if (retrieved == null)
				{
					throw new WorkItemNotFoundException(workItemId);
				}
				retrieved2 = retrieved;
			}
			return retrieved2;
		}

		protected override void InternalBeginProcessing()
		{
			Uri baseUri = string.IsNullOrEmpty(this.Endpoint) ? UpgradeHandlerContext.AnchorConfig.GetConfig<Uri>("WebServiceUri") : new Uri(this.Endpoint);
			this.WorkloadUri = new Uri(baseUri, "WorkloadService.svc");
			string subject = string.IsNullOrEmpty(this.CertSubject) ? UpgradeHandlerContext.AnchorConfig.GetConfig<string>("CertificateSubject") : this.CertSubject;
			this.Certificate = CertificateHelper.GetExchangeCertificate(subject);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is LocalizedException;
		}

		protected object GetPropertyValue(PSMemberInfoCollection<PSPropertyInfo> properties, string propertyName)
		{
			object result;
			try
			{
				result = properties.Single((PSPropertyInfo PropertyInfo) => PropertyInfo.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)).Value;
			}
			catch (InvalidOperationException exception)
			{
				base.ThrowTerminatingError(exception, ErrorCategory.InvalidData, properties);
				result = null;
			}
			return result;
		}
	}
}
