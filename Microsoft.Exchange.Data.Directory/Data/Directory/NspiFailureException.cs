using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NspiFailureException : DataSourceOperationException
	{
		public NspiFailureException(int status) : base(DirectoryStrings.NspiFailureException(status))
		{
			this.status = status;
		}

		public NspiFailureException(int status, Exception innerException) : base(DirectoryStrings.NspiFailureException(status), innerException)
		{
			this.status = status;
		}

		protected NspiFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.status = (int)info.GetValue("status", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("status", this.status);
		}

		public int Status
		{
			get
			{
				return this.status;
			}
		}

		private readonly int status;
	}
}
