using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class ResourceReservation : DisposeTrackableBase
	{
		internal ResourceReservation(WorkloadClassification classification, IEnumerable<IResourceAdmissionControl> reserved, double delayFactor)
		{
			if (reserved == null)
			{
				throw new ArgumentNullException("reserved");
			}
			if (delayFactor < 0.0)
			{
				throw new ArgumentOutOfRangeException("delayFactor", "Delay factor must be greater or equal to 0.");
			}
			this.Classification = classification;
			this.reserved = reserved;
			this.DelayFactor = delayFactor;
		}

		public WorkloadClassification Classification { get; private set; }

		public IEnumerable<ResourceKey> Resources
		{
			get
			{
				if (this.resources == null)
				{
					lock (this.lockObject)
					{
						if (this.resources == null)
						{
							this.resources = new List<ResourceKey>();
							foreach (IResourceAdmissionControl resourceAdmissionControl in this.reserved)
							{
								this.resources.Add(resourceAdmissionControl.ResourceKey);
							}
						}
					}
				}
				return this.resources;
			}
		}

		public bool IsActive
		{
			get
			{
				return !base.IsDisposed;
			}
		}

		public double DelayFactor { get; private set; }

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.reserved != null)
			{
				lock (this.lockObject)
				{
					foreach (IResourceAdmissionControl resourceAdmissionControl in this.reserved)
					{
						resourceAdmissionControl.Release(this.Classification);
					}
					this.reserved = ResourceReservation.emptyReserved;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ResourceReservation>(this);
		}

		private static readonly IEnumerable<IResourceAdmissionControl> emptyReserved = new IResourceAdmissionControl[0];

		private IEnumerable<IResourceAdmissionControl> reserved;

		private List<ResourceKey> resources;

		private object lockObject = new object();
	}
}
