using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidResourceOpException : ClusCommonFailException
	{
		public InvalidResourceOpException(string resName) : base(Strings.InvalidResourceOpException(resName))
		{
			this.resName = resName;
		}

		public InvalidResourceOpException(string resName, Exception innerException) : base(Strings.InvalidResourceOpException(resName), innerException)
		{
			this.resName = resName;
		}

		protected InvalidResourceOpException(SerializationInfo info, StreamingContext context) : base(info, context)
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
