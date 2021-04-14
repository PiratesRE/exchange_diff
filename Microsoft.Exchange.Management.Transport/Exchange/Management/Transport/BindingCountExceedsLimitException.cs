using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class BindingCountExceedsLimitException : LocalizedException
	{
		public BindingCountExceedsLimitException(string workLoad, int limit) : base(Strings.BindingCountExceedsLimit(workLoad, limit))
		{
			this.workLoad = workLoad;
			this.limit = limit;
		}

		public BindingCountExceedsLimitException(string workLoad, int limit, Exception innerException) : base(Strings.BindingCountExceedsLimit(workLoad, limit), innerException)
		{
			this.workLoad = workLoad;
			this.limit = limit;
		}

		protected BindingCountExceedsLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.workLoad = (string)info.GetValue("workLoad", typeof(string));
			this.limit = (int)info.GetValue("limit", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("workLoad", this.workLoad);
			info.AddValue("limit", this.limit);
		}

		public string WorkLoad
		{
			get
			{
				return this.workLoad;
			}
		}

		public int Limit
		{
			get
			{
				return this.limit;
			}
		}

		private readonly string workLoad;

		private readonly int limit;
	}
}
