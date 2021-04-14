using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LicensingRpcParameters : RpcParameters
	{
		public RmsClientManagerContext ClientManagerContext
		{
			get
			{
				if (this.clientManagerContext == null)
				{
					OrganizationId organizationId = base.GetParameterValue("OrgId") as OrganizationId;
					if (organizationId == null)
					{
						throw new ArgumentNullException("orgId");
					}
					object parameterValue = base.GetParameterValue("ContextId");
					RmsClientManagerContext.ContextId contextId = (RmsClientManagerContext.ContextId)parameterValue;
					string contextValue = base.GetParameterValue("ContextValue") as string;
					parameterValue = base.GetParameterValue("TransactioniId");
					Guid transactionId = (Guid)parameterValue;
					Guid externalDirectoryOrgId = (Guid)base.GetParameterValue("ExternalDirectoryOrgId");
					this.clientManagerContext = new RmsClientManagerContext(organizationId, contextId, contextValue, transactionId, externalDirectoryOrgId);
				}
				return this.clientManagerContext;
			}
		}

		public LicensingRpcParameters(byte[] data) : base(data)
		{
		}

		public LicensingRpcParameters(RmsClientManagerContext rmsClientManagerContext)
		{
			if (rmsClientManagerContext == null)
			{
				throw new ArgumentNullException("rmsClientManagerContext");
			}
			if (rmsClientManagerContext.OrgId == null)
			{
				throw new ArgumentNullException("rmsClientManagerContext.OrgId");
			}
			base.SetParameterValue("OrgId", rmsClientManagerContext.OrgId);
			base.SetParameterValue("ContextId", rmsClientManagerContext.ContextID);
			base.SetParameterValue("ContextValue", rmsClientManagerContext.ContextValue);
			base.SetParameterValue("TransactioniId", rmsClientManagerContext.TransactionId);
			base.SetParameterValue("ExternalDirectoryOrgId", rmsClientManagerContext.ExternalDirectoryOrgId);
		}

		private const string OrgIdParameterName = "OrgId";

		private const string ContextIdParameterName = "ContextId";

		private const string ContextValueParameterName = "ContextValue";

		private const string TransactionIdParameterName = "TransactioniId";

		private const string ExternalDirectoryOrgIdParameterName = "ExternalDirectoryOrgId";

		private RmsClientManagerContext clientManagerContext;
	}
}
