using System;
using System.Diagnostics;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class VersionedServiceBase : DisposeTrackableBase, IVersionedService, IDisposeTrackable, IDisposable
	{
		protected VersionedServiceBase(ILogger logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			this.Logger = logger;
		}

		public VersionInformation ClientVersion { get; private set; }

		private protected ILogger Logger { protected get; private set; }

		protected abstract VersionInformation ServiceVersion { get; }

		public void ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			this.ClientVersion = clientVersion;
			serverVersion = this.ServiceVersion;
		}

		protected TResult ForwardExceptions<TResult>(Func<TResult> operation)
		{
			this.Logger.Log(MigrationEventType.Instrumentation, "BEGIN Processing service call: {0}", new object[]
			{
				operation.Method
			});
			Stopwatch stopwatch = Stopwatch.StartNew();
			Exception failure = null;
			TResult result = default(TResult);
			try
			{
				CommonUtils.FailureDelegate processFailure = delegate(Exception exception)
				{
					failure = exception;
					return true;
				};
				CommonUtils.ProcessKnownExceptionsWithoutTracing(delegate
				{
					result = operation();
				}, processFailure);
			}
			catch (Exception ex)
			{
				if (failure == null)
				{
					failure = ex;
				}
				else
				{
					this.Logger.Log(MigrationEventType.Warning, "Re-throwing exception while processing service call {0} since another failure was already recorded: {1}", new object[]
					{
						operation.Method,
						ex
					});
				}
				throw;
			}
			finally
			{
				stopwatch.Stop();
				this.Logger.Log(MigrationEventType.Instrumentation, "END Processing service call: {0}. Duration: {1} ms. Exception: {2}", new object[]
				{
					operation.Method,
					stopwatch.ElapsedMilliseconds,
					failure
				});
			}
			if (failure != null)
			{
				this.Logger.LogError(failure, "Service call {0} failed.", new object[]
				{
					operation.Method
				});
				LoadBalanceFault.Throw(failure);
			}
			return result;
		}

		protected void ForwardExceptions(Action operation)
		{
			this.Logger.Log(MigrationEventType.Instrumentation, "BEGIN Processing service call: {0}", new object[]
			{
				operation.Method
			});
			Stopwatch stopwatch = Stopwatch.StartNew();
			Exception failure = null;
			try
			{
				CommonUtils.CatchKnownExceptions(operation, delegate(Exception exception)
				{
					failure = exception;
				});
			}
			catch (Exception ex)
			{
				if (failure == null)
				{
					failure = ex;
				}
				else
				{
					this.Logger.Log(MigrationEventType.Warning, "Re-throwing exception while processing service call {0} since another failure was already recorded: {1}", new object[]
					{
						operation.Method,
						ex
					});
				}
				throw;
			}
			finally
			{
				stopwatch.Stop();
				this.Logger.Log(MigrationEventType.Instrumentation, "END Processing service call: {0}. Duration: {1} ms. Exception: {2}", new object[]
				{
					operation.Method,
					stopwatch.ElapsedMilliseconds,
					failure
				});
			}
			if (failure != null)
			{
				this.Logger.LogError(failure, "Service call {0} failed.", new object[]
				{
					operation.Method
				});
				LoadBalanceFault.Throw(failure);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<VersionedServiceBase>(this);
		}
	}
}
