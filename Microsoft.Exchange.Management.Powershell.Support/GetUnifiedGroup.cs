using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.UnifiedGroups;
using Microsoft.WindowsAzure.ActiveDirectory;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "UnifiedGroup")]
	public sealed class GetUnifiedGroup : UnifiedGroupTask
	{
		[Parameter(Mandatory = false)]
		public Guid? Identity { get; set; }

		protected override void InternalProcessRecord()
		{
			AADClient aadclient = AADClientFactory.Create(base.OrganizationId, GraphProxyVersions.Version14);
			if (aadclient == null)
			{
				base.WriteError(new TaskException(Strings.ErrorUnableToSessionWithAAD), ExchangeErrorCategory.Client, null);
				return;
			}
			if (this.Identity != null)
			{
				Group group;
				try
				{
					group = aadclient.GetGroup(this.Identity.Value.ToString(), true);
				}
				catch (AADException ex)
				{
					base.WriteVerbose("GetGroup failed with exception: {0}", new object[]
					{
						ex
					});
					base.WriteError(new TaskException(Strings.ErrorUnableToGetUnifiedGroup), base.GetErrorCategory(ex), null);
					return;
				}
				aadclient.Service.LoadProperty(group, "createdOnBehalfOf");
				aadclient.Service.LoadProperty(group, "members");
				aadclient.Service.LoadProperty(group, "owners");
				this.WriteAADGroupObject(group);
				return;
			}
			try
			{
				foreach (Group group2 in aadclient.GetGroups())
				{
					this.WriteAADGroupObject(group2);
				}
			}
			catch (AADException ex2)
			{
				base.WriteVerbose("GetGroups failed with exception: {0}", new object[]
				{
					ex2
				});
				base.WriteError(new TaskException(Strings.ErrorUnableToGetUnifiedGroup), base.GetErrorCategory(ex2), null);
			}
		}

		private void WriteAADGroupObject(Group group)
		{
			AADGroupPresentationObject aadgroupPresentationObject = new AADGroupPresentationObject(group);
			if (base.NeedSuppressingPiiData)
			{
				aadgroupPresentationObject = this.Redact(aadgroupPresentationObject);
			}
			base.WriteObject(aadgroupPresentationObject);
		}

		private AADDirectoryObjectPresentationObject[] Redact(AADDirectoryObjectPresentationObject[] values)
		{
			if (values != null && values.Length > 0)
			{
				for (int i = 0; i < values.Length; i++)
				{
					values[i] = this.Redact(values[i]);
				}
			}
			return values;
		}

		private AADDirectoryObjectPresentationObject Redact(AADDirectoryObjectPresentationObject value)
		{
			if (value != null)
			{
				AADUserPresentationObject aaduserPresentationObject = value as AADUserPresentationObject;
				if (aaduserPresentationObject != null)
				{
					return this.Redact(aaduserPresentationObject);
				}
				AADGroupPresentationObject aadgroupPresentationObject = value as AADGroupPresentationObject;
				if (aadgroupPresentationObject != null)
				{
					return this.Redact(aadgroupPresentationObject);
				}
				value.Owners = this.Redact(value.Owners);
				value.Members = this.Redact(value.Members);
			}
			return value;
		}

		private AADUserPresentationObject Redact(AADUserPresentationObject value)
		{
			if (value != null)
			{
				value.DisplayName = SuppressingPiiData.Redact(value.DisplayName);
				value.MailNickname = SuppressingPiiData.Redact(value.MailNickname);
				value.Owners = this.Redact(value.Owners);
				value.Members = this.Redact(value.Members);
			}
			return value;
		}

		private AADGroupPresentationObject Redact(AADGroupPresentationObject value)
		{
			if (value != null)
			{
				value.AllowAccessTo = this.Redact(value.AllowAccessTo);
				value.Description = SuppressingPiiData.Redact(value.Description);
				value.DisplayName = SuppressingPiiData.Redact(value.DisplayName);
				value.ExchangeResources = SuppressingPiiData.Redact(value.ExchangeResources);
				value.Mail = SuppressingPiiData.Redact(value.Mail);
				value.PendingMembers = this.Redact(value.PendingMembers);
				value.ProxyAddresses = SuppressingPiiData.Redact(value.ProxyAddresses);
				value.SharePointResources = SuppressingPiiData.Redact(value.SharePointResources);
				value.Owners = this.Redact(value.Owners);
				value.Members = this.Redact(value.Members);
			}
			return value;
		}
	}
}
