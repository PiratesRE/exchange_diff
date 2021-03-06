using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class VerificationCodeSentTooManyTimesException : LocalizedException
	{
		public VerificationCodeSentTooManyTimesException(string phonenumber) : base(Strings.ErrorVerificationCodeSentTooManyTimes(phonenumber))
		{
			this.phonenumber = phonenumber;
		}

		public VerificationCodeSentTooManyTimesException(string phonenumber, Exception innerException) : base(Strings.ErrorVerificationCodeSentTooManyTimes(phonenumber), innerException)
		{
			this.phonenumber = phonenumber;
		}

		protected VerificationCodeSentTooManyTimesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.phonenumber = (string)info.GetValue("phonenumber", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("phonenumber", this.phonenumber);
		}

		public string Phonenumber
		{
			get
			{
				return this.phonenumber;
			}
		}

		private readonly string phonenumber;
	}
}
