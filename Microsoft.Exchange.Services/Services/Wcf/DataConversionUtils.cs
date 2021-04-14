using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Services.Wcf
{
	public static class DataConversionUtils
	{
		public static string[] ToStringArray<T>(this MultiValuedProperty<T> objectProperty)
		{
			if (objectProperty == null || MultiValuedPropertyBase.IsNullOrEmpty(objectProperty))
			{
				return null;
			}
			return (from e in objectProperty
			select e.ToString()).ToArray<string>();
		}

		public static string ConvertHtmlToText(string html)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(html);
			string @string;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					new HtmlToText
					{
						InputEncoding = Encoding.UTF8,
						OutputEncoding = Encoding.UTF8
					}.Convert(memoryStream, memoryStream2);
					@string = Encoding.UTF8.GetString(memoryStream2.ToArray());
				}
			}
			return @string;
		}
	}
}
