using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class MailboxAuditLogSearch : AuditLogSearchBase
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MailboxAuditLogSearchSchema>();
			}
		}

		public MultiValuedProperty<ADObjectId> Mailboxes
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailboxAuditLogSearchSchema.MailboxIds];
			}
			set
			{
				this[MailboxAuditLogSearchSchema.MailboxIds] = value;
			}
		}

		public MultiValuedProperty<string> LogonTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxAuditLogSearchSchema.LogonTypeStrings];
			}
			set
			{
				this[MailboxAuditLogSearchSchema.LogonTypeStrings] = value;
			}
		}

		public MultiValuedProperty<string> Operations
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxAuditLogSearchSchema.Operations];
			}
			set
			{
				this[MailboxAuditLogSearchSchema.Operations] = value;
			}
		}

		public bool ShowDetails
		{
			get
			{
				return (bool)this[MailboxAuditLogSearchSchema.ShowDetails];
			}
			set
			{
				this[MailboxAuditLogSearchSchema.ShowDetails] = value;
			}
		}

		internal MultiValuedProperty<AuditScopes> LogonTypesUserInput { get; set; }

		internal MultiValuedProperty<MailboxAuditOperations> OperationsUserInput { get; set; }

		internal override void Initialize(AuditLogSearchItemBase item)
		{
			MailboxAuditLogSearchItem mailboxAuditLogSearchItem = (MailboxAuditLogSearchItem)item;
			base.Initialize(item);
			this.Mailboxes = mailboxAuditLogSearchItem.MailboxIds;
			this.LogonTypes = mailboxAuditLogSearchItem.LogonTypeStrings;
			this.Operations = mailboxAuditLogSearchItem.Operations;
			this.ShowDetails = mailboxAuditLogSearchItem.ShowDetails;
		}

		internal override void Initialize(AuditLogSearchBase item)
		{
			MailboxAuditLogSearch mailboxAuditLogSearch = (MailboxAuditLogSearch)item;
			base.Initialize(item);
			this.Mailboxes = mailboxAuditLogSearch.Mailboxes;
			this.LogonTypes = mailboxAuditLogSearch.LogonTypes;
			this.Operations = mailboxAuditLogSearch.Operations;
			this.ShowDetails = (mailboxAuditLogSearch.ShowDetails ?? false);
		}

		internal void Validate(Task.TaskErrorLoggingDelegate writeError)
		{
			this.LogonTypes = new MultiValuedProperty<string>();
			if (this.LogonTypesUserInput == null)
			{
				this.LogonTypes.Add("Admin");
				this.LogonTypes.Add("Delegate");
				if (this.ShowDetails)
				{
					this.LogonTypes.Add("Owner");
				}
			}
			else
			{
				if (!this.ShowDetails && base.ExternalAccess != null && base.ExternalAccess.Value && this.LogonTypesUserInput.Count == 1 && (this.LogonTypesUserInput[0] & AuditScopes.Delegate) == AuditScopes.Delegate)
				{
					writeError(new ArgumentException(Strings.ErrorInvalidLogonType), ErrorCategory.InvalidArgument, null);
				}
				foreach (AuditScopes auditScopes in this.LogonTypesUserInput)
				{
					if ((auditScopes & AuditScopes.Admin) == AuditScopes.Admin)
					{
						this.LogonTypes.Add("Admin");
					}
					if ((auditScopes & AuditScopes.Delegate) == AuditScopes.Delegate)
					{
						this.LogonTypes.Add("Delegate");
					}
					if ((auditScopes & AuditScopes.Owner) == AuditScopes.Owner)
					{
						if (!this.ShowDetails)
						{
							writeError(new ArgumentException(Strings.ErrorInvalidMailboxAuditLogSearchCriteria), ErrorCategory.InvalidArgument, null);
						}
						else
						{
							this.LogonTypes.Add("Owner");
						}
					}
				}
			}
			if (this.OperationsUserInput != null)
			{
				if (!this.LogonTypes.Contains("Owner") && this.OperationsUserInput.Contains(MailboxAuditOperations.MailboxLogin))
				{
					writeError(new ArgumentException(Strings.ErrorInvalidOperation), ErrorCategory.InvalidArgument, null);
				}
				foreach (MailboxAuditOperations mailboxAuditOperations in this.OperationsUserInput)
				{
					if (mailboxAuditOperations != MailboxAuditOperations.None)
					{
						this.Operations.Add(mailboxAuditOperations.ToString());
					}
				}
			}
		}

		internal QueryFilter GetRecipientFilter()
		{
			QueryFilter recipientTypeDetailsFilter = RecipientIdParameter.GetRecipientTypeDetailsFilter(MailboxAuditLogSearch.SupportedRecipientTypes);
			List<QueryFilter> list = new List<QueryFilter>();
			DateTime value = base.EndDateUtc.Value;
			DateTime value2 = base.StartDateUtc.Value;
			if (base.ExternalAccess == null)
			{
				if (this.LogonTypes != null && this.LogonTypes.Contains("Admin"))
				{
					QueryFilter item = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.AuditLastAdminAccess, value2);
					list.Add(item);
				}
				if (this.LogonTypes != null && this.LogonTypes.Contains("Delegate"))
				{
					QueryFilter item2 = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.AuditLastDelegateAccess, value2);
					list.Add(item2);
				}
				if (this.LogonTypes == null || this.LogonTypes.Count != 1 || !this.LogonTypes[0].Equals("Delegate"))
				{
					QueryFilter item3 = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.AuditLastExternalAccess, value2);
					list.Add(item3);
				}
			}
			else if (base.ExternalAccess.Value)
			{
				QueryFilter item4 = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.AuditLastExternalAccess, value2);
				list.Add(item4);
			}
			else
			{
				if (this.LogonTypes != null && this.LogonTypes.Contains("Admin"))
				{
					QueryFilter item5 = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.AuditLastAdminAccess, value2);
					list.Add(item5);
				}
				if (this.LogonTypes != null && this.LogonTypes.Contains("Delegate"))
				{
					QueryFilter item6 = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.AuditLastDelegateAccess, value2);
					list.Add(item6);
				}
			}
			QueryFilter queryFilter = QueryFilter.OrTogether(list.ToArray());
			base.QueryComplexity = list.Count;
			return new AndFilter(new QueryFilter[]
			{
				recipientTypeDetailsFilter,
				queryFilter
			});
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.ToString());
			stringBuilder.AppendLine();
			AuditLogSearchBase.AppendStringSearchTerm(stringBuilder, "LogonTypes", this.LogonTypes);
			AuditLogSearchBase.AppendStringSearchTerm(stringBuilder, "Operations", this.Operations);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("ShowDetails={0}", this.ShowDetails);
			return stringBuilder.ToString();
		}

		internal static MultiValuedProperty<ADObjectId> ConvertTo(IRecipientSession recipientSession, MultiValuedProperty<MailboxIdParameter> mailboxIds, DataAccessHelper.GetDataObjectDelegate getDataObject, Task.TaskErrorLoggingDelegate writeError)
		{
			if (mailboxIds == null)
			{
				return null;
			}
			MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
			foreach (MailboxIdParameter mailboxIdParameter in mailboxIds)
			{
				ADRecipient adrecipient = (ADRecipient)getDataObject(mailboxIdParameter, recipientSession, null, null, new LocalizedString?(Strings.ExceptionUserObjectNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorSearchUserNotUnique(mailboxIdParameter.ToString())));
				if (Array.IndexOf<RecipientTypeDetails>(MailboxAuditLogSearch.SupportedRecipientTypes, adrecipient.RecipientTypeDetails) == -1)
				{
					writeError(new ArgumentException(Strings.ErrorInvalidRecipientType(adrecipient.ToString(), adrecipient.RecipientTypeDetails.ToString())), ErrorCategory.InvalidArgument, null);
				}
				if (!multiValuedProperty.Contains(adrecipient.Id))
				{
					multiValuedProperty.Add(adrecipient.Id);
				}
			}
			return multiValuedProperty;
		}

		internal const int MaxLogsForEmailAttachment = 50000;

		internal static readonly RecipientTypeDetails[] SupportedRecipientTypes = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.UserMailbox,
			RecipientTypeDetails.EquipmentMailbox,
			RecipientTypeDetails.LinkedMailbox,
			RecipientTypeDetails.RoomMailbox,
			RecipientTypeDetails.TeamMailbox,
			RecipientTypeDetails.SharedMailbox
		};
	}
}
