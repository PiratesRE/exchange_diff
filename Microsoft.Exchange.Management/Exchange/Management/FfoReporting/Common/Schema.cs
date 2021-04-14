using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ReportingTask;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	internal static class Schema
	{
		internal const int MinPageRange = 1;

		internal const int MaxPageRange = 1000;

		internal const int MinPageSizeRange = 1;

		internal const int MaxPageSizeRange = 5000;

		[Flags]
		public enum Actions
		{
			AddBccRecipient = 1,
			AddCcRecipient = 2,
			AddManagerAsRecipient = 4,
			AddToRecipient = 8,
			ApplyClassification = 16,
			ApplyHtmlDisclaimer = 32,
			DeleteMessage = 64,
			GenerateIncidentReport = 256,
			ModerateMessageByManager = 512,
			ModerateMessageByUser = 1024,
			NotifySender = 4096,
			PrependSubject = 8192,
			Quarantine = 16384,
			RedirectMessage = 32768,
			RejectMessage = 65536,
			RemoveMessageHeader = 131072,
			RequireTLS = 262144,
			RightsProtectMessage = 524288,
			RouteMessageUsingConnector = 1048576,
			SetAuditSeverityHigh = 2097152,
			SetAuditSeverityLow = 4194304,
			SetAuditSeverityMedium = 8388608,
			SetMessageHeader = 16777216,
			SetSpamConfidenceLevel = 33554432,
			StopRuleProcessing = 67108864
		}

		[Flags]
		public enum EventTypes
		{
			DLPActionHits = 1,
			DLPMessages = 2,
			DLPPolicyFalsePositive = 4,
			DLPPolicyHits = 8,
			DLPPolicyOverride = 16,
			DLPRuleHits = 32,
			GoodMail = 64,
			Malware = 128,
			SpamContentFiltered = 256,
			SpamEnvelopeBlock = 512,
			SpamIPBlock = 1024,
			TopMailUser = 2048,
			TopMalware = 4096,
			TopMalwareUser = 8192,
			TopSpamUser = 16384,
			TransportRuleActionHits = 32768,
			TransportRuleHits = 65536,
			TransportRuleMessages = 131072,
			SpamDBEBFilter = 262144
		}

		[Flags]
		public enum SummarizeByValues
		{
			Action = 1,
			DlpPolicy = 2,
			Domain = 4,
			EventType = 8,
			TransportRule = 16
		}

		[Flags]
		internal enum DirectionValues
		{
			Inbound = 1,
			Outbound = 2
		}

		internal enum AggregateByValues
		{
			Hour,
			Day,
			Summary
		}

		[Flags]
		internal enum DeliveryStatusValues
		{
			None = 1,
			Delivered = 2,
			Pending = 4,
			Failed = 8,
			Expanded = 16
		}

		[Flags]
		public enum Source
		{
			EXO = 1,
			SPO = 2,
			ODB = 4
		}

		internal static class DalTypes
		{
			internal const string HygieneDataAssemblyName = "Microsoft.Exchange.Hygiene.Data";

			internal const string DefaultDataSessionTypeName = "Microsoft.Exchange.Hygiene.Data.MessageTrace.MessageTraceSession";

			internal const string AsyncQueueDataSessionTypeName = "Microsoft.Exchange.Hygiene.Data.AsyncQueue.AsyncQueueSession";

			internal const string DefaultDataSessionMethodName = "FindReportObject";

			internal const string FindMigrationReportMethodName = "FindMigrationReport";

			internal const string SchemaTypeName = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.Schema, Microsoft.Exchange.Hygiene.Data";

			internal const string TrafficReport = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.TrafficReport, Microsoft.Exchange.Hygiene.Data";

			internal const string TopTrafficReport = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.TopTrafficReport, Microsoft.Exchange.Hygiene.Data";

			internal const string PolicyTrafficReport = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyTrafficReport, Microsoft.Exchange.Hygiene.Data";

			internal const string MessageDetailReport = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.MessageDetailReport, Microsoft.Exchange.Hygiene.Data";

			internal const string MessageTrace = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.MessageTrace, Microsoft.Exchange.Hygiene.Data";

			internal const string MessageTraceDetail = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.MessageTraceDetail, Microsoft.Exchange.Hygiene.Data";

			internal const string DLPMessageDetail = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.DLPMessageDetail, Microsoft.Exchange.Hygiene.Data";

			internal const string DLPReportDetail = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.DLPUnifiedDetail, Microsoft.Exchange.Hygiene.Data";

			internal const string DLPPolicyReport = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyTrafficReport, Microsoft.Exchange.Hygiene.Data";

			internal const string MalwareMessageDetail = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.MalwareMessageDetail, Microsoft.Exchange.Hygiene.Data";

			internal const string PolicyMessageDetail = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.PolicyMessageDetail, Microsoft.Exchange.Hygiene.Data";

			internal const string SpamMessageDetail = "Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.SpamMessageDetail, Microsoft.Exchange.Hygiene.Data";

			internal const string AsyncQueueReport = "Microsoft.Exchange.Hygiene.Data.AsyncQueue.AsyncQueueReport, Microsoft.Exchange.Hygiene.Data";
		}

		internal static class Utilities
		{
			internal static int ToQueryDate(DateTime date)
			{
				return int.Parse(string.Format("{0}{1:D2}{2:D2}", date.Year, date.Month, date.Day));
			}

			internal static int ToQueryHour(DateTime date)
			{
				return date.Hour;
			}

			internal static DateTime FromQueryDate(int date, int hour)
			{
				if (date == 0 && hour == 0)
				{
					return DateTime.MinValue;
				}
				return new DateTime(date / 10000, date % 10000 / 100, date % 100, hour, 0, 0);
			}

			internal static PropertyDefinition GetSchemaPropertyDefinition(string name)
			{
				Type type = Type.GetType("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.Schema, Microsoft.Exchange.Hygiene.Data");
				FieldInfo field = type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				return (PropertyDefinition)field.GetValue(null);
			}

			internal static DataTable CreateDataTable(IEnumerable values)
			{
				DataTable dataTable = new DataTable("TrafficTypeFilterTableType");
				dataTable.Columns.Add("TrafficTypeFilter");
				foreach (object obj in values)
				{
					dataTable.Rows.Add(new object[]
					{
						obj.ToString()
					});
				}
				return dataTable;
			}

			internal static DataTable CreateDataTable(Enum values)
			{
				DataTable dataTable = new DataTable("TrafficTypeFilterTableType");
				dataTable.Columns.Add("TrafficTypeFilter");
				foreach (string text in values.ToString().Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					dataTable.Rows.Add(new object[]
					{
						text.Trim()
					});
				}
				return dataTable;
			}

			internal static IList<Tuple<PropertyInfo, TAttribute>> GetProperties<TAttribute>(Type type)
			{
				List<Tuple<PropertyInfo, TAttribute>> list = new List<Tuple<PropertyInfo, TAttribute>>();
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				foreach (PropertyInfo propertyInfo in properties)
				{
					object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(TAttribute), true);
					for (int j = 0; j < customAttributes.Length; j++)
					{
						TAttribute item = (TAttribute)((object)customAttributes[j]);
						list.Add(Tuple.Create<PropertyInfo, TAttribute>(propertyInfo, item));
					}
				}
				return list;
			}

			internal static object Invoke(MethodInfo method, object instance, params object[] args)
			{
				try
				{
					if (method != null)
					{
						return method.Invoke(instance, args);
					}
				}
				catch (TargetInvocationException ex)
				{
					throw ex.GetBaseException();
				}
				return null;
			}

			internal static bool ValidateEmailAddress(string address, out bool isWildCard)
			{
				isWildCard = false;
				if (string.IsNullOrEmpty(address))
				{
					return false;
				}
				if (address[0] == '@')
				{
					address = string.Format("*{0}", address);
				}
				SmtpAddress smtpAddress = new SmtpAddress(address);
				bool isValidAddress = smtpAddress.IsValidAddress;
				isWildCard = (isValidAddress && smtpAddress.Local.Contains('*'));
				return isValidAddress;
			}

			internal static string GenerateDetailedError(Exception e)
			{
				StringBuilder stringBuilder = new StringBuilder("Exception Details: \n");
				while (e != null)
				{
					Type type = e.GetType();
					if (type.IsGenericType && typeof(FaultException<>) == type.GetGenericTypeDefinition())
					{
						PropertyInfo property = type.GetProperty("Detail", BindingFlags.Instance | BindingFlags.Public);
						if (property != null)
						{
							object value = property.GetValue(e, null);
							if (value != null)
							{
								stringBuilder.AppendFormat("Fault Detail: {0}\n", value.ToString());
							}
						}
					}
					stringBuilder.AppendFormat("Message: {0}\n", e.Message);
					stringBuilder.AppendFormat("Call Stack: {0}\n", e.StackTrace);
					stringBuilder.AppendFormat("Additional Data:\n", new object[0]);
					foreach (object obj in e.Data)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						stringBuilder.AppendFormat("Key = {0}, Value = {1}", dictionaryEntry.Key, dictionaryEntry.Value);
					}
					e = e.InnerException;
				}
				return stringBuilder.ToString();
			}

			internal static void ValidateParameters(Task task, Func<IConfigDataProvider> getConfigDataProviderFunction, IEnumerable<CmdletValidator.ValidatorTypes> validationTypes)
			{
				foreach (Tuple<PropertyInfo, CmdletValidator> tuple in Schema.Utilities.GetProperties<CmdletValidator>(task.GetType()))
				{
					if (validationTypes.Contains(tuple.Item2.ValidatorType))
					{
						tuple.Item2.Validate(new CmdletValidator.CmdletValidatorArgs(tuple.Item1, task, getConfigDataProviderFunction));
					}
				}
			}

			internal static bool HasDlpRole(Task task)
			{
				return !DatacenterRegistry.IsForefrontForOffice() && (task.ExchangeRunspaceConfig == null || task.ExchangeRunspaceConfig.HasRoleOfType(RoleType.DataLossPrevention) || task.ExchangeRunspaceConfig.HasRoleOfType(RoleType.PersonallyIdentifiableInformation));
			}

			internal static IList<string> GetEventTypes(Task task)
			{
				List<string> list = Enum.GetNames(typeof(Schema.EventTypes)).ToList<string>();
				if (!Schema.Utilities.HasDlpRole(task))
				{
					Schema.Utilities.RemoveDlpEventTypes(list);
				}
				return list;
			}

			internal static IList<string> GetSources(Task task)
			{
				return Enum.GetNames(typeof(Schema.Source)).ToList<string>();
			}

			internal static void RemoveDlpEventTypes(List<string> eventTypes)
			{
				HashSet<string> dlpValues = new HashSet<string>
				{
					Schema.EventTypes.DLPActionHits.ToString().ToLower(),
					Schema.EventTypes.DLPMessages.ToString().ToLower(),
					Schema.EventTypes.DLPPolicyFalsePositive.ToString().ToLower(),
					Schema.EventTypes.DLPPolicyHits.ToString().ToLower(),
					Schema.EventTypes.DLPPolicyOverride.ToString().ToLower(),
					Schema.EventTypes.DLPRuleHits.ToString().ToLower()
				};
				eventTypes.RemoveAll((string value) => dlpValues.Contains(value.ToLower()));
			}

			internal static void CheckDates(DateTime? startDate, DateTime? endDate, Schema.Utilities.NotifyNeedDefaultDatesDelegate needDefaultDateAction, Schema.Utilities.ValidateDatesDelegate validateDatesAction)
			{
				if (startDate != null && endDate != null)
				{
					if (validateDatesAction != null)
					{
						validateDatesAction(startDate.Value, endDate.Value);
						return;
					}
				}
				else
				{
					if (startDate != null || endDate != null)
					{
						LocalizedString message = (startDate == null) ? Strings.RequiredStartDateParameter : Strings.RequiredEndDateParameter;
						throw new Microsoft.Exchange.Management.ReportingTask.InvalidExpressionException(message);
					}
					if (needDefaultDateAction != null)
					{
						needDefaultDateAction();
						return;
					}
				}
			}

			internal static void VerifyDateRange(DateTime startDate, DateTime endDate)
			{
				if (endDate < startDate)
				{
					throw new Microsoft.Exchange.Management.ReportingTask.InvalidExpressionException(Strings.InvalidEndDate);
				}
			}

			internal static string[] Split(Enum enumeration)
			{
				return enumeration.ToString().Split(new char[]
				{
					',',
					' '
				}, StringSplitOptions.RemoveEmptyEntries);
			}

			internal static void AddRange<T>(IList<T> property, IEnumerable<T> values)
			{
				foreach (T item in values)
				{
					property.Add(item);
				}
			}

			internal static void Redact(object targetOfRedaction)
			{
				Schema.Utilities.Redact(targetOfRedaction, Schema.Utilities.GetProperties<RedactAttribute>(targetOfRedaction.GetType()));
			}

			internal static void Redact(object targetOfRedaction, IList<Tuple<PropertyInfo, RedactAttribute>> redactionAttributes)
			{
				foreach (Tuple<PropertyInfo, RedactAttribute> tuple in redactionAttributes)
				{
					tuple.Item2.Redact(tuple.Item1, targetOfRedaction);
				}
			}

			internal static string GetOrganizationName(OrganizationIdParameter organization)
			{
				string result = string.Empty;
				if (organization != null)
				{
					result = ((organization.InternalADObjectId != null) ? organization.InternalADObjectId.Name : organization.RawIdentity);
				}
				return result;
			}

			internal delegate void NotifyNeedDefaultDatesDelegate();

			internal delegate void ValidateDatesDelegate(DateTime startDate, DateTime endDate);
		}
	}
}
