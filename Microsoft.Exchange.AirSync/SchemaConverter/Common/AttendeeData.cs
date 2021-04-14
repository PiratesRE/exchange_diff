using System;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal struct AttendeeData
	{
		public AttendeeData(string emailAddress, string displayName)
		{
			AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.CommonTracer, null, "AttendeeData Created email={0} displayname={1}", emailAddress, displayName);
			this.emailAddress = emailAddress;
			this.displayName = displayName;
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		public string EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
			set
			{
				this.emailAddress = value;
			}
		}

		private string displayName;

		private string emailAddress;
	}
}
