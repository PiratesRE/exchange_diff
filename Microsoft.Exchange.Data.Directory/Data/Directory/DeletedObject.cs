using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public class DeletedObject : ADObject
	{
		public DeletedObject()
		{
		}

		internal DeletedObject(IDirectorySession session, PropertyBag propertyBag)
		{
			this.m_Session = session;
			this.propertyBag = (ADPropertyBag)propertyBag;
			base.ResetChangeTracking(true);
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return DeletedObject.filter;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return string.Empty;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return DeletedObject.schema;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
		}

		private static readonly DeletedObjectSchema schema = ObjectSchema.GetInstance<DeletedObjectSchema>();

		private static readonly QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DeletedObjectSchema.IsDeleted, true);
	}
}
