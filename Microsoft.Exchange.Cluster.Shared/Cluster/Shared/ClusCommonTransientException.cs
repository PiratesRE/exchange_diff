using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusCommonTransientException : TransientException
	{
		public ClusCommonTransientException(string error) : base(Strings.ClusCommonTransientException(error))
		{
			this.error = error;
		}

		public ClusCommonTransientException(string error, Exception innerException) : base(Strings.ClusCommonTransientException(error), innerException)
		{
			this.error = error;
		}

		protected ClusCommonTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
