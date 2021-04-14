using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	public class NotificationServiceClient : IDisposable
	{
		static NotificationServiceClient()
		{
			NotificationServiceClient.xmlNamespaces.Add("t", "http://schemas.microsoft.com/exchange/services/2006/types");
			NotificationServiceClient.xmlNamespaces.Add("m", "http://schemas.microsoft.com/exchange/services/2006/messages");
		}

		public NotificationServiceClient(string url, string callerData)
		{
			this.url = url;
			this.callerData = callerData;
		}

		public void SendNotificationAsync(SendNotificationResponse sendNotification, NotificationServiceClient.SendNotificationResultCallback resultCallback, object state)
		{
			this.sendNotification = sendNotification;
			this.callback = resultCallback;
			this.asyncState = state;
			this.webRequest = this.PrepareHttpWebRequest();
			AsyncCallback asyncCallback = new AsyncCallback(NotificationServiceClient.GetRequestStreamCallback);
			this.operationTimeoutTimer = new Timer(new TimerCallback(NotificationServiceClient.TimeoutCallback), this, this.timeout, -1);
			try
			{
				this.webRequest.BeginGetRequestStream(asyncCallback, this);
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.PushSubscriptionTracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "NotificationServiceClient::SendNotificationAsync Exception: {0}", ex);
				this.MakeCallbackAndDispose(this.asyncState, null, ex);
			}
		}

		private static void GetRequestStreamCallback(IAsyncResult asyncResult)
		{
			NotificationServiceClient notificationServiceClient = (NotificationServiceClient)asyncResult.AsyncState;
			notificationServiceClient.CreateSendNotificationRequestAsync(asyncResult);
		}

		private static void GetResponseCallback(IAsyncResult asyncResult)
		{
			NotificationServiceClient notificationServiceClient = (NotificationServiceClient)asyncResult.AsyncState;
			notificationServiceClient.HandleResponse(asyncResult);
		}

		private static void ResponseStreamReadCallback(IAsyncResult asyncResult)
		{
			NotificationServiceClient notificationServiceClient = asyncResult.AsyncState as NotificationServiceClient;
			Exception ex = null;
			int num = 0;
			lock (notificationServiceClient)
			{
				if (notificationServiceClient.operationTimedOut != null)
				{
					return;
				}
				try
				{
					num = notificationServiceClient.responseStream.EndRead(asyncResult);
				}
				catch (IOException ex2)
				{
					ExTraceGlobals.PushSubscriptionTracer.TraceError<IOException>((long)notificationServiceClient.GetHashCode(), "NotificationServiceClient::ResponseStreamReadCallback: Exception {0}", ex2);
					ex = ex2;
				}
				if (num == 0)
				{
					notificationServiceClient.operationTimedOut = new bool?(false);
				}
			}
			if (ex != null)
			{
				notificationServiceClient.MakeCallbackAndDispose(notificationServiceClient.asyncState, null, ex);
			}
			else
			{
				if (num == 0)
				{
					Exception exception = null;
					SendNotificationResult result = null;
					try
					{
						result = notificationServiceClient.ReadSendNotificationResult();
					}
					catch (InvalidOperationException ex3)
					{
						exception = ex3;
					}
					notificationServiceClient.MakeCallbackAndDispose(notificationServiceClient.asyncState, result, exception);
					return;
				}
				notificationServiceClient.responseBufferBytesRead += num;
				if (notificationServiceClient.responseBufferBytesRead >= notificationServiceClient.responseLimitInBytes)
				{
					notificationServiceClient.MakeCallbackAndDispose(notificationServiceClient.asyncState, null, new InvalidOperationException(string.Format("Response to Push Notification was larger than allowed. Limit : {0} bytes", notificationServiceClient.responseLimitInBytes)));
					return;
				}
				notificationServiceClient.SetUpResponseStreamRead();
				return;
			}
		}

		private static void TimeoutCallback(object state)
		{
			NotificationServiceClient notificationServiceClient = state as NotificationServiceClient;
			lock (notificationServiceClient)
			{
				if (notificationServiceClient.operationTimedOut != null)
				{
					return;
				}
				notificationServiceClient.operationTimedOut = new bool?(true);
			}
			ExTraceGlobals.PushSubscriptionTracer.TraceError((long)notificationServiceClient.GetHashCode(), "NotificationServiceClient::TimeoutCallback: The PushNotification timed out");
			notificationServiceClient.MakeCallbackAndDispose(notificationServiceClient.asyncState, null, new WebException("Request timed out", WebExceptionStatus.Timeout));
			HttpWebRequest httpWebRequest = notificationServiceClient.webRequest;
			httpWebRequest.Abort();
		}

		private void CreateSendNotificationRequestAsync(IAsyncResult requestAsyncResult)
		{
			try
			{
				using (Stream stream = this.webRequest.EndGetRequestStream(requestAsyncResult))
				{
					this.WriteSendNotificationRequestToStream(this.sendNotification, stream);
				}
			}
			catch (WebException webEx)
			{
				this.TraceWebException(webEx, "CreateSendNotificationRequestAsync");
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.PushSubscriptionTracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "NotificationServiceClient::CreateSendNotificationRequestAsync Exception: {0}", ex);
				this.MakeCallbackAndDispose(this.asyncState, null, ex);
				return;
			}
			AsyncCallback asyncCallback = new AsyncCallback(NotificationServiceClient.GetResponseCallback);
			try
			{
				this.webRequest.BeginGetResponse(asyncCallback, this);
			}
			catch (InvalidOperationException ex2)
			{
				ExTraceGlobals.PushSubscriptionTracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "NotificationServiceClient::CreateSendNotificationRequestAsync Exception: {0}", ex2);
				this.MakeCallbackAndDispose(this.asyncState, null, ex2);
			}
		}

		private void HandleResponse(IAsyncResult responseAsyncResult)
		{
			try
			{
				this.webResponse = this.webRequest.EndGetResponse(responseAsyncResult);
				this.responseStream = this.webResponse.GetResponseStream();
				this.SetUpInitialStreamRead();
			}
			catch (WebException webEx)
			{
				this.TraceWebException(webEx, "HandleResponse");
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.PushSubscriptionTracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "NotificationServiceClient::HandleResponse Exception: {0}", ex);
				this.MakeCallbackAndDispose(this.asyncState, null, ex);
			}
		}

		private void TraceWebException(WebException webEx, string source)
		{
			ExTraceGlobals.PushSubscriptionTracer.TraceError<string, WebException>((long)this.GetHashCode(), "NotificationServiceClient::{0} Exception: {1}", source, webEx);
			this.MakeCallbackAndDispose(this.asyncState, null, webEx);
		}

		private void SetUpInitialStreamRead()
		{
			this.responseBufferBytesRead = 0;
			this.responseBuffer = new byte[this.responseLimitInBytes + 1];
			this.SetUpResponseStreamRead();
		}

		private IAsyncResult SetUpResponseStreamRead()
		{
			int count = this.responseLimitInBytes - this.responseBufferBytesRead + 1;
			AsyncCallback asyncCallback = new AsyncCallback(NotificationServiceClient.ResponseStreamReadCallback);
			return this.responseStream.BeginRead(this.responseBuffer, this.responseBufferBytesRead, count, asyncCallback, this);
		}

		private void WriteSendNotificationRequestToStream(SendNotificationResponse requestData, Stream requestStream)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(requestStream, new XmlWriterSettings
			{
				Encoding = new UTF8Encoding(false)
			}))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(NotificationRequestServerVersion));
				XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(SendNotificationResponse));
				xmlWriter.WriteStartElement("soap11", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
				xmlWriter.WriteStartElement("soap11", "Header", "http://schemas.xmlsoap.org/soap/envelope/");
				if (this.RequestServerVersionValue != null)
				{
					xmlSerializer.Serialize(xmlWriter, this.RequestServerVersionValue, NotificationServiceClient.xmlNamespaces);
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("soap11", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
				xmlSerializer2.Serialize(xmlWriter, requestData, NotificationServiceClient.xmlNamespaces);
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			}
		}

		private HttpWebRequest PrepareHttpWebRequest()
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.url);
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Timeout = this.timeout;
			httpWebRequest.ContentType = "text/xml; charset=utf-8";
			httpWebRequest.Method = "POST";
			httpWebRequest.Accept = "text/xml";
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.KeepAlive = false;
			if (!string.IsNullOrEmpty(this.callerData))
			{
				httpWebRequest.Headers.Add("CallerData", this.callerData);
			}
			httpWebRequest.Headers.Add("SOAPAction", "http://schemas.microsoft.com/exchange/services/2006/messages/SendNotification");
			return httpWebRequest;
		}

		private SendNotificationResult ReadSendNotificationResult()
		{
			SendNotificationResult result;
			using (this.responseStream = new MemoryStream(this.responseBuffer))
			{
				XmlReader xmlReader = this.CreateXmlReader(this.responseStream);
				result = this.ParseNotificationResults(xmlReader);
			}
			return result;
		}

		private SendNotificationResult ParseNotificationResults(XmlReader xmlReader)
		{
			SendNotificationResult sendNotificationResult = new SendNotificationResult();
			try
			{
				xmlReader.Read();
				while (!xmlReader.EOF)
				{
					if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.LocalName == "SubscriptionStatus")
					{
						try
						{
							string value = xmlReader.ReadString();
							sendNotificationResult.SubscriptionStatus = (SubscriptionStatus)Enum.Parse(typeof(SubscriptionStatus), value);
							break;
						}
						catch (ArgumentException)
						{
							sendNotificationResult.SubscriptionStatus = SubscriptionStatus.Invalid;
							break;
						}
					}
					xmlReader.Read();
				}
			}
			catch (XmlException innerException)
			{
				throw new InvalidOperationException("Malformed SOAP response", innerException);
			}
			return sendNotificationResult;
		}

		private XmlReader CreateXmlReader(Stream stream)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(stream);
			xmlTextReader.Normalization = false;
			return XmlReader.Create(xmlTextReader, xmlReaderSettings);
		}

		public NotificationRequestServerVersion RequestServerVersionValue
		{
			get
			{
				return this.requestServerVersionValueField;
			}
			set
			{
				this.requestServerVersionValueField = value;
			}
		}

		public ICredentials Credentials
		{
			set
			{
				this.credentials = value;
			}
		}

		public int Timeout
		{
			set
			{
				this.timeout = value;
			}
		}

		public int ResponseLimitInBytes
		{
			set
			{
				this.responseLimitInBytes = value;
			}
		}

		private void MakeCallbackAndDispose(object state, SendNotificationResult result, Exception exception)
		{
			NotificationServiceClient.SendNotificationResultCallback sendNotificationResultCallback = null;
			lock (this)
			{
				if (this.callback == null)
				{
					return;
				}
				sendNotificationResultCallback = this.callback;
				this.callback = null;
			}
			if (this.operationTimeoutTimer != null)
			{
				this.operationTimeoutTimer.Change(-1, -1);
			}
			sendNotificationResultCallback(state, result, exception);
			this.Dispose();
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool isDisposing)
		{
			if (this.webRequest != null)
			{
				this.webRequest.Abort();
			}
			if (this.webResponse != null)
			{
				this.webResponse.Close();
			}
			if (this.operationTimeoutTimer != null)
			{
				this.operationTimeoutTimer.Dispose();
			}
			GC.SuppressFinalize(this);
		}

		private const string SOAPEnvelopeElementName = "Envelope";

		private const string SOAPHeaderElementName = "Header";

		private const string SOAPBodyElementName = "Body";

		private const string SubscriptionStatusElementName = "SubscriptionStatus";

		private const string SOAPActionHeaderName = "SOAPAction";

		private const string CallerDataHeaderName = "CallerData";

		private const string SOAPActionHeaderValue = "http://schemas.microsoft.com/exchange/services/2006/messages/SendNotification";

		private readonly string callerData;

		private static XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();

		private NotificationRequestServerVersion requestServerVersionValueField;

		private string url;

		private ICredentials credentials;

		private int timeout;

		private int responseLimitInBytes;

		private SendNotificationResponse sendNotification;

		private NotificationServiceClient.SendNotificationResultCallback callback;

		private object asyncState;

		private HttpWebRequest webRequest;

		private WebResponse webResponse;

		private bool? operationTimedOut = null;

		private byte[] responseBuffer;

		private int responseBufferBytesRead;

		private Stream responseStream;

		private Timer operationTimeoutTimer;

		public delegate void SendNotificationResultCallback(object state, SendNotificationResult result, Exception exception);
	}
}
