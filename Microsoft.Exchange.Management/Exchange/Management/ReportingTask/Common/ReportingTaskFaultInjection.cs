using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Exchange.Diagnostics.Components.ReportingTask;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	public class ReportingTaskFaultInjection
	{
		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ReportingTaskFaultInjection.faultInjectionTracer == null)
				{
					lock (ReportingTaskFaultInjection.lockObject)
					{
						if (ReportingTaskFaultInjection.faultInjectionTracer == null)
						{
							FaultInjectionTrace faultInjectionTrace = ExTraceGlobals.FaultInjectionTracer;
							faultInjectionTrace.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(ReportingTaskFaultInjection.Callback));
							ReportingTaskFaultInjection.faultInjectionTracer = faultInjectionTrace;
						}
					}
				}
				return ReportingTaskFaultInjection.faultInjectionTracer;
			}
		}

		public static Exception Callback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null)
			{
				if (typeof(DatabaseException).FullName.Equals(exceptionType))
				{
					return new DatabaseException(0, "DatabaseException");
				}
				if (typeof(SqlTypeException).FullName.Equals(exceptionType))
				{
					return new SqlTypeException("SqlTypeException");
				}
				if (typeof(InvalidOperationException).FullName.Equals(exceptionType))
				{
					SqlCommand sqlCommand = new SqlCommand();
					sqlCommand.ExecuteScalar();
				}
			}
			return result;
		}

		private static object lockObject = new object();

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
