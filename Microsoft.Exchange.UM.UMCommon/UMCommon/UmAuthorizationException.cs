using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UmAuthorizationException : LocalizedException
	{
		public UmAuthorizationException(string user, string activity) : base(Strings.UmAuthorizationException(user, activity))
		{
			this.user = user;
			this.activity = activity;
		}

		public UmAuthorizationException(string user, string activity, Exception innerException) : base(Strings.UmAuthorizationException(user, activity), innerException)
		{
			this.user = user;
			this.activity = activity;
		}

		protected UmAuthorizationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
			this.activity = (string)info.GetValue("activity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
			info.AddValue("activity", this.activity);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public string Activity
		{
			get
			{
				return this.activity;
			}
		}

		private readonly string user;

		private readonly string activity;
	}
}
