using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal static class AmFaultInject
	{
		internal static void Init()
		{
			AmFaultInject.m_faultInjectHelper.Init();
		}

		internal static void SleepIfRequired(Guid dbGuid, string propertyName)
		{
			AmFaultInject.m_faultInjectHelper.SleepIfRequired(dbGuid, propertyName);
		}

		internal static void SleepIfRequired(string propertyName)
		{
			AmFaultInject.m_faultInjectHelper.SleepIfRequired(propertyName);
		}

		internal static void SleepIfRequired(Guid dbGuid, AmSleepTag sleepTag)
		{
			AmFaultInject.m_faultInjectHelper.SleepIfRequired(dbGuid, sleepTag.ToString());
		}

		internal static void SleepIfRequired(AmSleepTag sleepTag)
		{
			AmFaultInject.m_faultInjectHelper.SleepIfRequired(sleepTag.ToString());
		}

		internal static void SleepIfRequired(string subKeyName, string propertyName)
		{
			AmFaultInject.m_faultInjectHelper.SleepIfRequired(subKeyName, propertyName);
		}

		internal static void GenerateMapiExceptionIfRequired(Guid dbGuid, AmServerName serverName)
		{
			AmFaultInject.m_faultInjectHelper.GenerateMapiExceptionIfRequired(dbGuid, serverName);
		}

		private static AmFaultInjectHelper m_faultInjectHelper = new AmFaultInjectHelper();
	}
}
