using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Security.RightsManagement.Protectors;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class RightsManagementPermanentException : StoragePermanentException
	{
		public virtual RightsManagementFailureCode FailureCode { get; private set; }

		public RightsManagementPermanentException(RightsManagementFailureCode failureCode, LocalizedString message) : base(message)
		{
			EnumValidator.ThrowIfInvalid<RightsManagementFailureCode>(failureCode, "failureCode");
			this.FailureCode = failureCode;
		}

		public RightsManagementPermanentException(LocalizedString message, LocalizedException innerException) : base(message, innerException)
		{
			this.FailureCode = RightsManagementPermanentException.GetRightsManagementFailureCode(innerException);
		}

		protected RightsManagementPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.FailureCode = (RightsManagementFailureCode)info.GetValue("failureCode", typeof(RightsManagementFailureCode));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("failureCode", this.FailureCode);
		}

		private static RightsManagementFailureCode GetRightsManagementFailureCode(Exception exception)
		{
			if (exception is InvalidRpmsgFormatException)
			{
				return RightsManagementFailureCode.CorruptData;
			}
			RightsManagementException ex = exception as RightsManagementException;
			if (exception is AttachmentProtectionException)
			{
				ex = (exception.InnerException as RightsManagementException);
				if (ex == null)
				{
					return RightsManagementFailureCode.CorruptData;
				}
			}
			if (ex != null)
			{
				return ex.FailureCode;
			}
			return RightsManagementFailureCode.UnknownFailure;
		}

		private const string FailureCodeLabel = "failureCode";
	}
}
