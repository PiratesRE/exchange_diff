using System;
using System.IO;
using System.Text;
using Microsoft.Mapi;

namespace Microsoft.Exchange.OAB
{
	internal sealed class OABPropertyValue
	{
		public PropTag PropTag { get; set; }

		public object Value { get; set; }

		public bool IsWritable
		{
			get
			{
				PropTypeHandler handler = PropTypeHandler.GetHandler(this.PropTag.ValueType());
				return handler.IsWritable;
			}
		}

		public static OABPropertyValue ReadFrom(BinaryReader reader, PropTag propTag, string elementName)
		{
			PropTypeHandler handler = PropTypeHandler.GetHandler(propTag.ValueType());
			object value = handler.ReadFrom(reader, elementName);
			return new OABPropertyValue
			{
				PropTag = propTag,
				Value = value
			};
		}

		public void WriteTo(BinaryWriter writer)
		{
			PropTypeHandler handler = PropTypeHandler.GetHandler(this.PropTag.ValueType());
			handler.WriteTo(writer, this.Value);
		}

		public override string ToString()
		{
			PropTypeHandler handler = PropTypeHandler.GetHandler(this.PropTag.ValueType());
			int num = 1;
			Array array = this.Value as Array;
			if (array != null)
			{
				num = array.Length;
			}
			StringBuilder stringBuilder = new StringBuilder(20 * num);
			stringBuilder.Append("PropTag=");
			stringBuilder.Append(this.PropTag.ToString("X8"));
			stringBuilder.Append(", Value=");
			handler.AppendText(stringBuilder, this.Value);
			return stringBuilder.ToString();
		}
	}
}
