using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationJobReportingCursor
	{
		public MigrationJobReportingCursor(ReportingStageEnum stage = ReportingStageEnum.Unknown)
		{
			this.reportingStage = stage;
			this.MigrationSuccessCount = new MigrationObjectsCount(new int?(0));
			this.MigrationErrorCount = new MigrationObjectsCount(new int?(0));
			this.PartialMigrationCounts = 0;
		}

		public MigrationJobReportingCursor(ReportingStageEnum stage, string cursorPosition, MigrationReportId successReportId, MigrationReportId failureReportId) : this(stage)
		{
			MigrationUtil.ThrowOnNullArgument(successReportId, "successReportId");
			MigrationUtil.ThrowOnNullArgument(failureReportId, "failureReportId");
			this.currentCursorPosition = cursorPosition;
			this.successReportId = successReportId;
			this.failureReportId = failureReportId;
		}

		public static string MoacHelpUrlFormat
		{
			get
			{
				return "http://go.microsoft.com/fwlink/?LinkId=183883&clcid=0x{0:X4}";
			}
		}

		public MigrationReportId SuccessReportId
		{
			get
			{
				return this.successReportId;
			}
		}

		public MigrationReportId FailureReportId
		{
			get
			{
				return this.failureReportId;
			}
		}

		public ReportingStageEnum ReportingStage
		{
			get
			{
				return this.reportingStage;
			}
		}

		public string CurrentCursorPosition
		{
			get
			{
				return this.currentCursorPosition;
			}
		}

		public MigrationObjectsCount MigrationSuccessCount { get; set; }

		public MigrationObjectsCount MigrationErrorCount { get; set; }

		public int PartialMigrationCounts { get; set; }

		public TimeSpan? SyncDuration { get; set; }

		public bool HasErrors
		{
			get
			{
				return this.MigrationErrorCount.GetTotal() > 0;
			}
		}

		public bool AreSuccessfulMigrationsPresent
		{
			get
			{
				return this.MigrationSuccessCount.GetTotal() > 0;
			}
		}

		public static string GetLicensingHtml(string moacHelpUrl)
		{
			if (string.IsNullOrEmpty(moacHelpUrl))
			{
				return string.Empty;
			}
			MigrationUtil.ThrowOnNullOrEmptyArgument(moacHelpUrl, "moacHelpUrl");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td colspan=\"2\" class=\"spacer\">&nbsp;</td>");
			stringBuilder.AppendLine("</tr>");
			stringBuilder.AppendLine("<tr>");
			stringBuilder.AppendLine("<td width=\"16px\" valign=\"top\"><img width=\"16\" height=\"16\" src=\"cid:Information\" /></td>");
			stringBuilder.AppendLine(string.Format(CultureInfo.CurrentCulture, "<td>{0}</td>", new object[]
			{
				Strings.MoacWarningMessage(moacHelpUrl)
			}));
			stringBuilder.AppendLine("</tr>");
			return stringBuilder.ToString();
		}

		public static MigrationJobReportingCursor Deserialize(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}
			MigrationJobReportingCursor result;
			try
			{
				PersistableDictionary persistableDictionary = PersistableDictionary.Create(s);
				ReportingStageEnum stage = (ReportingStageEnum)persistableDictionary["ReportingStage"];
				result = new MigrationJobReportingCursor(stage, (string)persistableDictionary["CurrentCursorPosition"], new MigrationReportId((string)persistableDictionary["SuccessReportId"]), new MigrationReportId((string)persistableDictionary["FailureReportId"]))
				{
					MigrationSuccessCount = MigrationObjectsCount.FromValue((string)persistableDictionary["SuccessCount"]),
					MigrationErrorCount = MigrationObjectsCount.FromValue((string)persistableDictionary["FailureCount"]),
					PartialMigrationCounts = (int)persistableDictionary["PartialMigrationCounts"],
					SyncDuration = persistableDictionary.Get<TimeSpan?>("SyncDuration", null)
				};
			}
			catch (XmlException exception)
			{
				MigrationLogger.Log(MigrationEventType.Error, exception, "Xml Exception occured trying to parse deserialize MigrationJobReportingCursor. Data was {0}", new object[]
				{
					s
				});
				result = null;
			}
			return result;
		}

		public string Serialize()
		{
			PersistableDictionary persistableDictionary = new PersistableDictionary();
			persistableDictionary.Add("ReportingStage", (int)this.ReportingStage);
			persistableDictionary.Add("CurrentCursorPosition", this.CurrentCursorPosition);
			persistableDictionary.Add("SuccessReportId", this.SuccessReportId.ToString());
			persistableDictionary.Add("FailureReportId", this.FailureReportId.ToString());
			persistableDictionary.Add("FailureCount", this.MigrationErrorCount.ToValue());
			persistableDictionary.Add("SuccessCount", this.MigrationSuccessCount.ToValue());
			persistableDictionary.Add("PartialMigrationCounts", this.PartialMigrationCounts);
			persistableDictionary.Set<TimeSpan?>("SyncDuration", this.SyncDuration);
			return persistableDictionary.Serialize();
		}

		public MigrationJobReportingCursor GetNextCursor(ReportingStageEnum stage, string cursorPosition)
		{
			return new MigrationJobReportingCursor(stage, cursorPosition, this.SuccessReportId, this.FailureReportId)
			{
				MigrationErrorCount = this.MigrationErrorCount,
				MigrationSuccessCount = this.MigrationSuccessCount,
				PartialMigrationCounts = this.PartialMigrationCounts,
				SyncDuration = this.SyncDuration
			};
		}

		private const string ReportingStageKey = "ReportingStage";

		private const string CurrentCursorPositionKey = "CurrentCursorPosition";

		private const string SuccessReportIdKey = "SuccessReportId";

		private const string FailureReportIdKey = "FailureReportId";

		private const string PartialMigrationCountsKey = "PartialMigrationCounts";

		private const string FailureCountKey = "FailureCount";

		private const string SuccessCountKey = "SuccessCount";

		private const string SyncDurationKey = "SyncDuration";

		internal static readonly HashSet<MigrationUserStatus> FailureStatuses = new HashSet<MigrationUserStatus>(MigrationJobItem.ErrorStatuses);

		private readonly ReportingStageEnum reportingStage;

		private readonly string currentCursorPosition;

		private readonly MigrationReportId successReportId;

		private readonly MigrationReportId failureReportId;
	}
}
