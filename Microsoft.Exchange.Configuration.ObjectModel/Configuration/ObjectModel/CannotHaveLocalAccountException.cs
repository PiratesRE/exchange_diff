﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotHaveLocalAccountException : LocalizedException
	{
		public CannotHaveLocalAccountException(string user) : base(Strings.CannotHaveLocalAccountException(user))
		{
			this.user = user;
		}

		public CannotHaveLocalAccountException(string user, Exception innerException) : base(Strings.CannotHaveLocalAccountException(user), innerException)
		{
			this.user = user;
		}

		protected CannotHaveLocalAccountException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		private readonly string user;
	}
}
