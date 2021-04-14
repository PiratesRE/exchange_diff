using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetServerTimeZones : SingleStepServiceCommand<GetServerTimeZonesRequest, GetServerTimeZoneResultType>
	{
		public GetServerTimeZones(CallContext callContext, GetServerTimeZonesRequest request) : base(callContext, request)
		{
			this.timeZoneIds = request.Id;
			this.returnFullTimeZoneData = request.ReturnFullTimeZoneData;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetServerTimeZonesResponse getServerTimeZonesResponse = new GetServerTimeZonesResponse();
			getServerTimeZonesResponse.ProcessServiceResult<GetServerTimeZoneResultType>(base.Result);
			return getServerTimeZonesResponse;
		}

		internal override ServiceResult<GetServerTimeZoneResultType> Execute()
		{
			GetServerTimeZoneResultType serverTimeZonesResult = this.GetServerTimeZonesResult(this.timeZoneIds, this.returnFullTimeZoneData);
			return new ServiceResult<GetServerTimeZoneResultType>(serverTimeZonesResult);
		}

		private GetServerTimeZoneResultType GetServerTimeZonesResult(string[] timeZoneIds, bool returnFullTimeZoneData)
		{
			List<TimeZoneDefinitionType> list = new List<TimeZoneDefinitionType>();
			if (timeZoneIds != null)
			{
				foreach (string text in timeZoneIds)
				{
					ExTimeZone exchTimeZone;
					if (!string.IsNullOrEmpty(text) && ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(text, out exchTimeZone))
					{
						list.Add(new TimeZoneDefinitionType(exchTimeZone));
					}
				}
			}
			else
			{
				foreach (ExTimeZone exchTimeZone2 in ExTimeZoneEnumerator.Instance)
				{
					list.Add(new TimeZoneDefinitionType(exchTimeZone2));
				}
			}
			return new GetServerTimeZoneResultType(this.returnFullTimeZoneData, list.ToArray());
		}

		private string[] timeZoneIds;

		private bool returnFullTimeZoneData;
	}
}
