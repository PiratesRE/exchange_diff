using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TopologyServiceMissingDC : LocalizedException
	{
		public TopologyServiceMissingDC(string operationType) : base(Strings.messageTopologyServiceMissingDCExceptionThrown(operationType))
		{
			this.operationType = operationType;
		}

		public TopologyServiceMissingDC(string operationType, Exception innerException) : base(Strings.messageTopologyServiceMissingDCExceptionThrown(operationType), innerException)
		{
			this.operationType = operationType;
		}

		protected TopologyServiceMissingDC(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operationType = (string)info.GetValue("operationType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operationType", this.operationType);
		}

		public string OperationType
		{
			get
			{
				return this.operationType;
			}
		}

		private readonly string operationType;
	}
}
