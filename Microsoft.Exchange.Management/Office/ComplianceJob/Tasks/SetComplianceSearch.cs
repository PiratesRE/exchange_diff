using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	[Cmdlet("Set", "ComplianceSearch", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetComplianceSearch : SetComplianceJob<ComplianceSearch>
	{
		[Parameter(Mandatory = false)]
		public string KeywordQuery
		{
			get
			{
				return (string)base.Fields["KeywordQuery"];
			}
			set
			{
				base.Fields["KeywordQuery"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? StartDate
		{
			get
			{
				return (DateTime?)base.Fields["StartDate"];
			}
			set
			{
				base.Fields["StartDate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? EndDate
		{
			get
			{
				return (DateTime?)base.Fields["EndDate"];
			}
			set
			{
				base.Fields["EndDate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)base.Fields["Language"];
			}
			set
			{
				base.Fields["Language"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IncludeUnindexedItems
		{
			get
			{
				return (bool)base.Fields["IncludeUnindexedItems"];
			}
			set
			{
				base.Fields["IncludeUnindexedItems"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.SetComplianceSearchConfirmation(this.Identity.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ComplianceSearch complianceSearch = (ComplianceSearch)base.PrepareDataObject();
			if (base.Fields.IsModified("StartDate"))
			{
				complianceSearch.StartDate = new DateTime?(this.StartDate.Value);
			}
			if (base.Fields.IsModified("EndDate"))
			{
				complianceSearch.EndDate = new DateTime?(this.EndDate.Value);
			}
			if (base.Fields.IsModified("KeywordQuery"))
			{
				complianceSearch.KeywordQuery = this.KeywordQuery;
			}
			if (base.Fields.IsModified("Language"))
			{
				complianceSearch.Language = this.Language;
			}
			if (base.Fields.IsModified("IncludeUnindexedItems"))
			{
				complianceSearch.IncludeUnindexedItems = this.IncludeUnindexedItems;
			}
			return complianceSearch;
		}

		private const string ParameterKeywordQuery = "KeywordQuery";

		private const string ParameterLanguage = "Language";

		private const string ParameterStatusMailRecipients = "StatusMailRecipients";

		private const string ParameterLogLevel = "LogLevel";

		private const string ParameterIncludeUnindexedItems = "IncludeUnindexedItems";

		private const string ParameterStartDate = "StartDate";

		private const string ParameterEndDate = "EndDate";
	}
}
