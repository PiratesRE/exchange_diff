using System;
using System.Globalization;
using Microsoft.Exchange.AddressBook.Service;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.AddressBook.Nspi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PhotoRequestAddressbookLogger : IPerformanceDataLogger
	{
		internal PhotoRequestAddressbookLogger(ProtocolLogSession logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			this.logger = logger;
		}

		public void Log(string marker, string counter, TimeSpan dataPoint)
		{
			if (PhotoRequestAddressbookLogger.ShouldLogMarker(marker))
			{
				ProtocolLogSession protocolLogSession;
				(protocolLogSession = this.logger)[ProtocolLog.Field.OperationSpecific] = protocolLogSession[ProtocolLog.Field.OperationSpecific] + string.Format(CultureInfo.InvariantCulture, ";{0}.{1}={2}", new object[]
				{
					marker,
					counter,
					dataPoint.TotalMilliseconds
				});
			}
		}

		public void Log(string marker, string counter, uint dataPoint)
		{
			if (PhotoRequestAddressbookLogger.ShouldLogMarker(marker))
			{
				ProtocolLogSession protocolLogSession;
				(protocolLogSession = this.logger)[ProtocolLog.Field.OperationSpecific] = protocolLogSession[ProtocolLog.Field.OperationSpecific] + string.Format(CultureInfo.InvariantCulture, ";{0}.{1}={2}", new object[]
				{
					marker,
					counter,
					dataPoint
				});
			}
		}

		public void Log(string marker, string counter, string dataPoint)
		{
			if (PhotoRequestAddressbookLogger.ShouldLogMarker(marker))
			{
				ProtocolLogSession protocolLogSession;
				(protocolLogSession = this.logger)[ProtocolLog.Field.OperationSpecific] = protocolLogSession[ProtocolLog.Field.OperationSpecific] + string.Format(CultureInfo.InvariantCulture, ";{0}.{1}={2}", new object[]
				{
					marker,
					counter,
					dataPoint
				});
			}
		}

		public void AppendToLog(string logEntry)
		{
			ProtocolLogSession protocolLogSession;
			(protocolLogSession = this.logger)[ProtocolLog.Field.OperationSpecific] = protocolLogSession[ProtocolLog.Field.OperationSpecific] + string.Format(CultureInfo.InvariantCulture, ";GPP.{0}", new object[]
			{
				logEntry
			});
		}

		private static bool ShouldLogMarker(string marker)
		{
			return "getuserphotototal".Equals(marker, StringComparison.OrdinalIgnoreCase);
		}

		private const string GetUserPhotoLatencyMarker = "getuserphotototal";

		private readonly ProtocolLogSession logger;
	}
}
