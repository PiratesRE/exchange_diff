using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClassificationRuleCollectionInternalValidationException : LocalizedException
	{
		public ClassificationRuleCollectionInternalValidationException(int error) : base(Strings.ClassificationRuleCollectionInternalFailure(error))
		{
			this.error = error;
		}

		public ClassificationRuleCollectionInternalValidationException(int error, Exception innerException) : base(Strings.ClassificationRuleCollectionInternalFailure(error), innerException)
		{
			this.error = error;
		}

		protected ClassificationRuleCollectionInternalValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (int)info.GetValue("error", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public int Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly int error;
	}
}
