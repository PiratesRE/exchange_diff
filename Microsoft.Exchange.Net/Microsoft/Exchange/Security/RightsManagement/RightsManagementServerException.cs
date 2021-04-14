using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.RightsManagementServices.Core;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class RightsManagementServerException : LocalizedException
	{
		public RightsManagementServerException(LocalizedString message, bool isPermanentFailure) : base(message)
		{
			this.isPermanentFailure = isPermanentFailure;
			this.wellKnownErrorCode = 0;
		}

		public RightsManagementServerException(LocalizedString message, WellKnownErrorCode wellKnownErrorCode, bool isPermanentFailure) : base(message)
		{
			this.wellKnownErrorCode = wellKnownErrorCode;
			this.isPermanentFailure = isPermanentFailure;
		}

		public RightsManagementServerException(string info, bool isPermanentFailure) : base(new LocalizedString(info))
		{
			this.isPermanentFailure = isPermanentFailure;
			this.wellKnownErrorCode = 0;
		}

		public RightsManagementServerException(LocalizedString message, CoreException innerException) : base(message, innerException)
		{
			this.wellKnownErrorCode = innerException.ErrorCode;
			this.isPermanentFailure = innerException.IsPermanentFailure;
		}

		public RightsManagementServerException(LocalizedString message, BaseException innerException) : base(message, innerException)
		{
			this.isPermanentFailure = innerException.IsPermanentFailure;
			this.wellKnownErrorCode = 0;
		}

		public RightsManagementServerException(LocalizedString message, Exception innerException, bool isPermanentFailure) : base(message, innerException)
		{
			this.isPermanentFailure = isPermanentFailure;
			this.wellKnownErrorCode = 0;
		}

		public bool IsPermanentFailure
		{
			get
			{
				return this.isPermanentFailure;
			}
		}

		public WellKnownErrorCode WellKnownErrorCode
		{
			get
			{
				return this.wellKnownErrorCode;
			}
		}

		protected RightsManagementServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.isPermanentFailure = info.GetBoolean("IsPermanentFailure");
			this.wellKnownErrorCode = (WellKnownErrorCode)info.GetValue("WellKnownErrorCode", this.wellKnownErrorCode.GetType());
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("IsPermanentFailure", this.isPermanentFailure);
			info.AddValue("WellKnownErrorCode", this.wellKnownErrorCode);
		}

		private const string SerializationIsPermanent = "IsPermanentFailure";

		private const string SerializationErrorCode = "WellKnownErrorCode";

		private readonly bool isPermanentFailure;

		private readonly WellKnownErrorCode wellKnownErrorCode;
	}
}
