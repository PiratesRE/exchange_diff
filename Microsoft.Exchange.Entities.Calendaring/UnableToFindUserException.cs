using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnableToFindUserException : StoragePermanentException
	{
		public UnableToFindUserException(ADOperationErrorCode operationErrorCode) : base(CalendaringStrings.UnableToFindUser(operationErrorCode))
		{
			this.operationErrorCode = operationErrorCode;
		}

		public UnableToFindUserException(ADOperationErrorCode operationErrorCode, Exception innerException) : base(CalendaringStrings.UnableToFindUser(operationErrorCode), innerException)
		{
			this.operationErrorCode = operationErrorCode;
		}

		protected UnableToFindUserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operationErrorCode = (ADOperationErrorCode)info.GetValue("operationErrorCode", typeof(ADOperationErrorCode));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operationErrorCode", this.operationErrorCode);
		}

		public ADOperationErrorCode OperationErrorCode
		{
			get
			{
				return this.operationErrorCode;
			}
		}

		private readonly ADOperationErrorCode operationErrorCode;
	}
}
