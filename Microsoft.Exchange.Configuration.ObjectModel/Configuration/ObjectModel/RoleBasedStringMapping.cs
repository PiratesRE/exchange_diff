using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ObjectModel;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal static class RoleBasedStringMapping
	{
		private static ExEventLog EventLog
		{
			get
			{
				if (RoleBasedStringMapping.eventlog == null)
				{
					RoleBasedStringMapping.eventlog = new ExEventLog(ExTraceGlobals.RoleBasedStringMappingTracer.Category, "MSExchange Configuration Object Model - String Interface Packs");
				}
				return RoleBasedStringMapping.eventlog;
			}
		}

		internal static CultureInfo GetRoleBasedCultureInfo(CultureInfo parentCulture, ICollection<RoleType> roleTypesCollection)
		{
			CultureInfo result;
			try
			{
				if (roleTypesCollection is List<RoleType>)
				{
					List<RoleType> list = (List<RoleType>)roleTypesCollection;
				}
				else
				{
					new List<RoleType>(roleTypesCollection);
				}
				string text = null;
				CultureInfo cultureInfo;
				if (text == null)
				{
					cultureInfo = parentCulture;
				}
				else
				{
					cultureInfo = SipCultureInfoFactory.CreateInstance(parentCulture, text);
				}
				result = cultureInfo;
			}
			catch (Exception ex)
			{
				RoleBasedStringMapping.EventLog.LogEvent(TaskEventLogConstants.Tuple_RoleBasedStringMappingFailure, null, new object[]
				{
					ex.GetType().Name,
					ex.ToString()
				});
				result = parentCulture;
			}
			return result;
		}

		private static ExEventLog eventlog;
	}
}
