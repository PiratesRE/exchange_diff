using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class AirSyncFatalException : LocalizedException
	{
		internal AirSyncFatalException(LocalizedString errorMessage, string loggerString, bool watsonReportEnabled) : this(errorMessage, loggerString, watsonReportEnabled, null)
		{
		}

		internal AirSyncFatalException(LocalizedString errorMessage, string loggerString, bool watsonReportEnabled, Exception innerException) : base(errorMessage, innerException)
		{
			if (string.IsNullOrEmpty(loggerString))
			{
				throw new ArgumentException("loggerstring cannot be null or empty");
			}
			this.loggerString = loggerString;
			this.watsonReportEnabled = watsonReportEnabled;
		}

		protected AirSyncFatalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.watsonReportEnabled = info.GetBoolean("watsonReportEnabled");
			this.loggerString = info.GetString("loggerString");
		}

		internal string LoggerString
		{
			get
			{
				return this.loggerString;
			}
		}

		internal bool WatsonReportEnabled
		{
			get
			{
				return this.watsonReportEnabled;
			}
			set
			{
				this.watsonReportEnabled = value;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("watsonReportEnabled", this.watsonReportEnabled);
			info.AddValue("loggerString", this.loggerString);
		}

		private string loggerString;

		private bool watsonReportEnabled;
	}
}
