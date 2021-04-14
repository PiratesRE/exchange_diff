using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoveDagConfigurationNeedsZeroDagsException : LocalizedException
	{
		public RemoveDagConfigurationNeedsZeroDagsException(int dagCount) : base(Strings.RemoveDagConfigurationNeedsZeroDagsException(dagCount))
		{
			this.dagCount = dagCount;
		}

		public RemoveDagConfigurationNeedsZeroDagsException(int dagCount, Exception innerException) : base(Strings.RemoveDagConfigurationNeedsZeroDagsException(dagCount), innerException)
		{
			this.dagCount = dagCount;
		}

		protected RemoveDagConfigurationNeedsZeroDagsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagCount = (int)info.GetValue("dagCount", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagCount", this.dagCount);
		}

		public int DagCount
		{
			get
			{
				return this.dagCount;
			}
		}

		private readonly int dagCount;
	}
}
