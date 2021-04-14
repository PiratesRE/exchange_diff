using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.UnifiedGroups
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Office.Server.Directory")]
	public class Relation : SchemaObject
	{
		internal Relation(Guid relationTypeId, Guid targetObjectId) : base(relationTypeId)
		{
			this.InitializeRelation(relationTypeId, targetObjectId);
		}

		private Relation()
		{
		}

		[DataMember]
		public SchemaDictionary Properties
		{
			get
			{
				return this.properties;
			}
			private set
			{
				if (this.properties != null)
				{
					this.properties.InternalStorage = value.InternalStorage;
					return;
				}
				this.properties = value;
			}
		}

		private void InitializeRelation(Guid relationTypeId, Guid targetObjectId)
		{
			base.TypeId = relationTypeId;
			this.targetObjectId = targetObjectId;
			this.Properties = new SchemaDictionary<Relation>(this, "RelationProperties", null, null, null, null);
		}

		private SchemaDictionary properties;

		[DataMember]
		private Guid targetObjectId;
	}
}
