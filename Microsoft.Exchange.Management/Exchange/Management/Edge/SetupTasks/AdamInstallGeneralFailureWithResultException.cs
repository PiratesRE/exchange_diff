using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdamInstallGeneralFailureWithResultException : LocalizedException
	{
		public AdamInstallGeneralFailureWithResultException(int adamErrorCode) : base(Strings.AdamInstallGeneralFailureWithResult(adamErrorCode))
		{
			this.adamErrorCode = adamErrorCode;
		}

		public AdamInstallGeneralFailureWithResultException(int adamErrorCode, Exception innerException) : base(Strings.AdamInstallGeneralFailureWithResult(adamErrorCode), innerException)
		{
			this.adamErrorCode = adamErrorCode;
		}

		protected AdamInstallGeneralFailureWithResultException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.adamErrorCode = (int)info.GetValue("adamErrorCode", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("adamErrorCode", this.adamErrorCode);
		}

		public int AdamErrorCode
		{
			get
			{
				return this.adamErrorCode;
			}
		}

		private readonly int adamErrorCode;
	}
}
