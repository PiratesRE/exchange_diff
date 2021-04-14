using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ChangeHighlightsProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, IPropertyCommand
	{
		private ChangeHighlightsProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ChangeHighlightsProperty CreateCommand(CommandContext commandContext)
		{
			return new ChangeHighlightsProperty(commandContext);
		}

		private static ChangeHighlightsType Render(StoreObject storeObject)
		{
			MeetingRequest meetingRequest = (MeetingRequest)storeObject;
			bool flag = (meetingRequest.ChangeHighlight & ChangeHighlightProperties.Location) != ChangeHighlightProperties.None;
			bool flag2 = (meetingRequest.ChangeHighlight & ChangeHighlightProperties.MapiStartTime) != ChangeHighlightProperties.None;
			bool flag3 = (meetingRequest.ChangeHighlight & ChangeHighlightProperties.MapiEndTime) != ChangeHighlightProperties.None;
			string start = null;
			string end = null;
			if (flag2)
			{
				if (PropertyCommand.StorePropertyExists(storeObject, MeetingRequestSchema.OldStartWhole))
				{
					ExDateTime systemDateTime = (ExDateTime)storeObject.TryGetProperty(MeetingRequestSchema.OldStartWhole);
					start = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime);
				}
				else
				{
					flag2 = false;
				}
			}
			if (flag3)
			{
				if (PropertyCommand.StorePropertyExists(storeObject, MeetingRequestSchema.OldEndWhole))
				{
					ExDateTime systemDateTime2 = (ExDateTime)storeObject.TryGetProperty(MeetingRequestSchema.OldEndWhole);
					end = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime2);
				}
				else
				{
					flag3 = false;
				}
			}
			return new ChangeHighlightsType
			{
				HasLocationChanged = flag,
				Location = (flag ? meetingRequest.OldLocation : null),
				HasStartTimeChanged = flag2,
				Start = start,
				HasEndTimeChanged = flag3,
				End = end
			};
		}

		internal static ChangeHighlightsType Render(ItemPart itemPart)
		{
			IStorePropertyBag storePropertyBag = itemPart.StorePropertyBag;
			ChangeHighlightProperties valueOrDefault = storePropertyBag.GetValueOrDefault<ChangeHighlightProperties>(CalendarItemBaseSchema.ChangeHighlight, ChangeHighlightProperties.None);
			bool flag = (valueOrDefault & ChangeHighlightProperties.Location) != ChangeHighlightProperties.None;
			bool flag2 = (valueOrDefault & ChangeHighlightProperties.MapiStartTime) != ChangeHighlightProperties.None;
			bool flag3 = (valueOrDefault & ChangeHighlightProperties.MapiEndTime) != ChangeHighlightProperties.None;
			string start = null;
			string end = null;
			if (flag2)
			{
				if (storePropertyBag.TryGetProperty(MeetingRequestSchema.OldStartWhole) != null)
				{
					ExDateTime systemDateTime = (ExDateTime)storePropertyBag.TryGetProperty(MeetingRequestSchema.OldStartWhole);
					start = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime);
				}
				else
				{
					flag2 = false;
				}
			}
			if (flag3)
			{
				if (storePropertyBag.TryGetProperty(MeetingRequestSchema.OldEndWhole) != null)
				{
					ExDateTime systemDateTime2 = (ExDateTime)storePropertyBag.TryGetProperty(MeetingRequestSchema.OldEndWhole);
					end = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime2);
				}
				else
				{
					flag3 = false;
				}
			}
			return new ChangeHighlightsType
			{
				HasLocationChanged = flag,
				Location = (flag ? storePropertyBag.GetValueOrDefault<string>(CalendarItemBaseSchema.OldLocation, string.Empty) : null),
				HasStartTimeChanged = flag2,
				Start = start,
				HasEndTimeChanged = flag3,
				End = end
			};
		}

		public void ToXml()
		{
			throw new InvalidOperationException("ChangeHighlightsProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
			{
				serviceObject[MeetingRequestSchema.ChangeHighlights] = ChangeHighlightsProperty.Render(storeObject);
				return;
			}
			serviceObject[MeetingRequestSchema.ChangeHighlights] = null;
		}
	}
}
