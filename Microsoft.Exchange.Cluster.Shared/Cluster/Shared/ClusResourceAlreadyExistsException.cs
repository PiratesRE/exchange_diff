using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusResourceAlreadyExistsException : ClusCommonFailException
	{
		public ClusResourceAlreadyExistsException(string resourceName) : base(Strings.ClusResourceAlreadyExistsException(resourceName))
		{
			this.resourceName = resourceName;
		}

		public ClusResourceAlreadyExistsException(string resourceName, Exception innerException) : base(Strings.ClusResourceAlreadyExistsException(resourceName), innerException)
		{
			this.resourceName = resourceName;
		}

		protected ClusResourceAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.resourceName = (string)info.GetValue("resourceName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("resourceName", this.resourceName);
		}

		public string ResourceName
		{
			get
			{
				return this.resourceName;
			}
		}

		private readonly string resourceName;
	}
}
