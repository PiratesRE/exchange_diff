using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "DistributionGroupMember")]
	public sealed class GetDistributionGroupMember : GetRecipientObjectTask<DistributionGroupMemberIdParameter, ReducedRecipient>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true)]
		public override DistributionGroupMemberIdParameter Identity
		{
			get
			{
				return (DistributionGroupMemberIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter]
		public SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return base.InternalIgnoreDefaultScope;
			}
			set
			{
				base.InternalIgnoreDefaultScope = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession reducedRecipientSession = DirectorySessionFactory.Default.GetReducedRecipientSession((IRecipientSession)base.CreateSession(), 58, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\DistributionList\\GetDistributionGroupMember.cs");
			if (base.InternalIgnoreDefaultScope.IsPresent)
			{
				reducedRecipientSession.EnforceDefaultScope = false;
				reducedRecipientSession.UseGlobalCatalog = true;
			}
			return reducedRecipientSession;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			LocalizedString? localizedString;
			IEnumerable<ReducedRecipient> dataObjects = base.GetDataObjects(this.Identity, base.OptionalIdentityData, out localizedString);
			this.WriteResult<ReducedRecipient>(dataObjects);
			if (!base.HasErrors && localizedString != null)
			{
				base.WriteError(new ManagementObjectNotFoundException(localizedString.Value), ErrorCategory.InvalidData, null);
			}
			TaskLogger.LogExit();
		}
	}
}
