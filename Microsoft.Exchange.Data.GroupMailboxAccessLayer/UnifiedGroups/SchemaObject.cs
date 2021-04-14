using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.UnifiedGroups
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Office.Server.Directory")]
	public class SchemaObject
	{
		protected SchemaObject()
		{
		}

		protected SchemaObject(Guid schemaTypeId)
		{
			this.TypeId = schemaTypeId;
		}

		[DataMember]
		public Guid TypeId { get; protected set; }

		[DataMember]
		public bool IsModified { get; internal set; }

		[DataMember]
		public bool IsInitialized { get; internal set; }

		internal virtual void ClearChanges()
		{
			this.IsModified = false;
		}
	}
}
