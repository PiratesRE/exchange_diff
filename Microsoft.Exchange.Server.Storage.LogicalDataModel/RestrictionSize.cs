using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionSize : Restriction
	{
		public RestrictionSize(StorePropTag propertyTag, RelationOperator op, int value)
		{
			this.propertyTag = propertyTag;
			this.op = op;
			this.value = value;
		}

		public RestrictionSize(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.op = (RelationOperator)ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.propertyTag = Mailbox.GetStorePropTag(context, mailbox, ParseSerialize.GetDword(byteForm, ref position, posMax), objectType);
			this.value = (int)ParseSerialize.GetDword(byteForm, ref position, posMax);
			Restriction.CheckRelationOperator(this.op);
		}

		public StorePropTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public RelationOperator Operator
		{
			get
			{
				return this.op;
			}
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("SIZE([");
			this.propertyTag.AppendToString(sb);
			sb.Append("], ");
			sb.Append(this.op);
			sb.Append(", ");
			sb.Append(this.value.ToString());
			sb.Append(')');
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 7);
			ParseSerialize.SetByte(byteForm, ref position, (byte)this.op);
			ParseSerialize.SetDword(byteForm, ref position, this.propertyTag.ExternalTag);
			ParseSerialize.SetDword(byteForm, ref position, this.value);
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			Column termColumn = PropertySchema.MapToColumn(database, objectType, this.propertyTag);
			Column lhs = Factory.CreateSizeOfColumn(termColumn);
			Column rhs = Factory.CreateConstantColumn(this.value);
			SearchCriteriaCompare.SearchRelOp searchRelOp = Restriction.GetSearchRelOp(this.op);
			return Factory.CreateSearchCriteriaCompare(lhs, searchRelOp, rhs);
		}

		public override bool RefersToProperty(StorePropTag propTag)
		{
			return this.propertyTag == propTag;
		}

		private readonly StorePropTag propertyTag;

		private readonly int value;

		private readonly RelationOperator op;
	}
}
