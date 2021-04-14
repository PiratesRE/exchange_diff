using System;
using System.Globalization;
using System.Net;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	[Serializable]
	internal class AirSyncStartDueDateProperty : AirSyncProperty, IStartDueDateProperty, IProperty
	{
		public AirSyncStartDueDateProperty(string xmlNodeNamespace) : base(xmlNodeNamespace, new string[]
		{
			"UtcStartDate",
			"StartDate",
			"UtcDueDate",
			"DueDate"
		}, true)
		{
		}

		public ExDateTime? DueDate
		{
			get
			{
				if (!this.datesParsed)
				{
					this.ParseDates();
				}
				return this.dates[3];
			}
		}

		public ExDateTime? StartDate
		{
			get
			{
				if (!this.datesParsed)
				{
					this.ParseDates();
				}
				return this.dates[1];
			}
		}

		public ExDateTime? UtcDueDate
		{
			get
			{
				if (!this.datesParsed)
				{
					this.ParseDates();
				}
				return this.dates[2];
			}
		}

		public ExDateTime? UtcStartDate
		{
			get
			{
				if (!this.datesParsed)
				{
					this.ParseDates();
				}
				return this.dates[0];
			}
		}

		public override void Unbind()
		{
			base.Unbind();
			this.datesParsed = false;
			this.dates = null;
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IStartDueDateProperty startDueDateProperty = srcProperty as IStartDueDateProperty;
			if (startDueDateProperty == null)
			{
				throw new UnexpectedTypeException("IStartDueDateProperty", srcProperty);
			}
			if (startDueDateProperty.UtcStartDate != null)
			{
				base.CreateAirSyncNode(base.AirSyncTagNames[0], startDueDateProperty.UtcStartDate.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo));
			}
			if (startDueDateProperty.StartDate != null)
			{
				base.CreateAirSyncNode(base.AirSyncTagNames[1], startDueDateProperty.StartDate.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo));
			}
			if (startDueDateProperty.UtcDueDate != null)
			{
				base.CreateAirSyncNode(base.AirSyncTagNames[2], startDueDateProperty.UtcDueDate.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo));
			}
			if (startDueDateProperty.DueDate != null)
			{
				base.CreateAirSyncNode(base.AirSyncTagNames[3], startDueDateProperty.DueDate.Value.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo));
			}
		}

		private void ParseDates()
		{
			this.dates = new ExDateTime?[4];
			for (int i = 0; i < this.dates.Length; i++)
			{
				XmlNode xmlNode = base.XmlNode.ParentNode[base.AirSyncTagNames[i]];
				if (xmlNode != null)
				{
					ExDateTime value;
					if (!ExDateTime.TryParseExact(xmlNode.InnerText, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out value))
					{
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
						{
							ErrorStringForProtocolLogger = "InvalidDateTimeInAirSyncStartDueDate"
						};
					}
					this.dates[i] = new ExDateTime?(value);
				}
			}
			this.datesParsed = true;
		}

		private ExDateTime?[] dates;

		private bool datesParsed;
	}
}
