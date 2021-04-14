using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Clients.Security
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LiveExternalUnknownFCodeException : LiveExternalException
	{
		public LiveExternalUnknownFCodeException(string fCodeString, string msppErrorString) : base(Strings.LiveExternalUnknownFCodeException(fCodeString, msppErrorString))
		{
			this.fCodeString = fCodeString;
			this.msppErrorString = msppErrorString;
		}

		public LiveExternalUnknownFCodeException(string fCodeString, string msppErrorString, Exception innerException) : base(Strings.LiveExternalUnknownFCodeException(fCodeString, msppErrorString), innerException)
		{
			this.fCodeString = fCodeString;
			this.msppErrorString = msppErrorString;
		}

		protected LiveExternalUnknownFCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fCodeString = (string)info.GetValue("fCodeString", typeof(string));
			this.msppErrorString = (string)info.GetValue("msppErrorString", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fCodeString", this.fCodeString);
			info.AddValue("msppErrorString", this.msppErrorString);
		}

		public string FCodeString
		{
			get
			{
				return this.fCodeString;
			}
		}

		public string MsppErrorString
		{
			get
			{
				return this.msppErrorString;
			}
		}

		private readonly string fCodeString;

		private readonly string msppErrorString;
	}
}
