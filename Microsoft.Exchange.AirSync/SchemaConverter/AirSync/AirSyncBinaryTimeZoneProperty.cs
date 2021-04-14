using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncBinaryTimeZoneProperty : AirSyncProperty, ITimeZoneProperty, IProperty
	{
		public AirSyncBinaryTimeZoneProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public ExTimeZone TimeZone
		{
			get
			{
				byte[] timeZoneInformation = Convert.FromBase64String(base.XmlNode.InnerText);
				return TimeZoneConverter.GetExTimeZone(timeZoneInformation);
			}
		}

		public ExDateTime EffectiveTime
		{
			get
			{
				return ExDateTime.MinValue;
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			ITimeZoneProperty timeZoneProperty = srcProperty as ITimeZoneProperty;
			if (timeZoneProperty == null)
			{
				throw new UnexpectedTypeException("ITimeZoneProperty", srcProperty);
			}
			string strData = Convert.ToBase64String(TimeZoneConverter.GetBytes(timeZoneProperty.TimeZone, timeZoneProperty.EffectiveTime));
			base.CreateAirSyncNode(strData);
		}
	}
}
