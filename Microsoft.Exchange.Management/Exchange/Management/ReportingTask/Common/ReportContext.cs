using System;
using System.Data;
using System.Data.Common;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class ReportContext : DisposeTrackableBase, IReportContext, IDisposable
	{
		public ReportContext(IDbConnection connection)
		{
			this.mappingSource = new MappingSourceWrapper(new AttributeMappingSource());
			this.dataContext = new ReportDataContext(connection, this.mappingSource);
			this.dataContext.ObjectTrackingEnabled = false;
		}

		public IQueryable<TReportObject> GetReports<TReportObject>() where TReportObject : ReportObject
		{
			return this.dataContext.GetTable<TReportObject>();
		}

		public IQueryable<TReportObject> GetScaledQuery<TReportObject>(IQueryable<TReportObject> query) where TReportObject : ReportObject
		{
			return this.dataContext.GetScaledQuery<TReportObject>(query);
		}

		public string GetSqlCommandText(IQueryable query)
		{
			DbCommand command = this.dataContext.GetCommand(query);
			StringBuilder stringBuilder = new StringBuilder(command.CommandText);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Parameters:");
			foreach (object obj in command.Parameters)
			{
				DbParameter dbParameter = (DbParameter)obj;
				stringBuilder.AppendFormat("{0}={1}", dbParameter.ParameterName, dbParameter.Value);
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		public void ChangeViewName(Type reportType, string viewName)
		{
			if (!string.IsNullOrEmpty(viewName))
			{
				this.mappingSource.AddMapping(reportType, viewName);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ReportContext>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.dataContext != null)
			{
				this.dataContext.Dispose();
				this.dataContext = null;
			}
		}

		private readonly MappingSourceWrapper mappingSource;

		private ReportDataContext dataContext;
	}
}
