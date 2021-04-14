using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnexpectedSmtpServerResponseException : LocalizedException
	{
		public UnexpectedSmtpServerResponseException(int expectedResponseCode, int actualResponseCode, string wholeResponse) : base(NetException.UnexpectedSmtpServerResponseException(expectedResponseCode, actualResponseCode, wholeResponse))
		{
			this.expectedResponseCode = expectedResponseCode;
			this.actualResponseCode = actualResponseCode;
			this.wholeResponse = wholeResponse;
		}

		public UnexpectedSmtpServerResponseException(int expectedResponseCode, int actualResponseCode, string wholeResponse, Exception innerException) : base(NetException.UnexpectedSmtpServerResponseException(expectedResponseCode, actualResponseCode, wholeResponse), innerException)
		{
			this.expectedResponseCode = expectedResponseCode;
			this.actualResponseCode = actualResponseCode;
			this.wholeResponse = wholeResponse;
		}

		protected UnexpectedSmtpServerResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.expectedResponseCode = (int)info.GetValue("expectedResponseCode", typeof(int));
			this.actualResponseCode = (int)info.GetValue("actualResponseCode", typeof(int));
			this.wholeResponse = (string)info.GetValue("wholeResponse", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("expectedResponseCode", this.expectedResponseCode);
			info.AddValue("actualResponseCode", this.actualResponseCode);
			info.AddValue("wholeResponse", this.wholeResponse);
		}

		public int ExpectedResponseCode
		{
			get
			{
				return this.expectedResponseCode;
			}
		}

		public int ActualResponseCode
		{
			get
			{
				return this.actualResponseCode;
			}
		}

		public string WholeResponse
		{
			get
			{
				return this.wholeResponse;
			}
		}

		private readonly int expectedResponseCode;

		private readonly int actualResponseCode;

		private readonly string wholeResponse;
	}
}
