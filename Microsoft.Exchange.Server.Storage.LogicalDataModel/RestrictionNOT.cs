using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionNOT : Restriction
	{
		public RestrictionNOT(Restriction nestedRestriction)
		{
			if (nestedRestriction == null)
			{
				nestedRestriction = new RestrictionFalse();
			}
			this.nestedRestriction = nestedRestriction;
		}

		public RestrictionNOT(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.nestedRestriction = Restriction.Deserialize(context, byteForm, ref position, posMax, mailbox, objectType);
		}

		public Restriction NestedRestriction
		{
			get
			{
				return this.nestedRestriction;
			}
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("NOT(");
			if (this.nestedRestriction != null)
			{
				this.nestedRestriction.AppendToString(sb);
			}
			sb.Append(")");
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 2);
			this.NestedRestriction.Serialize(byteForm, ref position);
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			SearchCriteria criteria = this.NestedRestriction.ToSearchCriteria(database, objectType);
			return Factory.CreateSearchCriteriaNot(criteria);
		}

		public override bool HasClauseMeetingPredicate(Predicate<Restriction> predicate)
		{
			return predicate(this) || (this.nestedRestriction != null && this.nestedRestriction.HasClauseMeetingPredicate(predicate));
		}

		protected override Restriction InspectAndFixChildren(Restriction.InspectAndFixRestrictionDelegate callback)
		{
			Restriction objA = this.nestedRestriction.InspectAndFix(callback);
			if (!object.ReferenceEquals(objA, this.nestedRestriction))
			{
				return new RestrictionNOT(objA);
			}
			return this;
		}

		private readonly Restriction nestedRestriction;
	}
}
