using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Filtering;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaRulesEvaluationContext : BaseTransportRulesEvaluationContext
	{
		internal string RuleEvalLatency
		{
			get
			{
				return this.ruleEvalLatency.ToString();
			}
		}

		internal string RuleEvalLResult
		{
			get
			{
				return this.ruleEvalResult.ToString();
			}
		}

		public OwaRulesEvaluationContext(RuleCollection rules, ScanResultStorageProvider scanResultStorageProvider, Item item, string fromAddress, ShortList<string> recipients, PolicyTipRequestLogger policyTipRequestLogger) : base(rules, new OwaRulesTracer())
		{
			if (rules == null)
			{
				throw new ArgumentNullException("rules");
			}
			if (scanResultStorageProvider == null)
			{
				throw new ArgumentNullException("scanResultStorageProvider");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (fromAddress == null)
			{
				throw new ArgumentNullException("fromAddress");
			}
			if (policyTipRequestLogger == null)
			{
				throw new ArgumentNullException("policyTipRequestLogger");
			}
			this.Item = item;
			this.ScanResultStorageProvider = scanResultStorageProvider;
			this.FromAddress = fromAddress;
			this.Recipients = recipients;
			this.userComparer = Microsoft.Exchange.Clients.Owa2.Server.Core.UserComparer.CreateInstance();
			this.membershipChecker = new MembershipChecker(this.OrganizationId);
			base.SetConditionEvaluationMode(ConditionEvaluationMode.Full);
			this.policyTipRequestLogger = policyTipRequestLogger;
			this.RuleExecutionMonitor = new RuleHealthMonitor(RuleHealthMonitor.ActivityType.Execute, 1L, 0L, delegate(string eventMessageDetails)
			{
			});
			this.RuleExecutionMonitor.MtlLogWriter = new RuleHealthMonitor.MtlLogWriterDelegate(this.AppendPerRuleDiagnosticData);
			this.RuleExecutionMonitor.TenantId = this.OrganizationId.ToString();
		}

		public void AppendPerRuleDiagnosticData(string agentName, string eventTopic, List<KeyValuePair<string, string>> data)
		{
			if (data != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in data)
				{
					this.ruleEvalLatency.Append(keyValuePair.Key);
					this.ruleEvalLatency.Append("=");
					this.ruleEvalLatency.Append(keyValuePair.Value);
					this.ruleEvalLatency.Append("|");
				}
			}
		}

		public List<DlpPolicyMatchDetail> DlpPolicyMatchDetails
		{
			get
			{
				return this.dlpPolicyMatchDetails;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.Item.Session.OrganizationId;
			}
		}

		public RuleHealthMonitor RuleExecutionMonitor { get; set; }

		public Item Item { get; private set; }

		public ScanResultStorageProvider ScanResultStorageProvider { get; private set; }

		public bool NoContentMatch { get; private set; }

		public ShortList<string> Recipients { get; private set; }

		public string FromAddress { get; private set; }

		public override IStringComparer UserComparer
		{
			get
			{
				return this.userComparer;
			}
		}

		public override IStringComparer MembershipChecker
		{
			get
			{
				return this.membershipChecker;
			}
		}

		protected override FilteringServiceInvokerRequest FilteringServiceInvokerRequest
		{
			get
			{
				return OwaFilteringServiceInvokerRequest.CreateInstance(this.Item, this.ScanResultStorageProvider);
			}
		}

		public override bool ShouldInvokeFips()
		{
			this.invokeFips = (this.Item != null && this.ScanResultStorageProvider.NeedsClassificationForBodyOrAnyAttachments());
			this.policyTipRequestLogger.AppendData("InvokeFips".ToString(), this.invokeFips ? "1" : "0");
			if (this.invokeFips)
			{
				this.fipsScanStartTime = DateTime.UtcNow;
			}
			return this.invokeFips;
		}

		protected override void OnDataClassificationsRetrieved(FilteringResults filteringResults)
		{
			if (this.invokeFips)
			{
				TimeSpan timeSpan = DateTime.UtcNow - this.fipsScanStartTime;
				this.policyTipRequestLogger.AppendExtraData("FipsLatency", timeSpan.TotalMilliseconds.ToString());
				if (filteringResults != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (filteringResults.Streams != null)
					{
						foreach (StreamIdentity streamIdentity in filteringResults.Streams)
						{
							stringBuilder.Append(string.Format("(StreamName:{0}/StreamId:{1}/ParentId:{2})", streamIdentity.Name, streamIdentity.Id, streamIdentity.ParentId));
						}
					}
					StringBuilder stringBuilder2 = new StringBuilder();
					if (filteringResults.ScanResults != null)
					{
						foreach (ScanResult scanResult in filteringResults.ScanResults)
						{
							stringBuilder2.Append(string.Format("(StreamName:{0}/StreamId:{1}/ElapsedTime:{2})", scanResult.Stream.Name, scanResult.Stream.Id, scanResult.ElapsedTime));
						}
					}
					this.policyTipRequestLogger.AppendExtraData("FipsParsedStreams", stringBuilder.ToString());
					this.policyTipRequestLogger.AppendExtraData("FipsPerScanResultLatency", stringBuilder2.ToString());
				}
			}
			IEnumerable<DiscoveredDataClassification> enumerable = FipsResultParser.ParseDataClassifications(filteringResults, base.Tracer);
			this.policyTipRequestLogger.AppendData("NewClassifications", DiscoveredDataClassification.ToString(enumerable));
			IEnumerable<DiscoveredDataClassification> dlpDetectedClassificationObjects = this.ScanResultStorageProvider.GetDlpDetectedClassificationObjects();
			this.policyTipRequestLogger.AppendData("OldClassifications", DiscoveredDataClassification.ToString(dlpDetectedClassificationObjects));
			IEnumerable<DiscoveredDataClassification> enumerable2 = FipsResultParser.UnionDiscoveredDataClassificationsFromDistinctStreams(dlpDetectedClassificationObjects, enumerable);
			this.policyTipRequestLogger.AppendData("UnionClassifications", DiscoveredDataClassification.ToString(enumerable2));
			base.SetDataClassifications(enumerable2);
			this.ScanResultStorageProvider.SetHasDlpDetectedClassifications();
			this.ScanResultStorageProvider.SetDlpDetectedClassificationObjects(enumerable2);
			StringBuilder stringBuilder3 = new StringBuilder();
			foreach (DiscoveredDataClassification discoveredDataClassification in enumerable2)
			{
				stringBuilder3.Append(discoveredDataClassification.Id);
				stringBuilder3.Append("|");
			}
			this.ScanResultStorageProvider.SetDlpDetectedClassifications(stringBuilder3.ToString());
			if (!enumerable2.Any<DiscoveredDataClassification>())
			{
				this.NoContentMatch = true;
			}
		}

		internal void CapturePerRuleData()
		{
			RuleEvaluationResult currentRuleResult = base.RulesEvaluationHistory.GetCurrentRuleResult(this);
			this.ruleEvalResult.Append(string.Format("RuleName:{0};Result:{1}.", base.CurrentRule.Name, (currentRuleResult == null) ? "null" : OwaRulesEvaluationContext.RuleEvalResultToString(currentRuleResult)));
		}

		internal void CapturePerRuleMatchData()
		{
			DlpPolicyMatchDetail dlpPolicyMatchDetail = new DlpPolicyMatchDetail();
			dlpPolicyMatchDetail.Action = (DlpPolicyTipAction)Enum.Parse(typeof(DlpPolicyTipAction), base.ActionName);
			if (base.MatchedClassifications != null)
			{
				dlpPolicyMatchDetail.Classifications = new string[base.MatchedClassifications.Count];
				int num = 0;
				foreach (string key in base.MatchedClassifications.Keys)
				{
					dlpPolicyMatchDetail.Classifications[num++] = base.MatchedClassifications[key];
				}
			}
			EmailAddressWrapper[] recipients = null;
			string[] attachmentIds = null;
			OwaRulesEvaluationContext.TrackMatchingRecipientsAndAttachments(base.RulesEvaluationHistory.GetCurrentRuleResult(this), this.policyTipRequestLogger, out recipients, out attachmentIds);
			dlpPolicyMatchDetail.Recipients = recipients;
			dlpPolicyMatchDetail.AttachmentIds = OwaRulesEvaluationContext.ConvertAttachmentIdsToAttachmentIdTypes(attachmentIds, this.Item, this.policyTipRequestLogger);
			this.DlpPolicyMatchDetails.Add(dlpPolicyMatchDetail);
		}

		private static AttachmentIdType[] ConvertAttachmentIdsToAttachmentIdTypes(string[] attachmentIds, Item item, PolicyTipRequestLogger policyTipRequestLogger)
		{
			if (attachmentIds == null || attachmentIds.Length == 0)
			{
				return null;
			}
			List<AttachmentIdType> list = new List<AttachmentIdType>();
			foreach (string text in attachmentIds)
			{
				AttachmentId item2 = null;
				try
				{
					item2 = AttachmentId.Deserialize(text);
				}
				catch (CorruptDataException)
				{
					policyTipRequestLogger.AppendData("InvalidAttachment", text);
				}
				AttachmentIdType item3 = new AttachmentIdType(new IdAndSession(item.Id, item.Session, new List<AttachmentId>
				{
					item2
				}).GetConcatenatedId().Id);
				list.Add(item3);
			}
			return list.ToArray();
		}

		internal static void TrackMatchingRecipientsAndAttachments(RuleEvaluationResult ruleEvaluationResult, PolicyTipRequestLogger policyTipRequestLogger, out EmailAddressWrapper[] recipientEmails, out string[] attachmentIds)
		{
			recipientEmails = null;
			attachmentIds = null;
			if (ruleEvaluationResult == null)
			{
				return;
			}
			if (ruleEvaluationResult.Predicates != null)
			{
				List<string> listA = null;
				PredicateEvaluationResult predicateEvaluationResult = RuleEvaluationResult.GetPredicateEvaluationResult(typeof(SentToPredicate), ruleEvaluationResult.Predicates).FirstOrDefault<PredicateEvaluationResult>();
				if (predicateEvaluationResult != null)
				{
					listA = predicateEvaluationResult.MatchResults;
				}
				List<string> listB = null;
				PredicateEvaluationResult predicateEvaluationResult2 = RuleEvaluationResult.GetPredicateEvaluationResult(typeof(SentToScopePredicate), ruleEvaluationResult.Predicates).FirstOrDefault<PredicateEvaluationResult>();
				if (predicateEvaluationResult2 != null)
				{
					listB = predicateEvaluationResult2.MatchResults;
				}
				List<string> locationList = null;
				PredicateEvaluationResult predicateEvaluationResult3 = (from mcdc in RuleEvaluationResult.GetPredicateEvaluationResult(typeof(ContainsDataClassificationPredicate), ruleEvaluationResult.Predicates)
				where mcdc.SupplementalInfo == 1
				select mcdc).FirstOrDefault<PredicateEvaluationResult>();
				if (predicateEvaluationResult3 != null)
				{
					locationList = predicateEvaluationResult3.MatchResults;
				}
				recipientEmails = OwaRulesEvaluationContext.IntersectAndReturnEmailAddressWrappers(listA, listB);
				attachmentIds = OwaRulesEvaluationContext.GetAttachmentTypeIds(locationList, policyTipRequestLogger);
			}
		}

		private static string[] GetAttachmentTypeIds(List<string> locationList, PolicyTipRequestLogger policyTipRequestLogger)
		{
			if (locationList == null)
			{
				return null;
			}
			List<string> list = new List<string>();
			foreach (string text in locationList)
			{
				if (!string.IsNullOrEmpty(text) && !text.Equals("Message Body", StringComparison.OrdinalIgnoreCase))
				{
					int num = text.LastIndexOf(':');
					if (num <= 0 || num == text.Length - 1)
					{
						policyTipRequestLogger.AppendData("InvalidAttachment", text);
					}
					else
					{
						string value = text.Substring(0, num);
						string text2 = text.Substring(num + 1);
						if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(text2))
						{
							policyTipRequestLogger.AppendData("InvalidAttachment", text);
						}
						else
						{
							list.Add(text2);
						}
					}
				}
			}
			return list.ToArray();
		}

		private static EmailAddressWrapper[] IntersectAndReturnEmailAddressWrappers(List<string> listA, List<string> listB)
		{
			EmailAddressWrapper[] array = null;
			List<string> list;
			if (listA == null)
			{
				list = listB;
			}
			else if (listB == null)
			{
				list = listA;
			}
			else
			{
				list = listA.Intersect(listB, StringComparer.OrdinalIgnoreCase).ToList<string>();
			}
			if (list != null)
			{
				array = new EmailAddressWrapper[list.Count];
				int num = 0;
				foreach (string emailAddress in list)
				{
					EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper();
					emailAddressWrapper.EmailAddress = emailAddress;
					array[num++] = emailAddressWrapper;
				}
			}
			return array;
		}

		internal static string RuleEvalResultToString(RuleEvaluationResult ruleEvalResult)
		{
			if (ruleEvalResult == null)
			{
				return string.Empty;
			}
			return string.Format("IsRuleMatch:{0}/Predicates:{1}/Actions:{2}.", ruleEvalResult.IsMatch ? "1" : "0", string.Join(";", from predicate in ruleEvalResult.Predicates
			select OwaRulesEvaluationContext.PredicateEvalResultToString(predicate)), string.Join(";", ruleEvalResult.Actions));
		}

		internal static string PredicateEvalResultToString(PredicateEvaluationResult predicateEvalResult)
		{
			if (predicateEvalResult == null)
			{
				return string.Empty;
			}
			List<string> values = (predicateEvalResult.Type == typeof(SentToPredicate) || predicateEvalResult.Type == typeof(SentToScopePredicate) || predicateEvalResult.Type == typeof(OwaIsSameUserPredicate)) ? PolicyTipRequestLogger.MarkAsPII(predicateEvalResult.MatchResults) : predicateEvalResult.MatchResults;
			return string.Format("IsPredicateMatch:{0}/MatchResults:{1}/SupplInfo:{2}/Type:{3}/PropertyName:{4}.", new object[]
			{
				predicateEvalResult.IsMatch ? "1" : "0",
				string.Join(";", values),
				predicateEvalResult.SupplementalInfo,
				predicateEvalResult.Type,
				predicateEvalResult.PropertyName ?? string.Empty
			});
		}

		private const string RuleEvalResultToStringFormat = "IsRuleMatch:{0}/Predicates:{1}/Actions:{2}.";

		private const string PredicateEvalResultToStringFormat = "IsPredicateMatch:{0}/MatchResults:{1}/SupplInfo:{2}/Type:{3}/PropertyName:{4}.";

		private const string FipsPerScanResultLatencyFormat = "(StreamName:{0}/StreamId:{1}/ElapsedTime:{2})";

		private const string FipsParsedStreamsFormat = "(StreamName:{0}/StreamId:{1}/ParentId:{2})";

		private IStringComparer userComparer;

		private IStringComparer membershipChecker;

		private PolicyTipRequestLogger policyTipRequestLogger;

		private StringBuilder ruleEvalLatency = new StringBuilder();

		private StringBuilder ruleEvalResult = new StringBuilder();

		private List<DlpPolicyMatchDetail> dlpPolicyMatchDetails = new List<DlpPolicyMatchDetail>();

		private bool invokeFips;

		private DateTime fipsScanStartTime;
	}
}
