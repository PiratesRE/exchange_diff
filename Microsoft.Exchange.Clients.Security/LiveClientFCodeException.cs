using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Clients.Security
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LiveClientFCodeException : LiveClientException
	{
		public LiveClientFCodeException(int fCode, string msppErrorString) : base(Strings.LiveClientFCodeException(fCode, msppErrorString))
		{
			this.fCode = fCode;
			this.msppErrorString = msppErrorString;
		}

		public LiveClientFCodeException(int fCode, string msppErrorString, Exception innerException) : base(Strings.LiveClientFCodeException(fCode, msppErrorString), innerException)
		{
			this.fCode = fCode;
			this.msppErrorString = msppErrorString;
		}

		protected LiveClientFCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fCode = (int)info.GetValue("fCode", typeof(int));
			this.msppErrorString = (string)info.GetValue("msppErrorString", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fCode", this.fCode);
			info.AddValue("msppErrorString", this.msppErrorString);
		}

		public int FCode
		{
			get
			{
				return this.fCode;
			}
		}

		public string MsppErrorString
		{
			get
			{
				return this.msppErrorString;
			}
		}

		private readonly int fCode;

		private readonly string msppErrorString;
	}
}
