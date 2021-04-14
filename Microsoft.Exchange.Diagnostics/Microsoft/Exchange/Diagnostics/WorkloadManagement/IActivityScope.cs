using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal interface IActivityScope
	{
		Guid ActivityId { get; }

		Guid LocalId { get; }

		ActivityContextStatus Status { get; }

		ActivityType ActivityType { get; }

		DateTime StartTime { get; }

		DateTime? EndTime { get; }

		double TotalMilliseconds { get; }

		object UserState { get; set; }

		string UserId { get; set; }

		string Puid { get; set; }

		string UserEmail { get; set; }

		string AuthenticationType { get; set; }

		string AuthenticationToken { get; set; }

		string TenantId { get; set; }

		string TenantType { get; set; }

		string Component { get; set; }

		string ComponentInstance { get; set; }

		string Feature { get; set; }

		string Protocol { get; set; }

		string ClientInfo { get; set; }

		string ClientRequestId { get; set; }

		string ReturnClientRequestId { get; set; }

		string Action { get; set; }

		IEnumerable<KeyValuePair<Enum, object>> Metadata { get; }

		AggregatedOperationStatistics TakeStatisticsSnapshot(AggregatedOperationType type);

		IEnumerable<KeyValuePair<OperationKey, OperationStatistics>> Statistics { get; }

		ActivityContextState Suspend();

		void End();

		bool AddOperation(ActivityOperationType operation, string instance, float value = 0f, int count = 1);

		void SetProperty(Enum property, string value);

		bool AppendToProperty(Enum property, string value);

		string GetProperty(Enum property);

		List<KeyValuePair<string, object>> GetFormattableMetadata();

		IEnumerable<KeyValuePair<string, object>> GetFormattableMetadata(IEnumerable<Enum> properties);

		List<KeyValuePair<string, object>> GetFormattableStatistics();

		void UpdateFromMessage(HttpRequestMessageProperty wcfMessage);

		void UpdateFromMessage(OperationContext wcfOperationContext);

		void UpdateFromMessage(HttpRequest httpRequest);

		void UpdateFromMessage(HttpRequestBase httpRequestBase);

		void SerializeTo(OperationContext wcfOperationContext);

		void SerializeTo(HttpRequestMessageProperty wcfMessage);

		void SerializeTo(HttpWebRequest httpWebRequest);

		void SerializeTo(HttpResponse httpResponse);

		void SerializeMinimalTo(HttpWebRequest httpWebRequest);

		void SerializeMinimalTo(HttpRequestBase httpRequest);
	}
}
