using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WMIErrorException : BitlockerUtilException
	{
		public WMIErrorException(int returnValue, string methodName, string errorMessage) : base(DiskManagementStrings.WMIError(returnValue, methodName, errorMessage))
		{
			this.returnValue = returnValue;
			this.methodName = methodName;
			this.errorMessage = errorMessage;
		}

		public WMIErrorException(int returnValue, string methodName, string errorMessage, Exception innerException) : base(DiskManagementStrings.WMIError(returnValue, methodName, errorMessage), innerException)
		{
			this.returnValue = returnValue;
			this.methodName = methodName;
			this.errorMessage = errorMessage;
		}

		protected WMIErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.returnValue = (int)info.GetValue("returnValue", typeof(int));
			this.methodName = (string)info.GetValue("methodName", typeof(string));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("returnValue", this.returnValue);
			info.AddValue("methodName", this.methodName);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public int ReturnValue
		{
			get
			{
				return this.returnValue;
			}
		}

		public string MethodName
		{
			get
			{
				return this.methodName;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly int returnValue;

		private readonly string methodName;

		private readonly string errorMessage;
	}
}
