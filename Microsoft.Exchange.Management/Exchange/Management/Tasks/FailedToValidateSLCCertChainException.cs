using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.RightsManagementServices.Core;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToValidateSLCCertChainException : LocalizedException
	{
		public FailedToValidateSLCCertChainException(WellKnownErrorCode eCode) : base(Strings.FailedToValidateSLCCertChain(eCode))
		{
			this.eCode = eCode;
		}

		public FailedToValidateSLCCertChainException(WellKnownErrorCode eCode, Exception innerException) : base(Strings.FailedToValidateSLCCertChain(eCode), innerException)
		{
			this.eCode = eCode;
		}

		protected FailedToValidateSLCCertChainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.eCode = (WellKnownErrorCode)info.GetValue("eCode", typeof(WellKnownErrorCode));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("eCode", this.eCode);
		}

		public WellKnownErrorCode ECode
		{
			get
			{
				return this.eCode;
			}
		}

		private readonly WellKnownErrorCode eCode;
	}
}
