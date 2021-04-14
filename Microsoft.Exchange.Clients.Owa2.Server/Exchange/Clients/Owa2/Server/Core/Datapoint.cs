using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class Datapoint
	{
		public Datapoint(DatapointConsumer consumers, string id, string time, string[] keys, string[] values)
		{
			this.Id = id;
			this.Time = time;
			this.Consumers = consumers;
			this.Keys = keys;
			this.Values = values;
		}

		[DataMember]
		public string Id { get; private set; }

		[DataMember]
		public string Time { get; private set; }

		[DataMember]
		public DatapointConsumer Consumers { get; private set; }

		[DataMember]
		public string[] Keys { get; private set; }

		[DataMember]
		public string[] Values { get; private set; }

		public int Size
		{
			get
			{
				if (this.size == null)
				{
					int num;
					if (this.Keys != null)
					{
						num = this.Keys.Sum(delegate(string key)
						{
							if (key != null)
							{
								return key.Length;
							}
							return 0;
						});
					}
					else
					{
						num = 0;
					}
					int num2 = num;
					int num3;
					if (this.Values != null)
					{
						num3 = this.Values.Sum(delegate(string value)
						{
							if (value != null)
							{
								return value.Length;
							}
							return 0;
						});
					}
					else
					{
						num3 = 0;
					}
					int num4 = num3;
					int length = this.Consumers.ToString().Length;
					int num5 = (this.Id == null) ? 0 : this.Id.Length;
					int num6 = (this.Time == null) ? 0 : this.Time.Length;
					this.size = new int?(num5 + num6 + length + num2 + num4);
				}
				return this.size.Value;
			}
		}

		public bool IsForConsumer(DatapointConsumer consumer)
		{
			return (this.Consumers & consumer) != DatapointConsumer.None;
		}

		public void AppendTo(StringBuilder buffer)
		{
			buffer.AppendFormat("{0},", this.Time);
			buffer.AppendFormat("{0},", Datapoint.GetCsvEscapedString(this.Id));
			buffer.AppendFormat("{0},", Datapoint.GetCsvEscapedString(this.Consumers));
			if (this.Keys == null && this.Values == null)
			{
				return;
			}
			string[] array = this.Keys ?? Datapoint.EmptyStringArray;
			string[] array2 = this.Values ?? Datapoint.EmptyStringArray;
			KeyValuePair<string, string>[] array3 = new KeyValuePair<string, string>[Math.Max(array.Length, array2.Length)];
			for (int i = 0; i < Math.Min(array.Length, array2.Length); i++)
			{
				array3[i] = Datapoint.NewKeyValuePair(array[i], array2[i]);
			}
			if (array.Length > array2.Length)
			{
				for (int j = array3.Length; j < array.Length; j++)
				{
					array3[j] = Datapoint.NewKeyValuePair(array[j], null);
				}
			}
			else
			{
				for (int k = array3.Length; k < array2.Length; k++)
				{
					array3[k] = Datapoint.NewKeyValuePair(null, array2[k]);
				}
			}
			bool flag;
			string text = LogRowFormatter.FormatCollection(array3, true, out flag);
			buffer.Append(flag ? Datapoint.GetCsvEscapedString(text) : text);
		}

		private static KeyValuePair<string, string> NewKeyValuePair(string key, string value)
		{
			return new KeyValuePair<string, string>(key ?? "?", value ?? string.Empty);
		}

		private static string GetCsvEscapedString(object value)
		{
			return Encoding.UTF8.GetString(Utf8Csv.EncodeAndEscape(value.ToString(), true));
		}

		private static readonly string[] EmptyStringArray = new string[0];

		private int? size;
	}
}
