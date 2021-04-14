using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class TaskContext : ITaskContext
	{
		public TaskContext(Task task)
		{
			this.task = task;
		}

		public Guid CurrentOrganizationGuid
		{
			get
			{
				return this.task.CurrentOrganizationId.OrganizationalUnit.ObjectGuid;
			}
		}

		public Guid CurrentOrganizationExternalDirectoryId
		{
			get
			{
				return Guid.Parse(this.task.CurrentOrganizationId.ToExternalDirectoryOrganizationId());
			}
		}

		public bool IsCurrentOrganizationForestWide
		{
			get
			{
				return this.task.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId);
			}
		}

		public void WriteError(LocalizedException localizedException, ExchangeErrorCategory exchangeErrorCategory, object target)
		{
			this.task.WriteError(localizedException, exchangeErrorCategory, target);
		}

		public void WriteWarning(LocalizedString text)
		{
			this.task.WriteWarning(text);
		}

		public void WriteVerbose(LocalizedString text)
		{
			this.task.WriteVerbose(text);
		}

		private Task task;
	}
}
