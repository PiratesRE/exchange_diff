using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Task
{
	internal class ValidateSource : SearchTask<SearchSource>
	{
		internal ValidateSource.ValidateSourceContext TaskContext
		{
			get
			{
				return base.Context.TaskContext as ValidateSource.ValidateSourceContext;
			}
		}

		public override void Process(SearchSource item)
		{
			Recorder.Trace(4L, TraceType.InfoTrace, "ValidateSource.Process Item:", item);
			if (item.Recipient == null || item.Recipient.ADEntry == null)
			{
				Recorder.Trace(4L, TraceType.ErrorTrace, "ValidateSource.Process Failed ADEntry Missing Item:", item);
				base.Executor.Fail(new SearchException(KnownError.ErrorRecipientTypeNotSupported)
				{
					ErrorSource = item
				});
				return;
			}
			if (this.TaskContext != null)
			{
				if (this.TaskContext.AllowedRecipientTypeDetails != null)
				{
					RecipientTypeDetails typeDetails = (RecipientTypeDetails)item.GetProperty(ADRecipientSchema.RecipientTypeDetails);
					if (!this.TaskContext.AllowedRecipientTypeDetails.Any((RecipientTypeDetails t) => t == typeDetails))
					{
						Recorder.Trace(4L, TraceType.ErrorTrace, new object[]
						{
							"ValidateSource.Process Failed RecipientTypeDetails TypeDetials:",
							typeDetails,
							"Allowed:",
							this.TaskContext.AllowedRecipientTypeDetails
						});
						base.Executor.Fail(new SearchException(KnownError.ErrorRecipientTypeNotSupported)
						{
							ErrorSource = item
						});
						return;
					}
				}
				if (this.TaskContext.AllowedRecipientTypes != null)
				{
					RecipientType type = (RecipientType)item.GetProperty(ADRecipientSchema.RecipientType);
					if (!this.TaskContext.AllowedRecipientTypes.Any((RecipientType t) => t == type))
					{
						Recorder.Trace(4L, TraceType.ErrorTrace, new object[]
						{
							"ValidateSource.Process Failed RecipientTypes Type:",
							type,
							"Allowed:",
							this.TaskContext.AllowedRecipientTypes
						});
						base.Executor.Fail(new SearchException(KnownError.ErrorRecipientTypeNotSupported)
						{
							ErrorSource = item
						});
						return;
					}
				}
				if (this.TaskContext.MinimumVersion != null && item.Recipient.ADEntry.ExchangeVersion.IsOlderThan(this.TaskContext.MinimumVersion))
				{
					Recorder.Trace(4L, TraceType.ErrorTrace, new object[]
					{
						"ValidateSource.Process Failed Version Version:",
						item.Recipient.ADEntry.ExchangeVersion,
						"Allowed:",
						this.TaskContext.MinimumVersion
					});
					base.Executor.Fail(new SearchException(KnownError.ErrorMailboxVersionNotSupported)
					{
						ErrorSource = item
					});
					return;
				}
				RecipientSoftDeletedStatusFlags recipientSoftDeletedStatusFlags = (RecipientSoftDeletedStatusFlags)item.GetProperty(ADRecipientSchema.RecipientSoftDeletedStatus);
				if (recipientSoftDeletedStatusFlags != RecipientSoftDeletedStatusFlags.None && !recipientSoftDeletedStatusFlags.HasFlag(RecipientSoftDeletedStatusFlags.Inactive))
				{
					Recorder.Trace(4L, TraceType.ErrorTrace, "Skipping non-inactive, soft deleted item:", recipientSoftDeletedStatusFlags);
					return;
				}
				if (item.SourceType != SourceType.PublicFolder && !string.IsNullOrEmpty(this.TaskContext.RequiredCmdlet) && !string.IsNullOrEmpty(this.TaskContext.RequiredCmdletParameters) && !base.Policy.CallerInfo.IsOpenAsAdmin && base.Policy.RunspaceConfiguration != null && !base.Executor.Policy.RunspaceConfiguration.IsCmdletAllowedInScope(this.TaskContext.RequiredCmdlet, this.TaskContext.RequiredCmdletParameters.Split(new char[]
				{
					','
				}), item.Recipient.ADEntry, ScopeLocation.RecipientWrite))
				{
					Recorder.Trace(4L, TraceType.ErrorTrace, new object[]
					{
						"ValidateSource.Process Failed Permission Entry:",
						item.Recipient.ADEntry,
						"Allowed:",
						this.TaskContext.RequiredCmdlet,
						this.TaskContext.RequiredCmdletParameters
					});
					base.Executor.Fail(new SearchException(KnownError.ErrorNoPermissionToSearchOrHoldMailbox)
					{
						ErrorSource = item
					});
					return;
				}
			}
			base.Executor.EnqueueNext(item);
		}

		internal class ValidateSourceContext
		{
			public ExchangeObjectVersion MinimumVersion { get; set; }

			public IEnumerable<RecipientTypeDetails> AllowedRecipientTypeDetails { get; set; }

			public IEnumerable<RecipientType> AllowedRecipientTypes { get; set; }

			public string RequiredCmdlet { get; set; }

			public string RequiredCmdletParameters { get; set; }
		}
	}
}
