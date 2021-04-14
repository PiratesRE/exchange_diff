using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public class ADScope
	{
		protected ADScope()
		{
		}

		public ADScope(ADObjectId root, QueryFilter filter)
		{
			this.root = root;
			this.filter = filter;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Root == null && null == this.Filter;
			}
		}

		public virtual ADObjectId Root
		{
			get
			{
				return this.root;
			}
			protected internal set
			{
				if (this == ADScope.Empty || this == ADScope.NoAccess)
				{
					throw new InvalidOperationException();
				}
				this.root = value;
			}
		}

		public virtual QueryFilter Filter
		{
			get
			{
				return this.filter;
			}
			protected internal set
			{
				this.filter = value;
			}
		}

		public string GetFilterString()
		{
			if (this.Filter != null)
			{
				return this.Filter.GenerateInfixString(FilterLanguage.Monad);
			}
			return string.Empty;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ADScope);
		}

		public bool Equals(ADScope obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!ADObjectId.Equals(this.Root, obj.Root))
			{
				return false;
			}
			if (this.Filter != null)
			{
				return this.Filter.Equals(obj.Filter);
			}
			return obj.Filter == null;
		}

		public override int GetHashCode()
		{
			int num = (this.Root == null) ? 0 : this.Root.GetHashCode();
			int num2 = (this.Filter == null) ? 0 : this.Filter.GetHashCode();
			return num ^ num2;
		}

		public override string ToString()
		{
			return string.Format("{{{0}, {1}}}", this.Root, this.Filter);
		}

		private static QueryFilter CombineScopes(IList<ADScope> combinableScopes)
		{
			QueryFilter[] array = new QueryFilter[combinableScopes.Count];
			for (int i = 0; i < combinableScopes.Count; i++)
			{
				if (combinableScopes[i].Root != combinableScopes[0].Root)
				{
					throw new ArgumentException("combinableScopes");
				}
				array[i] = combinableScopes[i].Filter;
			}
			return (array.Length == 1) ? array[0] : new OrFilter(array);
		}

		internal static ADScope CombineScopeCollections(IList<ADScopeCollection> combinableScopeCollections)
		{
			QueryFilter[] array = new QueryFilter[combinableScopeCollections.Count];
			for (int i = 0; i < combinableScopeCollections.Count; i++)
			{
				ADScopeCollection combinableScopes = combinableScopeCollections[i];
				if (combinableScopeCollections[i][0].Root != combinableScopeCollections[0][0].Root)
				{
					throw new ArgumentException("combinableScopeCollections");
				}
				array[i] = ADScope.CombineScopes(combinableScopes);
			}
			QueryFilter queryFilter = (array.Length == 1) ? array[0] : new AndFilter(array);
			return new ADScope(combinableScopeCollections[0][0].Root, queryFilter);
		}

		internal static readonly QueryFilter NoObjectFilter = new NotFilter(new ExistsFilter(ADObjectSchema.ObjectClass));

		internal static readonly ADScope NoAccess = new ADScope(null, ADScope.NoObjectFilter);

		internal static readonly ADScope Empty = new ADScope();

		private static readonly ADScope[] EmptyArray = new ADScope[0];

		private ADObjectId root;

		private QueryFilter filter;
	}
}
