using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.FfoReporting.Common;
using Microsoft.Exchange.Management.FfoReporting.Data;
using Microsoft.Exchange.Management.FfoReporting.Providers;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[OutputType(new Type[]
	{
		typeof(MessageTraceDetail)
	})]
	[Cmdlet("Get", "MessageTraceDetail")]
	public sealed class GetMessageTraceDetail : MtrtTask<MessageTraceDetail>
	{
		public GetMessageTraceDetail() : base("Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports.MessageTraceDetail, Microsoft.Exchange.Hygiene.Data")
		{
			this.MessageTraceId = Guid.Empty;
			this.MessageId = null;
			this.RecipientAddress = null;
			this.SenderAddress = null;
			this.Action = new MultiValuedProperty<string>();
			this.Event = new MultiValuedProperty<string>();
			this.functionMap.Add("receive", (GetMessageTraceDetail.DalPlaceholder dalObject) => this.GetMessageTraceMail(dalObject, "MtrtMessageRecieve", Strings.MtrtNoDetailInformation, new string[]
			{
				"ServerHostName"
			}));
			this.functionMap.Add("send", (GetMessageTraceDetail.DalPlaceholder dalObject) => this.GetMessageTraceMail(dalObject, "MtrtMessageSend", Strings.MtrtNoDetailInformation, new string[]
			{
				"ConnectorId"
			}));
			this.functionMap.Add("deliver", (GetMessageTraceDetail.DalPlaceholder dalObject) => this.GetMessageTraceMail(dalObject, "MtrtMessageDeliverDetailMessage", Strings.MtrtNoDetailInformation, null));
			this.functionMap.Add("badmail", (GetMessageTraceDetail.DalPlaceholder dalObject) => this.GetMessageTraceMail(dalObject, "MtrtMessageBadmail", Strings.MtrtNoDetailInformation, new string[0]));
			this.functionMap.Add("expand", (GetMessageTraceDetail.DalPlaceholder dalObject) => this.GetMessageTraceMail(dalObject, "MtrtMessageExpandDetailMessage", Strings.MtrtNoDetailInformation, null));
			this.functionMap.Add("submit", (GetMessageTraceDetail.DalPlaceholder dalObject) => this.GetMessageTraceMail(dalObject, "MtrtMessageSubmitDetailMessage", Strings.MtrtNoDetailInformation, null));
			this.functionMap.Add("defer", (GetMessageTraceDetail.DalPlaceholder dalObject) => this.GetMessageTraceMail(dalObject, "MtrtMessageFailureReason", Strings.MtrtMessageDefer, new string[]
			{
				"RecipientStatus"
			}));
			this.functionMap.Add("fail", (GetMessageTraceDetail.DalPlaceholder dalObject) => this.GetMessageTraceMail(dalObject, "MtrtMessageFailureReason", Strings.MtrtMessageFail, new string[]
			{
				"RecipientStatus"
			}));
			this.functionMap.Add("agentinfo", new GetMessageTraceDetail.ParseDataDelegate(this.GetAgentInfoMessageTrace));
			this.functionMap.Add("ama", new GetMessageTraceDetail.ParseDataDelegate(this.GetMalwareMessageTrace));
			this.functionMap.Add("sfa", new GetMessageTraceDetail.ParseDataDelegate(this.GetSpamMessageTrace));
			this.functionMap.Add("tra", new GetMessageTraceDetail.ParseDataDelegate(this.GetTransportRuleTrace));
		}

		[QueryParameter("InternalMessageIdQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateRequiredField", new object[]
		{

		})]
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public Guid MessageTraceId { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		[QueryParameter("ClientMessageIdQueryDefinition", new string[]
		{

		})]
		public string MessageId { get; set; }

		[QueryParameter("SenderAddressQueryDefinition", new string[]
		{

		})]
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		[CmdletValidator("ValidateEmailAddress", new object[]
		{
			CmdletValidator.EmailAddress.Sender,
			CmdletValidator.WildcardValidationOptions.Disallow
		})]
		public string SenderAddress { get; set; }

		[CmdletValidator("ValidateEmailAddress", new object[]
		{
			CmdletValidator.EmailAddress.Recipient,
			CmdletValidator.WildcardValidationOptions.Disallow
		})]
		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		[QueryParameter("RecipientAddressQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateRequiredField", new object[]
		{

		})]
		public string RecipientAddress { get; set; }

		[Parameter(Mandatory = false)]
		[CmdletValidator("ValidateEnum", new object[]
		{
			typeof(Schema.Actions)
		}, ErrorMessage = Strings.IDs.InvalidActionParameter)]
		[QueryParameter("ActionListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> Action { get; set; }

		[Parameter(Mandatory = false)]
		[QueryParameter("EventListQueryDefinition", new string[]
		{

		})]
		public MultiValuedProperty<string> Event { get; set; }

		protected override void CustomInternalValidate()
		{
			base.CustomInternalValidate();
			if (!string.IsNullOrEmpty(this.MessageId))
			{
				bool flag = this.MessageId[0] != '<' && this.MessageId[this.MessageId.Length - 1] != '>';
				if (flag)
				{
					this.MessageId = '<' + this.MessageId + '>';
				}
			}
		}

		protected override IReadOnlyList<MessageTraceDetail> AggregateOutput()
		{
			IEnumerable dalRecords = base.GetDalRecords(new FfoReportingDalTask<MessageTraceDetail>.DalRetrievalDelegate(ServiceLocator.Current.GetService<IDalProvider>().GetSingleDataPage), null);
			IReadOnlyList<GetMessageTraceDetail.DalPlaceholder> readOnlyList = DataProcessorDriver.Process<GetMessageTraceDetail.DalPlaceholder>(dalRecords, ConversionProcessor.Create<GetMessageTraceDetail.DalPlaceholder>(this));
			List<MessageTraceDetail> list = new List<MessageTraceDetail>();
			foreach (GetMessageTraceDetail.DalPlaceholder dalPlaceholder in readOnlyList)
			{
				GetMessageTraceDetail.ParseDataDelegate parseDataDelegate;
				if (string.IsNullOrEmpty(dalPlaceholder.EventDescription))
				{
					base.Diagnostics.TraceWarning("Unknown EventDescription");
				}
				else if (this.functionMap.TryGetValue(dalPlaceholder.EventDescription.ToLower(), out parseDataDelegate))
				{
					MessageTraceDetail messageTraceDetail = parseDataDelegate(dalPlaceholder);
					if (messageTraceDetail != null)
					{
						list.Add(messageTraceDetail);
						base.Diagnostics.Checkpoint(dalPlaceholder.EventDescription);
					}
					else
					{
						base.Diagnostics.TraceWarning(string.Format("EventDescription not defined[{0}]", dalPlaceholder.EventDescription));
					}
				}
			}
			return list;
		}

		internal MessageTraceDetail GetMessageTraceMail(GetMessageTraceDetail.DalPlaceholder dalObject, string detailMessage, string defaultDetailMessage, params string[] propertyNames)
		{
			MessageTraceDetail messageTraceDetail = this.CreateMessageTraceDetail(dalObject);
			if (string.IsNullOrEmpty(detailMessage))
			{
				base.Diagnostics.TraceWarning("Unknown detail message");
				messageTraceDetail.Detail = Strings.MtrtNoDetailInformation;
			}
			else if (propertyNames == null || propertyNames.Length == 0)
			{
				PropertyInfo property = typeof(Strings).GetProperty(detailMessage, BindingFlags.Static | BindingFlags.Public);
				messageTraceDetail.Detail = (LocalizedString)property.GetValue(null, null);
			}
			else
			{
				List<string> list = this.ParseXml(dalObject.Data, propertyNames);
				if (list.Count > 0)
				{
					MethodInfo method = typeof(Strings).GetMethod(detailMessage, BindingFlags.Static | BindingFlags.Public);
					messageTraceDetail.Detail = (LocalizedString)Schema.Utilities.Invoke(method, null, list.ToArray());
				}
				else
				{
					base.Diagnostics.TraceWarning("No detail xml");
					if (string.IsNullOrWhiteSpace(defaultDetailMessage))
					{
						base.Diagnostics.TraceWarning("Unknown default detail message");
						messageTraceDetail.Detail = Strings.MtrtNoDetailInformation;
					}
					else
					{
						messageTraceDetail.Detail = defaultDetailMessage;
					}
				}
			}
			return messageTraceDetail;
		}

		internal MessageTraceDetail GetAgentInfoMessageTrace(GetMessageTraceDetail.DalPlaceholder dalObject)
		{
			GetMessageTraceDetail.ParseDataDelegate parseDataDelegate;
			if (dalObject.AgentName != null && this.functionMap.TryGetValue(dalObject.AgentName.ToLower(), out parseDataDelegate))
			{
				return parseDataDelegate(dalObject);
			}
			string arg = (dalObject.AgentName == null) ? "Null" : dalObject.AgentName;
			base.Diagnostics.TraceWarning(string.Format("Unknown AgentName[{0}]", arg));
			return null;
		}

		internal MessageTraceDetail GetSpamMessageTrace(GetMessageTraceDetail.DalPlaceholder dalObject)
		{
			string detailMessage = string.Empty;
			string text = null;
			string action;
			switch (action = dalObject.Action)
			{
			case "sn":
				detailMessage = "MtrtMessageSpamNonProvisionedDomain";
				text = "SCL";
				break;
			case "st":
				detailMessage = "MtrtMessageSpamAdditional";
				break;
			case "sd":
			case "so":
			case "sq":
			case "sr":
			case "ss":
			case "sx":
				detailMessage = "MtrtMessageSpam";
				text = "SCL";
				break;
			}
			return this.GetMessageTraceMail(dalObject, detailMessage, Strings.MtrtNoDetailInformation, new string[]
			{
				text
			});
		}

		internal MessageTraceDetail GetMalwareMessageTrace(GetMessageTraceDetail.DalPlaceholder dalObject)
		{
			string action;
			if ((action = dalObject.Action) != null && (action == "b" || action == "r"))
			{
				return this.GetMessageTraceMail(dalObject, "MtrtMessageMalware", Strings.MtrtNoDetailInformation, new string[]
				{
					"name",
					"file"
				});
			}
			string arg = (dalObject.Action == null) ? "Null" : dalObject.Action;
			base.Diagnostics.TraceWarning(string.Format("Unknown Malware Action[{0}]", arg));
			return null;
		}

		internal MessageTraceDetail GetTransportRuleTrace(GetMessageTraceDetail.DalPlaceholder dalObject)
		{
			MessageTraceDetail messageTraceDetail = this.CreateMessageTraceDetail(dalObject);
			string ruleName = dalObject.RuleName ?? string.Empty;
			string id = dalObject.RuleId ?? string.Empty;
			if (string.IsNullOrWhiteSpace(dalObject.PolicyId))
			{
				messageTraceDetail.Detail = Strings.MtrtMessageTransportRule(ruleName, id);
			}
			else
			{
				string policyName = dalObject.PolicyName ?? string.Empty;
				string dlpid = dalObject.PolicyId ?? string.Empty;
				messageTraceDetail.Detail = Strings.MtrtMessageDLPRule(ruleName, id, policyName, dlpid);
			}
			return messageTraceDetail;
		}

		private MessageTraceDetail CreateMessageTraceDetail(GetMessageTraceDetail.DalPlaceholder dalObject)
		{
			MessageTraceDetail messageTraceDetail = new MessageTraceDetail();
			messageTraceDetail.Organization = dalObject.Organization;
			messageTraceDetail.MessageTraceId = dalObject.InternalMessageId;
			messageTraceDetail.MessageId = dalObject.MessageId;
			messageTraceDetail.Date = dalObject.EventDate;
			messageTraceDetail.Data = dalObject.Data;
			messageTraceDetail.Index = dalObject.Index;
			if (string.Equals(dalObject.EventDescription, "agentinfo", StringComparison.InvariantCultureIgnoreCase))
			{
				string @event;
				if (!string.IsNullOrEmpty(dalObject.AgentName) && this.eventNameMap.TryGetValue(dalObject.AgentName.ToLower(), out @event))
				{
					messageTraceDetail.Event = @event;
				}
				string action;
				if (!string.IsNullOrEmpty(dalObject.Action) && this.actionNameMap.TryGetValue(dalObject.Action.ToLower(), out action))
				{
					messageTraceDetail.Action = action;
				}
			}
			else
			{
				messageTraceDetail.Event = dalObject.EventDescription;
			}
			return messageTraceDetail;
		}

		private List<string> ParseXml(string xml, params string[] propertyNames)
		{
			List<string> list = new List<string>();
			bool flag = false;
			if (propertyNames != null && !string.IsNullOrWhiteSpace(xml))
			{
				XDocument xdocument = XDocument.Parse(xml);
				IEnumerable<XElement> source = xdocument.Descendants("MEP");
				int i = 0;
				while (i < propertyNames.Length)
				{
					string propertyName = propertyNames[i];
					XElement xelement = (from elem in source
					where string.Equals(elem.Attribute("Name").Value, propertyName, StringComparison.InvariantCultureIgnoreCase)
					select elem).FirstOrDefault<XElement>();
					if (xelement == null)
					{
						goto IL_BD;
					}
					XAttribute xattribute = (from att in xelement.Attributes()
					where !string.Equals(att.Name.LocalName, "Name", StringComparison.InvariantCultureIgnoreCase)
					select att).FirstOrDefault<XAttribute>();
					if (xattribute == null)
					{
						goto IL_BD;
					}
					list.Add(xattribute.Value);
					flag = true;
					IL_C8:
					i++;
					continue;
					IL_BD:
					list.Add(string.Empty);
					goto IL_C8;
				}
			}
			if (!flag)
			{
				list.Clear();
			}
			return list;
		}

		private Dictionary<string, string> eventNameMap = new Dictionary<string, string>
		{
			{
				"receive",
				Strings.MtrtEventReceive
			},
			{
				"send",
				Strings.MtrtEventSend
			},
			{
				"fail",
				Strings.MtrtEventFail
			},
			{
				"deliver",
				Strings.MtrtEventDeliver
			},
			{
				"badmail",
				Strings.MtrtEventBadmail
			},
			{
				"expand",
				Strings.MtrtEventExpand
			},
			{
				"submit",
				Strings.MtrtEventSubmit
			},
			{
				"defer",
				Strings.MtrtEventDefer
			},
			{
				"ama",
				Strings.MtrtEventMalware
			},
			{
				"sfa",
				Strings.MtrtEventSpam
			},
			{
				"tra",
				Strings.MtrtEventTransportRule
			}
		};

		private Dictionary<string, string> actionNameMap = new Dictionary<string, string>
		{
			{
				"blindcopyto",
				Strings.MtrtAddBlindCopyToRecipient
			},
			{
				"copyto",
				Strings.MtrtAddCopyToRecipient
			},
			{
				"sx",
				Strings.MtrtAddHeader
			},
			{
				"addmanagerasrecipienttype",
				Strings.MtrtAddManagerAsRecipient
			},
			{
				"addtorecipient",
				Strings.MtrtAddToRecipient
			},
			{
				"applyclassification ",
				Strings.MtrtApplyClassification
			},
			{
				"applyhtmldisclaimer",
				Strings.MtrtApplyHtmlDisclaimer
			},
			{
				"decrypt",
				Strings.MtrtDecrypt
			},
			{
				"r",
				Strings.MtrtDeleteAttachment
			},
			{
				"b",
				Strings.MtrtDeleteMessage
			},
			{
				"deletemessage",
				Strings.MtrtDeleteMessage
			},
			{
				"sd",
				Strings.MtrtDeleteMessage
			},
			{
				"generateincidentreport",
				Strings.MtrtGenerateIncidentReport
			},
			{
				"moderatemessagebymanager",
				Strings.MtrtModerateMessageByManager
			},
			{
				"moderatemessagebyuser",
				Strings.MtrtModerateMessageByUser
			},
			{
				"sendernotify",
				Strings.MtrtNotifySender
			},
			{
				"prependsubject",
				Strings.MtrtPrependSubject
			},
			{
				"ss",
				Strings.MtrtPrependSubject
			},
			{
				"quarantine",
				Strings.MtrtQuarantine
			},
			{
				"sq",
				Strings.MtrtQuarantine
			},
			{
				"redirectmessage",
				Strings.MtrtRedirectMessage
			},
			{
				"sr",
				Strings.MtrtRedirectMessage
			},
			{
				"rejectmessage",
				Strings.MtrtRejectMessage
			},
			{
				"removeheader",
				Strings.MtrtRemoveHeader
			},
			{
				"reportseveritylevelhigh",
				Strings.MtrtReportSeverityLevelHigh
			},
			{
				"reportseveritylevellow",
				Strings.MtrtReportSeverityLevelLow
			},
			{
				"reportseveritylevelmed",
				Strings.MtrtReportSeverityLevelMed
			},
			{
				"routemessageoutboundrequiretls",
				Strings.MtrtRequireTLS
			},
			{
				"encryptmessage",
				Strings.MtrtRequireEncryption
			},
			{
				"decryptmessage",
				Strings.MtrtRequireDecryption
			},
			{
				"rightsprotectmessage",
				Strings.MtrtRightsProtectMessage
			},
			{
				"sn",
				Strings.MtrtRouteMessageHighRisk
			},
			{
				"so",
				Strings.MtrtRouteMessageHighRisk
			},
			{
				"routemessageoutboundconnector",
				Strings.MtrtRouteMessageUsingConnector
			},
			{
				"setauditseverity",
				Strings.MtrtSetAuditSeverity
			},
			{
				"setheader",
				Strings.MtrtSetHeader
			},
			{
				"st",
				Strings.MtrtSetHeader
			},
			{
				"setscl",
				Strings.MtrtSetSpamConfidenceLevel
			}
		};

		private Dictionary<string, GetMessageTraceDetail.ParseDataDelegate> functionMap = new Dictionary<string, GetMessageTraceDetail.ParseDataDelegate>();

		internal static class LocalizedStringMethods
		{
			internal const string Malware = "MtrtMessageMalware";

			internal const string Spam = "MtrtMessageSpam";

			internal const string SpamDomain = "MtrtMessageSpamNonProvisionedDomain";

			internal const string SpamAdditional = "MtrtMessageSpamAdditional";

			internal const string TransportRule = "MtrtMessageTransportRule";

			internal const string DLPRule = "MtrtMessageDLPRule";

			internal const string Receive = "MtrtMessageRecieve";

			internal const string Send = "MtrtMessageSend";

			internal const string Fail = "MtrtMessageFail";

			internal const string FailureDetails = "MtrtMessageFailureReason";

			internal const string Deliver = "MtrtMessageDeliverDetailMessage";

			internal const string Badmail = "MtrtMessageBadmail";

			internal const string Expand = "MtrtMessageExpandDetailMessage";

			internal const string Submit = "MtrtMessageSubmitDetailMessage";

			internal const string Defer = "MtrtMessageDefer";
		}

		internal static class PropertyNames
		{
			internal const string ServerHostName = "ServerHostName";

			internal const string ConnectorId = "ConnectorId";

			internal const string Name = "name";

			internal const string File = "file";

			internal const string SCL = "SCL";

			internal const string RecipientStatus = "RecipientStatus";
		}

		internal static class Actions
		{
			internal const string AddManagerAsRecipientType = "addmanagerasrecipienttype";

			internal const string AddToRecipient = "addtorecipient";

			internal const string ApplyClassification = "applyclassification ";

			internal const string ApplyHtmlDisclaimer = "applyhtmldisclaimer";

			internal const string b = "b";

			internal const string BlindCopyTo = "blindcopyto";

			internal const string CopyTo = "copyto";

			internal const string Decrypt = "decrypt";

			internal const string DeleteMessage = "deletemessage";

			internal const string GenerateIncidentReport = "generateincidentreport";

			internal const string ModerateMessageByManager = "moderatemessagebymanager";

			internal const string ModerateMessageByUser = "moderatemessagebyuser";

			internal const string PrependSubject = "prependsubject";

			internal const string Quarantine = "quarantine";

			internal const string r = "r";

			internal const string RedirectMessage = "redirectmessage";

			internal const string RejectMessage = "rejectmessage";

			internal const string RemoveHeader = "removeheader";

			internal const string ReportSeverityLevelHigh = "reportseveritylevelhigh";

			internal const string ReportSeverityLevelLow = "reportseveritylevellow";

			internal const string ReportSeverityLevelMed = "reportseveritylevelmed";

			internal const string RightsProtectMessage = "rightsprotectmessage";

			internal const string RouteMessageOutboundConnector = "routemessageoutboundconnector";

			internal const string RouteMessageOutboundRequireTls = "routemessageoutboundrequiretls";

			internal const string EncryptMessage = "encryptmessage";

			internal const string DecryptMessage = "decryptmessage";

			internal const string SD = "sd";

			internal const string SetAuditSeverity = "setauditseverity";

			internal const string SenderNotify = "sendernotify";

			internal const string SetHeader = "setheader";

			internal const string SetSCL = "setscl";

			internal const string SN = "sn";

			internal const string SO = "so";

			internal const string SQ = "sq";

			internal const string SR = "sr";

			internal const string SS = "ss";

			internal const string ST = "st";

			internal const string SX = "sx";
		}

		internal static class EventNames
		{
			internal const string Receive = "receive";

			internal const string Send = "send";

			internal const string Fail = "fail";

			internal const string Deliver = "deliver";

			internal const string Badmail = "badmail";

			internal const string Expand = "expand";

			internal const string Submit = "submit";

			internal const string Defer = "defer";

			internal const string Malware = "ama";

			internal const string Spam = "sfa";

			internal const string TransportRule = "tra";

			internal const string AgentInfo = "agentinfo";
		}

		private delegate MessageTraceDetail ParseDataDelegate(GetMessageTraceDetail.DalPlaceholder dalObject);

		internal sealed class DalPlaceholder : IPageableObject
		{
			[DalConversion("OrganizationFromTask", "Organization", new string[]
			{

			})]
			public string Organization { get; set; }

			[DalConversion("DefaultSerializer", "InternalMessageId", new string[]
			{

			})]
			public Guid InternalMessageId { get; set; }

			[DalConversion("DefaultSerializer", "ClientMessageId", new string[]
			{

			})]
			public string MessageId { get; set; }

			[DalConversion("DefaultSerializer", "EventDate", new string[]
			{

			})]
			public DateTime EventDate { get; set; }

			[DalConversion("DefaultSerializer", "EventDescription", new string[]
			{

			})]
			public string EventDescription { get; set; }

			[DalConversion("DefaultSerializer", "AgentName", new string[]
			{

			})]
			public string AgentName { get; set; }

			[DalConversion("DefaultSerializer", "Action", new string[]
			{

			})]
			public string Action { get; set; }

			[DalConversion("DefaultSerializer", "RuleId", new string[]
			{

			})]
			public string RuleId { get; set; }

			[DalConversion("DefaultSerializer", "RuleName", new string[]
			{

			})]
			public string RuleName { get; set; }

			[DalConversion("DefaultSerializer", "PolicyId", new string[]
			{

			})]
			public string PolicyId { get; set; }

			[DalConversion("DefaultSerializer", "PolicyName", new string[]
			{

			})]
			public string PolicyName { get; set; }

			[DalConversion("DefaultSerializer", "Data", new string[]
			{

			})]
			public string Data { get; set; }

			public int Index { get; set; }
		}
	}
}
