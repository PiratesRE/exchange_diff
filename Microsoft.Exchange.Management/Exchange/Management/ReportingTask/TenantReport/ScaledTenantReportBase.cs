using System;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	public abstract class ScaledTenantReportBase<TReportObject> : TenantReportBase<TReportObject> where TReportObject : ReportObject, IDateColumn, ITenantColumn
	{
		protected override DataMartType DataMartType
		{
			get
			{
				return DataMartType.TenantsScaled;
			}
		}
	}
}
