using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagNetworkManagementException : LocalizedException
	{
		public DagNetworkManagementException(string err) : base(ServerStrings.DagNetworkManagementError(err))
		{
			this.err = err;
		}

		public DagNetworkManagementException(string err, Exception innerException) : base(ServerStrings.DagNetworkManagementError(err), innerException)
		{
			this.err = err;
		}

		protected DagNetworkManagementException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.err = (string)info.GetValue("err", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("err", this.err);
		}

		public string Err
		{
			get
			{
				return this.err;
			}
		}

		private readonly string err;
	}
}
