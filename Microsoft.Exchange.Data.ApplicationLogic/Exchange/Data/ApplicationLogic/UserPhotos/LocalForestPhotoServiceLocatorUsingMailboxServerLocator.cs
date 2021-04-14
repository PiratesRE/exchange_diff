using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ServerLocator;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class LocalForestPhotoServiceLocatorUsingMailboxServerLocator : IPhotoServiceLocator
	{
		public LocalForestPhotoServiceLocatorUsingMailboxServerLocator(IPerformanceDataLogger perfLogger, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("perfLogger", perfLogger);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.perfLogger = perfLogger;
			this.tracer = upstreamTracer;
		}

		public Uri Locate(ADUser target)
		{
			ArgumentValidator.ThrowIfNull("target", target);
			ArgumentValidator.ThrowIfNull("target.Database", target.Database);
			if (this.lastLocatedTarget != null && this.lastLocatedTarget.Equals(target))
			{
				return this.lastLocatedUri;
			}
			Uri result;
			try
			{
				using (MailboxServerLocator mailboxServerLocator = MailboxServerLocator.CreateWithResourceForestFqdn(target.Database.ObjectGuid, new Fqdn(TopologyProvider.LocalForestFqdn)))
				{
					Uri photoServiceUri = this.GetPhotoServiceUri(this.LocateServer(mailboxServerLocator));
					this.lastLocatedUri = photoServiceUri;
					this.lastLocatedTarget = target;
					result = photoServiceUri;
				}
			}
			catch (BackEndLocatorException arg)
			{
				this.tracer.TraceError<BackEndLocatorException>((long)this.GetHashCode(), "SERVICE LOCATOR[MAILBOXSERVERLOCATOR]: failed to locate service because MailboxServerLocator failed.  Exception: {0}", arg);
				throw;
			}
			catch (ServerLocatorClientTransientException arg2)
			{
				this.tracer.TraceError<ServerLocatorClientTransientException>((long)this.GetHashCode(), "SERVICE LOCATOR[MAILBOXSERVERLOCATOR]: hit a transient error in ServerLocator trying to locate photo service.  Exception: {0}", arg2);
				throw;
			}
			catch (ServiceDiscoveryTransientException arg3)
			{
				this.tracer.TraceError<ServiceDiscoveryTransientException>((long)this.GetHashCode(), "SERVICE LOCATOR[MAILBOXSERVERLOCATOR]: hit a transient error in service discovery trying to locate photo service.  Exception: {0}", arg3);
				throw;
			}
			catch (TransientException arg4)
			{
				this.tracer.TraceError<TransientException>((long)this.GetHashCode(), "SERVICE LOCATOR[MAILBOXSERVERLOCATOR]: hit a transient error trying to locate photo service.  Exception: {0}", arg4);
				throw;
			}
			catch (Exception arg5)
			{
				this.tracer.TraceError<Exception>((long)this.GetHashCode(), "SERVICE LOCATOR[MAILBOXSERVERLOCATOR]: failed to locate photo service.  Exception: {0}", arg5);
				throw;
			}
			return result;
		}

		public bool IsServiceOnThisServer(Uri service)
		{
			ArgumentValidator.ThrowIfNull("service", service);
			return LocalServerCache.LocalServerFqdn.Equals(service.Host, StringComparison.OrdinalIgnoreCase);
		}

		private BackEndServer LocateServer(MailboxServerLocator locator)
		{
			BackEndServer result;
			using (new StopwatchPerformanceTracker("LocalForestPhotoServiceLocatorLocateServer", this.perfLogger))
			{
				using (new ADPerformanceTracker("LocalForestPhotoServiceLocatorLocateServer", this.perfLogger))
				{
					IAsyncResult asyncResult = locator.BeginGetServer(null, null);
					if (!asyncResult.AsyncWaitHandle.WaitOne(LocalForestPhotoServiceLocatorUsingMailboxServerLocator.LocateServerTimeout))
					{
						this.tracer.TraceError((long)this.GetHashCode(), "SERVICE LOCATOR[MAILBOXSERVERLOCATOR]: timed out waiting for a response from locator.");
						throw new TimeoutException();
					}
					result = locator.EndGetServer(asyncResult);
				}
			}
			return result;
		}

		private Uri GetPhotoServiceUri(BackEndServer server)
		{
			Uri backEndWebServicesUrl;
			using (new StopwatchPerformanceTracker("LocalForestPhotoServiceLocatorGetPhotoServiceUri", this.perfLogger))
			{
				using (new ADPerformanceTracker("LocalForestPhotoServiceLocatorGetPhotoServiceUri", this.perfLogger))
				{
					backEndWebServicesUrl = BackEndLocator.GetBackEndWebServicesUrl(server);
				}
			}
			return backEndWebServicesUrl;
		}

		private static readonly TimeSpan LocateServerTimeout = TimeSpan.FromSeconds(10.0);

		private readonly IPerformanceDataLogger perfLogger;

		private readonly ITracer tracer;

		private ADUser lastLocatedTarget;

		private Uri lastLocatedUri;
	}
}
