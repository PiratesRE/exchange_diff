using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OperationValidOnlyOnLonghornException : ClusCommonFailException
	{
		public OperationValidOnlyOnLonghornException(string resName) : base(Strings.OperationValidOnlyOnLonghornException(resName))
		{
			this.resName = resName;
		}

		public OperationValidOnlyOnLonghornException(string resName, Exception innerException) : base(Strings.OperationValidOnlyOnLonghornException(resName), innerException)
		{
			this.resName = resName;
		}

		protected OperationValidOnlyOnLonghornException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.resName = (string)info.GetValue("resName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("resName", this.resName);
		}

		public string ResName
		{
			get
			{
				return this.resName;
			}
		}

		private readonly string resName;
	}
}
