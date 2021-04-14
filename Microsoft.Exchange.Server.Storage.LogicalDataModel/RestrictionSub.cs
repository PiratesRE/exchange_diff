using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionSub : Restriction
	{
		public RestrictionSub(int subObject, Restriction restriction)
		{
			this.subObject = subObject;
			this.nestedRestriction = restriction;
		}

		public RestrictionSub(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			this.subObject = (int)ParseSerialize.GetDword(byteForm, ref position, posMax);
			this.nestedRestriction = Restriction.Deserialize(context, byteForm, ref position, posMax, mailbox, objectType);
		}

		public int SubObject
		{
			get
			{
				return this.subObject;
			}
		}

		public Restriction Restriction
		{
			get
			{
				return this.nestedRestriction;
			}
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("SUB(");
			sb.Append(this.subObject);
			sb.Append(", ");
			if (this.nestedRestriction != null)
			{
				this.nestedRestriction.AppendToString(sb);
			}
			else
			{
				sb.Append("NULL");
			}
			sb.Append(')');
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 9);
			ParseSerialize.SetDword(byteForm, ref position, this.subObject);
			this.nestedRestriction.Serialize(byteForm, ref position);
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			if (objectType != ObjectType.Message || this.subObject != (int)PropTag.Message.MessageRecipients.PropTag)
			{
				return Factory.CreateSearchCriteriaFalse();
			}
			StorePropTag propertyTag = PropTag.Message.SearchRecipients;
			RestrictionTextProperty restrictionTextProperty = this.nestedRestriction as RestrictionTextProperty;
			RestrictionAND restrictionAND = this.nestedRestriction as RestrictionAND;
			RestrictionOR restrictionOR = this.nestedRestriction as RestrictionOR;
			if (restrictionTextProperty == null && restrictionAND == null && restrictionOR == null)
			{
				return Factory.CreateSearchCriteriaFalse();
			}
			if (restrictionTextProperty != null)
			{
				if (restrictionTextProperty.PropertyTag != PropTag.Message.DisplayName)
				{
					return Factory.CreateSearchCriteriaFalse();
				}
			}
			else if (restrictionAND != null)
			{
				if (restrictionAND.NestedRestrictions.Length != 2)
				{
					return Factory.CreateSearchCriteriaFalse();
				}
				RestrictionProperty restrictionProperty = restrictionAND.NestedRestrictions[0] as RestrictionProperty;
				restrictionTextProperty = (restrictionAND.NestedRestrictions[1] as RestrictionTextProperty);
				if (restrictionProperty == null)
				{
					restrictionProperty = (restrictionAND.NestedRestrictions[1] as RestrictionProperty);
					restrictionTextProperty = (restrictionAND.NestedRestrictions[0] as RestrictionTextProperty);
				}
				if (restrictionProperty == null || restrictionTextProperty == null || restrictionProperty.PropertyTag != PropTag.Message.RecipientType || restrictionTextProperty.PropertyTag != PropTag.Message.DisplayName)
				{
					return Factory.CreateSearchCriteriaFalse();
				}
				switch ((RecipientType)restrictionProperty.Value)
				{
				case RecipientType.To:
					propertyTag = PropTag.Message.SearchRecipientsTo;
					break;
				case RecipientType.Cc:
					propertyTag = PropTag.Message.SearchRecipientsCc;
					break;
				default:
					return Factory.CreateSearchCriteriaFalse();
				}
			}
			else if (restrictionOR != null)
			{
				if (restrictionOR.NestedRestrictions.Length != 2)
				{
					return Factory.CreateSearchCriteriaFalse();
				}
				IEnumerable<RestrictionTextProperty> source = from restriction in restrictionOR.NestedRestrictions
				select restriction as RestrictionTextProperty;
				RestrictionTextProperty restrictionTextProperty2 = (from restriction in source
				where restriction != null && restriction.PropertyTag == PropTag.Message.DisplayName
				select restriction).FirstOrDefault<RestrictionTextProperty>();
				RestrictionTextProperty restrictionTextProperty3 = (from restriction in source
				where restriction != null && restriction.PropertyTag == PropTag.Message.EmailAddress
				select restriction).FirstOrDefault<RestrictionTextProperty>();
				if (restrictionTextProperty2 == null || restrictionTextProperty3 == null || restrictionTextProperty2.FuzzyLevel != restrictionTextProperty3.FuzzyLevel || restrictionTextProperty2.Fullness != restrictionTextProperty3.Fullness || string.Compare(restrictionTextProperty2.Value as string, restrictionTextProperty3.Value as string, StringComparison.Ordinal) != 0)
				{
					return Factory.CreateSearchCriteriaFalse();
				}
				restrictionTextProperty = restrictionTextProperty2;
			}
			restrictionTextProperty = new RestrictionTextProperty(propertyTag, restrictionTextProperty.Value, restrictionTextProperty.FuzzyLevel, restrictionTextProperty.Fullness);
			return restrictionTextProperty.ToSearchCriteria(database, objectType);
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
				return new RestrictionSub(this.subObject, restriction);
			}
			return this;
		}

		private readonly int subObject;

		private readonly Restriction nestedRestriction;
	}
}
