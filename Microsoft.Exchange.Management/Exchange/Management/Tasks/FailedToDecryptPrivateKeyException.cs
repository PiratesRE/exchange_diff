using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToDecryptPrivateKeyException : LocalizedException
	{
		public FailedToDecryptPrivateKeyException(Exception e) : base(Strings.FailedToDecryptPrivateKey(e))
		{
			this.e = e;
		}

		public FailedToDecryptPrivateKeyException(Exception e, Exception innerException) : base(Strings.FailedToDecryptPrivateKey(e), innerException)
		{
			this.e = e;
		}

		protected FailedToDecryptPrivateKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.e = (Exception)info.GetValue("e", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("e", this.e);
		}

		public Exception E
		{
			get
			{
				return this.e;
			}
		}

		private readonly Exception e;
	}
}
