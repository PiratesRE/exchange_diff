using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Clients.Security
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LiveExternalFCodeException : LiveExternalException
	{
		public LiveExternalFCodeException(int fCode, string msppErrorString) : base(Strings.LiveExternalFCodeException(fCode, msppErrorString))
		{
			this.fCode = fCode;
			this.msppErrorString = msppErrorString;
		}

		public LiveExternalFCodeException(int fCode, string msppErrorString, Exception innerException) : base(Strings.LiveExternalFCodeException(fCode, msppErrorString), innerException)
		{
			this.fCode = fCode;
			this.msppErrorString = msppErrorString;
		}

		protected LiveExternalFCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
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
