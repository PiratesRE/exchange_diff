using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class ResourceReservationContext : DisposeTrackableBase
	{
		public ResourceReservationContext() : this(false)
		{
		}

		internal ResourceReservationContext(bool ignoreImplicitLocalCpuResource) : this(new ResourceAdmissionControlManager(), ignoreImplicitLocalCpuResource)
		{
		}

		internal ResourceReservationContext(ResourceAdmissionControlManager admissionManager) : this(admissionManager, false)
		{
		}

		internal ResourceReservationContext(ResourceAdmissionControlManager admissionManager, bool ignoreImplicitLocalCpuResource)
		{
			this.admissionManager = admissionManager;
			this.ignoreImplicitLocalCpuResource = ignoreImplicitLocalCpuResource;
		}

		internal ResourceAdmissionControlManager AdmissionControlManager
		{
			get
			{
				return this.admissionManager;
			}
		}

		public ResourceReservation GetUnthrottledReservation(SystemWorkloadBase workload)
		{
			if (workload.WorkloadType != WorkloadType.MailboxReplicationServiceHighPriority && workload.WorkloadType != WorkloadType.MailboxReplicationServiceInteractive && workload.WorkloadType != WorkloadType.MailboxReplicationServiceInternalMaintenance)
			{
				WorkloadType workloadType = workload.WorkloadType;
			}
			return new ResourceReservation(workload.Classification, ResourceReservationContext.emptyReservation, 0.0);
		}

		public ResourceReservation GetReservation(SystemWorkloadBase workload, IEnumerable<ResourceKey> resources)
		{
			return this.GetReservation(workload.Classification, resources);
		}

		public ResourceReservation GetReservation(SystemWorkloadBase workload, IEnumerable<ResourceKey> resources, out ResourceKey throttledResource)
		{
			return this.GetReservation(workload.Classification, resources, out throttledResource);
		}

		public ResourceReservation GetReservation(WorkloadClassification classification, IEnumerable<ResourceKey> resources)
		{
			ResourceKey resourceKey = null;
			return this.GetReservation(classification, resources, out resourceKey);
		}

		public ResourceReservation GetReservation(WorkloadClassification classification, IEnumerable<ResourceKey> resources, out ResourceKey throttledResource)
		{
			if (resources == null)
			{
				throw new ArgumentNullException("resources");
			}
			List<IResourceAdmissionControl> list = null;
			ResourceReservation resourceReservation = null;
			double delayFactor = 0.0;
			bool flag = false;
			throttledResource = null;
			lock (this.admissionManager)
			{
				try
				{
					foreach (ResourceKey resourceKey in resources)
					{
						if (!this.GetReservationForResource(classification, resourceKey, ref list, ref delayFactor, ref throttledResource))
						{
							return null;
						}
						if (!flag)
						{
							flag = resourceKey.Equals(ProcessorResourceKey.Local);
						}
					}
					if (!flag && !this.ignoreImplicitLocalCpuResource && !this.GetReservationForResource(classification, ProcessorResourceKey.Local, ref list, ref delayFactor, ref throttledResource))
					{
						return null;
					}
					resourceReservation = new ResourceReservation(classification, list ?? ResourceReservationContext.emptyReservation, delayFactor);
				}
				finally
				{
					if (resourceReservation == null && list != null)
					{
						foreach (IResourceAdmissionControl resourceAdmissionControl in list)
						{
							resourceAdmissionControl.Release(classification);
						}
					}
				}
			}
			return resourceReservation;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.admissionManager.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ResourceReservationContext>(this);
		}

		private bool GetReservationForResource(WorkloadClassification classification, ResourceKey resource, ref List<IResourceAdmissionControl> reservedAdmissions, ref double delayRatio, ref ResourceKey throttledResource)
		{
			try
			{
				IResourceAdmissionControl resourceAdmissionControl = this.admissionManager.Get(resource);
				double num = 0.0;
				if (!resourceAdmissionControl.TryAcquire(classification, out num))
				{
					ExTraceGlobals.SchedulerTracer.TraceDebug<ResourceKey, WorkloadClassification>((long)this.GetHashCode(), "[ResourceReservationContext.GetReservationForResource] Unable to reserve resource {0}. Classification: {1}", resource, classification);
					throttledResource = resource;
					return false;
				}
				if (reservedAdmissions == null)
				{
					reservedAdmissions = new List<IResourceAdmissionControl>();
				}
				reservedAdmissions.Add(resourceAdmissionControl);
				if (num > delayRatio)
				{
					delayRatio = num;
					throttledResource = resource;
				}
			}
			catch (NonOperationalAdmissionControlException arg)
			{
				ExTraceGlobals.SchedulerTracer.TraceError<NonOperationalAdmissionControlException>((long)this.GetHashCode(), "[ResourceReservationContext.GetReservationForResource] Encountered exception while reserving resources: {1}", arg);
				return false;
			}
			return true;
		}

		private static readonly IEnumerable<IResourceAdmissionControl> emptyReservation = new IResourceAdmissionControl[0];

		private readonly bool ignoreImplicitLocalCpuResource;

		private ResourceAdmissionControlManager admissionManager;
	}
}
