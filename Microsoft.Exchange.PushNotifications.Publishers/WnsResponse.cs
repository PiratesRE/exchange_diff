using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsResponse
	{
		public WnsResponse(HttpStatusCode responseCode, string messageId, string debugTrace, string errorDescription)
		{
			this.ResponseCode = responseCode;
			this.MessageId = messageId;
			this.DebugTrace = debugTrace;
			this.ErrorDescription = errorDescription;
		}

		public HttpStatusCode ResponseCode { get; private set; }

		public string MessageId { get; private set; }

		public string DebugTrace { get; private set; }

		public string ErrorDescription { get; private set; }

		public static WnsResponse Create(HttpWebResponse response)
		{
			ArgumentValidator.ThrowIfNull("response", response);
			return new WnsResponse(response.StatusCode, response.Headers["X-WNS-Msg-ID"], response.Headers["X-WNS-Debug-Trace"], response.Headers["X-WNS-Error-Description"]);
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Format("responseCode={0}; messageId={1}; error={2}; debugTrace={3}", new object[]
				{
					this.ResponseCode.ToString(),
					this.MessageId.ToNullableString(),
					this.ErrorDescription.ToNullableString(),
					this.DebugTrace.ToNullableString()
				});
			}
			return this.toString;
		}

		private const string WnsHeaderMessageId = "X-WNS-Msg-ID";

		private const string WnsHeaderDebugTrace = "X-WNS-Debug-Trace";

		private const string WnsHeaderErrorDescription = "X-WNS-Error-Description";

		private const string WnsHeaderNotificationStatus = "X-WNS-NotificationStatus";

		private const string WnsHeaderDeviceStatus = "X-WNS-DeviceConnectionStatus";

		public static readonly WnsResponse Succeeded = new WnsResponse(HttpStatusCode.OK, null, null, null);

		private string toString;
	}
}
