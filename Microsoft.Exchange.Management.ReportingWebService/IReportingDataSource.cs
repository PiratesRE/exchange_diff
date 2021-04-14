using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal interface IReportingDataSource
	{
		IList<T> GetData<T>(IEntity entity, Expression expression);

		IList GetData(IEntity entity, Expression expression);
	}
}
