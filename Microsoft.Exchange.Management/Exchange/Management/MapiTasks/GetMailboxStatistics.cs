using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Management.MapiTasks.Presentation;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Get", "MailboxStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxStatistics : GetStatisticsBase<GeneralMailboxOrMailUserIdParameter, Microsoft.Exchange.Data.Mapi.MailboxStatistics, Microsoft.Exchange.Management.MapiTasks.Presentation.MailboxStatistics>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeMoveHistory
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeMoveHistory"] ?? false);
			}
			set
			{
				base.Fields["IncludeMoveHistory"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeMoveReport
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeMoveReport"] ?? false);
			}
			set
			{
				base.Fields["IncludeMoveReport"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AuditLog")]
		public SwitchParameter AuditLog
		{
			get
			{
				return (SwitchParameter)(base.Fields["AuditLog"] ?? false);
			}
			set
			{
				base.Fields["AuditLog"] = value;
			}
		}

		[Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, ParameterSetName = "Database")]
		public StoreMailboxIdParameter StoreMailboxIdentity
		{
			get
			{
				return (StoreMailboxIdParameter)base.Fields["StoreMailboxIdentity"];
			}
			set
			{
				base.Fields["StoreMailboxIdentity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter NoADLookup
		{
			get
			{
				return (SwitchParameter)(base.Fields["NoADLookup"] ?? false);
			}
			set
			{
				base.Fields["NoADLookup"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeQuarantineDetails
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeQuarantineDetails"] ?? false);
			}
			set
			{
				base.Fields["IncludeQuarantineDetails"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Database")]
		[Parameter(Mandatory = false, ParameterSetName = "Server")]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				base.Fields["Filter"] = value;
			}
		}

		internal override MoveHistoryOption GetMoveHistoryOption()
		{
			if (this.IncludeMoveReport)
			{
				return MoveHistoryOption.IncludeMoveHistoryAndReport;
			}
			if (this.IncludeMoveHistory)
			{
				return MoveHistoryOption.IncludeMoveHistory;
			}
			return MoveHistoryOption.None;
		}

		internal override bool GetArchiveMailboxStatistics()
		{
			return this.Archive;
		}

		internal override bool GetAuditLogMailboxStatistics()
		{
			return this.AuditLog;
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (this.Filter != null)
				{
					QueryParser.ConvertValueFromStringDelegate convertDelegate = new QueryParser.ConvertValueFromStringDelegate(MonadFilter.ConvertValueFromString);
					QueryParser queryParser = new QueryParser(this.Filter, ObjectSchema.GetInstance<MailboxStatisticsSchema>(), QueryParser.Capabilities.All, new QueryParser.EvaluateVariableDelegate(base.GetVariableValue), convertDelegate);
					return queryParser.ParseTree;
				}
				return null;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			Microsoft.Exchange.Management.MapiTasks.Presentation.MailboxStatistics mailboxStatistics = (Microsoft.Exchange.Management.MapiTasks.Presentation.MailboxStatistics)((MapiObject)dataObject);
			mailboxStatistics.IncludeQuarantineDetails = this.IncludeQuarantineDetails;
			base.WriteResult(dataObject);
		}

		protected override StoreMailboxIdParameter StoreMailboxId
		{
			get
			{
				return this.StoreMailboxIdentity;
			}
		}

		protected override bool NoADLookupForMailboxStatistics
		{
			get
			{
				return this.NoADLookup;
			}
		}

		private const string ParameterIncludeMoveHistory = "IncludeMoveHistory";

		private const string ParameterIncludeMoveReport = "IncludeMoveReport";

		private const string ParameterFilter = "Filter";

		private const string ParameterStoreMailboxIdentity = "StoreMailboxIdentity";

		private const string ParameterNoAdLookup = "NoADLookup";

		private const string ParameterIncludeQuarantineDetails = "IncludeQuarantineDetails";
	}
}
