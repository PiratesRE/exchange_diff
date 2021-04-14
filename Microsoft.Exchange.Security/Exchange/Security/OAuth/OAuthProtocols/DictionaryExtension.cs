using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Microsoft.Exchange.Security.OAuth.OAuthProtocols
{
	internal static class DictionaryExtension
	{
		private static string NullEncode(string value)
		{
			return value;
		}

		public static void Decode(this IDictionary<string, string> self, string encodedDictionary)
		{
			self.Decode(encodedDictionary, '&', '=', DictionaryExtension.DefaultDecoder, DictionaryExtension.DefaultDecoder, false);
		}

		public static void Decode(this IDictionary<string, string> self, string encodedDictionary, DictionaryExtension.Encoder decoder)
		{
			self.Decode(encodedDictionary, '&', '=', decoder, decoder, false);
		}

		public static void Decode(this IDictionary<string, string> self, string encodedDictionary, char separator, char keyValueSplitter, bool endsWithSeparator)
		{
			self.Decode(encodedDictionary, separator, keyValueSplitter, DictionaryExtension.DefaultDecoder, DictionaryExtension.DefaultDecoder, endsWithSeparator);
		}

		public static void Decode(this IDictionary<string, string> self, string encodedDictionary, char separator, char keyValueSplitter, DictionaryExtension.Encoder keyDecoder, DictionaryExtension.Encoder valueDecoder, bool endsWithSeparator)
		{
			if (encodedDictionary == null)
			{
				throw new ArgumentNullException("encodedDictionary");
			}
			if (keyDecoder == null)
			{
				throw new ArgumentNullException("keyDecoder");
			}
			if (valueDecoder == null)
			{
				throw new ArgumentNullException("valueDecoder");
			}
			if (endsWithSeparator && encodedDictionary.LastIndexOf(separator) == encodedDictionary.Length - 1)
			{
				encodedDictionary = encodedDictionary.Substring(0, encodedDictionary.Length - 1);
			}
			foreach (string text in encodedDictionary.Split(new char[]
			{
				separator
			}))
			{
				string[] array2 = text.Split(new char[]
				{
					keyValueSplitter
				});
				if ((array2.Length == 1 || array2.Length > 2) && !string.IsNullOrEmpty(array2[0]))
				{
					throw new ArgumentException("The request is not properly formatted.", "encodedDictionary");
				}
				if (array2.Length != 2)
				{
					throw new ArgumentException("The request is not properly formatted.", "encodedDictionary");
				}
				string text2 = keyDecoder(array2[0].Trim());
				string value = valueDecoder(array2[1].Trim().Trim(new char[]
				{
					'"'
				}));
				try
				{
					self.Add(text2, value);
				}
				catch (ArgumentException)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "The request is not properly formatted. The parameter '{0}' is duplicated.", new object[]
					{
						text2
					});
					throw new ArgumentException(message, "encodedDictionary");
				}
			}
		}

		public static void DecodeFromJson(this IDictionary<string, string> self, string encodedDictionary)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			Dictionary<string, object> dictionary = javaScriptSerializer.DeserializeObject(encodedDictionary) as Dictionary<string, object>;
			if (dictionary == null)
			{
				throw new ArgumentException("Invalid request format.", "encodedDictionary");
			}
			foreach (KeyValuePair<string, object> keyValuePair in dictionary)
			{
				if (keyValuePair.Value == null)
				{
					self.Add(keyValuePair.Key, null);
				}
				else if (keyValuePair.Value is object[])
				{
					self.Add(keyValuePair.Key, javaScriptSerializer.Serialize(keyValuePair.Value));
				}
				else
				{
					self.Add(keyValuePair.Key, keyValuePair.Value.ToString());
				}
			}
		}

		public static string Encode(this IDictionary<string, string> self)
		{
			return self.Encode('&', '=', DictionaryExtension.DefaultEncoder, DictionaryExtension.DefaultEncoder, false);
		}

		public static string Encode(this IDictionary<string, string> self, DictionaryExtension.Encoder encoder)
		{
			return self.Encode('&', '=', encoder, encoder, false);
		}

		public static string Encode(this IDictionary<string, string> self, char separator, char keyValueSplitter, bool endsWithSeparator)
		{
			return self.Encode(separator, keyValueSplitter, DictionaryExtension.DefaultEncoder, DictionaryExtension.DefaultEncoder, endsWithSeparator);
		}

		public static string Encode(this IDictionary<string, string> self, char separator, char keyValueSplitter, DictionaryExtension.Encoder keyEncoder, DictionaryExtension.Encoder valueEncoder, bool endsWithSeparator)
		{
			if (keyEncoder == null)
			{
				throw new ArgumentNullException("keyEncoder");
			}
			if (valueEncoder == null)
			{
				throw new ArgumentNullException("valueEncoder");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in self)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(separator);
				}
				stringBuilder.AppendFormat("{0}{1}{2}", keyEncoder(keyValuePair.Key), keyValueSplitter, valueEncoder(keyValuePair.Value));
			}
			if (endsWithSeparator)
			{
				stringBuilder.Append(separator);
			}
			return stringBuilder.ToString();
		}

		public static string EncodeToJson(this IDictionary<string, string> self)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			return javaScriptSerializer.Serialize(self);
		}

		public const char DefaultSeparator = '&';

		public const char DefaultKeyValueSeparator = '=';

		public static DictionaryExtension.Encoder DefaultDecoder = new DictionaryExtension.Encoder(HttpUtility.UrlDecode);

		public static DictionaryExtension.Encoder DefaultEncoder = new DictionaryExtension.Encoder(HttpUtility.UrlEncode);

		public static DictionaryExtension.Encoder NullEncoder = new DictionaryExtension.Encoder(DictionaryExtension.NullEncode);

		public delegate string Encoder(string input);
	}
}
