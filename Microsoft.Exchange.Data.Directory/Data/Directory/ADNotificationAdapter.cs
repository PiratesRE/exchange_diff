using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ADNotificationAdapter
	{
		public static ADNotificationRequestCookie RegisterChangeNotification<T>(ADObjectId baseDN, ADNotificationCallback callback) where T : ADConfigurationObject, new()
		{
			return ADNotificationAdapter.RegisterChangeNotification<T>(baseDN, callback, null, 10);
		}

		public static ADNotificationRequestCookie RegisterChangeNotification<T>(ADObjectId baseDN, ADNotificationCallback callback, object context, int retryCount) where T : ADConfigurationObject, new()
		{
			object wrappedContext;
			ADNotificationAdapter.CreateWrappedContextForRegisterChangeNotification(ref baseDN, callback, context, out wrappedContext);
			return ADNotificationAdapter.RegisterChangeNotification<T>(baseDN, wrappedContext, true, retryCount);
		}

		public static ADNotificationRequestCookie RegisterChangeNotification<T>(ADObjectId baseDN, object wrappedContext, bool wrapAsADOperation, int retryCount) where T : ADConfigurationObject, new()
		{
			ADNotificationRequestCookie cookie = null;
			if (wrapAsADOperation)
			{
				ADNotificationAdapter.RunADOperation(delegate()
				{
					cookie = ADNotificationAdapter.RegisterChangeNotification<T>(baseDN, new ADNotificationCallback(ADNotificationAdapter.OnNotification), wrappedContext);
				}, retryCount);
			}
			else
			{
				cookie = ADNotificationAdapter.RegisterChangeNotification<T>(baseDN, new ADNotificationCallback(ADNotificationAdapter.OnNotification), wrappedContext);
			}
			return cookie;
		}

		public static ADNotificationRequestCookie RegisterChangeNotification<T>(ADObjectId baseDN, ADNotificationCallback callback, object context) where T : ADConfigurationObject, new()
		{
			return ADNotificationAdapter.RegisterChangeNotification<T>(Activator.CreateInstance<T>(), baseDN, callback, context);
		}

		private static ADNotificationRequestCookie RegisterChangeNotification<T>(T dummyObject, ADObjectId baseDN, ADNotificationCallback callback, object context) where T : ADConfigurationObject, new()
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			if (baseDN == null || string.IsNullOrEmpty(baseDN.DistinguishedName))
			{
				throw new ArgumentNullException("baseDN");
			}
			string forestFQDN = baseDN.GetPartitionId().ForestFQDN;
			if (!baseDN.IsDescendantOf(ADSession.GetConfigurationNamingContext(forestFQDN)) && !ADSession.IsTenantIdentity(baseDN, forestFQDN))
			{
				throw new ArgumentException(DirectoryStrings.ExArgumentException("baseDN", baseDN), "baseDN");
			}
			ADNotificationRequest adnotificationRequest = new ADNotificationRequest(typeof(T), dummyObject.MostDerivedObjectClass, baseDN, callback, context);
			ADNotificationListener.RegisterChangeNotification(adnotificationRequest);
			return new ADNotificationRequestCookie(new ADNotificationRequest[]
			{
				adnotificationRequest
			});
		}

		public static ADOperationResult TryRegisterChangeNotification<T>(ADObjectId baseDN, ADNotificationCallback callback, int retryCount, out ADNotificationRequestCookie cookie) where T : ADConfigurationObject, new()
		{
			return ADNotificationAdapter.TryRegisterChangeNotification<T>(() => baseDN, callback, null, retryCount, out cookie);
		}

		public static ADOperationResult TryRegisterChangeNotification<T>(Func<ADObjectId> baseDNGetter, ADNotificationCallback callback, object context, int retryCount, out ADNotificationRequestCookie cookie) where T : ADConfigurationObject, new()
		{
			cookie = null;
			ADNotificationRequestCookie returnedCookie = cookie;
			ADOperationResult result = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADObjectId baseDN = (baseDNGetter == null) ? null : baseDNGetter();
				object wrappedContext;
				ADNotificationAdapter.CreateWrappedContextForRegisterChangeNotification(ref baseDN, callback, context, out wrappedContext);
				returnedCookie = ADNotificationAdapter.RegisterChangeNotification<T>(baseDN, wrappedContext, false, 0);
			}, retryCount);
			cookie = returnedCookie;
			return result;
		}

		public static void UnregisterChangeNotification(ADNotificationRequestCookie request)
		{
			ADNotificationAdapter.UnregisterChangeNotification(request, false);
		}

		public static void UnregisterChangeNotification(ADNotificationRequestCookie requestCookie, bool block)
		{
			if (requestCookie == null)
			{
				throw new ArgumentNullException("requestCookie");
			}
			foreach (ADNotificationRequest request in requestCookie.Requests)
			{
				ADNotificationListener.UnRegisterChangeNotification(request, block);
			}
		}

		public static ADNotificationRequestCookie RegisterExchangeTopologyChangeNotification(ADNotificationCallback callback, object context)
		{
			return ADNotificationAdapter.RegisterExchangeTopologyChangeNotification(callback, context, ExchangeTopologyScope.ADAndExchangeServerAndSiteTopology);
		}

		public static ADNotificationRequestCookie RegisterExchangeTopologyChangeNotification(ADNotificationCallback callback, object context, ExchangeTopologyScope topologyType)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 387, "RegisterExchangeTopologyChangeNotification", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADNotificationAdapter.cs");
			ADObjectId childId = topologyConfigurationSession.ConfigurationNamingContext.GetChildId("CN", "Sites");
			ADObjectId childId2 = childId.GetChildId("CN", "Inter-Site Transports").GetChildId("CN", "IP");
			ADObjectId orgContainerId = topologyConfigurationSession.GetOrgContainerId();
			ADNotificationRequest[] requests;
			switch (topologyType)
			{
			case ExchangeTopologyScope.Complete:
				requests = new ADNotificationRequest[]
				{
					ADNotificationAdapter.RegisterChangeNotification<ADServer>(childId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADSite>(childId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADSiteLink>(childId2, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADSubnet>(childId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADVirtualDirectory>(orgContainerId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<Server>(orgContainerId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ReceiveConnector>(orgContainerId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADEmailTransport>(orgContainerId, callback, context).Requests[0]
				};
				break;
			case ExchangeTopologyScope.ServerAndSiteTopology:
				requests = new ADNotificationRequest[]
				{
					ADNotificationAdapter.RegisterChangeNotification<ADSite>(childId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADSiteLink>(childId2, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<Server>(orgContainerId, callback, context).Requests[0]
				};
				break;
			case ExchangeTopologyScope.ADAndExchangeServerAndSiteTopology:
				requests = new ADNotificationRequest[]
				{
					ADNotificationAdapter.RegisterChangeNotification<ADServer>(childId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADSite>(childId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADSiteLink>(childId2, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<Server>(orgContainerId, callback, context).Requests[0]
				};
				break;
			case ExchangeTopologyScope.ADAndExchangeServerAndSiteAndVirtualDirectoryTopology:
				requests = new ADNotificationRequest[]
				{
					ADNotificationAdapter.RegisterChangeNotification<ADServer>(childId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADSite>(childId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADSiteLink>(childId2, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<Server>(orgContainerId, callback, context).Requests[0],
					ADNotificationAdapter.RegisterChangeNotification<ADVirtualDirectory>(orgContainerId, callback, context).Requests[0]
				};
				break;
			default:
				throw new ArgumentException("topologyType", "topologyType");
			}
			return new ADNotificationRequestCookie(requests);
		}

		public static T ReadConfiguration<T>(ADConfigurationReader<T> configurationReader)
		{
			return ADNotificationAdapter.ReadConfiguration<T>(configurationReader, 10);
		}

		public static T ReadConfiguration<T>(ADConfigurationReader<T> configurationReader, int retryCount)
		{
			if (configurationReader == null)
			{
				throw new ArgumentNullException("configurationReader");
			}
			if (retryCount < 0)
			{
				throw new ArgumentOutOfRangeException("retryCount", "Number of retries must be equal to or larger than zero.");
			}
			T result = default(T);
			ADNotificationAdapter.RunADOperation(delegate()
			{
				result = configurationReader();
			}, retryCount);
			return result;
		}

		public static bool TryReadConfiguration<T>(ADConfigurationReader<T> configurationReader, out T result)
		{
			ADOperationResult adoperationResult;
			return ADNotificationAdapter.TryReadConfiguration<T>(configurationReader, out result, 10, out adoperationResult);
		}

		public static bool TryReadConfiguration<T>(ADConfigurationReader<T> configurationReader, out T result, out ADOperationResult operationStatus)
		{
			return ADNotificationAdapter.TryReadConfiguration<T>(configurationReader, out result, 10, out operationStatus);
		}

		public static bool TryReadConfiguration<T>(ADConfigurationReader<T> configurationReader, out T result, int retryCount, out ADOperationResult operationStatus)
		{
			if (configurationReader == null)
			{
				throw new ArgumentNullException("configurationReader");
			}
			if (retryCount < 0)
			{
				throw new ArgumentOutOfRangeException("retryCount", "Number of retries must be equal to or larger than zero.");
			}
			result = default(T);
			T objectReturned = result;
			operationStatus = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				objectReturned = configurationReader();
			}, retryCount);
			result = objectReturned;
			if (operationStatus.Succeeded)
			{
				return result != null;
			}
			if (operationStatus.Exception is ComputerNameNotCurrentlyAvailableException)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_FIND_LOCAL_SERVER_FAILED, Environment.MachineName, new object[]
				{
					operationStatus.Exception.Message,
					Environment.MachineName
				});
			}
			return false;
		}

		public static void ReadConfigurationPaged<T>(ADConfigurationReader<ADPagedReader<T>> configurationReader, ADConfigurationProcessor<T> configurationProcessor) where T : IConfigurable, new()
		{
			ADNotificationAdapter.ReadConfigurationPaged<T>(configurationReader, configurationProcessor, 10);
		}

		public static void ReadConfigurationPaged<T>(ADConfigurationReader<ADPagedReader<T>> configurationReader, ADConfigurationProcessor<T> configurationProcessor, int retryCount) where T : IConfigurable, new()
		{
			if (configurationProcessor == null)
			{
				throw new ArgumentNullException("configurationProcessor");
			}
			ADPagedReader<T> pagedReader = ADNotificationAdapter.ReadConfiguration<ADPagedReader<T>>(configurationReader, retryCount);
			IEnumerator<T> enumerator = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				enumerator = pagedReader.GetEnumerator();
			}, retryCount);
			Breadcrumbs<Exception> exceptions = new Breadcrumbs<Exception>(32);
			try
			{
				for (;;)
				{
					bool hasMore = false;
					ADNotificationAdapter.RunADOperation(delegate()
					{
						try
						{
							hasMore = enumerator.MoveNext();
						}
						catch (Exception bc)
						{
							exceptions.Drop(bc);
							enumerator.Dispose();
							enumerator = pagedReader.GetEnumerator();
							throw;
						}
					}, retryCount);
					if (!hasMore)
					{
						break;
					}
					configurationProcessor(enumerator.Current);
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		public static bool TryReadConfigurationPaged<T>(ADConfigurationReader<ADPagedReader<T>> configurationReader, ADConfigurationProcessor<T> configurationProcessor) where T : IConfigurable, new()
		{
			ADOperationResult adoperationResult;
			return ADNotificationAdapter.TryReadConfigurationPaged<T>(configurationReader, configurationProcessor, 10, out adoperationResult);
		}

		public static bool TryReadConfigurationPaged<T>(ADConfigurationReader<ADPagedReader<T>> configurationReader, ADConfigurationProcessor<T> configurationProcessor, out ADOperationResult operationStatus) where T : IConfigurable, new()
		{
			return ADNotificationAdapter.TryReadConfigurationPaged<T>(configurationReader, configurationProcessor, 10, out operationStatus);
		}

		public static bool TryReadConfigurationPaged<T>(ADConfigurationReader<ADPagedReader<T>> configurationReader, ADConfigurationProcessor<T> configurationProcessor, int retryCount, out ADOperationResult operationStatus) where T : IConfigurable, new()
		{
			if (configurationProcessor == null)
			{
				throw new ArgumentNullException("configurationProcessor");
			}
			ADPagedReader<T> pagedReader;
			if (!ADNotificationAdapter.TryReadConfiguration<ADPagedReader<T>>(configurationReader, out pagedReader, retryCount, out operationStatus))
			{
				return false;
			}
			IEnumerator<T> enumerator = null;
			operationStatus = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				enumerator = pagedReader.GetEnumerator();
			}, retryCount);
			if (!operationStatus.Succeeded)
			{
				return false;
			}
			Breadcrumbs<Exception> exceptions = new Breadcrumbs<Exception>(32);
			try
			{
				for (;;)
				{
					bool hasMore = false;
					operationStatus = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						try
						{
							hasMore = enumerator.MoveNext();
						}
						catch (Exception bc)
						{
							exceptions.Drop(bc);
							enumerator.Dispose();
							enumerator = pagedReader.GetEnumerator();
							throw;
						}
					}, retryCount);
					if (!operationStatus.Succeeded)
					{
						break;
					}
					if (!hasMore)
					{
						goto IL_AB;
					}
					configurationProcessor(enumerator.Current);
				}
				return false;
				IL_AB:;
			}
			finally
			{
				enumerator.Dispose();
			}
			return true;
		}

		public static void RunADOperation(ADOperation adOperation)
		{
			ADNotificationAdapter.RunADOperation(adOperation, 10);
		}

		public static void RunADOperation(ADOperation adOperation, int retryCount)
		{
			if (adOperation == null)
			{
				throw new ArgumentNullException("adOperation");
			}
			if (retryCount < 0)
			{
				throw new ArgumentOutOfRangeException("retryCount", "Number of retries must be equal to or larger than zero.");
			}
			for (int i = 0; i <= retryCount; i++)
			{
				try
				{
					adOperation();
					break;
				}
				catch (ADInvalidCredentialException ex)
				{
					if (i == retryCount)
					{
						throw new ADTransientException(ex.LocalizedString, ex);
					}
				}
				catch (TransientException)
				{
					if (i == retryCount)
					{
						throw;
					}
				}
				Thread.Sleep(i * 1000);
			}
		}

		public static ADOperationResult TryRunADOperation(ADOperation adOperation)
		{
			return ADNotificationAdapter.TryRunADOperation(adOperation, 10);
		}

		public static ADOperationResult TryRunADOperation(ADOperation adOperation, int retryCount)
		{
			try
			{
				ADNotificationAdapter.RunADOperation(adOperation, retryCount);
			}
			catch (TransientException ex)
			{
				ExTraceGlobals.ADNotificationsTracer.TraceDebug<TransientException>(0L, "AD operation failed with exception: {0}", ex);
				return new ADOperationResult(ADOperationErrorCode.RetryableError, ex);
			}
			catch (DataSourceOperationException ex2)
			{
				ExTraceGlobals.ADNotificationsTracer.TraceDebug<DataSourceOperationException>(0L, "AD operation failed with exception: {0}", ex2);
				return new ADOperationResult(ADOperationErrorCode.PermanentError, ex2);
			}
			catch (DataValidationException ex3)
			{
				ExTraceGlobals.ADNotificationsTracer.TraceDebug<DataValidationException>(0L, "AD operation failed with exception: {0}", ex3);
				return new ADOperationResult(ADOperationErrorCode.PermanentError, ex3);
			}
			return ADOperationResult.Success;
		}

		private static void OnNotification(ADNotificationEventArgs args)
		{
			if (args != null)
			{
				ADNotificationAdapter.WrappedContext wrappedContext = args.Context as ADNotificationAdapter.WrappedContext;
				if (wrappedContext != null && wrappedContext.Callback != null)
				{
					wrappedContext.Callback.SerializedRun(args);
				}
			}
		}

		private static object CreateWrappingContext(ADNotificationCallback callback, object context)
		{
			ADNotificationAdapter.WrappedADNotificationCallback callback2 = new ADNotificationAdapter.WrappedADNotificationCallback(callback);
			return new ADNotificationAdapter.WrappedContext(callback2, context);
		}

		private static void CreateWrappedContextForRegisterChangeNotification(ref ADObjectId baseDN, ADNotificationCallback callback, object context, out object wrappedContext)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			if (baseDN == null)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1089, "CreateWrappedContextForRegisterChangeNotification", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADNotificationAdapter.cs");
				baseDN = topologyConfigurationSession.GetOrgContainerId();
			}
			ExTraceGlobals.ADNotificationsTracer.TraceDebug<ADObjectId, string, string>(0L, "new change notification registration for {0} from {1}.{2}", baseDN, callback.Method.DeclaringType.FullName, callback.Method.Name);
			wrappedContext = ADNotificationAdapter.CreateWrappingContext(callback, context);
		}

		private const int DefaultReadRetryCount = 10;

		private const int RetryInterval = 1000;

		private class WrappedContext
		{
			public WrappedContext(ADNotificationAdapter.WrappedADNotificationCallback callback, object context)
			{
				this.Callback = callback;
				this.Context = context;
			}

			public readonly ADNotificationAdapter.WrappedADNotificationCallback Callback;

			public readonly object Context;
		}

		private class WrappedADNotificationCallback
		{
			public WrappedADNotificationCallback(ADNotificationCallback callback)
			{
				this.callback = callback;
				this.waitHandle = new AutoResetEvent(true);
				this.hasQueuedRun = false;
			}

			public void SerializedRun(ADNotificationEventArgs args)
			{
				bool flag = false;
				lock (this)
				{
					if (!this.hasQueuedRun)
					{
						this.hasQueuedRun = true;
						flag = true;
					}
				}
				if (flag)
				{
					bool timedOut = !this.waitHandle.WaitOne(600000, false);
					this.RunCallback(args, timedOut);
				}
			}

			private void RunCallback(ADNotificationEventArgs args, bool timedOut)
			{
				if (!timedOut)
				{
					lock (this)
					{
						this.hasQueuedRun = false;
					}
					try
					{
						try
						{
							this.callback(args);
						}
						catch (ADInvalidCredentialException)
						{
							ExTraceGlobals.ADNotificationsTracer.TraceError(0L, "Some component's AD Notification callback didn't catch ADInvalidCredentialException.");
						}
						catch (ADTransientException)
						{
							ExTraceGlobals.ADNotificationsTracer.TraceError(0L, "Some component's AD Notification callback didn't catch ADTransientException.");
						}
						catch (DataValidationException)
						{
							ExTraceGlobals.ADNotificationsTracer.TraceError(0L, "Some component's AD Notification callback didn't catch DataValidationException.");
						}
						return;
					}
					finally
					{
						this.waitHandle.Set();
					}
				}
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_AD_NOTIFICATION_CALLBACK_TIMED_OUT, args.Type.Name, new object[]
				{
					args.Type.Name
				});
				ExTraceGlobals.ADNotificationsTracer.TraceError(0L, "Some component's AD Notification callback hasn't returned after 10 minutes. config. change notifications ARE NO LONGER BEING DELIVERED TO SUCH COMPONENT.");
			}

			private const int ClientCallbackTimeout = 600000;

			private ADNotificationCallback callback;

			private AutoResetEvent waitHandle;

			private bool hasQueuedRun;
		}
	}
}
