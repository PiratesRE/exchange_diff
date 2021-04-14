using System;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport
{
	internal class Exch50
	{
		protected static byte[] FilterTransportProperties(byte[] data, Exch50.TransportPropertyFilter filter)
		{
			TransportPropertyReader transportPropertyReader = new TransportPropertyReader(data, 0, data.Length);
			TransportPropertyWriter transportPropertyWriter = new TransportPropertyWriter();
			while (transportPropertyReader.ReadNextProperty())
			{
				byte[] array = transportPropertyReader.Value;
				if (transportPropertyReader.Range == Exch50.ExchangeMailMsgPropertyRange)
				{
					array = filter((ExchangeMailMsgProperty)transportPropertyReader.Id, array);
				}
				if (array != null)
				{
					transportPropertyWriter.Add(transportPropertyReader.Range, transportPropertyReader.Id, transportPropertyReader.Value);
				}
			}
			if (transportPropertyWriter.Length == 0)
			{
				return null;
			}
			return transportPropertyWriter.GetBytes();
		}

		protected static void CopyJournalProperties(byte[] data, TransportMailItem mailItem)
		{
			TransportPropertyReader transportPropertyReader = new TransportPropertyReader(data, 0, data.Length);
			LegacyJournalReportType legacyJournalReportType = LegacyJournalReportType.Default;
			while (transportPropertyReader.ReadNextProperty())
			{
				byte[] value = transportPropertyReader.Value;
				ExchangeMailMsgProperty id = (ExchangeMailMsgProperty)transportPropertyReader.Id;
				if (id != ExchangeMailMsgProperty.IMMPID_EMP_ORIGINAL_P1_RECIPIENT_LIST)
				{
					switch (id)
					{
					case ExchangeMailMsgProperty.IMMPID_EMP_EJ_RECIPIENT_LIST:
						mailItem.ExtendedProperties.SetValue<byte[]>("Microsoft.Exchange.EnvelopeJournalRecipientList", value);
						if (legacyJournalReportType != LegacyJournalReportType.EnvelopeV2)
						{
							legacyJournalReportType = LegacyJournalReportType.Envelope;
						}
						break;
					case ExchangeMailMsgProperty.IMMPID_EMP_EJ_RECIPIENT_P2_TYPE_LIST:
						legacyJournalReportType = LegacyJournalReportType.EnvelopeV2;
						mailItem.ExtendedProperties.SetValue<byte[]>("Microsoft.Exchange.EnvelopeJournalRecipientType", value);
						break;
					case ExchangeMailMsgProperty.IMMPID_EMP_EJ_EXPANSION_HISTORY_LIST:
						legacyJournalReportType = LegacyJournalReportType.EnvelopeV2;
						mailItem.ExtendedProperties.SetValue<byte[]>("Microsoft.Exchange.EnvelopeJournalExpansionHistory", value);
						break;
					}
				}
				else
				{
					mailItem.ExtendedProperties.SetValue<byte[]>("Microsoft.Exchange.OriginalP1RecipientList", value);
					legacyJournalReportType = LegacyJournalReportType.Bcc;
				}
			}
			mailItem.ExtendedProperties.SetValue<int>("Microsoft.Exchange.LegacyJournalReport", (int)legacyJournalReportType);
		}

		public const string ContentIdentifierProperty = "Microsoft.Exchange.ContentIdentifier";

		public const string RecipientMdbefPassThru = "Microsoft.Exchange.Legacy.PassThru";

		public const string ExJournalData = "EXJournalData";

		public const string JournalRecipientList = "Microsoft.Exchange.JournalRecipientList";

		protected const string PublicMdbSuffix = "/cn=Microsoft Public MDB";

		protected static readonly Guid ExchangeMailMsgPropertyRange = new Guid(4233967246U, 33985, 4562, 155, 237, 0, 160, 201, 94, 97, 67);

		protected delegate byte[] TransportPropertyFilter(ExchangeMailMsgProperty id, byte[] value);
	}
}
