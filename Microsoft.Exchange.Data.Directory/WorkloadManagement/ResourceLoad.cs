using System;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal struct ResourceLoad
	{
		public ResourceLoad(double loadRatio, int? metric, object info = null)
		{
			if (loadRatio >= 1.7976931348623157E+308)
			{
				this.loadRatio = double.MaxValue;
			}
			else
			{
				this.loadRatio = loadRatio + 1.0;
			}
			this.metric = metric;
			this.info = info;
		}

		public static bool operator ==(ResourceLoad load1, ResourceLoad load2)
		{
			return load1.LoadRatio == load2.LoadRatio;
		}

		public static bool operator !=(ResourceLoad load1, ResourceLoad load2)
		{
			return load1.LoadRatio != load2.LoadRatio;
		}

		public static bool operator <(ResourceLoad load1, ResourceLoad load2)
		{
			return load1.LoadRatio < load2.LoadRatio;
		}

		public static bool operator >(ResourceLoad load1, ResourceLoad load2)
		{
			return load1.LoadRatio > load2.LoadRatio;
		}

		public static bool operator <=(ResourceLoad load1, ResourceLoad load2)
		{
			return load1.LoadRatio <= load2.LoadRatio;
		}

		public static bool operator >=(ResourceLoad load1, ResourceLoad load2)
		{
			return load1.LoadRatio >= load2.LoadRatio;
		}

		public static double operator -(ResourceLoad load1, ResourceLoad load2)
		{
			ResourceLoadState state = load1.State;
			if (state == ResourceLoadState.Unknown || state == ResourceLoadState.Critical)
			{
				return double.NaN;
			}
			ResourceLoadState state2 = load2.State;
			if (state2 == ResourceLoadState.Unknown || state2 == ResourceLoadState.Critical)
			{
				return double.NaN;
			}
			return load1.LoadRatio - load2.LoadRatio;
		}

		public static ResourceLoad operator -(ResourceLoad load, double delta)
		{
			ResourceLoadState state = load.State;
			if (state == ResourceLoadState.Unknown || state == ResourceLoadState.Critical)
			{
				return load;
			}
			return new ResourceLoad(load.LoadRatio - delta, load.Metric, load.Info);
		}

		public static ResourceLoad operator +(ResourceLoad load, double delta)
		{
			ResourceLoadState state = load.State;
			if (state == ResourceLoadState.Unknown || state == ResourceLoadState.Critical)
			{
				return load;
			}
			return new ResourceLoad(load.LoadRatio + delta, load.Metric, load.Info);
		}

		public ResourceLoadState State
		{
			get
			{
				if (this.LoadRatio < 0.0)
				{
					return ResourceLoadState.Unknown;
				}
				if (this.LoadRatio == 1.7976931348623157E+308)
				{
					return ResourceLoadState.Critical;
				}
				if (this.LoadRatio > 1.0)
				{
					return ResourceLoadState.Overloaded;
				}
				if (this.LoadRatio == 1.0)
				{
					return ResourceLoadState.Full;
				}
				if (this.LoadRatio < 1.0)
				{
					return ResourceLoadState.Underloaded;
				}
				throw new InvalidOperationException("Unexpected load ratio value.");
			}
		}

		public double LoadRatio
		{
			get
			{
				if (this.loadRatio != 1.7976931348623157E+308)
				{
					return this.loadRatio - 1.0;
				}
				return double.MaxValue;
			}
		}

		public int? Metric
		{
			get
			{
				return this.metric;
			}
		}

		public object Info
		{
			get
			{
				return this.info;
			}
		}

		public override string ToString()
		{
			ResourceLoadState state = this.State;
			if (state != ResourceLoadState.Critical && state != ResourceLoadState.Unknown)
			{
				return string.Format("{0}/{1:#0.00}/{2}", state, this.LoadRatio, this.Metric);
			}
			return string.Format("{0}//{1}", state, this.Metric);
		}

		public override int GetHashCode()
		{
			return this.LoadRatio.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return this.State == ResourceLoadState.Unknown;
			}
			if (obj is ResourceLoad)
			{
				ResourceLoad resourceLoad = (ResourceLoad)obj;
				return this.loadRatio == resourceLoad.loadRatio;
			}
			return false;
		}

		public static readonly ResourceLoad Unknown = default(ResourceLoad);

		public static readonly ResourceLoad Zero = new ResourceLoad(0.0, null, null);

		public static readonly ResourceLoad Full = new ResourceLoad(1.0, null, null);

		public static readonly ResourceLoad Critical = new ResourceLoad(double.MaxValue, null, null);

		private readonly double loadRatio;

		private readonly int? metric;

		private readonly object info;
	}
}
