using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdamUninstallGeneralFailureWithResultException : LocalizedException
	{
		public AdamUninstallGeneralFailureWithResultException(int adamErrorCode) : base(Strings.AdamUninstallGeneralFailureWithResult(adamErrorCode))
		{
			this.adamErrorCode = adamErrorCode;
		}

		public AdamUninstallGeneralFailureWithResultException(int adamErrorCode, Exception innerException) : base(Strings.AdamUninstallGeneralFailureWithResult(adamErrorCode), innerException)
		{
			this.adamErrorCode = adamErrorCode;
		}

		protected AdamUninstallGeneralFailureWithResultException(SerializationInfo info, StreamingContext context) : base(info, context)
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
