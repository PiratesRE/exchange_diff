using System;
using System.Xml.Linq;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class DiagnosticQueryXmlFormatter : DiagnosticQueryFormatter<XElement>
	{
		private DiagnosticQueryXmlFormatter(DiagnosticQueryResults results) : base(results)
		{
		}

		public static DiagnosticQueryXmlFormatter Create(DiagnosticQueryResults results)
		{
			return new DiagnosticQueryXmlFormatter(results);
		}

		public static XElement FormatException(DiagnosticQueryException e)
		{
			DiagnosticQueryResults results = DiagnosticQueryResults.Create(new string[]
			{
				e.GetType().Name
			}, new Type[]
			{
				e.Message.GetType()
			}, new uint[]
			{
				(uint)e.Message.Length
			}, new object[][]
			{
				new object[]
				{
					e.Message
				}
			}, false, false);
			return DiagnosticQueryXmlFormatter.WriteResults(results);
		}

		public override XElement FormatResults()
		{
			return DiagnosticQueryXmlFormatter.WriteResults(base.Results);
		}

		private static XElement WriteResults(DiagnosticQueryResults results)
		{
			XElement xelement = DiagnosticQueryXmlFormatter.WriteColumns(results);
			XElement xelement2 = DiagnosticQueryXmlFormatter.WriteRows(results);
			XAttribute xattribute = new XAttribute("Truncated", results.IsTruncated);
			XAttribute xattribute2 = new XAttribute("Interrupted", results.IsInterrupted);
			return new XElement("Results", new object[]
			{
				xattribute,
				xattribute2,
				xelement,
				xelement2
			});
		}

		private static XElement WriteColumns(DiagnosticQueryResults results)
		{
			XElement xelement = new XElement("Columns");
			for (int i = 0; i < results.Names.Count; i++)
			{
				XElement xelement2 = new XElement("Column");
				xelement2.Add(new XAttribute("Index", i));
				xelement2.Add(new XAttribute("Name", results.Names[i]));
				xelement2.Add(new XAttribute("Type", results.Types[i].FullName));
				xelement.Add(xelement2);
			}
			return xelement;
		}

		private static XElement WriteRows(DiagnosticQueryResults results)
		{
			XElement xelement = new XElement("Rows");
			foreach (object[] collection in results.Values)
			{
				XElement xelement2 = new XElement("Row");
				DiagnosticQueryFormatter<XElement>.WriteIndexedElements(xelement2, "Value", collection);
				xelement.Add(xelement2);
			}
			return xelement;
		}
	}
}
