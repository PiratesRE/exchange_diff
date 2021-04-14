using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Filtering;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class GetDlpPolicyTipsCommand : ServiceCommand<GetDlpPolicyTipsResponse>
	{
		public BaseItemId ItemId { get; set; }

		public bool NeedToReclassify { get; set; }

		public bool BodyOrSubjectChanged { get; set; }

		public bool CustomizedStringsNeeded { get; set; }

		public EventTrigger EventTrigger { get; set; }

		public EmailAddressWrapper[] Recipients { get; set; }

		private string ScanResultData { get; set; }

		private bool ClientSupportsScanResultData { get; set; }

		public GetDlpPolicyTipsCommand(CallContext callContext, GetDlpPolicyTipsRequest request) : base(callContext)
		{
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsPendingRequests.Increment();
			this.ItemId = request.ItemId;
			this.NeedToReclassify = request.NeedToReclassify;
			this.BodyOrSubjectChanged = request.BodyOrSubjectChanged;
			this.Recipients = request.Recipients;
			this.CustomizedStringsNeeded = request.CustomizedStringsNeeded;
			this.EventTrigger = request.EventTrigger;
			if (callContext != null && callContext.HttpContext != null && callContext.HttpContext.Request != null && callContext.HttpContext.Request.Headers != null)
			{
				this.CorrelationId = callContext.HttpContext.Request.Headers["X-OWA-CorrelationId"];
			}
			if (this.CorrelationId == null)
			{
				this.CorrelationId = Guid.NewGuid().ToString();
			}
			if (request.ClientSupportsScanResultData)
			{
				this.ClientSupportsScanResultData = true;
				this.ScanResultData = request.ScanResultData;
				return;
			}
			this.ClientSupportsScanResultData = false;
			this.ScanResultData = null;
		}

		protected override GetDlpPolicyTipsResponse InternalExecute()
		{
			PolicyTipRequestLogger policyTipRequestLogger = PolicyTipRequestLogger.CreateInstance(this.CorrelationId);
			policyTipRequestLogger.StartStage(LogStage.ReceiveRequest);
			Item item = null;
			GetDlpPolicyTipsResponse result;
			try
			{
				GetDlpPolicyTipsCommand.SetReceiveRequestLogData(policyTipRequestLogger, this.ItemId, this.NeedToReclassify, this.BodyOrSubjectChanged, this.Recipients, this.EventTrigger, this.CustomizedStringsNeeded, this.ClientSupportsScanResultData, this.ScanResultData);
				if (base.CallContext != null && base.CallContext.AccessingADUser != null && base.CallContext.AccessingADUser.OrganizationId != null)
				{
					this.OrganizationId = base.CallContext.AccessingADUser.OrganizationId;
					if (this.ItemId == null || string.IsNullOrEmpty(this.ItemId.GetId()))
					{
						GetDlpPolicyTipsResponse invalidStoreItemIdResponse = GetDlpPolicyTipsResponse.InvalidStoreItemIdResponse;
						this.TransitionToSendResponse(false, true, invalidStoreItemIdResponse, policyTipRequestLogger, true);
						result = invalidStoreItemIdResponse;
					}
					else if (this.ItemId.GetId().Equals(GetDlpPolicyTipsCommand.pingRequestItemId, StringComparison.OrdinalIgnoreCase))
					{
						policyTipRequestLogger.AppendData("Ping", "1");
						GetDlpPolicyTipsResponse responseToPingRequest = GetDlpPolicyTipsResponse.GetResponseToPingRequest();
						this.TransitionToSendResponse(true, false, responseToPingRequest, policyTipRequestLogger, false);
						result = responseToPingRequest;
					}
					else if (!GetDlpPolicyTipsCommand.AddItemToCurrentPending(this.ItemId.GetId()))
					{
						policyTipRequestLogger.AppendData("ItemAlreadyBeingProcessed", "1");
						GetDlpPolicyTipsResponse itemAlreadyBeingProcessedResponse = GetDlpPolicyTipsResponse.ItemAlreadyBeingProcessedResponse;
						this.TransitionToSendResponse(true, true, itemAlreadyBeingProcessedResponse, policyTipRequestLogger, false);
						result = itemAlreadyBeingProcessedResponse;
					}
					else
					{
						ShortList<string> recipients = GetDlpPolicyTipsCommand.ValidateAndGetEmailAddressStrings(this.Recipients, policyTipRequestLogger);
						IdAndSession idAndSession = null;
						try
						{
							idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(this.ItemId);
						}
						catch (InvalidStoreIdException exception)
						{
							policyTipRequestLogger.SetException(exception);
							GetDlpPolicyTipsResponse invalidStoreItemIdResponse2 = GetDlpPolicyTipsResponse.InvalidStoreItemIdResponse;
							this.TransitionToSendResponse(false, true, invalidStoreItemIdResponse2, policyTipRequestLogger, true);
							return invalidStoreItemIdResponse2;
						}
						catch (ObjectNotFoundException exception2)
						{
							policyTipRequestLogger.SetException(exception2);
							GetDlpPolicyTipsResponse invalidStoreItemIdResponse3 = GetDlpPolicyTipsResponse.InvalidStoreItemIdResponse;
							this.TransitionToSendResponse(false, true, invalidStoreItemIdResponse3, policyTipRequestLogger, true);
							return invalidStoreItemIdResponse3;
						}
						catch (AccessDeniedException exception3)
						{
							policyTipRequestLogger.SetException(exception3);
							GetDlpPolicyTipsResponse accessDeniedStoreItemIdResponse = GetDlpPolicyTipsResponse.AccessDeniedStoreItemIdResponse;
							this.TransitionToSendResponse(false, true, accessDeniedStoreItemIdResponse, policyTipRequestLogger, true);
							return accessDeniedStoreItemIdResponse;
						}
						policyTipRequestLogger.EndStageAndTransitionToStage(LogStage.LoadItem);
						List<DlpPolicyMatchDetail> list = null;
						bool flag = false;
						string empty = string.Empty;
						string empty2 = string.Empty;
						item = Item.Bind(idAndSession.Session, idAndSession.Id);
						ScanResultStorageProvider scanResultStorageProvider = null;
						if (this.ClientSupportsScanResultData)
						{
							try
							{
								scanResultStorageProvider = new ClientScanResultStorageProvider(this.ScanResultData, item);
								goto IL_274;
							}
							catch (ClientScanResultParseException exception4)
							{
								policyTipRequestLogger.SetException(exception4);
								GetDlpPolicyTipsResponse invalidClientScanResultResponse = GetDlpPolicyTipsResponse.InvalidClientScanResultResponse;
								this.TransitionToSendResponse(false, true, invalidClientScanResultResponse, policyTipRequestLogger, true);
								return invalidClientScanResultResponse;
							}
						}
						item.OpenAsReadWrite();
						scanResultStorageProvider = new StoreItemScanResultStorageProvider(item);
						IL_274:
						string empty3 = string.Empty;
						if (!GetDlpPolicyTipsCommand.IsSupportedStoreItemType(item, policyTipRequestLogger, out empty3))
						{
							GetDlpPolicyTipsResponse getDlpPolicyTipsResponse = new GetDlpPolicyTipsResponse(EvaluationResult.PermanentError);
							getDlpPolicyTipsResponse.DiagnosticData = string.Format("{0}:{1}", "UnSupportedStoreItemType", empty3);
							this.TransitionToSendResponse(false, true, getDlpPolicyTipsResponse, policyTipRequestLogger, true);
							result = getDlpPolicyTipsResponse;
						}
						else
						{
							if (item != null)
							{
								policyTipRequestLogger.AppendData("Subject", PolicyTipRequestLogger.MarkAsPII(item.GetValueOrDefault<string>(InternalSchema.Subject, string.Empty)));
							}
							string fromAddress = GetDlpPolicyTipsCommand.GetFromAddress(idAndSession, item, policyTipRequestLogger);
							if (string.IsNullOrEmpty(fromAddress))
							{
								policyTipRequestLogger.AppendData("NullFrom", "1");
								GetDlpPolicyTipsResponse getDlpPolicyTipsResponse2 = new GetDlpPolicyTipsResponse(EvaluationResult.PermanentError);
								getDlpPolicyTipsResponse2.DiagnosticData = "NullFrom";
								this.TransitionToSendResponse(false, true, getDlpPolicyTipsResponse2, policyTipRequestLogger, true);
								result = getDlpPolicyTipsResponse2;
							}
							else if (!GetDlpPolicyTipsCommand.HasContent(item, scanResultStorageProvider, policyTipRequestLogger))
							{
								policyTipRequestLogger.AppendData("NoContent", "1");
								GetDlpPolicyTipsResponse noContentResponse = GetDlpPolicyTipsResponse.NoContentResponse;
								this.TransitionToSendResponse(true, true, noContentResponse, policyTipRequestLogger, true);
								result = noContentResponse;
							}
							else
							{
								policyTipRequestLogger.EndStageAndTransitionToStage(LogStage.RefreshClassifications);
								policyTipRequestLogger.AppendData("BeforeRefreshClassifications", DiscoveredDataClassification.ToString(scanResultStorageProvider.GetDlpDetectedClassificationObjects()));
								if (this.NeedToReclassify)
								{
									scanResultStorageProvider.ResetAllClassifications();
								}
								else
								{
									if (this.BodyOrSubjectChanged)
									{
										scanResultStorageProvider.RefreshBodyClassifications();
									}
									scanResultStorageProvider.RefreshAttachmentClassifications();
								}
								policyTipRequestLogger.AppendData("AfterRefreshClassifications", DiscoveredDataClassification.ToString(scanResultStorageProvider.GetDlpDetectedClassificationObjects()));
								policyTipRequestLogger.EndStageAndTransitionToStage(LogStage.LoadRules);
								policyTipRequestLogger.AppendData("OrganizationId", this.OrganizationId.ToString());
								RuleCollection ruleCollection = GetDlpPolicyTipsCommand.LoadRules(this.OrganizationId);
								if (ruleCollection == null || ruleCollection.Count == 0)
								{
									policyTipRequestLogger.AppendData("RuleCount", "0");
									GetDlpPolicyTipsResponse noRulesResponse = GetDlpPolicyTipsResponse.NoRulesResponse;
									this.TransitionToSendResponse(true, true, noRulesResponse, policyTipRequestLogger, true);
									result = noRulesResponse;
								}
								else
								{
									policyTipRequestLogger.AppendData("RuleCount", ruleCollection.Count.ToString());
									policyTipRequestLogger.AppendData("RuleNames", GetDlpPolicyTipsCommand.GetRuleNamesForTracking(ruleCollection));
									policyTipRequestLogger.EndStageAndTransitionToStage(LogStage.EvaluateRules);
									ExecutionStatus executionStatus = GetDlpPolicyTipsCommand.RunRules(ruleCollection, scanResultStorageProvider, item, fromAddress, recipients, out list, out flag, out empty, out empty2, policyTipRequestLogger);
									policyTipRequestLogger.AppendData("ExecutionStatus", executionStatus.ToString());
									policyTipRequestLogger.AppendData("MatchResults", (list == null) ? string.Empty : DlpPolicyMatchDetail.ToString(list));
									policyTipRequestLogger.AppendData("RuleEvalLatency", empty);
									policyTipRequestLogger.AppendData("RuleEvalResult", empty2);
									PolicyTipCustomizedStrings policyTipCustomizedStrings = null;
									if (this.CustomizedStringsNeeded)
									{
										policyTipRequestLogger.EndStageAndTransitionToStage(LogStage.LoadCustomStrings);
										UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
										CultureInfo userCulture = userContext.UserCulture;
										policyTipRequestLogger.AppendData("CallersCulture", userCulture.Name);
										policyTipCustomizedStrings = ADUtils.GetPolicyTipStrings(this.OrganizationId, userCulture.Name);
										policyTipRequestLogger.AppendData("PolicyTipStrings", (policyTipCustomizedStrings == null) ? string.Empty : string.Format("Url:{0}/Notify:{1}/Override:{2}/Block:{3}", new object[]
										{
											policyTipCustomizedStrings.ComplianceURL ?? string.Empty,
											policyTipCustomizedStrings.PolicyTipMessageNotifyString ?? string.Empty,
											policyTipCustomizedStrings.PolicyTipMessageOverrideString ?? string.Empty,
											policyTipCustomizedStrings.PolicyTipMessageBlockString ?? string.Empty
										}));
									}
									GetDlpPolicyTipsResponse getDlpPolicyTipsResponse3 = new GetDlpPolicyTipsResponse(EvaluationResult.Success);
									if (this.ClientSupportsScanResultData)
									{
										getDlpPolicyTipsResponse3.ScanResultData = ((ClientScanResultStorageProvider)scanResultStorageProvider).GetScanResultData();
										getDlpPolicyTipsResponse3.DetectedClassificationIds = ((ClientScanResultStorageProvider)scanResultStorageProvider).GetDetectedClassificationIds();
									}
									else
									{
										item.Save(SaveMode.ResolveConflicts);
									}
									if (list != null)
									{
										getDlpPolicyTipsResponse3.Matches = list.ToArray();
									}
									if (flag)
									{
										getDlpPolicyTipsResponse3.OptimizationResult = OptimizationResult.NoContentMatch;
									}
									if (this.CustomizedStringsNeeded)
									{
										getDlpPolicyTipsResponse3.CustomizedStrings = policyTipCustomizedStrings;
									}
									this.TransitionToSendResponse(true, false, getDlpPolicyTipsResponse3, policyTipRequestLogger, true);
									result = getDlpPolicyTipsResponse3;
								}
							}
						}
					}
				}
				else
				{
					GetDlpPolicyTipsResponse nullOrganizationResponse = GetDlpPolicyTipsResponse.NullOrganizationResponse;
					this.TransitionToSendResponse(false, true, nullOrganizationResponse, policyTipRequestLogger, true);
					result = nullOrganizationResponse;
				}
			}
			catch (Exception ex)
			{
				policyTipRequestLogger.SetException(ex);
				GetDlpPolicyTipsResponse getDlpPolicyTipsResponse4;
				if (!GetDlpPolicyTipsCommand.CheckIfKnownExceptionAndUpdatePerfCounters(ex))
				{
					getDlpPolicyTipsResponse4 = new GetDlpPolicyTipsResponse(EvaluationResult.UnexpectedPermanentError);
					this.TransitionToSendResponse(false, false, getDlpPolicyTipsResponse4, policyTipRequestLogger, true);
					throw;
				}
				getDlpPolicyTipsResponse4 = new GetDlpPolicyTipsResponse(EvaluationResult.PermanentError);
				this.TransitionToSendResponse(false, false, getDlpPolicyTipsResponse4, policyTipRequestLogger, true);
				List<string> list2 = null;
				List<string> list3 = null;
				string text = null;
				PolicyTipProtocolLog.GetExceptionTypeAndDetails(ex, out list2, out list3, out text, false);
				getDlpPolicyTipsResponse4.DiagnosticData = string.Format("OuterExceptionType:{0}/OuterExceptionMessage:{1}/InnerExceptionType:{2}/InnerExceptionMessage:{3}/ExceptionChain:{4}.", new object[]
				{
					list2[0],
					list3[0],
					(list2.Count > 1) ? list2[list2.Count - 1] : string.Empty,
					(list2.Count > 1) ? list3[list3.Count - 1] : string.Empty,
					text
				});
				result = getDlpPolicyTipsResponse4;
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
			}
			return result;
		}

		private void TransitionToSendResponse(bool isSuccess, bool isSkip, GetDlpPolicyTipsResponse response, PolicyTipRequestLogger policyTipRequestLogger, bool removeItemFromPendingList = true)
		{
			policyTipRequestLogger.EndStageAndTransitionToStage(LogStage.SendResponse);
			policyTipRequestLogger.AppendData("IsSuccess", isSuccess ? "1" : "0");
			if (response != null)
			{
				policyTipRequestLogger.AppendData("EvaluationResult", response.EvaluationResult.ToString());
				policyTipRequestLogger.AppendData("OptimizationResult", response.OptimizationResult.ToString());
				policyTipRequestLogger.AppendData("ScanResultData", response.ScanResultData);
				policyTipRequestLogger.AppendData("DetectedClassificationIds", response.DetectedClassificationIds);
			}
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsPendingRequests.Decrement();
			PolicyTipPerfMon.IncrementTotalRequests();
			if (isSkip)
			{
				DlpPolicyTipsPerformanceCounters.DlpPolicyTipsSkippedRequestsInputError.Increment();
			}
			else if (isSuccess)
			{
				DlpPolicyTipsPerformanceCounters.DlpPolicyTipsSuccessfulRequests.Increment();
			}
			TimeSpan timeSpan = policyTipRequestLogger.EndStage();
			PolicyTipPerfMon.RecordPerRequestLatency(timeSpan);
			if (timeSpan > GetDlpPolicyTipsCommand.HighLatencyThreshold)
			{
				PolicyTipPerfMon.IncrementPercentHighLatency();
			}
			PolicyTipPerfMon.RefreshPerformanceCounters(null);
			if (removeItemFromPendingList)
			{
				lock (GetDlpPolicyTipsCommand.CurrentPendingItems)
				{
					GetDlpPolicyTipsCommand.CurrentPendingItems.Remove(this.ItemId.GetId());
				}
			}
		}

		private static bool AddItemToCurrentPending(string itemId)
		{
			bool result;
			lock (GetDlpPolicyTipsCommand.CurrentPendingItems)
			{
				if (GetDlpPolicyTipsCommand.CurrentPendingItems.Contains(itemId))
				{
					result = false;
				}
				else
				{
					GetDlpPolicyTipsCommand.CurrentPendingItems.Add(itemId);
					result = true;
				}
			}
			return result;
		}

		internal static bool HasContent(Item storeItem, ScanResultStorageProvider scanResultStorageProvider, PolicyTipRequestLogger policyTipRequestLogger)
		{
			if (storeItem != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Format("Body:{0}/", scanResultStorageProvider.NeedsClassificationScan() ? "0" : "1"));
				bool flag = storeItem.Body.Size != 0L;
				bool flag2 = false;
				AttachmentCollection attachmentCollection = storeItem.AttachmentCollection;
				if (attachmentCollection != null && attachmentCollection.Count > 0)
				{
					foreach (AttachmentHandle handle in attachmentCollection)
					{
						using (Attachment attachment = storeItem.AttachmentCollection.Open(handle))
						{
							stringBuilder.Append(string.Format("{0}:{1}:ExcludedFromDlp:{2}/", attachment.FileName, scanResultStorageProvider.NeedsClassificationScan(attachment) ? "0" : "1", ScanResultStorageProvider.IsExcludedFromDlp(attachment) ? "1" : "0"));
							if (attachment != null && !ScanResultStorageProvider.IsExcludedFromDlp(attachment) && attachment.Size != 0L)
							{
								flag2 = true;
							}
						}
					}
				}
				policyTipRequestLogger.AppendData("ItemClassifiedParts", stringBuilder.ToString());
				return flag || flag2;
			}
			return false;
		}

		internal static bool IsSupportedStoreItemType(Item storeItem, PolicyTipRequestLogger policyTipRequestLogger, out string typeName)
		{
			typeName = storeItem.GetType().UnderlyingSystemType.Name;
			if (!(storeItem is MessageItem) && !(storeItem is CalendarItem))
			{
				policyTipRequestLogger.AppendData("UnSupportedStoreItemType", typeName);
				return false;
			}
			policyTipRequestLogger.AppendData("StoreItemType", typeName);
			return true;
		}

		internal static string GetFromAddress(IdAndSession itemIdAndSession, Item storeItem, PolicyTipRequestLogger policyTipRequestLogger)
		{
			string text = ((MailboxSession)itemIdAndSession.Session).MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			Participant valueOrDefault = storeItem.GetValueOrDefault<Participant>(InternalSchema.From);
			string text2;
			if (valueOrDefault != null)
			{
				text2 = valueOrDefault.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress);
				if (string.IsNullOrEmpty(text2))
				{
					policyTipRequestLogger.AppendData("FromPropRoutingType", valueOrDefault.RoutingType);
					policyTipRequestLogger.AppendData("FromPropEmail", PolicyTipRequestLogger.MarkAsPII(valueOrDefault.EmailAddress));
				}
			}
			else
			{
				text2 = text;
			}
			policyTipRequestLogger.AppendData("FromAddress", PolicyTipRequestLogger.MarkAsPII(text2));
			policyTipRequestLogger.AppendData("MailboxOwnerAddress", PolicyTipRequestLogger.MarkAsPII(text));
			return text2;
		}

		internal static RuleCollection LoadRules(OrganizationId organizationId)
		{
			return ADUtils.GetPolicyTipRulesPerTenantSettings(organizationId).RuleCollection;
		}

		internal static ShortList<string> ValidateAndGetEmailAddressStrings(EmailAddressWrapper[] recipients, PolicyTipRequestLogger policyTipRequestLogger)
		{
			if (recipients == null || recipients.Length == 0)
			{
				return null;
			}
			ShortList<string> shortList = new ShortList<string>();
			StringBuilder stringBuilder = null;
			foreach (EmailAddressWrapper emailAddressWrapper in recipients)
			{
				if (emailAddressWrapper != null && !string.IsNullOrEmpty(emailAddressWrapper.EmailAddress) && SmtpAddress.IsValidSmtpAddress(emailAddressWrapper.EmailAddress))
				{
					shortList.Add(emailAddressWrapper.EmailAddress);
				}
				else
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					stringBuilder.Append((emailAddressWrapper == null || emailAddressWrapper.EmailAddress == null) ? "NullRecipientOrEmptyEmail" : PolicyTipRequestLogger.MarkAsPII(emailAddressWrapper.EmailAddress));
					stringBuilder.Append("|");
				}
			}
			if (stringBuilder != null)
			{
				policyTipRequestLogger.AppendData("InvalidRecipients", stringBuilder.ToString());
			}
			return shortList;
		}

		internal static ExecutionStatus RunRules(RuleCollection rules, ScanResultStorageProvider scanResultStorageProvider, Item storeItem, string fromAddress, ShortList<string> recipients, out List<DlpPolicyMatchDetail> dlpPolicyMatchDetails, out bool noContentMatch, out string ruleEvalLatency, out string ruleEvalResult, PolicyTipRequestLogger policyTipRequestLogger)
		{
			OwaRulesEvaluationContext owaRulesEvaluationContext = new OwaRulesEvaluationContext(rules, scanResultStorageProvider, storeItem, fromAddress, recipients, policyTipRequestLogger);
			OwaRulesEvaluator owaRulesEvaluator = new OwaRulesEvaluator(owaRulesEvaluationContext);
			owaRulesEvaluator.Run();
			dlpPolicyMatchDetails = owaRulesEvaluationContext.DlpPolicyMatchDetails;
			noContentMatch = owaRulesEvaluationContext.NoContentMatch;
			ruleEvalLatency = owaRulesEvaluationContext.RuleEvalLatency;
			ruleEvalResult = owaRulesEvaluationContext.RuleEvalLResult;
			return owaRulesEvaluationContext.ExecutionStatus;
		}

		internal static bool CheckIfKnownExceptionAndUpdatePerfCounters(Exception e)
		{
			if (e == null)
			{
				return false;
			}
			PolicyTipPerfMon.IncrementAllServerFailures();
			if (GetDlpPolicyTipsCommand.knownExceptionsFipsTimeOut.Any((Type exception) => exception.IsInstanceOfType(e)))
			{
				DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsFipsTimeOut.Increment();
				return true;
			}
			if (GetDlpPolicyTipsCommand.knownExceptionsFips.Any((Type exception) => exception.IsInstanceOfType(e)))
			{
				DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsFips.Increment();
				return true;
			}
			if (GetDlpPolicyTipsCommand.knownExceptionsEtr.Any((Type exception) => exception.IsInstanceOfType(e)))
			{
				DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsEtr.Increment();
				return true;
			}
			if (GetDlpPolicyTipsCommand.knownExceptionsAd.Any((Type exception) => exception.IsInstanceOfType(e)))
			{
				DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsAd.Increment();
				return true;
			}
			if (GetDlpPolicyTipsCommand.knownExceptionsXso.Any((Type exception) => exception.IsInstanceOfType(e)))
			{
				DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsXso.Increment();
				return true;
			}
			if (GetDlpPolicyTipsCommand.knownExceptionsOws.Any((Type exception) => exception.IsInstanceOfType(e)))
			{
				DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsOws.Increment();
				return true;
			}
			DlpPolicyTipsPerformanceCounters.DlpPolicyTipsFailedRequestsUnknownError.Increment();
			return false;
		}

		internal static string GetRuleNamesForTracking(RuleCollection ruleCollection)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Rule rule in ruleCollection)
			{
				stringBuilder.Append(rule.Name);
				stringBuilder.Append(",");
			}
			return stringBuilder.ToString();
		}

		internal static void SetReceiveRequestLogData(PolicyTipRequestLogger policyTipRequestLogger, BaseItemId itemId, bool needToReclassify, bool bodyOrSubjectChanged, EmailAddressWrapper[] recipients, EventTrigger eventTrigger, bool customizedStringsNeeded, bool clientSupportsScanResultData, string scanResultData)
		{
			policyTipRequestLogger.AppendData("NeedToReclassify", needToReclassify ? "1" : "0");
			policyTipRequestLogger.AppendData("ItemId", (itemId == null) ? string.Empty : (itemId.GetId() ?? string.Empty));
			policyTipRequestLogger.AppendData("BodyChanged", bodyOrSubjectChanged ? "1" : "0");
			string key = "Recipients";
			string value;
			if (recipients != null)
			{
				value = string.Join(";", from recipient in recipients
				select PolicyTipRequestLogger.MarkAsPII((recipient == null || recipient.EmailAddress == null) ? string.Empty : recipient.EmailAddress));
			}
			else
			{
				value = string.Empty;
			}
			policyTipRequestLogger.AppendData(key, value);
			policyTipRequestLogger.AppendData("EventTrigger", eventTrigger.ToString());
			policyTipRequestLogger.AppendData("CustomizedStringsNeeded", customizedStringsNeeded ? "1" : "0");
			policyTipRequestLogger.AppendData("ClientSupportsScanResultData", clientSupportsScanResultData ? "1" : "0");
			policyTipRequestLogger.AppendData("ScanResultData", scanResultData ?? string.Empty);
		}

		private const string ExceptionLogFormat = "OuterExceptionType:{0}/OuterExceptionMessage:{1}/InnerExceptionType:{2}/InnerExceptionMessage:{3}/ExceptionChain:{4}.";

		private static readonly HashSet<string> CurrentPendingItems = new HashSet<string>();

		private static readonly TimeSpan HighLatencyThreshold = new TimeSpan(0, 1, 0);

		private static readonly string pingRequestItemId = Guid.Empty.ToString();

		private OrganizationId OrganizationId;

		private readonly string CorrelationId = string.Empty;

		private static readonly List<Type> knownExceptionsFipsTimeOut = new List<Type>
		{
			typeof(FilteringServiceTimeoutException),
			typeof(ScanQueueTimeoutException),
			typeof(TimeoutException)
		};

		private static readonly List<Type> knownExceptionsFips = new List<Type>
		{
			typeof(FilteringServiceFailureException),
			typeof(ScannerCrashException),
			typeof(ResultsValidationException),
			typeof(ClassificationEngineInvalidOobConfigurationException),
			typeof(ClassificationEngineInvalidCustomConfigurationException),
			typeof(BiasException),
			typeof(QueueFullException),
			typeof(ConfigurationException),
			typeof(ServiceUnavailableException),
			typeof(ScanAbortedException),
			typeof(FilteringException)
		};

		private static readonly List<Type> knownExceptionsEtr = new List<Type>
		{
			typeof(TransportRuleException)
		};

		private static readonly List<Type> knownExceptionsAd = new List<Type>
		{
			typeof(TransientException),
			typeof(DataSourceOperationException),
			typeof(DataValidationException),
			typeof(InvalidDataResultException)
		};

		private static readonly List<Type> knownExceptionsXso = new List<Type>
		{
			typeof(StoragePermanentException),
			typeof(StorageTransientException)
		};

		private static readonly List<Type> knownExceptionsOws = new List<Type>
		{
			typeof(InvalidUserSidException),
			typeof(NonExistentMailboxException)
		};
	}
}
