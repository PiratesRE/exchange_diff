using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class GetServerTimeZoneResultType : ResponseMessage
	{
		internal GetServerTimeZoneResultType(bool returnFullTimeZoneData, TimeZoneDefinitionType[] timeZoneDefinitions)
		{
			this.returnFullTimeZoneData = returnFullTimeZoneData;
			this.timeZoneDefinitions = timeZoneDefinitions;
		}

		public TimeZoneDefinitionType[] ToTimeZoneDefinitionType()
		{
			foreach (TimeZoneDefinitionType timeZoneDefinitionType in this.timeZoneDefinitions)
			{
				timeZoneDefinitionType.Render(this.returnFullTimeZoneData, EWSSettings.ClientCulture);
			}
			return this.timeZoneDefinitions;
		}

		private bool returnFullTimeZoneData;

		private TimeZoneDefinitionType[] timeZoneDefinitions;
	}
}
