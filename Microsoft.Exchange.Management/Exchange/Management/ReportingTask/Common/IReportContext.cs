using System;
using System.Linq;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal interface IReportContext : IDisposable
	{
		IQueryable<TReportObject> GetReports<TReportObject>() where TReportObject : ReportObject;

		IQueryable<TReportObject> GetScaledQuery<TReportObject>(IQueryable<TReportObject> query) where TReportObject : ReportObject;

		string GetSqlCommandText(IQueryable query);

		void ChangeViewName(Type reportType, string viewName);
	}
}
