using System;
using System.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class DetailsTemplateFilterBuilder : IExchangeCommandFilterBuilder
	{
		public virtual void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			ADObjectId adobjectId = row["Identity"] as ADObjectId;
			parameterList = ((adobjectId != null) ? string.Format(" -Identity '{0}'", adobjectId) : null);
			filter = null;
			preArgs = null;
		}
	}
}
