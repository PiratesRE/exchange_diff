using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsFeedbackStream
	{
		public ApnsFeedbackStream(string appId, ApnsChannelSettings settings) : this(appId, settings, ExTraceGlobals.ApnsPublisherTracer)
		{
		}

		public ApnsFeedbackStream(string appId, ApnsChannelSettings settings, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("appId", appId);
			ArgumentValidator.ThrowIfNull("settings", settings);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.appId = appId;
			this.config = settings;
			this.tracer = tracer;
		}

		public IEnumerable<ApnsFeedbackResponse> ReadFeedbackResponses()
		{
			this.tracer.TraceDebug((long)this.GetHashCode(), "[ReadFeedbackResponses] Requesting feedback from APNs");
			PushNotificationsCrimsonEvents.ApnsFeedbackChannelConsuming.Log<string>(this.appId);
			ApnsCertProvider certProvider = this.CreateCertProvider();
			X509Certificate2 cert = certProvider.LoadCertificate(this.config.CertificateThumbprint, this.config.CertificateThumbprintFallback);
			ApnsFeedbackStream.ApnsFeedbackClient feedbackClient = new ApnsFeedbackStream.ApnsFeedbackClient();
			feedbackClient.TcpClient = this.CreateTcpClient();
			this.Connect(feedbackClient);
			feedbackClient.SslStream = this.CreateSslStream(feedbackClient.TcpClient, certProvider, cert);
			this.Authenticate(feedbackClient, cert);
			for (;;)
			{
				ApnsFeedbackResponse response = this.Read(feedbackClient);
				if (response == null)
				{
					break;
				}
				yield return response;
			}
			yield break;
			yield break;
		}

		protected virtual ApnsCertProvider CreateCertProvider()
		{
			return new ApnsCertProvider(new ApnsCertStore(new X509Store(StoreName.My, StoreLocation.LocalMachine)), this.config.IgnoreCertificateErrors, this.tracer, this.appId);
		}

		protected virtual ApnsTcpClient CreateTcpClient()
		{
			return new ApnsTcpClient(new TcpClient());
		}

		protected virtual ApnsSslStream CreateSslStream(ApnsTcpClient tcpClient, ApnsCertProvider certProvider, X509Certificate2 cert)
		{
			return new ApnsSslStream(new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(certProvider.ValidateCertificate), (object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers) => cert));
		}

		private void Connect(ApnsFeedbackStream.ApnsFeedbackClient feedbackClient)
		{
			try
			{
				feedbackClient.TcpClient.Connect(this.config.FeedbackHost, this.config.FeedbackPort);
			}
			catch (SocketException exception)
			{
				throw this.HandleException("Connect", exception);
			}
		}

		private void Authenticate(ApnsFeedbackStream.ApnsFeedbackClient feedbackClient, X509Certificate2 cert)
		{
			try
			{
				IAsyncResult asyncResult = feedbackClient.SslStream.BeginAuthenticateAsClient(this.config.FeedbackHost, new X509Certificate2Collection
				{
					cert
				}, SslProtocols.Default, false, null, null);
				if (!asyncResult.AsyncWaitHandle.WaitOne(this.config.ConnectTotalTimeout))
				{
					feedbackClient.AuthenticateAsyncResult = asyncResult;
					throw this.HandleException("Authenticate", new AuthenticationException(Strings.ApnsChannelAuthenticationTimeout));
				}
				feedbackClient.SslStream.EndAuthenticateAsClient(asyncResult);
			}
			catch (AuthenticationException exception)
			{
				throw this.HandleException("Authenticate", exception);
			}
			catch (IOException exception2)
			{
				throw this.HandleException("Authenticate", exception2);
			}
		}

		private ApnsFeedbackResponse Read(ApnsFeedbackStream.ApnsFeedbackClient feedbackClient)
		{
			byte[] array = new byte[38];
			int num = 0;
			int num2 = 38;
			bool flag = false;
			ApnsFeedbackResponse result;
			try
			{
				while (!flag)
				{
					int num3 = feedbackClient.SslStream.Read(array, num, num2);
					if (num3 == 0 && num == 0)
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "[Read] No more feedback available from APNs at this moment");
						PushNotificationsCrimsonEvents.ApnsFeedbackChannelDone.Log<string>(this.appId);
						return null;
					}
					if (num3 == 0)
					{
						throw this.HandleException("Read", new ApnsFeedbackException(Strings.ApnsFeedbackResponseInvalidLength(num)));
					}
					num += num3;
					num2 -= num3;
					flag = (num2 == 0);
				}
				ApnsFeedbackResponse apnsFeedbackResponse = ApnsFeedbackResponse.FromApnsFormat(array);
				this.tracer.TraceDebug<ApnsFeedbackResponse>((long)this.GetHashCode(), "[Read] Feedback response from APNs: {0}", apnsFeedbackResponse);
				PushNotificationsCrimsonEvents.ApnsFeedbackChannelResponse.Log<string, string, ApnsFeedbackResponse>(this.appId, string.Empty, apnsFeedbackResponse);
				result = apnsFeedbackResponse;
			}
			catch (IOException exception)
			{
				throw this.HandleException("Read", exception);
			}
			return result;
		}

		private Exception HandleException(string header, Exception exception)
		{
			this.tracer.TraceError<string, string>((long)this.GetHashCode(), "[{0}] Exception: {1}", header, exception.ToTraceString());
			PushNotificationsCrimsonEvents.ApnsFeedbackChannelError.Log<string, string, string>(this.appId, string.Empty, exception.ToTraceString());
			if (exception is ApnsFeedbackException)
			{
				return exception;
			}
			return new ApnsFeedbackException(Strings.ApnsFeedbackError(this.appId, exception.GetType().ToString(), exception.Message), exception);
		}

		private readonly string appId;

		private ApnsChannelSettings config;

		private ITracer tracer;

		private class ApnsFeedbackClient : IDisposable
		{
			public string AppId { get; set; }

			public ApnsTcpClient TcpClient { get; set; }

			public ApnsSslStream SslStream { get; set; }

			public IAsyncResult AuthenticateAsyncResult { get; set; }

			public void Dispose()
			{
				this.Dispose(true);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						if (this.AuthenticateAsyncResult != null)
						{
							Task task = new Task(delegate()
							{
								try
								{
									this.SslStream.EndAuthenticateAsClient(this.AuthenticateAsyncResult);
								}
								catch (Exception exception)
								{
									PushNotificationsCrimsonEvents.ApnsChannelCleanupUnexpectedError.Log<string, string, string>(this.AppId, string.Empty, exception.ToTraceString());
								}
								finally
								{
									this.Disconnect();
								}
							});
							task.Start();
						}
						else
						{
							this.Disconnect();
						}
					}
					this.disposed = true;
				}
			}

			private void Disconnect()
			{
				if (this.SslStream != null)
				{
					this.SslStream.Dispose();
				}
				if (this.TcpClient != null)
				{
					this.TcpClient.Dispose();
				}
			}

			private bool disposed;
		}
	}
}
