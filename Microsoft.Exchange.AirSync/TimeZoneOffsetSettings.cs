using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data.ApplicationLogic.TimeZoneOffsets;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class TimeZoneOffsetSettings : SettingsBase
	{
		public TimeZoneOffsetSettings(XmlNode request, XmlNode response, IAirSyncUser user, ProtocolLogger protocolLogger) : base(request, response, protocolLogger)
		{
			this.user = user;
		}

		public override void Execute()
		{
			using (this.user.Context.Tracker.Start(TimeId.TimeZoneOffsetSettingsExecute))
			{
				XmlNode firstChild = base.Request.FirstChild;
				string localName;
				if ((localName = firstChild.LocalName) != null && localName == "Get")
				{
					this.ProcessGet(firstChild);
				}
			}
		}

		private void ProcessGet(XmlNode verbNode)
		{
			using (this.user.Context.Tracker.Start(TimeId.TimeZoneOffsetSettingsGet))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Processing TimeZoneOffsetSettings - Get");
				XmlNode xmlNode = null;
				try
				{
					XmlElement xmlElement = verbNode["StartTime"];
					ExDateTime startTime;
					if (xmlElement == null || !ExDateTime.TryParseExact(xmlElement.InnerText, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out startTime))
					{
						this.status = StatusCode.InvalidStartTime;
						base.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.Error, "InvalidStartTime");
						return;
					}
					XmlElement xmlElement2 = verbNode["EndTime"];
					ExDateTime endTime;
					if (xmlElement2 == null || !ExDateTime.TryParseExact(xmlElement2.InnerText, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out endTime))
					{
						this.status = StatusCode.InvalidEndTime;
						base.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.Error, "InvalidEndTime");
						return;
					}
					XmlNode xmlNode2 = base.Response.OwnerDocument.CreateElement("TimeZoneOffsets", "Settings:");
					TimeZoneOffset[] array = null;
					try
					{
						array = TimeZoneOffsets.GetTheTimeZoneOffsets(startTime, endTime, null, null);
					}
					catch (ArgumentException arg)
					{
						AirSyncDiagnostics.TraceError<ArgumentException>(ExTraceGlobals.RequestsTracer, this, "TimeZoneOffsets.GetTheTimeZoneOffsets has thrown {0}", arg);
						this.status = StatusCode.InvalidTimezoneRange;
						base.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.Error, "InvalidTimezoneRange");
						return;
					}
					if (array != null)
					{
						foreach (TimeZoneOffset timeZoneOffset in array)
						{
							XmlNode xmlNode3 = base.Response.OwnerDocument.CreateElement("TimeZoneOffset", "Settings:");
							XmlNode xmlNode4 = base.Response.OwnerDocument.CreateElement("TimeZoneID", "Settings:");
							xmlNode4.InnerText = timeZoneOffset.TimeZoneId;
							xmlNode3.AppendChild(xmlNode4);
							XmlNode xmlNode5 = base.Response.OwnerDocument.CreateElement("TimeZoneRanges", "Settings:");
							foreach (TimeZoneRange timeZoneRange in timeZoneOffset.OffsetRanges)
							{
								XmlNode xmlNode6 = base.Response.OwnerDocument.CreateElement("TimeZoneRange", "Settings:");
								XmlNode xmlNode7 = base.Response.OwnerDocument.CreateElement("UtcTime", "Settings:");
								xmlNode7.InnerText = timeZoneRange.UtcTime.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z");
								xmlNode6.AppendChild(xmlNode7);
								XmlNode xmlNode8 = base.Response.OwnerDocument.CreateElement("Offset", "Settings:");
								xmlNode8.InnerText = timeZoneRange.Offset.ToString();
								xmlNode6.AppendChild(xmlNode8);
								xmlNode5.AppendChild(xmlNode6);
							}
							xmlNode3.AppendChild(xmlNode5);
							xmlNode2.AppendChild(xmlNode3);
						}
					}
					xmlNode = base.Response.OwnerDocument.CreateElement("Get", "Settings:");
					xmlNode.AppendChild(xmlNode2);
				}
				finally
				{
					XmlNode xmlNode9 = base.Response.OwnerDocument.CreateElement("Status", "Settings:");
					XmlNode xmlNode10 = xmlNode9;
					int num = (int)this.status;
					xmlNode10.InnerText = num.ToString(CultureInfo.InvariantCulture);
					base.Response.AppendChild(xmlNode9);
					if (xmlNode != null)
					{
						base.Response.AppendChild(xmlNode);
					}
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Done processing TimeZoneOffsetSettings - Get.");
			}
		}

		private StatusCode status = StatusCode.Success;

		private IAirSyncUser user;
	}
}
