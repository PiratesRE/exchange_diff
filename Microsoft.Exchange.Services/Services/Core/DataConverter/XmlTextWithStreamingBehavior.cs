using System;
using System.Xml;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class XmlTextWithStreamingBehavior : XmlText
	{
		public XmlTextWithStreamingBehavior(XmlDocument doc, object item, object copyBuffer, WriteStreamData writeData, params object[] parameters) : base("_WS_ToReplace_", doc)
		{
			this.item = item;
			this.copyBuffer = copyBuffer;
			this.writeData = writeData;
			this.parameters = parameters;
		}

		public override void WriteTo(XmlWriter writer)
		{
			this.writeData(writer, this.item, this.copyBuffer, this.parameters);
		}

		private const string TextString = "_WS_ToReplace_";

		private object item;

		private object copyBuffer;

		private WriteStreamData writeData;

		private object[] parameters;
	}
}
