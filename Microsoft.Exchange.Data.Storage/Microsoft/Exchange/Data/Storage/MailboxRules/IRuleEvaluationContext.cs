using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRuleEvaluationContext : IDisposable
	{
		CultureInfo CultureInfo { get; }

		Folder CurrentFolder { get; set; }

		string CurrentFolderDisplayName { get; }

		Rule CurrentRule { get; set; }

		string DefaultDomainName { get; }

		MessageItem DeliveredMessage { get; set; }

		Folder DeliveryFolder { get; set; }

		ExecutionStage ExecutionStage { get; set; }

		List<KeyValuePair<string, string>> ExtraTrackingEventData { get; }

		StoreId FinalDeliveryFolderId { get; set; }

		bool IsOof { get; }

		bool IsInternetMdnDisabled { get; }

		IsMemberOfResolver<string> IsMemberOfResolver { get; }

		string LocalServerFqdn { get; }

		IPAddress LocalServerNetworkAddress { get; }

		MessageItem Message { get; }

		long MimeSize { get; }

		int NestedLevel { get; set; }

		Dictionary<PropertyDefinition, object> PropertiesForAllMessageCopies { get; set; }

		Dictionary<PropertyDefinition, object> PropertiesForDelegateForward { get; set; }

		Dictionary<PropertyDefinition, object> SharedPropertiesBetweenAgents { get; set; }

		ProxyAddress Recipient { get; }

		IADRecipientCache RecipientCache { get; }

		ExEventLog.EventTuple OofHistoryCorruption { get; }

		ExEventLog.EventTuple OofHistoryFolderMissing { get; }

		bool ProcessingTestMessage { get; }

		bool ShouldExecuteDisabledAndInErrorRules { get; }

		ProxyAddress Sender { get; }

		string SenderAddress { get; set; }

		bool ShouldSkipMoveRule { get; set; }

		StoreSession StoreSession { get; }

		object this[PropTag tag]
		{
			get;
		}

		string XLoopValue { get; }

		LimitChecker LimitChecker { get; }

		IRuleConfig RuleConfig { get; }

		bool CompareSingleValue(PropTag tag, Restriction.RelOp op, object x, object y);

		void CopyProperty(MessageItem src, MessageItem target, PropertyDefinition property);

		MessageItem CreateMessageItem(PropertyDefinition[] prefetchProperties);

		void DisableAndMarkRuleInError(Rule rule, RuleAction.Type actionType, int actionIndex, DeferredError.RuleError errorCode);

		void MarkRuleInError(Rule rule, RuleAction.Type actionType, int actionIndex, DeferredError.RuleError errorCode);

		void RecordError(Exception exception, string stage);

		void RecordError(string message);

		ISubmissionItem GenerateSubmissionItem(MessageItem item, WorkItem workItem);

		IRuleEvaluationContext GetAttachmentContext(Attachment attachment);

		Folder GetDeletedItemsFolder();

		StorePropertyDefinition GetPropertyDefinitionForTag(PropTag tag);

		IRuleEvaluationContext GetRecipientContext(Recipient recipient);

		void SetMailboxOwnerAsSender(MessageItem message);

		bool HasRuleFiredBefore(Rule rule);

		void AddCurrentFolderIdTo(HashSet<StoreId> hashSet);

		List<Rule> LoadRules();

		void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs);

		void LogWorkItemExecution(WorkItem workItem);

		Folder OpenFolder(StoreId folderId);

		bool DetectLoop();

		void TraceDebug(string message);

		void TraceDebug<T>(string format, T argument);

		void TraceDebug<T1, T2>(string format, T1 argument1, T2 argument2);

		void TraceError(string message);

		void TraceError<T>(string format, T argument);

		void TraceError<T1, T2>(string format, T1 argument1, T2 argument2);

		void TraceError<T1, T2, T3>(string format, T1 argument1, T2 argument2, T3 argument3);

		void TraceFunction(string message);

		void TraceFunction<T>(string format, T argument);

		void TraceFunction<T1, T2>(string format, T1 argument1, T2 argument2);

		void TraceFunction<T1, T2, T3>(string format, T1 argument1, T2 argument2, T3 argument3);

		void TraceFunction<T1, T2, T3, T4>(string format, T1 argument1, T2 argument2, T3 argument3, T4 argument4);

		bool TryOpenLocalStore(byte[] folderEntryId, out Folder folder);

		ExTimeZone DetermineRecipientTimeZone();
	}
}
