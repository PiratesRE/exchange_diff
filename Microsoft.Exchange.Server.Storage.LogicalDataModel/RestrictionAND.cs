using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class RestrictionAND : Restriction
	{
		public RestrictionAND(params Restriction[] nestedRestrictions)
		{
			this.nestedRestrictions = nestedRestrictions;
		}

		public RestrictionAND(Context context, byte[] byteForm, ref int position, int posMax, Mailbox mailbox, ObjectType objectType)
		{
			ParseSerialize.GetByte(byteForm, ref position, posMax);
			ushort word = ParseSerialize.GetWord(byteForm, ref position, posMax);
			this.nestedRestrictions = new Restriction[(int)word];
			for (int i = 0; i < this.nestedRestrictions.Length; i++)
			{
				this.nestedRestrictions[i] = Restriction.Deserialize(context, byteForm, ref position, posMax, mailbox, objectType);
			}
		}

		public Restriction[] NestedRestrictions
		{
			get
			{
				return this.nestedRestrictions;
			}
		}

		internal override void AppendToString(StringBuilder sb)
		{
			sb.Append("AND(");
			if (this.nestedRestrictions != null)
			{
				for (int i = 0; i < this.nestedRestrictions.Length; i++)
				{
					if (i > 0)
					{
						sb.Append(", ");
					}
					this.nestedRestrictions[i].AppendToString(sb);
				}
			}
			sb.Append(")");
		}

		public override void Serialize(byte[] byteForm, ref int position)
		{
			ParseSerialize.SetByte(byteForm, ref position, 0);
			ParseSerialize.SetWord(byteForm, ref position, (short)((this.nestedRestrictions == null) ? 0 : this.nestedRestrictions.Length));
			if (this.nestedRestrictions != null)
			{
				for (int i = 0; i < this.nestedRestrictions.Length; i++)
				{
					this.nestedRestrictions[i].Serialize(byteForm, ref position);
				}
			}
		}

		public override SearchCriteria ToSearchCriteria(StoreDatabase database, ObjectType objectType)
		{
			if (this.nestedRestrictions == null || this.nestedRestrictions.Length == 0)
			{
				return Factory.CreateSearchCriteriaTrue();
			}
			SearchCriteria[] array = new SearchCriteria[this.nestedRestrictions.Length];
			for (int i = 0; i < this.nestedRestrictions.Length; i++)
			{
				array[i] = this.nestedRestrictions[i].ToSearchCriteria(database, objectType);
			}
			return Factory.CreateSearchCriteriaAnd(array);
		}

		public override bool HasClauseMeetingPredicate(Predicate<Restriction> predicate)
		{
			if (predicate(this))
			{
				return true;
			}
			if (this.nestedRestrictions != null)
			{
				for (int i = 0; i < this.nestedRestrictions.Length; i++)
				{
					if (this.nestedRestrictions[i].HasClauseMeetingPredicate(predicate))
					{
						return true;
					}
				}
			}
			return false;
		}

		protected override Restriction InspectAndFixChildren(Restriction.InspectAndFixRestrictionDelegate callback)
		{
			List<Restriction> list = null;
			for (int i = 0; i < this.nestedRestrictions.Length; i++)
			{
				Restriction restriction = this.nestedRestrictions[i].InspectAndFix(callback);
				if (list != null || !object.ReferenceEquals(restriction, this.nestedRestrictions[i]))
				{
					if (list == null)
					{
						list = new List<Restriction>(this.nestedRestrictions.Length);
						if (i != 0)
						{
							for (int j = 0; j < i; j++)
							{
								list.Add(this.nestedRestrictions[j]);
							}
						}
					}
					list.Add(restriction);
				}
			}
			if (list != null)
			{
				return new RestrictionAND(list.ToArray());
			}
			return this;
		}

		private readonly Restriction[] nestedRestrictions;
	}
}
