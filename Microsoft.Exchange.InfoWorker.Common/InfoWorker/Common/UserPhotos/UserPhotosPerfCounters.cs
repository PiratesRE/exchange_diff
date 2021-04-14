using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.UserPhotos
{
	internal static class UserPhotosPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (UserPhotosPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in UserPhotosPerfCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchange UserPhotos";

		public static readonly ExPerformanceCounter UserPhotosCurrentRequests = new ExPerformanceCounter("MSExchange UserPhotos", "UserPhotos Current Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			UserPhotosPerfCounters.UserPhotosCurrentRequests
		};
	}
}
