using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToReEnrollTPDException : LocalizedException
	{
		public FailedToReEnrollTPDException(Exception e) : base(Strings.FailedToReEnrollTPD(e))
		{
			this.e = e;
		}

		public FailedToReEnrollTPDException(Exception e, Exception innerException) : base(Strings.FailedToReEnrollTPD(e), innerException)
		{
			this.e = e;
		}

		protected FailedToReEnrollTPDException(SerializationInfo info, StreamingContext context) : base(info, context)
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
