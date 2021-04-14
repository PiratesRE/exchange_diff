using System;
using System.IO;
using System.Security;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class RequestBody
	{
		private RequestBody(string bodyFormat, RequestBody.RequestBodyItemWrapper[] parameters)
		{
			this.bodyFormat = bodyFormat;
			this.parameters = parameters;
		}

		public static RequestBody Format(string bodyFormat, params object[] parameters)
		{
			RequestBody.RequestBodyItemWrapper[] array = null;
			if (parameters != null && parameters.Length > 0)
			{
				array = new RequestBody.RequestBodyItemWrapper[parameters.Length];
				int num = 0;
				foreach (object wrappedObject in parameters)
				{
					array[num] = RequestBody.RequestBodyItemWrapper.Create(wrappedObject);
					num++;
				}
			}
			return new RequestBody(bodyFormat, array);
		}

		public override string ToString()
		{
			if (this.parameters != null)
			{
				return string.Format(this.bodyFormat, this.parameters);
			}
			return this.bodyFormat;
		}

		public void Write(Stream stream)
		{
			using (StreamWriter streamWriter = new StreamWriter(stream))
			{
				string text = this.ToString();
				string[] array = text.Split(new string[]
				{
					"<...>"
				}, StringSplitOptions.None);
				int i = 0;
				foreach (string value in array)
				{
					streamWriter.Write(value);
					if (this.parameters != null)
					{
						while (i < this.parameters.Length)
						{
							if (this.parameters[i].Value is SecureString)
							{
								this.parameters[i].Write(streamWriter);
								i++;
								break;
							}
							i++;
						}
					}
				}
			}
		}

		private readonly string bodyFormat;

		private RequestBody.RequestBodyItemWrapper[] parameters;

		internal class RequestBodyItemWrapper
		{
			private RequestBodyItemWrapper()
			{
			}

			public object Value { get; private set; }

			public bool UrlEncode { get; private set; }

			public static RequestBody.RequestBodyItemWrapper Create(object wrappedObject)
			{
				if (wrappedObject is RequestBody.RequestBodyItemWrapper)
				{
					return wrappedObject as RequestBody.RequestBodyItemWrapper;
				}
				if (wrappedObject is SecureString)
				{
					return RequestBody.RequestBodyItemWrapper.Create(wrappedObject, true);
				}
				return RequestBody.RequestBodyItemWrapper.Create(wrappedObject, false);
			}

			public static RequestBody.RequestBodyItemWrapper Create(object wrappedObject, bool urlEncode)
			{
				if (wrappedObject is RequestBody.RequestBodyItemWrapper)
				{
					throw new ArgumentException("wrappedObject is already of type RequestBodyItemWrapper");
				}
				return new RequestBody.RequestBodyItemWrapper
				{
					Value = wrappedObject,
					UrlEncode = urlEncode
				};
			}

			public override string ToString()
			{
				if (this.Value is SecureString)
				{
					return "<...>";
				}
				if (this.Value == null)
				{
					return "<null>";
				}
				if (this.UrlEncode)
				{
					return HttpUtility.UrlEncode(this.Value.ToString());
				}
				return this.Value.ToString();
			}

			internal void Write(StreamWriter writer)
			{
				if (this.Value is SecureString)
				{
					byte[] array = null;
					char[] array2 = null;
					try
					{
						array = (this.Value as SecureString).ConvertToByteArray();
						if (this.UrlEncode)
						{
							char[] chars = Encoding.Unicode.GetChars(array);
							byte[] bytes = Encoding.UTF8.GetBytes(chars);
							Array.Clear(array, 0, array.Length);
							Array.Clear(chars, 0, chars.Length);
							array = bytes;
							byte[] array3 = HttpUtility.UrlEncodeToBytes(array);
							Array.Clear(array, 0, array.Length);
							array = array3;
						}
						array2 = Encoding.UTF8.GetChars(array);
						writer.Write(array2);
						return;
					}
					finally
					{
						if (array != null)
						{
							Array.Clear(array, 0, array.Length);
						}
						if (array2 != null)
						{
							Array.Clear(array2, 0, array2.Length);
						}
					}
				}
				if (this.Value != null)
				{
					if (this.UrlEncode)
					{
						writer.Write(HttpUtility.UrlEncode(this.Value.ToString()));
						return;
					}
					writer.Write(this.Value);
				}
			}

			public const string SecureStringReplacement = "<...>";
		}
	}
}
