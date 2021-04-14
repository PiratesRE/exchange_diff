using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Transport
{
	internal class MultiValueHeader
	{
		internal MultiValueHeader(IMailItemStorage mailItem, string headerName)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			if (string.IsNullOrEmpty(headerName))
			{
				throw new ArgumentException("headerName cannot be null or empty", "headerName");
			}
			this.mailItem = mailItem;
			this.headerName = headerName;
		}

		public string HeaderName
		{
			get
			{
				return this.headerName;
			}
		}

		private Dictionary<string, string> PropertyBag
		{
			get
			{
				if (this.propertyBag == null)
				{
					this.propertyBag = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					this.Deserialize();
				}
				return this.propertyBag;
			}
		}

		public void SetStringValue(string property, string value)
		{
			if (string.IsNullOrEmpty(property))
			{
				throw new ArgumentException("Cannot set property to empty or null string", "property");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!MultiValueHeader.IsValidToken(property) || !MultiValueHeader.IsValidToken(value))
			{
				throw new FormatException("Invalid property format");
			}
			string a;
			if (this.PropertyBag.TryGetValue(property, out a) && string.Equals(a, value, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			this.PropertyBag[property] = value;
			string headerValue = this.Serialize();
			this.UpdateMimeDoc(headerValue);
		}

		public void SetBoolValue(string property, bool value)
		{
			this.SetStringValue(property, value.ToString(CultureInfo.InvariantCulture));
		}

		public void SetInt32Value(string property, int value)
		{
			this.SetStringValue(property, value.ToString(CultureInfo.InvariantCulture));
		}

		public bool TryGetStringValue(string property, out string value)
		{
			return this.PropertyBag.TryGetValue(property, out value);
		}

		public bool TryGetBoolValue(string property, out bool value)
		{
			value = false;
			string value2;
			return this.TryGetStringValue(property, out value2) && bool.TryParse(value2, out value);
		}

		public bool TryGetInt32Value(string property, out int value)
		{
			value = 0;
			string s;
			return this.TryGetStringValue(property, out s) && int.TryParse(s, out value);
		}

		private static bool IsValidToken(string token)
		{
			char[] anyOf = new char[]
			{
				'\r',
				'\n',
				';',
				'='
			};
			return token.IndexOfAny(anyOf) == -1;
		}

		private string Serialize()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in this.PropertyBag)
			{
				stringBuilder.AppendFormat("{0}={1}; ", keyValuePair.Key, keyValuePair.Value);
			}
			if (stringBuilder.Length > 2)
			{
				stringBuilder.Length -= 2;
			}
			return stringBuilder.ToString();
		}

		private void Deserialize()
		{
			if (this.mailItem.MimeDocument.RootPart == null)
			{
				throw new InvalidOperationException("MimeDocument.RootPart is not accessible");
			}
			char[] separator = new char[]
			{
				';'
			};
			char[] separator2 = new char[]
			{
				'='
			};
			HeaderList headers = this.mailItem.MimeDocument.RootPart.Headers;
			Header header = headers.FindFirst(this.headerName);
			if (header != null)
			{
				string value = header.Value;
				string[] array = value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					string[] array3 = text.Split(separator2);
					if (array3.Length == 2)
					{
						this.PropertyBag[array3[0].Trim()] = array3[1].Trim();
					}
				}
			}
		}

		private void UpdateMimeDoc(string headerValue)
		{
			if (this.mailItem.MimeDocument.RootPart == null)
			{
				throw new InvalidOperationException("MimeDocument.RootPart is not accessible");
			}
			if (!string.IsNullOrEmpty(headerValue))
			{
				HeaderList headers = this.mailItem.MimeDocument.RootPart.Headers;
				Header header = headers.FindFirst(this.headerName);
				if (header == null)
				{
					headers.AppendChild(new TextHeader(this.headerName, headerValue));
					return;
				}
				header.Value = headerValue;
			}
		}

		private readonly IMailItemStorage mailItem;

		private readonly string headerName;

		private Dictionary<string, string> propertyBag;
	}
}
