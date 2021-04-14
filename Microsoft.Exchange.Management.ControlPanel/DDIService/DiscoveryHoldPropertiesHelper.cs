using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class DiscoveryHoldPropertiesHelper
	{
		public static void GetObjectForNewPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			dataRow["StartDate"] = ExDateTime.Now.ToUserDateTimeGeneralFormatString();
			dataRow["EndDate"] = ExDateTime.Now.AddDays(1.0).ToUserDateTimeGeneralFormatString();
		}

		public static void GetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			if (dataRow["StartDate"] != DBNull.Value)
			{
				ExDateTime? exDateTime = (ExDateTime?)dataRow["StartDate"];
				dataRow["StartDateEnabled"] = (exDateTime != null);
				dataRow["SearchStartDate"] = ((exDateTime != null) ? exDateTime.Value.ToUserDateTimeGeneralFormatString() : ExDateTime.Now.ToUserDateTimeGeneralFormatString());
			}
			else
			{
				dataRow["StartDateEnabled"] = false;
				dataRow["SearchStartDate"] = ExDateTime.Now.ToUserDateTimeGeneralFormatString();
			}
			if (dataRow["EndDate"] != DBNull.Value)
			{
				ExDateTime? exDateTime2 = (ExDateTime?)dataRow["EndDate"];
				dataRow["EndDateEnabled"] = (exDateTime2 != null);
				dataRow["SearchEndDate"] = ((exDateTime2 != null) ? exDateTime2.Value.ToUserDateTimeGeneralFormatString() : ExDateTime.Now.AddDays(1.0).ToUserExDateTime().ToUserDateTimeGeneralFormatString());
			}
			else
			{
				dataRow["EndDateEnabled"] = false;
				dataRow["SearchEndDate"] = ExDateTime.Now.AddDays(1.0).ToUserExDateTime().Date.ToUserDateTimeGeneralFormatString();
			}
			if (dataRow["ItemHoldPeriod"] != DBNull.Value)
			{
				Unlimited<EnhancedTimeSpan> unlimited = (Unlimited<EnhancedTimeSpan>)dataRow["ItemHoldPeriod"];
				dataRow["HoldIndefinitely"] = unlimited.IsUnlimited;
				dataRow["ItemHoldPeriodDays"] = (unlimited.IsUnlimited ? null : unlimited.Value.Days.ToString());
			}
			DiscoveryHoldPropertiesHelper.GetValuesForListRow(dataRow);
		}

		public static void NewObjectPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			List<string> list = new List<string>();
			if (!dataRow["SearchAllMailboxes"].IsNullValue() && dataRow["SearchAllMailboxes"].IsTrue())
			{
				dataRow["SourceMailboxes"] = null;
				dataRow["AllSourceMailboxes"] = true;
			}
			if (!dataRow["SearchAllPublicFolders"].IsNullValue() && dataRow["SearchAllPublicFolders"].IsTrue())
			{
				dataRow["PublicFolderSources"] = null;
				dataRow["AllPublicFolderSources"] = true;
			}
			if (!dataRow["HoldIndefinitely"].IsNullValue() && dataRow["HoldIndefinitely"].IsTrue())
			{
				dataRow["ItemHoldPeriod"] = Unlimited<EnhancedTimeSpan>.UnlimitedValue;
				list.Add("ItemHoldPeriod");
			}
			if (!dataRow["SearchContent"].IsNullValue())
			{
				bool flag = (bool)dataRow["SearchContent"];
				if (flag)
				{
					if (!string.IsNullOrEmpty((string)dataRow["SenderList"]))
					{
						dataRow["Senders"] = ((string)dataRow["SenderList"]).ToArrayOfStrings();
						list.Add("Senders");
					}
					if (!string.IsNullOrEmpty((string)dataRow["RecipientList"]))
					{
						dataRow["Recipients"] = ((string)dataRow["RecipientList"]).ToArrayOfStrings();
						list.Add("Recipients");
					}
					if (!string.IsNullOrEmpty((string)dataRow["MessageTypeList"]))
					{
						dataRow["MessageTypes"] = ((string)dataRow["MessageTypeList"]).ToArrayOfStrings();
						list.Add("MessageTypes");
					}
				}
			}
			store.SetModifiedColumns(list);
		}

		public static void SetObjectPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			List<string> list = new List<string>();
			if (!dataRow["SearchAllMailboxes"].IsNullValue() && dataRow["SearchAllMailboxes"].IsTrue())
			{
				dataRow["SourceMailboxes"] = null;
				list.Add("SourceMailboxes");
			}
			if (!dataRow["SearchAllPublicFolders"].IsNullValue() && dataRow["SearchAllPublicFolders"].IsTrue())
			{
				dataRow["PublicFolderSources"] = null;
				list.Add("PublicFolderSources");
				dataRow["AllPublicFolderSources"] = true;
				list.Add("AllPublicFolderSources");
			}
			if (!dataRow["HoldIndefinitely"].IsNullValue() && dataRow["HoldIndefinitely"].IsTrue())
			{
				dataRow["ItemHoldPeriod"] = Unlimited<EnhancedTimeSpan>.UnlimitedValue;
				list.Add("ItemHoldPeriod");
			}
			else if (!dataRow["ItemHoldPeriodDays"].IsNullValue())
			{
				dataRow["ItemHoldPeriod"] = Unlimited<EnhancedTimeSpan>.Parse((string)dataRow["ItemHoldPeriodDays"]);
				list.Add("ItemHoldPeriod");
			}
			if (!dataRow["StartDateEnabled"].IsNullValue())
			{
				if (dataRow["StartDateEnabled"].IsTrue())
				{
					DiscoveryHoldPropertiesHelper.SetDate(dataRow, list, "StartDate", (string)dataRow["SearchStartDate"]);
				}
				else
				{
					dataRow["StartDate"] = null;
					list.Add("StartDate");
				}
			}
			else if (!dataRow["SearchStartDate"].IsNullValue())
			{
				DiscoveryHoldPropertiesHelper.SetDate(dataRow, list, "StartDate", (string)dataRow["SearchStartDate"]);
			}
			if (!dataRow["EndDateEnabled"].IsNullValue())
			{
				if (dataRow["EndDateEnabled"].IsTrue())
				{
					DiscoveryHoldPropertiesHelper.SetDate(dataRow, list, "EndDate", (string)dataRow["SearchEndDate"]);
				}
				else
				{
					dataRow["EndDate"] = null;
					list.Add("EndDate");
				}
			}
			else if (!dataRow["SearchEndDate"].IsNullValue())
			{
				DiscoveryHoldPropertiesHelper.SetDate(dataRow, list, "EndDate", (string)dataRow["SearchEndDate"]);
			}
			DiscoveryHoldPropertiesHelper.SetChangedListProperty(dataRow, list, "SenderList", "Senders");
			DiscoveryHoldPropertiesHelper.SetChangedListProperty(dataRow, list, "RecipientList", "Recipients");
			DiscoveryHoldPropertiesHelper.SetChangedListProperty(dataRow, list, "MessageTypeList", "MessageTypes");
			if (!dataRow["SearchContent"].IsNullValue() && dataRow["SearchContent"].IsFalse())
			{
				dataRow["SearchQuery"] = DBNull.Value;
				list.Add("SearchQuery");
				dataRow["Senders"] = DBNull.Value;
				list.Add("Senders");
				dataRow["Recipients"] = DBNull.Value;
				list.Add("Recipients");
				dataRow["MessageTypes"] = DBNull.Value;
				list.Add("MessageTypes");
				dataRow["StartDate"] = DBNull.Value;
				list.Add("StartDate");
				dataRow["EndDate"] = DBNull.Value;
				list.Add("EndDate");
			}
			if (RbacPrincipal.Current.IsInRole("Set-MailboxSearch?EstimateOnly&ExcludeDuplicateMessages&LogLevel"))
			{
				dataRow["EstimateOnly"] = true;
				list.Add("EstimateOnly");
				dataRow["ExcludeDuplicateMessages"] = false;
				list.Add("ExcludeDuplicateMessages");
				dataRow["LogLevel"] = LoggingLevel.Suppress;
				list.Add("LogLevel");
			}
			store.SetModifiedColumns(list);
		}

		public static void GetObjectForCopySearchPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			dataRow["EnableFullLogging"] = ((LoggingLevel)dataRow["LogLevel"] == LoggingLevel.Full);
			dataRow["SendMeEmailOnComplete"] = ((MultiValuedProperty<ADObjectId>)dataRow["StatusMailRecipients"]).Contains(RbacPrincipal.Current.ExecutingUserId);
			DiscoveryHoldPropertiesHelper.GetValuesForListRow(dataRow);
		}

		public static void SetObjectForCopySearchPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			List<string> list = new List<string>();
			dataRow["EstimateOnly"] = false;
			list.Add("EstimateOnly");
			if (!dataRow["EnableFullLogging"].IsNullValue())
			{
				if (dataRow["EnableFullLogging"].IsTrue())
				{
					dataRow["LogLevel"] = LoggingLevel.Full;
				}
				else
				{
					dataRow["LogLevel"] = LoggingLevel.Basic;
				}
				list.Add("LogLevel");
			}
			if (!dataRow["LogLevel"].IsNullValue() && (LoggingLevel)dataRow["LogLevel"] == LoggingLevel.Suppress)
			{
				dataRow["LogLevel"] = LoggingLevel.Basic;
				list.Add("LogLevel");
			}
			if (!dataRow["SendMeEmailOnComplete"].IsNullValue())
			{
				if (dataRow["SendMeEmailOnComplete"].IsTrue())
				{
					dataRow["StatusMailRecipients"] = RbacPrincipal.Current.ExecutingUserId;
				}
				else
				{
					dataRow["StatusMailRecipients"] = null;
				}
				list.Add("StatusMailRecipients");
			}
			store.SetModifiedColumns(list);
		}

		public static Identity GetIdentity(object identity, object name)
		{
			return new Identity(identity as string, name as string);
		}

		public static string GetCommaSeparatedUserList(object userObjects)
		{
			string result = string.Empty;
			MultiValuedProperty<string> multiValuedProperty = userObjects as MultiValuedProperty<string>;
			if (multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				result = string.Join(", ", multiValuedProperty.ToArray<string>());
			}
			return result;
		}

		public static string GetCommaSeparatedMessageTypeList(object messageTypeObjects)
		{
			string result = "[]";
			MultiValuedProperty<KindKeyword> multiValuedProperty = messageTypeObjects as MultiValuedProperty<KindKeyword>;
			if (multiValuedProperty != null)
			{
				result = (from messageType in multiValuedProperty
				select messageType.ToString()).ToArray<string>().ToJsonString(null);
			}
			return result;
		}

		public static Identity GetTargetMailboxIdentity(object targetMailboxIdObject)
		{
			ADObjectId adobjectId = targetMailboxIdObject as ADObjectId;
			Identity result = null;
			if (adobjectId != null)
			{
				RecipientObjectResolverRow recipientObjectResolverRow = RecipientObjectResolver.Instance.ResolveObjects(new ADObjectId[]
				{
					adobjectId
				}).FirstOrDefault<RecipientObjectResolverRow>();
				if (recipientObjectResolverRow == null)
				{
					result = adobjectId.ToIdentity();
				}
				else
				{
					result = recipientObjectResolverRow.Identity;
				}
			}
			return result;
		}

		public static object GetPublicFolderId(object publicFolderObject)
		{
			PublicFolderIdParameter publicFolderIdParameter = publicFolderObject as PublicFolderIdParameter;
			if (publicFolderIdParameter != null)
			{
				return publicFolderIdParameter.ToString();
			}
			return publicFolderObject;
		}

		public static ExDateTime? GetUtcExDateTime(string dateString)
		{
			DateTime dateTime;
			if (DateTime.TryParse(dateString, out dateTime))
			{
				ExDateTime? exDateTime = dateTime.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture).ToEcpExDateTime();
				if (exDateTime != null)
				{
					return new ExDateTime?(exDateTime.Value.ToUtc());
				}
			}
			return null;
		}

		public static bool IsObjectVersionOriginal(SearchObjectVersion version)
		{
			return version == SearchObjectVersion.Original;
		}

		internal static void GetValuesForListRow(DataRow row)
		{
			ExDateTime? exDateTimeValue = row["LastModifiedTime"].IsNullValue() ? null : ((ExDateTime?)row["LastModifiedTime"]);
			SearchState searchState = (SearchState)row["Status"];
			bool flag = (bool)row["EstimateOnly"];
			row["HoldStatusDescription"] = DiscoveryHoldPropertiesHelper.GetHoldStatusDescription((bool)row["InPlaceHoldEnabled"]);
			row["LastModifiedTimeDisplay"] = exDateTimeValue.ToUserDateTimeString();
			row["LastModifiedUTCDateTime"] = ((exDateTimeValue != null) ? exDateTimeValue.Value.UniversalTime : DateTime.MinValue);
			row["CreatedByDisplayName"] = DiscoveryHoldPropertiesHelper.GetCreatedByUserDisplayName((string)row["CreatedBy"]);
			row["IsEstimateOnly"] = flag;
			row["IsStartable"] = DiscoveryHoldPropertiesHelper.IsStartable(flag, searchState);
			row["IsPreviewable"] = (searchState != SearchState.EstimateFailed && searchState != SearchState.Failed);
			row["IsStoppable"] = (searchState == SearchState.InProgress || SearchState.EstimateInProgress == searchState);
			row["IsResumable"] = (SearchState.PartiallySucceeded == searchState);
		}

		internal static string GetHoldStatusDescription(bool inPlaceHoldEnabled)
		{
			if (RbacPrincipal.Current.IsInRole("LegalHold"))
			{
				return inPlaceHoldEnabled ? Strings.DiscoveryHoldHoldStatusYes : Strings.DiscoveryHoldHoldStatusNo;
			}
			return string.Empty;
		}

		internal static string GetCreatedByUserDisplayName(string rawName)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(rawName))
			{
				result = rawName;
				int num = rawName.IndexOf("\\");
				if (num > -1 && num < rawName.Length - 1)
				{
					result = rawName.Substring(num + 1);
				}
				else
				{
					try
					{
						SecurityIdentifier sid = new SecurityIdentifier(rawName);
						SecurityPrincipalIdParameter securityPrincipalIdParameter = new SecurityPrincipalIdParameter(sid);
						IEnumerable<SecurityPrincipalIdParameter> sidPrincipalId = new SecurityPrincipalIdParameter[]
						{
							securityPrincipalIdParameter
						}.AsEnumerable<SecurityPrincipalIdParameter>();
						List<AcePermissionRecipientRow> list = RecipientObjectResolver.Instance.ResolveSecurityPrincipalId(sidPrincipalId).ToList<AcePermissionRecipientRow>();
						if (list.Count > 0)
						{
							result = list[0].DisplayName;
						}
					}
					catch (ArgumentException ex)
					{
						DDIHelper.Trace("Created by value is not a valid SID: " + ex.Message);
					}
				}
			}
			return result;
		}

		internal static bool IsStartable(bool estimateOnly, SearchState searchStatus)
		{
			if (estimateOnly)
			{
				return SearchState.EstimateInProgress != searchStatus && SearchState.EstimateStopping != searchStatus;
			}
			return searchStatus != SearchState.InProgress && SearchState.Stopping != searchStatus;
		}

		private static void SetDate(DataRow row, List<string> modifiedColumns, string propertyName, string dateString)
		{
			ExDateTime? utcExDateTime = DiscoveryHoldPropertiesHelper.GetUtcExDateTime(dateString);
			if (utcExDateTime != null)
			{
				row[propertyName] = utcExDateTime.Value.ToUtc();
				modifiedColumns.Add(propertyName);
				return;
			}
			DDIHelper.Trace(string.Format("{0} was not set because an expected date {1} resulted in null.", propertyName, dateString));
		}

		private static void SetChangedListProperty(DataRow row, List<string> modifiedColumns, string displayColumnName, string taskColumnName)
		{
			if (!row[displayColumnName].IsNullValue())
			{
				if (!string.IsNullOrEmpty((string)row[displayColumnName]))
				{
					row[taskColumnName] = ((string)row[displayColumnName]).ToArrayOfStrings();
				}
				else
				{
					row[taskColumnName] = null;
				}
				modifiedColumns.Add(taskColumnName);
			}
		}

		internal const string StartDateColumnName = "StartDate";

		internal const string EndDateColumnName = "EndDate";

		internal const string StartDateEnabledColumnName = "StartDateEnabled";

		internal const string EndDateEnabledColumnName = "EndDateEnabled";

		internal const string SearchStartDateColumnName = "SearchStartDate";

		internal const string SearchEndDateColumnName = "SearchEndDate";

		internal const string ItemHoldPeriodColumnName = "ItemHoldPeriod";

		internal const string HoldIndefinitelyColumnName = "HoldIndefinitely";

		internal const string ItemHoldPeriodDaysColumnName = "ItemHoldPeriodDays";

		internal const string SearchContentColumnName = "SearchContent";

		internal const string SearchAllMailboxesColumnName = "SearchAllMailboxes";

		internal const string AllSourceMailboxesColumnName = "AllSourceMailboxes";

		internal const string SourceMailboxesColumnName = "SourceMailboxes";

		internal const string SearchAllPublicFoldersColumnName = "SearchAllPublicFolders";

		internal const string AllPublicFolderSourcesColumnName = "AllPublicFolderSources";

		internal const string PublicFolderSourcesColumnName = "PublicFolderSources";

		internal const string SenderListColumnName = "SenderList";

		internal const string SendersColumnName = "Senders";

		internal const string RecipientListColumnName = "RecipientList";

		internal const string RecipientsColumnName = "Recipients";

		internal const string MessageTypeListColumnName = "MessageTypeList";

		internal const string MessageTypesColumnName = "MessageTypes";

		internal const string SearchQueryColumnName = "SearchQuery";

		internal const string EstimateOnlyColumnName = "EstimateOnly";

		internal const string ExcludeDuplicateMessagesColumnName = "ExcludeDuplicateMessages";

		internal const string LogLevelColumnName = "LogLevel";

		internal const string EnableFullLoggingColumnName = "EnableFullLogging";

		internal const string SendMeEmailOnCompleteColumnName = "SendMeEmailOnComplete";

		internal const string StatusMailRecipientsColumnName = "StatusMailRecipients";

		internal const string LastModifiedTimeColumnName = "LastModifiedTime";

		internal const string StatusColumnName = "Status";

		internal const string HoldStatusDescriptionColumnName = "HoldStatusDescription";

		internal const string InPlaceHoldEnabledColumnName = "InPlaceHoldEnabled";

		internal const string LastModifiedTimeDisplayColumnName = "LastModifiedTimeDisplay";

		internal const string LastModifiedUTCDateTimeColumnName = "LastModifiedUTCDateTime";

		internal const string CreatedByDisplayNameColumnName = "CreatedByDisplayName";

		internal const string CreatedByColumnName = "CreatedBy";

		internal const string IsEstimateOnlyColumnName = "IsEstimateOnly";

		internal const string IsStartableColumnName = "IsStartable";

		internal const string IsPreviewableColumnName = "IsPreviewable";

		internal const string IsStoppableColumnName = "IsStoppable";

		internal const string IsResumableColumnName = "IsResumable";
	}
}
