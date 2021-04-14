using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Office.Server.Directory;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal sealed class CorrelationContext : DisposeTrackableBase
	{
		public static IActivityScope GetActivityScope(Guid correlationId)
		{
			IActivityScope result;
			lock (CorrelationContext.activityScopes)
			{
				CorrelationContext.ActivityScopeCount activityScopeCount;
				if (CorrelationContext.activityScopes.TryGetValue(correlationId, out activityScopeCount))
				{
					result = activityScopeCount.ActivityScope;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public CorrelationContext()
		{
			IActivityScope activityScope = ActivityContext.GetCurrentActivityScope();
			if (activityScope == null)
			{
				activityScope = ActivityContext.Start(null);
				this.endActivityScope = activityScope;
				CorrelationContext.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "CorrelationContext started new activity scope with ID: {0}", activityScope.ActivityId);
			}
			else
			{
				CorrelationContext.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "CorrelationContext using existing activity scope with ID: {0}", activityScope.ActivityId);
			}
			this.correlationId = activityScope.ActivityId;
			CorrelationContext.AddActivityScope(this.correlationId, activityScope);
			LogWriter.Initialize();
			LogManager.CorrelationStart(this.correlationId);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CorrelationContext>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.correlationId != Guid.Empty)
				{
					LogManager.CorrelationEnd();
					CorrelationContext.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "CorrelationContext ending with ID: {0}", this.correlationId);
					CorrelationContext.RemoveActivityScope(this.correlationId);
					this.correlationId = Guid.Empty;
				}
				if (this.endActivityScope != null)
				{
					this.endActivityScope.End();
					this.endActivityScope = null;
				}
			}
		}

		private static void AddActivityScope(Guid correlationId, IActivityScope activityScope)
		{
			lock (CorrelationContext.activityScopes)
			{
				CorrelationContext.ActivityScopeCount activityScopeCount;
				if (!CorrelationContext.activityScopes.TryGetValue(correlationId, out activityScopeCount))
				{
					activityScopeCount = new CorrelationContext.ActivityScopeCount(activityScope);
					CorrelationContext.activityScopes.Add(correlationId, activityScopeCount);
				}
				activityScopeCount.Count++;
			}
		}

		private static void RemoveActivityScope(Guid correlationId)
		{
			lock (CorrelationContext.activityScopes)
			{
				CorrelationContext.ActivityScopeCount activityScopeCount;
				if (CorrelationContext.activityScopes.TryGetValue(correlationId, out activityScopeCount))
				{
					activityScopeCount.Count--;
					if (activityScopeCount.Count == 0)
					{
						CorrelationContext.activityScopes.Remove(correlationId);
					}
				}
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.FederatedDirectoryTracer;

		private Guid correlationId;

		private IActivityScope endActivityScope;

		private static readonly Dictionary<Guid, CorrelationContext.ActivityScopeCount> activityScopes = new Dictionary<Guid, CorrelationContext.ActivityScopeCount>();

		private sealed class ActivityScopeCount
		{
			public ActivityScopeCount(IActivityScope activityScope)
			{
				this.ActivityScope = activityScope;
			}

			public int Count { get; set; }

			public IActivityScope ActivityScope { get; private set; }
		}
	}
}
