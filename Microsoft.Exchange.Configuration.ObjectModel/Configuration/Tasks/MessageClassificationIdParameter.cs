using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MessageClassificationIdParameter : ADIdParameter
	{
		public MessageClassificationIdParameter()
		{
		}

		public MessageClassificationIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MessageClassificationIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected MessageClassificationIdParameter(string identity) : base(identity)
		{
			ADObjectId internalADObjectId = base.InternalADObjectId;
		}

		public static MessageClassificationIdParameter Parse(string s)
		{
			return new MessageClassificationIdParameter(s);
		}

		public override string ToString()
		{
			if (base.InternalADObjectId == null || base.InternalADObjectId.Parent == null)
			{
				return base.ToString();
			}
			string name = base.InternalADObjectId.Parent.Name;
			if (!name.Equals("Default", StringComparison.OrdinalIgnoreCase))
			{
				return name + "\\" + base.InternalADObjectId.Name;
			}
			return base.InternalADObjectId.Name;
		}

		internal static ADObjectId DefaultRoot(IConfigDataProvider session)
		{
			IConfigurationSession configurationSession = (IConfigurationSession)session;
			ADObjectId orgContainerId = configurationSession.GetOrgContainerId();
			return orgContainerId.GetDescendantId(MessageClassificationIdParameter.DefaultsRoot);
		}

		internal override IEnumerable<T> GetObjectsInOrganization<T>(string identityString, ADObjectId rootId, IDirectorySession session, OptionalIdentityData optionalData)
		{
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjectsInOrganization<T>(identityString, rootId, session, optionalData));
			if (wrapper.HasElements())
			{
				return wrapper;
			}
			int num = identityString.IndexOf('\\');
			if (0 < num && identityString.Length > num + 1)
			{
				string propertyValue = identityString.Substring(num + 1);
				string unescapedCommonName = identityString.Substring(0, num);
				OptionalIdentityData optionalIdentityData = null;
				if (optionalData != null)
				{
					optionalIdentityData = optionalData.Clone();
					optionalIdentityData.ConfigurationContainerRdn = null;
				}
				ADObjectId adobjectId = (rootId == null) ? MessageClassificationIdParameter.DefaultRoot((IConfigDataProvider)session) : rootId;
				ADObjectId childId = adobjectId.Parent.GetChildId(unescapedCommonName);
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, propertyValue);
				return base.PerformPrimarySearch<T>(filter, childId, session, false, optionalIdentityData);
			}
			return new T[0];
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		private const char LocaleIdentityDivider = '\\';

		private const string DefaultContainerName = "Default";

		public static ADObjectId DefaultsRoot = new ADObjectId("CN=Default,CN=Message Classifications,CN=Transport Settings");

		public static ADObjectId MessageClassificationRdn = new ADObjectId("CN=Message Classifications,CN=Transport Settings");
	}
}
