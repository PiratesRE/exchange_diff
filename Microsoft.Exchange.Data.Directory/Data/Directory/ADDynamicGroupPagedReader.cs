using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADDynamicGroupPagedReader<TResult> : ADPagedReader<TResult> where TResult : IConfigurable, new()
	{
		internal ADDynamicGroupPagedReader(IDirectorySession session, ADObjectId rootId, QueryScope scope, string ldapFilter, int pageSize, CustomExceptionHandler customExceptionHandler, IEnumerable<PropertyDefinition> properties) : base(session, rootId, scope, new CustomLdapFilter(ldapFilter), null, pageSize, properties, false)
		{
			base.CustomExceptionHandler = customExceptionHandler;
		}

		protected override SearchResultEntryCollection GetNextResultCollection()
		{
			SearchResultEntryCollection result;
			try
			{
				result = base.GetNextResultCollection();
			}
			catch (DataValidationException)
			{
				base.RetrievedAllData = new bool?(true);
				result = null;
			}
			return result;
		}
	}
}
