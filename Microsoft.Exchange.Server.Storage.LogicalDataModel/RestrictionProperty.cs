using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionProperty : Restriction
	{
		public RestrictionProperty(StorePropTag propertyTag, RelationOperator op, object value)
		{
			this.propertyTag = propertyTag;
			this.value = value;
			this.op = op;
		}

		public RestrictionProperty(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.op = (RelationOperator)ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.propertyTag = Mailbox.GetStorePropTag(context, mailbox, ParseSerialize.GetDword(byteForm, ref position, posMax), objectType);
			StorePropTag storePropTag = Mailbox.GetStorePropTag(context, mailbox, ParseSerialize.GetDword(byteForm, ref position, posMax), objectType);
			this.value = Restriction.DeserializeValue(byteForm, ref position, storePropTag);
			Restriction.CheckRelationOperator(this.op);
			if (!Restriction.FSamePropType(storePropTag.PropType, this.propertyTag.PropType))
			{
				throw new StoreException((LID)63480U, ErrorCodeValue.TooComplex);
			}
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

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("PROPERTY([");
			this.propertyTag.AppendToString(sb);
			sb.Append("], ");
			sb.Append(this.op);
			sb.Append(", ");
			sb.AppendAsString(this.value);
			sb.Append(')');
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 4);
			ParseSerialize.SetByte(byteForm, ref position, (byte)this.op);
			ParseSerialize.SetDword(byteForm, ref position, this.propertyTag.ExternalTag);
			StorePropTag propTag = this.propertyTag;
			if (propTag.IsMultiValued && (this.value == null || !this.value.GetType().IsArray || this.value is byte[]))
			{
				propTag = StorePropTag.CreateWithoutInfo(this.propertyTag.PropId, this.propertyTag.PropType & (PropertyType)61439, this.propertyTag.BaseObjectType);
			}
			else if (!propTag.IsMultiValued && this.value != null && this.value.GetType().IsArray && !(this.value is byte[]))
			{
				propTag = StorePropTag.CreateWithoutInfo(this.propertyTag.PropId, this.propertyTag.PropType | PropertyType.MVFlag, this.propertyTag.BaseObjectType);
			}
			ParseSerialize.SetDword(byteForm, ref position, propTag.ExternalTag);
			Restriction.SerializeValue(byteForm, ref position, propTag, this.value);
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			Column lhs = PropertySchema.MapToColumn(database, objectType, this.propertyTag);
			Column rhs = Factory.CreateConstantColumn(this.value);
			SearchCriteriaCompare.SearchRelOp searchRelOp = Restriction.GetSearchRelOp(this.op);
			return Factory.CreateSearchCriteriaCompare(lhs, searchRelOp, rhs);
		}

		public override bool RefersToProperty(StorePropTag propTag)
		{
			return this.propertyTag == propTag;
		}

		private readonly StorePropTag propertyTag;

		private readonly RelationOperator op;

		private readonly object value;
	}
}
