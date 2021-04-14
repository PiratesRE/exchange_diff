using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RuleEvaluationContextBase : DisposeTrackableBase, IRuleEvaluationContext, IDisposable
	{
		protected RuleEvaluationContextBase(Folder folder, MessageItem message, StoreSession session, ProxyAddress recipient, IADRecipientCache recipientCache, long mimeSize, IRuleConfig ruleConfig, Trace tracer)
		{
			if (message != null)
			{
				this.ruleHistory = message.GetRuleHistory(session);
			}
			this.message = message;
			this.session = session;
			this.cultureInfo = this.session.PreferedCulture;
			this.folderSet = new Dictionary<StoreId, Folder>();
			this.TrackFolder(folder.Id, folder);
			this.folder = folder;
			this.recipientAddress = recipient;
			this.recipientCache = recipientCache;
			this.mimeSize = mimeSize;
			this.ruleConfig = ruleConfig;
			this.tracer = tracer;
		}

		protected RuleEvaluationContextBase(RuleEvaluationContextBase parentContext)
		{
			this.message = parentContext.message;
			this.ruleHistory = parentContext.ruleHistory;
			this.session = parentContext.session;
			this.cultureInfo = parentContext.cultureInfo;
			this.folderSet = parentContext.folderSet;
			this.folder = parentContext.folder;
			this.recipientAddress = parentContext.recipientAddress;
			this.recipientCache = parentContext.recipientCache;
			this.mimeSize = parentContext.mimeSize;
			this.traceFormatter = parentContext.traceFormatter;
			this.currentRule = parentContext.currentRule;
			this.limitChecker = parentContext.limitChecker;
			this.ruleConfig = parentContext.ruleConfig;
			this.tracer = parentContext.tracer;
			this.ShouldExecuteDisabledAndInErrorRules = parentContext.ShouldExecuteDisabledAndInErrorRules;
		}

		protected RuleEvaluationContextBase()
		{
		}

		public CultureInfo CultureInfo
		{
			get
			{
				return this.cultureInfo;
			}
			internal set
			{
				this.cultureInfo = value;
			}
		}

		public Folder CurrentFolder
		{
			get
			{
				return this.folder;
			}
			set
			{
				this.folder = value;
			}
		}

		public string CurrentFolderDisplayName
		{
			get
			{
				return this.CurrentFolder.DisplayName;
			}
		}

		public Rule CurrentRule
		{
			get
			{
				return this.currentRule;
			}
			set
			{
				this.currentRule = value;
			}
		}

		public abstract string DefaultDomainName { get; }

		public MessageItem DeliveredMessage
		{
			get
			{
				return this.deliveredMessage;
			}
			set
			{
				this.deliveredMessage = value;
			}
		}

		public Folder DeliveryFolder
		{
			get
			{
				return this.deliveryFolder;
			}
			set
			{
				this.deliveryFolder = value;
			}
		}

		public List<string> ErrorRecords
		{
			get
			{
				return this.errorRecords;
			}
		}

		public ExecutionStage ExecutionStage
		{
			get
			{
				return this.executionStage;
			}
			set
			{
				this.executionStage = value;
			}
		}

		public abstract List<KeyValuePair<string, string>> ExtraTrackingEventData { get; }

		public StoreId FinalDeliveryFolderId
		{
			get
			{
				return this.finalDeliveryFolderId;
			}
			set
			{
				this.finalDeliveryFolderId = value;
			}
		}

		public virtual bool IsOof
		{
			get
			{
				return this.isOof;
			}
			protected set
			{
				this.isOof = value;
			}
		}

		public bool IsInternetMdnDisabled
		{
			get
			{
				object obj = this.StoreSession.Mailbox.TryGetProperty(MailboxSchema.InternetMdns);
				return obj != null && obj is bool && (bool)obj;
			}
		}

		public abstract IsMemberOfResolver<string> IsMemberOfResolver { get; }

		public abstract string LocalServerFqdn { get; }

		public abstract IPAddress LocalServerNetworkAddress { get; }

		public MessageItem Message
		{
			get
			{
				return this.message;
			}
			protected set
			{
				this.message = value;
			}
		}

		public long MimeSize
		{
			get
			{
				return this.mimeSize;
			}
		}

		public int NestedLevel
		{
			get
			{
				return this.traceFormatter.NestedLevel;
			}
			set
			{
				this.traceFormatter.NestedLevel = value;
			}
		}

		public Dictionary<PropertyDefinition, object> PropertiesForAllMessageCopies
		{
			get
			{
				return this.propertiesForAllMessageCopies;
			}
			set
			{
				this.propertiesForAllMessageCopies = value;
			}
		}

		public Dictionary<PropertyDefinition, object> PropertiesForDelegateForward
		{
			get
			{
				return this.propertiesForDelegateForward;
			}
			set
			{
				this.propertiesForDelegateForward = value;
			}
		}

		public Dictionary<PropertyDefinition, object> SharedPropertiesBetweenAgents
		{
			get
			{
				return this.sharedPropertiesBetweenAgents;
			}
			set
			{
				this.sharedPropertiesBetweenAgents = value;
			}
		}

		public ProxyAddress Recipient
		{
			get
			{
				return this.recipientAddress;
			}
			protected set
			{
				this.recipientAddress = value;
			}
		}

		public IADRecipientCache RecipientCache
		{
			get
			{
				return this.recipientCache;
			}
		}

		public abstract ExEventLog.EventTuple OofHistoryCorruption { get; }

		public abstract ExEventLog.EventTuple OofHistoryFolderMissing { get; }

		public bool ProcessingTestMessage
		{
			get
			{
				return this.traceFormatter.HasTraceHistory;
			}
		}

		public bool ShouldExecuteDisabledAndInErrorRules { get; protected set; }

		public ProxyAddress Sender
		{
			get
			{
				if (this.message.Sender == null)
				{
					return null;
				}
				return ProxyAddress.Parse(this.message.Sender.RoutingType ?? string.Empty, this.message.Sender.EmailAddress ?? string.Empty);
			}
		}

		public string SenderAddress
		{
			get
			{
				return this.senderAddress;
			}
			set
			{
				this.senderAddress = value;
			}
		}

		public bool ShouldSkipMoveRule
		{
			get
			{
				return this.shouldSkipMoveRule;
			}
			set
			{
				this.shouldSkipMoveRule = value;
			}
		}

		public StoreSession StoreSession
		{
			get
			{
				return this.session;
			}
			protected set
			{
				this.session = value;
			}
		}

		public object this[PropTag tag]
		{
			get
			{
				tag = RuleUtil.NormalizeTag(tag);
				object obj = this.CalculatePropertyValue(tag);
				if (obj == null)
				{
					obj = this.GetPropertyValue(tag);
				}
				RuleUtil.CheckValueType(obj, tag);
				return obj;
			}
		}

		public string XLoopValue
		{
			get
			{
				return this.recipientAddress.AddressString;
			}
		}

		public LimitChecker LimitChecker
		{
			get
			{
				return this.limitChecker;
			}
			protected set
			{
				this.limitChecker = value;
			}
		}

		public IRuleConfig RuleConfig
		{
			get
			{
				return this.ruleConfig;
			}
		}

		protected abstract IStorePropertyBag PropertyBag { get; }

		protected static string GetMessageBody(RuleEvaluationContextBase context)
		{
			string text;
			using (TextReader textReader = context.Message.Body.OpenTextReader(BodyFormat.TextPlain))
			{
				text = textReader.ReadToEnd();
				if (text == null)
				{
					text = string.Empty;
				}
			}
			return text;
		}

		protected static RecipientItemType GetRecipientType(RuleEvaluationContextBase context)
		{
			RecipientItemType result = RecipientItemType.Unknown;
			string text = context.PropertyBag[StoreObjectSchema.ItemClass] as string;
			if (!string.IsNullOrEmpty(text) && ObjectClass.IsReport(text))
			{
				result = RecipientItemType.To;
			}
			else
			{
				RecipientCollection recipients = context.Message.Recipients;
				for (int i = 0; i < recipients.Count; i++)
				{
					if (recipients[i].Participant.RoutingType != null && recipients[i].Participant.EmailAddress != null)
					{
						ProxyAddress addressToResolve = ProxyAddress.Parse(recipients[i].Participant.RoutingType, recipients[i].Participant.EmailAddress);
						if (RuleUtil.IsSameUser(context, context.RecipientCache, addressToResolve, context.Recipient))
						{
							result = recipients[i].RecipientItemType;
							break;
						}
					}
				}
			}
			return result;
		}

		public virtual bool CompareSingleValue(PropTag tag, Restriction.RelOp op, object x, object y)
		{
			if (x == null)
			{
				return y == null;
			}
			if (y == null)
			{
				return false;
			}
			if (!RestrictionEvaluator.IsSupportedOperation(op))
			{
				throw new InvalidRuleException(string.Format("Operation {0} is not supported", op));
			}
			if (x.GetType() != y.GetType() && (!(x.GetType() == typeof(ExDateTime)) || !(y.GetType() == typeof(DateTime))))
			{
				throw new InvalidRuleException(string.Format("Can not compare values of type {0} and type {1}", x.GetType(), y.GetType()));
			}
			if (op == Restriction.RelOp.MemberOfDL)
			{
				byte[] array = x as byte[];
				if (array == null)
				{
					this.TraceDebug("CompareSingleValue: MemberOf, recipientEntryId is not a byte array.");
					this.RecordError(ServerStrings.FolderRuleErrorInvalidRecipientEntryId);
					return false;
				}
				byte[] array2 = y as byte[];
				if (array2 == null)
				{
					this.TraceDebug("CompareSingleValue: MemberOf, groupEntryId is not a byte array, marking rule in error.");
					throw new InvalidRuleException(ServerStrings.FolderRuleErrorInvalidGroup(y.GetType().Name));
				}
				return RuleUtil.IsMemberOf(this, array, array2);
			}
			else
			{
				int? num = RestrictionEvaluator.CompareValue(this.CultureInfo, tag, x, y);
				if (num == null)
				{
					throw new InvalidRuleException(string.Format("Can't compare value '{0}' and '{1}'", x, y));
				}
				return RestrictionEvaluator.GetOperationResultFromOrder(op, num.Value);
			}
		}

		public void CopyProperty(MessageItem src, MessageItem target, PropertyDefinition property)
		{
			object obj = src.TryGetProperty(property);
			if (!(obj is PropertyError))
			{
				target[property] = obj;
				return;
			}
			this.TraceDebug<object>("Can't read property: {0}", obj);
		}

		public abstract MessageItem CreateMessageItem(PropertyDefinition[] prefetchProperties);

		public virtual void DisableAndMarkRuleInError(Rule rule, RuleAction.Type actionType, int actionIndex, DeferredError.RuleError errorCode)
		{
			this.TraceError<Rule, DeferredError.RuleError>("Rule {0} will be disabled due to error: {1}", rule, errorCode);
			RuleUtil.DisableRule(rule, this.folder.MapiFolder);
			this.MarkRuleInError(rule, actionType, actionIndex, errorCode);
		}

		public virtual void MarkRuleInError(Rule rule, RuleAction.Type actionType, int actionIndex, DeferredError.RuleError errorCode)
		{
			RuleUtil.MarkRuleInError(rule, this.folder.MapiFolder);
		}

		public virtual void RecordError(Exception exception, string stage)
		{
			string text2;
			if (this.currentRule != null)
			{
				string text = ((ulong)this.currentRule.ID).ToString();
				this.TraceError<string, Exception>("Unexpected exception while processing rule \"{0}\": {1}", text, exception);
				text2 = ServerStrings.FolderRuleErrorRecordForSpecificRule(text, this.recipientAddress.ToString(), stage, exception.GetType().Name, exception.Message);
			}
			else
			{
				this.TraceError<Exception>("Unexpected exception while processing rules: {0}", exception);
				text2 = ServerStrings.FolderRuleErrorRecord(this.recipientAddress.ToString(), stage, exception.GetType().Name, exception.Message);
			}
			RuleStatistics.LogException(exception);
			this.RecordError(text2);
		}

		public virtual void RecordError(string message)
		{
			if (this.errorRecords == null)
			{
				this.errorRecords = new List<string>(1);
			}
			this.errorRecords.Add(message);
		}

		public abstract ISubmissionItem GenerateSubmissionItem(MessageItem item, WorkItem workItem);

		public virtual IRuleEvaluationContext GetAttachmentContext(Attachment attachment)
		{
			throw new InvalidRuleException("Attachment sub restriction can only be used in message context");
		}

		public abstract Folder GetDeletedItemsFolder();

		public StorePropertyDefinition GetPropertyDefinitionForTag(PropTag tag)
		{
			if (tag == PropTag.DisplayBcc)
			{
				return ItemSchema.DisplayBcc;
			}
			if (tag == PropTag.DisplayCc)
			{
				return ItemSchema.DisplayCc;
			}
			if (tag == PropTag.DisplayTo)
			{
				return ItemSchema.DisplayTo;
			}
			NativeStorePropertyDefinition[] array = PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType, this.StoreSession.Mailbox.MapiStore, this.StoreSession, new PropTag[]
			{
				tag
			});
			if (array == null || array.Length == 0 || array[0] == null)
			{
				throw new InvalidRuleException(string.Format("Can't get property definition for tag {0}", tag));
			}
			return array[0];
		}

		public virtual IRuleEvaluationContext GetRecipientContext(Recipient recipient)
		{
			throw new InvalidRuleException("Recipient sub restriction can only be used in message context");
		}

		public abstract void SetMailboxOwnerAsSender(MessageItem message);

		public virtual bool HasRuleFiredBefore(Rule rule)
		{
			if (this.ruleHistory.Count > 11)
			{
				this.TraceDebug("Rule history length has exceeded the limit. Treat this as loop.");
				return true;
			}
			return this.ruleHistory.Contains(rule.ID);
		}

		public void AddCurrentFolderIdTo(HashSet<StoreId> hashSet)
		{
			hashSet.Add(this.CurrentFolder.Id);
		}

		public virtual List<Rule> LoadRules()
		{
			return RuleLoader.LoadRules(this);
		}

		public abstract void LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs);

		public virtual void LogWorkItemExecution(WorkItem workItem)
		{
			this.TraceDebug<WorkItem>("Executed work item: {0}", workItem);
		}

		public Folder OpenFolder(StoreId folderId)
		{
			Folder result;
			if (this.folderSet.TryGetValue(folderId, out result))
			{
				return result;
			}
			result = Folder.Bind(this.session, folderId, RuleEvaluationContextBase.AdditionalFolderProperties);
			this.TrackFolder(folderId, result);
			return result;
		}

		public bool DetectLoop()
		{
			string[] valueOrDefault = this.message.GetValueOrDefault<string[]>(MessageItemSchema.XLoop, null);
			if (valueOrDefault == null)
			{
				this.TraceDebug("No X-Loop values present.");
				return false;
			}
			if (this.limitChecker.DoesExceedXLoopMaxCount(valueOrDefault.Length))
			{
				return true;
			}
			this.TraceDebug<string>("Looking for X-Loop header containing: {0}", this.XLoopValue);
			string[] array = valueOrDefault;
			int i = 0;
			while (i < array.Length)
			{
				string text = array[i];
				this.TraceDebug<string>("X-Loop: {0}", text);
				bool result;
				if (text.Length >= 1000)
				{
					this.TraceDebug("Possible loop detected: value exceeds allowable length.");
					result = true;
				}
				else
				{
					if (!this.XLoopValue.Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						i++;
						continue;
					}
					this.TraceDebug("Loop detected: value matches the user's X-Loop token.");
					result = true;
				}
				return result;
			}
			this.TraceDebug("No loop detected.");
			return false;
		}

		public void TraceDebug(string message)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.DebugTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(message);
				if (flag)
				{
					this.tracer.TraceDebug(0L, text);
				}
			}
		}

		public void TraceDebug<T>(string format, T argument)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.DebugTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(format, argument);
				if (flag)
				{
					this.tracer.TraceDebug(0L, text);
				}
			}
		}

		public void TraceDebug<T1, T2>(string format, T1 argument1, T2 argument2)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.DebugTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(format, argument1, argument2);
				if (flag)
				{
					this.tracer.TraceDebug(0L, text);
				}
			}
		}

		public void TraceError(string message)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.ErrorTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(message);
				if (flag)
				{
					this.tracer.TraceError(0L, text);
				}
			}
		}

		public void TraceError<T>(string format, T argument)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.ErrorTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(format, argument);
				if (flag)
				{
					this.tracer.TraceError(0L, text);
				}
			}
		}

		public void TraceError<T1, T2>(string format, T1 argument1, T2 argument2)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.ErrorTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(format, argument1, argument2);
				if (flag)
				{
					this.tracer.TraceError(0L, text);
				}
			}
		}

		public void TraceError<T1, T2, T3>(string format, T1 argument1, T2 argument2, T3 argument3)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.ErrorTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(format, argument1, argument2, argument3);
				if (flag)
				{
					this.tracer.TraceError(0L, text);
				}
			}
		}

		public void TraceFunction(string message)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.FunctionTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(message);
				if (flag)
				{
					this.tracer.TraceFunction(0L, text);
				}
			}
		}

		public void TraceFunction<T>(string format, T argument)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.FunctionTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(format, argument);
				if (flag)
				{
					this.tracer.TraceFunction(0L, text);
				}
			}
		}

		public void TraceFunction<T1, T2>(string format, T1 argument1, T2 argument2)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.FunctionTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(format, argument1, argument2);
				if (flag)
				{
					this.tracer.TraceFunction(0L, text);
				}
			}
		}

		public void TraceFunction<T1, T2, T3>(string format, T1 argument1, T2 argument2, T3 argument3)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.FunctionTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(format, argument1, argument2, argument3);
				if (flag)
				{
					this.tracer.TraceFunction(0L, text);
				}
			}
		}

		public void TraceFunction<T1, T2, T3, T4>(string format, T1 argument1, T2 argument2, T3 argument3, T4 argument4)
		{
			bool flag = this.tracer.IsTraceEnabled(TraceType.FunctionTrace);
			if (flag || this.ProcessingTestMessage)
			{
				string text = this.traceFormatter.Format(format, argument1, argument2, argument3, argument4);
				if (flag)
				{
					this.tracer.TraceFunction(0L, text);
				}
			}
		}

		public bool TryOpenLocalStore(byte[] folderEntryId, out Folder folder)
		{
			folder = null;
			StoreObjectId storeObjectId = null;
			if (!IdConverter.IsFolderId(folderEntryId))
			{
				this.TraceDebug<byte[]>("Can't open folder, id {0} is not an Exchange folder entry id.", folderEntryId);
				return false;
			}
			try
			{
				storeObjectId = StoreObjectId.FromProviderSpecificId(folderEntryId, StoreObjectType.Folder);
			}
			catch (CorruptDataException argument)
			{
				this.TraceDebug<byte[], CorruptDataException>("Can't open folder, id {0} is corrupt: {1}", folderEntryId, argument);
				return false;
			}
			bool result;
			try
			{
				folder = this.OpenFolder(storeObjectId);
				result = true;
			}
			catch (ObjectNotFoundException argument2)
			{
				this.TraceDebug<StoreObjectId, ObjectNotFoundException>("Can't open the folder with id {0} due to error {1}", storeObjectId, argument2);
				result = false;
			}
			return result;
		}

		public abstract ExTimeZone DetermineRecipientTimeZone();

		protected override void InternalDispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (this.folderSet != null)
			{
				foreach (Folder folder in this.folderSet.Values)
				{
					folder.Dispose();
				}
				this.folderSet = null;
			}
			if (this.traceFormatter != null)
			{
				this.traceFormatter.Dispose();
				this.traceFormatter = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RuleEvaluationContextBase>(this);
		}

		protected virtual object CalculatePropertyValue(PropTag tag)
		{
			return null;
		}

		public bool CompareAddresses(object messageValue, object ruleValue)
		{
			ProxyAddress proxyAddressFromSearchKey = RuleUtil.GetProxyAddressFromSearchKey(messageValue);
			ProxyAddress proxyAddressFromSearchKey2 = RuleUtil.GetProxyAddressFromSearchKey(ruleValue);
			if (proxyAddressFromSearchKey2 == null || proxyAddressFromSearchKey2 is InvalidProxyAddress || string.IsNullOrEmpty(proxyAddressFromSearchKey2.ValueString))
			{
				string recipient = ServerStrings.Null;
				if (proxyAddressFromSearchKey2 != null)
				{
					recipient = proxyAddressFromSearchKey2.ToString();
				}
				this.DisableAndMarkRuleInError(this.CurrentRule, RuleAction.Type.OP_INVALID, 0, DeferredError.RuleError.Parsing);
				this.RecordError(ServerStrings.FolderRuleErrorInvalidRecipient(recipient));
				return false;
			}
			this.TraceDebug<ProxyAddress, ProxyAddress>("Comparing recipients, message address {0}, rule address {1}", proxyAddressFromSearchKey, proxyAddressFromSearchKey2);
			RuleUtil.FaultInjection((FaultInjectionLid)4257530192U);
			return RuleUtil.IsSameUser(this, this.RecipientCache, proxyAddressFromSearchKey, proxyAddressFromSearchKey2);
		}

		protected object GetPropertyValue(PropTag tag)
		{
			StorePropertyDefinition propertyDefinitionForTag = this.GetPropertyDefinitionForTag(tag);
			IStorePropertyBag propertyBag = this.PropertyBag;
			object obj = propertyBag.TryGetProperty(propertyDefinitionForTag);
			if (obj is PropertyError)
			{
				if (!PropertyError.IsPropertyValueTooBig(obj))
				{
					this.TraceError<object, StorePropertyDefinition>("Encountered error {0} while reading value of property {1}", obj, propertyDefinitionForTag);
					return null;
				}
				obj = RuleUtil.ReadStreamedProperty(propertyBag, propertyDefinitionForTag);
			}
			if (obj.GetType().GetTypeInfo().IsEnum)
			{
				obj = RuleUtil.ConvertEnumValue(tag, obj);
			}
			return obj;
		}

		internal virtual void SetRecipient(ProxyAddress recipient)
		{
			this.Recipient = recipient;
		}

		private void TrackFolder(StoreId folderId, Folder folder)
		{
			this.folderSet[folderId] = folder;
			if (!folder.Id.Equals(folderId))
			{
				this.folderSet[folder.Id] = folder;
			}
		}

		private const int MaxNumberOfRulesInHistory = 11;

		internal static readonly PropertyDefinition[] AdditionalFolderProperties = new PropertyDefinition[]
		{
			FolderSchema.FolderRulesSize,
			StoreObjectSchema.EntryId,
			StoreObjectSchema.PolicyTag,
			StoreObjectSchema.RetentionPeriod,
			StoreObjectSchema.RetentionFlags,
			StoreObjectSchema.ArchiveTag,
			StoreObjectSchema.ArchivePeriod,
			FolderSchema.RetentionTagEntryId,
			FolderSchema.MailEnabled,
			FolderSchema.ProxyGuid
		};

		private CultureInfo cultureInfo;

		private Rule currentRule;

		private MessageItem deliveredMessage;

		private Folder deliveryFolder;

		private List<string> errorRecords;

		private ExecutionStage executionStage;

		private StoreId finalDeliveryFolderId;

		private Folder folder;

		private Dictionary<StoreId, Folder> folderSet;

		private bool isOof;

		private LimitChecker limitChecker;

		private MessageItem message;

		private long mimeSize;

		private Dictionary<PropertyDefinition, object> propertiesForAllMessageCopies;

		private Dictionary<PropertyDefinition, object> propertiesForDelegateForward;

		private Dictionary<PropertyDefinition, object> sharedPropertiesBetweenAgents;

		private ProxyAddress recipientAddress;

		private IADRecipientCache recipientCache;

		private IRuleConfig ruleConfig;

		protected RuleHistory ruleHistory;

		private string senderAddress;

		private StoreSession session;

		private bool shouldSkipMoveRule;

		protected TraceFormatter traceFormatter;

		protected Trace tracer;
	}
}
