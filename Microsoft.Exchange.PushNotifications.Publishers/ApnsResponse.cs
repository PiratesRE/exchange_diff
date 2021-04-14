using System;
using System.Net;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsResponse
	{
		private ApnsResponse()
		{
			this.origStatus = byte.MaxValue;
			this.Status = ApnsResponseStatus.Unknown;
		}

		public int Identifier { get; private set; }

		public ApnsResponseStatus Status { get; private set; }

		public static ApnsResponse FromApnsFormat(byte[] binaryForm)
		{
			if (binaryForm == null || binaryForm.Length <= 0)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (binaryForm.Length != 6)
			{
				throw new ArgumentException(string.Format("Unexpected number of bytes: {0}", binaryForm.Length), "binaryForm");
			}
			ApnsResponse apnsResponse = new ApnsResponse();
			apnsResponse.origStatus = binaryForm[1];
			if (apnsResponse.origStatus <= 8)
			{
				apnsResponse.Status = (ApnsResponseStatus)apnsResponse.origStatus;
			}
			apnsResponse.Identifier = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(binaryForm, 2));
			return apnsResponse;
		}

		public override string ToString()
		{
			if (this.toStringCache == null)
			{
				this.toStringCache = string.Format("{{id:{0}; status:{1}; origStatus:{2}}}", this.Identifier.ToString(), this.Status.ToString(), this.origStatus.ToString());
			}
			return this.toStringCache;
		}

		public const int Length = 6;

		private byte origStatus;

		private string toStringCache;
	}
}
