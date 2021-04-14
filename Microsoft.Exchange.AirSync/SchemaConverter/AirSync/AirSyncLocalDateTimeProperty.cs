using System;
using System.Globalization;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncLocalDateTimeProperty : AirSyncProperty, IDateTimeProperty, IProperty
	{
		public AirSyncLocalDateTimeProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public AirSyncLocalDateTimeProperty(string xmlNodeNamespace, string airSyncTagName, AirSyncDateFormat format, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
			this.format = format;
		}

		public ExDateTime DateTime
		{
			get
			{
				return ExDateTime.ParseExact(base.XmlNode.InnerText, AirSyncLocalDateTimeProperty.formatString[(int)this.format], DateTimeFormatInfo.InvariantInfo);
			}
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IDateTimeProperty dateTimeProperty = srcProperty as IDateTimeProperty;
			if (dateTimeProperty == null)
			{
				throw new UnexpectedTypeException("IDateTimeProperty", srcProperty);
			}
			base.CreateAirSyncNode(dateTimeProperty.DateTime.ToString(AirSyncLocalDateTimeProperty.formatString[(int)this.format], DateTimeFormatInfo.InvariantInfo));
		}

		private static readonly string[] formatString = new string[]
		{
			"yyyyMMdd\\THHmmss",
			"yyyy-MM-dd\\THH:mm:ss.fff"
		};

		private AirSyncDateFormat format;
	}
}
