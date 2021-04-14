using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LatencyReportingThresholdContainer
	{
		private LatencyReportingThresholdContainer()
		{
		}

		public static LatencyReportingThresholdContainer Instance
		{
			get
			{
				return LatencyReportingThresholdContainer.singletonInstance;
			}
		}

		internal IDictionary<string, LatencyDetectionLocation> Locations
		{
			get
			{
				return this.locationsByName;
			}
		}

		public void Clear()
		{
			foreach (LatencyDetectionLocation latencyDetectionLocation in this.Locations.Values)
			{
				latencyDetectionLocation.ClearThresholds();
			}
		}

		public void SetThreshold(string identity, LoggingType type, TimeSpan threshold)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			LatencyDetectionLocation latencyDetectionLocation;
			if (this.locationsByName.TryGetValue(identity, out latencyDetectionLocation))
			{
				latencyDetectionLocation.SetThreshold(type, threshold);
				return;
			}
			throw new ArgumentException("Not the id of an existing location.", "identity");
		}

		public TimeSpan GetThreshold(string identity, LoggingType type)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentException("Should not be null or empty.", "identity");
			}
			LatencyDetectionLocation latencyDetectionLocation = null;
			if (this.locationsByName.TryGetValue(identity, out latencyDetectionLocation))
			{
				return latencyDetectionLocation.GetThreshold(type).Threshold;
			}
			throw new ArgumentException("Not the id of an existing location.", "identity");
		}

		internal void ValidateLocation(LatencyDetectionLocation location)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			string identity = location.Identity;
			TimeSpan minimumThreshold = location.MinimumThreshold;
			if (location.MinimumThreshold < LatencyReportingThreshold.MinimumThresholdValue)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Minimum threshold for location with identity \"{0}\", {1}, is below the allowed minimum of {2}.", new object[]
				{
					identity,
					minimumThreshold,
					LatencyReportingThreshold.MinimumThresholdValue
				});
				throw new ArgumentException(message, "location");
			}
			LatencyDetectionLocation latencyDetectionLocation;
			if (this.locationsByName.TryGetValue(identity, out latencyDetectionLocation))
			{
				if (location != latencyDetectionLocation)
				{
					string message2 = string.Format(CultureInfo.InvariantCulture, "More than one {0} found with Identity = \"{1}\"", new object[]
					{
						typeof(LatencyDetectionLocation).FullName,
						identity
					});
					throw new ArgumentException(message2, "location");
				}
			}
			else
			{
				this.locationsByName[identity] = location;
			}
		}

		private static LatencyReportingThresholdContainer singletonInstance = new LatencyReportingThresholdContainer();

		private readonly IDictionary<string, LatencyDetectionLocation> locationsByName = new Dictionary<string, LatencyDetectionLocation>();
	}
}
