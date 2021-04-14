using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterUnsupportedRegistryTypeException : ClusterException
	{
		public ClusterUnsupportedRegistryTypeException(string typeName) : base(Strings.ClusterUnsupportedRegistryTypeException(typeName))
		{
			this.typeName = typeName;
		}

		public ClusterUnsupportedRegistryTypeException(string typeName, Exception innerException) : base(Strings.ClusterUnsupportedRegistryTypeException(typeName), innerException)
		{
			this.typeName = typeName;
		}

		protected ClusterUnsupportedRegistryTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.typeName = (string)info.GetValue("typeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("typeName", this.typeName);
		}

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		private readonly string typeName;
	}
}
