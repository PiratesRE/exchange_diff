using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning
{
	[Serializable]
	public class WindowsLiveException : Exception
	{
		public WindowsLiveException()
		{
		}

		public WindowsLiveException(string message) : base(message)
		{
		}

		public WindowsLiveException(int errorCodeParameter, string message) : base(message)
		{
			this.errorCode = errorCodeParameter;
		}

		public WindowsLiveException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected WindowsLiveException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ErrorCode", this.ErrorCode);
			base.GetObjectData(info, context);
		}

		private readonly int errorCode;
	}
}
