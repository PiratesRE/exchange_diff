using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ActionList : IList<ActionBase>, ICollection<ActionBase>, IEnumerable<ActionBase>, IEnumerable
	{
		public ActionList(Rule rule)
		{
			this.internalList = new List<ActionBase>();
			this.rule = rule;
		}

		int IList<ActionBase>.IndexOf(ActionBase action)
		{
			return this.internalList.IndexOf(action);
		}

		void IList<ActionBase>.Insert(int index, ActionBase action)
		{
			this.CheckForDuplicate(action);
			this.internalList.Insert(index, action);
			this.rule.SetDirty();
		}

		void IList<ActionBase>.RemoveAt(int index)
		{
			this.internalList.RemoveAt(index);
			this.rule.SetDirty();
		}

		ActionBase IList<ActionBase>.this[int index]
		{
			get
			{
				return this.internalList[index];
			}
			set
			{
				if (value.ActionType != this.internalList[index].ActionType)
				{
					this.CheckForDuplicate(value);
				}
				this.internalList[index] = value;
				this.rule.SetDirty();
			}
		}

		void ICollection<ActionBase>.Add(ActionBase action)
		{
			this.CheckForDuplicate(action);
			this.internalList.Add(action);
			this.rule.SetDirty();
		}

		void ICollection<ActionBase>.Clear()
		{
			this.internalList.Clear();
			this.rule.SetDirty();
		}

		bool ICollection<ActionBase>.Contains(ActionBase action)
		{
			return this.internalList.Contains(action);
		}

		void ICollection<ActionBase>.CopyTo(ActionBase[] actions, int index)
		{
			this.internalList.CopyTo(actions, index);
		}

		bool ICollection<ActionBase>.Remove(ActionBase action)
		{
			this.rule.SetDirty();
			return this.internalList.Remove(action);
		}

		int ICollection<ActionBase>.Count
		{
			get
			{
				return this.internalList.Count;
			}
		}

		bool ICollection<ActionBase>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		IEnumerator<ActionBase> IEnumerable<ActionBase>.GetEnumerator()
		{
			return this.internalList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.internalList.GetEnumerator();
		}

		public ActionBase[] ToArray()
		{
			return this.internalList.ToArray();
		}

		private void CheckForDuplicate(ActionBase newAction)
		{
			foreach (ActionBase actionBase in ((IEnumerable<ActionBase>)this))
			{
				if (actionBase.ActionType == newAction.ActionType)
				{
					this.rule.ThrowValidateException(delegate
					{
						throw new DuplicateActionException(ServerStrings.DuplicateAction);
					}, ServerStrings.DuplicateAction);
				}
				else if (newAction.ActionType == ActionType.DeleteAction)
				{
					if (actionBase.ActionType == ActionType.MoveToFolderAction)
					{
						MoveToFolderAction moveToFolderAction = actionBase as MoveToFolderAction;
						MailboxSession mailboxSession = this.rule.Folder.Session as MailboxSession;
						if (mailboxSession != null && moveToFolderAction != null && moveToFolderAction.Id.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems)))
						{
							this.rule.ThrowValidateException(delegate
							{
								throw new DuplicateActionException(ServerStrings.DuplicateAction);
							}, ServerStrings.DuplicateAction);
						}
					}
				}
				else if (newAction.ActionType == ActionType.MoveToFolderAction)
				{
					MoveToFolderAction moveToFolderAction2 = newAction as MoveToFolderAction;
					MailboxSession mailboxSession2 = this.rule.Folder.Session as MailboxSession;
					if (mailboxSession2 != null && moveToFolderAction2.Id.Equals(mailboxSession2.GetDefaultFolderId(DefaultFolderType.DeletedItems)) && actionBase.ActionType == ActionType.DeleteAction)
					{
						this.rule.ThrowValidateException(delegate
						{
							throw new DuplicateActionException(ServerStrings.DuplicateAction);
						}, ServerStrings.DuplicateAction);
					}
				}
			}
		}

		private List<ActionBase> internalList;

		private Rule rule;
	}
}
