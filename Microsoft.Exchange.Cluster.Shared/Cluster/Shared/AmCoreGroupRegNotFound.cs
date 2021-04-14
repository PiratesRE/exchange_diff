using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmCoreGroupRegNotFound : ClusterException
	{
		public AmCoreGroupRegNotFound(string regvalueName) : base(Strings.AmCoreGroupRegNotFound(regvalueName))
		{
			this.regvalueName = regvalueName;
		}

		public AmCoreGroupRegNotFound(string regvalueName, Exception innerException) : base(Strings.AmCoreGroupRegNotFound(regvalueName), innerException)
		{
			this.regvalueName = regvalueName;
		}

		protected AmCoreGroupRegNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.regvalueName = (string)info.GetValue("regvalueName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("regvalueName", this.regvalueName);
		}

		public string RegvalueName
		{
			get
			{
				return this.regvalueName;
			}
		}

		private readonly string regvalueName;
	}
}
