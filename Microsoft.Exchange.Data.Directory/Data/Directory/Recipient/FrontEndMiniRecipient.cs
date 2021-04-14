using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public class FrontEndMiniRecipient : MiniObject
	{
		internal FrontEndMiniRecipient(IRecipientSession session, PropertyBag propertyBag)
		{
			this.m_Session = session;
			this.propertyBag = (ADPropertyBag)propertyBag;
		}

		public FrontEndMiniRecipient()
		{
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[FrontEndMiniRecipientSchema.ExchangeGuid];
			}
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[FrontEndMiniRecipientSchema.Database];
			}
		}

		public ADObjectId ArchiveDatabase
		{
			get
			{
				return (ADObjectId)this[FrontEndMiniRecipientSchema.ArchiveDatabase];
			}
		}

		public DateTime? LastExchangeChangedTime
		{
			get
			{
				return (DateTime?)this[FrontEndMiniRecipientSchema.LastExchangeChangedTime];
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return FrontEndMiniRecipient.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return FrontEndMiniRecipient.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return FrontEndMiniRecipient.implicitFilterInternal;
			}
		}

		private static FrontEndMiniRecipientSchema schema = ObjectSchema.GetInstance<FrontEndMiniRecipientSchema>();

		private static string mostDerivedClass = "user";

		private static string objectCategoryNameInternal = "person";

		private static QueryFilter implicitFilterInternal = new AndFilter(new QueryFilter[]
		{
			ADObject.ObjectClassFilter(FrontEndMiniRecipient.mostDerivedClass, true),
			ADObject.ObjectCategoryFilter(FrontEndMiniRecipient.objectCategoryNameInternal)
		});
	}
}
