using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionComment : Restriction
	{
		public RestrictionComment(StorePropTag[] propTags, object[] values, Restriction nestedRestriction)
		{
			if (propTags != null && propTags.Length != 0)
			{
				this.propTags = propTags;
				this.values = values;
			}
			this.nestedRestriction = nestedRestriction;
		}

		public RestrictionComment(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			byte @byte = ParseSerialize.GetByte(byteForm, ref position, posMax);
			if (@byte != 0)
			{
				this.propTags = new StorePropTag[(int)@byte];
				this.values = new object[(int)@byte];
				for (int i = 0; i < (int)@byte; i++)
				{
					this.propTags[i] = Mailbox.GetStorePropTag(context, mailbox, ParseSerialize.GetDword(byteForm, ref position, posMax), objectType);
					this.values[i] = Restriction.DeserializeValue(byteForm, ref position, this.propTags[i]);
				}
			}
			bool boolean = ParseSerialize.GetBoolean(byteForm, ref position, posMax);
			if (boolean)
			{
				this.nestedRestriction = Restriction.Deserialize(context, byteForm, ref position, posMax, mailbox, objectType);
			}
		}

		public StorePropTag[] PropTags
		{
			get
			{
				return this.propTags;
			}
		}

		public object[] Values
		{
			get
			{
				return this.values;
			}
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
			sb.Append("COMMENT({");
			if (this.propTags != null)
			{
				for (int i = 0; i < this.propTags.Length; i++)
				{
					if (i != 0)
					{
						sb.Append(", ");
					}
					sb.Append("[");
					this.propTags[i].AppendToString(sb);
					sb.Append("]=");
					sb.Append(this.values[i]);
				}
			}
			sb.Append("}, ");
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
			ParseSerialize.SetByte(byteForm, ref position, 10);
			ParseSerialize.SetByte(byteForm, ref position, (byte)((this.propTags == null) ? 0 : this.propTags.Length));
			if (this.propTags != null)
			{
				for (int i = 0; i < this.propTags.Length; i++)
				{
					ParseSerialize.SetDword(byteForm, ref position, this.propTags[i].ExternalTag);
					Restriction.SerializeValue(byteForm, ref position, this.propTags[i], this.values[i]);
				}
			}
			ParseSerialize.SetBoolean(byteForm, ref position, this.nestedRestriction != null);
			if (this.nestedRestriction != null)
			{
				this.nestedRestriction.Serialize(byteForm, ref position);
			}
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			if (this.nestedRestriction == null)
			{
				return Factory.CreateSearchCriteriaFalse();
			}
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
				return new RestrictionComment(this.propTags, this.values, objA);
			}
			return this;
		}

		private readonly StorePropTag[] propTags;

		private readonly object[] values;

		private readonly Restriction nestedRestriction;
	}
}
