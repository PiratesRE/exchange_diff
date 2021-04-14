using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Authorization
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CommonAccessTokenException : LocalizedException
	{
		public CommonAccessTokenException(int version, LocalizedString reason) : base(AuthorizationStrings.CommonAccessTokenException(version, reason))
		{
			this.version = version;
			this.reason = reason;
		}

		public CommonAccessTokenException(int version, LocalizedString reason, Exception innerException) : base(AuthorizationStrings.CommonAccessTokenException(version, reason), innerException)
		{
			this.version = version;
			this.reason = reason;
		}

		protected CommonAccessTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.version = (int)info.GetValue("version", typeof(int));
			this.reason = (LocalizedString)info.GetValue("reason", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("version", this.version);
			info.AddValue("reason", this.reason);
		}

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public LocalizedString Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly int version;

		private readonly LocalizedString reason;
	}
}
