using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class ProtocolResponse : IDisposeTrackable, IDisposable
	{
		public ProtocolResponse(ProtocolRequest request) : this(request, null)
		{
		}

		public ProtocolResponse(string input) : this(null, input)
		{
		}

		public ProtocolResponse(ProtocolRequest request, string input)
		{
			this.responseStringBuilder = new StringBuilder(256);
			this.responseStringBuilder.Append(input);
			this.request = request;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public ProtocolRequest Request
		{
			get
			{
				return this.request;
			}
		}

		public bool IsDisconnectResponse
		{
			get
			{
				return this.disconnectAfterSend;
			}
			set
			{
				this.disconnectAfterSend = value;
			}
		}

		public bool NeedToDelayStoreAction
		{
			get
			{
				return this.request != null && this.request.NeedToDelayStoreAction;
			}
		}

		public virtual bool IsCommandFailedResponse
		{
			get
			{
				return false;
			}
		}

		public bool StartTls
		{
			get
			{
				return this.startTlsAfterSend;
			}
			set
			{
				this.startTlsAfterSend = value;
			}
		}

		public string MessageString
		{
			get
			{
				return this.responseStringBuilder.ToString();
			}
			set
			{
				this.responseStringBuilder.Length = 0;
				this.responseStringBuilder.Append(value);
			}
		}

		public virtual string DataToSend
		{
			get
			{
				return this.responseStringBuilder.ToString();
			}
		}

		public void Append(char value)
		{
			this.responseStringBuilder.Append(value);
		}

		public void Append(string value)
		{
			this.responseStringBuilder.Append(value);
		}

		public void AppendFormat(string format, params object[] args)
		{
			this.responseStringBuilder.AppendFormat(format, args);
		}

		public void AppendFormat(string format, object arg0)
		{
			this.responseStringBuilder.AppendFormat(format, arg0);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ProtocolResponse>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.request != null)
				{
					IDisposable disposable = this.request as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					this.request = null;
				}
			}
		}

		protected StringBuilder GetAuthError()
		{
			IProxyLogin proxyLogin = this.Request as IProxyLogin;
			if (proxyLogin != null && (!string.IsNullOrEmpty(proxyLogin.ClientIp) || ProtocolBaseServices.AuthErrorReportEnabled(proxyLogin.UserName)))
			{
				StringBuilder stringBuilder = new StringBuilder(128);
				if (!string.IsNullOrEmpty(proxyLogin.AuthenticationError))
				{
					if (proxyLogin.AuthenticationError.IndexOf('"') > -1)
					{
						throw new ApplicationException("Error string contains quotes:" + proxyLogin.AuthenticationError);
					}
					if (proxyLogin.AuthenticationError.IndexOfAny(ProtocolResponse.QuotableChars) > -1)
					{
						stringBuilder.AppendFormat("Error=\"{0}\"", proxyLogin.AuthenticationError);
					}
					else
					{
						stringBuilder.AppendFormat("Error={0}", proxyLogin.AuthenticationError);
					}
				}
				if (!string.IsNullOrEmpty(proxyLogin.ProxyDestination))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(' ');
					}
					stringBuilder.AppendFormat("Proxy={0}", proxyLogin.ProxyDestination);
				}
				if (stringBuilder.Length > 0)
				{
					return stringBuilder;
				}
			}
			return null;
		}

		protected static readonly char[] QuotableChars = new char[]
		{
			' ',
			'='
		};

		private bool disconnectAfterSend;

		private bool startTlsAfterSend;

		private StringBuilder responseStringBuilder;

		private ProtocolRequest request;

		private DisposeTracker disposeTracker;
	}
}
