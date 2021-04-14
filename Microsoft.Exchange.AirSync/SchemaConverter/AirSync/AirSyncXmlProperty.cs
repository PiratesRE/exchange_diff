using System;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncXmlProperty : AirSyncProperty, IXmlProperty, IProperty
	{
		public AirSyncXmlProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public XmlNode XmlData
		{
			get
			{
				return base.XmlNode;
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
		}
	}
}
