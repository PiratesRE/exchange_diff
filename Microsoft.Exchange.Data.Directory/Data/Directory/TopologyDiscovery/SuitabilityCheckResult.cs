using System;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	internal class SuitabilityCheckResult
	{
		public SuitabilityCheckResult()
		{
			this.IsEnabled = false;
			this.IsDNSEntryAvailable = false;
			this.IsReachableByTCPConnection = ADServerRole.None;
			this.IsSynchronized = ADServerRole.None;
			this.IsPDC = false;
			this.IsSACLRightAvailable = false;
			this.IsCriticalDataAvailable = false;
			this.IsNetlogonAllowed = ADServerRole.None;
			this.IsOSVersionSuitable = false;
			this.WritableNC = string.Empty;
			this.SchemaNC = string.Empty;
			this.RootNC = string.Empty;
		}

		public string ConfigNC { get; set; }

		public string RootNC { get; set; }

		public string SchemaNC { get; set; }

		public string WritableNC { get; set; }

		public bool IsDNSEntryAvailable { get; set; }

		public ADServerRole IsReachableByTCPConnection { get; set; }

		public bool IsEnabled { get; set; }

		public bool IsSACLRightAvailable { get; set; }

		public bool IsCriticalDataAvailable { get; set; }

		public ADServerRole IsSynchronized { get; set; }

		public bool IsOSVersionSuitable { get; set; }

		public bool IsInMM { get; set; }

		public ADServerRole IsNetlogonAllowed { get; set; }

		public bool IsPDC { get; set; }

		public bool IsReadOnlyDC { get; set; }

		public override string ToString()
		{
			if (Globals.IsDatacenter)
			{
				return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", new object[]
				{
					Convert.ToInt16(this.IsEnabled),
					(int)this.IsReachableByTCPConnection,
					(int)this.IsSynchronized,
					Convert.ToInt16((this.IsSynchronized & ADServerRole.GlobalCatalog) != ADServerRole.None),
					Convert.ToInt16(this.IsPDC),
					Convert.ToInt16(this.IsSACLRightAvailable),
					Convert.ToInt16(this.IsCriticalDataAvailable),
					(int)this.IsNetlogonAllowed,
					Convert.ToInt16(this.IsOSVersionSuitable),
					Convert.ToInt16(this.IsInMM)
				});
			}
			return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", new object[]
			{
				Convert.ToInt16(this.IsEnabled),
				(int)this.IsReachableByTCPConnection,
				(int)this.IsSynchronized,
				Convert.ToInt16((this.IsSynchronized & ADServerRole.GlobalCatalog) != ADServerRole.None),
				Convert.ToInt16(this.IsPDC),
				Convert.ToInt16(this.IsSACLRightAvailable),
				Convert.ToInt16(this.IsCriticalDataAvailable),
				(int)this.IsNetlogonAllowed,
				Convert.ToInt16(this.IsOSVersionSuitable)
			});
		}

		internal bool IsSuitable(ADServerRole role)
		{
			return this.IsEnabled && (this.IsReachableByTCPConnection & role) == role && (this.IsSynchronized & role) == role && this.IsSACLRightAvailable && this.IsCriticalDataAvailable && (this.IsNetlogonAllowed & role) == role && this.IsOSVersionSuitable && (!Globals.IsDatacenter || !this.IsInMM);
		}
	}
}
