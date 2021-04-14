using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("FindMessageTrackingReportRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class FindMessageTrackingReportRequest : BaseRequest, IMessageTrackingRequestLogInformation
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new FindMessageTrackingReport(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}

		public void AddRequestDataForLogging(List<KeyValuePair<string, object>> requestData)
		{
			requestData.Add(new KeyValuePair<string, object>("MessageId", this.MessageId ?? string.Empty));
			requestData.Add(new KeyValuePair<string, object>("Domain", this.Domain ?? string.Empty));
			string value = (this.Sender == null || this.Sender.EmailAddress == null) ? string.Empty : this.Sender.EmailAddress;
			requestData.Add(new KeyValuePair<string, object>("Sender", value));
			requestData.Add(new KeyValuePair<string, object>("ServerHint", this.ServerHint ?? string.Empty));
		}

		public string Scope;

		public string Domain;

		public EmailAddressWrapper Sender;

		public EmailAddressWrapper Recipient;

		public string Subject;

		public DateTime StartDateTime;

		[XmlIgnore]
		public bool StartDateTimeSpecified;

		public DateTime EndDateTime;

		[XmlIgnore]
		public bool EndDateTimeSpecified;

		public string MessageId;

		public EmailAddressWrapper FederatedDeliveryMailbox;

		public string DiagnosticsLevel;

		public string ServerHint;

		[XmlArrayItem("TrackingPropertyType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
