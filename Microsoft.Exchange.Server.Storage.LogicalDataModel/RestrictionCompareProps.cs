using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionCompareProps : Restriction
	{
		public RestrictionCompareProps(StorePropTag propertyTag1, RelationOperator op, StorePropTag propertyTag2)
		{
			this.propertyTag1 = propertyTag1;
			this.op = op;
			this.propertyTag2 = propertyTag2;
		}

		public RestrictionCompareProps(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.op = (RelationOperator)ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.propertyTag1 = Mailbox.GetStorePropTag(context, mailbox, ParseSerialize.GetDword(byteForm, ref position, posMax), objectType);
			this.propertyTag2 = Mailbox.GetStorePropTag(context, mailbox, ParseSerialize.GetDword(byteForm, ref position, posMax), objectType);
		}

		public StorePropTag PropertyTag1
		{
			get
			{
				return this.propertyTag1;
			}
		}

		public StorePropTag PropertyTag2
		{
			get
			{
				return this.propertyTag2;
			}
		}

		public RelationOperator Operator
		{
			get
			{
				return this.op;
			}
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("COMPAREPROPS([");
			this.propertyTag1.AppendToString(sb);
			sb.Append("], ");
			sb.Append(this.op);
			sb.Append(", [");
			this.propertyTag2.AppendToString(sb);
			sb.Append("])");
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 5);
			ParseSerialize.SetByte(byteForm, ref position, (byte)this.op);
			ParseSerialize.SetDword(byteForm, ref position, this.propertyTag1.ExternalTag);
			ParseSerialize.SetDword(byteForm, ref position, this.propertyTag2.ExternalTag);
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			Column lhs = PropertySchema.MapToColumn(database, objectType, this.propertyTag1);
			Column rhs = PropertySchema.MapToColumn(database, objectType, this.propertyTag2);
			SearchCriteriaCompare.SearchRelOp searchRelOp = Restriction.GetSearchRelOp(this.op);
			return Factory.CreateSearchCriteriaCompare(lhs, searchRelOp, rhs);
		}

		public override bool RefersToProperty(StorePropTag propTag)
		{
			return this.propertyTag1 == propTag || this.propertyTag2 == propTag;
		}

		private readonly StorePropTag propertyTag1;

		private readonly StorePropTag propertyTag2;

		private readonly RelationOperator op;
	}
}
