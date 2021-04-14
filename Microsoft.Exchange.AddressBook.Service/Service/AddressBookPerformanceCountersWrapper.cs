using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal static class AddressBookPerformanceCountersWrapper
	{
		public static IAddressBookPerformanceCounters AddressBookPerformanceCounters
		{
			get
			{
				return AddressBookPerformanceCountersWrapper.addressBookPerformanceCounters;
			}
		}

		public static void Initialize(IAddressBookPerformanceCounters addressBookPerformanceCounters)
		{
			AddressBookPerformanceCountersWrapper.addressBookPerformanceCounters = addressBookPerformanceCounters;
			AddressBookPerformanceCountersWrapper.InitializeCounters(AddressBookPerformanceCountersWrapper.addressBookPerformanceCounters);
		}

		private static void InitializeCounters(object performanceCounters)
		{
			Type typeFromHandle = typeof(IExPerformanceCounter);
			foreach (PropertyInfo propertyInfo in performanceCounters.GetType().GetProperties())
			{
				if (typeFromHandle.IsAssignableFrom(propertyInfo.PropertyType))
				{
					IExPerformanceCounter exPerformanceCounter = propertyInfo.GetValue(performanceCounters, null) as IExPerformanceCounter;
					if (exPerformanceCounter != null)
					{
						exPerformanceCounter.RawValue = 0L;
					}
				}
			}
		}

		private static IAddressBookPerformanceCounters addressBookPerformanceCounters = new NullAddressBookPerformanceCounters();
	}
}
