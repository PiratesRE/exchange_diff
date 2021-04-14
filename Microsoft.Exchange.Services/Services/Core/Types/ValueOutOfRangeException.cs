using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ValueOutOfRangeException : ServicePermanentException
	{
		public ValueOutOfRangeException() : base(CoreResources.IDs.ErrorValueOutOfRange)
		{
		}

		public ValueOutOfRangeException(Exception innerException) : base(CoreResources.IDs.ErrorValueOutOfRange, innerException)
		{
		}

		public ValueOutOfRangeException(Exception innerException, string[] keys, string[] values) : base(CoreResources.IDs.ErrorValueOutOfRange, innerException)
		{
			if (keys != null && values != null)
			{
				int num = 0;
				while (num < keys.Length && num < values.Length)
				{
					if (!string.IsNullOrEmpty(keys[num]) && !base.ConstantValues.ContainsKey(keys[num]))
					{
						base.ConstantValues.Add(keys[num], values[num]);
					}
					num++;
				}
			}
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}
	}
}
