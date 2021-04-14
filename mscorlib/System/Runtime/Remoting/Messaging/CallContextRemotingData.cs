using System;

namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	internal class CallContextRemotingData : ICloneable
	{
		internal string LogicalCallID
		{
			get
			{
				return this._logicalCallID;
			}
			set
			{
				this._logicalCallID = value;
			}
		}

		internal bool HasInfo
		{
			get
			{
				return this._logicalCallID != null;
			}
		}

		public object Clone()
		{
			return new CallContextRemotingData
			{
				LogicalCallID = this.LogicalCallID
			};
		}

		private string _logicalCallID;
	}
}
