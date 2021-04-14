using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class VerificationCodeNeverSentException : LocalizedException
	{
		public VerificationCodeNeverSentException(string phonenumber) : base(Strings.ErrorVerificationCodeNeverSent(phonenumber))
		{
			this.phonenumber = phonenumber;
		}

		public VerificationCodeNeverSentException(string phonenumber, Exception innerException) : base(Strings.ErrorVerificationCodeNeverSent(phonenumber), innerException)
		{
			this.phonenumber = phonenumber;
		}

		protected VerificationCodeNeverSentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
