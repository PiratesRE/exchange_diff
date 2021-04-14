using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionBitmask : Restriction
	{
		public RestrictionBitmask(StorePropTag propertyTag, long mask, BitmaskOperation operation)
		{
			RestrictionBitmask.ValidateOperation(operation);
			RestrictionBitmask.ValidatePropertyTag(propertyTag);
			this.propertyTag = propertyTag;
			this.mask = mask;
			this.operation = operation;
		}

		public RestrictionBitmask(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.operation = (BitmaskOperation)ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.propertyTag = Mailbox.GetStorePropTag(context, mailbox, ParseSerialize.GetDword(byteForm, ref position, posMax), objectType);
			this.mask = (long)((ulong)ParseSerialize.GetDword(byteForm, ref position, posMax));
			RestrictionBitmask.ValidateOperation(this.operation);
			RestrictionBitmask.ValidatePropertyTag(this.propertyTag);
		}

		public StorePropTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public BitmaskOperation Operation
		{
			get
			{
				return this.operation;
			}
		}

		public long Mask
		{
			get
			{
				return this.mask;
			}
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("BITMASK([");
			this.propertyTag.AppendToString(sb);
			sb.Append("] & 0x");
			sb.Append(this.mask.ToString("X16"));
			if (this.operation == BitmaskOperation.EqualToZero)
			{
				sb.Append(" = 0");
			}
			else
			{
				sb.Append(" <> 0");
			}
			sb.Append(")");
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 6);
			ParseSerialize.SetByte(byteForm, ref position, (byte)this.operation);
			ParseSerialize.SetDword(byteForm, ref position, this.propertyTag.ExternalTag);
			ParseSerialize.SetDword(byteForm, ref position, (int)this.mask);
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			object value;
			if (this.propertyTag.PropType == PropertyType.Int16)
			{
				value = (short)this.mask;
			}
			else if (this.propertyTag.PropType == PropertyType.Int32)
			{
				value = (int)this.mask;
			}
			else
			{
				if (this.propertyTag.PropType != PropertyType.Int64)
				{
					return Factory.CreateSearchCriteriaFalse();
				}
				value = this.mask;
			}
			Column lhs = PropertySchema.MapToColumn(database, objectType, this.propertyTag);
			Column rhs = Factory.CreateConstantColumn(value);
			SearchCriteriaBitMask.SearchBitMaskOp op = (this.operation == BitmaskOperation.EqualToZero) ? SearchCriteriaBitMask.SearchBitMaskOp.EqualToZero : SearchCriteriaBitMask.SearchBitMaskOp.NotEqualToZero;
			return Factory.CreateSearchCriteriaBitMask(lhs, rhs, op);
		}

		public override bool RefersToProperty(StorePropTag propTag)
		{
			return this.propertyTag == propTag;
		}

		private static void ValidateOperation(BitmaskOperation operation)
		{
			if (operation != BitmaskOperation.EqualToZero && operation != BitmaskOperation.NotEqualToZero)
			{
				throw new StoreException((LID)33272U, ErrorCodeValue.TooComplex);
			}
		}

		private static void ValidatePropertyTag(StorePropTag propertyTag)
		{
			PropertyType propType = propertyTag.PropType;
			if (propType != PropertyType.Int16 && propType != PropertyType.Int32 && propType != PropertyType.Int64)
			{
				throw new StoreException((LID)57848U, ErrorCodeValue.TooComplex);
			}
		}

		private readonly StorePropTag propertyTag;

		private readonly long mask;

		private readonly BitmaskOperation operation;
	}
}
