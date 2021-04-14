using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class VerificationCodeUnmatchException : LocalizedException
	{
		public VerificationCodeUnmatchException(string passcode) : base(Strings.ErrorVerificationCodeUnmatch(passcode))
		{
			this.passcode = passcode;
		}

		public VerificationCodeUnmatchException(string passcode, Exception innerException) : base(Strings.ErrorVerificationCodeUnmatch(passcode), innerException)
		{
			this.passcode = passcode;
		}

		protected VerificationCodeUnmatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.passcode = (string)info.GetValue("passcode", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("passcode", this.passcode);
		}

		public string Passcode
		{
			get
			{
				return this.passcode;
			}
		}

		private readonly string passcode;
	}
}
