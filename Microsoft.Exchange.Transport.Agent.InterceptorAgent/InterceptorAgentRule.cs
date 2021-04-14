using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	[XmlRoot("Rule")]
	public sealed class InterceptorAgentRule : IConfigurable, IEquatable<InterceptorAgentRule>
	{
		public InterceptorAgentRule()
		{
			this.conditions = new List<InterceptorAgentCondition>();
			this.events = InterceptorAgentEvent.Invalid;
		}

		internal InterceptorAgentRule(string name, string description, List<InterceptorAgentCondition> conditions, InterceptorAgentAction action, InterceptorAgentEvent evt) : this(name, description, conditions, action, evt, SourceType.User, InterceptorAgentRule.DefaultUser)
		{
		}

		internal InterceptorAgentRule(string name, string description, List<InterceptorAgentCondition> conditions, InterceptorAgentAction action, InterceptorAgentEvent evt, SourceType source, string createdBy)
		{
			this.Name = name;
			this.description = description;
			this.conditions = conditions;
			this.events = evt;
			this.action = action;
			this.Source = source;
			this.CreatedBy = createdBy;
		}

		public static IEnumerable<KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>> InvalidConditionEventPairs
		{
			get
			{
				return InterceptorAgentRule.invalidConditionEventPairs;
			}
		}

		public static IEnumerable<KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>> InvalidActionEventPairs
		{
			get
			{
				return InterceptorAgentRule.invalidActionEventPairs;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[XmlAttribute]
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		[XmlIgnore]
		public DateTime ExpireTime
		{
			get
			{
				return this.expireTime;
			}
		}

		[XmlIgnore]
		public DateTime ExpireTimeUtc
		{
			get
			{
				return this.expireTimeUtc;
			}
		}

		[XmlIgnore]
		public MultiValuedProperty<ADObjectId> Target
		{
			get
			{
				return this.target;
			}
		}

		[XmlIgnore]
		public Version RuleVersion
		{
			get
			{
				return this.version;
			}
		}

		[XmlAttribute("Event")]
		public InterceptorAgentEvent Events
		{
			get
			{
				return this.events;
			}
			set
			{
				this.events = value;
			}
		}

		[XmlAttribute("Source")]
		public SourceType Source
		{
			get
			{
				return this.source;
			}
			set
			{
				this.source = value;
			}
		}

		[XmlAttribute("CreatedBy")]
		public string CreatedBy
		{
			get
			{
				return this.createdBy;
			}
			set
			{
				this.createdBy = value;
			}
		}

		[XmlArray]
		[XmlArrayItem("Condition")]
		public List<InterceptorAgentCondition> Conditions
		{
			get
			{
				return this.conditions;
			}
			set
			{
				this.conditions = value;
			}
		}

		public InterceptorAgentAction Action
		{
			get
			{
				return this.action;
			}
			set
			{
				this.action = value;
			}
		}

		[XmlIgnore]
		public bool IsValid
		{
			get
			{
				return true;
			}
		}

		[XmlIgnore]
		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		[XmlIgnore]
		public ObjectState ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		[XmlIgnore]
		public string DistinguishedName
		{
			get
			{
				return this.distinguishedName;
			}
		}

		[XmlIgnore]
		public ADObjectId ObjectCategory
		{
			get
			{
				return this.objectCategory;
			}
		}

		[XmlIgnore]
		public string AdminDisplayName
		{
			get
			{
				return this.adminDisplayName;
			}
		}

		[XmlIgnore]
		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		[XmlIgnore]
		public DateTime? WhenChangedUtc
		{
			get
			{
				return this.whenChangedUtc;
			}
		}

		[XmlIgnore]
		public DateTime? WhenChanged
		{
			get
			{
				return this.whenChanged;
			}
		}

		[XmlIgnore]
		public DateTime? WhenCreatedUtc
		{
			get
			{
				return this.whenCreatedUtc;
			}
		}

		[XmlIgnore]
		public DateTime? WhenCreated
		{
			get
			{
				return this.whenCreated;
			}
		}

		public static InterceptorAgentRule CreateRuleFromXml(string xml)
		{
			InterceptorAgentRule result;
			using (StringReader stringReader = new StringReader(xml))
			{
				InterceptorAgentRule interceptorAgentRule = (InterceptorAgentRule)InterceptorAgentRule.serializer.Deserialize(stringReader);
				interceptorAgentRule.Verify();
				result = interceptorAgentRule;
			}
			return result;
		}

		public string ToXmlString()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(InterceptorAgentRule));
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				xmlSerializer.Serialize(stringWriter, this);
				result = stringWriter.GetStringBuilder().ToString();
			}
			return result;
		}

		public bool IsMatch(string subject, string envelopeFrom, string messageId, EnvelopeRecipientCollection recipients, RoutingAddress envelopeTo, HeaderList headers, string smtpClientHostName, Guid tenantId, MailDirectionality directionality, string accountForest)
		{
			foreach (InterceptorAgentCondition interceptorAgentCondition in this.conditions)
			{
				if (!interceptorAgentCondition.IsMatch(subject, envelopeFrom, messageId, recipients, envelopeTo, headers, smtpClientHostName, tenantId, directionality, accountForest))
				{
					return false;
				}
			}
			if (this.ExpireTime < DateTime.UtcNow + InterceptorAgentRule.RuleExpirationWarningInterval)
			{
				Util.EventLog.LogEvent(TransportEventLogConstants.Tuple_InterceptorRuleNearingExpiration, this.name, new object[]
				{
					this.name,
					this.ExpireTime.ToLocalTime()
				});
				EventNotificationItem.Publish(ExchangeComponent.FfoTransport.Name, "InterceptorRuleNearingExpiration", null, string.Format("The interceptor rule {0} is nearing expiration.", this.name), ResultSeverityLevel.Warning, false);
			}
			return true;
		}

		public void ToString(StringBuilder result)
		{
			result.AppendFormat("<rule name=\"{0}\" description=\"{1}\" Source = \"{2}\" CreatedBy = \"{3}\" >\n", new object[]
			{
				this.name,
				this.description,
				this.source.ToString(),
				this.createdBy
			});
			result.Append("<conditions>\n");
			foreach (InterceptorAgentCondition interceptorAgentCondition in this.conditions)
			{
				result.Append("\t <condition ");
				interceptorAgentCondition.ToString(result);
				result.Append("/>\n");
			}
			result.Append("</conditions>\n");
			result.Append("<actions>\n");
			result.Append("\t <");
			this.action.ToString(result);
			result.AppendFormat(" event=\"{0}\" />\n", this.events.ToString());
			result.Append("</actions>\n");
			result.Append("</rule>\n");
		}

		public ValidationError[] Validate()
		{
			return new ValidationError[0];
		}

		public void ActivateRule()
		{
			if (this.isActive)
			{
				return;
			}
			this.ruleStartTime = new MemoryCounter("Rule Start Time")
			{
				RawValue = Stopwatch.GetTimestamp()
			};
			this.lastActionTakenTime = new MemoryCounter("Last Action Taken Time")
			{
				RawValue = Stopwatch.GetTimestamp()
			};
			this.eventPerfCountersGroup = new InterceptorCountersGroup(this.Events);
			this.actionPerfCountersGroup = new InterceptorCountersGroup(this.Action.Action);
			this.isActive = true;
		}

		public void DeactivateRule()
		{
			if (!this.isActive)
			{
				return;
			}
			this.eventPerfCountersGroup.StopTracking();
			this.actionPerfCountersGroup.StopTracking();
			this.isActive = false;
		}

		public bool Equals(InterceptorAgentRule other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(this, other) || (!(this.version != other.version) && !(this.Name != other.Name) && !(this.Description != other.description) && this.source == other.source && !(this.createdBy != other.createdBy) && this.Events == other.Events && !(this.ExpireTimeUtc != other.ExpireTimeUtc) && this.Action.Equals(other.Action) && (this.Target != null || other.Target == null) && (this.Target == null || this.Target.Equals(other.Target)) && this.ConditionsEqual(other)));
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as InterceptorAgentRule);
		}

		public override int GetHashCode()
		{
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name.GetHashCode();
			}
			return 0;
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
		}

		void IConfigurable.ResetChangeTracking()
		{
		}

		internal static string ToString(IEnumerable<InterceptorAgentRule> rules)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\n<rules>\n");
			if (rules != null)
			{
				foreach (InterceptorAgentRule interceptorAgentRule in rules)
				{
					interceptorAgentRule.ToString(stringBuilder);
				}
			}
			stringBuilder.Append("</rules>");
			return stringBuilder.ToString();
		}

		internal static InterceptorAgentRule InternalEvaluate(IEnumerable<InterceptorAgentRule> rules, InterceptorAgentEvent evt, string subject, string envelopeFrom, string messageId, EnvelopeRecipientCollection recipients, RoutingAddress envelopeTo, HeaderList headers, string smtpClientHostName, Guid tenantId, MailDirectionality directionality, string accountForest)
		{
			InterceptorAgentRule interceptorAgentRule = null;
			foreach (InterceptorAgentRule interceptorAgentRule2 in rules)
			{
				if ((ushort)(interceptorAgentRule2.Events & evt) != 0)
				{
					interceptorAgentRule2.eventPerfCountersGroup.Increment(evt, false);
					if (interceptorAgentRule2.IsMatch(subject, envelopeFrom, messageId, recipients, envelopeTo, headers, smtpClientHostName, tenantId, directionality, accountForest))
					{
						interceptorAgentRule = interceptorAgentRule2;
						interceptorAgentRule.eventPerfCountersGroup.Increment(evt, true);
						break;
					}
				}
			}
			return interceptorAgentRule;
		}

		internal bool MatchRoleAndServerVersion(ProcessTransportRole role, ServerVersion version)
		{
			foreach (InterceptorAgentCondition interceptorAgentCondition in this.conditions)
			{
				if (!interceptorAgentCondition.MatchRoleAndServerVersion(role, version))
				{
					return false;
				}
			}
			return true;
		}

		internal void AddCondition(InterceptorAgentCondition condition)
		{
			this.conditions.Add(condition);
		}

		internal void SetAction(InterceptorAgentEvent evt, InterceptorAgentRuleBehavior action)
		{
			this.SetAction(new InterceptorAgentAction(action));
		}

		internal void SetAction(InterceptorAgentAction action)
		{
			if (this.action != null)
			{
				throw new InvalidOperationException("Only 1 action allowed");
			}
			this.action = action;
		}

		internal InterceptorAgentRuleBehavior PerformAction(MailItem mailItem, Action drop, Action<SmtpResponse> reject, Action<TimeSpan> defer = null)
		{
			InterceptorAgentRuleBehavior interceptorAgentRuleBehavior = this.Action.PerformAction(this, mailItem, drop, reject, defer);
			this.IncrementActionPerfCounter(interceptorAgentRuleBehavior);
			return interceptorAgentRuleBehavior;
		}

		internal string GetSourceContext(string agentName, InterceptorAgentEvent evt, bool includeResponseString)
		{
			string result;
			if (!includeResponseString || string.IsNullOrEmpty(this.Action.ResponseString))
			{
				result = string.Format("Interceptor Agent = {0}; Interceptor rule '{1}\\{2} Message' {3}", new object[]
				{
					agentName,
					this.Name,
					this.Action.Action.ToString(),
					evt.ToString()
				});
			}
			else
			{
				result = string.Format("Interceptor Agent = {0}; Interceptor rule '{1}\\{2} Message' {3}; <{4}>", new object[]
				{
					agentName,
					this.Name,
					this.Action.Action.ToString(),
					evt.ToString(),
					this.Action.ResponseString
				});
			}
			return result;
		}

		internal void SetPropertiesFromAdObjet(InterceptorRule adRule)
		{
			this.identity = adRule.Identity;
			if (!Version.TryParse(adRule.Version, out this.version))
			{
				throw new InvalidOperationException(string.Format("Invalid version property: {0}", adRule.Version));
			}
			this.target = adRule.Target;
			this.expireTimeUtc = adRule.ExpireTimeUtc;
			this.expireTime = adRule.ExpireTime;
			this.distinguishedName = adRule.DistinguishedName;
			this.objectCategory = adRule.ObjectCategory;
			this.guid = adRule.Guid;
			this.adminDisplayName = adRule.AdminDisplayName;
			this.whenCreatedUtc = adRule.WhenCreatedUTC;
			this.whenCreated = adRule.WhenCreated;
			this.whenChangedUtc = adRule.WhenChangedUTC;
			this.whenChanged = adRule.WhenChanged;
		}

		internal XElement GetDiagnosticInfoOfPerfCounters()
		{
			XElement xelement = new XElement("PerfCounters");
			this.eventPerfCountersGroup.GetDiagnosticInfo(xelement);
			this.actionPerfCountersGroup.GetDiagnosticInfo(xelement);
			TimeSpan timeSpan = new TimeSpan(Stopwatch.GetTimestamp() - this.ruleStartTime.RawValue);
			xelement.Add(new XElement("RuleStartElapsedTime", timeSpan.ToString()));
			timeSpan = new TimeSpan(Stopwatch.GetTimestamp() - this.lastActionTakenTime.RawValue);
			xelement.Add(new XElement("LastActionTakenElapsedTime", timeSpan.ToString()));
			return xelement;
		}

		private void IncrementActionPerfCounter(InterceptorAgentRuleBehavior actions)
		{
			if (actions.HasFlag(InterceptorAgentRuleBehavior.Archive))
			{
				this.actionPerfCountersGroup.Increment(InterceptorAgentRuleBehavior.Archive);
			}
			else if (actions.HasFlag(InterceptorAgentRuleBehavior.ArchiveHeaders))
			{
				this.actionPerfCountersGroup.Increment(InterceptorAgentRuleBehavior.ArchiveHeaders);
			}
			this.actionPerfCountersGroup.Increment(actions & ~(InterceptorAgentRuleBehavior.Archive | InterceptorAgentRuleBehavior.ArchiveHeaders));
			this.lastActionTakenTime.RawValue = Stopwatch.GetTimestamp();
		}

		private void Verify()
		{
			foreach (KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent> keyValuePair in InterceptorAgentRule.InvalidConditionEventPairs)
			{
				if ((ushort)(keyValuePair.Value & this.Events) != 0)
				{
					foreach (InterceptorAgentCondition interceptorAgentCondition in this.Conditions)
					{
						if (keyValuePair.Key == interceptorAgentCondition.Property)
						{
							string message = string.Format("Condition '{0}' does not support event '{1}'", interceptorAgentCondition.Property, keyValuePair.Value);
							throw new InvalidOperationException(message);
						}
					}
				}
			}
			foreach (KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent> keyValuePair2 in InterceptorAgentRule.InvalidActionEventPairs)
			{
				if ((ushort)(keyValuePair2.Value & this.Events) != 0)
				{
					InterceptorAgentAction interceptorAgentAction = this.Action;
					if (interceptorAgentAction != null && keyValuePair2.Key == interceptorAgentAction.Action)
					{
						string message = string.Format("Action '{0}' does not support event '{1}'", interceptorAgentAction.Action, keyValuePair2.Value & this.Events);
						throw new InvalidOperationException(message);
					}
				}
			}
			foreach (InterceptorAgentCondition interceptorAgentCondition2 in this.conditions)
			{
				interceptorAgentCondition2.Verify();
			}
			this.action.Verify();
			this.VerifyRequiredFields();
		}

		private void VerifyRequiredFields()
		{
			string text = null;
			if (string.IsNullOrEmpty(this.Name))
			{
				text = "Name";
			}
			else if (this.Action == null)
			{
				text = "Action";
			}
			else if (this.Events == InterceptorAgentEvent.Invalid)
			{
				text = "Event";
			}
			else if (this.Conditions == null)
			{
				text = "Conditions";
			}
			else if (string.IsNullOrEmpty(this.Description))
			{
				text = "Description";
			}
			if (!string.IsNullOrEmpty(text))
			{
				throw new MissingMemberException(string.Format("'{0}' is missing. This rule can not be used on this server.", text));
			}
		}

		private bool ConditionsEqual(InterceptorAgentRule other)
		{
			ArgumentValidator.ThrowIfNull("other", other);
			return this.conditions.Count == other.conditions.Count && !this.conditions.Except(other.conditions).Any<InterceptorAgentCondition>() && !other.conditions.Except(this.conditions).Any<InterceptorAgentCondition>();
		}

		internal const string RuleStartTime = "Rule Start Time";

		internal const string LastActionTakenTime = "Last Action Taken Time";

		internal static readonly Version Version = new Version(1, 2);

		internal static readonly string DefaultUser = "NotSpecified";

		private static readonly TimeSpan RuleExpirationWarningInterval = TimeSpan.FromHours(24.0);

		private static readonly InterceptorAgentEvent CanDropOrDeferEvents = InterceptorAgentEvent.OnSubmittedMessage | InterceptorAgentEvent.OnResolvedMessage | InterceptorAgentEvent.OnRoutedMessage | InterceptorAgentEvent.OnCategorizedMessage;

		private static readonly InterceptorAgentEvent CanTransientlyRejectEvents = InterceptorAgentEvent.OnMailFrom | InterceptorAgentEvent.OnRcptTo | InterceptorAgentEvent.OnEndOfHeaders | InterceptorAgentEvent.OnEndOfData;

		private static readonly InterceptorAgentEvent CanDropEvents = (InterceptorAgentEvent)((ushort)(InterceptorAgentRule.CanDropOrDeferEvents | InterceptorAgentEvent.OnLoadedMessage) | 4 | 8 | 256 | 8192 | 1024);

		private static readonly InterceptorAgentEvent CanArchiveEvents = InterceptorAgentEvent.OnEndOfData | InterceptorAgentEvent.OnSubmittedMessage | InterceptorAgentEvent.OnResolvedMessage | InterceptorAgentEvent.OnRoutedMessage | InterceptorAgentEvent.OnCategorizedMessage | InterceptorAgentEvent.OnInitMsg | InterceptorAgentEvent.OnPromotedMessage | InterceptorAgentEvent.OnCreatedMessage | InterceptorAgentEvent.OnDemotedMessage;

		private static readonly InterceptorAgentEvent CanArchiveHeadersEvents = InterceptorAgentRule.CanArchiveEvents | InterceptorAgentEvent.OnEndOfHeaders;

		private static readonly InterceptorAgentEvent CanArchiveAndTransientRejectEvents = InterceptorAgentRule.CanArchiveEvents & InterceptorAgentRule.CanTransientlyRejectEvents;

		private static readonly InterceptorAgentEvent CanArchiveAndDropEvents = InterceptorAgentRule.CanArchiveEvents & InterceptorAgentRule.CanDropEvents;

		private static readonly InterceptorAgentEvent CanArchiveHeadersAndDropEvents = InterceptorAgentRule.CanArchiveHeadersEvents & InterceptorAgentRule.CanDropEvents;

		private static readonly InterceptorAgentEvent CanArchiveHeadersAndTransientRejectEvents = InterceptorAgentRule.CanArchiveHeadersEvents & InterceptorAgentRule.CanTransientlyRejectEvents;

		private static readonly List<KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>> invalidConditionEventPairs = new List<KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>>
		{
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.MessageSubject, InterceptorAgentEvent.OnMailFrom),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.MessageSubject, InterceptorAgentEvent.OnRcptTo),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.MessageId, InterceptorAgentEvent.OnMailFrom),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.MessageId, InterceptorAgentEvent.OnRcptTo),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.EnvelopeTo, InterceptorAgentEvent.OnMailFrom),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.HeaderName, InterceptorAgentEvent.OnMailFrom),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.HeaderValue, InterceptorAgentEvent.OnMailFrom),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.HeaderName, InterceptorAgentEvent.OnRcptTo),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.HeaderValue, InterceptorAgentEvent.OnRcptTo),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.HeaderName, InterceptorAgentEvent.OnLoadedMessage),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.HeaderValue, InterceptorAgentEvent.OnLoadedMessage),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.SmtpClientHostName, InterceptorAgentEvent.OnSubmittedMessage),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.SmtpClientHostName, InterceptorAgentEvent.OnResolvedMessage),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.SmtpClientHostName, InterceptorAgentEvent.OnRoutedMessage),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.SmtpClientHostName, InterceptorAgentEvent.OnCategorizedMessage),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.SmtpClientHostName, InterceptorAgentEvent.OnInitMsg),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.SmtpClientHostName, InterceptorAgentEvent.OnPromotedMessage),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.SmtpClientHostName, InterceptorAgentEvent.OnCreatedMessage),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.SmtpClientHostName, InterceptorAgentEvent.OnDemotedMessage),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.SmtpClientHostName, InterceptorAgentEvent.OnLoadedMessage),
			new KeyValuePair<InterceptorAgentConditionType, InterceptorAgentEvent>(InterceptorAgentConditionType.AccountForest, InterceptorAgentEvent.OnMailFrom)
		};

		private static readonly List<KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>> invalidActionEventPairs = new List<KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>>
		{
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.Drop, ~InterceptorAgentRule.CanDropEvents),
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.Defer, ~InterceptorAgentRule.CanDropOrDeferEvents),
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.TransientReject, ~InterceptorAgentRule.CanTransientlyRejectEvents),
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.Delay, InterceptorAgentEvent.OnLoadedMessage),
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.Archive, ~InterceptorAgentRule.CanArchiveEvents),
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.ArchiveHeaders, ~InterceptorAgentRule.CanArchiveHeadersEvents),
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.ArchiveAndTransientReject, ~InterceptorAgentRule.CanArchiveAndTransientRejectEvents),
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.ArchiveAndPermanentReject, ~InterceptorAgentRule.CanArchiveEvents),
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.ArchiveAndDrop, ~InterceptorAgentRule.CanArchiveAndDropEvents),
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.ArchiveHeadersAndDrop, ~InterceptorAgentRule.CanArchiveHeadersAndDropEvents),
			new KeyValuePair<InterceptorAgentRuleBehavior, InterceptorAgentEvent>(InterceptorAgentRuleBehavior.ArchiveHeadersAndTransientReject, ~InterceptorAgentRule.CanArchiveHeadersAndTransientRejectEvents)
		};

		private static XmlSerializer serializer = new XmlSerializer(typeof(InterceptorAgentRule));

		private string name = string.Empty;

		private string description;

		private SourceType source;

		private string createdBy = InterceptorAgentRule.DefaultUser;

		private InterceptorAgentEvent events;

		private List<InterceptorAgentCondition> conditions;

		private InterceptorAgentAction action;

		private Version version;

		private MultiValuedProperty<ADObjectId> target;

		private DateTime expireTimeUtc;

		private DateTime expireTime;

		private InterceptorCountersGroup eventPerfCountersGroup;

		private InterceptorCountersGroup actionPerfCountersGroup;

		private MemoryCounter ruleStartTime;

		private MemoryCounter lastActionTakenTime;

		private bool isActive;

		private ObjectId identity;

		private string distinguishedName;

		private ADObjectId objectCategory;

		private string adminDisplayName;

		private Guid guid;

		private DateTime? whenChangedUtc;

		private DateTime? whenChanged;

		private DateTime? whenCreatedUtc;

		private DateTime? whenCreated;
	}
}
