using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionExists : Restriction
	{
		public RestrictionExists(StorePropTag propertyTag)
		{
			this.propertyTag = propertyTag;
		}

		public RestrictionExists(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.propertyTag = Mailbox.GetStorePropTag(context, mailbox, ParseSerialize.GetDword(byteForm, ref position, posMax), objectType);
		}

		public StorePropTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("EXISTS([");
			this.propertyTag.AppendToString(sb);
			sb.Append("])");
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 8);
			ParseSerialize.SetDword(byteForm, ref position, this.propertyTag.ExternalTag);
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			Column column = PropertySchema.MapToColumn(database, objectType, this.propertyTag);
			if (!column.IsNullable)
			{
				return Factory.CreateSearchCriteriaTrue();
			}
			return Factory.CreateSearchCriteriaCompare(column, SearchCriteriaCompare.SearchRelOp.NotEqual, Factory.CreateConstantColumn(null, column));
		}

		public override bool RefersToProperty(StorePropTag propTag)
		{
			return this.propertyTag == propTag;
		}

		private readonly StorePropTag propertyTag;
	}
}
