using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorUnableToFindFqdnForHostADErrorException : LocalizedException
	{
		public ErrorUnableToFindFqdnForHostADErrorException(string computerName, string ex) : base(Strings.ErrorUnableToFindFqdnForHostADErrorException(computerName, ex))
		{
			this.computerName = computerName;
			this.ex = ex;
		}

		public ErrorUnableToFindFqdnForHostADErrorException(string computerName, string ex, Exception innerException) : base(Strings.ErrorUnableToFindFqdnForHostADErrorException(computerName, ex), innerException)
		{
			this.computerName = computerName;
			this.ex = ex;
		}

		protected ErrorUnableToFindFqdnForHostADErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.computerName = (string)info.GetValue("computerName", typeof(string));
			this.ex = (string)info.GetValue("ex", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("computerName", this.computerName);
			info.AddValue("ex", this.ex);
		}

		public string ComputerName
		{
			get
			{
				return this.computerName;
			}
		}

		public string Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly string computerName;

		private readonly string ex;
	}
}
