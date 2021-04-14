using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.WBXml
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class WBXmlBase
	{
		internal static WBXmlSchema Schema
		{
			get
			{
				return WBXmlBase.WBXmlSchema;
			}
		}

		protected const byte AttributeBit = 128;

		protected const int AttributeBitMask = 255;

		protected const byte ContentBit = 64;

		protected const byte ContinuationBit = 128;

		protected const byte MinimumTagValue = 5;

		protected const int NamespaceBitMask = 65280;

		protected const byte NumberBitMask = 127;

		protected const byte ValidTagBitMask = 63;

		private static readonly WBXmlSchema WBXmlSchema = new WBXmlSchema30();
	}
}
