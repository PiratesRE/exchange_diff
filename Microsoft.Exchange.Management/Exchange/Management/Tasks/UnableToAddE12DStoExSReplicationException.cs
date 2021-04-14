using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToAddE12DStoExSReplicationException : LocalizedException
	{
		public UnableToAddE12DStoExSReplicationException(string dom) : base(Strings.UnableToAddE12DStoExSReplicationException(dom))
		{
			this.dom = dom;
		}

		public UnableToAddE12DStoExSReplicationException(string dom, Exception innerException) : base(Strings.UnableToAddE12DStoExSReplicationException(dom), innerException)
		{
			this.dom = dom;
		}

		protected UnableToAddE12DStoExSReplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dom = (string)info.GetValue("dom", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dom", this.dom);
		}

		public string Dom
		{
			get
			{
				return this.dom;
			}
		}

		private readonly string dom;
	}
}
