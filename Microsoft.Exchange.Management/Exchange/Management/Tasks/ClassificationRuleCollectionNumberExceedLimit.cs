using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClassificationRuleCollectionNumberExceedLimit : LocalizedException
	{
		public ClassificationRuleCollectionNumberExceedLimit(int limit) : base(Strings.ClassificationRuleCollectionNumberExceedLimit(limit))
		{
			this.limit = limit;
		}

		public ClassificationRuleCollectionNumberExceedLimit(int limit, Exception innerException) : base(Strings.ClassificationRuleCollectionNumberExceedLimit(limit), innerException)
		{
			this.limit = limit;
		}

		protected ClassificationRuleCollectionNumberExceedLimit(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.limit = (int)info.GetValue("limit", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("limit", this.limit);
		}

		public int Limit
		{
			get
			{
				return this.limit;
			}
		}

		private readonly int limit;
	}
}
