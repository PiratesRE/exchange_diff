using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal static class StoreDriverUtils
	{
		public static string GetOldestPoisonEntryValueName(SynchronizedDictionary<string, CrashProperties> poisonEntryDictionary)
		{
			ArgumentValidator.ThrowIfNull("poisonEntryDictionary", poisonEntryDictionary);
			return (from keyValuePair in poisonEntryDictionary
			orderby keyValuePair.Value.LastCrashTime
			select keyValuePair.Key).FirstOrDefault<string>();
		}

		public static void SendInformationalWatson(Exception exception, string detailedExceptionInformation)
		{
			ArgumentValidator.ThrowIfNull("exception", exception);
			ArgumentValidator.ThrowIfNullOrEmpty("detailedExceptionInformation", detailedExceptionInformation);
			ExWatson.SendGenericWatsonReport("E12", ExWatson.ApplicationVersion.ToString(), ExWatson.AppName, "15.00.1497.015", Assembly.GetExecutingAssembly().GetName().Name, exception.GetType().Name, exception.StackTrace, exception.GetHashCode().ToString(), exception.TargetSite.Name, detailedExceptionInformation);
		}

		public static bool CheckIfDateTimeExceedsThreshold(DateTime dateTime, DateTime dateTimeReference, TimeSpan timeSpan)
		{
			return dateTimeReference > dateTime + timeSpan;
		}
	}
}
