using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncBooleanProperty : AirSyncProperty, IBooleanProperty, IProperty
	{
		public AirSyncBooleanProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public bool BooleanData
		{
			get
			{
				string innerText;
				if ((innerText = base.XmlNode.InnerText) != null)
				{
					if (innerText == "0")
					{
						return false;
					}
					if (innerText == "1")
					{
						return true;
					}
				}
				throw new ConversionException("Incorrectly-formatted boolean");
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IBooleanProperty booleanProperty = srcProperty as IBooleanProperty;
			if (booleanProperty == null)
			{
				throw new UnexpectedTypeException("IBooleanProperty", srcProperty);
			}
			if (booleanProperty.BooleanData)
			{
				base.CreateAirSyncNode("1");
				return;
			}
			base.CreateAirSyncNode("0");
		}
	}
}
