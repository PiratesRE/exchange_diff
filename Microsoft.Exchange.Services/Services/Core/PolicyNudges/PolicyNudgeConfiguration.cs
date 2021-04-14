using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	internal abstract class PolicyNudgeConfiguration
	{
		internal abstract XmlElement SerializeConfiguration(XElement clientConfig, CachedOrganizationConfiguration serverConfig, ADObjectId senderADObjectId, XmlDocument xmlDoc);
	}
}
