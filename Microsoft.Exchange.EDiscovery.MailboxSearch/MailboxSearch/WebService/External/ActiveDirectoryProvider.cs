using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.External
{
	internal class ActiveDirectoryProvider : IDirectoryProvider
	{
		public IEnumerable<SearchRecipient> Query(ISearchPolicy policy, DirectoryQueryParameters request)
		{
			Recorder.Trace(5L, TraceType.InfoTrace, new object[]
			{
				"ActiveDirectoryProvider.Query Query:",
				request.Query,
				"PageSize:",
				request.PageSize,
				"Properties:",
				request.Properties,
				"ExpandGroups:",
				request.ExpandGroups
			});
			ADPagedReader<ADRawEntry> pagedReader = policy.RecipientSession.FindPagedADRawEntry(null, QueryScope.SubTree, request.Query, null, request.PageSize, request.Properties);
			new List<SearchRecipient>();
			foreach (ADRawEntry entry in pagedReader)
			{
				if (request.ExpandGroups && SearchRecipient.IsMembershipGroup(entry))
				{
					Recorder.Trace(5L, TraceType.InfoTrace, "ActiveDirectoryProvider.Query Expanding:", entry);
					List<ADRawEntry> groupEntries = new List<ADRawEntry>();
					ADRecipientExpansion expansion = new ADRecipientExpansion(policy.RecipientSession, false, request.Properties);
					ADRecipientExpansion.HandleRecipientDelegate handleDelegate = delegate(ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
					{
						if (recipientExpansionType == ExpansionType.GroupMembership)
						{
							return ExpansionControl.Continue;
						}
						groupEntries.Add(recipient);
						return ExpansionControl.Skip;
					};
					ADRecipientExpansion.HandleFailureDelegate failureDelegate = delegate(ExpansionFailure expansionFailure, ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
					{
						Recorder.Trace(5L, TraceType.ErrorTrace, new object[]
						{
							"ActiveDirectoryProvider.Query Expansion Failed:",
							recipient,
							"Error:",
							expansionFailure,
							"ExpansionType:",
							recipientExpansionType,
							"Parent:",
							parent,
							"ParentyExpansionType:",
							parentExpansionType
						});
						return ExpansionControl.Skip;
					};
					expansion.Expand(entry, handleDelegate, failureDelegate);
					foreach (ADRawEntry groupEntry in groupEntries)
					{
						yield return new SearchRecipient(groupEntry, entry);
					}
				}
				else
				{
					yield return new SearchRecipient(entry, null);
				}
			}
			yield break;
		}
	}
}
