﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UserAlreadyUmDisabledException : LocalizedException
	{
		public UserAlreadyUmDisabledException(string s) : base(Strings.ExceptionUserAlreadyUmDisabled(s))
		{
			this.s = s;
		}

		public UserAlreadyUmDisabledException(string s, Exception innerException) : base(Strings.ExceptionUserAlreadyUmDisabled(s), innerException)
		{
			this.s = s;
		}

		protected UserAlreadyUmDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.s = (string)info.GetValue("s", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("s", this.s);
		}

		public string S
		{
			get
			{
				return this.s;
			}
		}

		private readonly string s;
	}
}