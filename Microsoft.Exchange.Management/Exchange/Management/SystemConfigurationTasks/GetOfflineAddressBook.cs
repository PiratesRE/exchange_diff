using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "OfflineAddressBook", DefaultParameterSetName = "Identity")]
	public sealed class GetOfflineAddressBook : GetMultitenancySystemConfigurationObjectTask<OfflineAddressBookIdParameter, OfflineAddressBook>
	{
		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(OrgContainerNotFoundException).IsInstanceOfType(exception) || typeof(TenantOrgContainerNotFoundException).IsInstanceOfType(exception);
		}

		[Parameter(Mandatory = true, ParameterSetName = "Server", ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return this.internalFilter ?? base.InternalFilter;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void InternalValidate()
		{
			if (this.Server != null)
			{
				Server server = (Server)base.GetDataObject<Server>(this.Server, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
				this.internalFilter = new ComparisonFilter(ComparisonOperator.Equal, OfflineAddressBookSchema.Server, server.Id);
			}
			base.InternalValidate();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			OfflineAddressBook offlineAddressBook = (OfflineAddressBook)dataObject;
			offlineAddressBook.ResolveConfiguredAttributes();
			base.WriteResult(new OfflineAddressBookPresentationObject(offlineAddressBook));
		}

		private QueryFilter internalFilter;
	}
}
