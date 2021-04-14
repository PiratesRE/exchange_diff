using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.LoggingCommon;
using Microsoft.Filtering;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class TransportRulesEvaluationContext : BaseTransportRulesEvaluationContext
	{
		public PerTenantTransportSettings PerTenantTransportSettings { get; private set; }

		public IAgentLog TheAgentLog
		{
			get
			{
				return this.theAgentLog ?? AgentLog.Instance;
			}
			set
			{
				this.theAgentLog = value;
			}
		}

		public RuleHealthMonitor RuleExecutionMonitor { get; set; }

		public TransportRulesCostMonitor RuleSetExecutionMonitor { get; set; }

		private static PerTenantTransportSettings GetPerTenantTransportSettings(OrganizationId orgId)
		{
			if (null == orgId)
			{
				return null;
			}
			ITransportConfiguration transportConfiguration;
			if (Components.TryGetConfigurationComponent(out transportConfiguration))
			{
				PerTenantTransportSettings result;
				transportConfiguration.TryGetTransportSettings(orgId, out result);
				return result;
			}
			return null;
		}

		private TransportRulesEvaluationContext(TransportRuleCollection rules, MailItem mailItem, SmtpServer server, TransportRulesTracer tracer = null) : base(rules, tracer ?? new TransportRulesTracer(mailItem, false))
		{
			this.mailItem = mailItem;
			this.server = server;
			this.message = this.CreateMailMessage(mailItem);
			this.ExecutedActions = new List<Action>();
			this.ShouldAuditRules = false;
			this.SenderOverridden = false;
			this.SenderOverrideJustification = string.Empty;
			this.FpOverriden = false;
			this.CurrentAuditSeverityLevel = AuditSeverityLevel.Low;
			this.PerTenantTransportSettings = TransportRulesEvaluationContext.GetPerTenantTransportSettings(TransportUtils.GetOrganizationID(mailItem));
			this.RuleExecutionMonitor = TransportRulesEvaluationContext.CreateRuleHealthMonitor(mailItem);
			if (this.server != null)
			{
				this.userComparer = new UserComparer(this.server.AddressBook);
				this.membershipChecker = new MembershipChecker(this.server.AddressBook);
			}
		}

		public TransportRulesEvaluationContext(TransportRuleCollection rules, MailItem mailItem, SmtpServer server, ReceiveMessageEventSource endOfDataSource, SmtpSession session, TransportRulesCostMonitor ruleSetExecutionMonitor = null) : this(rules, mailItem, server, null)
		{
			this.endOfDataSource = endOfDataSource;
			this.session = session;
			this.eventType = EventType.EndOfData;
			this.RuleSetExecutionMonitor = ruleSetExecutionMonitor;
		}

		public TransportRulesEvaluationContext(TransportRuleCollection rules, MailItem mailItem, SmtpServer server, QueuedMessageEventSource eventSource, TransportRulesCostMonitor ruleSetExecutionMonitor = null, bool shouldAuditRules = false, TenantConfigurationCache<TransportRulesPerTenantSettings> transportRulesCache = null, TransportRulesTracer tracer = null) : this(rules, mailItem, server, tracer)
		{
			if (eventSource == null)
			{
				return;
			}
			if (eventSource is ResolvedMessageEventSource)
			{
				this.eventType = EventType.OnResolvedMessage;
				this.OnResolvedSource = (eventSource as ResolvedMessageEventSource);
			}
			else
			{
				if (!(eventSource is RoutedMessageEventSource))
				{
					throw new InvalidTransportRuleEventSourceTypeException(eventSource.GetType().FullName);
				}
				this.eventType = EventType.OnRoutedMessage;
				this.OnRoutedSource = (eventSource as RoutedMessageEventSource);
			}
			this.eventSource = eventSource;
			this.RuleSetExecutionMonitor = ruleSetExecutionMonitor;
			this.ShouldAuditRules = shouldAuditRules;
			this.RuleExecutionMonitor = TransportRulesEvaluationContext.CreateRuleHealthMonitor(mailItem);
			this.transportRulesCache = transportRulesCache;
		}

		public EventType EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public MailItem MailItem
		{
			get
			{
				return this.mailItem;
			}
		}

		public SmtpServer Server
		{
			get
			{
				return this.server;
			}
		}

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

		public bool CanModify
		{
			get
			{
				return (this.mailItem.Message.IsInterpersonalMessage || this.mailItem.Message.MapiMessageClass.StartsWith("IPM.Note", StringComparison.OrdinalIgnoreCase)) && this.mailItem.Message.MessageSecurityType == MessageSecurityType.None;
			}
		}

		public List<EnvelopeRecipient> MatchedRecipients
		{
			get
			{
				return this.matchedRecipients;
			}
			set
			{
				this.matchedRecipients = value;
			}
		}

		public ReceiveMessageEventSource EndOfDataSource
		{
			get
			{
				return this.endOfDataSource;
			}
		}

		public ResolvedMessageEventSource OnResolvedSource { get; private set; }

		public RoutedMessageEventSource OnRoutedSource { get; private set; }

		public QueuedMessageEventSource EventSource
		{
			get
			{
				return this.eventSource;
			}
		}

		public List<TransportRulesEvaluationContext.AddedRecipient> RecipientsToAdd
		{
			get
			{
				if (this.recipientsToAdd == null)
				{
					this.recipientsToAdd = new List<TransportRulesEvaluationContext.AddedRecipient>();
				}
				return this.recipientsToAdd;
			}
		}

		public RecipientState RecipientState
		{
			get
			{
				return this.recipientState;
			}
			set
			{
				this.recipientState = value;
			}
		}

		public SmtpResponse? EdgeRejectResponse
		{
			get
			{
				return this.edgeResponse;
			}
			set
			{
				this.edgeResponse = value;
			}
		}

		public bool MessageQuarantined
		{
			get
			{
				return this.messageQuarantined;
			}
			set
			{
				this.messageQuarantined = value;
			}
		}

		public SmtpSession Session
		{
			get
			{
				return this.session;
			}
		}

		public MailMessage Message
		{
			get
			{
				return this.message;
			}
		}

		internal void ResetRulesCache()
		{
			if (this.transportRulesCache != null)
			{
				OrganizationId organizationID = TransportUtils.GetOrganizationID(this.MailItem);
				if (organizationID != null)
				{
					this.transportRulesCache.RemoveValue(organizationID);
				}
			}
		}

		protected override FilteringServiceInvokerRequest FilteringServiceInvokerRequest
		{
			get
			{
				if (this.mailItem != null)
				{
					return TransportFilteringServiceInvokerRequest.CreateInstance(this.mailItem, null);
				}
				return null;
			}
		}

		internal static RuleHealthMonitor CreateRuleHealthMonitor(MailItem mailItem)
		{
			int num = 0;
			if (mailItem != null)
			{
				num = (int)mailItem.MimeStreamLength;
			}
			num = ((num == 0) ? TransportRulesEvaluationContext.MinDataLengthForThresholdCalculations : num);
			int mtlLoggingThresholdMs = (int)((double)num / (double)Components.TransportAppConfig.TransportRuleConfig.TransportRuleExecutionTimeReportingThresholdInBytesPerSecond * 1000.0);
			int eventLoggingThresholdMs = (int)((double)num / (double)Components.TransportAppConfig.TransportRuleConfig.TransportRuleExecutionTimeAlertingThresholdInBytesPerSecond * 1000.0);
			return TransportRulesEvaluationContext.CreateRuleHealthMonitor(mtlLoggingThresholdMs, eventLoggingThresholdMs);
		}

		internal static RuleHealthMonitor CreateRuleHealthMonitor(int mtlLoggingThresholdMs, int eventLoggingThresholdMs)
		{
			return new RuleHealthMonitor(RuleHealthMonitor.ActivityType.Execute, (long)mtlLoggingThresholdMs, (long)eventLoggingThresholdMs, delegate(string eventMessageDetails)
			{
				TransportAction.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RuleExecutionTimeExceededThreshold, null, new object[]
				{
					eventMessageDetails
				});
			});
		}

		protected override void OnDataClassificationsRetrieved(FilteringResults textExtractionResults)
		{
			if (textExtractionResults != null)
			{
				this.message.SetUnifiedContent(textExtractionResults);
			}
			this.AuditMessageClassifications();
		}

		public List<Action> ExecutedActions { get; private set; }

		public bool ShouldAuditRules { get; set; }

		public bool SenderOverridden { get; set; }

		public string SenderOverrideJustification { get; set; }

		public bool FpOverriden { get; set; }

		public AuditSeverityLevel CurrentAuditSeverityLevel { get; set; }

		public override void ResetPerRuleData()
		{
			base.ResetPerRuleData();
			this.ExecutedActions.Clear();
			this.CurrentAuditSeverityLevel = AuditSeverityLevel.Low;
		}

		public static void AddRuleData(ICollection<KeyValuePair<string, string>> data, string key, string value)
		{
			data.Add(new KeyValuePair<string, string>(key, value));
		}

		internal void Defer(TimeSpan deferTime)
		{
			if (this.EventSource != null)
			{
				if (this.RuleSetExecutionMonitor != null)
				{
					this.RuleSetExecutionMonitor.StopAndSetReporter(null);
				}
				this.EventSource.Defer(deferTime);
			}
		}

		internal TransportRulesTracer TransportRulesTracer
		{
			get
			{
				return base.Tracer as TransportRulesTracer;
			}
		}

		internal TestMessageConfig TestMessageConfig { get; set; }

		internal bool IsTestMessage
		{
			get
			{
				return this.TestMessageConfig != null && this.TestMessageConfig.IsTestMessage && (this.TestMessageConfig.LogTypes & LogTypesEnum.TransportRules) != LogTypesEnum.None;
			}
		}

		internal bool SuppressDelivery
		{
			get
			{
				return this.TestMessageConfig != null && this.TestMessageConfig.SuppressDelivery;
			}
		}

		protected virtual MailMessage CreateMailMessage(MailItem thisMailItem)
		{
			return new MailMessage(thisMailItem);
		}

		internal static int GetAttachmentTextScanLimit(MailItem mailItem)
		{
			TransportMailItem transportMailItem = TransportUtils.GetTransportMailItem(mailItem);
			if (transportMailItem == null)
			{
				return 0;
			}
			return transportMailItem.TransportSettings.TransportRuleAttachmentTextScanLimit;
		}

		private void AuditMessageClassifications()
		{
			if (!this.ShouldAuditRules || !base.HaveDataClassificationsBeenRetrieved)
			{
				return;
			}
			if (this.MailItem != null)
			{
				object obj;
				if (this.MailItem.Properties.TryGetValue("DCAudited", out obj))
				{
					return;
				}
				this.MailItem.Properties["DCAudited"] = true;
			}
			try
			{
				foreach (DiscoveredDataClassification discoveredDataClassification in base.DataClassifications)
				{
					List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
					TransportRulesEvaluationContext.AddRuleData(data, "dcid", discoveredDataClassification.Id);
					TransportRulesEvaluationContext.AddRuleData(data, "count", discoveredDataClassification.TotalCount.ToString());
					TransportRulesEvaluationContext.AddRuleData(data, "conf", discoveredDataClassification.MaxConfidenceLevel.ToString());
					this.EventSource.TrackAgentInfo(TrackAgentInfoAgentName.TRA.ToString("G"), TrackAgentInfoGroupName.DC.ToString("G"), data);
				}
			}
			catch (InvalidOperationException)
			{
				base.Tracer.TraceWarning("InvalidOperationException thrown while attempting to audit classification information. Expected when data size to Audit is high.");
			}
		}

		private static readonly int MinDataLengthForThresholdCalculations = 1024;

		private readonly MailMessage message;

		private readonly SmtpSession session;

		private readonly EventType eventType;

		private readonly MailItem mailItem;

		private readonly SmtpServer server;

		private readonly ReceiveMessageEventSource endOfDataSource;

		private readonly QueuedMessageEventSource eventSource;

		private TenantConfigurationCache<TransportRulesPerTenantSettings> transportRulesCache;

		private List<TransportRulesEvaluationContext.AddedRecipient> recipientsToAdd;

		private RecipientState recipientState;

		private SmtpResponse? edgeResponse;

		private bool messageQuarantined;

		private List<EnvelopeRecipient> matchedRecipients;

		private IAgentLog theAgentLog;

		private IStringComparer userComparer;

		private IStringComparer membershipChecker;

		internal struct AddedRecipient
		{
			public string Address
			{
				get
				{
					return this.address;
				}
			}

			public string DisplayName { get; set; }

			public RecipientP2Type RecipientP2Type
			{
				get
				{
					return this.recipientP2Type;
				}
			}

			public AddedRecipient(string address, string displayName, RecipientP2Type recipientP2Type)
			{
				this = default(TransportRulesEvaluationContext.AddedRecipient);
				this.address = address;
				this.recipientP2Type = recipientP2Type;
				this.DisplayName = displayName;
			}

			private readonly string address;

			private readonly RecipientP2Type recipientP2Type;
		}
	}
}
