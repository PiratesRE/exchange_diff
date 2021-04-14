using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "GlobalAddressList", DefaultParameterSetName = "Identity")]
	public sealed class GetGlobalAddressList : GetMultitenancySystemConfigurationObjectTask<GlobalAddressListIdParameter, AddressBookBase>
	{
		[Parameter(Mandatory = true, ParameterSetName = "DefaultOnly")]
		public SwitchParameter DefaultOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["DefaultOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DefaultOnly"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity == null)
				{
					return GlobalAddressListIdParameter.GetRootContainerId((IConfigurationSession)base.DataSession, base.CurrentOrganizationId);
				}
				return null;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (this.DefaultOnly.IsPresent)
				{
					return new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, base.CurrentOrgContainerId.GetDescendantId(GlobalAddressList.RdnGalContainerToOrganization)),
						new ComparisonFilter(ComparisonOperator.Equal, AddressBookBaseSchema.IsDefaultGlobalAddressList, true)
					});
				}
				if (this.Identity == null)
				{
					return new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, base.CurrentOrgContainerId.GetDescendantId(GlobalAddressList.RdnGalContainerToOrganization));
				}
				return null;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception) || typeof(DataSourceOperationException).IsInstanceOfType(exception);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			base.WriteResult(new GlobalAddressList((AddressBookBase)dataObject));
			TaskLogger.LogExit();
		}
	}
}
