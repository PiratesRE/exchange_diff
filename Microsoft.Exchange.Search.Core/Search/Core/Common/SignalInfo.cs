using System;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class SignalInfo
	{
		private SignalInfo(Enum enumValue, SignalPriority priority)
		{
			this.enumValue = enumValue;
			this.value = Convert.ToUInt32(enumValue);
			this.name = enumValue.ToString();
			this.priority = priority;
		}

		internal uint Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
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

		internal SignalPriority Priority
		{
			get
			{
				return this.priority;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", this.Name, this.Value);
		}

		internal static SignalInfo Create(Enum enumValue, SignalPriority priority)
		{
			return new SignalInfo(enumValue, priority);
		}

		internal SignalInfo Clone()
		{
			return new SignalInfo(this.enumValue, this.priority);
		}

		private Enum enumValue;

		private uint value;

		private string name;

		private SignalPriority priority;
	}
}
