using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToDkmProtectPrivateKeyException : LocalizedException
	{
		public FailedToDkmProtectPrivateKeyException(Exception ex) : base(Strings.FailedToDkmProtectPrivateKey(ex))
		{
			this.ex = ex;
		}

		public FailedToDkmProtectPrivateKeyException(Exception ex, Exception innerException) : base(Strings.FailedToDkmProtectPrivateKey(ex), innerException)
		{
			this.ex = ex;
		}

		protected FailedToDkmProtectPrivateKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ex = (Exception)info.GetValue("ex", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ex", this.ex);
		}

		public Exception Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly Exception ex;
	}
}
