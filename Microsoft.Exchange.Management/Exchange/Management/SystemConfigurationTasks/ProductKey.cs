using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public struct ProductKey
	{
		public ProductKey(string productKey)
		{
			this.productKey = productKey;
			this.sku = Sku.GetSku(productKey);
			if (this.Sku == null)
			{
				throw new InvalidProductKeyException();
			}
		}

		public static ProductKey Parse(string productKey)
		{
			return new ProductKey(productKey);
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.productKey);
			}
		}

		public override string ToString()
		{
			return this.productKey ?? "";
		}

		internal Sku Sku
		{
			get
			{
				return this.sku;
			}
		}

		internal string GenerateProductID()
		{
			if (this.IsEmpty)
			{
				throw new InvalidProductKeyException();
			}
			return this.Sku.GenerateProductID(this.productKey);
		}

		private readonly string productKey;

		private readonly Sku sku;

		public static readonly ProductKey Empty = default(ProductKey);
	}
}
