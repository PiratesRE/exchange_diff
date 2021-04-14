using System;
using System.Collections;
using System.Text;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class LiveIdTicketDictionary
	{
		public LiveIdTicketDictionary(string ticket)
		{
			if (ticket == null)
			{
				throw new ArgumentNullException("ticket");
			}
			string[] array = ticket.Split(new char[]
			{
				'&'
			});
			this.values = new Hashtable(array.Length);
			int i = 0;
			while (i < array.Length - 3)
			{
				string key = array[i++];
				string text = array[i++];
				string text2 = LiveIdTicketDictionary.ReadEscapedString(array, ref i);
				string a;
				if ((a = text) != null)
				{
					if (!(a == "string"))
					{
						uint num2;
						if (!(a == "UInt32"))
						{
							long num;
							if (!(a == "Int64"))
							{
								byte b;
								if (!(a == "Byte"))
								{
									if (a == "DateTime")
									{
										DateTime dateTime;
										if (DateTime.TryParse(text2, out dateTime))
										{
											this.values[key] = dateTime;
										}
									}
								}
								else if (byte.TryParse(text2, out b))
								{
									this.values[key] = b;
								}
							}
							else if (long.TryParse(text2, out num))
							{
								this.values[key] = num;
							}
						}
						else if (uint.TryParse(text2, out num2))
						{
							this.values[key] = num2;
						}
					}
					else
					{
						this.values[key] = text2;
					}
				}
			}
		}

		public bool TryGetValue(string key, out object value)
		{
			value = null;
			if (this.values.ContainsKey(key))
			{
				value = this.values[key];
				return true;
			}
			return false;
		}

		public bool TryGetValue<T>(string key, out T value)
		{
			value = default(T);
			if (this.values.ContainsKey(key))
			{
				object obj = this.values[key];
				if (obj.GetType() == typeof(T))
				{
					value = (T)((object)obj);
					return true;
				}
			}
			return false;
		}

		private static string ReadEscapedString(string[] splits, ref int index)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (index < splits.Length)
			{
				stringBuilder.Append(splits[index]);
				index++;
				if (index >= splits.Length - 1 || splits[index].Length != 0)
				{
					break;
				}
				stringBuilder.Append('&');
				index++;
			}
			return stringBuilder.ToString();
		}

		private readonly Hashtable values;
	}
}
