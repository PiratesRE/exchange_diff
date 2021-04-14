using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class EnumValue : BaseRow
	{
		public EnumValue(Enum value) : base(null, null)
		{
			this.cachedValue = value;
			this.Value = value.ToString();
		}

		public EnumValue(Enum value, bool useLocalizedValue) : this(value)
		{
			this.useLocalizedValue = useLocalizedValue;
		}

		public EnumValue(string displayText, string value) : base(null, null)
		{
			this.displayText = displayText;
			this.Value = value;
		}

		[DataMember]
		public string DisplayText
		{
			get
			{
				if (this.displayText != null)
				{
					return this.displayText;
				}
				return LocalizedDescriptionAttribute.FromEnum(this.cachedValue.GetType(), this.cachedValue);
			}
			protected set
			{
				this.displayText = value;
			}
		}

		[DataMember]
		public string Value
		{
			get
			{
				if (this.useLocalizedValue)
				{
					return LocalizedDescriptionAttribute.FromEnum(this.cachedValue.GetType(), this.cachedValue);
				}
				return this.value;
			}
			protected set
			{
				this.value = value;
			}
		}

		private Enum cachedValue;

		private bool useLocalizedValue;

		private string displayText;

		private string value;
	}
}
