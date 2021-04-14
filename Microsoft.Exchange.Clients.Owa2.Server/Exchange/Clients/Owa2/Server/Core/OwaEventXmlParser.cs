using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OwaEventXmlParser : OwaEventParserBase
	{
		internal OwaEventXmlParser(OwaEventHandlerBase eventHandler) : base(eventHandler, 4)
		{
		}

		protected override void ThrowParserException(string description)
		{
			int num = 0;
			int num2 = 0;
			if (this.reader != null)
			{
				num = this.reader.LineNumber;
				num2 = this.reader.LinePosition;
			}
			Stream inputStream = base.EventHandler.HttpContext.Request.InputStream;
			string text;
			using (StreamReader streamReader = new StreamReader(inputStream))
			{
				text = streamReader.ReadToEnd();
			}
			throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "Invalid request. Line number: {0} Position: {1}. Url: {2}. Request body: {3}. {4}", new object[]
			{
				num.ToString(CultureInfo.InvariantCulture),
				num2.ToString(CultureInfo.InvariantCulture),
				base.EventHandler.HttpContext.Request.RawUrl,
				text,
				(description != null) ? (" " + description) : string.Empty
			}), null, this);
		}

		protected override Hashtable ParseParameters()
		{
			Stream inputStream = base.EventHandler.HttpContext.Request.InputStream;
			if (inputStream.Length <= 0L)
			{
				return base.ParameterTable;
			}
			try
			{
				this.reader = new XmlTextReader(inputStream);
				this.reader.WhitespaceHandling = WhitespaceHandling.All;
				this.paramInfo = null;
				this.itemArray = null;
				this.state = OwaEventXmlParser.XmlParseState.Start;
				while (this.state != OwaEventXmlParser.XmlParseState.Finished && this.reader.Read())
				{
					switch (this.state)
					{
					case OwaEventXmlParser.XmlParseState.Start:
						this.ParseStart();
						break;
					case OwaEventXmlParser.XmlParseState.Root:
						this.ParseRoot();
						break;
					case OwaEventXmlParser.XmlParseState.Child:
						this.ParseChild();
						break;
					case OwaEventXmlParser.XmlParseState.ChildText:
						this.ParseChildText();
						break;
					case OwaEventXmlParser.XmlParseState.ChildEnd:
						this.ParseChildEnd();
						break;
					case OwaEventXmlParser.XmlParseState.Item:
						this.ParseItem();
						break;
					case OwaEventXmlParser.XmlParseState.ItemText:
						this.ParseItemText();
						break;
					case OwaEventXmlParser.XmlParseState.ItemEnd:
						this.ParseItemEnd();
						break;
					}
				}
			}
			catch (XmlException ex)
			{
				ExTraceGlobals.OehTracer.TraceDebug<string>(0L, "Parser threw an XML exception: {0}'", ex.Message);
				throw new OwaInvalidRequestException(ex.Message, ex, this);
			}
			finally
			{
				this.reader.Close();
			}
			return base.ParameterTable;
		}

		private void ParseStart()
		{
			if (XmlNodeType.Element != this.reader.NodeType || !string.Equals("params", this.reader.Name, StringComparison.OrdinalIgnoreCase))
			{
				base.ThrowParserException();
				return;
			}
			if (this.reader.IsEmptyElement)
			{
				this.state = OwaEventXmlParser.XmlParseState.Finished;
				return;
			}
			this.state = OwaEventXmlParser.XmlParseState.Root;
		}

		private void ParseRoot()
		{
			if (this.reader.NodeType == XmlNodeType.Element)
			{
				this.paramInfo = base.GetParamInfo(this.reader.Name);
				if (this.reader.IsEmptyElement)
				{
					base.AddEmptyParameter(this.paramInfo);
					this.state = OwaEventXmlParser.XmlParseState.ChildEnd;
					this.paramInfo = null;
					return;
				}
				this.state = OwaEventXmlParser.XmlParseState.Child;
				return;
			}
			else
			{
				if (this.reader.NodeType == XmlNodeType.EndElement && string.Equals("params", this.reader.Name, StringComparison.OrdinalIgnoreCase))
				{
					this.state = OwaEventXmlParser.XmlParseState.Finished;
					return;
				}
				base.ThrowParserException();
				return;
			}
		}

		private void ParseChild()
		{
			if (this.reader.NodeType == XmlNodeType.Text)
			{
				if (this.paramInfo.IsArray)
				{
					base.ThrowParserException();
				}
				base.AddSimpleTypeParameter(this.paramInfo, this.reader.Value);
				this.state = OwaEventXmlParser.XmlParseState.ChildText;
				return;
			}
			if (this.reader.NodeType == XmlNodeType.Whitespace)
			{
				if (this.reader.Value == null)
				{
					base.ThrowParserException();
				}
				base.AddSimpleTypeParameter(this.paramInfo, this.reader.Value);
				this.state = OwaEventXmlParser.XmlParseState.ChildText;
				return;
			}
			if (this.reader.NodeType == XmlNodeType.EndElement && string.Equals(this.reader.Name, this.paramInfo.Name, StringComparison.OrdinalIgnoreCase))
			{
				base.AddEmptyParameter(this.paramInfo);
				this.state = OwaEventXmlParser.XmlParseState.ChildEnd;
				this.paramInfo = null;
				return;
			}
			if (this.reader.NodeType == XmlNodeType.Element)
			{
				if (!this.paramInfo.IsArray)
				{
					this.ThrowParserException("Array expected");
				}
				if (string.Equals(this.reader.Name, "item", StringComparison.OrdinalIgnoreCase))
				{
					this.itemArray = new ArrayList();
					if (this.reader.IsEmptyElement)
					{
						base.AddEmptyItemToArray(this.paramInfo, this.itemArray);
						this.state = OwaEventXmlParser.XmlParseState.ItemEnd;
						return;
					}
					this.state = OwaEventXmlParser.XmlParseState.Item;
					return;
				}
			}
			else
			{
				base.ThrowParserException();
			}
		}

		private void ParseChildText()
		{
			if (this.reader.NodeType == XmlNodeType.EndElement && string.Equals(this.reader.Name, this.paramInfo.Name, StringComparison.OrdinalIgnoreCase))
			{
				this.state = OwaEventXmlParser.XmlParseState.ChildEnd;
				this.paramInfo = null;
				return;
			}
			base.ThrowParserException();
		}

		private void ParseChildEnd()
		{
			if (this.reader.NodeType == XmlNodeType.EndElement && string.Equals("params", this.reader.Name, StringComparison.OrdinalIgnoreCase))
			{
				this.state = OwaEventXmlParser.XmlParseState.Finished;
				return;
			}
			if (this.reader.NodeType != XmlNodeType.Element)
			{
				base.ThrowParserException();
				return;
			}
			this.paramInfo = base.GetParamInfo(this.reader.Name);
			if (this.reader.IsEmptyElement)
			{
				base.AddEmptyParameter(this.paramInfo);
				this.state = OwaEventXmlParser.XmlParseState.ChildEnd;
				this.paramInfo = null;
				return;
			}
			this.state = OwaEventXmlParser.XmlParseState.Child;
		}

		private void ParseItem()
		{
			if (this.reader.NodeType == XmlNodeType.Text)
			{
				base.AddSimpleTypeToArray(this.paramInfo, this.itemArray, this.reader.Value);
				this.state = OwaEventXmlParser.XmlParseState.ItemText;
				return;
			}
			if (this.reader.NodeType == XmlNodeType.Whitespace)
			{
				base.AddSimpleTypeToArray(this.paramInfo, this.itemArray, this.reader.Value);
				this.state = OwaEventXmlParser.XmlParseState.ItemText;
				return;
			}
			if (this.reader.NodeType == XmlNodeType.EndElement && string.Equals(this.reader.Name, "item", StringComparison.OrdinalIgnoreCase))
			{
				base.AddEmptyItemToArray(this.paramInfo, this.itemArray);
				this.state = OwaEventXmlParser.XmlParseState.ItemEnd;
				return;
			}
			base.ThrowParserException();
		}

		private void ParseItemText()
		{
			if (this.reader.NodeType == XmlNodeType.EndElement && string.Equals(this.reader.Name, "item", StringComparison.OrdinalIgnoreCase))
			{
				this.state = OwaEventXmlParser.XmlParseState.ItemEnd;
				return;
			}
			base.ThrowParserException();
		}

		private void ParseItemEnd()
		{
			if (this.reader.NodeType == XmlNodeType.Element && string.Equals(this.reader.Name, "item", StringComparison.OrdinalIgnoreCase))
			{
				if (this.reader.IsEmptyElement)
				{
					base.AddEmptyItemToArray(this.paramInfo, this.itemArray);
					this.state = OwaEventXmlParser.XmlParseState.ItemEnd;
					return;
				}
				this.state = OwaEventXmlParser.XmlParseState.Item;
				return;
			}
			else
			{
				if (this.reader.NodeType == XmlNodeType.EndElement && string.Equals(this.reader.Name, this.paramInfo.Name, StringComparison.OrdinalIgnoreCase))
				{
					base.AddArrayParameter(this.paramInfo, this.itemArray);
					this.state = OwaEventXmlParser.XmlParseState.ChildEnd;
					this.itemArray = null;
					this.paramInfo = null;
					return;
				}
				base.ThrowParserException();
				return;
			}
		}

		internal const string RequestBodyItemName = "item";

		internal const string RequestBodyParamsName = "params";

		private OwaEventParameterAttribute paramInfo;

		private ArrayList itemArray;

		private OwaEventXmlParser.XmlParseState state;

		private XmlTextReader reader;

		private enum XmlParseState
		{
			Start,
			Root,
			Child,
			ChildText,
			ChildEnd,
			Item,
			ItemText,
			ItemEnd,
			Finished
		}
	}
}
