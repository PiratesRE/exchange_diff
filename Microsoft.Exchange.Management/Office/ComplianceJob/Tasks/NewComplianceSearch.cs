using System;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	[Cmdlet("New", "ComplianceSearch", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class NewComplianceSearch : NewComplianceJob<ComplianceSearch>
	{
		[Parameter(Mandatory = false)]
		public CultureInfo Language
		{
			get
			{
				return this.objectToSave.Language;
			}
			set
			{
				this.objectToSave.Language = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] StatusMailRecipients
		{
			get
			{
				return this.objectToSave.StatusMailRecipients.ToArray();
			}
			set
			{
				this.objectToSave.StatusMailRecipients = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ComplianceJobLogLevel LogLevel
		{
			get
			{
				return this.objectToSave.LogLevel;
			}
			set
			{
				this.objectToSave.LogLevel = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IncludeUnindexedItems
		{
			get
			{
				return this.objectToSave.IncludeUnindexedItems;
			}
			set
			{
				this.objectToSave.IncludeUnindexedItems = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string KeywordQuery
		{
			get
			{
				return this.objectToSave.KeywordQuery;
			}
			set
			{
				this.objectToSave.KeywordQuery = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartDate
		{
			get
			{
				return this.objectToSave.StartDate;
			}
			set
			{
				this.objectToSave.StartDate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? EndDate
		{
			get
			{
				return this.objectToSave.EndDate;
			}
			set
			{
				this.objectToSave.EndDate = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (base.ExchangeRunspaceConfig == null)
			{
				base.ThrowTerminatingError(new ComplianceJobTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
			}
			byte[] bytes = base.ExchangeRunspaceConfig.OrganizationId.GetBytes(Encoding.UTF8);
			OrganizationId organizationId;
			if (OrganizationId.TryCreateFromBytes(bytes, Encoding.UTF8, out organizationId))
			{
				TaskLogger.LogEnter();
			}
			return new ComplianceJobProvider(base.ExchangeRunspaceConfig.OrganizationId);
		}
	}
}
