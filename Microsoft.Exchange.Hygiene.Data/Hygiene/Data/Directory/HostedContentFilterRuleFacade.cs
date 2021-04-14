using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class HostedContentFilterRuleFacade : HygieneFilterRuleFacade
	{
		public override ObjectId Identity
		{
			get
			{
				return (ADObjectId)this[ADObjectSchema.Id];
			}
		}

		public string HostedContentFilterPolicy
		{
			get
			{
				return (string)this[HostedContentFilterRuleFacadeSchema.HostedContentFilterPolicy];
			}
			set
			{
				this[HostedContentFilterRuleFacadeSchema.HostedContentFilterPolicy] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return HostedContentFilterRuleFacade.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return HostedContentFilterRuleFacade.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly HostedContentFilterRuleFacadeSchema schema = ObjectSchema.GetInstance<HostedContentFilterRuleFacadeSchema>();

		private static string mostDerivedClass = "HostedContentFilterRuleFacade";
	}
}
