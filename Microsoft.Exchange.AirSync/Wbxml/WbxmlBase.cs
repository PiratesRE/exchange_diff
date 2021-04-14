using System;

namespace Microsoft.Exchange.AirSync.Wbxml
{
	internal class WbxmlBase
	{
		public static WbxmlSchema Schema
		{
			get
			{
				return WbxmlBase.wbxmlSchema;
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

		private static WbxmlSchema wbxmlSchema = new WbxmlSchema30();
	}
}
