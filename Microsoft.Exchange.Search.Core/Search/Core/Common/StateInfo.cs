using System;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class StateInfo
	{
		private StateInfo(Enum enumValue)
		{
			this.enumValue = enumValue;
			this.value = Convert.ToUInt32(enumValue);
			this.name = enumValue.ToString();
		}

		internal uint Value
		{
			get
			{
				return this.value;
			}
		}

		internal Enum Enum
		{
			get
			{
				return this.enumValue;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", this.Name, this.Value);
		}

		internal static StateInfo Create(Enum enumValue)
		{
			return new StateInfo(enumValue);
		}

		private Enum enumValue;

		private uint value;

		private string name;
	}
}
