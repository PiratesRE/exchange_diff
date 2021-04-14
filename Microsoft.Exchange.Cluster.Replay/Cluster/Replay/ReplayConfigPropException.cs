using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayConfigPropException : TransientException
	{
		public ReplayConfigPropException(string id, string propertyName) : base(ReplayStrings.ReplayConfigPropException(id, propertyName))
		{
			this.id = id;
			this.propertyName = propertyName;
		}

		public ReplayConfigPropException(string id, string propertyName, Exception innerException) : base(ReplayStrings.ReplayConfigPropException(id, propertyName), innerException)
		{
			this.id = id;
			this.propertyName = propertyName;
		}

		protected ReplayConfigPropException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
			info.AddValue("propertyName", this.propertyName);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		private readonly string id;

		private readonly string propertyName;
	}
}
