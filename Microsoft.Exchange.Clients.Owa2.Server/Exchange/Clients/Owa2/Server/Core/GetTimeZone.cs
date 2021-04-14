using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetTimeZone : ServiceCommand<TimeZoneConfiguration>
	{
		public GetTimeZone(CallContext callContext, bool needTimeZoneList) : base(callContext)
		{
			this.needTimeZoneList = needTimeZoneList;
		}

		protected override TimeZoneConfiguration InternalExecute()
		{
			return GetTimeZone.GetSetting(base.CallContext, this.needTimeZoneList);
		}

		public static TimeZoneConfiguration GetSetting(CallContext callContext, bool needTimeZoneList)
		{
			TimeZoneConfiguration timeZoneConfiguration = new TimeZoneConfiguration();
			if (needTimeZoneList)
			{
				List<TimeZoneEntry> list = new List<TimeZoneEntry>();
				foreach (ExTimeZone timezone in ExTimeZoneEnumerator.Instance)
				{
					list.Add(new TimeZoneEntry(timezone));
				}
				timeZoneConfiguration.TimeZoneList = list.ToArray();
			}
			UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.TimeZone);
			UserOptionsType userOptionsType = new UserOptionsType();
			userOptionsType.Load(callContext, new UserConfigurationPropertyDefinition[]
			{
				propertyDefinition
			});
			timeZoneConfiguration.CurrentTimeZone = userOptionsType.TimeZone;
			return timeZoneConfiguration;
		}

		private readonly bool needTimeZoneList;
	}
}
