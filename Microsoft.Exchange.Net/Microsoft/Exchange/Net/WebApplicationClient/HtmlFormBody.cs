using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal class HtmlFormBody : TextBody, IEnumerable
	{
		public HtmlFormBody() : this(Encoding.UTF8)
		{
		}

		public HtmlFormBody(Encoding encoding)
		{
			base.ContentType = new ContentType("application/x-www-form-urlencoded")
			{
				CharSet = encoding.WebName
			};
			this.Encoding = encoding;
			this.Parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}

		public Encoding Encoding { get; private set; }

		public IDictionary<string, object> Parameters { get; private set; }

		public void Add(string key, object value)
		{
			this.Parameters.Add(key, value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Parameters.GetEnumerator();
		}

		public override void Write(TextWriter writer)
		{
			bool flag = false;
			foreach (KeyValuePair<string, object> keyValuePair in this.Parameters)
			{
				if (keyValuePair.Value is Array)
				{
					Array array = (Array)keyValuePair.Value;
					for (int i = 0; i < array.Length; i++)
					{
						this.Write(writer, keyValuePair.Key, array.GetValue(i), ref flag);
					}
				}
				else
				{
					this.Write(writer, keyValuePair.Key, keyValuePair.Value, ref flag);
				}
			}
		}

		private void Write(TextWriter writer, string name, object value, ref bool writeSeparator)
		{
			if (writeSeparator)
			{
				writer.Write('&');
			}
			writer.Write(HttpUtility.UrlEncode(name, this.Encoding));
			writer.Write('=');
			writer.Write(HttpUtility.UrlEncode(Convert.ToString(value, CultureInfo.InvariantCulture), this.Encoding));
			writeSeparator = true;
		}
	}
}
