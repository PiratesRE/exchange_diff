using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncExDateTimeProperty : AirSyncProperty, IDateTimeProperty, IProperty
	{
		public AirSyncExDateTimeProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public ExDateTime DateTime
		{
			get
			{
				return TimeZoneConverter.Parse(base.XmlNode.InnerText, "AirSyncExDateTime");
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IDateTimeProperty dateTimeProperty = srcProperty as IDateTimeProperty;
			if (dateTimeProperty == null)
			{
				throw new UnexpectedTypeException("IDateTimeProperty", srcProperty);
			}
			base.CreateAirSyncNode(TimeZoneConverter.ToString(dateTimeProperty.DateTime));
		}
	}
}
