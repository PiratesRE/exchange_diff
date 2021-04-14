using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(ManagementRoleObject))]
	[DataContract]
	public class ManagementRoleObject : ManagementRoleRow
	{
		public ManagementRoleObject(ExchangeRole exchangeRole) : base(exchangeRole)
		{
		}

		[DataMember]
		public string Description
		{
			get
			{
				return base.ExchangeRole.Description;
			}
			set
			{
				base.ExchangeRole.Description = value;
			}
		}

		public ScopeType ImplicitConfigWriteScope
		{
			get
			{
				return base.ExchangeRole.ImplicitConfigWriteScope;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public ScopeType ImplicitRecipientWriteScope
		{
			get
			{
				return base.ExchangeRole.ImplicitRecipientWriteScope;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ImplicitConfigWriteScopeName
		{
			get
			{
				return base.ExchangeRole.ImplicitConfigWriteScope.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ImplicitRecipientWriteScopeName
		{
			get
			{
				return base.ExchangeRole.ImplicitRecipientWriteScope.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
