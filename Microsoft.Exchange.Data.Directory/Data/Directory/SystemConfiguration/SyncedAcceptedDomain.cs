using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class SyncedAcceptedDomain : AcceptedDomain
	{
		public MultiValuedProperty<string> SyncErrors
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncedAcceptedDomainSchema.SyncErrors];
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return SyncedAcceptedDomain.SchemaObject;
			}
		}

		private static readonly SyncedAcceptedDomainSchema SchemaObject = ObjectSchema.GetInstance<SyncedAcceptedDomainSchema>();
	}
}
