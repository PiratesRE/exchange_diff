using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class InTransitInfo : IComponentData
	{
		private InTransitInfo(InTransitStatus inTransitStatus, List<object> clientHandles)
		{
			this.inTransitStatus = inTransitStatus;
			this.clientHandles = clientHandles;
		}

		internal static void Initialize()
		{
			if (InTransitInfo.inTransitStatusListSlot == -1)
			{
				InTransitInfo.inTransitStatusListSlot = MailboxState.AllocateComponentDataSlot(true);
			}
		}

		public static bool IsMoveUser(InTransitStatus inTransitStatus)
		{
			return (inTransitStatus & InTransitStatus.DirectionMask) != InTransitStatus.NotInTransit;
		}

		public static bool IsMoveDestination(InTransitStatus inTransitStatus)
		{
			return (inTransitStatus & InTransitStatus.DirectionMask) == InTransitStatus.DestinationOfMove;
		}

		public static bool IsMoveSource(InTransitStatus inTransitStatus)
		{
			return (inTransitStatus & InTransitStatus.DirectionMask) == InTransitStatus.SourceOfMove;
		}

		public static bool IsOnlineMove(InTransitStatus inTransitStatus)
		{
			return (inTransitStatus & InTransitStatus.OnlineMove) == InTransitStatus.OnlineMove;
		}

		public static bool IsForPublicFolderMove(InTransitStatus inTransitStatus)
		{
			return (inTransitStatus & InTransitStatus.ForPublicFolderMove) == InTransitStatus.ForPublicFolderMove;
		}

		public static bool IsPureOnlineSourceOfMove(InTransitStatus inTransitStatus)
		{
			return inTransitStatus == (InTransitStatus.SourceOfMove | InTransitStatus.OnlineMove);
		}

		public static ErrorCode SetInTransitState(MailboxState mailboxState, InTransitStatus newStatus, object clientHandle)
		{
			InTransitInfo inTransitInfo = (InTransitInfo)mailboxState.GetComponentData(InTransitInfo.inTransitStatusListSlot);
			if (ConfigurationSchema.MultipleSyncSourceClientsForPublicFolderMailbox.Value)
			{
				if (inTransitInfo != null)
				{
					List<object> list = inTransitInfo.clientHandles;
					if (mailboxState.IsPublicFolderMailbox && clientHandle != null && InTransitInfo.IsPureOnlineSourceOfMove(inTransitInfo.inTransitStatus) && !InTransitInfo.IsOnlineMove(newStatus))
					{
						list = new List<object>
						{
							clientHandle
						};
					}
					else if (mailboxState.IsPublicFolderMailbox && clientHandle != null && InTransitInfo.IsPureOnlineSourceOfMove(inTransitInfo.inTransitStatus) && InTransitInfo.IsPureOnlineSourceOfMove(newStatus))
					{
						if (!list.Contains(clientHandle))
						{
							list.Add(clientHandle);
						}
					}
					else
					{
						if (list.Count > 0 && clientHandle == null)
						{
							return ErrorCode.CreateNoAccess((LID)58916U);
						}
						if (list.Count > 0 && !list.Contains(clientHandle))
						{
							return ErrorCode.CreateNoAccess((LID)47168U);
						}
						if (list.Count > 1 && inTransitInfo.inTransitStatus != newStatus)
						{
							return ErrorCode.CreateNoAccess((LID)34340U);
						}
						if (clientHandle != null && list.Count == 0)
						{
							list.Add(clientHandle);
						}
					}
					if (mailboxState.IsPublicFolderMailbox)
					{
						InTransitInfo.IsPureOnlineSourceOfMove(newStatus);
					}
					mailboxState.SetComponentData(InTransitInfo.inTransitStatusListSlot, new InTransitInfo(newStatus, list));
				}
				else
				{
					List<object> list2 = new List<object>();
					if (clientHandle != null)
					{
						list2.Add(clientHandle);
					}
					mailboxState.SetComponentData(InTransitInfo.inTransitStatusListSlot, new InTransitInfo(newStatus, list2));
				}
			}
			else
			{
				if (inTransitInfo != null && inTransitInfo.clientHandles.Count > 0 && (clientHandle == null || !inTransitInfo.clientHandles.Contains(clientHandle)))
				{
					return ErrorCode.CreateNoAccess((LID)29684U);
				}
				List<object> list3 = new List<object>();
				if (clientHandle != null)
				{
					list3.Add(clientHandle);
				}
				mailboxState.SetComponentData(InTransitInfo.inTransitStatusListSlot, new InTransitInfo(newStatus, list3));
			}
			if ((newStatus & InTransitStatus.OnlineMove) != InTransitStatus.OnlineMove && (newStatus & InTransitStatus.DirectionMask) == InTransitStatus.SourceOfMove)
			{
				mailboxState.InvalidateLogons();
				UserInformation.LockUserEntryForModification(mailboxState.MailboxGuid);
			}
			return ErrorCode.NoError;
		}

		public static ErrorCode RemoveInTransitState(MailboxState mailboxState, object clientHandle)
		{
			InTransitInfo inTransitInfo = (InTransitInfo)mailboxState.GetComponentData(InTransitInfo.inTransitStatusListSlot);
			if (inTransitInfo != null)
			{
				if (clientHandle != null)
				{
					if (!inTransitInfo.clientHandles.Contains(clientHandle))
					{
						return ErrorCode.CreateNoAccess((LID)29688U);
					}
					inTransitInfo.clientHandles.Remove(clientHandle);
				}
				mailboxState.SetComponentData(InTransitInfo.inTransitStatusListSlot, (inTransitInfo.clientHandles.Count != 0) ? inTransitInfo : null);
				if (mailboxState.SupportsPerUserFeatures && InTransitInfo.IsMoveDestination(inTransitInfo.inTransitStatus))
				{
					SearchQueue.DrainSearchQueueTask.Launch(mailboxState);
				}
				UserInformation.UnlockUserEntryForModification(mailboxState.MailboxGuid);
			}
			return ErrorCode.NoError;
		}

		public static InTransitStatus GetInTransitStatusForClient(MailboxState mailboxState, object clientHandle)
		{
			InTransitInfo inTransitInfo = (InTransitInfo)mailboxState.GetComponentData(InTransitInfo.inTransitStatusListSlot);
			if (inTransitInfo != null && inTransitInfo.clientHandles.Contains(clientHandle))
			{
				return inTransitInfo.inTransitStatus;
			}
			return InTransitStatus.NotInTransit;
		}

		public static InTransitStatus GetInTransitStatus(MailboxState mailboxState)
		{
			InTransitInfo inTransitInfo = (InTransitInfo)mailboxState.GetComponentData(InTransitInfo.inTransitStatusListSlot);
			if (inTransitInfo != null)
			{
				return inTransitInfo.inTransitStatus;
			}
			return InTransitStatus.NotInTransit;
		}

		public static List<object> GetInTransitClientHandles(MailboxState mailboxState)
		{
			InTransitInfo inTransitInfo = (InTransitInfo)mailboxState.GetComponentData(InTransitInfo.inTransitStatusListSlot);
			if (inTransitInfo != null)
			{
				return inTransitInfo.clientHandles;
			}
			return null;
		}

		public static bool IsClientNotAllowedToLogIn(MailboxState mailboxState, bool clientIsNotMigrationOrContentIndexing, object clientHandle)
		{
			InTransitInfo inTransitInfo = (InTransitInfo)mailboxState.GetComponentData(InTransitInfo.inTransitStatusListSlot);
			if (inTransitInfo == null)
			{
				return false;
			}
			if ((inTransitInfo.inTransitStatus & InTransitStatus.OnlineMove) == InTransitStatus.OnlineMove && (inTransitInfo.inTransitStatus & InTransitStatus.DirectionMask) == InTransitStatus.DestinationOfMove && clientIsNotMigrationOrContentIndexing)
			{
				return !inTransitInfo.clientHandles.Contains(clientHandle);
			}
			return (inTransitInfo.inTransitStatus & InTransitStatus.OnlineMove) != InTransitStatus.OnlineMove && !inTransitInfo.clientHandles.Contains(clientHandle);
		}

		bool IComponentData.DoCleanup(Context context)
		{
			return false;
		}

		private static int inTransitStatusListSlot = -1;

		private InTransitStatus inTransitStatus;

		private List<object> clientHandles;
	}
}
