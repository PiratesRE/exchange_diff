using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetMessageTrackingReportRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetMessageTrackingReportRequest : BaseRequest, IMessageTrackingRequestLogInformation
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetMessageTrackingReport(callContext, this);
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
			requestData.Add(new KeyValuePair<string, object>("MessageTrackingReportId", this.MessageTrackingReportId ?? string.Empty));
			requestData.Add(new KeyValuePair<string, object>("ReportTemplate", Names<Microsoft.Exchange.InfoWorker.Common.MessageTracking.ReportTemplate>.Map[(int)this.ReportTemplate]));
			string value = (this.RecipientFilter == null || this.RecipientFilter.EmailAddress == null) ? string.Empty : this.RecipientFilter.EmailAddress;
			requestData.Add(new KeyValuePair<string, object>("RecipientFilter", value));
			requestData.Add(new KeyValuePair<string, object>("Scope", this.Scope ?? string.Empty));
		}

		public string Scope;

		public MessageTrackingReportTemplate ReportTemplate;

		public EmailAddressWrapper RecipientFilter;

		public string MessageTrackingReportId;

		public bool ReturnQueueEvents;

		public string DiagnosticsLevel;

		[XmlArrayItem("TrackingPropertyType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
