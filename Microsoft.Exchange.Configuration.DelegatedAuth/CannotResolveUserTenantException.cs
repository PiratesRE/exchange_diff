﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.DelegatedAuthentication.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotResolveUserTenantException : LocalizedException
	{
		public CannotResolveUserTenantException(string userId) : base(Strings.CannotResolveUserTenantException(userId))
		{
			this.userId = userId;
		}

		public CannotResolveUserTenantException(string userId, Exception innerException) : base(Strings.CannotResolveUserTenantException(userId), innerException)
		{
			this.userId = userId;
		}

		protected CannotResolveUserTenantException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userId = (string)info.GetValue("userId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userId", this.userId);
		}

		public string UserId
		{
			get
			{
				return this.userId;
			}
		}

		private readonly string userId;
	}
}
