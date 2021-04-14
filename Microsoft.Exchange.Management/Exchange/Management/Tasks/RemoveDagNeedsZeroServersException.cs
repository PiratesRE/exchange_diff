using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoveDagNeedsZeroServersException : LocalizedException
	{
		public RemoveDagNeedsZeroServersException(int serverCount) : base(Strings.RemoveDagNeedsZeroServersException(serverCount))
		{
			this.serverCount = serverCount;
		}

		public RemoveDagNeedsZeroServersException(int serverCount, Exception innerException) : base(Strings.RemoveDagNeedsZeroServersException(serverCount), innerException)
		{
			this.serverCount = serverCount;
		}

		protected RemoveDagNeedsZeroServersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverCount = (int)info.GetValue("serverCount", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverCount", this.serverCount);
		}

		public int ServerCount
		{
			get
			{
				return this.serverCount;
			}
		}

		private readonly int serverCount;
	}
}
