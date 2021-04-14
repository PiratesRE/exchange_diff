using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FacebookClient : DisposeTrackableBase, IFacebookClient, IDisposable
	{
		public FacebookClient(Uri endpoint)
		{
			ArgumentValidator.ThrowIfNull("Endpoint", endpoint);
			WebChannelFactory<IFacebookService> webChannelFactory = new WebChannelFactory<IFacebookService>(endpoint);
			WebHttpBinding webHttpBinding = (WebHttpBinding)webChannelFactory.Endpoint.Binding;
			webHttpBinding.MaxReceivedMessageSize = 5242880L;
			webHttpBinding.ContentTypeMapper = new FacebookClient.JsonContentMapper();
			FacebookClientMessageInspector facebookClientMessageInspector = new FacebookClientMessageInspector();
			facebookClientMessageInspector.MessageDownloaded += this.OnMessageDownload;
			webChannelFactory.Endpoint.Behaviors.Add(new FacebookClientMessageBehavior(facebookClientMessageInspector));
			this.channelFactory = webChannelFactory;
			this.service = webChannelFactory.CreateChannel();
		}

		public IAsyncResult BeginGetFriends(string accessToken, string fields, string limit, string offset, AsyncCallback callback, object state)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNullOrEmpty("accessToken", accessToken);
			ArgumentValidator.ThrowIfNullOrEmpty("fields", fields);
			return this.service.BeginGetFriends(accessToken, fields, limit, offset, callback, state);
		}

		public FacebookUsersList EndGetFriends(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("AsyncResult", asyncResult);
			return this.service.EndGetFriends(asyncResult);
		}

		public IAsyncResult BeginGetUsers(string accessToken, string userIds, string fields, AsyncCallback callback, object state)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNullOrEmpty("accessToken", accessToken);
			ArgumentValidator.ThrowIfNullOrEmpty("userIds", userIds);
			ArgumentValidator.ThrowIfNullOrEmpty("fields", fields);
			return this.service.BeginGetUsers(accessToken, userIds, fields, callback, state);
		}

		public FacebookUsersList EndGetUsers(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("AsyncResult", asyncResult);
			return this.service.EndGetUsers(asyncResult);
		}

		public FacebookUser GetProfile(string accessToken, string fields)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNullOrEmpty("accessToken", accessToken);
			ArgumentValidator.ThrowIfNullOrEmpty("fields", fields);
			FacebookUser profile;
			using (new OperationContextScope((IContextChannel)this.service))
			{
				profile = this.service.GetProfile(accessToken, fields);
			}
			return profile;
		}

		public void RemoveApplication(string accessToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNullOrEmpty("accessToken", accessToken);
			using (new OperationContextScope((IContextChannel)this.service))
			{
				this.service.RemoveApplication(accessToken);
			}
		}

		public FacebookImportContactsResult UploadContacts(string accessToken, bool continuous, bool async, string source, string contactsFormat, string contactsStreamContentType, Stream contacts)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNullOrEmpty("accessToken", accessToken);
			ArgumentValidator.ThrowIfNullOrEmpty("source", source);
			ArgumentValidator.ThrowIfNullOrEmpty("format", contactsFormat);
			ArgumentValidator.ThrowIfNull("contacts", contacts);
			FacebookImportContactsResult result;
			using (new OperationContextScope((IContextChannel)this.service))
			{
				if (!string.IsNullOrEmpty(contactsStreamContentType))
				{
					WebOperationContext.Current.OutgoingRequest.ContentType = contactsStreamContentType;
				}
				result = this.service.ImportContacts(accessToken, contactsFormat, string.Empty, continuous, async, source, contacts);
			}
			return result;
		}

		public static void AppendDiagnoseDataToException(CommunicationException exception)
		{
			ArgumentValidator.ThrowIfNull("exception", exception);
			if (exception.Data != null)
			{
				using (MemoryStream responseStreamFromException = FacebookClient.GetResponseStreamFromException(exception))
				{
					if (responseStreamFromException != null && responseStreamFromException.Length > 0L)
					{
						using (StreamReader streamReader = new StreamReader(responseStreamFromException))
						{
							exception.Data.Add("FBError.ResponseText", streamReader.ReadToEnd());
						}
					}
				}
			}
		}

		private static MemoryStream GetResponseStreamFromException(CommunicationException e)
		{
			MemoryStream memoryStream = new MemoryStream();
			WebException ex = e.InnerException as WebException;
			if (ex != null)
			{
				HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
				if (httpWebResponse != null)
				{
					Stream responseStream = httpWebResponse.GetResponseStream();
					if (responseStream != null)
					{
						responseStream.CopyTo(memoryStream);
						memoryStream.Position = 0L;
					}
				}
			}
			return memoryStream;
		}

		public void Cancel()
		{
			this.channelFactory.Abort();
		}

		public void SubscribeDownloadCompletedEvent(EventHandler<DownloadCompleteEventArgs> eventHandler)
		{
			base.CheckDisposed();
			this.downloadCompleted = (EventHandler<DownloadCompleteEventArgs>)Delegate.Combine(this.downloadCompleted, eventHandler);
		}

		protected virtual HttpStatusCode GetHttpWebResponseStatusCode(HttpWebRequest httpWebRequest)
		{
			HttpStatusCode statusCode;
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				statusCode = httpWebResponse.StatusCode;
			}
			return statusCode;
		}

		internal void OnMessageDownload(object sender, FacebookMessageEventArgs eventArgs)
		{
			object obj;
			if (eventArgs != null && eventArgs.MessageTransferred != null && !eventArgs.MessageTransferred.IsEmpty && eventArgs.MessageTransferred.Properties.TryGetValue(HttpResponseMessageProperty.Name, out obj) && obj is HttpResponseMessageProperty)
			{
				string s = ((HttpResponseMessageProperty)obj).Headers[HttpResponseHeader.ContentLength];
				long num;
				if (long.TryParse(s, out num))
				{
					this.bytesDownloaded = num;
					if (this.downloadCompleted != null)
					{
						this.downloadCompleted(this, new DownloadCompleteEventArgs(this.bytesDownloaded, 0L));
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FacebookClient>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.channelFactory.Close();
			}
		}

		private const int DefaultMaxReceivedMessageSize = 5242880;

		private const string ErrorResponseTextPropertyName = "FBError.ResponseText";

		private readonly ChannelFactory<IFacebookService> channelFactory;

		private readonly IFacebookService service;

		private long bytesDownloaded;

		private EventHandler<DownloadCompleteEventArgs> downloadCompleted;

		private class JsonContentMapper : WebContentTypeMapper
		{
			public override WebContentFormat GetMessageFormatForContentType(string contentType)
			{
				return WebContentFormat.Json;
			}
		}
	}
}
