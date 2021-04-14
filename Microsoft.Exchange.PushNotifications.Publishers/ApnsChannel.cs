using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsChannel : PushNotificationChannel<ApnsNotification>
	{
		public ApnsChannel(ApnsChannelSettings settings, ITracer tracer) : base(settings.AppId, tracer)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			settings.Validate();
			this.config = settings;
			this.State = ApnsChannelState.Init;
			this.WaitingExit = ExDateTime.UtcNow;
			this.unconfirmedNotifications = new Queue<ApnsNotification>();
			this.Counters = ApnsChannelCounters.GetInstance(base.AppId);
		}

		private protected ApnsChannelState State { protected get; private set; }

		private protected ExDateTime WaitingExit { protected get; private set; }

		protected bool IsDisconnected
		{
			get
			{
				return this.tcpClient == null && this.sslStream == null && this.readTask == null;
			}
		}

		private ApnsChannelCountersInstance Counters { get; set; }

		public override void Send(ApnsNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (!notification.IsValid)
			{
				this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(notification, new InvalidPushNotificationException(notification.ValidationErrors[0])));
				return;
			}
			ApnsChannelContext apnsChannelContext = new ApnsChannelContext(notification, cancelToken, base.Tracer, this.config);
			ApnsChannelState apnsChannelState = this.State;
			while (apnsChannelContext.IsActive)
			{
				this.CheckCancellation(apnsChannelContext);
				switch (this.State)
				{
				case ApnsChannelState.Init:
					apnsChannelState = this.ProcessInit(apnsChannelContext);
					break;
				case ApnsChannelState.Connecting:
					apnsChannelState = this.ProcessConnecting(apnsChannelContext);
					break;
				case ApnsChannelState.DelayingConnect:
					apnsChannelState = this.ProcessDelayingConnect(apnsChannelContext);
					break;
				case ApnsChannelState.Authenticating:
					apnsChannelState = this.ProcessAuthenticating(apnsChannelContext);
					break;
				case ApnsChannelState.Reading:
					apnsChannelState = this.ProcessReading(apnsChannelContext);
					break;
				case ApnsChannelState.Sending:
					apnsChannelState = this.ProcessSending(apnsChannelContext);
					break;
				case ApnsChannelState.Waiting:
					apnsChannelState = this.ProcessWaiting(apnsChannelContext);
					break;
				}
				base.Tracer.TraceDebug<ApnsChannelState, ApnsChannelState>((long)this.GetHashCode(), "[Send] Transitioning from {0} to {1}", this.State, apnsChannelState);
				this.State = apnsChannelState;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[InternalDispose] Disposing the channel for '{0}'", base.AppId);
				this.State = this.TransitionToInit();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ApnsChannel>(this);
		}

		protected virtual ApnsCertProvider CreateCertProvider()
		{
			return new ApnsCertProvider(new ApnsCertStore(new X509Store(StoreName.My, StoreLocation.LocalMachine)), this.config.IgnoreCertificateErrors, base.Tracer, base.AppId);
		}

		protected virtual void Delay(int delayTime)
		{
			if (delayTime > 0)
			{
				Thread.Sleep(delayTime);
			}
		}

		protected virtual ApnsTcpClient CreateTcpClient()
		{
			return new ApnsTcpClient(new TcpClient());
		}

		protected virtual ApnsSslStream CreateSslStream()
		{
			return new ApnsSslStream(new SslStream(this.tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(this.certProvider.ValidateCertificate), (object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers) => this.cert));
		}

		private ApnsChannelState ProcessInit(ApnsChannelContext currentNotification)
		{
			if (this.certProvider == null)
			{
				this.certProvider = this.CreateCertProvider();
			}
			base.Tracer.TraceDebug<string, ApnsChannelContext>((long)this.GetHashCode(), "[ProcessInit] Finding certificate by thumbprint '{0}' for {1}", this.config.CertificateThumbprint, currentNotification);
			this.cert = null;
			ApnsChannelState result;
			try
			{
				this.cert = this.certProvider.LoadCertificate(this.config.CertificateThumbprint, this.config.CertificateThumbprintFallback);
				result = ApnsChannelState.DelayingConnect;
			}
			catch (ApnsCertificateException)
			{
				result = this.TransitionToWaiting();
			}
			return result;
		}

		private ApnsChannelState ProcessDelayingConnect(ApnsChannelContext currentNotification)
		{
			int num = currentNotification.CurrentRetryDelay;
			if (num > 0)
			{
				base.Tracer.TraceDebug<int, ApnsChannelContext>((long)this.GetHashCode(), "[ProcessDelayingConnect] Delaying our next connection try {0} milliseconds for {1}", num, currentNotification);
			}
			bool flag = false;
			while (!flag && !currentNotification.IsCancelled)
			{
				if (num > this.config.ConnectStepTimeout)
				{
					this.Delay(this.config.ConnectStepTimeout);
					num -= this.config.ConnectStepTimeout;
				}
				else
				{
					this.Delay(num);
					flag = true;
				}
			}
			return ApnsChannelState.Connecting;
		}

		private ApnsChannelState ProcessConnecting(ApnsChannelContext currentNotification)
		{
			this.tcpClient = this.CreateTcpClient();
			base.Tracer.TraceDebug<string, int, ApnsChannelContext>((long)this.GetHashCode(), "[ProcessConnecting] Connecting to {0}:{1} for {2}", this.config.Host, this.config.Port, currentNotification);
			bool flag = true;
			AverageTimeCounterBase averageTimeCounterBase = new AverageTimeCounterBase(this.Counters.AverageApnsConnectionTime, this.Counters.AverageApnsConnectionTimeBase, true);
			try
			{
				IAsyncResult asyncResult = this.tcpClient.BeginConnect(this.config.Host, this.config.Port, null, base.AppId);
				while (!currentNotification.IsCancelled)
				{
					if (asyncResult.AsyncWaitHandle.WaitOne(this.config.ConnectStepTimeout))
					{
						this.tcpClient.EndConnect(asyncResult);
						if (this.tcpClient.Connected)
						{
							flag = false;
							currentNotification.ResetConnectRetries();
							averageTimeCounterBase.Stop();
							PushNotificationsMonitoring.PublishSuccessNotification("ApnsChannelConnect", base.AppId);
							return ApnsChannelState.Authenticating;
						}
						this.Counters.ApnsConnectionFailed.Increment();
						base.Tracer.TraceDebug((long)this.GetHashCode(), "[ProcessConnecting] EndConnect didn't fail but the TcpClient was not connected");
						break;
					}
				}
			}
			catch (SocketException ex)
			{
				string text = ex.ToTraceString();
				base.Tracer.TraceError<string>((long)this.GetHashCode(), "[TryEndConnect] Unexpected SocketException: {0}", text);
				if (currentNotification.IsRetryable)
				{
					PushNotificationsCrimsonEvents.PushNotificationRetryableError.Log<string, string, string>(base.AppId, string.Empty, text);
				}
				else
				{
					PushNotificationsCrimsonEvents.ApnsChannelConnectError.Log<string, SocketError, string>(base.AppId, ex.SocketErrorCode, text);
				}
			}
			finally
			{
				if (flag)
				{
					this.Disconnect();
				}
			}
			if (currentNotification.IsRetryable || currentNotification.IsCancelled)
			{
				currentNotification.IncrementConnectRetries();
				return ApnsChannelState.DelayingConnect;
			}
			PushNotificationsMonitoring.PublishFailureNotification("ApnsChannelConnect", base.AppId, "");
			return this.TransitionToWaiting();
		}

		private ApnsChannelState ProcessAuthenticating(ApnsChannelContext currentNotification)
		{
			this.sslStream = this.CreateSslStream();
			base.Tracer.TraceDebug<string, ApnsChannelContext>((long)this.GetHashCode(), "[ProcessAuthenticating] Authenticating to {0} for {1}", this.config.Host, currentNotification);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool flag = true;
			int num = this.config.ConnectTotalTimeout;
			AverageTimeCounterBase averageTimeCounterBase = new AverageTimeCounterBase(this.Counters.AverageApnsAuthTime, this.Counters.AverageApnsAuthTimeBase, true);
			try
			{
				IAsyncResult asyncResult = this.sslStream.BeginAuthenticateAsClient(this.config.Host, new X509Certificate2Collection(this.cert), SslProtocols.Default, false, null, base.AppId);
				while (!currentNotification.IsCancelled)
				{
					if (asyncResult.AsyncWaitHandle.WaitOne(this.config.ConnectStepTimeout))
					{
						this.sslStream.EndAuthenticateAsClient(asyncResult);
						if (this.sslStream.IsMutuallyAuthenticated && this.sslStream.CanRead && this.sslStream.CanWrite)
						{
							stopwatch.Stop();
							averageTimeCounterBase.Stop();
							if (this.averageTimeApnsChannelOpen == null)
							{
								this.averageTimeApnsChannelOpen = new AverageTimeCounterBase(this.Counters.AverageApnsChannelOpenTime, this.Counters.AverageApnsChannelOpenTimeBase, true);
							}
							flag = false;
							PushNotificationsCrimsonEvents.ApnsSuccessfulAuthentication.Log<string, long>(base.AppId, stopwatch.ElapsedMilliseconds);
							PushNotificationsMonitoring.PublishSuccessNotification("ApnsChannelAuthenticate", base.AppId);
							this.sslStream.WriteTimeout = this.config.WriteTimeout;
							return ApnsChannelState.Reading;
						}
						base.Tracer.TraceDebug<string, string, string>((long)ApnsChannel.ClassTraceId, "[ProcessAuthenticating] Closing stream{0}{1}{2}", this.sslStream.IsMutuallyAuthenticated ? string.Empty : " - Stream not mutually authenticated", this.sslStream.CanRead ? string.Empty : " - Stream not readable", this.sslStream.CanWrite ? string.Empty : " - Stream not writable");
						break;
					}
					else
					{
						num -= this.config.ConnectStepTimeout;
						if (num <= 0)
						{
							this.ScheduleConnectionCleanup(asyncResult);
							throw new AuthenticationException(Strings.ApnsChannelAuthenticationTimeout);
						}
					}
				}
			}
			catch (AuthenticationException aex)
			{
				string text = aex.ToTraceString();
				base.Tracer.TraceError<string>((long)this.GetHashCode(), "[ProcessAuthenticating] AuthenticationException calling EndAuthenticateAsClient: {0}", text);
				if (currentNotification.IsRetryable)
				{
					PushNotificationsCrimsonEvents.PushNotificationRetryableError.Log<string, string, string>(base.AppId, string.Empty, text);
				}
				else
				{
					PushNotificationsCrimsonEvents.ApnsChannelAuthenticateError.Log<string, string, string>(base.AppId, string.Empty, text);
				}
			}
			catch (IOException ioex)
			{
				string text2 = ioex.ToTraceString();
				base.Tracer.TraceError<string>((long)this.GetHashCode(), "[ProcessAuthenticating] IOException calling EndAuthenticateAsClient: {0}", text2);
				if (currentNotification.IsRetryable)
				{
					PushNotificationsCrimsonEvents.PushNotificationRetryableError.Log<string, string, string>(base.AppId, string.Empty, text2);
				}
				else
				{
					PushNotificationsCrimsonEvents.ApnsChannelAuthenticateError.Log<string, string, string>(base.AppId, string.Empty, text2);
				}
			}
			finally
			{
				if (flag)
				{
					stopwatch.Stop();
					this.Disconnect();
				}
			}
			if (currentNotification.IsRetryable || currentNotification.IsCancelled)
			{
				currentNotification.IncrementAuthenticateRetries();
				return this.TransitionToInit();
			}
			PushNotificationsMonitoring.PublishFailureNotification("ApnsChannelAuthenticate", base.AppId, "");
			return this.TransitionToWaiting();
		}

		private ApnsChannelState ProcessReading(ApnsChannelContext currentNotification)
		{
			if (this.readTask == null)
			{
				this.readTask = Task.Factory.StartNew<ApnsResponse>(() => this.Read());
			}
			else if (this.readTask.IsCompleted)
			{
				this.Counters.ApnsReadTaskEnded.Increment();
				base.Tracer.TraceDebug((long)this.GetHashCode(), "[ProcessReading] Read task finished. Analyze the result and reset.");
				return this.TransitionToInit();
			}
			return ApnsChannelState.Sending;
		}

		private ApnsChannelState ProcessSending(ApnsChannelContext currentNotification)
		{
			ApnsChannelState result;
			try
			{
				AverageTimeCounterBase averageTimeCounterBase = new AverageTimeCounterBase(this.Counters.AverageApnsChannelSendTime, this.Counters.AverageApnsChannelSendTimeBase, true);
				byte[] buffer = currentNotification.Notification.ConvertToApnsBinaryFormat();
				if (currentNotification.Notification.IsMonitoring)
				{
					PushNotificationsMonitoring.PublishSuccessNotification("NotificationProcessed", base.AppId);
				}
				else
				{
					this.sslStream.Write(buffer);
					PushNotificationTracker.ReportSent(currentNotification.Notification, PushNotificationPlatform.None);
					currentNotification.Notification.SentTime = ExDateTime.UtcNow;
					this.unconfirmedNotifications.Enqueue(currentNotification.Notification);
					this.ConfirmCachedNotifications();
					averageTimeCounterBase.Stop();
				}
				currentNotification.Done();
				result = ApnsChannelState.Reading;
			}
			catch (IOException ioex)
			{
				string text = ioex.ToTraceString();
				base.Tracer.TraceError<string, string>((long)this.GetHashCode(), "[ProcessSending] IOException while sending notification '{0}'. {1}", currentNotification.Notification.ToFullString(), text);
				PushNotificationsCrimsonEvents.ApnsChannelSendingError.Log<string, string, string>(base.AppId, currentNotification.Notification.ToFullString(), text);
				currentNotification.Drop(text);
				result = this.TransitionToInit();
			}
			return result;
		}

		private ApnsChannelState ProcessWaiting(ApnsChannelContext currentNotification)
		{
			if (this.WaitingExit > ExDateTime.UtcNow)
			{
				currentNotification.Drop(this.State.ToString());
				return ApnsChannelState.Waiting;
			}
			return ApnsChannelState.Init;
		}

		private ApnsChannelState TransitionToInit()
		{
			this.Reset();
			return ApnsChannelState.Init;
		}

		private ApnsChannelState TransitionToWaiting()
		{
			this.Reset();
			this.WaitingExit = ExDateTime.UtcNow.AddSeconds((double)this.config.BackOffTimeInSeconds);
			PushNotificationsCrimsonEvents.ApnsChannelTransitionToWaiting.LogPeriodic<string, ExDateTime>(base.AppId, TimeSpan.FromSeconds((double)this.config.BackOffTimeInSeconds), base.AppId, this.WaitingExit);
			return ApnsChannelState.Waiting;
		}

		private void CheckCancellation(ApnsChannelContext currentNotification)
		{
			if (currentNotification.IsCancelled)
			{
				switch (this.State)
				{
				case ApnsChannelState.Authenticating:
					this.State = this.TransitionToInit();
					break;
				case ApnsChannelState.Sending:
					this.State = ApnsChannelState.Reading;
					break;
				}
				base.Tracer.TraceDebug<ApnsChannelState>((long)this.GetHashCode(), "[CheckCancellation] Cancellation requested, next state is {0}", this.State);
				throw new OperationCanceledException();
			}
		}

		private ApnsResponse Read()
		{
			ApnsResponse apnsResponse = null;
			try
			{
				base.Tracer.TraceDebug((long)this.GetHashCode(), "[Read] Beginning read from APNs");
				byte[] array = new byte[6];
				if (6 == this.sslStream.Read(array, 0, 6))
				{
					apnsResponse = ApnsResponse.FromApnsFormat(array);
					base.Tracer.TraceError<ApnsResponse>((long)this.GetHashCode(), "[Read] APNs response: '{0}'", apnsResponse);
				}
				else
				{
					base.Tracer.TraceWarning((long)this.GetHashCode(), "[Read] Unexpected number of bytes read from the SSL stream");
				}
			}
			catch (ObjectDisposedException exception)
			{
				base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[Read] {0}", exception.ToTraceString());
			}
			return apnsResponse;
		}

		private void AnalyzeReadResult(bool ignoreException)
		{
			if (this.readTask.IsCanceled)
			{
				return;
			}
			if (!this.readTask.IsFaulted)
			{
				if (this.readTask.Result != null)
				{
					ApnsNotification apnsNotification = null;
					foreach (ApnsNotification apnsNotification2 in this.unconfirmedNotifications)
					{
						if (apnsNotification2.SequenceNumber == this.readTask.Result.Identifier)
						{
							apnsNotification = apnsNotification2;
							break;
						}
						base.Tracer.TraceDebug<ApnsNotification>((long)this.GetHashCode(), "[AnalyzeReadResult] Notification confirmed '{0}'", apnsNotification2);
					}
					if (apnsNotification != null)
					{
						this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(apnsNotification, new InvalidPushNotificationException(Strings.InvalidReportFromApns(this.readTask.Result.ToString()))));
						return;
					}
				}
			}
			else
			{
				Exception innerException = this.readTask.Exception.InnerException;
				base.Tracer.TraceError<Exception>((long)this.GetHashCode(), "[AnalyzeReadResult] Read task failed with an exception: {0}", innerException);
				if (!ignoreException)
				{
					PushNotificationsCrimsonEvents.ApnsChannelReadError.Log<string, string, Exception>(base.AppId, string.Empty, this.readTask.Exception.InnerException);
				}
			}
		}

		private void ConfirmCachedNotifications()
		{
			ExDateTime t = ExDateTime.UtcNow.Subtract(TimeSpan.FromSeconds(2.0));
			while (this.unconfirmedNotifications.Count > 0 && this.unconfirmedNotifications.Peek().SentTime < t)
			{
				base.Tracer.TraceDebug<ApnsNotification>((long)this.GetHashCode(), "[ConfirmCachedNotifications] Notification confirmed '{0}'", this.unconfirmedNotifications.Dequeue());
			}
		}

		private void Reset()
		{
			this.Counters.ApnsChannelReset.Increment();
			if (this.averageTimeApnsChannelOpen != null)
			{
				this.averageTimeApnsChannelOpen.Stop();
				this.averageTimeApnsChannelOpen = null;
			}
			base.Tracer.TraceDebug((long)this.GetHashCode(), "[Reset] Resetting channel");
			this.ConfirmCachedNotifications();
			this.Disconnect();
			this.cert = null;
			this.WaitingExit = ExDateTime.UtcNow;
			if (this.unconfirmedNotifications.Count != 0)
			{
				this.unconfirmedNotifications = new Queue<ApnsNotification>();
			}
		}

		private void Disconnect()
		{
			base.Tracer.TraceDebug((long)this.GetHashCode(), "[Disconnect] Disconnecting channel");
			bool ignoreException = this.readTask == null || !this.readTask.IsCompleted;
			if (this.sslStream != null)
			{
				this.sslStream.Dispose();
			}
			if (this.tcpClient != null)
			{
				this.tcpClient.Dispose();
			}
			if (this.readTask != null)
			{
				if (!this.readTask.IsCompleted)
				{
					base.Tracer.TraceDebug((long)this.GetHashCode(), "[Disconnect] Waiting for the Read task to finish");
					try
					{
						this.readTask.Wait();
					}
					catch (AggregateException)
					{
					}
				}
				this.AnalyzeReadResult(ignoreException);
				this.readTask.Dispose();
			}
			this.readTask = null;
			this.sslStream = null;
			this.tcpClient = null;
		}

		private void ScheduleConnectionCleanup(IAsyncResult beginAuthenticateResult)
		{
			base.Tracer.TraceDebug((long)this.GetHashCode(), "[ScheduleConnectionCleanup] Creating a task to close this.sslStream when authentication ends");
			ApnsSslStream tempStreamRef = this.sslStream;
			ApnsTcpClient tempClientRef = this.tcpClient;
			string tempAppIdRef = base.AppId;
			this.sslStream = null;
			this.tcpClient = null;
			Task task = new Task(delegate()
			{
				try
				{
					tempStreamRef.EndAuthenticateAsClient(beginAuthenticateResult);
				}
				catch (Exception exception)
				{
					string text = exception.ToTraceString();
					this.Tracer.TraceError<string>((long)ApnsChannel.ClassTraceId, "[ScheduleConnectionCleanup] Unexpected exception: {0}", text);
					PushNotificationsCrimsonEvents.ApnsChannelCleanupUnexpectedError.Log<string, string, string>(tempAppIdRef, string.Empty, text);
				}
				finally
				{
					tempStreamRef.Dispose();
					tempClientRef.Dispose();
				}
			});
			task.Start();
		}

		private static readonly int ClassTraceId = typeof(ApnsChannel).GetHashCode();

		private ApnsChannelSettings config;

		private X509Certificate2 cert;

		private ApnsCertProvider certProvider;

		private ApnsTcpClient tcpClient;

		private ApnsSslStream sslStream;

		private Queue<ApnsNotification> unconfirmedNotifications;

		private Task<ApnsResponse> readTask;

		private AverageTimeCounterBase averageTimeApnsChannelOpen;
	}
}
