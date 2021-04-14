using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CanaryLogEvent : ILogEvent
	{
		public CanaryLogEvent(string version, string activityContextId, string canaryName, string canaryPath, string logonUniqueKey, CanaryStatus canaryStatus, DateTime creationTime, string logData)
		{
			if (string.IsNullOrEmpty(canaryName))
			{
				throw new ArgumentNullException("canaryName");
			}
			if (string.IsNullOrEmpty(canaryPath))
			{
				throw new ArgumentNullException("canaryPath");
			}
			this.version = this.TranslateStringValueToLog(version);
			this.activityContextId = activityContextId;
			this.canaryName = canaryName;
			this.canaryPath = canaryPath;
			this.logonUniqueKey = this.TranslateStringValueToLog(logonUniqueKey);
			this.canaryStatus = canaryStatus;
			this.creationTime = creationTime;
			this.logData = this.TranslateStringValueToLog(logData);
		}

		public string EventId
		{
			get
			{
				return ActivityContextLoggerId.Canary.ToString();
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("ACTID", this.activityContextId),
				new KeyValuePair<string, object>("V", this.version),
				new KeyValuePair<string, object>("LUK", ExtensibleLogger.FormatPIIValue(this.logonUniqueKey)),
				new KeyValuePair<string, object>("CN.N", this.canaryName),
				new KeyValuePair<string, object>("CN.P", this.canaryPath),
				new KeyValuePair<string, object>("CN.S", string.Format("0x{0:X}", (int)this.canaryStatus)),
				new KeyValuePair<string, object>("CN.T", this.creationTime),
				new KeyValuePair<string, object>("CN.L", this.logData)
			};
		}

		private string TranslateStringValueToLog(string value)
		{
			if (value == null)
			{
				return "<null>";
			}
			if (value == string.Empty)
			{
				return "<empty>";
			}
			return value;
		}

		private readonly string activityContextId;

		private readonly string canaryName;

		private readonly string canaryPath;

		private readonly string logonUniqueKey;

		private readonly string version;

		private readonly CanaryStatus canaryStatus;

		private readonly DateTime creationTime;

		private readonly string logData;
	}
}
