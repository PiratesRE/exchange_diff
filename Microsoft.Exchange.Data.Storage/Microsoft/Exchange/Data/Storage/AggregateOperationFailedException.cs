using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class AggregateOperationFailedException : StoragePermanentException
	{
		public AggregateOperationFailedException(LocalizedString message, AggregateOperationResult aggregateOperationResult) : base(message)
		{
			this.aggregateOperationResult = aggregateOperationResult;
		}

		protected AggregateOperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.aggregateOperationResult = (AggregateOperationResult)info.GetValue("aggregateOperationResult", typeof(AggregateOperationResult));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("aggregateOperationResult", this.aggregateOperationResult);
		}

		public AggregateOperationResult AggregateOperationResult
		{
			get
			{
				return this.aggregateOperationResult;
			}
		}

		private const string AggregateOperationResultLabel = "aggregateOperationResult";

		private readonly AggregateOperationResult aggregateOperationResult;
	}
}
