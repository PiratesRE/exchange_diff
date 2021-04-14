using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalendarDiagnosticAnalyzer
	{
		internal CalendarDiagnosticAnalyzer(ExchangePrincipal principal, AnalysisDetailLevel detailLevel)
		{
			this.principal = principal;
			this.detailLevel = detailLevel;
		}

		internal IEnumerable<CalendarLogAnalysis> AnalyzeLogs(IEnumerable<CalendarLog> calendarLogs)
		{
			CalendarLog calendarLog = calendarLogs.FirstOrDefault<CalendarLog>();
			IEnumerable<CalendarLogAnalysis> enumerable;
			if (calendarLog != null && calendarLog.IsFileLink)
			{
				enumerable = this.LoadMsgLogs(calendarLogs);
			}
			else
			{
				enumerable = this.LoadMailboxLogs(calendarLogs);
			}
			string[] array = null;
			if (!this.VerifyItemCohesion(enumerable, out array))
			{
				throw new InvalidLogCollectionException();
			}
			enumerable.OrderBy((CalendarLogAnalysis f) => f, CalendarLogAnalysis.GetComparer());
			return this.PerformAnalysis(enumerable);
		}

		private bool VerifyItemCohesion(IEnumerable<CalendarLogAnalysis> logs, out string[] ids)
		{
			List<string> list = new List<string>();
			foreach (CalendarLogAnalysis calendarLogAnalysis in logs)
			{
				if (!list.Contains(calendarLogAnalysis.InternalIdentity.CleanGlobalObjectId))
				{
					list.Add(calendarLogAnalysis.InternalIdentity.CleanGlobalObjectId);
				}
			}
			ids = list.ToArray();
			return list.Count<string>() == 1;
		}

		private IEnumerable<CalendarLogAnalysis> PerformAnalysis(IEnumerable<CalendarLogAnalysis> logs)
		{
			IEnumerable<AnalysisRule> analysisRules = AnalysisRule.GetAnalysisRules();
			LinkedList<CalendarLogAnalysis> linkedList = new LinkedList<CalendarLogAnalysis>(logs);
			foreach (AnalysisRule analysisRule in analysisRules)
			{
				analysisRule.Analyze(linkedList);
			}
			return linkedList.ToList<CalendarLogAnalysis>();
		}

		private IEnumerable<CalendarLogAnalysis> LoadMailboxLogs(IEnumerable<CalendarLog> logs)
		{
			if (this.principal == null)
			{
				throw new InvalidOperationException("The Analyzer was not provided with session objects during construction and cannot connect to the specified mailbox");
			}
			List<CalendarLogAnalysis> list = new List<CalendarLogAnalysis>();
			using (MailboxSession mailboxSession = StoreTasksHelper.OpenMailboxSession(this.principal, "Get-CalendarDiagnosticLogs"))
			{
				foreach (CalendarLog calendarLog in logs)
				{
					CalendarLogId calendarLogId = calendarLog.Identity as CalendarLogId;
					if (calendarLogId != null)
					{
						UriHandler uriHandler = new UriHandler(calendarLogId.Uri);
						if (uriHandler.IsValidLink && !uriHandler.IsFileLink)
						{
							CalendarLogAnalysis calendarLogAnalysis = this.LoadFromMailbox(calendarLogId, uriHandler, mailboxSession);
							if (calendarLogAnalysis != null)
							{
								list.Add(calendarLogAnalysis);
							}
						}
					}
				}
			}
			return list;
		}

		private IEnumerable<CalendarLogAnalysis> LoadMsgLogs(IEnumerable<CalendarLog> logs)
		{
			List<CalendarLogAnalysis> list = new List<CalendarLogAnalysis>();
			foreach (CalendarLog calendarLog in logs)
			{
				CalendarLogId calendarLogId = calendarLog.Identity as CalendarLogId;
				if (calendarLogId != null)
				{
					UriHandler uriHandler = new UriHandler(calendarLogId.Uri);
					if (uriHandler.IsValidLink && uriHandler.IsFileLink)
					{
						CalendarLogAnalysis calendarLogAnalysis = this.LoadFromFile(calendarLogId, uriHandler);
						if (calendarLogAnalysis != null)
						{
							list.Add(calendarLogAnalysis);
						}
					}
				}
			}
			return list;
		}

		private CalendarLogAnalysis LoadFromFile(CalendarLogId id, UriHandler handler)
		{
			FileInfo fileInfo = new FileInfo(handler.Uri.LocalPath);
			if (fileInfo.Exists)
			{
				using (MessageItem messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties))
				{
					using (FileStream fileStream = fileInfo.OpenRead())
					{
						ItemConversion.ConvertMsgStorageToItem(fileStream, messageItem, new InboundConversionOptions(new EmptyRecipientCache(), null));
						IEnumerable<PropertyDefinition> displayProperties = AnalysisDetailLevels.GetDisplayProperties(this.detailLevel);
						return new CalendarLogAnalysis(id, messageItem, displayProperties);
					}
				}
			}
			throw new ArgumentException("Item argument cannot be resolved.", "item");
		}

		private CalendarLogAnalysis LoadFromMailbox(CalendarLogId id, UriHandler handler, MailboxSession session)
		{
			StoreObjectId storeId = StoreObjectId.Deserialize(handler.Id);
			CalendarLogAnalysis result;
			using (Item item = Item.Bind(session, storeId))
			{
				IEnumerable<PropertyDefinition> displayProperties = AnalysisDetailLevels.GetDisplayProperties(this.detailLevel);
				item.Load(displayProperties.ToArray<PropertyDefinition>());
				result = new CalendarLogAnalysis(id, item, displayProperties);
			}
			return result;
		}

		private ExchangePrincipal principal;

		private AnalysisDetailLevel detailLevel;
	}
}
