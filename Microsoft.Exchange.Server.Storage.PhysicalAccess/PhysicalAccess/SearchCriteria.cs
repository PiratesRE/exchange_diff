using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class SearchCriteria : IEquatable<SearchCriteria>
	{
		public abstract bool Evaluate(ITWIR twir, CompareInfo compareInfo);

		public void EnumerateColumns(Action<Column, object> callback, object state)
		{
			this.EnumerateColumns(callback, state, false);
		}

		public virtual void EnumerateColumns(Action<Column, object> callback, object state, bool explodeCompositeColumns)
		{
		}

		public SearchCriteria InspectAndFix(SearchCriteria.InspectAndFixCriteriaDelegate callback, CompareInfo compareInfo, bool simplifyNegation)
		{
			if (callback != null)
			{
				SearchCriteria searchCriteria = callback(this, compareInfo);
				if (searchCriteria == null)
				{
					return this;
				}
				if (!object.ReferenceEquals(searchCriteria, this))
				{
					return searchCriteria;
				}
			}
			return this.InspectAndFixChildren(callback, compareInfo, simplifyNegation);
		}

		protected virtual SearchCriteria InspectAndFixChildren(SearchCriteria.InspectAndFixCriteriaDelegate callback, CompareInfo compareInfo, bool simplifyNegation)
		{
			return this;
		}

		internal virtual bool CanBeNegated
		{
			get
			{
				return false;
			}
		}

		internal virtual SearchCriteria Negate()
		{
			return Factory.CreateSearchCriteriaNot(this);
		}

		public abstract void AppendToString(StringBuilder sb, StringFormatOptions formatOptions);

		public override string ToString()
		{
			return this.ToString(StringFormatOptions.IncludeDetails);
		}

		public string ToString(StringFormatOptions formatOptions)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.AppendToString(stringBuilder, formatOptions);
			return stringBuilder.ToString();
		}

		public bool Equals(SearchCriteria other)
		{
			return object.ReferenceEquals(this, other) || this.CriteriaEquivalent(other);
		}

		public override bool Equals(object other)
		{
			SearchCriteria searchCriteria = other as SearchCriteria;
			return searchCriteria != null && this.Equals(searchCriteria);
		}

		public override int GetHashCode()
		{
			return 0;
		}

		protected abstract bool CriteriaEquivalent(SearchCriteria other);

		internal SearchCriteria.LegEnumerator AndLegs
		{
			get
			{
				return new SearchCriteria.LegEnumerator(this, false);
			}
		}

		internal SearchCriteria.LegEnumerator OrLegs
		{
			get
			{
				return new SearchCriteria.LegEnumerator(this, true);
			}
		}

		protected const int MaxAndOrLegsToOptimize = 100;

		public delegate SearchCriteria InspectAndFixCriteriaDelegate(SearchCriteria criteria, CompareInfo compareInfo);

		internal struct LegEnumerator : IEnumerator<SearchCriteria>, IDisposable, IEnumerator
		{
			public LegEnumerator(SearchCriteria criteria, bool orLegs)
			{
				this.legIndex = -1;
				this.legs = criteria;
				if (orLegs)
				{
					SearchCriteriaOr searchCriteriaOr = criteria as SearchCriteriaOr;
					if (searchCriteriaOr != null)
					{
						this.legs = searchCriteriaOr.NestedCriteria;
						return;
					}
				}
				else
				{
					SearchCriteriaAnd searchCriteriaAnd = criteria as SearchCriteriaAnd;
					if (searchCriteriaAnd != null)
					{
						this.legs = searchCriteriaAnd.NestedCriteria;
					}
				}
			}

			public SearchCriteria.LegEnumerator GetEnumerator()
			{
				return this;
			}

			public int Count
			{
				get
				{
					SearchCriteria[] array = this.legs as SearchCriteria[];
					if (array != null)
					{
						return array.Length;
					}
					return 1;
				}
			}

			public SearchCriteria Current
			{
				get
				{
					SearchCriteria[] array = this.legs as SearchCriteria[];
					if (array != null)
					{
						return array[this.legIndex];
					}
					return this.legs as SearchCriteria;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				int num = 0;
				SearchCriteria[] array = this.legs as SearchCriteria[];
				if (array != null)
				{
					num = array.Length - 1;
				}
				if (this.legIndex >= num)
				{
					return false;
				}
				this.legIndex++;
				return true;
			}

			public void Reset()
			{
				this.legIndex = -1;
			}

			public void Dispose()
			{
			}

			public SearchCriteria.LegEnumerator Clone()
			{
				return this;
			}

			private object legs;

			private int legIndex;
		}
	}
}
