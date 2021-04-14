using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionNEAR : Restriction
	{
		public RestrictionNEAR(int distance, bool ordered, RestrictionAND nestedRestriction)
		{
			RestrictionNEAR.ValidateRestriction(nestedRestriction);
			this.distance = distance;
			this.ordered = ordered;
			this.nestedRestriction = nestedRestriction;
		}

		public RestrictionNEAR(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.distance = (int)ParseSerialize.GetDword(byteForm, ref position, posMax);
			this.ordered = (ParseSerialize.GetDword(byteForm, ref position, posMax) == 1U);
			this.nestedRestriction = (Restriction.Deserialize(context, byteForm, ref position, posMax, mailbox, objectType) as RestrictionAND);
			RestrictionNEAR.ValidateRestriction(this.nestedRestriction);
		}

		public int Distance
		{
			get
			{
				return this.distance;
			}
		}

		public bool Ordered
		{
			get
			{
				return this.ordered;
			}
		}

		public RestrictionAND NestedRestriction
		{
			get
			{
				return this.nestedRestriction;
			}
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("NEAR(");
			if (this.nestedRestriction != null)
			{
				for (int i = 0; i < this.nestedRestriction.NestedRestrictions.Length; i++)
				{
					this.nestedRestriction.NestedRestrictions[i].AppendToString(sb);
					sb.Append(", ");
				}
			}
			sb.Append(this.distance);
			sb.Append(", ");
			sb.Append(this.ordered);
			sb.Append(")");
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 13);
			ParseSerialize.SetDword(byteForm, ref position, this.distance);
			ParseSerialize.SetDword(byteForm, ref position, this.ordered ? 1 : 0);
			this.nestedRestriction.Serialize(byteForm, ref position);
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			SearchCriteriaAnd criteria = this.nestedRestriction.ToSearchCriteria(database, objectType) as SearchCriteriaAnd;
			return Factory.CreateSearchCriteriaNear(this.distance, this.ordered, criteria);
		}

		private static void ValidateRestriction(Restriction nestedRestriction)
		{
			RestrictionAND restrictionAND = nestedRestriction as RestrictionAND;
			if (restrictionAND == null)
			{
				throw new StoreException((LID)55632U, ErrorCodeValue.TooComplex, "NEAR requires a non-null AND restriction");
			}
			if (restrictionAND.NestedRestrictions == null || restrictionAND.NestedRestrictions.Length < 2)
			{
				throw new StoreException((LID)51536U, ErrorCodeValue.TooComplex, "NEAR requires at least two restrictions");
			}
			if (!RestrictionNEAR.InspectNestedRestriction(restrictionAND.NestedRestrictions))
			{
				throw new StoreException((LID)43344U, ErrorCodeValue.TooComplex, "NEAR can only contain OR, NEAR or TEXT restrictions");
			}
		}

		private static bool InspectNestedRestriction(Restriction[] nestedRestrictions)
		{
			if (nestedRestrictions == null || nestedRestrictions.Length == 0)
			{
				return false;
			}
			foreach (Restriction restriction in nestedRestrictions)
			{
				if (restriction is RestrictionOR)
				{
					if (!RestrictionNEAR.InspectNestedRestriction(((RestrictionOR)restriction).NestedRestrictions))
					{
						return false;
					}
				}
				else if (!(restriction is RestrictionTextProperty) && !(restriction is RestrictionNEAR))
				{
					return false;
				}
			}
			return true;
		}

		public override bool HasClauseMeetingPredicate(Predicate<Restriction> predicate)
		{
			return predicate(this) || (this.nestedRestriction != null && this.nestedRestriction.HasClauseMeetingPredicate(predicate));
		}

		protected override Restriction InspectAndFixChildren(Restriction.InspectAndFixRestrictionDelegate callback)
		{
			Restriction restriction = this.nestedRestriction.InspectAndFix(callback);
			if (!object.ReferenceEquals(restriction, this.nestedRestriction))
			{
				return new RestrictionNEAR(this.distance, this.ordered, (RestrictionAND)restriction);
			}
			return this;
		}

		private readonly int distance;

		private readonly bool ordered;

		private readonly RestrictionAND nestedRestriction;
	}
}
