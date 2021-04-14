using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.UM.Rpc
{
	[Serializable]
	public class ActiveCalls : UMDiagnosticObject
	{
		public string GatewayId
		{
			get
			{
				return this.gatewayId;
			}
			set
			{
				this.gatewayId = value;
			}
		}

		public string ServerId
		{
			get
			{
				return this.serverId;
			}
			set
			{
				this.serverId = value;
			}
		}

		public string DialPlan
		{
			get
			{
				return this.dialPlan;
			}
			set
			{
				this.dialPlan = value;
			}
		}

		public string DialedNumber
		{
			get
			{
				return this.dialedNumber;
			}
			set
			{
				this.dialedNumber = value;
			}
		}

		public string CallType
		{
			get
			{
				return this.callType;
			}
			set
			{
				this.callType = value;
			}
		}

		public string CallingNumber
		{
			get
			{
				return this.callingNumber;
			}
			set
			{
				this.callingNumber = value;
			}
		}

		public string DiversionNumber
		{
			get
			{
				return this.diversionNumber;
			}
			set
			{
				this.diversionNumber = value;
			}
		}

		public string CallState
		{
			get
			{
				return this.callState;
			}
			set
			{
				this.callState = value;
			}
		}

		public string AppState
		{
			get
			{
				return this.appState;
			}
			set
			{
				this.appState = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		private string gatewayId;

		private string serverId;

		private string dialPlan;

		private string dialedNumber;

		private string callType;

		private string callingNumber;

		private string diversionNumber;

		private string callState;

		private string appState;
	}
}
