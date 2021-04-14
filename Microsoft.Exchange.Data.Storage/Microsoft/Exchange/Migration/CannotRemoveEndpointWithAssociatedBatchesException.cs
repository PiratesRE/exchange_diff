using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotRemoveEndpointWithAssociatedBatchesException : MigrationTransientException
	{
		public CannotRemoveEndpointWithAssociatedBatchesException(string endpointId, string batches) : base(Strings.ErrorCannotRemoveEndpointWithAssociatedBatches(endpointId, batches))
		{
			this.endpointId = endpointId;
			this.batches = batches;
		}

		public CannotRemoveEndpointWithAssociatedBatchesException(string endpointId, string batches, Exception innerException) : base(Strings.ErrorCannotRemoveEndpointWithAssociatedBatches(endpointId, batches), innerException)
		{
			this.endpointId = endpointId;
			this.batches = batches;
		}

		protected CannotRemoveEndpointWithAssociatedBatchesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.endpointId = (string)info.GetValue("endpointId", typeof(string));
			this.batches = (string)info.GetValue("batches", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("endpointId", this.endpointId);
			info.AddValue("batches", this.batches);
		}

		public string EndpointId
		{
			get
			{
				return this.endpointId;
			}
		}

		public string Batches
		{
			get
			{
				return this.batches;
			}
		}

		private readonly string endpointId;

		private readonly string batches;
	}
}
