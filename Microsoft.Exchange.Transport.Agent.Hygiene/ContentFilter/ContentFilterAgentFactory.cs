using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Interop;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.ContentFilter;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.ContentFilter
{
	public sealed class ContentFilterAgentFactory : SmtpReceiveAgentFactory, IDisposable
	{
		public ContentFilterAgentFactory()
		{
			this.dispatcher = new ContentFilterAgentFactory.Dispatcher(this);
			CommonUtils.RegisterConfigurationChangeHandlers("Content Filtering", new ADOperation(this.RegisterConfigurationChangeHandlers), ExTraceGlobals.InitializationTracer, this);
			this.Configure(true);
			this.antispamUpdateModePollingTimer = new Timer(new TimerCallback(this.AntispamUpdateServiceMonitor), null, 600000, -1);
			this.isDataCenterEnvironment = Datacenter.IsMultiTenancyEnabled();
		}

		~ContentFilterAgentFactory()
		{
			this.Dispose(false);
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("ContentFilterAgentFactory");
			}
			BypassedRecipients bypassedRecipients = new BypassedRecipients(this.contentFilterConfig.BypassedRecipients, (server != null) ? server.AddressBook : null);
			return new ContentFilterAgent(this, this.contentFilterConfig, bypassedRecipients, this.bypassedSenders);
		}

		public override void Close()
		{
			this.UnregisterConfigurationChangeHandlers();
			Util.PerformanceCounters.RemoveCounters();
			this.Dispose();
		}

		internal bool IsDataCenterEnvironment
		{
			get
			{
				return this.isDataCenterEnvironment;
			}
		}

		internal IAsyncResult BeginScanMessage(AsyncCallback callback, ContentFilterAgent.AsyncState state)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			if (state == null)
			{
				throw new ArgumentNullException("state");
			}
			ContentFilterAgentFactory.ScanRequest scanRequest = new ContentFilterAgentFactory.ScanRequest(this.dispatcher, callback, state);
			this.dispatcher.Enqueue(scanRequest);
			return scanRequest;
		}

		internal ScanMessageResult EndScanMessage(IAsyncResult asyncResult)
		{
			ContentFilterAgentFactory.ScanRequest scanRequest = asyncResult as ContentFilterAgentFactory.ScanRequest;
			if (scanRequest == null)
			{
				throw new InvalidOperationException("the asyncResult argument must be one previously obtained from a call to BeginScanMessage().");
			}
			if (scanRequest.Exception != null)
			{
				throw scanRequest.Exception;
			}
			return scanRequest.ScanResult;
		}

		private void RegisterConfigurationChangeHandlers()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 237, "RegisterConfigurationChangeHandlers", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\ContentFilter\\Agent\\ContentFilterAgentFactory.cs");
			ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
			ADObjectId childId = orgContainerId.GetChildId("Transport Settings");
			ADObjectId childId2 = childId.GetChildId("Message Hygiene");
			TransportFacades.ConfigChanged += this.ConfigUpdate;
			this.configRequestCookie = ADNotificationAdapter.RegisterChangeNotification<ContentFilterConfig>(childId2, new ADNotificationCallback(this.Configure));
		}

		private void UnregisterConfigurationChangeHandlers()
		{
			TransportFacades.ConfigChanged -= this.ConfigUpdate;
			if (this.configRequestCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(this.configRequestCookie);
			}
		}

		private void ConfigUpdate(object source, EventArgs args)
		{
			this.Configure(false);
		}

		private void Configure(ADNotificationEventArgs args)
		{
			try
			{
				this.Configure(false);
			}
			catch (ExchangeConfigurationException arg)
			{
				string formatString = "SmartScreen could not be re-initialized with new configuration and will keep running with the current configuration. Details: {0}";
				ExTraceGlobals.InitializationTracer.TraceError<ExchangeConfigurationException>((long)this.GetHashCode(), formatString, arg);
			}
		}

		private void Configure(bool onStartup)
		{
			ContentFilterConfig contentFilterConfig;
			ADOperationResult adoperationResult;
			if (ADNotificationAdapter.TryReadConfiguration<ContentFilterConfig>(delegate()
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId.ForestWideOrgId);
				return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 318, "Configure", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\ContentFilter\\Agent\\ContentFilterAgentFactory.cs").FindSingletonConfigurationObject<ContentFilterConfig>();
			}, out contentFilterConfig, out adoperationResult))
			{
				this.contentFilterConfig = contentFilterConfig;
				this.bypassedSenders = new BypassedSenders(contentFilterConfig.BypassedSenders, contentFilterConfig.BypassedSenderDomains);
				this.SetPremiumModeEnabled(onStartup);
				lock (this.wrapperLock)
				{
					this.wrapper = this.InitializeFilter();
					return;
				}
			}
			CommonUtils.FailedToReadConfiguration("Content Filtering", onStartup, adoperationResult.Exception, ExTraceGlobals.InitializationTracer, Util.EventLogger, this);
		}

		private ContentFilterAgentFactory.ContentFilterWrapper RecreateContentFilterWrapper(ContentFilterAgentFactory.ContentFilterWrapper invalidWrapper)
		{
			lock (this.wrapperLock)
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("ContentFilterAgentFactory");
				}
				if (this.wrapper == invalidWrapper)
				{
					this.wrapper = this.InitializeFilter();
				}
			}
			return this.wrapper;
		}

		private ContentFilterAgentFactory.ContentFilterWrapper InitializeFilter()
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			ContentFilterAgentFactory.ContentFilterWrapper result;
			try
			{
				ComProxy comProxy = new ComProxy(Constants.ContentFilterWrapperGuid);
				ComArguments comArguments = new ComArguments();
				bool flag = false;
				ExTraceGlobals.InitializationTracer.TraceDebug((long)this.GetHashCode(), "Initializing the filter");
				comArguments[10] = Encoding.Unicode.GetBytes(directoryName);
				this.SaveCustomWordsToPropertyBag(comArguments);
				comArguments.SetBool(11, this.contentFilterConfig.OutlookEmailPostmarkValidationEnabled);
				comArguments.SetBool(17, this.premiumModeEnabled);
				try
				{
					Util.InitializeFilter(comProxy, comArguments);
				}
				catch (UnauthorizedAccessException arg)
				{
					ExTraceGlobals.InitializationTracer.TraceError<UnauthorizedAccessException>((long)this.GetHashCode(), "Caught UnauthorizedAccessException when initializing filter. Details: {0}. Retrying...", arg);
					flag = true;
				}
				catch (BadImageFormatException arg2)
				{
					ExTraceGlobals.InitializationTracer.TraceError<BadImageFormatException>((long)this.GetHashCode(), "Caught BadImageFormatException when initializing filter. Details: {0}. Retrying...", arg2);
					flag = true;
				}
				if (flag)
				{
					Thread.Sleep(1000);
					Util.InitializeFilter(comProxy, comArguments);
				}
				Util.LogContentFilterInitialized();
				ExTraceGlobals.InitializationTracer.TraceDebug((long)this.GetHashCode(), "Filter was successfully initialized");
				result = new ContentFilterAgentFactory.ContentFilterWrapper(comProxy);
			}
			catch (UnauthorizedAccessException ex)
			{
				ExTraceGlobals.InitializationTracer.TraceDebug<UnauthorizedAccessException>((long)this.GetHashCode(), "UnauthorizedAccessException when initializing filter: {0}", ex);
				Util.LogFailedWithUnauthorizedAccess(directoryName, ex);
				throw new ExchangeConfigurationException(ex.Message, ex);
			}
			catch (BadImageFormatException ex2)
			{
				ExTraceGlobals.InitializationTracer.TraceDebug<BadImageFormatException>((long)this.GetHashCode(), "BadImageFormatException when initializing filter: {0}", ex2);
				Util.LogFailedWithBadImageFormat(directoryName, ex2);
				throw new ExchangeConfigurationException(ex2.Message, ex2);
			}
			catch (FileNotFoundException ex3)
			{
				ExTraceGlobals.InitializationTracer.TraceDebug<FileNotFoundException>((long)this.GetHashCode(), "FileNotFoundException when initializing filter: {0}", ex3);
				Util.LogContentFilterInitFailedFileNotFound(ex3);
				throw new ExchangeConfigurationException(ex3.Message, ex3);
			}
			catch (COMException ex4)
			{
				ExTraceGlobals.InitializationTracer.TraceDebug<COMException>((long)this.GetHashCode(), "COMException when initializing filter: {0}", ex4);
				if (ex4.ErrorCode == -2147024774)
				{
					Util.LogFailedInsufficientBuffer(ex4);
				}
				else if (ex4.ErrorCode == -2147023649)
				{
					Util.LogFailedFSWatcherAlreadyInitialized(ex4);
				}
				else if (ex4.ErrorCode == -1067253755)
				{
					Util.LogExSMimeFailedToInitialize(ex4);
				}
				throw new ExchangeConfigurationException(ex4.Message, ex4);
			}
			catch (Exception ex5)
			{
				ExTraceGlobals.InitializationTracer.TraceDebug<Exception>((long)this.GetHashCode(), "Failed to initialize filter: {0}", ex5);
				Util.LogContentFilterNotInitialized(ex5);
				throw new ExchangeConfigurationException(ex5.Message, ex5);
			}
			return result;
		}

		private void SaveCustomWordsToPropertyBag(ComArguments comArguments)
		{
			ReadOnlyCollection<ContentFilterPhrase> phrases = this.contentFilterConfig.GetPhrases();
			int count = phrases.Count;
			byte[][] array = new byte[count][];
			byte[][] array2 = new byte[count][];
			byte[][] array3 = new byte[count][];
			int num = 0;
			int num2 = 0;
			foreach (ContentFilterPhrase contentFilterPhrase in phrases)
			{
				array[num] = Encoding.Unicode.GetBytes(contentFilterPhrase.Phrase);
				array2[num] = BitConverter.GetBytes((int)contentFilterPhrase.Influence);
				array3[num] = BitConverter.GetBytes(array[num].Length);
				num2 += array[num].Length + array2[num].Length + array3[num].Length;
				num++;
			}
			byte[] array4 = Util.SerializeByteArrays(num2, new byte[][][]
			{
				array3,
				array,
				array2
			});
			comArguments[6] = BitConverter.GetBytes(array4.Length);
			comArguments[7] = array4;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				lock (this.wrapperLock)
				{
					if (this.wrapper != null)
					{
						this.wrapper.Dispose();
					}
				}
				if (this.antispamUpdateModePollingTimer != null)
				{
					this.antispamUpdateModePollingTimer.Dispose();
					this.antispamUpdateModePollingTimer = null;
				}
				this.disposed = true;
			}
		}

		private void AntispamUpdateServiceMonitor(object state)
		{
			bool flag = this.premiumModeEnabled;
			this.SetPremiumModeEnabled(false);
			bool flag2 = flag != this.premiumModeEnabled;
			if (flag2)
			{
				ExTraceGlobals.InitializationTracer.TraceDebug<string>((long)this.GetHashCode(), "Anti-spam Update mode has been changed and SmartScreen is being re-initialized with {0} mode on.", this.premiumModeEnabled ? "Premium" : "Standard");
				Util.LogUpdateModeChangedReinitializingSmartScreen();
				lock (this.wrapperLock)
				{
					try
					{
						this.wrapper = this.InitializeFilter();
					}
					catch (ExchangeConfigurationException arg)
					{
						string formatString = "SmartScreen could not be re-initialized with {0} mode on and will keep running in the previous mode. Details: {1}";
						ExTraceGlobals.InitializationTracer.TraceError<string, ExchangeConfigurationException>((long)this.GetHashCode(), formatString, this.premiumModeEnabled ? "Premium" : "Standard", arg);
					}
				}
			}
			this.antispamUpdateModePollingTimer.Change(600000, -1);
		}

		private void SetPremiumModeEnabled(bool onStartup)
		{
			try
			{
				AntispamUpdates antispamUpdates = new AntispamUpdates();
				this.premiumModeEnabled = antispamUpdates.IsPremiumSKUInstalled();
			}
			catch (Exception ex)
			{
				if (onStartup)
				{
					throw new ExchangeConfigurationException(ex.Message, ex);
				}
				ExTraceGlobals.InitializationTracer.TraceError<Exception>((long)this.GetHashCode(), "Failed to refresh Anti-spam Update mode - will continue running with current mode. Exception: {0}", ex);
				Util.LogFailedToReadAntispamUpdateMode(ex);
			}
		}

		private const int AntispamUpdateModeMonitorPeriod = 600000;

		private bool disposed;

		private ContentFilterAgentFactory.ContentFilterWrapper wrapper;

		private object wrapperLock = new object();

		private ContentFilterConfig contentFilterConfig;

		private ContentFilterAgentFactory.Dispatcher dispatcher;

		private ADNotificationRequestCookie configRequestCookie;

		private BypassedSenders bypassedSenders;

		private Timer antispamUpdateModePollingTimer;

		private bool premiumModeEnabled;

		private bool isDataCenterEnvironment;

		private sealed class Dispatcher
		{
			public Dispatcher(ContentFilterAgentFactory factory)
			{
				this.factory = factory;
				this.pendingRequests = new ContentFilterAgentFactory.Dispatcher.PendingQueue();
				this.activeRequests = new ContentFilterAgentFactory.Dispatcher.ActiveRequests();
				new Thread(new ThreadStart(this.DispatcherProc))
				{
					Name = "ContentFilterMessageDispatcher",
					IsBackground = true
				}.Start();
			}

			public void Enqueue(ContentFilterAgentFactory.ScanRequest scanRequest)
			{
				this.pendingRequests.Enqueue(scanRequest);
			}

			public void OnRequestFinished(ContentFilterAgentFactory.ScanRequest request)
			{
				this.activeRequests.Remove(request);
			}

			private void DispatcherProc()
			{
				while (!TransportFacades.IsStopping)
				{
					ContentFilterAgentFactory.ScanRequest nextRequest = this.GetNextRequest();
					if (nextRequest != null)
					{
						this.activeRequests.Add(nextRequest);
						this.Dispatch(nextRequest, this.factory.wrapper);
					}
					else
					{
						Thread.Sleep(100);
					}
				}
				IEnumerable<ContentFilterAgentFactory.ScanRequest> enumerable = this.activeRequests.RemoveAll();
				foreach (ContentFilterAgentFactory.ScanRequest scanRequest in enumerable)
				{
					scanRequest.Abort((ScanMessageResult)4294967295U);
				}
				for (;;)
				{
					ContentFilterAgentFactory.ScanRequest scanRequest2 = this.pendingRequests.Dequeue();
					if (scanRequest2 != null)
					{
						scanRequest2.Abort((ScanMessageResult)4294967295U);
					}
					else
					{
						Thread.Sleep(100);
					}
				}
			}

			private ContentFilterAgentFactory.ScanRequest GetNextRequest()
			{
				ContentFilterAgentFactory.ScanRequest scanRequest = null;
				while (scanRequest == null)
				{
					scanRequest = this.pendingRequests.Dequeue();
					if (scanRequest == null)
					{
						break;
					}
					if ((DateTime.UtcNow - scanRequest.CreationTime).Minutes > 10)
					{
						ExTraceGlobals.ScanMessageTracer.TraceError((long)this.GetHashCode(), "Request timed out while sitting in the pending queue.");
						ThreadPool.QueueUserWorkItem(new WaitCallback(scanRequest.Abort), (ScanMessageResult)4294967294U);
						scanRequest = null;
					}
				}
				return scanRequest;
			}

			private void Dispatch(ContentFilterAgentFactory.ScanRequest request, ContentFilterAgentFactory.ContentFilterWrapper wrapper)
			{
				ScanMessageResult scanMessageResult = ScanMessageResult.Error;
				try
				{
					try
					{
						bool flag = request.EnterWrapper(wrapper);
						if (flag)
						{
							scanMessageResult = request.Submit(wrapper);
						}
						else
						{
							scanMessageResult = (ScanMessageResult)4294967294U;
							ExTraceGlobals.ScanMessageTracer.TraceError((long)this.GetHashCode(), "Timed out waiting to enter the COM wrapper. It might have crashed.");
							if (wrapper.TimeoutCount == 1)
							{
								if (Interlocked.CompareExchange(ref wrapper.PingPending, 1, 0) == 0)
								{
									scanMessageResult = request.Ping(wrapper);
									ExTraceGlobals.ScanMessageTracer.TraceError((long)this.GetHashCode(), "Ping request submitted.");
									Util.LogWrapperSendingPingRequest(15);
								}
								else
								{
									ExTraceGlobals.ScanMessageTracer.TraceError((long)this.GetHashCode(), "Ping request not being sent because a previous ping has not returned.");
								}
							}
							else if (wrapper.TimeoutCount == 10)
							{
								IEnumerable<ContentFilterAgentFactory.ScanRequest> enumerable = this.activeRequests.RemoveAll();
								foreach (ContentFilterAgentFactory.ScanRequest scanRequest in enumerable)
								{
									scanRequest.Abort((ScanMessageResult)4294967294U);
								}
								this.RecycleWrapper(wrapper);
							}
							else
							{
								Util.LogWrapperNotResponding();
							}
						}
					}
					catch (COMException ex)
					{
						ExTraceGlobals.ScanMessageTracer.TraceError<COMException>((long)this.GetHashCode(), "Content Filter wrapper appears to be down. Details: {0}", ex);
						Util.LogErrorSubmittingMessage(ex);
						IEnumerable<ContentFilterAgentFactory.ScanRequest> enumerable2 = this.activeRequests.RemoveAll();
						foreach (ContentFilterAgentFactory.ScanRequest scanRequest2 in enumerable2)
						{
							scanRequest2.Abort((ScanMessageResult)4294967294U);
						}
						this.factory.RecreateContentFilterWrapper(wrapper);
					}
				}
				catch (Exception ex2)
				{
					ExTraceGlobals.ScanMessageTracer.TraceError<Exception>((long)this.GetHashCode(), "Unhandled exception in ScanRequest.Submit(): {0}.", ex2);
					request.Exception = ex2;
				}
				finally
				{
					if (scanMessageResult != ScanMessageResult.Pending)
					{
						ExTraceGlobals.ScanMessageTracer.TraceDebug((long)this.GetHashCode(), "Message was not accepted by wrapper.");
						request.ExitWrapper();
						ThreadPool.QueueUserWorkItem(new WaitCallback(request.Abort), scanMessageResult);
					}
				}
			}

			private void RecycleWrapper(ContentFilterAgentFactory.ContentFilterWrapper wrapper)
			{
				ExTraceGlobals.ScanMessageTracer.TraceDebug((long)this.GetHashCode(), "Recycling the wrapper process...");
				Util.LogWrapperBeingRecycled();
				if (wrapper.Recycle())
				{
					ExTraceGlobals.ScanMessageTracer.TraceDebug((long)this.GetHashCode(), "Wrapper process has been recycled.");
					Util.LogWrapperSuccessfullyRecycled();
					this.factory.RecreateContentFilterWrapper(wrapper);
					return;
				}
				ExTraceGlobals.ScanMessageTracer.TraceDebug((long)this.GetHashCode(), "Wrapper recycle timed out.");
				Util.LogWrapperRecycleTimedout();
			}

			internal const int MaximumNumberOfActiveRequests = 15;

			private const int ScanRequestQueueTimeout = 10;

			private const int WrapperRecycleTimeoutThreshold = 10;

			private ContentFilterAgentFactory factory;

			private ContentFilterAgentFactory.Dispatcher.PendingQueue pendingRequests;

			private ContentFilterAgentFactory.Dispatcher.ActiveRequests activeRequests;

			private sealed class PendingQueue
			{
				public void Enqueue(ContentFilterAgentFactory.ScanRequest scanRequest)
				{
					lock (this.list)
					{
						this.list.AddLast(scanRequest);
					}
				}

				public ContentFilterAgentFactory.ScanRequest Dequeue()
				{
					ContentFilterAgentFactory.ScanRequest result = null;
					lock (this.list)
					{
						if (this.list.Count > 0)
						{
							result = this.list.First.Value;
							this.list.RemoveFirst();
						}
					}
					return result;
				}

				private LinkedList<ContentFilterAgentFactory.ScanRequest> list = new LinkedList<ContentFilterAgentFactory.ScanRequest>();
			}

			private sealed class ActiveRequests
			{
				public void Add(ContentFilterAgentFactory.ScanRequest request)
				{
					lock (this.syncRoot)
					{
						this.dictionary.Add(request, null);
					}
				}

				public void Remove(ContentFilterAgentFactory.ScanRequest request)
				{
					lock (this.syncRoot)
					{
						this.dictionary.Remove(request);
					}
				}

				public IEnumerable<ContentFilterAgentFactory.ScanRequest> RemoveAll()
				{
					IEnumerable<ContentFilterAgentFactory.ScanRequest> result = null;
					lock (this.syncRoot)
					{
						result = this.dictionary.Keys;
						this.dictionary = new Dictionary<ContentFilterAgentFactory.ScanRequest, object>();
					}
					return result;
				}

				private object syncRoot = new object();

				private Dictionary<ContentFilterAgentFactory.ScanRequest, object> dictionary = new Dictionary<ContentFilterAgentFactory.ScanRequest, object>();
			}
		}

		private sealed class ScanRequest : IAsyncResult
		{
			public ScanRequest(ContentFilterAgentFactory.Dispatcher dispatcher, AsyncCallback asyncCallback, ContentFilterAgent.AsyncState asyncState)
			{
				this.dispatcher = dispatcher;
				this.asyncCallback = asyncCallback;
				this.asyncState = asyncState;
				this.scanResult = ScanMessageResult.Error;
				this.creationTime = DateTime.UtcNow;
			}

			public ScanMessageResult ScanResult
			{
				get
				{
					return this.scanResult;
				}
			}

			public Exception Exception
			{
				get
				{
					return this.exception;
				}
				set
				{
					this.exception = value;
				}
			}

			public DateTime CreationTime
			{
				get
				{
					return this.creationTime;
				}
			}

			public object AsyncState
			{
				get
				{
					return this.asyncState;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public bool IsCompleted
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public void Abort(ScanMessageResult result)
			{
				this.scanResult = result;
				ExTraceGlobals.ScanMessageTracer.TraceDebug((long)this.GetHashCode(), "Scan request has been aborted.");
				this.Finish();
			}

			public void Abort(object result)
			{
				this.Abort((result is ScanMessageResult) ? ((ScanMessageResult)result) : ScanMessageResult.Error);
			}

			public bool EnterWrapper(ContentFilterAgentFactory.ContentFilterWrapper wrapper)
			{
				bool flag = wrapper.Enter();
				this.wrapper = (flag ? wrapper : null);
				return flag;
			}

			public void ExitWrapper()
			{
				ContentFilterAgentFactory.ContentFilterWrapper contentFilterWrapper = Interlocked.Exchange<ContentFilterAgentFactory.ContentFilterWrapper>(ref this.wrapper, null);
				if (contentFilterWrapper != null)
				{
					if (Interlocked.CompareExchange(ref contentFilterWrapper.PingPending, 0, 1) == 1)
					{
						ExTraceGlobals.ScanMessageTracer.TraceError((long)this.GetHashCode(), "Ping request returned.");
						return;
					}
					contentFilterWrapper.Exit();
				}
			}

			public ScanMessageResult Ping(ContentFilterAgentFactory.ContentFilterWrapper wrapper)
			{
				this.wrapper = wrapper;
				return this.Submit(wrapper);
			}

			public ScanMessageResult Submit(ContentFilterAgentFactory.ContentFilterWrapper wrapper)
			{
				try
				{
					Util.InvokeExLapi(wrapper.ComProxy, new ComProxy.AsyncCompletionCallback(this.ScanCompletedComCallback), this.asyncState.ComArguments, this.asyncState.EndOfDataEventArgs.MailItem, Constants.RequestTypes.ScanMessage);
					return (ScanMessageResult)this.asyncState.ComArguments.GetInt32(2);
				}
				catch (ArgumentException arg)
				{
					ExTraceGlobals.ScanMessageTracer.TraceError<ArgumentException>((long)this.GetHashCode(), "Error when reading result from ExLapi property bag: {0}.", arg);
				}
				catch (InvalidOperationException arg2)
				{
					ExTraceGlobals.ScanMessageTracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "Failed to open Mimestream. Error: {0}", arg2);
				}
				catch (ExchangeDataException arg3)
				{
					ExTraceGlobals.ScanMessageTracer.TraceError<ExchangeDataException>((long)this.GetHashCode(), "Failed to open Mimestream. Error: {0}", arg3);
				}
				catch (IOException ex)
				{
					int hrforException = Marshal.GetHRForException(ex);
					if (hrforException != -2147024784)
					{
						throw;
					}
					ExTraceGlobals.ScanMessageTracer.TraceError<IOException>((long)this.GetHashCode(), "Error when reading Mimestream. Error: {0}", ex);
				}
				return ScanMessageResult.Error;
			}

			private void Finish()
			{
				if (Interlocked.Exchange(ref this.finished, 1) == 0)
				{
					this.dispatcher.OnRequestFinished(this);
					if (this.asyncCallback != null)
					{
						ExTraceGlobals.ScanMessageTracer.TraceDebug((long)this.GetHashCode(), "Invoking the agent callback.");
						this.asyncCallback(this);
					}
				}
			}

			private void ScanCompletedComCallback(ComArguments arguments)
			{
				this.ExitWrapper();
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnScanMessageCompleted));
			}

			private void OnScanMessageCompleted(object state)
			{
				ExTraceGlobals.ScanMessageTracer.TraceDebug((long)this.GetHashCode(), "Filter has completed scanning message.");
				try
				{
					this.scanResult = (ScanMessageResult)this.asyncState.ComArguments.GetInt32(3);
				}
				catch (ArgumentException arg)
				{
					ExTraceGlobals.ScanMessageTracer.TraceError<ArgumentException>((long)this.GetHashCode(), "Error when reading result from ExLapi property bag: {0}.", arg);
					this.scanResult = ScanMessageResult.Error;
				}
				catch (Exception arg2)
				{
					ExTraceGlobals.ScanMessageTracer.TraceError<Exception>((long)this.GetHashCode(), "Unhandled exception in ScanRequest.OnScanMessageCompleted: {0}.", arg2);
					this.exception = arg2;
					this.scanResult = ScanMessageResult.Error;
				}
				this.Finish();
			}

			private readonly ContentFilterAgentFactory.Dispatcher dispatcher;

			private readonly AsyncCallback asyncCallback;

			private readonly ContentFilterAgent.AsyncState asyncState;

			private readonly DateTime creationTime;

			private ContentFilterAgentFactory.ContentFilterWrapper wrapper;

			private ScanMessageResult scanResult;

			private Exception exception;

			private int finished;
		}

		private sealed class ContentFilterWrapper : IDisposable
		{
			public ContentFilterWrapper(ComProxy comProxy)
			{
				if (comProxy == null)
				{
					throw new ArgumentNullException("comProxy");
				}
				this.semaphore = new Semaphore(15, 15);
				this.recycledEvent = new AutoResetEvent(false);
				this.comProxy = comProxy;
			}

			~ContentFilterWrapper()
			{
				this.Dispose(false);
			}

			public ComProxy ComProxy
			{
				get
				{
					return this.comProxy;
				}
			}

			public int TimeoutCount
			{
				get
				{
					return this.timeoutCount;
				}
			}

			public bool Enter()
			{
				bool flag = this.semaphore.WaitOne(30000, false);
				if (flag)
				{
					this.timeoutCount = 0;
				}
				else
				{
					this.timeoutCount++;
				}
				return flag;
			}

			public void Exit()
			{
				this.semaphore.Release();
			}

			public bool Recycle()
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.ShutdownProc));
				return this.recycledEvent.WaitOne(15000, false);
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (!this.disposed && disposing)
				{
					this.comProxy.Dispose();
					this.semaphore.Close();
					this.recycledEvent.Close();
					this.disposed = true;
				}
			}

			private void ShutdownProc(object state)
			{
				try
				{
					Util.InvokeExLapi(this.comProxy, null, new ComArguments(), null, Constants.RequestTypes.Shutdown);
				}
				catch (COMException ex)
				{
					if (ex.ErrorCode == -2147023170)
					{
						this.recycledEvent.Set();
					}
					else
					{
						ExTraceGlobals.ScanMessageTracer.TraceDebug<Exception>((long)this.GetHashCode(), "Error sending shutdown message to the wrapper. Details: {0}", ex);
						Util.LogWrapperRecycleError(ex);
					}
				}
				catch (Exception ex2)
				{
					ExTraceGlobals.ScanMessageTracer.TraceDebug<Exception>((long)this.GetHashCode(), "Error sending shutdown message to the wrapper. Details: {0}", ex2);
					Util.LogWrapperRecycleError(ex2);
				}
			}

			private const int ScanMessageTimeout = 30000;

			private const int ShutdownTimeout = 15000;

			public int PingPending;

			private readonly ComProxy comProxy;

			private readonly Semaphore semaphore;

			private readonly AutoResetEvent recycledEvent;

			private int timeoutCount;

			private bool disposed;
		}
	}
}
