using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxSession : ISession
	{
		public FxSession(IReadOnlyDictionary<PropertyTag, NamedProperty> namedPropertiesMapper)
		{
			this.propertyTagToNamedProperties = namedPropertiesMapper;
		}

		bool ISession.TryResolveToNamedProperty(PropertyTag propertyTag, out NamedProperty namedProperty)
		{
			namedProperty = null;
			return propertyTag.IsNamedProperty && this.propertyTagToNamedProperties.TryGetValue(propertyTag, out namedProperty);
		}

		bool ISession.TryResolveFromNamedProperty(NamedProperty namedProperty, ref PropertyTag propertyTag)
		{
			return false;
		}

		[Conditional("Debug")]
		private void ValidateUniquePropertyIDs(IReadOnlyDictionary<PropertyTag, NamedProperty> namedPropertiesMapper)
		{
			new HashSet<int>();
			foreach (PropertyTag propertyTag in namedPropertiesMapper.Keys)
			{
			}
		}

		private readonly IReadOnlyDictionary<PropertyTag, NamedProperty> propertyTagToNamedProperties;
	}
}
