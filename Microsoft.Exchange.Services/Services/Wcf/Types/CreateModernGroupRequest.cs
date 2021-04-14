using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Name = "CreateModernGroupRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateModernGroupRequest : ComposeModernGroupRequestBase
	{
		[DataMember(Name = "Alias", IsRequired = false)]
		public string Alias { get; set; }

		[DataMember(Name = "CollectLogs", IsRequired = false)]
		public bool CollectLogs { get; set; }

		[DataMember(Name = "GroupType", IsRequired = true)]
		public ModernGroupObjectType GroupType { get; set; }

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override void Validate()
		{
			if (string.IsNullOrWhiteSpace(base.Name))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			if (string.IsNullOrWhiteSpace(this.Alias))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			string text = CallContext.Current.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			if (base.AddedMembers == null)
			{
				base.AddedMembers = new string[]
				{
					text
				};
			}
			else
			{
				base.AddedMembers = base.AddedMembers.Concat(new string[]
				{
					text
				}).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray<string>();
			}
			if (base.AddedOwners == null)
			{
				base.AddedOwners = new string[]
				{
					text
				};
				return;
			}
			base.AddedOwners = base.AddedOwners.Concat(new string[]
			{
				text
			}).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray<string>();
		}
	}
}
