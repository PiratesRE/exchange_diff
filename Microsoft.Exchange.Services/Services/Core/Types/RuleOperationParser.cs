using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class RuleOperationParser
	{
		internal List<RuleOperationError> RuleOperationErrorList { get; private set; }

		internal bool HasValidationError
		{
			get
			{
				return this.ruleValidationErrorList != null && 0 < this.ruleValidationErrorList.Count;
			}
		}

		internal bool HasOperationError
		{
			get
			{
				return this.RuleOperationErrorList != null && 0 < this.RuleOperationErrorList.Count;
			}
		}

		internal RuleOperationParser(int operationCount, CallContext callContext, MailboxSession mailboxSession, Rules serverRules, Trace tracer, int hashCode)
		{
			this.priorityHashSet = new HashSet<int>(operationCount);
			this.ruleIdHashSet = new HashSet<string>(operationCount);
			this.callContext = callContext;
			this.mailboxSession = mailboxSession;
			this.serverRules = serverRules;
			this.parsedRules = new Rules(serverRules.Folder, false);
			this.tracer = tracer;
			this.hashCode = hashCode;
			this.timeZone = (EWSSettings.RequestTimeZone ?? ExTimeZone.UtcTimeZone);
		}

		internal Rules Parse(RuleOperation[] ruleOperationArray)
		{
			for (int i = 0; i < ruleOperationArray.Length; i++)
			{
				RuleOperation ruleOperation = ruleOperationArray[i];
				ruleOperation.Execute(this, this.serverRules);
				if (this.ruleValidationErrorList != null && 0 < this.ruleValidationErrorList.Count)
				{
					RuleOperationError ruleOperationError = new RuleOperationError();
					ruleOperationError.OperationIndex = i;
					ruleOperationError.ValidationErrors = this.ruleValidationErrorList.ToArray();
					this.ruleValidationErrorList.Clear();
					if (this.RuleOperationErrorList == null)
					{
						this.RuleOperationErrorList = new List<RuleOperationError>(ruleOperationArray.Length);
					}
					this.RuleOperationErrorList.Add(ruleOperationError);
				}
			}
			this.MergeServerRules();
			return this.parsedRules;
		}

		internal void InsertParsedRule(Rule parsedServerRule)
		{
			for (int i = 0; i < this.parsedRules.Count; i++)
			{
				if (this.parsedRules[i].Sequence > parsedServerRule.Sequence)
				{
					this.parsedRules.Insert(i, parsedServerRule);
					return;
				}
			}
			this.parsedRules.Add(parsedServerRule);
		}

		internal void AddDeletedRule(Rule deletedRule)
		{
			if (this.deletedRuleList == null)
			{
				this.deletedRuleList = new List<Rule>();
			}
			this.deletedRuleList.Add(deletedRule);
		}

		internal bool ValidateRuleField(RuleOperationParser.RuleFieldParser ruleFieldParser, RuleValidationErrorCode ruleValidationErrorCode, LocalizedString ruleValidationErrorMessage, RuleFieldURI ruleFieldUri, string ruleFieldValue)
		{
			bool flag = ruleFieldParser();
			if (!flag)
			{
				RuleValidationError ruleValidationError = new RuleValidationError();
				ruleValidationError.ErrorCode = ruleValidationErrorCode;
				ruleValidationError.ErrorMessage = ruleValidationErrorMessage.ToString(this.callContext.ServerCulture);
				ruleValidationError.FieldURI = ruleFieldUri;
				ruleValidationError.FieldValue = ruleFieldValue;
				if (this.ruleValidationErrorList == null)
				{
					this.ruleValidationErrorList = new List<RuleValidationError>();
				}
				this.ruleValidationErrorList.Add(ruleValidationError);
				this.tracer.TraceDebug<RuleFieldURI, RuleValidationErrorCode, string>((long)this.hashCode, "Field={0}, Error={1}, Value={2}", ruleFieldUri, ruleValidationErrorCode, ruleFieldValue);
			}
			return flag;
		}

		internal void ParseRule(EwsRule ewsRule)
		{
			if (this.ValidateRuleField(() => !string.IsNullOrEmpty(ewsRule.DisplayName), RuleValidationErrorCode.EmptyValueFound, CoreResources.RuleErrorEmptyValueFound, RuleFieldURI.DisplayName, string.Empty))
			{
				this.ValidateRuleField(() => 256 >= ewsRule.DisplayName.Length, RuleValidationErrorCode.StringValueTooBig, CoreResources.RuleErrorStringValueTooBig(256), RuleFieldURI.DisplayName, ewsRule.DisplayName);
			}
			if (this.ValidateRuleField(() => 0 < ewsRule.Priority, RuleValidationErrorCode.InvalidValue, CoreResources.RuleErrorInvalidValue, RuleFieldURI.Priority, ewsRule.Priority.ToString()))
			{
				this.ValidateRuleField(delegate
				{
					bool flag = !this.priorityHashSet.Contains(ewsRule.Priority);
					if (flag)
					{
						this.priorityHashSet.Add(ewsRule.Priority);
						ewsRule.ServerRule.Sequence = ewsRule.Priority + 10 - 1;
					}
					return flag;
				}, RuleValidationErrorCode.DuplicatedPriority, CoreResources.RuleErrorDuplicatedPriority, RuleFieldURI.Priority, ewsRule.Priority.ToString());
			}
			this.ValidateRuleField(() => !ewsRule.IsNotSupportedSpecified, RuleValidationErrorCode.NotSettable, CoreResources.RuleErrorNotSettable, RuleFieldURI.IsNotSupported, ewsRule.IsNotSupported.ToString());
			if (!ewsRule.ServerRule.IsNotSupported)
			{
				ewsRule.ServerRule.Name = ewsRule.DisplayName;
				ewsRule.ServerRule.IsEnabled = ewsRule.IsEnabled;
				ewsRule.ServerRule.IsParameterInError = ewsRule.IsInError;
				this.ParseRulePredicates(ewsRule.Conditions, true, ewsRule.ServerRule, ewsRule.ServerRule.Conditions);
				this.ParseRulePredicates(ewsRule.Exceptions, false, ewsRule.ServerRule, ewsRule.ServerRule.Exceptions);
				if (this.ValidateRuleField(() => ewsRule.HasActions(), RuleValidationErrorCode.MissingAction, CoreResources.RuleErrorMissingAction, RuleFieldURI.Actions, string.Empty))
				{
					this.ParseRuleActions(ewsRule.Actions, ewsRule.ServerRule.Actions, ewsRule.ServerRule);
				}
			}
			else
			{
				this.ValidateRuleField(() => ewsRule.DisplayName.Equals(ewsRule.ServerRule.Name, StringComparison.OrdinalIgnoreCase), RuleValidationErrorCode.UnsupportedRule, CoreResources.RuleErrorUnsupportedRule, RuleFieldURI.DisplayName, ewsRule.DisplayName);
				this.ValidateRuleField(() => ewsRule.ServerRule.IsEnabled == ewsRule.IsEnabled, RuleValidationErrorCode.UnsupportedRule, CoreResources.RuleErrorUnsupportedRule, RuleFieldURI.IsEnabled, ewsRule.IsEnabled.ToString());
				this.ValidateRuleField(() => !ewsRule.IsInErrorSpecified, RuleValidationErrorCode.UnsupportedRule, CoreResources.RuleErrorUnsupportedRule, RuleFieldURI.IsInError, ewsRule.IsInError.ToString());
				this.ValidateRuleField(() => ewsRule.Conditions == null || !ewsRule.Conditions.SpecifiedPredicates(), RuleValidationErrorCode.UnsupportedRule, CoreResources.RuleErrorUnsupportedRule, RuleFieldURI.Conditions, "Specified");
				this.ValidateRuleField(() => ewsRule.Exceptions == null || !ewsRule.Exceptions.SpecifiedPredicates(), RuleValidationErrorCode.UnsupportedRule, CoreResources.RuleErrorUnsupportedRule, RuleFieldURI.Exceptions, "Specified");
				this.ValidateRuleField(() => ewsRule.Actions == null || !ewsRule.Actions.SpecifiedActions(), RuleValidationErrorCode.UnsupportedRule, CoreResources.RuleErrorUnsupportedRule, RuleFieldURI.Actions, "Specified");
			}
			if (!this.HasValidationError)
			{
				Rule.ProviderIdEnum providerId = ewsRule.ServerRule.ProviderId;
				if (providerId != Rule.ProviderIdEnum.Unknown)
				{
					string text = Rule.ProviderStringArray[(int)ewsRule.ServerRule.ProviderId];
					if (text != ewsRule.ServerRule.Provider)
					{
						ewsRule.ServerRule.Provider = text;
					}
				}
				if (ewsRule.ServerRule.IsDirty)
				{
					if (ewsRule.ServerRule.IsNotSupported)
					{
						ewsRule.ServerRule.SaveNotSupported();
						return;
					}
					ewsRule.ServerRule.Save();
				}
			}
		}

		internal Rule ParseRuleId(string ruleId, int ruleIndex)
		{
			Rule serverRule = null;
			if (this.ValidateRuleField(() => !string.IsNullOrEmpty(ruleId), RuleValidationErrorCode.MissingParameter, CoreResources.RuleErrorMissingParameter, RuleFieldURI.RuleId, ruleId))
			{
				RuleId serverRuleId = null;
				this.ValidateRuleField(delegate
				{
					bool flag = !this.ruleIdHashSet.Contains(ruleId);
					if (flag)
					{
						this.ruleIdHashSet.Add(ruleId);
					}
					return flag;
				}, RuleValidationErrorCode.DuplicatedOperationOnTheSameRule, CoreResources.RuleErrorDuplicatedOperationOnTheSameRule, RuleFieldURI.RuleId, ruleId);
				if (this.ValidateRuleField(delegate
				{
					bool result;
					try
					{
						serverRuleId = RuleId.Deserialize(ruleId, ruleIndex);
						result = true;
					}
					catch (ArgumentException arg)
					{
						this.tracer.TraceError<string, ArgumentException>((long)this.hashCode, "Deserializing rule ID {0} triggered argument exception {1}", ruleId, arg);
						result = false;
					}
					catch (InvalidDataException arg2)
					{
						this.tracer.TraceError<string, InvalidDataException>((long)this.hashCode, "Deserializing rule ID {0} triggered invalid data exception {1}", ruleId, arg2);
						result = false;
					}
					catch (CorruptDataException arg3)
					{
						this.tracer.TraceError<string, CorruptDataException>((long)this.hashCode, "Deserializing rule ID {0} triggered corrupt data exception {1}", ruleId, arg3);
						result = false;
					}
					return result;
				}, RuleValidationErrorCode.InvalidValue, CoreResources.RuleErrorInvalidValue, RuleFieldURI.RuleId, ruleId))
				{
					this.ValidateRuleField(delegate
					{
						bool result;
						try
						{
							serverRule = this.serverRules.FindByRuleId(serverRuleId);
							result = true;
						}
						catch (ObjectNotFoundException)
						{
							this.tracer.TraceError<string>((long)this.hashCode, "Rule with ID {0} was not found on server.", ruleId);
							result = false;
						}
						return result;
					}, RuleValidationErrorCode.RuleNotFound, CoreResources.RuleErrorRuleNotFound, RuleFieldURI.RuleId, ruleId);
				}
			}
			return serverRule;
		}

		private void ParseRulePredicates(RulePredicates rulePredicates, bool isCondition, Rule serverRule, IList<Condition> serverConditionList)
		{
			TypeDictionary<Condition> typeDictionary = new TypeDictionary<Condition>(serverConditionList);
			if (rulePredicates == null)
			{
				serverConditionList.Clear();
				return;
			}
			this.ParseStringArrayRulePredicateField<AssignedCategoriesCondition>(rulePredicates.Categories, isCondition ? RuleFieldURI.ConditionCategories : RuleFieldURI.ExceptionCategories, 255, typeDictionary, serverConditionList, serverRule, new RuleOperationParser.TwoArgumentsConditionCreator<AssignedCategoriesCondition, string[]>(AssignedCategoriesCondition.Create));
			this.ParseStringArrayRulePredicateField<ContainsBodyStringCondition>(rulePredicates.ContainsBodyStrings, isCondition ? RuleFieldURI.ConditionContainsBodyStrings : RuleFieldURI.ExceptionContainsBodyStrings, 255, typeDictionary, serverConditionList, serverRule, new RuleOperationParser.TwoArgumentsConditionCreator<ContainsBodyStringCondition, string[]>(ContainsBodyStringCondition.Create));
			this.ParseStringArrayRulePredicateField<ContainsHeaderStringCondition>(rulePredicates.ContainsHeaderStrings, isCondition ? RuleFieldURI.ConditionContainsHeaderStrings : RuleFieldURI.ExceptionContainsHeaderStrings, 255, typeDictionary, serverConditionList, serverRule, new RuleOperationParser.TwoArgumentsConditionCreator<ContainsHeaderStringCondition, string[]>(ContainsHeaderStringCondition.Create));
			this.ParseStringArrayRulePredicateField<ContainsRecipientStringCondition>(rulePredicates.ContainsRecipientStrings, isCondition ? RuleFieldURI.ConditionContainsRecipientStrings : RuleFieldURI.ExceptionContainsRecipientStrings, 255, typeDictionary, serverConditionList, serverRule, new RuleOperationParser.TwoArgumentsConditionCreator<ContainsRecipientStringCondition, string[]>(ContainsRecipientStringCondition.Create));
			this.ParseStringArrayRulePredicateField<ContainsSenderStringCondition>(rulePredicates.ContainsSenderStrings, isCondition ? RuleFieldURI.ConditionContainsSenderStrings : RuleFieldURI.ExceptionContainsSenderStrings, 255, typeDictionary, serverConditionList, serverRule, new RuleOperationParser.TwoArgumentsConditionCreator<ContainsSenderStringCondition, string[]>(ContainsSenderStringCondition.Create));
			this.ParseStringArrayRulePredicateField<ContainsSubjectOrBodyStringCondition>(rulePredicates.ContainsSubjectOrBodyStrings, isCondition ? RuleFieldURI.ConditionContainsSubjectOrBodyStrings : RuleFieldURI.ExceptionContainsSubjectOrBodyStrings, 255, typeDictionary, serverConditionList, serverRule, new RuleOperationParser.TwoArgumentsConditionCreator<ContainsSubjectOrBodyStringCondition, string[]>(ContainsSubjectOrBodyStringCondition.Create));
			this.ParseStringArrayRulePredicateField<ContainsSubjectStringCondition>(rulePredicates.ContainsSubjectStrings, isCondition ? RuleFieldURI.ConditionContainsSubjectStrings : RuleFieldURI.ExceptionContainsSubjectStrings, 255, typeDictionary, serverConditionList, serverRule, new RuleOperationParser.TwoArgumentsConditionCreator<ContainsSubjectStringCondition, string[]>(ContainsSubjectStringCondition.Create));
			this.UpdateServerConditionIfSpecified<FlaggedForActionCondition, string>(typeDictionary, serverConditionList, serverRule, rulePredicates.FlaggedForActionSpecified, new RuleOperationParser.TwoArgumentsConditionCreator<FlaggedForActionCondition, string>(FlaggedForActionCondition.Create), (rulePredicates.FlaggedForAction == FlaggedForAction.Any) ? RequestedAction.Any.ToString() : LocalizedDescriptionAttribute.FromEnum(FlaggedForActionCondition.RequestedActionType, (int)rulePredicates.FlaggedForAction, this.callContext.ClientCulture));
			this.ParseEmailAddressArrayRulePredicateField<FromRecipientsCondition>(rulePredicates.FromAddresses, isCondition ? RuleFieldURI.ConditionFromAddresses : RuleFieldURI.ExceptionFromAddresses, typeDictionary, serverConditionList, serverRule, new RuleOperationParser.TwoArgumentsConditionCreator<FromRecipientsCondition, Participant[]>(FromRecipientsCondition.Create));
			this.ParseConnectedAccountsRuleField(rulePredicates.FromConnectedAccounts, isCondition ? RuleFieldURI.ConditionFromConnectedAccounts : RuleFieldURI.ExceptionFromConnectedAccounts, serverRule, serverConditionList, typeDictionary);
			this.ParseBoolRulePredicateField<HasAttachmentCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.HasAttachmentsSpecified, rulePredicates.HasAttachments, new RuleOperationParser.OneArgumentCreator<HasAttachmentCondition>(HasAttachmentCondition.Create));
			this.ParseEnumRulePredicateField<MarkedAsImportanceCondition, Importance>(typeDictionary, serverConditionList, serverRule, rulePredicates.ImportanceSpecified, (Importance)rulePredicates.Importance, new RuleOperationParser.TwoArgumentsConditionCreator<MarkedAsImportanceCondition, Importance>(MarkedAsImportanceCondition.Create));
			this.ParseBoolRulePredicateField<ApprovalRequestCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsApprovalRequestSpecified, rulePredicates.IsApprovalRequest, new RuleOperationParser.OneArgumentCreator<ApprovalRequestCondition>(ApprovalRequestCondition.Create));
			this.ParseBoolRulePredicateField<AutomaticForwardCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsAutomaticForwardSpecified, rulePredicates.IsAutomaticForward, new RuleOperationParser.OneArgumentCreator<AutomaticForwardCondition>(AutomaticForwardCondition.Create));
			this.ParseBoolRulePredicateField<MarkedAsOofCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsAutomaticReplySpecified, rulePredicates.IsAutomaticReply, new RuleOperationParser.OneArgumentCreator<MarkedAsOofCondition>(MarkedAsOofCondition.Create));
			this.ParseBoolRulePredicateField<EncryptedCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsEncryptedSpecified, rulePredicates.IsEncrypted, new RuleOperationParser.OneArgumentCreator<EncryptedCondition>(EncryptedCondition.Create));
			this.ParseBoolRulePredicateField<MeetingMessageCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsMeetingRequestSpecified, rulePredicates.IsMeetingRequest, new RuleOperationParser.OneArgumentCreator<MeetingMessageCondition>(MeetingMessageCondition.Create));
			this.ParseBoolRulePredicateField<MeetingResponseCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsMeetingResponseSpecified, rulePredicates.IsMeetingResponse, new RuleOperationParser.OneArgumentCreator<MeetingResponseCondition>(MeetingResponseCondition.Create));
			this.ParseBoolRulePredicateField<NdrCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsNDRSpecified, rulePredicates.IsNDR, new RuleOperationParser.OneArgumentCreator<NdrCondition>(NdrCondition.Create));
			this.ParseBoolRulePredicateField<PermissionControlledCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsPermissionControlledSpecified, rulePredicates.IsPermissionControlled, new RuleOperationParser.OneArgumentCreator<PermissionControlledCondition>(PermissionControlledCondition.Create));
			this.ParseBoolRulePredicateField<ReadReceiptCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsReadReceiptSpecified, rulePredicates.IsReadReceipt, new RuleOperationParser.OneArgumentCreator<ReadReceiptCondition>(ReadReceiptCondition.Create));
			this.ParseBoolRulePredicateField<SignedCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsSignedSpecified, rulePredicates.IsSigned, new RuleOperationParser.OneArgumentCreator<SignedCondition>(SignedCondition.Create));
			this.ParseBoolRulePredicateField<VoicemailCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.IsVoicemailSpecified, rulePredicates.IsVoicemail, new RuleOperationParser.OneArgumentCreator<VoicemailCondition>(VoicemailCondition.Create));
			string[] array;
			if (this.TryParseStringArrayRuleField(rulePredicates.ItemClasses, isCondition ? RuleFieldURI.ConditionItemClasses : RuleFieldURI.ExceptionItemClasses, 128, out array))
			{
				FormsCondition formsCondition = typeDictionary.Lookup<FormsCondition>();
				serverConditionList.Remove(formsCondition);
				if (array != null)
				{
					formsCondition = FormsCondition.Create(ConditionType.FormsCondition, serverRule, array);
					serverConditionList.Add(formsCondition);
					if (serverRule.ProviderId < formsCondition.ProviderId)
					{
						serverRule.ProviderId = formsCondition.ProviderId;
					}
				}
			}
			this.ParseMessageClassificationsRuleField(rulePredicates.MessageClassifications, isCondition ? RuleFieldURI.ConditionMessageClassifications : RuleFieldURI.ExceptionMessageClassifications, serverRule, serverConditionList, typeDictionary);
			this.ParseBoolRulePredicateField<NotSentToMeCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.NotSentToMeSpecified, rulePredicates.NotSentToMe, new RuleOperationParser.OneArgumentCreator<NotSentToMeCondition>(NotSentToMeCondition.Create));
			this.ParseBoolRulePredicateField<SentCcMeCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.SentCcMeSpecified, rulePredicates.SentCcMe, new RuleOperationParser.OneArgumentCreator<SentCcMeCondition>(SentCcMeCondition.Create));
			this.ParseBoolRulePredicateField<SentOnlyToMeCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.SentOnlyToMeSpecified, rulePredicates.SentOnlyToMe, new RuleOperationParser.OneArgumentCreator<SentOnlyToMeCondition>(SentOnlyToMeCondition.Create));
			this.ParseEmailAddressArrayRulePredicateField<SentToRecipientsCondition>(rulePredicates.SentToAddresses, isCondition ? RuleFieldURI.ConditionSentToAddresses : RuleFieldURI.ExceptionSentToAddresses, typeDictionary, serverConditionList, serverRule, new RuleOperationParser.TwoArgumentsConditionCreator<SentToRecipientsCondition, Participant[]>(SentToRecipientsCondition.Create));
			this.ParseBoolRulePredicateField<SentToMeCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.SentToMeSpecified, rulePredicates.SentToMe, new RuleOperationParser.OneArgumentCreator<SentToMeCondition>(SentToMeCondition.Create));
			this.ParseBoolRulePredicateField<SentToOrCcMeCondition>(typeDictionary, serverConditionList, serverRule, rulePredicates.SentToOrCcMeSpecified, rulePredicates.SentToOrCcMe, new RuleOperationParser.OneArgumentCreator<SentToOrCcMeCondition>(SentToOrCcMeCondition.Create));
			this.ParseEnumRulePredicateField<MarkedAsSensitivityCondition, Sensitivity>(typeDictionary, serverConditionList, serverRule, rulePredicates.SensitivitySpecified, (Sensitivity)rulePredicates.Sensitivity, new RuleOperationParser.TwoArgumentsConditionCreator<MarkedAsSensitivityCondition, Sensitivity>(MarkedAsSensitivityCondition.Create));
			this.ParseDateRangeRuleField(rulePredicates.WithinDateRange, isCondition ? RuleFieldURI.ConditionWithinDateRange : RuleFieldURI.ExceptionWithinDateRange, typeDictionary, serverConditionList, serverRule);
			this.ParseSizeRangeRuleField(rulePredicates.WithinSizeRange, isCondition ? RuleFieldURI.ConditionWithinSizeRange : RuleFieldURI.ExceptionWithinSizeRange, typeDictionary, serverConditionList, serverRule);
		}

		private void ParseRuleActions(RuleActions ruleActions, IList<ActionBase> serverActionList, Rule serverRule)
		{
			TypeDictionary<ActionBase> actionMap = new TypeDictionary<ActionBase>(serverActionList);
			this.ParseStringArrayRuleActionField<AssignCategoriesAction>(ruleActions.AssignCategories, RuleFieldURI.ActionAssignCategories, 255, actionMap, serverActionList, serverRule, new RuleOperationParser.TwoArgumentsActionCreator<AssignCategoriesAction, string[]>(AssignCategoriesAction.Create));
			this.ParseFolderRuleField<CopyToFolderAction>(ruleActions.CopyToFolder, RuleFieldURI.ActionCopyToFolder, actionMap, serverActionList, serverRule, new RuleOperationParser.TwoArgumentsActionCreator<CopyToFolderAction, StoreObjectId>(CopyToFolderAction.Create));
			this.ParseBoolRuleActionField<DeleteAction>(actionMap, serverActionList, serverRule, ruleActions.DeleteSpecified, ruleActions.Delete, new RuleOperationParser.OneArgumentCreator<DeleteAction>(DeleteAction.Create));
			this.ParseEmailAddressArrayRuleActionField<ForwardAsAttachmentToRecipientsAction>(ruleActions.ForwardAsAttachmentToRecipients, RuleFieldURI.ActionForwardAsAttachmentToRecipients, actionMap, serverActionList, serverRule, new RuleOperationParser.TwoArgumentsActionCreator<ForwardAsAttachmentToRecipientsAction, Participant[]>(ForwardAsAttachmentToRecipientsAction.Create));
			this.ParseEmailAddressArrayRuleActionField<ForwardToRecipientsAction>(ruleActions.ForwardToRecipients, RuleFieldURI.ActionForwardToRecipients, actionMap, serverActionList, serverRule, new RuleOperationParser.TwoArgumentsActionCreator<ForwardToRecipientsAction, Participant[]>(ForwardToRecipientsAction.Create));
			this.ParseEnumRuleActionField<MarkImportanceAction, Importance>(actionMap, serverActionList, serverRule, ruleActions.MarkImportanceSpecified, (Importance)ruleActions.MarkImportance, new RuleOperationParser.TwoArgumentsActionCreator<MarkImportanceAction, Importance>(MarkImportanceAction.Create));
			this.ParseBoolRuleActionField<MarkAsReadAction>(actionMap, serverActionList, serverRule, ruleActions.MarkAsReadSpecified, ruleActions.MarkAsRead, new RuleOperationParser.OneArgumentCreator<MarkAsReadAction>(MarkAsReadAction.Create));
			this.ParseFolderRuleField<MoveToFolderAction>(ruleActions.MoveToFolder, RuleFieldURI.ActionMoveToFolder, actionMap, serverActionList, serverRule, new RuleOperationParser.TwoArgumentsActionCreator<MoveToFolderAction, StoreObjectId>(MoveToFolderAction.Create));
			this.ParseBoolRuleActionField<PermanentDeleteAction>(actionMap, serverActionList, serverRule, ruleActions.PermanentDeleteSpecified, ruleActions.PermanentDelete, new RuleOperationParser.OneArgumentCreator<PermanentDeleteAction>(PermanentDeleteAction.Create));
			this.ParseEmailAddressArrayRuleActionField<RedirectToRecipientsAction>(ruleActions.RedirectToRecipients, RuleFieldURI.ActionRedirectToRecipients, actionMap, serverActionList, serverRule, new RuleOperationParser.TwoArgumentsActionCreator<RedirectToRecipientsAction, Participant[]>(RedirectToRecipientsAction.Create));
			this.ParseSmsAlertRuleActionField(ruleActions.SendSMSAlertToRecipients, RuleFieldURI.ActionSendSMSAlertToRecipients, actionMap, serverActionList, serverRule);
			this.ParseMessageTemplateRuleActionField(ruleActions.ServerReplyWithMessage, RuleFieldURI.ActionServerReplyWithMessage, actionMap, serverActionList, serverRule);
			this.ParseBoolRuleActionField<StopProcessingAction>(actionMap, serverActionList, serverRule, ruleActions.StopProcessingRulesSpecified, ruleActions.StopProcessingRules, new RuleOperationParser.OneArgumentCreator<StopProcessingAction>(StopProcessingAction.Create));
		}

		private bool ValidateEmptyCollection<T>(ICollection<T> collection, RuleFieldURI ruleFieldUri)
		{
			return this.ValidateRuleField(() => 0 < collection.Count, RuleValidationErrorCode.EmptyValueFound, CoreResources.RuleErrorEmptyValueFound, ruleFieldUri, string.Empty);
		}

		private bool TryParseStringArrayRuleField(string[] ruleFieldStringArray, RuleFieldURI ruleFieldUri, int maxElementLength, out string[] outputStringArray)
		{
			outputStringArray = null;
			if (ruleFieldStringArray == null)
			{
				return true;
			}
			if (!this.ValidateEmptyCollection<string>(ruleFieldStringArray, ruleFieldUri))
			{
				return false;
			}
			bool flag = true;
			for (int i = 0; i < ruleFieldStringArray.Length; i++)
			{
				string stringElement = ruleFieldStringArray[i];
				bool flag2 = this.ValidateRuleField(() => !string.IsNullOrEmpty(stringElement), RuleValidationErrorCode.EmptyValueFound, CoreResources.RuleErrorEmptyValueFound, ruleFieldUri, stringElement);
				flag = (flag && flag2);
				if (flag2)
				{
					flag &= this.ValidateRuleField(() => maxElementLength > stringElement.Length, RuleValidationErrorCode.StringValueTooBig, CoreResources.RuleErrorStringValueTooBig(maxElementLength), ruleFieldUri, stringElement);
				}
			}
			if (flag)
			{
				outputStringArray = ruleFieldStringArray;
			}
			return flag;
		}

		private void ParseStringArrayRulePredicateField<T>(string[] ruleFieldStringArray, RuleFieldURI ruleFieldUri, int maxElementLength, TypeDictionary<Condition> conditionMap, IList<Condition> serverConditionList, Rule serverRule, RuleOperationParser.TwoArgumentsConditionCreator<T, string[]> conditionCreator) where T : Condition
		{
			string[] parsedValue;
			if (this.TryParseStringArrayRuleField(ruleFieldStringArray, ruleFieldUri, maxElementLength, out parsedValue))
			{
				this.UpdateServerCondition<T, string[]>(conditionMap, serverConditionList, serverRule, conditionCreator, parsedValue);
			}
		}

		private void UpdateServerCondition<T, V>(TypeDictionary<Condition> conditionMap, IList<Condition> serverConditionList, Rule serverRule, RuleOperationParser.TwoArgumentsConditionCreator<T, V> conditionCreator, V parsedValue) where T : Condition
		{
			T t = conditionMap.Lookup<T>();
			serverConditionList.Remove(t);
			if (parsedValue != null)
			{
				t = conditionCreator(serverRule, parsedValue);
				serverConditionList.Add(t);
				if (serverRule.ProviderId < t.ProviderId)
				{
					serverRule.ProviderId = t.ProviderId;
				}
			}
		}

		private void UpdateServerConditionIfSpecified<T, V>(TypeDictionary<Condition> conditionMap, IList<Condition> serverConditionList, Rule serverRule, bool fieldSpecified, RuleOperationParser.TwoArgumentsConditionCreator<T, V> conditionCreator, V fieldValue) where T : Condition
		{
			T t = conditionMap.Lookup<T>();
			serverConditionList.Remove(t);
			if (fieldSpecified && fieldValue != null)
			{
				t = conditionCreator(serverRule, fieldValue);
				serverConditionList.Add(t);
				if (serverRule.ProviderId < t.ProviderId)
				{
					serverRule.ProviderId = t.ProviderId;
				}
			}
		}

		private void ParseEnumRulePredicateField<T, V>(TypeDictionary<Condition> conditionMap, IList<Condition> serverConditionList, Rule serverRule, bool fieldSpecified, V fieldValue, RuleOperationParser.TwoArgumentsConditionCreator<T, V> conditionCreator) where T : Condition where V : struct
		{
			T t = conditionMap.Lookup<T>();
			serverConditionList.Remove(t);
			if (fieldSpecified)
			{
				t = conditionCreator(serverRule, fieldValue);
				serverConditionList.Add(t);
				if (serverRule.ProviderId < t.ProviderId)
				{
					serverRule.ProviderId = t.ProviderId;
				}
			}
		}

		private bool TryParseEmailAddressArrayRuleField(EmailAddressWrapper[] ruleFieldEmailAddressArray, RuleFieldURI ruleFieldUri, IList<Participant> serverRuleParticipantList, out Participant[] parsedParticipantArray)
		{
			RuleOperationParser.<>c__DisplayClass32 CS$<>8__locals1 = new RuleOperationParser.<>c__DisplayClass32();
			CS$<>8__locals1.<>4__this = this;
			parsedParticipantArray = null;
			if (ruleFieldEmailAddressArray == null)
			{
				return true;
			}
			if (!this.ValidateEmptyCollection<EmailAddressWrapper>(ruleFieldEmailAddressArray, ruleFieldUri))
			{
				return false;
			}
			if (serverRuleParticipantList != null && ruleFieldEmailAddressArray.Length == serverRuleParticipantList.Count)
			{
				HashSet<Participant> participantHashSet = new HashSet<Participant>(serverRuleParticipantList.Count);
				foreach (Participant participant2 in serverRuleParticipantList)
				{
					if (!participantHashSet.TryAdd(participant2))
					{
						this.tracer.TraceDebug<Participant>((long)this.hashCode, "serverRuleParticipantList contains duplicate Participant : {0}", participant2);
					}
				}
				parsedParticipantArray = ParticipantsAddressesConverter.ToParticipants(ruleFieldEmailAddressArray);
				if (parsedParticipantArray.All((Participant participant) => participantHashSet.Contains(participant)))
				{
					return false;
				}
			}
			CS$<>8__locals1.proxyAddressList = new List<ProxyAddress>(ruleFieldEmailAddressArray.Length);
			bool flag = true;
			for (int i = 0; i < ruleFieldEmailAddressArray.Length; i++)
			{
				EmailAddressWrapper emailAddress = ruleFieldEmailAddressArray[i];
				bool flag2 = this.ValidateRuleField(() => null != emailAddress, RuleValidationErrorCode.InvalidAddress, CoreResources.RuleErrorInvalidAddress, ruleFieldUri, string.Empty);
				flag = (flag && flag2);
				if (flag2)
				{
					flag &= this.ValidateRuleField(delegate
					{
						bool result;
						try
						{
							if (string.IsNullOrEmpty(emailAddress.RoutingType))
							{
								emailAddress.RoutingType = "SMTP";
							}
							ProxyAddress proxyAddress = ProxyAddress.Parse(emailAddress.RoutingType, emailAddress.EmailAddress);
							bool flag3 = !(proxyAddress is InvalidProxyAddress);
							if (flag3 && !(proxyAddress is SmtpProxyAddress))
							{
								CS$<>8__locals1.proxyAddressList.Add(proxyAddress);
							}
							result = flag3;
						}
						catch (ArgumentNullException)
						{
							CS$<>8__locals1.<>4__this.tracer.TraceError<string, string>((long)CS$<>8__locals1.<>4__this.hashCode, "An address field was null in email address RoutingType={0}, EmailAddress={1}", emailAddress.RoutingType, emailAddress.EmailAddress);
							result = false;
						}
						return result;
					}, RuleValidationErrorCode.InvalidAddress, CoreResources.RuleErrorInvalidAddress, ruleFieldUri, string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
					{
						emailAddress.RoutingType,
						emailAddress.EmailAddress
					}));
				}
			}
			if (CS$<>8__locals1.proxyAddressList.Count == 0)
			{
				if (flag && parsedParticipantArray == null)
				{
					parsedParticipantArray = ParticipantsAddressesConverter.ToParticipants(ruleFieldEmailAddressArray);
				}
				return flag;
			}
			CS$<>8__locals1.adResolveResultArray = null;
			ADOperationResult adOperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IRecipientSession adrecipientSession = CS$<>8__locals1.<>4__this.mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
				CS$<>8__locals1.adResolveResultArray = adrecipientSession.FindByProxyAddresses(CS$<>8__locals1.proxyAddressList.ToArray(), null);
			});
			flag &= this.ValidateAdOperationResult(adOperationResult, ruleFieldUri);
			if (!flag)
			{
				return flag;
			}
			string[] array = new string[CS$<>8__locals1.proxyAddressList.Count];
			for (int j = 0; j < CS$<>8__locals1.proxyAddressList.Count; j++)
			{
				array[j] = CS$<>8__locals1.proxyAddressList[j].ToString();
			}
			flag &= this.ValidateRuleField(() => 0 < CS$<>8__locals1.adResolveResultArray.Length, RuleValidationErrorCode.RecipientDoesNotExist, CoreResources.RuleErrorRecipientDoesNotExist, ruleFieldUri, string.Join(",", array));
			int addressIndex;
			for (addressIndex = 0; addressIndex < CS$<>8__locals1.adResolveResultArray.Length; addressIndex++)
			{
				flag &= this.ValidateRuleField(() => null == CS$<>8__locals1.adResolveResultArray[addressIndex].Error, RuleValidationErrorCode.RecipientDoesNotExist, CoreResources.RuleErrorRecipientDoesNotExist, ruleFieldUri, CS$<>8__locals1.proxyAddressList[addressIndex].ToString());
			}
			if (flag && parsedParticipantArray == null)
			{
				parsedParticipantArray = ParticipantsAddressesConverter.ToParticipants(ruleFieldEmailAddressArray);
			}
			return flag;
		}

		private void ParseEmailAddressArrayRulePredicateField<T>(EmailAddressWrapper[] ruleFieldEmailAddressArray, RuleFieldURI ruleFieldUri, TypeDictionary<Condition> conditionMap, IList<Condition> serverConditionList, Rule serverRule, RuleOperationParser.TwoArgumentsConditionCreator<T, Participant[]> conditionCreator) where T : RecipientCondition
		{
			T t = conditionMap.Lookup<T>();
			Participant[] parsedValue;
			if (this.TryParseEmailAddressArrayRuleField(ruleFieldEmailAddressArray, ruleFieldUri, (t == null) ? null : t.Participants, out parsedValue))
			{
				this.UpdateServerCondition<T, Participant[]>(conditionMap, serverConditionList, serverRule, conditionCreator, parsedValue);
			}
		}

		private void ParseBoolRulePredicateField<T>(TypeDictionary<Condition> conditionMap, IList<Condition> serverConditionList, Rule serverRule, bool fieldSpecified, bool fieldValue, RuleOperationParser.OneArgumentCreator<T> conditionCreator) where T : Condition
		{
			T t = conditionMap.Lookup<T>();
			serverConditionList.Remove(t);
			if (fieldSpecified && fieldValue)
			{
				t = conditionCreator(serverRule);
				serverConditionList.Add(t);
				if (serverRule.ProviderId < t.ProviderId)
				{
					serverRule.ProviderId = t.ProviderId;
				}
			}
		}

		private void ParseBoolRuleActionField<T>(TypeDictionary<ActionBase> actionMap, IList<ActionBase> serverActionList, Rule serverRule, bool fieldSpecified, bool fieldValue, RuleOperationParser.OneArgumentCreator<T> actionCreator) where T : ActionBase
		{
			T t = actionMap.Lookup<T>();
			serverActionList.Remove(t);
			if (fieldSpecified && fieldValue)
			{
				t = actionCreator(serverRule);
				serverActionList.Add(t);
				if (serverRule.ProviderId < t.ProviderId)
				{
					serverRule.ProviderId = t.ProviderId;
				}
			}
		}

		private void ParseStringArrayRuleActionField<T>(string[] ruleFieldStringArray, RuleFieldURI ruleFieldUri, int maxElementLength, TypeDictionary<ActionBase> actionMap, IList<ActionBase> serverActionList, Rule serverRule, RuleOperationParser.TwoArgumentsActionCreator<T, string[]> actionCreator) where T : ActionBase
		{
			string[] parsedValue;
			if (this.TryParseStringArrayRuleField(ruleFieldStringArray, ruleFieldUri, maxElementLength, out parsedValue))
			{
				this.UpdateServerAction<T, string[]>(actionMap, serverActionList, serverRule, actionCreator, parsedValue);
			}
		}

		private void UpdateServerAction<T, V>(TypeDictionary<ActionBase> actionMap, IList<ActionBase> serverActionList, Rule serverRule, RuleOperationParser.TwoArgumentsActionCreator<T, V> actionCreator, V parsedValue) where T : ActionBase
		{
			T t = actionMap.Lookup<T>();
			serverActionList.Remove(t);
			if (parsedValue != null)
			{
				t = actionCreator(parsedValue, serverRule);
				serverActionList.Add(t);
				if (serverRule.ProviderId < t.ProviderId)
				{
					serverRule.ProviderId = t.ProviderId;
				}
			}
		}

		private void ParseEmailAddressArrayRuleActionField<T>(EmailAddressWrapper[] ruleFieldEmailAddressArray, RuleFieldURI ruleFieldUri, TypeDictionary<ActionBase> actionMap, IList<ActionBase> serverActionList, Rule serverRule, RuleOperationParser.TwoArgumentsActionCreator<T, Participant[]> actionCreator) where T : RecipientAction
		{
			T t = actionMap.Lookup<T>();
			Participant[] parsedValue;
			if (this.TryParseEmailAddressArrayRuleField(ruleFieldEmailAddressArray, ruleFieldUri, (t == null) ? null : t.Participants, out parsedValue))
			{
				this.UpdateServerAction<T, Participant[]>(actionMap, serverActionList, serverRule, actionCreator, parsedValue);
			}
		}

		private void ParseEnumRuleActionField<T, V>(TypeDictionary<ActionBase> actionMap, IList<ActionBase> serverActionList, Rule serverRule, bool fieldSpecified, V fieldValue, RuleOperationParser.TwoArgumentsActionCreator<T, V> actionCreator) where T : ActionBase where V : struct
		{
			T t = actionMap.Lookup<T>();
			serverActionList.Remove(t);
			if (fieldSpecified)
			{
				t = actionCreator(fieldValue, serverRule);
				serverActionList.Add(t);
				if (serverRule.ProviderId < t.ProviderId)
				{
					serverRule.ProviderId = t.ProviderId;
				}
			}
		}

		private bool ValidateAdOperationResult(ADOperationResult adOperationResult, RuleFieldURI ruleFieldUri)
		{
			return this.ValidateRuleField(() => adOperationResult.Succeeded, RuleValidationErrorCode.ADOperationFailure, CoreResources.ErrorADOperation, ruleFieldUri, (adOperationResult.Exception == null) ? string.Empty : adOperationResult.Exception.GetType().Name);
		}

		private bool ValidateStoreOperation(RuleOperationParser.StoreOperation storeOperation, RuleFieldURI ruleFieldUri, string ruleFieldValue)
		{
			bool result;
			try
			{
				storeOperation();
				result = true;
			}
			catch (LocalizedException ex)
			{
				this.tracer.TraceError<LocalizedException>((long)this.hashCode, "Store operation resulted in exception {0}", ex);
				result = this.ValidateRuleField(() => false, RuleValidationErrorCode.InvalidValue, ex.LocalizedString, ruleFieldUri, ruleFieldValue);
			}
			return result;
		}

		private bool TryParseGuidRuleField(string[] guidStringArray, RuleFieldURI ruleFieldUri, out List<Guid> parsedGuidList)
		{
			parsedGuidList = null;
			if (guidStringArray == null)
			{
				return true;
			}
			if (!this.ValidateEmptyCollection<string>(guidStringArray, ruleFieldUri))
			{
				return false;
			}
			bool flag = true;
			List<Guid> guidList = new List<Guid>(guidStringArray.Length);
			for (int i = 0; i < guidStringArray.Length; i++)
			{
				string guidString = guidStringArray[i];
				if (this.ValidateRuleField(() => !string.IsNullOrEmpty(guidString), RuleValidationErrorCode.EmptyValueFound, CoreResources.RuleErrorEmptyValueFound, ruleFieldUri, guidString))
				{
					flag &= this.ValidateRuleField(delegate
					{
						Guid item;
						bool flag2 = GuidHelper.TryParseGuid(guidString, out item);
						if (flag2)
						{
							guidList.Add(item);
						}
						return flag2;
					}, RuleValidationErrorCode.InvalidValue, CoreResources.RuleErrorInvalidValue, ruleFieldUri, guidString);
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				parsedGuidList = guidList;
			}
			return flag;
		}

		private void ParseConnectedAccountsRuleField(string[] connectedAccountsArray, RuleFieldURI ruleFieldUri, Rule serverRule, IList<Condition> serverConditionList, TypeDictionary<Condition> conditionMap)
		{
			FromSubscriptionCondition fromSubscriptionCondition = conditionMap.Lookup<FromSubscriptionCondition>();
			List<Guid> list;
			bool flag = this.TryParseGuidRuleField(connectedAccountsArray, ruleFieldUri, out list);
			if (list == null)
			{
				if (flag)
				{
					serverConditionList.Remove(fromSubscriptionCondition);
				}
				return;
			}
			if (fromSubscriptionCondition != null && fromSubscriptionCondition.Guids != null && list.Count == fromSubscriptionCondition.Guids.Length)
			{
				HashSet<Guid> subscriptionIdHashSet = new HashSet<Guid>(fromSubscriptionCondition.Guids);
				if (list.All((Guid subscriptionId) => subscriptionIdHashSet.Contains(subscriptionId)))
				{
					return;
				}
			}
			string[] array = new string[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = list[i].ToString();
			}
			List<AggregationSubscription> allSubscriptionList = null;
			flag &= this.ValidateStoreOperation(delegate
			{
				allSubscriptionList = SubscriptionManager.GetAllSubscriptions(this.mailboxSession, AggregationSubscriptionType.All);
			}, ruleFieldUri, string.Join(",", array));
			if (!flag)
			{
				return;
			}
			HashSet<Guid> allSubscriptionIdHashSet = new HashSet<Guid>(allSubscriptionList.Count);
			foreach (AggregationSubscription aggregationSubscription in allSubscriptionList)
			{
				allSubscriptionIdHashSet.TryAdd(aggregationSubscription.SubscriptionGuid);
			}
			using (List<Guid>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Guid subscriptionId = enumerator2.Current;
					bool flag2 = flag;
					RuleOperationParser.RuleFieldParser ruleFieldParser = () => allSubscriptionIdHashSet.Contains(subscriptionId);
					RuleValidationErrorCode ruleValidationErrorCode = RuleValidationErrorCode.ConnectedAccountNotFound;
					LocalizedString errorSubscriptionNotFound = CoreResources.ErrorSubscriptionNotFound;
					Guid subscriptionId2 = subscriptionId;
					flag = (flag2 & this.ValidateRuleField(ruleFieldParser, ruleValidationErrorCode, errorSubscriptionNotFound, ruleFieldUri, subscriptionId2.ToString()));
				}
			}
			if (flag)
			{
				serverConditionList.Remove(fromSubscriptionCondition);
				fromSubscriptionCondition = FromSubscriptionCondition.Create(serverRule, list.ToArray());
				serverConditionList.Add(fromSubscriptionCondition);
				if (serverRule.ProviderId < fromSubscriptionCondition.ProviderId)
				{
					serverRule.ProviderId = fromSubscriptionCondition.ProviderId;
				}
			}
		}

		private void ParseMessageClassificationsRuleField(string[] messageClassificationArray, RuleFieldURI ruleFieldUri, Rule serverRule, IList<Condition> serverConditionList, TypeDictionary<Condition> conditionMap)
		{
			MessageClassificationCondition messageClassificationCondition = conditionMap.Lookup<MessageClassificationCondition>();
			List<Guid> list;
			bool flag = this.TryParseGuidRuleField(messageClassificationArray, ruleFieldUri, out list);
			if (list == null)
			{
				if (flag)
				{
					serverConditionList.Remove(messageClassificationCondition);
				}
				return;
			}
			if (messageClassificationCondition != null && messageClassificationCondition.Text != null && list.Count == messageClassificationCondition.Text.Length)
			{
				try
				{
					Guid[] array = new Guid[messageClassificationCondition.Text.Length];
					for (int i = 0; i < messageClassificationCondition.Text.Length; i++)
					{
						array[i] = new Guid(messageClassificationCondition.Text[i]);
					}
					HashSet<Guid> classificationIdHashSet = new HashSet<Guid>(array);
					if (list.All((Guid classificationId) => classificationIdHashSet.Contains(classificationId)))
					{
						return;
					}
				}
				catch (FormatException arg)
				{
					this.tracer.TraceError<string[], FormatException>((long)this.hashCode, "At least one of the GUIDs in saved rule {0} triggered format exception {1}", messageClassificationCondition.Text, arg);
				}
				catch (OverflowException arg2)
				{
					this.tracer.TraceError<string[], OverflowException>((long)this.hashCode, "At least one of the GUIDs in saved rule {0} triggered overflow exception {1}", messageClassificationCondition.Text, arg2);
				}
			}
			List<ComparisonFilter> list2 = new List<ComparisonFilter>(list.Count);
			foreach (Guid guid3 in list)
			{
				ComparisonFilter item = new ComparisonFilter(ComparisonOperator.Equal, ClassificationSchema.ClassificationID, guid3);
				list2.Add(item);
			}
			OrFilter filter = new OrFilter(list2.ToArray());
			if (this.adConfigSession == null)
			{
				this.adConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, this.mailboxSession.GetADSessionSettings(), 2048, "ParseMessageClassificationsRuleField", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\Types\\Rule\\RuleOperationParser.cs");
				if (SharedConfiguration.IsDehydratedConfiguration(this.adConfigSession))
				{
					this.adConfigSession = SharedConfiguration.CreateScopedToSharedConfigADSession(this.adConfigSession.SessionSettings.CurrentOrganizationId);
				}
			}
			ADPagedReader<MessageClassification> adReader = this.adConfigSession.FindPaged<MessageClassification>(this.adConfigSession.SessionSettings.CurrentOrganizationId.ConfigurationUnit, QueryScope.SubTree, filter, null, 0);
			HashSet<Guid> serverGuidHashSet = null;
			ADOperationResult adOperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				serverGuidHashSet = new HashSet<Guid>((from messageClassification in adReader
				select messageClassification.ClassificationID).ToArray<Guid>());
			});
			if (this.ValidateAdOperationResult(adOperationResult, ruleFieldUri))
			{
				using (List<Guid>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Guid guid = enumerator2.Current;
						bool flag2 = flag;
						RuleOperationParser.RuleFieldParser ruleFieldParser = () => serverGuidHashSet.Contains(guid);
						RuleValidationErrorCode ruleValidationErrorCode = RuleValidationErrorCode.MessageClassificationNotFound;
						LocalizedString ruleErrorMessageClassificationNotFound = CoreResources.RuleErrorMessageClassificationNotFound;
						Guid guid2 = guid;
						flag = (flag2 & this.ValidateRuleField(ruleFieldParser, ruleValidationErrorCode, ruleErrorMessageClassificationNotFound, ruleFieldUri, guid2.ToString()));
					}
				}
				if (flag)
				{
					serverConditionList.Remove(messageClassificationCondition);
					messageClassificationCondition = MessageClassificationCondition.Create(serverRule, messageClassificationArray);
					serverConditionList.Add(messageClassificationCondition);
					if (serverRule.ProviderId < messageClassificationCondition.ProviderId)
					{
						serverRule.ProviderId = messageClassificationCondition.ProviderId;
					}
				}
				return;
			}
		}

		private void ParseDateRangeRuleField(RulePredicateDateRange ruleFieldDateRange, RuleFieldURI ruleFieldUri, TypeDictionary<Condition> conditionMap, IList<Condition> serverConditionList, Rule serverRule)
		{
			WithinDateRangeCondition item = conditionMap.Lookup<WithinDateRangeCondition>();
			if (ruleFieldDateRange == null)
			{
				serverConditionList.Remove(item);
				return;
			}
			if (!this.ValidateRuleField(() => (ruleFieldDateRange.StartDateTimeSpecified && !string.IsNullOrEmpty(ruleFieldDateRange.StartDateTime)) || (ruleFieldDateRange.EndDateTimeSpecified && !string.IsNullOrEmpty(ruleFieldDateRange.EndDateTime)), RuleValidationErrorCode.MissingRangeValue, CoreResources.RuleErrorMissingRangeValue, ruleFieldUri, string.Empty))
			{
				return;
			}
			bool flag = true;
			ExDateTime? startDateTime = null;
			if (ruleFieldDateRange.StartDateTimeSpecified && !string.IsNullOrEmpty(ruleFieldDateRange.StartDateTime))
			{
				flag &= this.ValidateRuleField(delegate
				{
					bool result;
					try
					{
						startDateTime = new ExDateTime?(ExDateTimeConverter.ParseTimeZoneRelated(ruleFieldDateRange.StartDateTime, this.timeZone));
						result = true;
					}
					catch (InvalidValueForPropertyException arg)
					{
						this.tracer.TraceError<string, InvalidValueForPropertyException>((long)this.hashCode, "ExDateTime parsing failed for start date time {0} with invalid value for property exception {1}", ruleFieldDateRange.StartDateTime, arg);
						result = false;
					}
					return result;
				}, RuleValidationErrorCode.InvalidValue, CoreResources.RuleErrorInvalidValue, ruleFieldUri, ruleFieldDateRange.StartDateTime);
			}
			ExDateTime? endDateTime = null;
			if (ruleFieldDateRange.EndDateTimeSpecified && !string.IsNullOrEmpty(ruleFieldDateRange.EndDateTime))
			{
				flag &= this.ValidateRuleField(delegate
				{
					bool result;
					try
					{
						endDateTime = new ExDateTime?(ExDateTimeConverter.ParseTimeZoneRelated(ruleFieldDateRange.EndDateTime, this.timeZone));
						result = true;
					}
					catch (InvalidValueForPropertyException arg)
					{
						this.tracer.TraceError<string, InvalidValueForPropertyException>((long)this.hashCode, "ExDateTime parsing failed for end date time {0} with invalid value for property exception {1}", ruleFieldDateRange.StartDateTime, arg);
						result = false;
					}
					return result;
				}, RuleValidationErrorCode.InvalidValue, CoreResources.RuleErrorInvalidValue, ruleFieldUri, ruleFieldDateRange.EndDateTime);
			}
			if (startDateTime != null && endDateTime != null)
			{
				flag &= this.ValidateRuleField(() => startDateTime.Value <= endDateTime.Value, RuleValidationErrorCode.InvalidDateRange, CoreResources.RuleErrorInvalidDateRange(ruleFieldDateRange.StartDateTime, ruleFieldDateRange.EndDateTime), ruleFieldUri, string.Format(CultureInfo.InvariantCulture, "{0},{1}", new object[]
				{
					ruleFieldDateRange.StartDateTime,
					ruleFieldDateRange.EndDateTime
				}));
			}
			if (flag)
			{
				serverConditionList.Remove(item);
				item = WithinDateRangeCondition.Create(serverRule, startDateTime, endDateTime);
				serverConditionList.Add(item);
			}
		}

		private void ParseSizeRangeRuleField(RulePredicateSizeRange ruleFieldSizeRange, RuleFieldURI ruleFieldUri, TypeDictionary<Condition> conditionMap, IList<Condition> serverConditionList, Rule serverRule)
		{
			WithinSizeRangeCondition item = conditionMap.Lookup<WithinSizeRangeCondition>();
			if (ruleFieldSizeRange == null)
			{
				serverConditionList.Remove(item);
				return;
			}
			if (!this.ValidateRuleField(() => ruleFieldSizeRange.MinimumSizeSpecified || ruleFieldSizeRange.MaximumSizeSpecified, RuleValidationErrorCode.MissingRangeValue, CoreResources.RuleErrorMissingRangeValue, ruleFieldUri, string.Empty))
			{
				return;
			}
			bool flag = true;
			int? minimumSize = null;
			if (ruleFieldSizeRange.MinimumSizeSpecified)
			{
				minimumSize = new int?(ruleFieldSizeRange.MinimumSize);
				flag &= this.ValidateRuleField(() => 0 <= minimumSize.Value, RuleValidationErrorCode.SizeLessThanZero, CoreResources.RuleErrorSizeLessThanZero, ruleFieldUri, ruleFieldSizeRange.MinimumSize.ToString());
				flag &= this.ValidateRuleField(() => 2097151 >= minimumSize.Value, RuleValidationErrorCode.InvalidValue, CoreResources.RuleErrorInvalidValue, ruleFieldUri, ruleFieldSizeRange.MinimumSize.ToString());
			}
			int? maximumSize = null;
			if (ruleFieldSizeRange.MaximumSizeSpecified)
			{
				maximumSize = new int?(ruleFieldSizeRange.MaximumSize);
				flag &= this.ValidateRuleField(() => 0 <= maximumSize.Value, RuleValidationErrorCode.SizeLessThanZero, CoreResources.RuleErrorSizeLessThanZero, ruleFieldUri, ruleFieldSizeRange.MaximumSize.ToString());
				flag &= this.ValidateRuleField(() => 2097151 >= maximumSize.Value, RuleValidationErrorCode.InvalidValue, CoreResources.RuleErrorInvalidValue, ruleFieldUri, ruleFieldSizeRange.MaximumSize.ToString());
			}
			if (ruleFieldSizeRange.MinimumSizeSpecified && ruleFieldSizeRange.MaximumSizeSpecified)
			{
				flag &= this.ValidateRuleField(() => ruleFieldSizeRange.MinimumSize <= ruleFieldSizeRange.MaximumSize, RuleValidationErrorCode.InvalidSizeRange, CoreResources.RuleErrorInvalidSizeRange(ruleFieldSizeRange.MinimumSize, ruleFieldSizeRange.MaximumSize), ruleFieldUri, string.Format(CultureInfo.InvariantCulture, "{0},{1}", new object[]
				{
					ruleFieldSizeRange.MinimumSize,
					ruleFieldSizeRange.MaximumSize
				}));
			}
			if (flag)
			{
				serverConditionList.Remove(item);
				item = WithinSizeRangeCondition.Create(serverRule, minimumSize, maximumSize);
				serverConditionList.Add(item);
			}
		}

		private void ParseFolderRuleField<T>(TargetFolderId ruleFieldTargetFolderId, RuleFieldURI ruleFieldUri, TypeDictionary<ActionBase> actionMap, IList<ActionBase> serverActionList, Rule serverRule, RuleOperationParser.TwoArgumentsActionCreator<T, StoreObjectId> actionCreator) where T : IdAction
		{
			T t = actionMap.Lookup<T>();
			if (ruleFieldTargetFolderId == null)
			{
				serverActionList.Remove(t);
				return;
			}
			if (!this.ValidateRuleField(() => null != ruleFieldTargetFolderId.BaseFolderId, RuleValidationErrorCode.EmptyValueFound, CoreResources.RuleErrorEmptyValueFound, ruleFieldUri, string.Empty))
			{
				return;
			}
			FolderId ruleFieldFolderId = ruleFieldTargetFolderId.BaseFolderId as FolderId;
			if (t != null && t.Id != null && ruleFieldFolderId != null)
			{
				FolderId folderId = IdConverter.ConvertStoreFolderIdToFolderId(t.Id, this.mailboxSession);
				if (ruleFieldFolderId.Id == folderId.Id)
				{
					return;
				}
			}
			IdAndSession idAndSession = null;
			bool flag;
			if (ruleFieldFolderId == null)
			{
				DistinguishedFolderId distinguishedFolderId = (DistinguishedFolderId)ruleFieldTargetFolderId.BaseFolderId;
				flag = this.ValidateStoreOperation(delegate
				{
					idAndSession = IdConverter.ConvertDistinguishedFolderId(this.callContext, distinguishedFolderId.Id.ToString(), distinguishedFolderId.ChangeKey, (distinguishedFolderId.Mailbox == null) ? null : distinguishedFolderId.Mailbox.EmailAddress);
				}, ruleFieldUri, distinguishedFolderId.Id.ToString());
			}
			else
			{
				flag = this.ValidateStoreOperation(delegate
				{
					IdHeaderInformation headerInformation = IdConverter.ConvertFromConcatenatedId(ruleFieldFolderId.Id, BasicTypes.Folder, null, false);
					idAndSession = IdConverter.ConvertId(this.callContext, headerInformation, IdConverter.ConvertOption.IgnoreChangeKey, BasicTypes.Folder, RuleOperationParser.EmptyAttachmentIdList, ruleFieldFolderId.ChangeKey, this.hashCode);
				}, ruleFieldUri, ruleFieldFolderId.Id);
			}
			if (flag)
			{
				serverActionList.Remove(t);
				t = actionCreator(idAndSession.GetAsStoreObjectId(), serverRule);
				serverActionList.Add(t);
				if (serverRule.ProviderId < t.ProviderId)
				{
					serverRule.ProviderId = t.ProviderId;
				}
			}
		}

		private void ParseSmsAlertRuleActionField(EmailAddressWrapper[] mobileAddressArray, RuleFieldURI ruleFieldUri, TypeDictionary<ActionBase> actionMap, IList<ActionBase> serverActionList, Rule serverRule)
		{
			SendSmsAlertToRecipientsAction sendSmsAlertToRecipientsAction = actionMap.Lookup<SendSmsAlertToRecipientsAction>();
			if (mobileAddressArray == null)
			{
				serverActionList.Remove(sendSmsAlertToRecipientsAction);
				return;
			}
			List<Participant> list = new List<Participant>(mobileAddressArray.Length);
			E164Number mobileNumber;
			for (int i = 0; i < mobileAddressArray.Length; i++)
			{
				EmailAddressWrapper mobileAddress = mobileAddressArray[i];
				if (this.ValidateRuleField(() => E164Number.TryParse(mobileAddress.EmailAddress, out mobileNumber), RuleValidationErrorCode.InvalidValue, CoreResources.RuleErrorInvalidValue, ruleFieldUri, mobileAddress.EmailAddress))
				{
					list.Add(new Participant(mobileAddress.Name, mobileAddress.EmailAddress, "MOBILE"));
				}
			}
			if (list.Count == mobileAddressArray.Length)
			{
				serverActionList.Remove(sendSmsAlertToRecipientsAction);
				sendSmsAlertToRecipientsAction = SendSmsAlertToRecipientsAction.Create(list, serverRule);
				serverActionList.Add(sendSmsAlertToRecipientsAction);
				if (serverRule.ProviderId < sendSmsAlertToRecipientsAction.ProviderId)
				{
					serverRule.ProviderId = sendSmsAlertToRecipientsAction.ProviderId;
				}
			}
		}

		private void ParseMessageTemplateRuleActionField(ItemId ruleFieldItemId, RuleFieldURI ruleFieldUri, TypeDictionary<ActionBase> actionMap, IList<ActionBase> serverActionList, Rule serverRule)
		{
			ServerReplyMessageAction serverReplyMessageAction = actionMap.Lookup<ServerReplyMessageAction>();
			if (ruleFieldItemId == null)
			{
				serverActionList.Remove(serverReplyMessageAction);
				return;
			}
			if (serverReplyMessageAction != null && serverReplyMessageAction.Id != null)
			{
				ItemId itemId = IdConverter.ConvertStoreItemIdToItemId(serverReplyMessageAction.Id, this.mailboxSession);
				if (itemId.Id == ruleFieldItemId.Id)
				{
					return;
				}
			}
			StoreObjectId storeObjectId = null;
			byte[] templateId = null;
			if (!this.ValidateStoreOperation(delegate
			{
				IdHeaderInformation headerInformation = IdConverter.ConvertFromConcatenatedId(ruleFieldItemId.Id, BasicTypes.Item, null, false);
				IdAndSession idAndSession = IdConverter.ConvertId(this.callContext, headerInformation, IdConverter.ConvertOption.IgnoreChangeKey, BasicTypes.Item, RuleOperationParser.EmptyAttachmentIdList, ruleFieldItemId.ChangeKey, this.hashCode);
				using (Item templateItem = idAndSession.GetRootXsoItem(RuleOperationParser.ReplyTemplateProperties))
				{
					if (this.ValidateRuleField(() => null != templateItem, RuleValidationErrorCode.InvalidValue, CoreResources.RuleErrorInvalidValue, ruleFieldUri, ruleFieldItemId.Id))
					{
						storeObjectId = templateItem.Id.ObjectId;
						templateId = (byte[])templateItem[ItemSchema.ReplyTemplateId];
					}
				}
			}, ruleFieldUri, ruleFieldItemId.Id))
			{
				return;
			}
			Guid templateGuid = Guid.Empty;
			if (this.ValidateRuleField(delegate
			{
				bool result;
				try
				{
					templateGuid = new Guid(templateId);
					result = true;
				}
				catch (ArgumentNullException)
				{
					this.tracer.TraceError((long)this.hashCode, "Template ID does not exist on message item loaded.");
					result = false;
				}
				catch (ArgumentException arg)
				{
					string[] array = new string[templateId.Length];
					for (int i = 0; i < templateId.Length; i++)
					{
						array[i] = templateId[i].ToString();
					}
					this.tracer.TraceError<string, ArgumentException>((long)this.hashCode, "Template ID loaded from message {0} is not a valid GUID. Argument exception generated was {1}.", string.Join("-", array), arg);
					result = false;
				}
				return result;
			}, RuleValidationErrorCode.InvalidValue, CoreResources.RuleErrorItemIsNotTemplate, ruleFieldUri, ruleFieldItemId.Id))
			{
				serverActionList.Remove(serverReplyMessageAction);
				serverReplyMessageAction = ServerReplyMessageAction.Create(storeObjectId, templateGuid, serverRule);
				serverActionList.Add(serverReplyMessageAction);
				if (serverRule.ProviderId < serverReplyMessageAction.ProviderId)
				{
					serverRule.ProviderId = serverReplyMessageAction.ProviderId;
				}
			}
		}

		private void MergeServerRules()
		{
			int i = 0;
			int num = 10;
			foreach (Rule rule in this.serverRules)
			{
				if (!rule.IsDirty)
				{
					while (i < this.parsedRules.Count)
					{
						int num2 = this.parsedRules[i].Sequence + i;
						if (num2 != num)
						{
							break;
						}
						i++;
						num++;
					}
					if (rule.Sequence != num)
					{
						rule.Sequence = num;
						if (rule.IsNotSupported)
						{
							rule.SaveNotSupported();
						}
						else
						{
							rule.Save();
						}
					}
					this.parsedRules.Insert(i, rule);
				}
			}
			for (i = 0; i < this.parsedRules.Count; i++)
			{
				num = 10 + i;
				Rule rule2 = this.parsedRules[i];
				if (num != rule2.Sequence)
				{
					rule2.Sequence = num;
					if (rule2.IsNotSupported)
					{
						rule2.SaveNotSupported();
					}
				}
			}
			if (this.deletedRuleList != null)
			{
				foreach (Rule item in this.deletedRuleList)
				{
					this.parsedRules.Add(item);
				}
			}
		}

		private const int MaxStringLength = 255;

		private const int MaxDisplayNameLength = 256;

		private const int MaxItemClassLength = 128;

		private static readonly PropertyDefinition[] ReplyTemplateProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.ReplyTemplateId
		};

		private static readonly List<AttachmentId> EmptyAttachmentIdList = new List<AttachmentId>(0);

		private HashSet<int> priorityHashSet;

		private HashSet<string> ruleIdHashSet;

		private List<RuleValidationError> ruleValidationErrorList;

		private CallContext callContext;

		private MailboxSession mailboxSession;

		private Rules serverRules;

		private Rules parsedRules;

		private List<Rule> deletedRuleList;

		private Trace tracer;

		private int hashCode;

		private ExTimeZone timeZone;

		private IConfigurationSession adConfigSession;

		private delegate void StoreOperation();

		private delegate T TwoArgumentsConditionCreator<T, V>(Rule serverRule, V conditionValue) where T : Condition;

		private delegate T OneArgumentCreator<T>(Rule serverRule);

		private delegate T TwoArgumentsActionCreator<T, V>(V actionValue, Rule serverRule) where T : ActionBase;

		internal delegate bool RuleFieldParser();
	}
}
