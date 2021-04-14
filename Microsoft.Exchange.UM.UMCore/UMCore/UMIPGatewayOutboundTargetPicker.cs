using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMIPGatewayOutboundTargetPicker : UMIPGatewayTargetPickerBase
	{
		private UMIPGatewayOutboundTargetPicker()
		{
		}

		public static UMIPGatewayOutboundTargetPicker Instance
		{
			get
			{
				return UMIPGatewayOutboundTargetPicker.staticInstance;
			}
		}

		protected override bool InternalIsValid(UMIPGateway gateway)
		{
			return gateway.OutcallsAllowed;
		}

		private static UMIPGatewayOutboundTargetPicker staticInstance = new UMIPGatewayOutboundTargetPicker();
	}
}
