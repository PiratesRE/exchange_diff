using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "CalendarDiagnosticAnalysis", DefaultParameterSetName = "DefaultSet")]
	public sealed class GetCalendarDiagnosticAnalysis : Task
	{
		[Parameter(Mandatory = true, ParameterSetName = "DefaultSet")]
		public CalendarLog[] CalendarLogs
		{
			get
			{
				return ((CalendarLog[])base.Fields["CalendarLogKey"]) ?? new CalendarLog[0];
			}
			set
			{
				base.Fields["CalendarLogKey"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "LocationSet")]
		public string[] LogLocation
		{
			get
			{
				return (string[])base.Fields["CalendarLogLocationKey"];
			}
			set
			{
				base.Fields["CalendarLogLocationKey"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string GlobalObjectId
		{
			get
			{
				return (string)base.Fields["ItemIDKey"];
			}
			set
			{
				base.Fields["ItemIDKey"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AnalysisDetailLevel DetailLevel
		{
			get
			{
				return (AnalysisDetailLevel)(base.Fields["DetailLevelKey"] ?? AnalysisDetailLevel.Basic);
			}
			set
			{
				base.Fields["DetailLevelKey"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OutputType OutputAs
		{
			get
			{
				return (OutputType)(base.Fields["OutputTypeKey"] ?? OutputType.CSV);
			}
			set
			{
				base.Fields["OutputTypeKey"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.LogLocation != null && this.LogLocation.Length > 0)
			{
				List<CalendarLog> list = new List<CalendarLog>();
				foreach (string identity in this.LogLocation)
				{
					list.AddRange(CalendarLog.Parse(identity));
				}
				this.CalendarLogs = list.ToArray();
			}
			else if (this.CalendarLogs.Count<CalendarLog>() == 0)
			{
				base.WriteError(new InvalidADObjectOperationException(Strings.CalendarLogsNotFound), ErrorCategory.InvalidData, null);
			}
			foreach (CalendarLog calendarLog in this.CalendarLogs)
			{
				if (calendarLog.IsFileLink != this.CalendarLogs.First<CalendarLog>().IsFileLink)
				{
					base.WriteError(new InvalidADObjectOperationException(Strings.CalendarAnalysisMixedModeNotSupported), ErrorCategory.InvalidData, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			CalendarLog calendarLog = this.CalendarLogs.FirstOrDefault<CalendarLog>();
			if (calendarLog == null)
			{
				return;
			}
			CalendarDiagnosticAnalyzer calendarDiagnosticAnalyzer;
			if (calendarLog.IsFileLink)
			{
				calendarDiagnosticAnalyzer = new CalendarDiagnosticAnalyzer(null, this.DetailLevel);
			}
			else
			{
				CalendarLogId calendarLogId = calendarLog.Identity as CalendarLogId;
				UriHandler uriHandler = new UriHandler(calendarLogId.Uri);
				string host = uriHandler.Host;
				SmtpAddress address = new SmtpAddress(uriHandler.UserName, host);
				if (!address.IsValidAddress)
				{
					base.WriteError(new InvalidADObjectOperationException(Strings.Error_InvalidAddress((string)address)), ErrorCategory.InvalidData, null);
				}
				ExchangePrincipal principal = ExchangePrincipal.FromProxyAddress(ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(host), (string)address, RemotingOptions.AllowCrossSite);
				calendarDiagnosticAnalyzer = new CalendarDiagnosticAnalyzer(principal, this.DetailLevel);
			}
			try
			{
				CalendarLog[] array;
				if (!string.IsNullOrEmpty(this.GlobalObjectId))
				{
					array = (from f in this.CalendarLogs
					where f.CleanGlobalObjectId == this.GlobalObjectId
					select f).ToArray<CalendarLog>();
				}
				else
				{
					array = this.CalendarLogs;
				}
				CalendarLog[] calendarLogs = array;
				IEnumerable<CalendarLogAnalysis> logs = calendarDiagnosticAnalyzer.AnalyzeLogs(calendarLogs);
				base.WriteObject(CalendarLogAnalysisSerializer.Serialize(logs, this.OutputAs, this.DetailLevel, true));
			}
			catch (InvalidLogCollectionException)
			{
				base.WriteError(new InvalidADObjectOperationException(Strings.Error_MultipleItemsFound), ErrorCategory.InvalidData, null);
			}
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is StorageTransientException || exception is StoragePermanentException || exception is ObjectNotFoundException || exception is IOException || exception is UnauthorizedAccessException;
		}

		private const string DetailLevelKey = "DetailLevelKey";

		private const string CalendarLogKey = "CalendarLogKey";

		private const string CalendarLogLocationKey = "CalendarLogLocationKey";

		private const string OutputTypeKey = "OutputTypeKey";

		private const string ItemIdKey = "ItemIDKey";

		private const string DefaultParameterSet = "DefaultSet";

		private const string LocationParameterSet = "LocationSet";
	}
}
