using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public class MonitoringPerformanceCounter
	{
		internal MonitoringPerformanceCounter(string performanceObject, string performanceCounter, string performanceInstance, double performanceValue)
		{
			if (string.IsNullOrEmpty(performanceObject))
			{
				throw new ArgumentNullException("performanceObject");
			}
			if (string.IsNullOrEmpty(performanceCounter))
			{
				throw new ArgumentNullException("performanceCounter");
			}
			if (string.IsNullOrEmpty(performanceInstance))
			{
				throw new ArgumentNullException("performanceInstance");
			}
			this.performanceObject = performanceObject;
			this.performanceCounter = performanceCounter;
			this.performanceInstance = performanceInstance;
			this.performanceValue = performanceValue;
		}

		public string Object
		{
			get
			{
				return this.performanceObject;
			}
		}

		public string Counter
		{
			get
			{
				return this.performanceCounter;
			}
		}

		public string Instance
		{
			get
			{
				return this.performanceInstance;
			}
		}

		public double Value
		{
			get
			{
				return this.performanceValue;
			}
		}

		public override string ToString()
		{
			return Strings.MonitoringPerfomanceCounterString(this.Object, this.Counter, this.Instance, this.Value);
		}

		private string performanceObject;

		private string performanceCounter;

		private string performanceInstance;

		private double performanceValue;
	}
}
