using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.EDiscovery.Export;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.Search;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal static class Util
	{
		internal static void ThrowIfNull(object objectToCheck, string parameterName)
		{
			if (objectToCheck == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		internal static void ThrowIfNullOrEmpty(string theString, string parameterName)
		{
			if (string.IsNullOrEmpty(theString))
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		internal static void UpdateSearchObject(IDiscoverySearchDataProvider dataProvider, DiscoverySearchBase searchObject)
		{
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						dataProvider.CreateOrUpdate<DiscoverySearchBase>(searchObject);
					});
				}
				catch (DataSourceOperationException ex)
				{
					ExTraceGlobals.SearchTracer.TraceError<string, string>(0L, "UpdateSearchObject:Update Search failed for {0}. Root cause: {1}", searchObject.Name, ex.ToString());
					SearchEventLogger.Instance.LogDiscoverySearchTaskErrorEvent(searchObject.Name, dataProvider.OrganizationId.ToString(), ex);
				}
				catch (GrayException ex2)
				{
					ExTraceGlobals.SearchTracer.TraceError<GrayException>(0L, "UpdateSearchObject: GrayException {0} is thrown", ex2);
					SearchEventLogger.Instance.LogDiscoverySearchTaskErrorEvent(searchObject.Name, dataProvider.OrganizationId.ToString(), ex2);
				}
			}, delegate(object exception)
			{
				ExTraceGlobals.SearchTracer.TraceError(0L, "UpdateSearchObject: Unhandled exception {0}", new object[]
				{
					exception
				});
				SearchEventLogger.Instance.LogDiscoverySearchTaskErrorEvent(searchObject.Name, dataProvider.OrganizationId.ToString(), exception.ToString());
				return !(exception is GrayException);
			});
		}

		internal static ITargetMailbox CreateTargetMailbox(IRecipientSession recipientSession, MailboxDiscoverySearch searchObject, IExportContext exportContext)
		{
			Util.ThrowIfNull(recipientSession, "recipientSession");
			Util.ThrowIfNull(searchObject, "searchObject");
			Util.ThrowIfNull(exportContext, "exportContext");
			ADSessionSettings sessionSettings = recipientSession.SessionSettings;
			string target = searchObject.Target;
			Util.ThrowIfNull(sessionSettings, "adSessionSettings");
			Util.ThrowIfNullOrEmpty(target, "targetLegacyDn");
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromLegacyDN(sessionSettings, target, RemotingOptions.AllowCrossSite);
			if (exchangePrincipal == null || string.IsNullOrEmpty(exchangePrincipal.MailboxInfo.Location.ServerFqdn))
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFoundId(target));
			}
			return new TargetMailbox(exchangePrincipal.MailboxInfo.OrganizationId, exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), target, BackEndLocator.GetBackEndWebServicesUrl(exchangePrincipal.MailboxInfo), exportContext)
			{
				StatusMailRecipients = Util.GetStatusMailRecipients(searchObject, recipientSession)
			};
		}

		internal static ITargetMailbox CreateTargetMailbox(IRecipientSession recipientSession, MailboxDiscoverySearch searchObject, string targetLegacyDn, ITargetLocation targetLocation)
		{
			Util.ThrowIfNull(recipientSession, "recipientSession");
			Util.ThrowIfNull(searchObject, "searchObject");
			Util.ThrowIfNullOrEmpty(targetLegacyDn, "targetLegacyDn");
			Util.ThrowIfNull(targetLocation, "targetLocation");
			ADSessionSettings sessionSettings = recipientSession.SessionSettings;
			Util.ThrowIfNull(sessionSettings, "adSessionSettings");
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromLegacyDN(sessionSettings, targetLegacyDn, RemotingOptions.AllowCrossSite);
			if (exchangePrincipal == null || string.IsNullOrEmpty(exchangePrincipal.MailboxInfo.Location.ServerFqdn))
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFoundId(targetLegacyDn));
			}
			return new TargetMailbox(exchangePrincipal.MailboxInfo.OrganizationId, exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), targetLegacyDn, BackEndLocator.GetBackEndWebServicesUrl(exchangePrincipal.MailboxInfo), targetLocation)
			{
				StatusMailRecipients = Util.GetStatusMailRecipients(searchObject, recipientSession)
			};
		}

		internal static ITargetLocation CreateTargetLocation(string exportLocationName)
		{
			Util.ThrowIfNullOrEmpty(exportLocationName, "exportLocationName");
			return new CopyTargetLocation(exportLocationName, exportLocationName + Constants.WorkingFolderSuffix);
		}

		internal static QueryFilter CreateRecipientTypeFilter(RecipientType recipientType)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, recipientType);
		}

		internal static QueryFilter CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails recipientTypeDetails)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, recipientTypeDetails);
		}

		internal static List<KeywordHit> ConvertToKeywordHitList(List<KeywordStatisticsSearchResultType> keywordStats, Dictionary<string, string> userKeywordsMap)
		{
			if (keywordStats == null)
			{
				return null;
			}
			List<KeywordHit> list = new List<KeywordHit>(keywordStats.Count);
			foreach (KeywordStatisticsSearchResultType keywordStatisticsSearchResultType in keywordStats)
			{
				string phrase = keywordStatisticsSearchResultType.Keyword;
				foreach (KeyValuePair<string, string> keyValuePair in userKeywordsMap)
				{
					if (string.Compare(keyValuePair.Value, keywordStatisticsSearchResultType.Keyword, StringComparison.OrdinalIgnoreCase) == 0)
					{
						phrase = keyValuePair.Key;
						break;
					}
				}
				list.Add(new KeywordHit
				{
					Phrase = phrase,
					Count = keywordStatisticsSearchResultType.ItemHits,
					Size = new ByteQuantifiedSize((ulong)keywordStatisticsSearchResultType.Size)
				});
			}
			return list;
		}

		internal static KeywordHit GetKeywordHit(IList<KeywordHit> keywordHits, string keyword)
		{
			if (keywordHits != null && keywordHits.Count > 0)
			{
				foreach (KeywordHit keywordHit in keywordHits)
				{
					if (string.Compare(keywordHit.Phrase, keyword, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return keywordHit;
					}
				}
			}
			return null;
		}

		internal static string GetUserNameFromUserId(IRecipientSession recipientSession, string userId)
		{
			Util.ThrowIfNull(recipientSession, "recipientSession");
			Util.ThrowIfNullOrEmpty(userId, "userId");
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = Util.ReadExchangeRunspaceConfiguration(recipientSession, userId);
			if (exchangeRunspaceConfiguration == null)
			{
				Util.Tracer.TraceError<string>(0L, "Unable to find executing user: {0}", userId);
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFoundId(userId));
			}
			string text = (!string.IsNullOrEmpty(exchangeRunspaceConfiguration.ExecutingUserDisplayName)) ? exchangeRunspaceConfiguration.ExecutingUserDisplayName : exchangeRunspaceConfiguration.IdentityName;
			return text ?? string.Empty;
		}

		internal static ExchangeRunspaceConfiguration ReadExchangeRunspaceConfiguration(IRecipientSession recipientSession, string userId)
		{
			RBACContext rbaccontext = null;
			if (RBACContext.TryParseRbacContextString(userId, out rbaccontext))
			{
				return rbaccontext.CreateExchangeRunspaceConfiguration();
			}
			try
			{
				Guid guid = new Guid(userId);
				ADUser aduser = (ADUser)recipientSession.Read(new ADObjectId(guid));
				if (aduser != null)
				{
					GenericIdentity identity = new GenericIdentity(aduser.Sid.ToString());
					return new ExchangeRunspaceConfiguration(identity);
				}
			}
			catch (Exception arg)
			{
				Util.Tracer.TraceError<Exception>(0L, "ReadExchangeRunspaceConfiguration error: {0}", arg);
			}
			return null;
		}

		internal static string QuoteValueIfRequired(string value)
		{
			if (!string.IsNullOrEmpty(value) && value.IndexOfAny(new char[]
			{
				',',
				'"'
			}) != -1)
			{
				value = value.Replace("\"", "\"\"");
				return string.Format("\"{0}\"", value);
			}
			return value;
		}

		internal static string CreateLogMailBody(MailboxDiscoverySearch searchObject, string[] statusMailRecipients, List<string> successfulMailboxes, List<string> unsuccessfulMailboxes, IList<ISource> srcMailboxes)
		{
			return Util.CreateMailBody(Util.LogMailTemplate, searchObject, statusMailRecipients, successfulMailboxes, unsuccessfulMailboxes, srcMailboxes);
		}

		internal static string CreateStatusMailBody(MailboxDiscoverySearch searchObject, string[] statusMailRecipients, List<string> successfulMailboxes, List<string> unsuccessfulMailboxes, IList<ISource> srcMailboxes)
		{
			return Util.CreateMailBody(Util.StatusMailTemplate, searchObject, statusMailRecipients, successfulMailboxes, unsuccessfulMailboxes, srcMailboxes);
		}

		internal static string GenerateErrorMessageFromErrorRecord(ErrorRecord errorRecord)
		{
			return string.Format("Failed Search or Export, Mailbox:{0}::Item:{1}::DocumentId:{2}::ItemId:{3} with error: [{4}] {5}", new object[]
			{
				errorRecord.SourceId,
				(errorRecord.Item == null) ? string.Empty : errorRecord.Item.Title,
				(errorRecord.Item == null) ? string.Empty : errorRecord.Item.DocumentId.ToString(CultureInfo.InvariantCulture),
				(errorRecord.Item == null) ? string.Empty : errorRecord.Item.Id,
				errorRecord.ErrorType.ToString(),
				errorRecord.DiagnosticMessage
			});
		}

		private static string CreateMailBody(string templateName, MailboxDiscoverySearch searchObject, string[] statusMailRecipients, List<string> successfulMailboxes, List<string> unsuccessfulMailboxes, IList<ISource> srcMailboxes)
		{
			Util.ThrowIfNullOrEmpty(templateName, "templateName");
			Util.ThrowIfNull(searchObject, "searchObject");
			StringBuilder stringBuilder = new StringBuilder();
			using (StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(templateName)))
			{
				stringBuilder.Append(streamReader.ReadToEnd());
			}
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.Identity, searchObject.Identity.ToString());
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.LastStartTime, searchObject.LastStartTime);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.LastEndTime, searchObject.LastEndTime);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.CreatedBy, searchObject.CreatedBy);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.Name, searchObject.Name);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.SearchQuery, searchObject.Query);
			string text = searchObject.Senders.AggregateOfDefault((string s, string x) => s + ", " + x);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.Senders, text.ValueOrDefault(Strings.LogMailAll));
			text = searchObject.Recipients.AggregateOfDefault((string s, string x) => s + ", " + x);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.Recipients, text.ValueOrDefault(Strings.LogMailAll));
			text = ((searchObject.StartDate != null) ? string.Format("{0}, {0:%z}", searchObject.StartDate.Value) : Strings.LogMailBlank);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.StartDate, text);
			text = ((searchObject.EndDate != null) ? string.Format("{0}, {0:%z}", searchObject.EndDate.Value) : Strings.LogMailBlank);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.EndDate, text);
			text = (from x in searchObject.MessageTypes
			select x.ToString()).AggregateOfDefault((string s, string x) => s + ", " + x);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.MessageTypes, text.ValueOrDefault(Strings.LogMailAll));
			if (searchObject.StatisticsOnly)
			{
				Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.TargetMailbox, Strings.LogMailNone);
			}
			else
			{
				Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.TargetMailbox, searchObject.Target);
			}
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.LogLevel, searchObject.LogLevel);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.ExcludeDuplicateMessages, searchObject.ExcludeDuplicateMessages);
			StringBuilder sb = stringBuilder;
			Globals.LogFields logField = Globals.LogFields.SourceRecipients;
			object value;
			if (srcMailboxes != null)
			{
				value = string.Join(", ", (from src in srcMailboxes
				select src.Id).ToArray<string>());
			}
			else
			{
				value = null;
			}
			Util.ReplaceLogFieldTags(sb, logField, value);
			text = statusMailRecipients.AggregateOfDefault((string s, string x) => s + ", " + x);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.StatusMailRecipients, text.ValueOrDefault(Strings.LogMailNone));
			text = (from x in searchObject.ManagedBy
			select x.ToString()).AggregateOfDefault((string s, string x) => s + ", " + x);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.ManagedBy, text.ValueOrDefault(Strings.LogMailNone));
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.LastRunBy, searchObject.LastModifiedBy);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.NumberMailboxesToSearch, searchObject.NumberOfMailboxes);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.NumberSuccessfulMailboxes, (successfulMailboxes == null) ? 0 : successfulMailboxes.Count);
			text = null;
			if (successfulMailboxes != null && successfulMailboxes.Count > 0)
			{
				text = (from x in successfulMailboxes
				select x).AggregateOfDefault((string s, string x) => s + ", " + x);
			}
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.SuccessfulMailboxes, string.IsNullOrEmpty(text) ? Strings.LogMailNone : text);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.NumberUnsuccessfulMailboxes, (unsuccessfulMailboxes == null) ? 0 : unsuccessfulMailboxes.Count);
			text = null;
			if (unsuccessfulMailboxes != null && unsuccessfulMailboxes.Count > 0)
			{
				text = (from x in unsuccessfulMailboxes
				select x).AggregateOfDefault((string s, string x) => s + ", " + x);
			}
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.UnsuccessfulMailboxes, string.IsNullOrEmpty(text) ? Strings.LogMailNone : text);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.Resume, searchObject.Resume);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.IncludeKeywordStatistics, searchObject.IncludeKeywordStatistics);
			if (searchObject.Status == SearchState.Stopped || searchObject.Status == SearchState.EstimateStopped)
			{
				Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.StoppedBy, searchObject.LastModifiedBy);
			}
			else
			{
				Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.StoppedBy, Strings.LogMailNotApplicable);
			}
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.PercentComplete, string.Format("{0}%", searchObject.PercentComplete));
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.ResultSize, new ByteQuantifiedSize((ulong)searchObject.ResultSizeCopied));
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.ResultSizeEstimate, searchObject.ResultSizeEstimate);
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.ResultSizeCopied, new ByteQuantifiedSize((ulong)searchObject.ResultSizeCopied));
			if (searchObject.StatisticsOnly)
			{
				Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.ResultsLink, string.Empty);
			}
			else
			{
				Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.ResultsLink, searchObject.ResultsLink);
			}
			int num = Math.Min(Util.MaxNumberOfErrorsInStatusMessage, searchObject.Errors.Count);
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int i = 0; i < num; i++)
			{
				stringBuilder2.Append(searchObject.Errors[i]);
			}
			text = stringBuilder2.ToString();
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.Errors, text.ValueOrDefault(Strings.LogMailNone));
			KeywordHit unsearchableHit = null;
			StringBuilder stringBuilder3 = new StringBuilder();
			if (searchObject.KeywordHits != null && searchObject.KeywordHits.Count > 0)
			{
				stringBuilder3.AppendLine("<table style=\"width: 100%\" cellspacing=\"0\">");
				stringBuilder3.AppendFormat("<tr> <td class=\"lefttd\"><strong>{0}</strong>&nbsp;</td><td class=\"lefttd\"><strong>{1}</strong>&nbsp;</td><td class=\"rightttd\"><strong>{2}</strong></td></tr>", Strings.LogFieldsKeywordKeyword, Strings.LogFieldsKeywordHitCount, Strings.LogFieldsKeywordMbxs);
				foreach (KeywordHit keywordHit in searchObject.KeywordHits)
				{
					if (keywordHit.Phrase != "652beee2-75f7-4ca0-8a02-0698a3919cb9")
					{
						stringBuilder3.AppendFormat("<tr><td class=\"lefttd\">{0}&nbsp;</td><td class=\"lefttd\">{1}&nbsp;</td><td class=\"rightttd\">{2}</td></tr>", keywordHit.Phrase, keywordHit.Count, keywordHit.MailboxCount);
					}
					else
					{
						unsearchableHit = keywordHit;
					}
				}
				stringBuilder3.Append("</table>");
			}
			else if (!searchObject.StatisticsOnly)
			{
				stringBuilder3.Append(Strings.NoKeywordStatsForCopySearch);
			}
			else if (string.IsNullOrEmpty(searchObject.Query))
			{
				stringBuilder3.Append(Strings.KeywordHitEmptyQuery);
			}
			else
			{
				stringBuilder3.Append(Strings.KeywordStatsNotRequested);
			}
			Util.ReplaceLogFieldTags(stringBuilder, Globals.LogFields.KeywordHits, stringBuilder3.ToString());
			Util.BuildResultNumbers(stringBuilder, searchObject, unsearchableHit);
			stringBuilder = stringBuilder.Replace(Globals.LogFields.LogMailHeader.ToLabelTag(), Strings.LogMailHeader(searchObject.Name, LocalizedDescriptionAttribute.FromEnum(typeof(SearchState), searchObject.Status)));
			if (!searchObject.StatisticsOnly)
			{
				stringBuilder = stringBuilder.Replace(Globals.LogFields.LogMailHeaderInstructions.ToLabelTag(), Strings.LogMailHeaderInstructions(searchObject.Name));
			}
			else
			{
				stringBuilder.Replace(Globals.LogFields.LogMailHeaderInstructions.ToLabelTag(), string.Empty);
			}
			stringBuilder = stringBuilder.Replace(Globals.LogFields.LogMailSeeAttachment.ToLabelTag(), Strings.LogMailSeeAttachment);
			stringBuilder = stringBuilder.Replace(Globals.LogFields.LogMailFooter.ToLabelTag(), Strings.LogMailFooter);
			return stringBuilder.ToString();
		}

		private static void BuildResultNumbers(StringBuilder sbLogMailBody, MailboxDiscoverySearch searchObject, KeywordHit unsearchableHit)
		{
			if (searchObject.IsFeatureFlighted("SearchStatsFlighted"))
			{
				Util.BuildResultNumbersEx(sbLogMailBody, searchObject);
				return;
			}
			Util.ReplaceLogFieldTags(sbLogMailBody, Globals.LogFields.ResultNumberEstimate, searchObject.ResultItemCountEstimate);
			if (searchObject.ExcludeDuplicateMessages)
			{
				Util.ReplaceLogFieldTags(sbLogMailBody, Globals.LogFields.EstimateNotExcludeDuplicates, Strings.LogFieldsEstimateNotExcludeDuplicates);
			}
			else
			{
				Util.ReplaceLogFieldTags(sbLogMailBody, Globals.LogFields.EstimateNotExcludeDuplicates, string.Empty);
			}
			string value = searchObject.ResultItemCountCopied.ToString();
			if (unsearchableHit != null)
			{
				value = string.Format(Strings.MailboxSeachCountIncludeUnsearchable, searchObject.ResultItemCountCopied, unsearchableHit.Count);
			}
			Util.ReplaceLogFieldTags(sbLogMailBody, Globals.LogFields.ResultNumber, value);
		}

		private static void BuildResultNumbersEx(StringBuilder sbLogMailBody, MailboxDiscoverySearch searchObject)
		{
			DiscoverySearchStats discoverySearchStats = null;
			if (searchObject.SearchStatistics != null)
			{
				if (searchObject.SearchStatistics.Count == 0)
				{
					searchObject.SearchStatistics.Add(new DiscoverySearchStats());
				}
				discoverySearchStats = searchObject.SearchStatistics[0];
			}
			if (discoverySearchStats == null)
			{
				Util.Tracer.TraceWarning((long)searchObject.GetHashCode(), "searchObject does not contain SearchStatistics");
				throw new CorruptDataException(ServerStrings.ErrorCorruptedData("searchObject"));
			}
			Util.ReplaceLogFieldTags(sbLogMailBody, Globals.LogFields.ResultNumber, string.Empty);
			sbLogMailBody = sbLogMailBody.Replace(", " + Globals.LogFields.ResultNumberEstimate.ToLabelTag(), string.Empty);
			Util.ReplaceLogFieldTags(sbLogMailBody, Globals.LogFields.ResultNumberEstimate, string.Empty);
			Util.ReplaceLogFieldTags(sbLogMailBody, Globals.LogFields.EstimateNotExcludeDuplicates, discoverySearchStats.ToHtmlString());
		}

		private static string[] GetStatusMailRecipients(MailboxDiscoverySearch searchObject, IRecipientSession recipientSession)
		{
			return (from r in searchObject.StatusMailRecipients.Select(delegate(ADObjectId recipientId)
			{
				ADRecipient adrecipient = recipientSession.Read(recipientId);
				if (adrecipient == null)
				{
					Util.Tracer.TraceWarning<ADObjectId>((long)searchObject.GetHashCode(), "Unable to find status mail recipient '{0}'", recipientId);
					return null;
				}
				return adrecipient.PrimarySmtpAddress.ToString();
			})
			where r != null
			select r).ToArray<string>();
		}

		private static void ReplaceLogFieldTags(StringBuilder sb, Globals.LogFields logField, object value)
		{
			sb = sb.Replace(logField.ToLabelTag(), LocalizedDescriptionAttribute.FromEnum(typeof(Globals.LogFields), logField) + ":");
			sb = sb.Replace(logField.ToValueTag(), string.Format("{0}", value));
		}

		internal static readonly Trace Tracer = ExTraceGlobals.SearchTracer;

		private static readonly string LogMailTemplate = "LogMailTemplate.htm";

		private static readonly string StatusMailTemplate = "StatusMailTemplate.htm";

		private static readonly int MaxNumberOfErrorsInStatusMessage = 500;
	}
}
