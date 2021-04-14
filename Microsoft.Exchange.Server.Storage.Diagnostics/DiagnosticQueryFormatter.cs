using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public abstract class DiagnosticQueryFormatter<T>
	{
		protected DiagnosticQueryFormatter(DiagnosticQueryResults results)
		{
			this.results = results;
		}

		protected DiagnosticQueryResults Results
		{
			get
			{
				return this.results;
			}
		}

		public abstract T FormatResults();

		protected static string FormatValue(object value)
		{
			if (value is byte[])
			{
				return string.Format("0x{0}", BitConverter.ToString((byte[])value).Replace("-", string.Empty));
			}
			if (value is DateTime)
			{
				return ((DateTime)value).ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff");
			}
			if (value != null)
			{
				return value.ToString();
			}
			return "NULL";
		}

		protected static void WriteValue(XElement element, object value)
		{
			Array array = value as Array;
			if (array == null || value is byte[])
			{
				element.Value = DiagnosticQueryFormatter<T>.FormatValue(value);
				return;
			}
			DiagnosticQueryFormatter<T>.WriteIndexedElements(element, "Item", array);
		}

		protected static void WriteIndexedElements(XElement parent, string name, Array collection)
		{
			for (int i = 0; i < collection.Length; i++)
			{
				XElement xelement = new XElement(name);
				DiagnosticQueryFormatter<T>.WriteValue(xelement, collection.GetValue(i));
				xelement.Add(new XAttribute("Index", i));
				parent.Add(xelement);
			}
		}

		private readonly DiagnosticQueryResults results;
	}
}
