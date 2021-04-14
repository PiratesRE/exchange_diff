using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EwsHttpWebRequestEx : IEwsHttpWebRequest
	{
		internal EwsHttpWebRequestEx(Uri uri)
		{
			this.request = (HttpWebRequest)WebRequest.Create(uri);
		}

		void IEwsHttpWebRequest.Abort()
		{
			this.request.Abort();
		}

		IAsyncResult IEwsHttpWebRequest.BeginGetRequestStream(AsyncCallback callback, object state)
		{
			return this.request.BeginGetRequestStream(callback, state);
		}

		IAsyncResult IEwsHttpWebRequest.BeginGetResponse(AsyncCallback callback, object state)
		{
			return this.request.BeginGetResponse(callback, state);
		}

		Stream IEwsHttpWebRequest.EndGetRequestStream(IAsyncResult asyncResult)
		{
			return this.request.EndGetRequestStream(asyncResult);
		}

		IEwsHttpWebResponse IEwsHttpWebRequest.EndGetResponse(IAsyncResult asyncResult)
		{
			return new EwsHttpWebResponse((HttpWebResponse)this.request.EndGetResponse(asyncResult));
		}

		Stream IEwsHttpWebRequest.GetRequestStream()
		{
			return this.request.GetRequestStream();
		}

		IEwsHttpWebResponse IEwsHttpWebRequest.GetResponse()
		{
			return new EwsHttpWebResponse(this.request.GetResponse() as HttpWebResponse);
		}

		string IEwsHttpWebRequest.Accept
		{
			get
			{
				return this.request.Accept;
			}
			set
			{
				this.request.Accept = value;
			}
		}

		bool IEwsHttpWebRequest.AllowAutoRedirect
		{
			get
			{
				return this.request.AllowAutoRedirect;
			}
			set
			{
				this.request.AllowAutoRedirect = value;
			}
		}

		X509CertificateCollection IEwsHttpWebRequest.ClientCertificates
		{
			get
			{
				return this.request.ClientCertificates;
			}
			set
			{
				this.request.ClientCertificates = value;
			}
		}

		string IEwsHttpWebRequest.ContentType
		{
			get
			{
				return this.request.ContentType;
			}
			set
			{
				this.request.ContentType = value;
			}
		}

		CookieContainer IEwsHttpWebRequest.CookieContainer
		{
			get
			{
				return this.request.CookieContainer;
			}
			set
			{
				this.request.CookieContainer = value;
			}
		}

		ICredentials IEwsHttpWebRequest.Credentials
		{
			get
			{
				return this.request.Credentials;
			}
			set
			{
				this.request.Credentials = value;
			}
		}

		WebHeaderCollection IEwsHttpWebRequest.Headers
		{
			get
			{
				return this.request.Headers;
			}
			set
			{
				this.request.Headers = value;
			}
		}

		string IEwsHttpWebRequest.Method
		{
			get
			{
				return this.request.Method;
			}
			set
			{
				this.request.Method = value;
			}
		}

		IWebProxy IEwsHttpWebRequest.Proxy
		{
			get
			{
				return this.request.Proxy;
			}
			set
			{
				this.request.Proxy = value;
			}
		}

		bool IEwsHttpWebRequest.PreAuthenticate
		{
			get
			{
				return this.request.PreAuthenticate;
			}
			set
			{
				this.request.PreAuthenticate = value;
			}
		}

		Uri IEwsHttpWebRequest.RequestUri
		{
			get
			{
				return this.request.RequestUri;
			}
		}

		int IEwsHttpWebRequest.Timeout
		{
			get
			{
				return this.request.Timeout;
			}
			set
			{
				this.request.Timeout = value;
			}
		}

		bool IEwsHttpWebRequest.UseDefaultCredentials
		{
			get
			{
				return this.request.UseDefaultCredentials;
			}
			set
			{
				this.request.UseDefaultCredentials = value;
			}
		}

		string IEwsHttpWebRequest.UserAgent
		{
			get
			{
				return this.request.UserAgent;
			}
			set
			{
				this.request.UserAgent = value;
			}
		}

		public bool KeepAlive
		{
			get
			{
				return this.request.KeepAlive;
			}
			set
			{
				this.request.KeepAlive = value;
			}
		}

		public string ConnectionGroupName
		{
			get
			{
				return this.request.ConnectionGroupName;
			}
			set
			{
				this.request.ConnectionGroupName = value;
			}
		}

		public RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				return this.request.ServerCertificateValidationCallback;
			}
			set
			{
				this.request.ServerCertificateValidationCallback = value;
			}
		}

		private HttpWebRequest request;
	}
}
