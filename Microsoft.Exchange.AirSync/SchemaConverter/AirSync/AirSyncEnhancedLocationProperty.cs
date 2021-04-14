using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncEnhancedLocationProperty : AirSyncNestedProperty
	{
		public AirSyncEnhancedLocationProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport, int protocolVersion) : base(xmlNodeNamespace, airSyncTagName, new EnhancedLocationData(protocolVersion), requiresClientSupport)
		{
		}
	}
}
