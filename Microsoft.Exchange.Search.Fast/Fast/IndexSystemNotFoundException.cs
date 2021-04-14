using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Fast
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class IndexSystemNotFoundException : ComponentFailedPermanentException
	{
		public IndexSystemNotFoundException(string flowName) : base(Strings.IndexSystemForFlowDoesNotExist(flowName))
		{
			this.flowName = flowName;
		}

		public IndexSystemNotFoundException(string flowName, Exception innerException) : base(Strings.IndexSystemForFlowDoesNotExist(flowName), innerException)
		{
			this.flowName = flowName;
		}

		protected IndexSystemNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.flowName = (string)info.GetValue("flowName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("flowName", this.flowName);
		}

		public string FlowName
		{
			get
			{
				return this.flowName;
			}
		}

		private readonly string flowName;
	}
}
