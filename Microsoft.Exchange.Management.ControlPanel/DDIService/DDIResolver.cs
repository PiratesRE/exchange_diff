using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DDIResolver : IOutputConverter, IDDIConverter
	{
		public DDIResolver(ResolverType resolverType)
		{
			this.resolverType = resolverType;
		}

		public bool CanConvert(object sourceObject)
		{
			return sourceObject is IEnumerable<ADObjectId> || sourceObject is ADObjectId || sourceObject is IEnumerable<SecurityPrincipalIdParameter> || sourceObject is SecurityPrincipalIdParameter;
		}

		public object Convert(object sourceObject)
		{
			switch (this.resolverType)
			{
			case ResolverType.Recipient:
				if (sourceObject is IEnumerable<ADObjectId>)
				{
					sourceObject = RecipientObjectResolver.Instance.ResolveObjects(sourceObject as IEnumerable<ADObjectId>).ToList<RecipientObjectResolverRow>();
				}
				else if (sourceObject is ADObjectId)
				{
					sourceObject = RecipientObjectResolver.Instance.ResolveObjects(new ADObjectId[]
					{
						sourceObject as ADObjectId
					}).First<RecipientObjectResolverRow>();
				}
				break;
			case ResolverType.OrganizationUnitIdentity:
				if (sourceObject is IEnumerable<ADObjectId>)
				{
					sourceObject = RecipientObjectResolver.Instance.ResolveOrganizationUnitIdentity(sourceObject as IEnumerable<ADObjectId>).ToList<Identity>();
				}
				else if (sourceObject is ADObjectId)
				{
					sourceObject = RecipientObjectResolver.Instance.ResolveOrganizationUnitIdentity(new ADObjectId[]
					{
						sourceObject as ADObjectId
					}).First<Identity>();
				}
				break;
			case ResolverType.SidToRecipient:
				if (sourceObject is IEnumerable<SecurityPrincipalIdParameter>)
				{
					sourceObject = RecipientObjectResolver.Instance.ResolveSecurityPrincipalId(sourceObject as IEnumerable<SecurityPrincipalIdParameter>).ToList<AcePermissionRecipientRow>();
				}
				else if (sourceObject is SecurityPrincipalIdParameter)
				{
					IEnumerable<SecurityPrincipalIdParameter> sidPrincipalId = new SecurityPrincipalIdParameter[]
					{
						sourceObject as SecurityPrincipalIdParameter
					}.AsEnumerable<SecurityPrincipalIdParameter>();
					sourceObject = RecipientObjectResolver.Instance.ResolveSecurityPrincipalId(sidPrincipalId).First<AcePermissionRecipientRow>();
				}
				break;
			case ResolverType.RetentionPolicyTag:
				if (sourceObject is IEnumerable<ADObjectId>)
				{
					sourceObject = RetentionPolicyTagObjectResolver.Instance.ResolveObjects(sourceObject as IEnumerable<ADObjectId>).ToList<RetentionPolicyTagResolverRow>();
				}
				else if (sourceObject is ADObjectId)
				{
					sourceObject = RetentionPolicyTagObjectResolver.Instance.ResolveObjects(new ADObjectId[]
					{
						sourceObject as ADObjectId
					}).First<RetentionPolicyTagResolverRow>();
				}
				break;
			case ResolverType.Server:
				if (sourceObject is IEnumerable<ADObjectId>)
				{
					sourceObject = ServerResolver.Instance.ResolveObjects(sourceObject as IEnumerable<ADObjectId>).ToList<ServerResolverRow>();
				}
				else if (sourceObject is ADObjectId)
				{
					sourceObject = ServerResolver.Instance.ResolveObjects(new ADObjectId[]
					{
						sourceObject as ADObjectId
					}).First<ServerResolverRow>();
				}
				break;
			}
			return sourceObject;
		}

		private ResolverType resolverType;
	}
}
