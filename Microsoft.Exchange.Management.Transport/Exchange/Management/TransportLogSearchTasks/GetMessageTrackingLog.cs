using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Tracking;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Management.Tracking;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.Search;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Management.TransportLogSearchTasks
{
	[OutputType(new Type[]
	{
		typeof(MessageTrackingEvent)
	})]
	[Cmdlet("Get", "MessageTrackingLog")]
	public sealed class GetMessageTrackingLog : LogSearchTask
	{
		private ADObjectId RootOrgContainerId
		{
			get
			{
				if (this.rootOrgContainerId == null)
				{
					this.rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
				}
				return this.rootOrgContainerId;
			}
		}

		public GetMessageTrackingLog() : base(Strings.MessageTrackingActivityName, MessageTrackingSchema.MessageTrackingEvent, "MSGTRK")
		{
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public string EventId
		{
			get
			{
				return this.eventId;
			}
			set
			{
				this.eventId = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public string Sender
		{
			get
			{
				return this.sender;
			}
			set
			{
				this.sender = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public string[] Recipients
		{
			get
			{
				return this.recipients;
			}
			set
			{
				this.recipients = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public string MessageId
		{
			get
			{
				return this.messageId;
			}
			set
			{
				this.messageId = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public string InternalMessageId
		{
			get
			{
				return this.internalMessageId;
			}
			set
			{
				this.internalMessageId = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public string MessageSubject
		{
			get
			{
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Default")]
		public string Reference
		{
			get
			{
				return this.reference;
			}
			set
			{
				this.reference = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (base.NeedSuppressingPiiData && base.ExchangeRunspaceConfig != null)
			{
				base.ExchangeRunspaceConfig.EnablePiiMap = true;
				this.ResolvePiiParameters();
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.eventId != null)
			{
				bool flag = false;
				foreach (string a in GetMessageTrackingLog.TrackingEventTypes)
				{
					if (string.Equals(a, this.eventId, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					base.ThrowTerminatingError(new LocalizedException(Strings.EventIdNotFound(this.eventId)), ErrorCategory.InvalidArgument, this.eventId);
				}
			}
		}

		protected override LogCondition GetCondition()
		{
			LogAndCondition logAndCondition = new LogAndCondition();
			if (this.eventId != null)
			{
				logAndCondition.Conditions.Add(this.GetFieldStringComparison(MessageTrackingField.EventId, this.eventId));
			}
			if (this.sender != null)
			{
				logAndCondition.Conditions.Add(this.GetSenderCondition());
			}
			if (this.recipients != null)
			{
				logAndCondition.Conditions.Add(this.GetRecipientsCondition());
			}
			if (this.messageId != null)
			{
				logAndCondition.Conditions.Add(this.GetFieldStringComparison(MessageTrackingField.MessageId, CsvFieldCache.NormalizeMessageID(this.messageId)));
			}
			if (this.internalMessageId != null)
			{
				logAndCondition.Conditions.Add(this.GetFieldStringComparison(MessageTrackingField.InternalMessageId, this.internalMessageId));
			}
			if (this.subject != null)
			{
				LogConditionField logConditionField = new LogConditionField();
				logConditionField.Name = base.Table.Fields[18].Name;
				LogConditionConstant logConditionConstant = new LogConditionConstant();
				logConditionConstant.Value = this.subject;
				LogStringContainsCondition logStringContainsCondition = new LogStringContainsCondition();
				logStringContainsCondition.Left = logConditionField;
				logStringContainsCondition.Right = logConditionConstant;
				logStringContainsCondition.IgnoreCase = true;
				logAndCondition.Conditions.Add(logStringContainsCondition);
			}
			if (this.reference != null)
			{
				logAndCondition.Conditions.Add(this.GetFieldForAnyCondition(MessageTrackingField.Reference, this.reference, "e"));
			}
			return logAndCondition;
		}

		protected override void WriteResult(LogSearchCursor cursor)
		{
			object field = cursor.GetField(0);
			if (!(field is DateTime))
			{
				return;
			}
			MessageTrackingEvent messageTrackingEvent = new MessageTrackingEvent();
			messageTrackingEvent.Timestamp = ((DateTime)field).ToLocalTime();
			messageTrackingEvent.ClientIp = (cursor.GetField(1) as string);
			messageTrackingEvent.ClientHostname = (string)cursor.GetField(2);
			messageTrackingEvent.ServerIp = (cursor.GetField(3) as string);
			messageTrackingEvent.ServerHostname = (cursor.GetField(4) as string);
			messageTrackingEvent.SourceContext = (cursor.GetField(5) as string);
			messageTrackingEvent.ConnectorId = (cursor.GetField(6) as string);
			messageTrackingEvent.Source = (cursor.GetField(7) as string);
			messageTrackingEvent.EventId = (cursor.GetField(8) as string);
			messageTrackingEvent.InternalMessageId = (cursor.GetField(9) as string);
			messageTrackingEvent.MessageId = (cursor.GetField(10) as string);
			messageTrackingEvent.Recipients = (cursor.GetField(12) as string[]);
			this.RedactPiiStringArrayIfNeeded(messageTrackingEvent.Recipients);
			messageTrackingEvent.RecipientStatus = (cursor.GetField(13) as string[]);
			if (messageTrackingEvent.EventId == "DELIVER" || messageTrackingEvent.EventId == "DUPLICATEDELIVER")
			{
				this.RedactPiiStringArrayIfNeeded(messageTrackingEvent.RecipientStatus);
			}
			messageTrackingEvent.TotalBytes = GetMessageTrackingLog.Unbox<int>(cursor.GetField(14));
			messageTrackingEvent.RecipientCount = GetMessageTrackingLog.Unbox<int>(cursor.GetField(15));
			messageTrackingEvent.RelatedRecipientAddress = this.RedactPiiStringIfNeeded(cursor.GetField(16) as string, false);
			messageTrackingEvent.Reference = (cursor.GetField(17) as string[]);
			if (messageTrackingEvent.Reference != null && messageTrackingEvent.Reference.Length == 1 && string.IsNullOrEmpty(messageTrackingEvent.Reference[0]))
			{
				messageTrackingEvent.Reference = null;
			}
			messageTrackingEvent.MessageSubject = this.RedactPiiStringIfNeeded(cursor.GetField(18) as string, true);
			messageTrackingEvent.Sender = this.RedactPiiStringIfNeeded(cursor.GetField(19) as string, false);
			messageTrackingEvent.ReturnPath = this.RedactPiiStringIfNeeded(cursor.GetField(20) as string, false);
			messageTrackingEvent.Directionality = (cursor.GetField(22) as string);
			messageTrackingEvent.TenantId = (cursor.GetField(23) as string);
			messageTrackingEvent.OriginalClientIp = (cursor.GetField(24) as string);
			messageTrackingEvent.MessageInfo = (cursor.GetField(21) as string);
			messageTrackingEvent.EventData = (cursor.GetField(26) as KeyValuePair<string, object>[]);
			if (base.NeedSuppressingPiiData)
			{
				Utils.RedactEventData(messageTrackingEvent.EventData, GetMessageTrackingLog.EventDataKeyToRedact, this);
			}
			GetMessageTrackingLog.CalculateLatencyProperties(messageTrackingEvent);
			base.WriteObject(messageTrackingEvent);
		}

		private string RedactPiiStringIfNeeded(string original, bool caseInvariant)
		{
			if (!base.NeedSuppressingPiiData)
			{
				return original;
			}
			if (caseInvariant)
			{
				original = original.ToUpperInvariant();
			}
			return Utils.RedactPiiString(original, this);
		}

		private void RedactPiiStringArrayIfNeeded(string[] original)
		{
			if (!base.NeedSuppressingPiiData)
			{
				return;
			}
			Utils.RedactPiiStringArray(original, this);
		}

		private void ResolvePiiParameters()
		{
			string text;
			if (Utils.TryResolveRedactedString(this.sender, this, out text))
			{
				this.sender = text;
			}
			if (Utils.TryResolveRedactedString(this.subject, this, out text))
			{
				this.subject = text;
			}
			Utils.TryResolveRedactedStringArray(this.recipients, this);
		}

		private string[] GetProxyAddresses(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return null;
			}
			int num = 0;
			string[] array = null;
			Exception ex = null;
			ADRawEntry adrawEntry = null;
			ProxyAddress proxyAddress = ProxyAddress.Parse(address);
			if (proxyAddress is InvalidProxyAddress)
			{
				ex = ((InvalidProxyAddress)proxyAddress).ParseException;
				LocalizedException exception = new LocalizedException(Strings.WarningProxyAddressIsInvalid(address, ex.Message));
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				return null;
			}
			if (this.recipSession == null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(SmtpAddress.Parse(address).Domain);
				this.recipSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.FullyConsistent, sessionSettings, 977, "GetProxyAddresses", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\LogSearch\\GetMessageTrackingLog.cs");
				if (!this.recipSession.IsReadConnectionAvailable())
				{
					this.recipSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, sessionSettings, 986, "GetProxyAddresses", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\LogSearch\\GetMessageTrackingLog.cs");
				}
			}
			AcceptedDomain acceptedDomain = null;
			bool flag;
			do
			{
				flag = false;
				try
				{
					this.GetRecipientInformation(proxyAddress.ToString(), out adrawEntry, out acceptedDomain);
				}
				catch (DataValidationException ex2)
				{
					ex = ex2;
				}
				catch (TransientException ex3)
				{
					if (num < 3)
					{
						flag = true;
						num++;
						Thread.Sleep(1000);
					}
					else
					{
						ex = ex3;
					}
				}
			}
			while (flag);
			if (ex != null)
			{
				this.WriteWarning(Strings.WarningProxyListUnavailable(address, ex.GetType().Name + ": " + ex.Message));
				return null;
			}
			if (adrawEntry == null)
			{
				return null;
			}
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)adrawEntry[ADRecipientSchema.EmailAddresses];
			string address2 = (string)adrawEntry[ADRecipientSchema.LegacyExchangeDN];
			array = new string[proxyAddressCollection.Count + 1];
			int num2 = 0;
			SmtpProxyAddress smtpProxyAddress;
			if (acceptedDomain != null && SmtpProxyAddress.TryEncapsulate(ProxyAddressPrefix.LegacyDN.PrimaryPrefix, address2, acceptedDomain.DomainName.Domain, out smtpProxyAddress))
			{
				array[0] = smtpProxyAddress.AddressString;
			}
			foreach (ProxyAddress proxyAddress2 in proxyAddressCollection)
			{
				num2++;
				SmtpProxyAddress smtpProxyAddress2 = null;
				if (proxyAddress2.Prefix == ProxyAddressPrefix.Smtp)
				{
					array[num2] = proxyAddress2.AddressString;
				}
				else if (acceptedDomain != null && SmtpProxyAddress.TryEncapsulate(proxyAddress2, acceptedDomain.DomainName.Domain, out smtpProxyAddress2))
				{
					array[num2] = smtpProxyAddress2.AddressString;
				}
			}
			return array;
		}

		private LogCondition GetSenderCondition()
		{
			string[] proxyAddresses = this.GetProxyAddresses(this.sender);
			if (proxyAddresses == null || proxyAddresses.Length == 1)
			{
				return this.GetFieldStringComparison(MessageTrackingField.SenderAddress, this.sender);
			}
			LogOrCondition logOrCondition = new LogOrCondition();
			foreach (string text in proxyAddresses)
			{
				if (text != null)
				{
					logOrCondition.Conditions.Add(this.GetFieldStringComparison(MessageTrackingField.SenderAddress, text));
				}
			}
			return logOrCondition;
		}

		private LogCondition GetRecipientsCondition()
		{
			LogOrCondition logOrCondition = new LogOrCondition();
			for (int i = 0; i < this.recipients.Length; i++)
			{
				if (this.recipients[i] != null)
				{
					string[] proxyAddresses = this.GetProxyAddresses(this.recipients[i]);
					if (proxyAddresses == null)
					{
						logOrCondition.Conditions.Add(this.GetFieldForAnyCondition(MessageTrackingField.RecipientAddress, this.recipients[i], "r" + i));
					}
					else
					{
						for (int j = 0; j < proxyAddresses.Length; j++)
						{
							if (proxyAddresses[j] != null)
							{
								logOrCondition.Conditions.Add(this.GetFieldForAnyCondition(MessageTrackingField.RecipientAddress, proxyAddresses[j], string.Concat(new object[]
								{
									"r",
									i,
									"p",
									j
								})));
							}
						}
					}
				}
			}
			return logOrCondition;
		}

		private LogStringComparisonCondition GetFieldStringComparison(MessageTrackingField field, string value)
		{
			LogConditionField logConditionField = new LogConditionField();
			logConditionField.Name = base.Table.Fields[(int)field].Name;
			LogConditionConstant logConditionConstant = new LogConditionConstant();
			logConditionConstant.Value = value;
			return new LogStringComparisonCondition
			{
				Left = logConditionField,
				Right = logConditionConstant,
				IgnoreCase = true,
				Operator = LogComparisonOperator.Equals
			};
		}

		private LogForAnyCondition GetFieldForAnyCondition(MessageTrackingField field, string value, string variableName)
		{
			LogConditionField logConditionField = new LogConditionField();
			logConditionField.Name = base.Table.Fields[(int)field].Name;
			LogConditionConstant logConditionConstant = new LogConditionConstant();
			logConditionConstant.Value = value;
			LogConditionVariable logConditionVariable = new LogConditionVariable();
			logConditionVariable.Name = variableName;
			LogStringComparisonCondition logStringComparisonCondition = new LogStringComparisonCondition();
			logStringComparisonCondition.Left = logConditionVariable;
			logStringComparisonCondition.Right = logConditionConstant;
			logStringComparisonCondition.IgnoreCase = true;
			logStringComparisonCondition.Operator = LogComparisonOperator.Equals;
			return new LogForAnyCondition
			{
				Field = logConditionField,
				Variable = logConditionVariable,
				Condition = logStringComparisonCondition
			};
		}

		private void GetRecipientInformation(string proxyAddress, out ADRawEntry adRecipientEntry, out AcceptedDomain defaultDomain)
		{
			adRecipientEntry = null;
			defaultDomain = null;
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, proxyAddress.ToString());
			ADPagedReader<ADRecipient> adpagedReader = this.recipSession.FindPaged(null, QueryScope.SubTree, filter, null, 0);
			using (IEnumerator<ADRecipient> enumerator = adpagedReader.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					adRecipientEntry = enumerator.Current;
					OrganizationId organizationId = ((ADRecipient)adRecipientEntry).OrganizationId;
					IConfigurationSession configurationSession = this.CreateConfigurationSession(organizationId);
					defaultDomain = configurationSession.GetDefaultAcceptedDomain();
					if (defaultDomain == null)
					{
						throw new DataValidationException(new ObjectValidationError(Strings.ErrorNoDefaultAcceptedDomainFound(organizationId.ToString()), null, null));
					}
				}
			}
		}

		private IConfigurationSession CreateConfigurationSession(OrganizationId orgId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.RootOrgContainerId, orgId, base.ExecutingUserOrganizationId, false);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.FullyConsistent, sessionSettings, 1280, "CreateConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\LogSearch\\GetMessageTrackingLog.cs");
		}

		private static T? Unbox<T>(object obj) where T : struct
		{
			if (obj == null || !(obj is T))
			{
				return null;
			}
			return new T?((T)((object)obj));
		}

		private static void CalculateLatencyProperties(MessageTrackingEvent mte)
		{
			string messageInfo = mte.MessageInfo;
			if (string.IsNullOrEmpty(messageInfo) || (mte.EventId != "DELIVER" && mte.EventId != "SEND" && mte.EventId != "SUBMIT" && (mte.EventId != "RESUBMIT" || mte.Source != "STOREDRIVER")))
			{
				mte.MessageLatency = null;
				mte.MessageLatencyType = MessageLatencyType.None;
				return;
			}
			GetMessageTrackingLog.MessageLatencyParser messageLatencyParser = new GetMessageTrackingLog.MessageLatencyParser();
			if (messageLatencyParser.TryParse(messageInfo))
			{
				if (messageLatencyParser.OriginalArrivalTime != DateTime.MinValue)
				{
					mte.MessageLatency = new EnhancedTimeSpan?((mte.Timestamp.ToUniversalTime() > messageLatencyParser.OriginalArrivalTime.ToUniversalTime()) ? (mte.Timestamp.ToUniversalTime() - messageLatencyParser.OriginalArrivalTime.ToUniversalTime()) : TimeSpan.Zero);
				}
				else
				{
					mte.MessageLatency = null;
				}
				mte.MessageLatencyType = messageLatencyParser.MessageLatencyType;
				return;
			}
			mte.MessageLatency = null;
			mte.MessageLatencyType = MessageLatencyType.None;
		}

		private const int AdRetryCount = 3;

		private const int AdRetryIntervalMsec = 1000;

		private static readonly PropertyDefinition[] PropertiesToGet = new PropertyDefinition[]
		{
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.LegacyExchangeDN
		};

		private static readonly string[] TrackingEventTypes = Enum.GetNames(typeof(MessageTrackingEvent));

		private static readonly HashSet<string> EventDataKeyToRedact = new HashSet<string>
		{
			"PurportedSender"
		};

		private IRecipientSession recipSession;

		private string eventId;

		private string sender;

		private string[] recipients;

		private string messageId;

		private string internalMessageId;

		private string subject;

		private string reference;

		private ADObjectId rootOrgContainerId;

		private sealed class MessageLatencyParser : LatencyParser
		{
			public MessageLatencyParser() : base(ExTraceGlobals.TaskTracer)
			{
				this.originalArrivalTime = DateTime.MinValue;
				this.messageLatencyType = MessageLatencyType.None;
			}

			public bool TryParse(string s)
			{
				if (string.IsNullOrEmpty(s))
				{
					return false;
				}
				bool flag = false;
				bool flag2 = false;
				DateTime dateTime;
				int num;
				if (ComponentLatencyParser.TryParseOriginalArrivalTime(s, out dateTime, out num))
				{
					this.originalArrivalTime = dateTime;
					flag = true;
				}
				if (num < s.Length)
				{
					flag2 = base.TryParse(s, num, s.Length - num);
				}
				return flag || flag2;
			}

			public DateTime OriginalArrivalTime
			{
				get
				{
					return this.originalArrivalTime;
				}
			}

			public MessageLatencyType MessageLatencyType
			{
				get
				{
					return this.messageLatencyType;
				}
			}

			protected override bool HandleLocalServerFqdn(string s, int startIndex, int count)
			{
				this.messageLatencyType = MessageLatencyType.LocalServer;
				return true;
			}

			protected override bool HandleServerFqdn(string s, int startIndex, int count)
			{
				this.messageLatencyType = MessageLatencyType.EndToEnd;
				return true;
			}

			private DateTime originalArrivalTime;

			private MessageLatencyType messageLatencyType;
		}
	}
}
