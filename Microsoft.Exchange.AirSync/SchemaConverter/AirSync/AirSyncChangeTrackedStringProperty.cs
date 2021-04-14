using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncChangeTrackedStringProperty : AirSyncStringProperty
	{
		public AirSyncChangeTrackedStringProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
			base.ClientChangeTracked = true;
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			base.XmlNode = base.XmlParentNode.OwnerDocument.CreateElement(base.AirSyncTagNames[0], base.Namespace);
			base.XmlParentNode.AppendChild(base.XmlNode);
		}
	}
}
