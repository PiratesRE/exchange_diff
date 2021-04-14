using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsSslStream : IDisposable
	{
		public ApnsSslStream(SslStream sslStream)
		{
			if (sslStream == null)
			{
				throw new ArgumentNullException("sslStream");
			}
			this.SslStream = sslStream;
		}

		public virtual int ReadTimeout
		{
			set
			{
				this.SslStream.ReadTimeout = value;
			}
		}

		public virtual int WriteTimeout
		{
			set
			{
				this.SslStream.WriteTimeout = value;
			}
		}

		public virtual bool CanRead
		{
			get
			{
				return this.SslStream.CanRead;
			}
		}

		public virtual bool CanWrite
		{
			get
			{
				return this.SslStream.CanWrite;
			}
		}

		public virtual bool IsMutuallyAuthenticated
		{
			get
			{
				return this.SslStream.IsMutuallyAuthenticated;
			}
		}

		private SslStream SslStream { get; set; }

		public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
		{
			return this.SslStream.BeginAuthenticateAsClient(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation, asyncCallback, asyncState);
		}

		public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
		{
			this.SslStream.EndAuthenticateAsClient(asyncResult);
		}

		public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
		{
			this.SslStream.AuthenticateAsClient(targetHost, clientCertificates, enabledSslProtocols, checkCertificateRevocation);
		}

		public virtual int Read(byte[] buffer, int offset, int count)
		{
			return this.SslStream.Read(buffer, offset, count);
		}

		public virtual IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return this.SslStream.BeginRead(buffer, offset, count, asyncCallback, asyncState);
		}

		public virtual int EndRead(IAsyncResult ar)
		{
			return this.SslStream.EndRead(ar);
		}

		public virtual void Write(byte[] buffer)
		{
			this.SslStream.Write(buffer);
		}

		public virtual void Dispose()
		{
			this.SslStream.Close();
		}
	}
}
