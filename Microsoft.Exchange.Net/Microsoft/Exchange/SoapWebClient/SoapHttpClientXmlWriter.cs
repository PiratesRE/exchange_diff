using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.SoapWebClient
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoapHttpClientXmlWriter : XmlWriter
	{
		public SoapHttpClientXmlWriter(XmlWriter writer, IEnumerable<XmlNamespaceDefinition> namespaceDefinitions)
		{
			if (namespaceDefinitions == null)
			{
				throw new ArgumentNullException("namespaceDefinitions");
			}
			this.innerWriter = writer;
			this.namespaceDefinitions = namespaceDefinitions;
		}

		public override void Close()
		{
			this.innerWriter.Close();
		}

		public override void Flush()
		{
			this.innerWriter.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			return this.innerWriter.LookupPrefix(ns);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.innerWriter.WriteBase64(buffer, index, count);
		}

		public override void WriteCData(string text)
		{
			this.innerWriter.WriteCData(text);
		}

		public override void WriteCharEntity(char ch)
		{
			this.innerWriter.WriteCharEntity(ch);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.innerWriter.WriteChars(buffer, index, count);
		}

		public override void WriteComment(string text)
		{
			this.innerWriter.WriteComment(text);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.innerWriter.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteEndAttribute()
		{
			this.innerWriter.WriteEndAttribute();
		}

		public override void WriteEndDocument()
		{
			this.innerWriter.WriteEndDocument();
		}

		public override void WriteEndElement()
		{
			this.innerWriter.WriteEndElement();
		}

		public override void WriteEntityRef(string name)
		{
			this.innerWriter.WriteEntityRef(name);
		}

		public override void WriteFullEndElement()
		{
			this.innerWriter.WriteFullEndElement();
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.innerWriter.WriteProcessingInstruction(name, text);
		}

		public override void WriteRaw(string data)
		{
			this.innerWriter.WriteRaw(data);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.innerWriter.WriteRaw(buffer, index, count);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.innerWriter.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.innerWriter.WriteStartDocument(standalone);
		}

		public override void WriteStartDocument()
		{
			this.innerWriter.WriteStartDocument();
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.innerWriter.WriteStartElement(prefix, localName, ns);
			if (!this.haveWrittenFirstElement)
			{
				foreach (XmlNamespaceDefinition xmlNamespaceDefinition in this.namespaceDefinitions)
				{
					xmlNamespaceDefinition.WriteAttribute(this);
				}
				this.haveWrittenFirstElement = true;
			}
		}

		public override WriteState WriteState
		{
			get
			{
				return this.innerWriter.WriteState;
			}
		}

		public override void WriteString(string text)
		{
			this.innerWriter.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.innerWriter.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteWhitespace(string ws)
		{
			this.innerWriter.WriteWhitespace(ws);
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return this.innerWriter.Settings;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.innerWriter.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.innerWriter.XmlSpace;
			}
		}

		private IEnumerable<XmlNamespaceDefinition> namespaceDefinitions;

		private XmlWriter innerWriter;

		private bool haveWrittenFirstElement;
	}
}
