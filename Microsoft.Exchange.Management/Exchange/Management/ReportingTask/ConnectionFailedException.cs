﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConnectionFailedException : ReportingException
	{
		public ConnectionFailedException(int number) : base(Strings.ConnectionFailedException(number))
		{
			this.number = number;
		}

		public ConnectionFailedException(int number, Exception innerException) : base(Strings.ConnectionFailedException(number), innerException)
		{
			this.number = number;
		}

		protected ConnectionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.number = (int)info.GetValue("number", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("number", this.number);
		}

		public int Number
		{
			get
			{
				return this.number;
			}
		}

		private readonly int number;
	}
}
