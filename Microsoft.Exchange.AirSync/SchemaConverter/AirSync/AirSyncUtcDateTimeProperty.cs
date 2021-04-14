using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncUtcDateTimeProperty : AirSyncProperty, IDateTimeProperty, IProperty
	{
		public AirSyncUtcDateTimeProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public AirSyncUtcDateTimeProperty(string xmlNodeNamespace, string airSyncTagName, AirSyncDateFormat format, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
			this.format = format;
		}

		public ExDateTime DateTime
		{
			get
			{
				ExDateTime result;
				if (!ExDateTime.TryParseExact(base.XmlNode.InnerText, AirSyncUtcDateTimeProperty.formatString[(int)this.format], DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidDateTimeInAirSyncUtcDateTime"
					};
				}
				return result;
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IDateTimeProperty dateTimeProperty = srcProperty as IDateTimeProperty;
			if (dateTimeProperty == null)
			{
				throw new UnexpectedTypeException("IDateTimeProperty", srcProperty);
			}
			base.CreateAirSyncNode(dateTimeProperty.DateTime.ToString(AirSyncUtcDateTimeProperty.formatString[(int)this.format], DateTimeFormatInfo.InvariantInfo));
		}

		private static readonly string[] formatString = new string[]
		{
			"yyyyMMdd\\THHmmss\\Z",
			"yyyy-MM-dd\\THH:mm:ss.fff\\Z"
		};

		private AirSyncDateFormat format;
	}
}
