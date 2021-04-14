using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionCount : Restriction
	{
		public RestrictionCount(int count, Restriction nestedRestriction)
		{
			this.count = RestrictionCount.SanitizeCount(count);
			this.nestedRestriction = nestedRestriction;
		}

		public RestrictionCount(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.count = RestrictionCount.SanitizeCount((int)ParseSerialize.GetDword(byteForm, ref position, posMax));
			this.nestedRestriction = Restriction.Deserialize(context, byteForm, ref position, posMax, mailbox, objectType);
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public Restriction NestedRestriction
		{
			get
			{
				return this.nestedRestriction;
			}
		}

		private static int SanitizeCount(int count)
		{
			return Math.Max(count, 0);
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("COUNT(");
			sb.Append(this.count);
			sb.Append(", ");
			if (this.nestedRestriction != null)
			{
				this.nestedRestriction.AppendToString(sb);
			}
			else
			{
				sb.Append("NULL");
			}
			sb.Append(")");
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 11);
			ParseSerialize.SetDword(byteForm, ref position, this.count);
			this.nestedRestriction.Serialize(byteForm, ref position);
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			return this.nestedRestriction.ToSearchCriteria(database, objectType);
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
				return new RestrictionCount(this.count, objA);
			}
			return this;
		}

		private readonly int count;

		private readonly Restriction nestedRestriction;
	}
}
