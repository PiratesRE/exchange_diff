using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class InitiationMessage
	{
		private InitiationMessage(EmailMessage message)
		{
			this.message = message;
		}

		public IList<RoutingAddress> DecisionMakers
		{
			get
			{
				if (!this.decisionMakersRead)
				{
					string text;
					RoutingAddress[] list;
					if (this.TryGetHeaderValue("X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers", out text) && ApprovalUtils.TryGetDecisionMakers(text, out list))
					{
						this.decisionMakers = new ReadOnlyCollection<RoutingAddress>(list);
					}
					InitiationMessage.diag.TraceDebug((long)this.GetHashCode(), "Missing decision maker or invalid decision makers.");
					this.decisionMakersRead = true;
				}
				return this.decisionMakers;
			}
		}

		public RoutingAddress Requestor
		{
			get
			{
				RoutingAddress routingAddress = RoutingAddress.Empty;
				string address;
				if (!this.TryGetHeaderValue("X-MS-Exchange-Organization-Approval-Requestor", out address))
				{
					return routingAddress;
				}
				routingAddress = (RoutingAddress)address;
				if (!Util.IsValidAddress(routingAddress))
				{
					routingAddress = RoutingAddress.Empty;
				}
				return routingAddress;
			}
		}

		public string ApprovalData { get; internal set; }

		public int? MessageItemLocale { get; internal set; }

		public MimeConstant.ApprovalAllowedAction VotingActions
		{
			get
			{
				string text;
				if (this.TryGetHeaderValue("X-MS-Exchange-Organization-Approval-Allowed-Actions", out text))
				{
					MimeConstant.ApprovalAllowedAction result;
					if (EnumValidator<MimeConstant.ApprovalAllowedAction>.TryParse(text, EnumParseOptions.IgnoreCase, out result))
					{
						return result;
					}
					InitiationMessage.diag.TraceDebug<string>((long)this.GetHashCode(), "Invalid Voting action {0}", text);
				}
				InitiationMessage.diag.TraceDebug((long)this.GetHashCode(), "No Voting action header value");
				return MimeConstant.ApprovalAllowedAction.ApproveReject;
			}
		}

		public string Subject
		{
			get
			{
				return this.message.Subject;
			}
		}

		internal EmailMessage EmailMessage
		{
			get
			{
				return this.message;
			}
		}

		internal string ApprovalInitiator
		{
			get
			{
				string result;
				if (this.TryGetHeaderValue("X-MS-Exchange-Organization-Approval-Initiator", out result))
				{
					return result;
				}
				return null;
			}
		}

		internal bool IsMapiInitiator
		{
			get
			{
				string approvalInitiator = this.ApprovalInitiator;
				return string.IsNullOrEmpty(approvalInitiator) || approvalInitiator.Equals("mapi", StringComparison.OrdinalIgnoreCase);
			}
		}

		public static bool TryCreate(EmailMessage message, out InitiationMessage initiationMessage)
		{
			initiationMessage = null;
			if (!InitiationMessage.IsInitiationMessage(message))
			{
				return false;
			}
			initiationMessage = new InitiationMessage(message);
			return true;
		}

		public static bool IsInitiationMessage(EmailMessage message)
		{
			HeaderList headers = message.MimeDocument.RootPart.Headers;
			Header header = headers.FindFirst("X-MS-Exchange-Organization-Approval-Initiator");
			Header header2 = headers.FindFirst("X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers");
			return header != null && header2 != null;
		}

		private bool TryGetHeaderValue(string headerName, out string value)
		{
			HeaderList headers = this.message.MimeDocument.RootPart.Headers;
			TextHeader textHeader = headers.FindFirst(headerName) as TextHeader;
			value = null;
			if (textHeader == null)
			{
				InitiationMessage.diag.TraceDebug<string>((long)this.GetHashCode(), "'{0}' header not found from message.", headerName);
				return false;
			}
			if (!textHeader.TryGetValue(out value) || string.IsNullOrEmpty(value))
			{
				InitiationMessage.diag.TraceDebug<string>((long)this.GetHashCode(), "'{0}' header cannot be read from message.", headerName);
				return false;
			}
			return true;
		}

		public const string MapiApprovalInitiator = "mapi";

		private static readonly Trace diag = ExTraceGlobals.ApprovalAgentTracer;

		private EmailMessage message;

		private bool decisionMakersRead;

		private ReadOnlyCollection<RoutingAddress> decisionMakers;
	}
}
