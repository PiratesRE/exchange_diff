using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ComponentManager
	{
		public ComponentManager(TimeSpan periodicStartInterval)
		{
			this.m_periodicStartInterval = periodicStartInterval;
		}

		public virtual void Start()
		{
			bool flag = false;
			this.m_componentsOnline = new HashSet<IServiceComponent>();
			foreach (IServiceComponent serviceComponent in this.m_components)
			{
				lock (this.m_locker)
				{
					if (this.m_fShutdown)
					{
						ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ComponentManager: Start() is bailing due to shutdown.");
						return;
					}
					if (!serviceComponent.IsEnabled || serviceComponent.Start())
					{
						this.RecordComponentOnline(serviceComponent);
					}
					else
					{
						if (serviceComponent.IsCritical)
						{
							throw new ReplayCriticalComponentFailedToStartException(serviceComponent.Name);
						}
						if (serviceComponent.IsRetriableOnError)
						{
							this.LogRetriableComponentStartFailed(serviceComponent);
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				lock (this.m_locker)
				{
					if (!this.m_fShutdown)
					{
						this.m_periodicStarter = new ComponentManager.PeriodicComponentStarter(this, this.m_periodicStartInterval);
						this.m_periodicStarter.Start();
					}
				}
				return;
			}
		}

		public virtual void Stop()
		{
			lock (this.m_locker)
			{
				this.m_fShutdown = true;
			}
			this.StopPeriodicStarter();
			foreach (IServiceComponent serviceComponent in this.m_components.Reverse<IServiceComponent>())
			{
				if (serviceComponent.IsEnabled && this.m_componentsOnline.Contains(serviceComponent))
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "ComponentManager: Component '{0}' is being stopped.", serviceComponent.Name);
					serviceComponent.Stop();
					this.RecordComponentOffline(serviceComponent);
				}
			}
		}

		internal void HavePossibleHungComponentInvoke(Action toInvoke)
		{
			if (this.m_fShutdown)
			{
				foreach (IServiceComponent serviceComponent in this.m_components.Reverse<IServiceComponent>())
				{
					bool flag = false;
					lock (this.m_locker)
					{
						flag = this.m_componentsOnline.Contains(serviceComponent);
					}
					if (serviceComponent.IsEnabled && flag)
					{
						serviceComponent.Invoke(toInvoke);
						break;
					}
				}
			}
		}

		private void StartRetriableComponents()
		{
			IEnumerable<IServiceComponent> enumerable = null;
			lock (this.m_locker)
			{
				if (this.m_fShutdown)
				{
					ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ComponentManager: StartRetriableComponents() is bailing due to shutdown.");
					return;
				}
				enumerable = this.GetOfflineRetriableComponents();
			}
			foreach (IServiceComponent serviceComponent in enumerable)
			{
				lock (this.m_locker)
				{
					if (this.m_fShutdown)
					{
						ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ComponentManager: StartRetriableComponents() is bailing due to shutdown.");
						break;
					}
					if (!serviceComponent.IsEnabled || serviceComponent.Start())
					{
						this.RecordComponentOnline(serviceComponent);
					}
					else
					{
						ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "ComponentManager: StartRetriableComponents() failed to start retriable component '{0}'.", serviceComponent.Name);
						this.LogRetriableComponentStartFailed(serviceComponent);
					}
				}
			}
		}

		private void RecordComponentOnline(IServiceComponent component)
		{
			bool flag = false;
			if (component.IsEnabled)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "ComponentManager: Component '{0}' was successfully started.", component.Name);
				ReplayCrimsonEvents.FacilityReady.Log<string>(component.Name);
			}
			else
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "ComponentManager: Component '{0}' was not started because it's disabled.", component.Name);
			}
			lock (this.m_locker)
			{
				if (!this.m_componentsOnline.Contains(component))
				{
					this.m_onlineCount++;
					this.m_componentsOnline.Add(component);
					if (this.m_componentsOnline.Count == this.m_components.Length)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ComponentManager: All components have been successfully started.");
				ReplayEventLogConstants.Tuple_AllFacilitiesAreOnline.LogEvent(null, new object[0]);
				ThreadPool.QueueUserWorkItem(delegate(object param0)
				{
					this.StopPeriodicStarter();
				});
			}
		}

		private void RecordComponentOffline(IServiceComponent component)
		{
			if (component.IsEnabled)
			{
				lock (this.m_locker)
				{
					if (this.m_componentsOnline.Contains(component))
					{
						this.m_onlineCount--;
						this.m_componentsOnline.Remove(component);
					}
				}
				ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "ComponentManager: Component '{0}' was stopped.", component.Name);
				ReplayCrimsonEvents.FacilityOffline.Log<string>(component.Name);
				return;
			}
			ExTraceGlobals.ReplayManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "ComponentManager: Component '{0}' was not stopped because it's disabled.", component.Name);
		}

		private List<IServiceComponent> GetOfflineRetriableComponents()
		{
			List<IServiceComponent> list = new List<IServiceComponent>(this.m_components.Length);
			foreach (IServiceComponent serviceComponent in this.m_components)
			{
				if (!this.m_componentsOnline.Contains(serviceComponent) && serviceComponent.IsRetriableOnError)
				{
					list.Add(serviceComponent);
				}
			}
			return list;
		}

		private void LogRetriableComponentStartFailed(IServiceComponent component)
		{
			ReplayEventLogConstants.Tuple_FailedToStartRetriableComponent.LogEvent(component.Name, new object[]
			{
				component.Name,
				(int)this.m_periodicStartInterval.TotalSeconds
			});
		}

		private void StopPeriodicStarter()
		{
			if (this.m_periodicStarter != null)
			{
				ExTraceGlobals.ReplayManagerTracer.TraceDebug((long)this.GetHashCode(), "ComponentManager: PeriodicComponentStarter is being stopped.");
				this.m_periodicStarter.Stop();
				this.m_periodicStarter = null;
			}
		}

		protected IServiceComponent[] m_components;

		private readonly TimeSpan m_periodicStartInterval;

		private ComponentManager.PeriodicComponentStarter m_periodicStarter;

		private HashSet<IServiceComponent> m_componentsOnline;

		private int m_onlineCount;

		private bool m_fShutdown;

		private object m_locker = new object();

		[ClassAccessLevel(AccessLevel.Implementation)]
		private class PeriodicComponentStarter : TimerComponent
		{
			public PeriodicComponentStarter(ComponentManager componentManager, TimeSpan periodicStartInterval) : base(periodicStartInterval, periodicStartInterval, "PeriodicComponentStarter")
			{
				this.m_componentManager = componentManager;
			}

			protected override void TimerCallbackInternal()
			{
				this.m_componentManager.StartRetriableComponents();
			}

			private ComponentManager m_componentManager;
		}
	}
}
