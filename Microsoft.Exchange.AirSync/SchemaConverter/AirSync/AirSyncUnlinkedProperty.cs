using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncUnlinkedProperty : AirSyncProperty, IUnlinkedProperty, IProperty
	{
		public AirSyncUnlinkedProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
		}
	}
}
