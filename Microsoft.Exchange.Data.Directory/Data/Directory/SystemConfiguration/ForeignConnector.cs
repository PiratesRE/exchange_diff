using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ForeignConnector : MailGateway
	{
		[Parameter]
		public string DropDirectory
		{
			get
			{
				return (string)this[ForeignConnectorSchema.DropDirectory];
			}
			set
			{
				this[ForeignConnectorSchema.DropDirectory] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> DropDirectoryQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ForeignConnectorSchema.DropDirectoryQuota];
			}
			set
			{
				this[ForeignConnectorSchema.DropDirectoryQuota] = value;
			}
		}

		[Parameter]
		public bool RelayDsnRequired
		{
			get
			{
				return (bool)this[ForeignConnectorSchema.RelayDsnRequired];
			}
			set
			{
				this[ForeignConnectorSchema.RelayDsnRequired] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool Enabled
		{
			get
			{
				return !(bool)this[ForeignConnectorSchema.Disabled];
			}
			set
			{
				this[ForeignConnectorSchema.Disabled] = !value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ForeignConnector.schema;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				ComparisonFilter comparisonFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "mailGateway");
				ExistsFilter existsFilter = new ExistsFilter(ForeignConnectorSchema.DropDirectory);
				return new AndFilter(new QueryFilter[]
				{
					comparisonFilter,
					existsFilter
				});
			}
		}

		private static ForeignConnectorSchema schema = ObjectSchema.GetInstance<ForeignConnectorSchema>();
	}
}
