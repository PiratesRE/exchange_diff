using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal interface IAirSyncContext : IReadOnlyPropertyBag, INotificationManagerContext, ISyncLogger
	{
		bool PerCallTracingEnabled { get; }

		IAirSyncUser User { get; set; }

		string TaskDescription { get; }

		TimeTracker Tracker { get; set; }

		IPrincipal Principal { get; set; }

		string WindowsLiveId { get; }

		Dictionary<EasFeature, bool> FlightingOverrides { get; }

		IAirSyncRequest Request { get; }

		IAirSyncResponse Response { get; }

		ProtocolLogger ProtocolLogger { get; }

		DeviceBehavior DeviceBehavior { get; set; }

		void PrepareToHang();

		void WriteActivityContextData();

		string GetActivityContextData();

		void SetDiagnosticValue(PropertyDefinition propDef, object value);

		void ClearDiagnosticValue(PropertyDefinition propDef);

		object GetThrottlingPolicyValue(Func<IThrottlingPolicy, object> func);

		bool TryGetElapsed(PropertyDefinition startTime, out TimeSpan elapsed);
	}
}
