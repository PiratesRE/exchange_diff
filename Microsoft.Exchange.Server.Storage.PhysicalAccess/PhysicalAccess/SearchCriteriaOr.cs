﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SearchCriteriaOr : SearchCriteria
	{
		protected SearchCriteriaOr(params SearchCriteria[] nestedCriteria)
		{
			this.nestedCriteria = nestedCriteria;
		}

		public SearchCriteria[] NestedCriteria
		{
			get
			{
				return this.nestedCriteria;
			}
		}

		public override bool Evaluate(ITWIR twir, CompareInfo compareInfo)
		{
			if (!this.hashSetOptimizationIsNotPossible && this.nestedCriteria.Length > 2)
			{
				if (this.rhsValues == null)
				{
					Column col = null;
					for (int i = 0; i < this.nestedCriteria.Length; i++)
					{
						SearchCriteriaCompare searchCriteriaCompare = this.nestedCriteria[i] as SearchCriteriaCompare;
						if (searchCriteriaCompare == null)
						{
							this.hashSetOptimizationIsNotPossible = true;
							break;
						}
						if (searchCriteriaCompare.RelOp != SearchCriteriaCompare.SearchRelOp.Equal)
						{
							this.hashSetOptimizationIsNotPossible = true;
							break;
						}
						if (i == 0)
						{
							if (searchCriteriaCompare.Lhs is ConstantColumn || (byte)(searchCriteriaCompare.Lhs.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 16 || searchCriteriaCompare.Lhs.ExtendedTypeCode == ExtendedTypeCode.String)
							{
								this.hashSetOptimizationIsNotPossible = true;
								break;
							}
							col = searchCriteriaCompare.Lhs;
						}
						else if (col != searchCriteriaCompare.Lhs)
						{
							this.hashSetOptimizationIsNotPossible = true;
							break;
						}
						if (!(searchCriteriaCompare.Rhs is ConstantColumn) || (byte)(searchCriteriaCompare.Rhs.ExtendedTypeCode & ExtendedTypeCode.MVFlag) == 16)
						{
							this.hashSetOptimizationIsNotPossible = true;
							break;
						}
					}
					if (!this.hashSetOptimizationIsNotPossible)
					{
						this.rhsValues = new HashSet<object>(new ValueEqualityComparer(CultureInfo.InvariantCulture.CompareInfo));
						for (int j = 0; j < this.nestedCriteria.Length; j++)
						{
							SearchCriteriaCompare searchCriteriaCompare2 = (SearchCriteriaCompare)this.nestedCriteria[j];
							this.rhsValues.Add(((ConstantColumn)searchCriteriaCompare2.Rhs).Value);
						}
					}
				}
				if (this.rhsValues != null)
				{
					object item = ((SearchCriteriaCompare)this.nestedCriteria[0]).Lhs.Evaluate(twir);
					return this.rhsValues.Contains(item);
				}
			}
			for (int k = 0; k < this.nestedCriteria.Length; k++)
			{
				if (this.nestedCriteria[k].Evaluate(twir, compareInfo))
				{
					return true;
				}
			}
			return false;
		}

		public override void EnumerateColumns(Action<Column, object> callback, object state, bool explodeCompositeColumns)
		{
			for (int i = 0; i < this.nestedCriteria.Length; i++)
			{
				this.nestedCriteria[i].EnumerateColumns(callback, state, explodeCompositeColumns);
			}
		}

		protected override SearchCriteria InspectAndFixChildren(SearchCriteria.InspectAndFixCriteriaDelegate callback, CompareInfo compareInfo, bool simplifyNegation)
		{
			List<SearchCriteria> list = null;
			for (int i = 0; i < this.nestedCriteria.Length; i++)
			{
				SearchCriteria searchCriteria = this.nestedCriteria[i].InspectAndFix(callback, compareInfo, simplifyNegation);
				if (list != null || !object.ReferenceEquals(searchCriteria, this.nestedCriteria[i]) || searchCriteria is SearchCriteriaFalse || searchCriteria is SearchCriteriaTrue || searchCriteria is SearchCriteriaOr)
				{
					if (searchCriteria is SearchCriteriaTrue)
					{
						return Factory.CreateSearchCriteriaTrue();
					}
					if (list == null)
					{
						list = new List<SearchCriteria>(this.nestedCriteria.Length);
						if (i != 0)
						{
							for (int j = 0; j < i; j++)
							{
								list.Add(this.nestedCriteria[j]);
							}
						}
					}
					if (!(searchCriteria is SearchCriteriaFalse))
					{
						if (searchCriteria is SearchCriteriaOr)
						{
							foreach (SearchCriteria item in ((SearchCriteriaOr)searchCriteria).NestedCriteria)
							{
								list.Add(item);
							}
						}
						else
						{
							list.Add(searchCriteria);
						}
					}
				}
			}
			IList<SearchCriteria> list2 = (list == null) ? ((IList<SearchCriteria>)this.nestedCriteria) : list;
			if (list2.Count < 100)
			{
				for (int l = list2.Count - 1; l > 0; l--)
				{
					for (int m = 0; m < l; m++)
					{
						if (list2[l].Equals(list2[m]))
						{
							if (list == null)
							{
								list = new List<SearchCriteria>(list2);
								list2 = list;
							}
							list2.RemoveAt(l);
							break;
						}
					}
				}
			}
			if (list2.Count == 0)
			{
				return Factory.CreateSearchCriteriaFalse();
			}
			if (list2.Count == 1)
			{
				return list2[0];
			}
			int count;
			using (SearchCriteria.LegEnumerator andLegs = list2[0].AndLegs)
			{
				count = andLegs.Count;
			}
			if (list2.Count < 100 && count < 100)
			{
				List<SearchCriteria> list3 = null;
				foreach (SearchCriteria searchCriteria2 in list2[0].AndLegs)
				{
					bool flag = true;
					for (int n = 1; n < list2.Count; n++)
					{
						bool flag2 = false;
						foreach (SearchCriteria other in list2[n].AndLegs)
						{
							if (searchCriteria2.Equals(other))
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						if (list3 == null)
						{
							list3 = new List<SearchCriteria>(2);
						}
						list3.Add(searchCriteria2);
					}
				}
				if (list3 != null)
				{
					if (list == null)
					{
						list = new List<SearchCriteria>(list2);
						list2 = list;
					}
					for (int num = 0; num < list2.Count; num++)
					{
						SearchCriteria searchCriteria3 = null;
						SearchCriteriaAnd searchCriteriaAnd = list2[num] as SearchCriteriaAnd;
						if (searchCriteriaAnd != null)
						{
							List<SearchCriteria> list4 = new List<SearchCriteria>(searchCriteriaAnd.NestedCriteria);
							for (int num2 = 0; num2 < list3.Count; num2++)
							{
								for (int num3 = list4.Count - 1; num3 >= 0; num3--)
								{
									if (list4[num3].Equals(list3[num2]))
									{
										list4.RemoveAt(num3);
										break;
									}
								}
							}
							if (list4.Count != 0)
							{
								if (list4.Count == 1)
								{
									searchCriteria3 = list4[0];
								}
								else
								{
									searchCriteria3 = Factory.CreateSearchCriteriaAnd(list4.ToArray());
								}
							}
						}
						if (searchCriteria3 == null)
						{
							list = (list2 = null);
							break;
						}
						list2[num] = searchCriteria3;
					}
					if (list != null)
					{
						list3.Add(Factory.CreateSearchCriteriaOr(list.ToArray()));
					}
					if (list3.Count == 1)
					{
						return list3[0];
					}
					return Factory.CreateSearchCriteriaAnd(list3.ToArray());
				}
			}
			if (list != null)
			{
				return Factory.CreateSearchCriteriaOr(list.ToArray());
			}
			return this;
		}

		internal override bool CanBeNegated
		{
			get
			{
				return true;
			}
		}

		internal override SearchCriteria Negate()
		{
			SearchCriteria[] array = new SearchCriteria[this.nestedCriteria.Length];
			for (int i = 0; i < this.nestedCriteria.Length; i++)
			{
				array[i] = this.nestedCriteria[i].Negate();
			}
			return Factory.CreateSearchCriteriaAnd(array);
		}

		public override void AppendToString(StringBuilder sb, StringFormatOptions formatOptions)
		{
			sb.Append("OR(");
			for (int i = 0; i < this.nestedCriteria.Length; i++)
			{
				if (i > 0)
				{
					sb.Append(", ");
				}
				this.nestedCriteria[i].AppendToString(sb, formatOptions);
			}
			sb.Append(")");
		}

		protected override bool CriteriaEquivalent(SearchCriteria other)
		{
			SearchCriteriaOr searchCriteriaOr = other as SearchCriteriaOr;
			if (searchCriteriaOr == null || this.nestedCriteria.Length != searchCriteriaOr.nestedCriteria.Length)
			{
				return false;
			}
			for (int i = 0; i < this.nestedCriteria.Length; i++)
			{
				if (!this.nestedCriteria[i].Equals(searchCriteriaOr.nestedCriteria[i]))
				{
					return false;
				}
			}
			return true;
		}

		private readonly SearchCriteria[] nestedCriteria;

		private HashSet<object> rhsValues;

		private bool hashSetOptimizationIsNotPossible;
	}
}
