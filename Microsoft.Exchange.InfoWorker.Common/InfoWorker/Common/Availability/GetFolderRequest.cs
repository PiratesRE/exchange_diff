using System;
using System.Net;
using System.Net.Security;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class GetFolderRequest : AsyncWebRequest, IDisposable
	{
		public GetFolderRequest(Application application, InternalClientContext clientContext, RequestType requestType, RequestLogger requestLogger, BaseQuery query, Uri url) : base(application, clientContext, requestLogger, "GetFolderRequest")
		{
			if (query.RecipientData == null || query.RecipientData.AssociatedFolderId == null)
			{
				throw new InvalidOperationException("Unable to get associated folder id");
			}
			this.query = query;
			this.url = url.OriginalString;
			this.binding = new ExchangeServiceBinding(Globals.CertificateValidationComponentId, new RemoteCertificateValidationCallback(CertificateErrorHandler.CertValidationCallback));
			this.binding.Url = url.OriginalString;
			this.binding.RequestServerVersionValue = new RequestServerVersion();
			this.binding.RequestServerVersionValue.Version = ExchangeVersionType.Exchange2007_SP1;
			Server localServer = LocalServerCache.LocalServer;
			if (localServer != null && localServer.InternetWebProxy != null)
			{
				GetFolderRequest.GetFolderRequestTracer.TraceDebug<GetFolderRequest, Uri>((long)this.GetHashCode(), "{0}: Using custom InternetWebProxy {1}", this, localServer.InternetWebProxy);
				this.binding.Proxy = new WebProxy(localServer.InternetWebProxy);
			}
		}

		public BaseQuery Query
		{
			get
			{
				return this.query;
			}
		}

		public string ResultFolderId { get; private set; }

		public void Dispose()
		{
			if (this.binding != null)
			{
				this.binding.Dispose();
			}
		}

		public override void Abort()
		{
			base.Abort();
			if (this.binding != null)
			{
				this.binding.Abort();
			}
		}

		protected override bool IsImpersonating
		{
			get
			{
				return true;
			}
		}

		protected override IAsyncResult BeginInvoke()
		{
			GetFolderType getFolder = new GetFolderType
			{
				FolderShape = GetFolderRequest.GetFolderShape,
				FolderIds = new BaseFolderIdType[]
				{
					new DistinguishedFolderIdType
					{
						Id = DistinguishedFolderIdNameType.calendar
					}
				}
			};
			this.binding.Authenticator = SoapHttpClientAuthenticator.CreateNetworkService();
			this.binding.Authenticator.AdditionalSoapHeaders.Add(new SerializedSecurityContextType
			{
				UserSid = (this.query.RecipientData.Sid ?? this.query.RecipientData.MasterAccountSid).ToString(),
				GroupSids = GetFolderRequest.SidStringAndAttributesConverter(ClientSecurityContext.DisabledEveryoneOnlySidStringAndAttributesArray),
				RestrictedGroupSids = null,
				PrimarySmtpAddress = this.query.RecipientData.PrimarySmtpAddress.ToString()
			});
			return this.binding.BeginGetFolder(getFolder, new AsyncCallback(base.Complete), null);
		}

		protected override void EndInvoke(IAsyncResult asyncResult)
		{
			GetFolderResponseType getFolderResponseType = this.binding.EndGetFolder(asyncResult);
			if (getFolderResponseType.ResponseMessages == null || getFolderResponseType.ResponseMessages.Items == null)
			{
				GetFolderRequest.GetFolderRequestTracer.TraceDebug((long)this.GetHashCode(), "{0}: GetFolder web request returned NULL ResponseMessages.", new object[]
				{
					TraceContext.Get()
				});
				this.SetErrorResultOnUnexpectedResponse();
				return;
			}
			FolderInfoResponseMessageType folderInfoResponseMessageType = getFolderResponseType.ResponseMessages.Items[0] as FolderInfoResponseMessageType;
			if (folderInfoResponseMessageType == null)
			{
				GetFolderRequest.GetFolderRequestTracer.TraceDebug((long)this.GetHashCode(), "{0}: GetFolder web request returned NULL FolderInfoResponseMessageType.", new object[]
				{
					TraceContext.Get()
				});
				this.SetErrorResultOnUnexpectedResponse();
				return;
			}
			if (folderInfoResponseMessageType.ResponseCode != ResponseCodeType.NoError)
			{
				GetFolderRequest.GetFolderRequestTracer.TraceDebug<object, ResponseCodeType>((long)this.GetHashCode(), "{0}: GetFolder web request returned ResponseCodeType {1}.", TraceContext.Get(), folderInfoResponseMessageType.ResponseCode);
				this.SetErrorResultOnUnexpectedResponse();
				return;
			}
			if (folderInfoResponseMessageType.Folders == null)
			{
				GetFolderRequest.GetFolderRequestTracer.TraceDebug((long)this.GetHashCode(), "{0}: GetFolder web request returned NULL Folders.", new object[]
				{
					TraceContext.Get()
				});
				this.SetErrorResultOnUnexpectedResponse();
				return;
			}
			BaseFolderType baseFolderType = folderInfoResponseMessageType.Folders[0];
			if (baseFolderType == null)
			{
				GetFolderRequest.GetFolderRequestTracer.TraceDebug<object, EmailAddress>((long)this.GetHashCode(), "{0}: GetFolder web request returned NULL FolderResponse for mailbox {1}.", TraceContext.Get(), this.query.Email);
				this.SetErrorResultOnUnexpectedResponse();
				return;
			}
			this.ResultFolderId = baseFolderType.FolderId.Id;
			GetFolderRequest.GetFolderRequestTracer.TraceDebug<object, EmailAddress, string>((long)this.GetHashCode(), "{0}: GetFolder web request returned folder id {2} for mailbox {1}.", TraceContext.Get(), this.query.Email, this.ResultFolderId);
		}

		protected override void HandleException(Exception exception)
		{
			if (GetFolderRequest.GetFolderRequestTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				GetFolderRequest.GetFolderRequestTracer.TraceError<object, Exception>((long)this.GetHashCode(), "{0}: Exception occurred while completing GetFolder web request. Exception info is {1}. ", TraceContext.Get(), exception);
			}
			GetFolderRequestProcessingException exception2 = this.GenerateException(HttpWebRequestExceptionHandler.TranslateExceptionString(exception));
			BaseQueryResult baseQueryResult = base.Application.CreateQueryResult(exception2);
			if (this.query.SetResultOnFirstCall(baseQueryResult))
			{
				GetFolderRequest.GetFolderRequestTracer.TraceError<object, EmailAddress, BaseQueryResult>((long)this.GetHashCode(), "{0}: the following result was set for query {1}: {2}", TraceContext.Get(), this.query.Email, baseQueryResult);
			}
		}

		private static SidAndAttributesType[] SidStringAndAttributesConverter(SidStringAndAttributes[] sidStringAndAttributesArray)
		{
			if (sidStringAndAttributesArray == null)
			{
				return null;
			}
			SidAndAttributesType[] array = new SidAndAttributesType[sidStringAndAttributesArray.Length];
			for (int i = 0; i < sidStringAndAttributesArray.Length; i++)
			{
				array[i] = new SidAndAttributesType
				{
					SecurityIdentifier = sidStringAndAttributesArray[i].SecurityIdentifier,
					Attributes = sidStringAndAttributesArray[i].Attributes
				};
			}
			return array;
		}

		private void SetErrorResultOnUnexpectedResponse()
		{
			this.query.SetResultOnFirstCall(base.Application.CreateQueryResult(this.GenerateException(string.Empty)));
		}

		private GetFolderRequestProcessingException GenerateException(string error)
		{
			return new GetFolderRequestProcessingException(Strings.descProxyRequestProcessingError(error, this.GetHttpRequestString()));
		}

		private string GetHttpRequestString()
		{
			return string.Format("GetFolderRequest url = {0}, mailbox = {1}\n", this.url, this.query.Email);
		}

		private readonly ExchangeServiceBinding binding;

		private readonly BaseQuery query;

		private readonly string url;

		private static readonly FolderResponseShapeType GetFolderShape = new FolderResponseShapeType
		{
			BaseShape = DefaultShapeNamesType.IdOnly
		};

		private static readonly Trace GetFolderRequestTracer = ExTraceGlobals.GetFolderRequestTracer;
	}
}
