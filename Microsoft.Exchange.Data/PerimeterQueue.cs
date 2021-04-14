using System;

namespace Microsoft.Exchange.Data
{
	public struct PerimeterQueue
	{
		public PerimeterQueue(int forestId, string forestName, int value, string additionalInfo)
		{
			this.forestIdentity = forestId;
			this.forestName = forestName;
			this.value = value;
			this.additionalInfo = additionalInfo;
		}

		public int ForestIdentity
		{
			get
			{
				return this.forestIdentity;
			}
		}

		public string Name
		{
			get
			{
				if (!string.IsNullOrEmpty(this.additionalInfo))
				{
					return this.forestName + " - " + this.additionalInfo;
				}
				return this.forestName;
			}
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		public override string ToString()
		{
			return this.Name + " : " + this.Value;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode() ^ this.Value.GetHashCode();
		}

		private int forestIdentity;

		private string forestName;

		private int value;

		private string additionalInfo;
	}
}
