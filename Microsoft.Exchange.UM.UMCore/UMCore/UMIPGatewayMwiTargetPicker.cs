using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMIPGatewayMwiTargetPicker : UMIPGatewayTargetPickerBase
	{
		private UMIPGatewayMwiTargetPicker()
		{
		}

		public static UMIPGatewayMwiTargetPicker Instance
		{
			get
			{
				return UMIPGatewayMwiTargetPicker.staticInstance;
			}
		}

		protected override bool InternalIsValid(UMIPGateway candidate)
		{
			return candidate.MessageWaitingIndicatorAllowed;
		}

		private static UMIPGatewayMwiTargetPicker staticInstance = new UMIPGatewayMwiTargetPicker();
	}
}
