using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClassificationRuleCollectionPayloadSizeExceededLimitException : ClassificationRuleCollectionValidationException
	{
		public ClassificationRuleCollectionPayloadSizeExceededLimitException(ulong inputSize, ulong limitSize) : base(Strings.ClassificationRuleCollectionPayloadSizeExceededLimitFailure(inputSize, limitSize))
		{
			this.inputSize = inputSize;
			this.limitSize = limitSize;
		}

		public ClassificationRuleCollectionPayloadSizeExceededLimitException(ulong inputSize, ulong limitSize, Exception innerException) : base(Strings.ClassificationRuleCollectionPayloadSizeExceededLimitFailure(inputSize, limitSize), innerException)
		{
			this.inputSize = inputSize;
			this.limitSize = limitSize;
		}

		protected ClassificationRuleCollectionPayloadSizeExceededLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.inputSize = (ulong)info.GetValue("inputSize", typeof(ulong));
			this.limitSize = (ulong)info.GetValue("limitSize", typeof(ulong));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("inputSize", this.inputSize);
			info.AddValue("limitSize", this.limitSize);
		}

		public ulong InputSize
		{
			get
			{
				return this.inputSize;
			}
		}

		public ulong LimitSize
		{
			get
			{
				return this.limitSize;
			}
		}

		private readonly ulong inputSize;

		private readonly ulong limitSize;
	}
}
