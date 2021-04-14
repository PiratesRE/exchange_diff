using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DataCenterForestChooser : IRedirectTargetChooser
	{
		public DataCenterForestChooser(CallContext callContext, string forestFqdn, string phoneNumber)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (string.IsNullOrEmpty(forestFqdn))
			{
				throw new ArgumentOutOfRangeException("forestFqdn", forestFqdn, "Invalid forest FQDN");
			}
			if (string.IsNullOrEmpty(phoneNumber))
			{
				throw new ArgumentOutOfRangeException("phoneNumber", phoneNumber, "Invalid phone number");
			}
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "DataCenterForestChooser constructor with forest FQDN = '{0}' and phone number = '_PhoneNumber'", new object[]
			{
				forestFqdn
			});
			this.callContext = callContext;
			this.forestFqdn = forestFqdn;
			this.phoneNumber = phoneNumber;
		}

		public string SubscriberLogId
		{
			get
			{
				return this.phoneNumber;
			}
		}

		public bool GetTargetServer(out string fqdn, out int port)
		{
			fqdn = this.forestFqdn;
			port = Utils.GetRedirectPort(this.callContext.IsSecuredCall);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "DataCenterForestChooser::GetTargetServer() returning {0}:{1}", new object[]
			{
				fqdn,
				port
			});
			return true;
		}

		public void HandleServerNotFound()
		{
		}

		private CallContext callContext;

		private string forestFqdn;

		private string phoneNumber;
	}
}
