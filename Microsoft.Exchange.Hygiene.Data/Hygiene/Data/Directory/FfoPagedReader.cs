using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class FfoPagedReader<T> : ADPagedReader<T> where T : IConfigurable, new()
	{
		public FfoPagedReader(IDirectorySession session, QueryFilter filter, ADObjectId rootId, int pageSize) : this(session, rootId, QueryScope.SubTree, filter, null, pageSize, null, false)
		{
		}

		public FfoPagedReader(IDirectorySession session, QueryFilter filter, ADObjectId rootId) : this(session, rootId, QueryScope.SubTree, filter, null, ADGenericPagedReader<T>.DefaultPageSize, null, false)
		{
		}

		public FfoPagedReader(IDirectorySession session, ADObjectId rootId, QueryScope queryScope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties, bool skipCheckVirtualIndex) : base(session, rootId, queryScope, filter, sortBy, pageSize, properties, skipCheckVirtualIndex)
		{
			this.properties = properties;
			this.queryFilter = filter;
		}

		protected override T[] GetNextPage()
		{
			QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(this.queryFilter, this.cookie);
			T[] result = null;
			try
			{
				result = this.Find(pagingQueryFilter);
			}
			catch (PermanentDALException)
			{
				base.RetrievedAllData = new bool?(true);
				throw;
			}
			bool value = false;
			this.cookie = PagingHelper.GetProcessedCookie(pagingQueryFilter, out value);
			base.RetrievedAllData = new bool?(value);
			return result;
		}

		private T[] Find(QueryFilter queryFilterWithPageCookie)
		{
			T[] array = this.FindInferredTypeObject<ExchangeRoleAssignment, ExchangeRoleAssignmentSchema>(queryFilterWithPageCookie);
			if (array != null)
			{
				return array;
			}
			T[] array2 = this.FindInferredTypeObject<ExchangeRole, ExchangeRoleSchema>(queryFilterWithPageCookie);
			if (array2 != null)
			{
				return array2;
			}
			T[] array3 = this.FindInferredTypeObject<ADRecipient, ExtendedSecurityPrincipalSchema>(queryFilterWithPageCookie);
			if (array3 != null)
			{
				return array3;
			}
			return ((IConfigDataProvider)base.Session).FindPaged<T>(queryFilterWithPageCookie, base.RootId, true, null, base.PageSize).ToArray<T>();
		}

		private T[] FindInferredTypeObject<IT, ITS>(QueryFilter queryFilterWithPageCookie) where IT : IConfigurable, new()
		{
			Func<IT, T> func = null;
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			IEnumerable<PropertyDefinition> adPropertyDefinitions = (from field in typeof(ITS).GetFields(bindingAttr)
			select field.GetValue(null)).OfType<PropertyDefinition>();
			if (typeof(T) == typeof(ADRawEntry) && this.properties != null && this.properties.Any((PropertyDefinition prop) => adPropertyDefinitions.Contains(prop)))
			{
				IEnumerable<IT> enumerable = ((IConfigDataProvider)base.Session).FindPaged<IT>(queryFilterWithPageCookie, base.RootId, true, null, base.PageSize);
				IEnumerable<IT> source = enumerable;
				if (func == null)
				{
					func = ((IT item) => (T)((object)item));
				}
				return source.Select(func).ToArray<T>();
			}
			return null;
		}

		private readonly QueryFilter queryFilter;

		private readonly IEnumerable<PropertyDefinition> properties;

		private string cookie;
	}
}
