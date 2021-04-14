using System;
using System.Collections.Generic;
using System.Security;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class SecureHtmlFormReader
	{
		public SecureHtmlFormReader(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			this.request = request;
			this.sensitiveKeys = new Dictionary<string, object>();
		}

		public void AddSensitiveInputName(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(key);
			}
			if (!this.sensitiveKeys.ContainsKey(key))
			{
				this.sensitiveKeys.Add(key, null);
			}
		}

		public bool TryReadSecureFormData(out SecureNameValueCollection formCollection)
		{
			bool flag = false;
			formCollection = new SecureNameValueCollection();
			bool result;
			try
			{
				if (string.Compare(this.request.ContentType, 0, "application/x-www-form-urlencoded", 0, "application/x-www-form-urlencoded".Length, StringComparison.OrdinalIgnoreCase) != 0)
				{
					result = false;
				}
				else
				{
					byte[] array = new byte[this.request.ContentLength];
					int num = array.Length;
					for (int i = 0; i < num; i++)
					{
						int num2 = i;
						int num3 = -1;
						while (i < num)
						{
							int num4 = this.request.InputStream.ReadByte();
							if (num4 == -1)
							{
								i = num;
								break;
							}
							array[i] = (byte)num4;
							if (array[i] == 61)
							{
								if (num3 < 0)
								{
									num3 = i;
								}
							}
							else if (array[i] == 38)
							{
								break;
							}
							i++;
						}
						string text;
						int offset;
						int count;
						if (num3 >= 0)
						{
							text = HttpUtility.UrlDecode(array, num2, num3 - num2, this.request.ContentEncoding);
							offset = num3 + 1;
							count = i - num3 - 1;
						}
						else
						{
							text = string.Empty;
							offset = num2;
							count = i - num2;
						}
						if (this.sensitiveKeys.ContainsKey(text))
						{
							SecureString secureString;
							using (SecureArray<byte> secureArray = new SecureArray<byte>(HttpUtility.UrlDecodeToBytes(array, offset, count)))
							{
								using (SecureArray<char> secureArray2 = new SecureArray<char>(this.request.ContentEncoding.GetChars(secureArray.ArrayValue)))
								{
									try
									{
										secureString = secureArray2.ArrayValue.ConvertToSecureString();
									}
									catch (ArgumentOutOfRangeException)
									{
										return false;
									}
								}
							}
							if (formCollection.ContainsSecureValue(text))
							{
								secureString.Dispose();
								return false;
							}
							formCollection.AddSecureNameValue(text, secureString);
						}
						else
						{
							string value = HttpUtility.UrlDecode(array, offset, count, this.request.ContentEncoding);
							if (formCollection.ContainsUnsecureValue(text))
							{
								return false;
							}
							formCollection.AddUnsecureNameValue(text, value);
						}
					}
					flag = true;
					result = true;
				}
			}
			finally
			{
				if (!flag && formCollection != null)
				{
					formCollection.Dispose();
					formCollection = null;
				}
			}
			return result;
		}

		private const string RegularUrlEncodedForm = "application/x-www-form-urlencoded";

		private HttpRequest request;

		private Dictionary<string, object> sensitiveKeys;
	}
}
