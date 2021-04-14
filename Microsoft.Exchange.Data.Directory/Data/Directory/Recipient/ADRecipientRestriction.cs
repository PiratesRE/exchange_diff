using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ADRecipientPermission;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADRecipientRestriction
	{
		private ADRecipientRestriction(ADObjectId senderId, bool isAdRecipient, bool senderIsAuthenticated, ICollection<ADObjectId> rejectMessagesFrom, ICollection<ADObjectId> rejectMessagesFromDLMembers, ICollection<ADObjectId> acceptMessagesFrom, ICollection<ADObjectId> acceptMessagesFromDLMembers, ICollection<ADObjectId> bypassModerationFrom, ICollection<ADObjectId> bypassModerationFromDLMembers, ICollection<ADObjectId> moderators, ICollection<ADObjectId> managedBy, bool requiresAllSendersAreAuthenticated, bool moderationEnabled, RecipientType recipientType, IRecipientSession session, ISimpleCache<ADObjectId, bool> senderMembershipCache, int adQueryLimit)
		{
			this.senderId = senderId;
			this.isAdRecipient = isAdRecipient;
			this.senderIsAuthenticated = senderIsAuthenticated;
			this.rejectMessagesFrom = rejectMessagesFrom;
			this.rejectMessagesFromDLMembers = rejectMessagesFromDLMembers;
			this.acceptMessagesFrom = acceptMessagesFrom;
			this.acceptMessagesFromDLMembers = acceptMessagesFromDLMembers;
			this.bypassModerationFrom = bypassModerationFrom;
			this.bypassModerationFromDLMembers = bypassModerationFromDLMembers;
			this.moderators = moderators;
			this.managedBy = managedBy;
			this.moderationEnabled = moderationEnabled;
			this.recipientType = recipientType;
			this.requiresAllSendersAreAuthenticated = requiresAllSendersAreAuthenticated;
			this.session = session;
			this.memberOfGroupsCache = senderMembershipCache;
			this.adQueryLimit = adQueryLimit;
			this.isFirstGroupSearch = true;
		}

		public static bool Accepted(RestrictionCheckResult result)
		{
			return (result & (RestrictionCheckResult)3221225472U) == (RestrictionCheckResult)0U;
		}

		public static bool Moderated(RestrictionCheckResult result)
		{
			return (result & RestrictionCheckResult.Moderated) != (RestrictionCheckResult)0U;
		}

		public static bool Failed(RestrictionCheckResult result)
		{
			return (result & (RestrictionCheckResult)2147483648U) != (RestrictionCheckResult)0U;
		}

		public static RestrictionCheckResult CheckDeliveryRestrictionForOneOffRecipient(ADObjectId senderId, bool senderIsAuthenticated)
		{
			return RestrictionCheckResult.AcceptedNoPermissionList;
		}

		public static RestrictionCheckResult CheckDeliveryRestrictionForAuthenticatedSender(ADObjectId senderId, ADRawEntry recipientEntry, IRecipientSession session)
		{
			return ADRecipientRestriction.CheckDeliveryRestriction(senderId, true, (MultiValuedProperty<ADObjectId>)recipientEntry[ADRecipientSchema.RejectMessagesFrom], (MultiValuedProperty<ADObjectId>)recipientEntry[ADRecipientSchema.RejectMessagesFromDLMembers], (MultiValuedProperty<ADObjectId>)recipientEntry[ADRecipientSchema.AcceptMessagesOnlyFrom], (MultiValuedProperty<ADObjectId>)recipientEntry[ADRecipientSchema.AcceptMessagesOnlyFromDLMembers], (MultiValuedProperty<ADObjectId>)recipientEntry[ADRecipientSchema.BypassModerationFrom], (MultiValuedProperty<ADObjectId>)recipientEntry[ADRecipientSchema.BypassModerationFromDLMembers], (MultiValuedProperty<ADObjectId>)recipientEntry[ADRecipientSchema.ModeratedBy], (MultiValuedProperty<ADObjectId>)recipientEntry[ADGroupSchema.ManagedBy], true, (bool)recipientEntry[ADRecipientSchema.ModerationEnabled], (RecipientType)recipientEntry[ADRecipientSchema.RecipientType], session, null, -1);
		}

		public static RestrictionCheckResult CheckDeliveryRestriction(ADObjectId senderId, bool senderIsAuthenticated, ICollection<ADObjectId> rejectMessagesFrom, ICollection<ADObjectId> rejectMessagesFromDLMembers, ICollection<ADObjectId> acceptMessagesFrom, ICollection<ADObjectId> acceptMessagesFromDLMembers, ICollection<ADObjectId> bypassModerationFrom, ICollection<ADObjectId> bypassModerationFromDLMembers, ICollection<ADObjectId> moderators, ICollection<ADObjectId> managedBy, bool requiresAllSendersAreAuthenticated, bool moderationEnabled, RecipientType recipientType, IRecipientSession session, ISimpleCache<ADObjectId, bool> senderMembershipCache)
		{
			return ADRecipientRestriction.CheckDeliveryRestriction(senderId, senderIsAuthenticated, rejectMessagesFrom, rejectMessagesFromDLMembers, acceptMessagesFrom, acceptMessagesFromDLMembers, bypassModerationFrom, bypassModerationFromDLMembers, moderators, managedBy, requiresAllSendersAreAuthenticated, moderationEnabled, recipientType, session, senderMembershipCache, -1);
		}

		public static RestrictionCheckResult CheckDeliveryRestriction(ADObjectId senderId, bool senderIsAuthenticated, ICollection<ADObjectId> rejectMessagesFrom, ICollection<ADObjectId> rejectMessagesFromDLMembers, ICollection<ADObjectId> acceptMessagesFrom, ICollection<ADObjectId> acceptMessagesFromDLMembers, ICollection<ADObjectId> bypassModerationFrom, ICollection<ADObjectId> bypassModerationFromDLMembers, ICollection<ADObjectId> moderators, ICollection<ADObjectId> managedBy, bool requiresAllSendersAreAuthenticated, bool moderationEnabled, RecipientType recipientType, IRecipientSession session, ISimpleCache<ADObjectId, bool> senderMembershipCache, int adQueryLimit)
		{
			ADRecipientRestriction adrecipientRestriction = new ADRecipientRestriction(senderId, true, senderIsAuthenticated, rejectMessagesFrom, rejectMessagesFromDLMembers, acceptMessagesFrom, acceptMessagesFromDLMembers, bypassModerationFrom, bypassModerationFromDLMembers, moderators, managedBy, requiresAllSendersAreAuthenticated, moderationEnabled, recipientType, session, senderMembershipCache, adQueryLimit);
			return adrecipientRestriction.Check();
		}

		private static bool IsNullOrEmptyCollection<T>(ICollection<T> collection)
		{
			return collection == null || collection.Count == 0;
		}

		private IRecipientSession Session
		{
			get
			{
				return this.session;
			}
		}

		private RestrictionCheckResult Check()
		{
			if (!this.CheckAuthentication())
			{
				return (RestrictionCheckResult)2147483655U;
			}
			RestrictionCheckResult restrictionCheckResult = this.CheckPermissionRestriction();
			if (this.moderationEnabled && ADRecipientRestriction.Accepted(restrictionCheckResult))
			{
				ADRecipientRestriction.tracer.TraceDebug<ADObjectId, RestrictionCheckResult>((long)this.GetHashCode(), "Sender {0} permission check result is {1}, with moderation enabled, now checking moderation bypasses", this.senderId, restrictionCheckResult);
				restrictionCheckResult = this.CheckModerationBypass();
			}
			ADRecipientRestriction.tracer.TraceDebug<ADObjectId, RestrictionCheckResult>((long)this.GetHashCode(), "Sender {0} permission check result is {1}", this.senderId, restrictionCheckResult);
			return restrictionCheckResult;
		}

		private RestrictionCheckResult CheckModerationBypass()
		{
			if (this.IsSenderOnSecurityList(this.managedBy))
			{
				return RestrictionCheckResult.AcceptedInOwnersList;
			}
			if (this.IsSenderOnSecurityList(this.moderators))
			{
				return RestrictionCheckResult.AcceptedInModeratorsList;
			}
			if (this.IsSenderOnSecurityList(this.bypassModerationFrom))
			{
				return RestrictionCheckResult.AcceptedInBypassModerationRecipientList;
			}
			bool flag2;
			bool flag = this.TryIsSenderMemberOfWithLimit(this.bypassModerationFromDLMembers, out flag2);
			if (!flag || flag2)
			{
				return RestrictionCheckResult.AcceptedInBypassModerationGroupList;
			}
			return RestrictionCheckResult.Moderated;
		}

		private RestrictionCheckResult CheckPermissionRestriction()
		{
			if (!this.HasPermissionRestriction())
			{
				return RestrictionCheckResult.AcceptedNoPermissionList;
			}
			if (this.senderId == null)
			{
				return this.ShouldAcceptByDefault();
			}
			if (this.IsSenderOnSecurityList(this.rejectMessagesFrom))
			{
				return (RestrictionCheckResult)2147483652U;
			}
			if (this.IsSenderOnSecurityList(this.acceptMessagesFrom))
			{
				return RestrictionCheckResult.AcceptedInRecipientList;
			}
			if (this.rejectMessagesFromDLMembers == null)
			{
				if (this.acceptMessagesFromDLMembers == null)
				{
					goto IL_A9;
				}
			}
			try
			{
				bool flag;
				if (this.TryIsSenderMemberOfWithLimit(this.rejectMessagesFromDLMembers, out flag) && flag)
				{
					return (RestrictionCheckResult)2147483653U;
				}
				bool flag2 = this.TryIsSenderMemberOfWithLimit(this.acceptMessagesFromDLMembers, out flag);
				if (!flag2 || flag)
				{
					return RestrictionCheckResult.AcceptedInGroupList;
				}
			}
			catch (DataValidationException arg)
			{
				ADRecipientRestriction.tracer.TraceError<DataValidationException, ADObjectId>((long)this.GetHashCode(), "Data validation exception {0} while checking DL membership for sender '{1}'", arg, this.senderId);
				return (RestrictionCheckResult)2147483656U;
			}
			IL_A9:
			return this.ShouldAcceptByDefault();
		}

		private RestrictionCheckResult ShouldAcceptByDefault()
		{
			if (ADRecipientRestriction.IsNullOrEmptyCollection<ADObjectId>(this.acceptMessagesFrom) && ADRecipientRestriction.IsNullOrEmptyCollection<ADObjectId>(this.acceptMessagesFromDLMembers))
			{
				return RestrictionCheckResult.AcceptedAcceptanceListEmpty;
			}
			return (RestrictionCheckResult)2147483654U;
		}

		private bool HasPermissionRestriction()
		{
			if (!this.isAdRecipient)
			{
				return false;
			}
			bool flag = ADRecipientRestriction.IsNullOrEmptyCollection<ADObjectId>(this.acceptMessagesFrom) && ADRecipientRestriction.IsNullOrEmptyCollection<ADObjectId>(this.acceptMessagesFromDLMembers) && ADRecipientRestriction.IsNullOrEmptyCollection<ADObjectId>(this.rejectMessagesFrom) && ADRecipientRestriction.IsNullOrEmptyCollection<ADObjectId>(this.rejectMessagesFromDLMembers);
			return !flag;
		}

		private bool CheckAuthentication()
		{
			return !this.isAdRecipient || (!this.requiresAllSendersAreAuthenticated && this.recipientType != RecipientType.SystemMailbox && this.recipientType != RecipientType.SystemAttendantMailbox) || this.senderIsAuthenticated;
		}

		private bool IsSenderOnSecurityList(ICollection<ADObjectId> list)
		{
			return list != null && list.Contains(this.senderId);
		}

		private bool TryIsSenderMemberOfWithLimit(IEnumerable<ADObjectId> groupIdList, out bool isSenderMemberOf)
		{
			isSenderMemberOf = false;
			if (groupIdList == null)
			{
				return true;
			}
			if (this.CheckMemberOfGroupInformationInCache(groupIdList))
			{
				ADRecipientRestriction.tracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "Found the IsMemberOf info in cache: {0}", this.senderId);
				isSenderMemberOf = true;
				return true;
			}
			foreach (ADObjectId adobjectId in groupIdList)
			{
				bool flag;
				if (this.memberOfGroupsCache != null && this.memberOfGroupsCache.TryGetValue(adobjectId, out flag))
				{
					ADRecipientRestriction.tracer.TraceDebug<ADObjectId, bool>((long)this.GetHashCode(), "Found the MemberOfGroup info in Cache: {0} : {1}", this.senderId, flag);
					ExAssert.RetailAssert(!flag, "We should have checked the cache before going to AD.");
				}
				else
				{
					if (!ADRecipient.TryIsMemberOfWithLimit(this.senderId, adobjectId, false, this.Session, ref this.adQueryLimit, out flag))
					{
						if (this.isFirstGroupSearch)
						{
							this.AddMemberOfGroup(adobjectId, false);
						}
						this.isFirstGroupSearch = false;
						return false;
					}
					this.isFirstGroupSearch = false;
					this.AddMemberOfGroup(adobjectId, flag);
					if (flag)
					{
						ADRecipientRestriction.tracer.TraceDebug<ADObjectId, ADObjectId>((long)this.GetHashCode(), "Sender {0} is a member of group {1}", this.senderId, adobjectId);
						isSenderMemberOf = true;
						return true;
					}
				}
			}
			return true;
		}

		private bool CheckMemberOfGroupInformationInCache(IEnumerable<ADObjectId> groupIdList)
		{
			if (this.memberOfGroupsCache == null)
			{
				return false;
			}
			foreach (ADObjectId key in groupIdList)
			{
				bool flag;
				if (this.memberOfGroupsCache.TryGetValue(key, out flag) && flag)
				{
					return true;
				}
			}
			return false;
		}

		private void AddMemberOfGroup(ADObjectId groupId, bool isMember)
		{
			if (this.memberOfGroupsCache != null)
			{
				this.memberOfGroupsCache.TryAddValue(groupId, isMember);
			}
		}

		internal const int ADQueryUnlimited = -1;

		private static readonly Trace tracer = ExTraceGlobals.ADPermissionTracer;

		private ADObjectId senderId;

		private bool senderIsAuthenticated;

		private bool isAdRecipient;

		private ICollection<ADObjectId> rejectMessagesFrom;

		private ICollection<ADObjectId> rejectMessagesFromDLMembers;

		private ICollection<ADObjectId> acceptMessagesFrom;

		private ICollection<ADObjectId> acceptMessagesFromDLMembers;

		private ICollection<ADObjectId> bypassModerationFrom;

		private ICollection<ADObjectId> bypassModerationFromDLMembers;

		private ICollection<ADObjectId> moderators;

		private ICollection<ADObjectId> managedBy;

		private bool moderationEnabled;

		private bool requiresAllSendersAreAuthenticated;

		private RecipientType recipientType;

		private IRecipientSession session;

		private ISimpleCache<ADObjectId, bool> memberOfGroupsCache;

		private int adQueryLimit;

		private bool isFirstGroupSearch;
	}
}
