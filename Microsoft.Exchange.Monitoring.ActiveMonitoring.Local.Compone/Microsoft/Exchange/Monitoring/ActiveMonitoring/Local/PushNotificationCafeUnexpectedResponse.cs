using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PushNotificationCafeUnexpectedResponse : LocalizedException
	{
		public PushNotificationCafeUnexpectedResponse(string response, string requestHeaders, string responseHeaders, string body) : base(Strings.PushNotificationCafeUnexpectedResponse(response, requestHeaders, responseHeaders, body))
		{
			this.response = response;
			this.requestHeaders = requestHeaders;
			this.responseHeaders = responseHeaders;
			this.body = body;
		}

		public PushNotificationCafeUnexpectedResponse(string response, string requestHeaders, string responseHeaders, string body, Exception innerException) : base(Strings.PushNotificationCafeUnexpectedResponse(response, requestHeaders, responseHeaders, body), innerException)
		{
			this.response = response;
			this.requestHeaders = requestHeaders;
			this.responseHeaders = responseHeaders;
			this.body = body;
		}

		protected PushNotificationCafeUnexpectedResponse(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.response = (string)info.GetValue("response", typeof(string));
			this.requestHeaders = (string)info.GetValue("requestHeaders", typeof(string));
			this.responseHeaders = (string)info.GetValue("responseHeaders", typeof(string));
			this.body = (string)info.GetValue("body", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("response", this.response);
			info.AddValue("requestHeaders", this.requestHeaders);
			info.AddValue("responseHeaders", this.responseHeaders);
			info.AddValue("body", this.body);
		}

		public string Response
		{
			get
			{
				return this.response;
			}
		}

		public string RequestHeaders
		{
			get
			{
				return this.requestHeaders;
			}
		}

		public string ResponseHeaders
		{
			get
			{
				return this.responseHeaders;
			}
		}

		public string Body
		{
			get
			{
				return this.body;
			}
		}

		private readonly string response;

		private readonly string requestHeaders;

		private readonly string responseHeaders;

		private readonly string body;
	}
}
