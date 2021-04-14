using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.SharedCache.Client;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.SharedCache
{
	public sealed class SharedCacheProbe : ProbeWorkItem
	{
		private static void PopulateDefinition(ProbeDefinition probe, string assemblyPath, string typeName, string name, string serviceName, string targetResource, int recurrenceIntervalSeconds, int rpcTimeout)
		{
			probe.AssemblyPath = assemblyPath;
			probe.TypeName = typeName;
			probe.Name = name;
			probe.ServiceName = serviceName;
			probe.TargetResource = targetResource;
			probe.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probe.Attributes[SharedCacheProbe.RpcTimeoutKey] = rpcTimeout.ToString();
		}

		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probe = definition as ProbeDefinition;
			SharedCacheProbe.PopulateDefinition(probe, SharedCacheProbe.AssemblyPath, SharedCacheProbe.ProbeTypeName, propertyBag[SharedCacheProbe.NamePropertyKey], propertyBag[SharedCacheProbe.ServiceNamePropertyKey], propertyBag[SharedCacheProbe.TargetResourcePropertyKey], 0, int.Parse(propertyBag[SharedCacheProbe.RpcTimeoutKey]));
		}

		public static ProbeDefinition CreateDefinition(ProbeIdentity probeIdentity, int rpcTimeout)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			SharedCacheProbe.PopulateDefinition(probeDefinition, SharedCacheProbe.AssemblyPath, SharedCacheProbe.ProbeTypeName, probeIdentity.Name, probeIdentity.Component.Name, probeIdentity.TargetResource, 60, rpcTimeout);
			return probeDefinition;
		}

		private int RpcTimeout
		{
			get
			{
				return int.Parse(base.Definition.Attributes[SharedCacheProbe.RpcTimeoutKey]);
			}
		}

		private SharedCacheClient CreateCacheClient(Guid cacheGuid, Breadcrumbs breadcrumbs)
		{
			breadcrumbs.Drop("Creating cache client.");
			return new SharedCacheClient(cacheGuid, "SharedCacheProbe", this.RpcTimeout, true);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			Breadcrumbs breadcrumbs = new Breadcrumbs(1024, base.TraceContext);
			try
			{
				SharedCacheClient client = this.CreateCacheClient(Guid.Parse(base.Definition.TargetResource), breadcrumbs);
				string original = Guid.NewGuid().ToString();
				byte[] value = null;
				string diag = null;
				breadcrumbs.Drop("Inserting value '{0}' into cache.", new object[]
				{
					original
				});
				SharedCacheProbe.ExecuteAndThrowTimeouts(delegate
				{
					client.TryInsert(SharedCacheProbe.CacheKey, Encoding.UTF8.GetBytes(original), DateTime.UtcNow, out diag);
				}, breadcrumbs, this.RpcTimeout);
				breadcrumbs.Drop(diag ?? "<null diagnostics>");
				breadcrumbs.Drop("Attempting to get the value.");
				bool success = false;
				SharedCacheProbe.ExecuteAndThrowTimeouts(delegate
				{
					success = client.TryGet(SharedCacheProbe.CacheKey, out value, out diag);
				}, breadcrumbs, this.RpcTimeout);
				breadcrumbs.Drop(diag ?? "<null diagnostics>");
				if (!success)
				{
					throw new ApplicationException("Value was not found in the shared cache but it should be present.");
				}
				string @string = Encoding.UTF8.GetString(value);
				if (!(@string == original))
				{
					throw new ApplicationException(string.Format("Value returned from cache '{0}' does not match the originally-inserted value '{1}'", @string, original));
				}
				breadcrumbs.Drop("Found out original value '{0}'", new object[]
				{
					@string
				});
			}
			catch (Exception ex)
			{
				breadcrumbs.Drop("Throwing an unhandled " + ex.GetType().Name + ": " + ex.Message);
				throw;
			}
			finally
			{
				breadcrumbs.Drop("Probe complete");
				base.Result.StateAttribute1 = breadcrumbs.ToString();
			}
		}

		private static void ExecuteAndThrowTimeouts(Action action, Breadcrumbs breadcrumbs, int timeoutMilliseconds)
		{
			Task task = new Task(delegate()
			{
				action();
			});
			breadcrumbs.Drop("Executing task and waiting " + timeoutMilliseconds + "ms");
			task.Start();
			task.Wait(timeoutMilliseconds);
			if (!task.IsCompleted)
			{
				throw new TimeoutException("Task didn't complete before timeout expired.");
			}
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string ProbeTypeName = typeof(SharedCacheProbe).FullName;

		private static readonly string NamePropertyKey = "Name";

		private static readonly string ServiceNamePropertyKey = "ServiceName";

		private static readonly string TargetResourcePropertyKey = "TargetResource";

		private static readonly string CacheKey = "SharedCacheProbe";

		private static readonly string RpcTimeoutKey = "RpcClientTimeout";
	}
}
