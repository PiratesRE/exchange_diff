using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "SiteMailbox", DefaultParameterSetName = "TeamMailboxITPro", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
	public sealed class SetSiteMailbox : SetRecipientObjectTask<RecipientIdParameter, TeamMailbox, ADUser>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public new RecipientIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter]
		public string DisplayName
		{
			get
			{
				return (string)base.Fields["DisplayName"];
			}
			set
			{
				base.Fields["DisplayName"] = value;
			}
		}

		[Parameter]
		public bool Active
		{
			get
			{
				return (bool)base.Fields["Active"];
			}
			set
			{
				base.Fields["Active"] = value;
			}
		}

		[Parameter]
		public bool RemoveDuplicateMessages
		{
			get
			{
				return (bool)base.Fields["RemoveDuplicateMessages"];
			}
			set
			{
				base.Fields["RemoveDuplicateMessages"] = value;
			}
		}

		[Parameter]
		public RecipientIdParameter[] Members
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["Members"];
			}
			set
			{
				base.Fields["Members"] = (value ?? new RecipientIdParameter[0]);
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public RecipientIdParameter[] Owners
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["Owners"];
			}
			set
			{
				base.Fields["Owners"] = (value ?? new RecipientIdParameter[0]);
			}
		}

		[Parameter(Mandatory = false)]
		public Uri SharePointUrl
		{
			get
			{
				return (Uri)base.Fields["SharePointUrl"];
			}
			set
			{
				if (value != null && (!value.IsAbsoluteUri || (!(value.Scheme == Uri.UriSchemeHttps) && !(value.Scheme == Uri.UriSchemeHttp))))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxSharePointUrl), ExchangeErrorCategory.Client, this.Identity);
				}
				base.Fields["SharePointUrl"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxIW")]
		public bool ShowInMyClient
		{
			get
			{
				return (bool)base.Fields["ShowInMyClient"];
			}
			set
			{
				base.Fields["ShowInMyClient"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		private new SwitchParameter IgnoreDefaultScope { get; set; }

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.ParameterSetName == "TeamMailboxIW" && !base.TryGetExecutingUserId(out this.executingUserId))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxCannotIdentifyTheUser), ExchangeErrorCategory.Client, this.Identity);
			}
			bool flag = TeamMailbox.IsRemoteTeamMailbox(this.DataObject);
			if ((!TeamMailbox.IsLocalTeamMailbox(this.DataObject) && !flag) || TeamMailbox.IsPendingDeleteSiteMailbox(this.DataObject))
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ExchangeErrorCategory.Client, this.Identity);
			}
			if (flag && (base.Fields.IsModified("DisplayName") || base.Fields.IsModified("Owners") || base.Fields.IsModified("Members") || base.Fields.IsModified("SharePointUrl") || base.Fields.IsModified("Active")))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorRemoteTeamMailboxIsNotChangeable(this.DataObject.DisplayName)), ExchangeErrorCategory.Client, this.Identity);
			}
			this.tm = TeamMailbox.FromDataObject(this.DataObject);
			this.teamMailboxHelper = new TeamMailboxHelper(this.tm, base.ExchangeRunspaceConfig.ExecutingUser, base.ExchangeRunspaceConfig.ExecutingUserOrganizationId, (IRecipientSession)base.DataSession, new TeamMailboxGetDataObject<ADUser>(base.GetDataObject<ADUser>));
			this.membershipHelper = new TeamMailboxMembershipHelper(this.tm, (IRecipientSession)base.DataSession);
			if (base.ParameterSetName == "TeamMailboxIW")
			{
				if (this.tm.OwnersAndMembers.Contains(this.executingUserId) && !base.Fields.IsModified("DisplayName") && !base.Fields.IsModified("Owners") && !base.Fields.IsModified("Members") && !base.Fields.IsModified("SharePointUrl") && !base.Fields.IsModified("Active") && base.Fields.IsModified("ShowInMyClient"))
				{
					this.executingUserIsMember = true;
				}
				if (!this.executingUserIsMember)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxChangeByNonMember(this.DataObject.DisplayName)), ExchangeErrorCategory.Client, this.Identity);
				}
			}
			int num = base.Fields.IsModified("Owners") ? this.Owners.Length : this.tm.Owners.Count;
			num += (base.Fields.IsModified("Members") ? this.Members.Length : this.tm.Members.Count);
			if (num > 1800)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxTooManyOwnersAndMembers(num, 1800)), ExchangeErrorCategory.Client, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			IList<ADObjectId> list = null;
			IList<ADObjectId> list2 = null;
			IList<ADObjectId> list3 = null;
			IList<ADObjectId> usersToRemove = null;
			bool flag = false;
			if (!this.executingUserIsMember)
			{
				if (this.DisplayName != null && this.DisplayName != this.DataObject.DisplayName)
				{
					this.DataObject.DisplayName = this.DisplayName;
					this.changeTracking = true;
				}
				if (base.Fields.IsModified("Active"))
				{
					if (this.Active)
					{
						if (!this.tm.Active)
						{
							this.tm.ClosedTime = null;
							flag = true;
							this.changeTracking = true;
						}
					}
					else if (this.tm.Active)
					{
						this.tm.ClosedTime = new DateTime?(DateTime.UtcNow);
						flag = true;
						this.changeTracking = true;
					}
				}
				if (base.Fields.IsModified("RemoveDuplicateMessages") && this.tm.RemoveDuplicateMessages != this.RemoveDuplicateMessages)
				{
					this.tm.RemoveDuplicateMessages = this.RemoveDuplicateMessages;
					this.changeTracking = true;
				}
				if (base.Fields.IsModified("Owners") || base.Fields.IsModified("Members"))
				{
					IList<ADObjectId> list4 = this.tm.Owners;
					if (base.Fields.IsModified("Owners"))
					{
						IList<RecipientIdParameter> list5;
						IList<ADUser> list6;
						list4 = this.teamMailboxHelper.RecipientIds2ADObjectIds(this.Owners, out list5, out list6);
						if (list5 != null && list5.Count > 0)
						{
							base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxUserNotResolved(TeamMailboxHelper.GetAggreatedIds(list5))), ExchangeErrorCategory.Client, null);
						}
						if (list6 != null && list6.Count > 0)
						{
							base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxUserNotResolved(TeamMailboxHelper.GetAggreatedUsers(list6))), ExchangeErrorCategory.Client, null);
						}
					}
					IList<ADObjectId> userList = this.tm.Members;
					if (base.Fields.IsModified("Members"))
					{
						IList<RecipientIdParameter> list5;
						IList<ADUser> list6;
						userList = this.teamMailboxHelper.RecipientIds2ADObjectIds(this.Members, out list5, out list6);
						if (list5 != null && list5.Count > 0)
						{
							base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxUserNotResolved(TeamMailboxHelper.GetAggreatedIds(list5))), ExchangeErrorCategory.Client, null);
						}
						if (list6 != null && list6.Count > 0)
						{
							base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxUserNotResolved(TeamMailboxHelper.GetAggreatedUsers(list6))), ExchangeErrorCategory.Client, null);
						}
					}
					IList<ADObjectId> newUserList = TeamMailbox.MergeUsers(list4, userList);
					if (base.Fields.IsModified("Owners") && this.membershipHelper.UpdateTeamMailboxUserList(this.tm.Owners, list4, out list, out list2))
					{
						this.changeTracking = true;
					}
					if (this.membershipHelper.UpdateTeamMailboxUserList(this.tm.OwnersAndMembers, newUserList, out list3, out usersToRemove))
					{
						if (list != null && list.Count != 0)
						{
							TeamMailbox.DiffUsers(list3, list);
						}
						Exception ex = null;
						try
						{
							this.membershipHelper.SetTeamMailboxUserPermissions(list3, usersToRemove, null, true);
						}
						catch (OverflowException ex2)
						{
							ex = ex2;
						}
						catch (COMException ex3)
						{
							ex = ex3;
						}
						catch (UnauthorizedAccessException ex4)
						{
							ex = ex4;
						}
						catch (TransientException ex5)
						{
							ex = ex5;
						}
						catch (DataSourceOperationException ex6)
						{
							ex = ex6;
						}
						if (ex != null)
						{
							base.WriteError(new RecipientTaskException(Strings.ErrorSetTeamMailboxUserPermissions(this.tm.DisplayName, ex.Message)), ExchangeErrorCategory.Client, null);
						}
						try
						{
							new TeamMailboxSecurityRefresher().Refresh(this.DataObject, (IRecipientSession)base.DataSession);
						}
						catch (DatabaseNotFoundException ex7)
						{
							ex = ex7;
						}
						catch (ObjectNotFoundException ex8)
						{
							ex = ex8;
						}
						catch (FormatException ex9)
						{
							ex = ex9;
						}
						if (ex != null)
						{
							base.WriteError(new RecipientTaskException(Strings.ErrorSetTeamMailboxUserPermissions(this.tm.DisplayName, ex.Message)), ExchangeErrorCategory.Client, null);
						}
						this.changeTracking = true;
					}
				}
				if (base.Fields.IsModified("SharePointUrl"))
				{
					try
					{
						this.changeTracking = this.teamMailboxHelper.LinkSharePointSite(this.SharePointUrl, false, this.Force);
					}
					catch (RecipientTaskException exception)
					{
						base.WriteError(exception, ExchangeErrorCategory.Client, this.Identity);
					}
				}
			}
			if (base.Fields.IsModified("ShowInMyClient"))
			{
				if (!this.tm.Active)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxSetShowInMyClientForClosedMailbox(this.DataObject.DisplayName)), ExchangeErrorCategory.Client, this.Identity);
				}
				bool flag2;
				Exception ex10;
				if (this.membershipHelper.SetShowInMyClient(this.executingUserId, this.ShowInMyClient, out flag2, out ex10))
				{
					this.changeTracking = true;
				}
				else if (ex10 != null)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxSetShowInMyClient(this.DataObject.DisplayName, ex10.ToString())), ExchangeErrorCategory.Client, this.Identity);
				}
				else if (flag2)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorTeamMailboxMaxShowInMyClientReached(this.DataObject.DisplayName, 10)), ExchangeErrorCategory.Client, this.Identity);
				}
			}
			base.InternalProcessRecord();
			if (flag)
			{
				TeamMailboxADUserResolver.RemoveIdIfExists(this.tm.Id);
			}
			IList<Exception> list7;
			this.membershipHelper.SetShowInMyClient(list3, usersToRemove, out list7);
			foreach (Exception ex11 in list7)
			{
				this.WriteWarning(Strings.ErrorTeamMailboxResolveUser(ex11.Message));
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			IConfigurable configurable = base.PrepareDataObject();
			if (!TeamMailbox.IsRemoteTeamMailbox(this.DataObject))
			{
				CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, (ADUser)configurable, false, this.ConfirmationMessage, null);
			}
			return configurable;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetTeamMailbox(this.Identity.ToString());
			}
		}

		protected override bool IsObjectStateChanged()
		{
			return this.changeTracking || base.IsObjectStateChanged();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return TeamMailbox.FromDataObject((ADUser)dataObject);
		}

		private TeamMailbox tm;

		private ADObjectId executingUserId;

		private bool changeTracking;

		private bool executingUserIsMember;

		private TeamMailboxMembershipHelper membershipHelper;

		private TeamMailboxHelper teamMailboxHelper;
	}
}
