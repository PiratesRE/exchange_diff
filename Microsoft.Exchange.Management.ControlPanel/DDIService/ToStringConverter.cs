using System;
using System.Collections;
using System.Text;

namespace Microsoft.Exchange.Management.DDIService
{
	public class ToStringConverter : IOutputConverter, IDDIConverter
	{
		public ToStringConverter(ConvertMode convertMode)
		{
			this.ConvertMode = convertMode;
		}

		public ConvertMode ConvertMode { get; private set; }

		public bool CanConvert(object sourceValue)
		{
			return true;
		}

		public object Convert(object sourceValue)
		{
			string result = string.Empty;
			if (sourceValue != null)
			{
				if (this.ConvertMode == ConvertMode.PerItemInEnumerable)
				{
					IEnumerable enumerable = sourceValue as IEnumerable;
					if (enumerable != null)
					{
						StringBuilder stringBuilder = new StringBuilder();
						bool flag = true;
						foreach (object obj in enumerable)
						{
							if (flag)
							{
								flag = false;
							}
							else
							{
								stringBuilder.Append(',');
							}
							stringBuilder.Append(obj.ToString());
						}
						result = stringBuilder.ToString();
					}
				}
				else
				{
					result = sourceValue.ToString();
				}
			}
			return result;
		}
	}
}
