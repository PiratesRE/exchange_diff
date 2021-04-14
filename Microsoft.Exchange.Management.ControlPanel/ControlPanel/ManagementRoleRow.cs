using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(ManagementRoleRow))]
	public class ManagementRoleRow : BaseRow
	{
		public ManagementRoleRow(ExchangeRole exchangeRole) : base(exchangeRole)
		{
			this.ExchangeRole = exchangeRole;
		}

		protected ExchangeRole ExchangeRole { get; set; }

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.ExchangeRole.Name;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return this.ExchangeRole.Name;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsEndUserRole
		{
			get
			{
				return this.ExchangeRole.IsEndUserRole;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
