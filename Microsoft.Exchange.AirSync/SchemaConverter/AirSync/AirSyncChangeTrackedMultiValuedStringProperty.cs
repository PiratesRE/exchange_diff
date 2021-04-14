using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncChangeTrackedMultiValuedStringProperty : AirSyncMultiValuedStringProperty
	{
		public AirSyncChangeTrackedMultiValuedStringProperty(string xmlNodeNamespace, string airSyncTagName, string airSyncChildTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, airSyncChildTagName, requiresClientSupport)
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
