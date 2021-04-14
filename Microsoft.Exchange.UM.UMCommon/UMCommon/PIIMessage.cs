using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class PIIMessage
	{
		private PIIMessage(PIIType key, string value)
		{
			this.Key = key;
			this.Value = value;
		}

		public PIIType Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		public string Value
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

		public string LogSafeValue
		{
			get
			{
				if (this.Value == null)
				{
					return "<null>";
				}
				return this.Value.Replace("​", "").Replace(',', '‚');
			}
		}

		public override string ToString()
		{
			return PIIMessage.PIITypeEnumToString[(int)this.key] + "=" + this.LogSafeValue;
		}

		public static PIIMessage Create(PIIType key, string value)
		{
			return new PIIMessage(key, value);
		}

		public static PIIMessage Create(PIIType key, object value)
		{
			if (value == null)
			{
				return new PIIMessage(key, "<null>");
			}
			return new PIIMessage(key, value.ToString());
		}

		private static string[] InitPIITypeEnumToStringMapping()
		{
			PIIType[] array = (PIIType[])Enum.GetValues(typeof(PIIType));
			string[] array2 = new string[array.Length];
			foreach (PIIType piitype in array)
			{
				array2[(int)piitype] = piitype.ToString();
			}
			return array2;
		}

		private static string[] PIITypeEnumToString = PIIMessage.InitPIITypeEnumToStringMapping();

		private PIIType key;

		private string value;
	}
}
