using System;

namespace Microsoft.Exchange.Transport.Storage
{
	[Serializable]
	internal class ColumnCacheString : ColumnCache<string>
	{
		public override string Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					value = null;
				}
				if (!string.Equals(this.Value, value))
				{
					if (value != null)
					{
						base.Value = value;
						return;
					}
					base.HasValue = false;
				}
			}
		}
	}
}
