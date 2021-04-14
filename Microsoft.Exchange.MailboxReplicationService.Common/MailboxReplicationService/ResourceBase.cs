using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class ResourceBase
	{
		public ResourceBase()
		{
			this.Reservations = new Dictionary<Guid, ReservationBase>();
			this.TransferRate = new FixedTimeSum(1000, 60);
		}

		public abstract string ResourceName { get; }

		public abstract string ResourceType { get; }

		public ExPerformanceCounter UtilizationPerfCounter { get; protected set; }

		public PerfCounterWithAverageRate TransferRatePerfCounter { get; protected set; }

		public virtual int StaticCapacity
		{
			get
			{
				return 0;
			}
		}

		public int Utilization
		{
			get
			{
				int count;
				lock (this.Locker)
				{
					count = this.Reservations.Count;
				}
				return count;
			}
		}

		public virtual bool IsUnhealthy
		{
			get
			{
				return false;
			}
		}

		private protected Dictionary<Guid, ReservationBase> Reservations { protected get; private set; }

		protected SettingsContextBase ConfigContext { get; set; }

		public void Reserve(ReservationBase reservation)
		{
			lock (this.Locker)
			{
				if (!this.Reservations.ContainsKey(reservation.ReservationId))
				{
					this.VerifyCapacity(reservation);
					this.AddReservation(reservation);
					if (this.UtilizationPerfCounter != null)
					{
						this.UtilizationPerfCounter.RawValue = (long)this.Utilization;
					}
				}
			}
		}

		public void Charge(uint bytes)
		{
			this.TransferRate.Add(bytes);
			if (this.TransferRatePerfCounter != null)
			{
				this.TransferRatePerfCounter.IncrementBy((long)((ulong)bytes));
			}
		}

		public XElement GetDiagnosticInfo(MRSDiagnosticArgument arguments)
		{
			string argument = arguments.GetArgument<string>("resources");
			if (!string.IsNullOrEmpty(argument) && !CommonUtils.IsValueInWildcardedList(this.ResourceName, argument) && !CommonUtils.IsValueInWildcardedList(this.ResourceType, argument))
			{
				return null;
			}
			if (arguments.HasArgument("unhealthy") && !this.IsUnhealthy)
			{
				return null;
			}
			return this.GetDiagnosticInfoInternal(arguments);
		}

		public override string ToString()
		{
			return string.Format("Resource {0}({1}), StaticCapacity {1}, Utilization {2}", new object[]
			{
				this.ResourceType,
				this.ResourceName,
				this.StaticCapacity,
				this.Utilization
			});
		}

		protected virtual void VerifyStaticCapacity(ReservationBase reservation)
		{
			if (this.Utilization >= this.StaticCapacity)
			{
				this.ThrowStaticCapacityExceededException();
			}
		}

		protected virtual void ThrowStaticCapacityExceededException()
		{
			throw new StaticCapacityExceededReservationException(this.ResourceName, this.ResourceType, this.StaticCapacity);
		}

		protected virtual void VerifyDynamicCapacity(ReservationBase reservation)
		{
		}

		protected virtual void AddReservation(ReservationBase reservation)
		{
			this.Reservations.Add(reservation.ReservationId, reservation);
			reservation.AddReleaseAction(new Action<ReservationBase>(this.ReleaseReservation));
		}

		protected virtual XElement GetDiagnosticInfoInternal(MRSDiagnosticArgument arguments)
		{
			XElement result;
			lock (this.Locker)
			{
				XElement xelement = new XElement("Resource", new object[]
				{
					new XAttribute("Type", this.ResourceType),
					new XAttribute("Name", this.ResourceName),
					new XAttribute("Utilization", this.Utilization)
				});
				foreach (ReservationBase reservationBase in this.Reservations.Values)
				{
					xelement.Add(new XElement("Client", reservationBase.ClientName));
				}
				result = xelement;
			}
			return result;
		}

		private void ReleaseReservation(ReservationBase reservation)
		{
			lock (this.Locker)
			{
				if (this.Reservations.Remove(reservation.ReservationId) && this.UtilizationPerfCounter != null)
				{
					this.UtilizationPerfCounter.RawValue = (long)this.Utilization;
				}
			}
		}

		private void VerifyCapacity(ReservationBase reservation)
		{
			this.VerifyStaticCapacity(reservation);
			this.VerifyDynamicCapacity(reservation);
		}

		protected readonly FixedTimeSum TransferRate;

		protected readonly object Locker = new object();
	}
}
