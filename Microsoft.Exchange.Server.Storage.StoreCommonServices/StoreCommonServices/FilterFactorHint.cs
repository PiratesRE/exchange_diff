using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct FilterFactorHint
	{
		public bool IsEquality
		{
			get
			{
				return true;
			}
		}

		public bool AnyValue
		{
			get
			{
				return true;
			}
		}

		public object Value
		{
			get
			{
				return null;
			}
		}

		public double FilterFactor { get; set; }
	}
}
