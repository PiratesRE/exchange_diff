using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics.Components.ADRecipientExpansion;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADRecipientExpansion
	{
		public ADRecipientExpansion() : this(OrganizationId.ForestWideOrgId)
		{
		}

		public ADRecipientExpansion(IList<PropertyDefinition> additionalProperties) : this(additionalProperties, OrganizationId.ForestWideOrgId)
		{
		}

		public ADRecipientExpansion(OrganizationId scope)
		{
			this.allProperties = ADRecipientExpansion.requiredProperties;
			this.session = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(scope), 248, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Recipient\\ADRecipientExpansion.cs");
		}

		public ADRecipientExpansion(IRecipientSession session, bool ignoreMailEnabledCase)
		{
			this.ignoreMailEnabledCase = ignoreMailEnabledCase;
			this.allProperties = ADRecipientExpansion.requiredProperties;
			this.session = session;
		}

		public ADRecipientExpansion(IRecipientSession session, bool ignoreMailEnabledCase, IList<PropertyDefinition> additionalProperties)
		{
			this.ignoreMailEnabledCase = ignoreMailEnabledCase;
			this.session = session;
			this.SetAdditionalProperties(additionalProperties);
		}

		public ADRecipientExpansion(IList<PropertyDefinition> additionalProperties, OrganizationId scope) : this(scope)
		{
			this.SetAdditionalProperties(additionalProperties);
		}

		public static IList<PropertyDefinition> RequiredProperties
		{
			get
			{
				return ADRecipientExpansion.requiredPropertiesReadOnly;
			}
		}

		public ADObjectId SecurityContext
		{
			get
			{
				return this.securityContext;
			}
			set
			{
				this.securityContext = value;
			}
		}

		public TimeSpan? LdapTimeout
		{
			get
			{
				return this.session.ServerTimeout;
			}
			set
			{
				this.session.ServerTimeout = value;
			}
		}

		public void Expand(ADRawEntry recipientToExpand, ADRecipientExpansion.HandleRecipientDelegate handleRecipient, ADRecipientExpansion.HandleFailureDelegate handleFailure)
		{
			if (recipientToExpand == null)
			{
				throw new ArgumentNullException("recipientToExpand");
			}
			ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Requested to expand recipient: {0}", new object[]
			{
				recipientToExpand[ADObjectSchema.Id]
			});
			Stack<ADRecipientExpansion.ExpandableEntry> stack = new Stack<ADRecipientExpansion.ExpandableEntry>();
			if (!this.ProcessChild(recipientToExpand, null, handleRecipient, handleFailure, stack))
			{
				return;
			}
			while (stack.Count > 0)
			{
				ADRecipientExpansion.ExpandableEntry expandableEntry = stack.Pop();
				ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Expanding recipient: {0}", new object[]
				{
					expandableEntry.Entry[ADObjectSchema.Id]
				});
				if (!this.ExpandEntry(expandableEntry, handleRecipient, handleFailure, stack))
				{
					ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Expansion terminated by delegate");
					return;
				}
			}
			ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Expansion completed");
		}

		private static ExpansionType GetExpansionType(ADRawEntry entry)
		{
			RecipientType recipientType = (RecipientType)entry[ADRecipientSchema.RecipientType];
			switch (recipientType)
			{
			case RecipientType.Invalid:
			case RecipientType.PublicDatabase:
			case RecipientType.SystemAttendantMailbox:
			case RecipientType.SystemMailbox:
			case RecipientType.Computer:
				return ExpansionType.None;
			case RecipientType.User:
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
				if (entry[ADRecipientSchema.ExternalEmailAddress] != null)
				{
					return ADRecipientExpansion.GetContactExpansionType(entry);
				}
				return ADRecipientExpansion.GetAlternateRecipientType(entry);
			case RecipientType.Contact:
			case RecipientType.MailContact:
				return ADRecipientExpansion.GetContactExpansionType(entry);
			case RecipientType.Group:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.DynamicDistributionGroup:
				return ExpansionType.GroupMembership;
			case RecipientType.PublicFolder:
			case RecipientType.MicrosoftExchange:
				return ADRecipientExpansion.GetAlternateRecipientType(entry);
			default:
				throw new InvalidOperationException("Unknown recipient type: " + recipientType);
			}
		}

		private static ExpansionType GetAlternateRecipientType(ADRawEntry recipient)
		{
			if (recipient[ADRecipientSchema.ForwardingAddress] == null)
			{
				return ExpansionType.None;
			}
			if (!(bool)recipient[IADMailStorageSchema.DeliverToMailboxAndForward])
			{
				return ExpansionType.AlternateRecipientForward;
			}
			return ExpansionType.AlternateRecipientDeliverAndForward;
		}

		private static ExpansionType GetContactExpansionType(ADRawEntry contact)
		{
			MultiValuedProperty<ProxyAddress> multiValuedProperty = (MultiValuedProperty<ProxyAddress>)contact[ADRecipientSchema.EmailAddresses];
			ProxyAddress proxyAddress = (ProxyAddress)contact[ADRecipientSchema.ExternalEmailAddress];
			if (!(proxyAddress == null) && !multiValuedProperty.Contains(proxyAddress))
			{
				return ExpansionType.ContactChain;
			}
			return ExpansionType.None;
		}

		private static ExpansionControl InvokeRecipientDelegate(ADRecipientExpansion.HandleRecipientDelegate handleRecipient, ADRawEntry recipient, ExpansionType expansionType, ADRecipientExpansion.ExpandableEntry parent)
		{
			if (handleRecipient == null)
			{
				return ExpansionControl.Continue;
			}
			ExpansionType expansionType2 = (parent == null) ? ExpansionType.None : parent.ExpansionType;
			ExTraceGlobals.ADExpansionTracer.TraceDebug(0L, "Invoking recipient delegate: recipient={0}; expansion-type={1}; parent={2}; parent-expansion-type={3}", new object[]
			{
				recipient[ADObjectSchema.Id],
				ADRecipientExpansion.GetExpansionTypeString(expansionType),
				(parent == null) ? "<null>" : parent.Entry[ADObjectSchema.Id],
				ADRecipientExpansion.GetExpansionTypeString(expansionType2)
			});
			ExpansionControl expansionControl = handleRecipient(recipient, expansionType, (parent == null) ? null : parent.Entry, expansionType2);
			ExTraceGlobals.ADExpansionTracer.TraceDebug<string>(0L, "Delegate returned '{0}'", ADRecipientExpansion.GetExpansionControlString(expansionControl));
			return expansionControl;
		}

		private static ExpansionControl InvokeFailureDelegate(ADRecipientExpansion.HandleFailureDelegate handleFailure, ExpansionFailure failure, ADRawEntry recipient, ExpansionType expansionType, ADRecipientExpansion.ExpandableEntry parent)
		{
			if (handleFailure == null)
			{
				return ExpansionControl.Continue;
			}
			ExpansionType expansionType2 = (parent == null) ? ExpansionType.None : parent.ExpansionType;
			ExTraceGlobals.ADExpansionTracer.TraceDebug(0L, "Invoking failure delegate: failure={0}; recipient={1}; expansion-type={2}; parent={3}; parent-expansion-type={4}", new object[]
			{
				ADRecipientExpansion.GetExpansionFailureString(failure),
				recipient[ADObjectSchema.Id],
				ADRecipientExpansion.GetExpansionTypeString(expansionType),
				(parent == null) ? "<null>" : parent.Entry[ADObjectSchema.Id],
				ADRecipientExpansion.GetExpansionTypeString(expansionType2)
			});
			ExpansionControl expansionControl = handleFailure(failure, recipient, expansionType, (parent == null) ? null : parent.Entry, expansionType2);
			ExTraceGlobals.ADExpansionTracer.TraceDebug<string>(0L, "Delegate returned '{0}'", ADRecipientExpansion.GetExpansionControlString(expansionControl));
			return expansionControl;
		}

		private static string GetExpansionTypeString(ExpansionType expansionType)
		{
			return ADRecipientExpansion.expansionTypeStrings[(int)expansionType];
		}

		private static string GetExpansionControlString(ExpansionControl expansionControl)
		{
			return ADRecipientExpansion.expansionControlStrings[(int)expansionControl];
		}

		private static string GetExpansionFailureString(ExpansionFailure expansionFailure)
		{
			return ADRecipientExpansion.expansionFailureStrings[(int)expansionFailure];
		}

		private static bool InvokeFailureDelegate(ADRecipientExpansion.HandleFailureDelegate handleFailure, ExpansionFailure failure, ADRecipientExpansion.ExpandableEntry recipient)
		{
			return ADRecipientExpansion.InvokeFailureDelegate(handleFailure, failure, recipient.Entry, recipient.ExpansionType, recipient.Parent) != ExpansionControl.Terminate;
		}

		private void SetAdditionalProperties(IList<PropertyDefinition> additionalProperties)
		{
			if (additionalProperties == null)
			{
				throw new ArgumentNullException("additionalProperties");
			}
			List<PropertyDefinition> list = new List<PropertyDefinition>(additionalProperties.Count + ADRecipientExpansion.RequiredProperties.Count);
			list.AddRange(ADRecipientExpansion.requiredProperties);
			list.AddRange(additionalProperties);
			this.allProperties = list.ToArray();
		}

		private bool ExpandEntry(ADRecipientExpansion.ExpandableEntry entry, ADRecipientExpansion.HandleRecipientDelegate handleRecipient, ADRecipientExpansion.HandleFailureDelegate handleFailure, Stack<ADRecipientExpansion.ExpandableEntry> expansionStack)
		{
			switch (entry.ExpansionType)
			{
			case ExpansionType.GroupMembership:
				return this.ExpandGroup(entry, handleRecipient, handleFailure, expansionStack);
			case ExpansionType.AlternateRecipientForward:
			case ExpansionType.AlternateRecipientDeliverAndForward:
				return this.ExpandAlternateRecipient(entry, handleRecipient, handleFailure, expansionStack);
			case ExpansionType.ContactChain:
				return this.ExpandContactChain(entry, handleRecipient, handleFailure, expansionStack);
			default:
				throw new InvalidOperationException("Invalid expansion type: " + entry.ExpansionType.ToString());
			}
		}

		private bool ExpandGroup(ADRecipientExpansion.ExpandableEntry group, ADRecipientExpansion.HandleRecipientDelegate handleRecipient, ADRecipientExpansion.HandleFailureDelegate handleFailure, Stack<ADRecipientExpansion.ExpandableEntry> expansionStack)
		{
			ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Expanding group: {0}", new object[]
			{
				group.Entry[ADObjectSchema.Id]
			});
			IADDistributionList iaddistributionList = this.session.FindByObjectGuid((Guid)group.Entry[ADObjectSchema.Guid]) as IADDistributionList;
			if (iaddistributionList == null)
			{
				ExTraceGlobals.ADExpansionTracer.TraceError((long)this.GetHashCode(), "Could not find group object by GUID {0}; treating the group as empty", new object[]
				{
					group.Entry[ADObjectSchema.Guid]
				});
				return ADRecipientExpansion.InvokeFailureDelegate(handleFailure, ExpansionFailure.NoMembers, group);
			}
			ADPagedReader<ADRawEntry> adpagedReader = iaddistributionList.Expand(1000, this.allProperties);
			bool flag = false;
			foreach (ADRawEntry child in adpagedReader)
			{
				flag = true;
				if (!this.ProcessChild(child, group, handleRecipient, handleFailure, expansionStack))
				{
					return false;
				}
			}
			if (!flag)
			{
				ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Expanded empty group: {0}", new object[]
				{
					group.Entry[ADObjectSchema.Id]
				});
				return ADRecipientExpansion.InvokeFailureDelegate(handleFailure, ExpansionFailure.NoMembers, group);
			}
			return true;
		}

		private bool ExpandAlternateRecipient(ADRecipientExpansion.ExpandableEntry recipient, ADRecipientExpansion.HandleRecipientDelegate handleRecipient, ADRecipientExpansion.HandleFailureDelegate handleFailure, Stack<ADRecipientExpansion.ExpandableEntry> expansionStack)
		{
			ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Expanding alternate recipient for: {0}", new object[]
			{
				recipient.Entry[ADObjectSchema.Id]
			});
			ADRawEntry adrawEntry = this.session.ReadADRawEntry((ADObjectId)recipient.Entry[ADRecipientSchema.ForwardingAddress], this.allProperties);
			if (adrawEntry == null)
			{
				ExTraceGlobals.ADExpansionTracer.TraceError((long)this.GetHashCode(), "Alternate recipient {0} for recipient {1} not found", new object[]
				{
					recipient.Entry[ADRecipientSchema.ForwardingAddress],
					recipient.Entry[ADObjectSchema.Id]
				});
				return ADRecipientExpansion.InvokeFailureDelegate(handleFailure, ExpansionFailure.AlternateRecipientNotFound, recipient);
			}
			return this.ProcessChild(adrawEntry, recipient, handleRecipient, handleFailure, expansionStack);
		}

		private bool ExpandContactChain(ADRecipientExpansion.ExpandableEntry contact, ADRecipientExpansion.HandleRecipientDelegate handleRecipient, ADRecipientExpansion.HandleFailureDelegate handleFailure, Stack<ADRecipientExpansion.ExpandableEntry> expansionStack)
		{
			ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Expanding possible contact chain for: {0}", new object[]
			{
				contact.Entry[ADObjectSchema.Id]
			});
			ADRawEntry adrawEntry = this.session.FindByProxyAddress((ProxyAddress)contact.Entry[ADRecipientSchema.ExternalEmailAddress], this.allProperties);
			if (handleRecipient != null)
			{
				ExpansionType expansionType = (adrawEntry == null) ? ExpansionType.None : ExpansionType.ContactChain;
				ExpansionControl expansionControl = ADRecipientExpansion.InvokeRecipientDelegate(handleRecipient, contact.Entry, expansionType, contact.Parent);
				if (expansionControl != ExpansionControl.Continue)
				{
					return expansionControl != ExpansionControl.Terminate;
				}
			}
			if (adrawEntry != null)
			{
				ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Found chained object: {0}", new object[]
				{
					adrawEntry[ADObjectSchema.Id]
				});
				return this.ProcessChild(adrawEntry, contact, handleRecipient, handleFailure, expansionStack);
			}
			ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "No contact chain found");
			return true;
		}

		private bool ProcessChild(ADRawEntry child, ADRecipientExpansion.ExpandableEntry parent, ADRecipientExpansion.HandleRecipientDelegate handleRecipient, ADRecipientExpansion.HandleFailureDelegate handleFailure, Stack<ADRecipientExpansion.ExpandableEntry> expansionStack)
		{
			ExpansionFailure failure = ExpansionFailure.NotMailEnabled;
			bool flag = false;
			ExpansionType expansionType = ADRecipientExpansion.GetExpansionType(child);
			ExTraceGlobals.ADExpansionTracer.TraceDebug<object, string>((long)this.GetHashCode(), "Processing recipient {0} with expansion type {1}", child[ADObjectSchema.Id], ADRecipientExpansion.GetExpansionTypeString(expansionType));
			if (!this.ignoreMailEnabledCase && !this.IsMailEnabled(child))
			{
				failure = ExpansionFailure.NotMailEnabled;
				flag = true;
			}
			if (!flag && !this.IsAuthorized(child))
			{
				failure = ExpansionFailure.NotAuthorized;
				flag = true;
			}
			if (!flag && expansionType != ExpansionType.None && this.IsLoopDetected(child, parent))
			{
				failure = ExpansionFailure.LoopDetected;
				flag = true;
			}
			if (flag)
			{
				return ADRecipientExpansion.InvokeFailureDelegate(handleFailure, failure, child, expansionType, parent) != ExpansionControl.Terminate;
			}
			ExpansionControl expansionControl = ExpansionControl.Continue;
			if (expansionType != ExpansionType.ContactChain)
			{
				expansionControl = ADRecipientExpansion.InvokeRecipientDelegate(handleRecipient, child, expansionType, parent);
			}
			if (expansionControl == ExpansionControl.Terminate)
			{
				return false;
			}
			if (expansionControl != ExpansionControl.Skip && expansionType != ExpansionType.None)
			{
				expansionStack.Push(new ADRecipientExpansion.ExpandableEntry(child, expansionType, parent));
				ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Recipient {0} pushed on the expansion stack", new object[]
				{
					child[ADObjectSchema.Id]
				});
			}
			return true;
		}

		private bool IsMailEnabled(ADRawEntry entry)
		{
			SmtpAddress value = (SmtpAddress)entry[ADRecipientSchema.PrimarySmtpAddress];
			if (value != SmtpAddress.Empty && value != SmtpAddress.NullReversePath && value.IsValidAddress)
			{
				return true;
			}
			ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Recipient {0} is not mail-enabled", new object[]
			{
				entry[ADObjectSchema.Id]
			});
			return false;
		}

		private bool IsAuthorized(ADRawEntry entry)
		{
			if (this.securityContext == null)
			{
				return true;
			}
			RestrictionCheckResult restrictionCheckResult = ADRecipientRestriction.CheckDeliveryRestrictionForAuthenticatedSender(this.securityContext, entry, this.session);
			ExTraceGlobals.ADExpansionTracer.TraceDebug<ADObjectId, RestrictionCheckResult, object>((long)this.GetHashCode(), "Sender {0} permission is {1} for recipient {2}", this.securityContext, restrictionCheckResult, entry[ADObjectSchema.Id]);
			return ADRecipientRestriction.Accepted(restrictionCheckResult);
		}

		private bool IsLoopDetected(ADRawEntry entry, ADRecipientExpansion.ExpandableEntry parent)
		{
			while (parent != null)
			{
				if ((Guid)entry[ADObjectSchema.Guid] == (Guid)parent.Entry[ADObjectSchema.Guid])
				{
					ExTraceGlobals.ADExpansionTracer.TraceDebug((long)this.GetHashCode(), "Loop detected for recipient {0}", new object[]
					{
						entry[ADObjectSchema.Id]
					});
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}

		private bool IsOnSecurityList(ICollection<ADObjectId> list)
		{
			return list.Contains(this.securityContext);
		}

		private bool IsMemberOf(IEnumerable<ADObjectId> groupIdList)
		{
			foreach (ADObjectId adobjectId in groupIdList)
			{
				if (ADRecipient.IsMemberOf(this.securityContext, adobjectId, false, this.session))
				{
					ExTraceGlobals.ADExpansionTracer.TraceDebug<ADObjectId, ADObjectId>((long)this.GetHashCode(), "Sender {0} is a member of group {1}", this.securityContext, adobjectId);
					return true;
				}
			}
			return false;
		}

		public const int PageSize = 1000;

		private static readonly PropertyDefinition[] requiredProperties = new PropertyDefinition[]
		{
			ADObjectSchema.Guid,
			ADObjectSchema.Id,
			ADRecipientSchema.AcceptMessagesOnlyFrom,
			ADRecipientSchema.AcceptMessagesOnlyFromDLMembers,
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.ExternalEmailAddress,
			ADRecipientSchema.ForwardingAddress,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.RecipientType,
			ADRecipientSchema.RejectMessagesFrom,
			ADRecipientSchema.RejectMessagesFromDLMembers,
			IADMailStorageSchema.DeliverToMailboxAndForward
		};

		private static readonly ReadOnlyCollection<PropertyDefinition> requiredPropertiesReadOnly = new ReadOnlyCollection<PropertyDefinition>(ADRecipientExpansion.requiredProperties);

		private static readonly string[] expansionTypeStrings = new string[]
		{
			ExpansionType.None.ToString(),
			ExpansionType.GroupMembership.ToString(),
			ExpansionType.AlternateRecipientForward.ToString(),
			ExpansionType.AlternateRecipientDeliverAndForward.ToString(),
			ExpansionType.ContactChain.ToString()
		};

		private static readonly string[] expansionControlStrings = new string[]
		{
			ExpansionControl.Continue.ToString(),
			ExpansionControl.Skip.ToString(),
			ExpansionControl.Terminate.ToString()
		};

		private static readonly string[] expansionFailureStrings = new string[]
		{
			ExpansionFailure.AlternateRecipientNotFound.ToString(),
			ExpansionFailure.LoopDetected.ToString(),
			ExpansionFailure.NoMembers.ToString(),
			ExpansionFailure.NotAuthorized.ToString(),
			ExpansionFailure.NotMailEnabled.ToString()
		};

		private PropertyDefinition[] allProperties;

		private IRecipientSession session;

		private ADObjectId securityContext;

		private bool ignoreMailEnabledCase;

		public delegate ExpansionControl HandleRecipientDelegate(ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType);

		public delegate ExpansionControl HandleFailureDelegate(ExpansionFailure failure, ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType);

		private class ExpandableEntry
		{
			public ExpandableEntry(ADRawEntry entry, ExpansionType expansionType, ADRecipientExpansion.ExpandableEntry parent)
			{
				this.entry = entry;
				this.expansionType = expansionType;
				this.parent = parent;
			}

			public ADRawEntry Entry
			{
				get
				{
					return this.entry;
				}
			}

			public ExpansionType ExpansionType
			{
				get
				{
					return this.expansionType;
				}
			}

			public ADRecipientExpansion.ExpandableEntry Parent
			{
				get
				{
					return this.parent;
				}
			}

			private ADRawEntry entry;

			private ExpansionType expansionType;

			private ADRecipientExpansion.ExpandableEntry parent;
		}
	}
}
