using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	internal class OpenMessageStates : IComponentData
	{
		internal OpenMessageStates()
		{
			this.openStates = new Dictionary<int, OpenMessageStates.OpenMessageState>(8);
		}

		internal static void Initialize()
		{
			if (OpenMessageStates.openMessageStateSlot == -1)
			{
				OpenMessageStates.openMessageStateSlot = MailboxState.AllocateComponentDataSlot(false);
			}
		}

		internal static OpenMessageInstance AddInstance(Context context, Mailbox mailbox, int documentId, DataRow dataRow)
		{
			OpenMessageStates cachedForMailbox = OpenMessageStates.GetCachedForMailbox(mailbox.SharedState, true);
			OpenMessageInstance result;
			using (LockManager.Lock(cachedForMailbox.openStates))
			{
				OpenMessageStates.OpenMessageState openMessageState;
				if (!cachedForMailbox.TryGetOpenMessageState(documentId, out openMessageState))
				{
					openMessageState = new OpenMessageStates.OpenMessageState(cachedForMailbox, documentId);
					cachedForMailbox.AddDocumentId(documentId, openMessageState);
				}
				OpenMessageInstance openMessageInstance = openMessageState.AddInstance(context, dataRow);
				result = openMessageInstance;
			}
			return result;
		}

		internal static bool DoesOpenMessageStateExist(Context context, Mailbox mailbox, int documentId)
		{
			OpenMessageStates cachedForMailbox = OpenMessageStates.GetCachedForMailbox(mailbox.SharedState, false);
			if (cachedForMailbox == null)
			{
				return false;
			}
			bool result;
			using (LockManager.Lock(cachedForMailbox.openStates))
			{
				OpenMessageStates.OpenMessageState openMessageState = null;
				result = cachedForMailbox.TryGetOpenMessageState(documentId, out openMessageState);
			}
			return result;
		}

		internal static void RemoveInstance(Context context, Mailbox mailbox, OpenMessageInstance instance)
		{
			OpenMessageStates.OpenMessageState state = instance.State;
			OpenMessageStates states = state.States;
			using (LockManager.Lock(states.openStates))
			{
				state.RemoveInstance(context, instance);
				if (state.Instances == null || state.Instances.Count == 0)
				{
					states.RemoveDocumentId(state.DocumentId);
				}
			}
		}

		internal static void OnBeforeFlushOrDelete(Context context, OpenMessageInstance instance, bool delete, bool move, bool nonConflictingChange)
		{
			OpenMessageStates.OpenMessageState state = instance.State;
			OpenMessageStates states = state.States;
			using (LockManager.Lock(states.openStates))
			{
				state.OnBeforeInstanceFlushOrDelete(context, instance, delete, move, nonConflictingChange);
			}
		}

		private static OpenMessageStates GetCachedForMailbox(MailboxState mailboxState, bool create)
		{
			OpenMessageStates openMessageStates = (OpenMessageStates)mailboxState.GetComponentData(OpenMessageStates.openMessageStateSlot);
			if (openMessageStates == null && create)
			{
				openMessageStates = new OpenMessageStates();
				OpenMessageStates openMessageStates2 = (OpenMessageStates)mailboxState.CompareExchangeComponentData(OpenMessageStates.openMessageStateSlot, null, openMessageStates);
				if (openMessageStates2 != null)
				{
					openMessageStates = openMessageStates2;
				}
			}
			return openMessageStates;
		}

		bool IComponentData.DoCleanup(Context context)
		{
			return this.openStates.Count == 0;
		}

		private bool TryGetOpenMessageState(int documentId, out OpenMessageStates.OpenMessageState value)
		{
			return this.openStates.TryGetValue(documentId, out value);
		}

		private void AddDocumentId(int documentId, OpenMessageStates.OpenMessageState value)
		{
			this.openStates.Add(documentId, value);
		}

		private void RemoveDocumentId(int documentId)
		{
			this.openStates.Remove(documentId);
		}

		private void InvokeOnCommit(Context context, OpenMessageStates.OpenMessageState openMessageState)
		{
			using (LockManager.Lock(this.openStates))
			{
				openMessageState.OnCommitImplementation(context);
			}
		}

		private void InvokeOnAbort(Context context, OpenMessageStates.OpenMessageState openMessageState)
		{
			using (LockManager.Lock(this.openStates))
			{
				openMessageState.OnAbortImplementation(context);
			}
		}

		private static int openMessageStateSlot = -1;

		private Dictionary<int, OpenMessageStates.OpenMessageState> openStates;

		internal class OpenMessageState : IStateObject
		{
			internal OpenMessageState(OpenMessageStates states, int documentId)
			{
				this.states = states;
				this.documentId = documentId;
			}

			internal IList<OpenMessageInstance> Instances
			{
				get
				{
					return this.instances;
				}
			}

			internal int DocumentId
			{
				get
				{
					return this.documentId;
				}
			}

			internal OpenMessageStates States
			{
				get
				{
					return this.states;
				}
			}

			internal void OnBeforeInstanceFlushOrDelete(Context context, OpenMessageInstance instance, bool delete, bool move, bool nonConflictingChange)
			{
				using (context.CriticalBlock((LID)36832U, CriticalBlockScope.MailboxShared))
				{
					if (!context.IsStateObjectRegistered(this))
					{
						context.RegisterStateObject(this);
					}
					instance.Tentative = true;
					if (this.instances != null && this.instances.Count != 0 && (this.instances.Count > 1 || this.instances[0] != instance))
					{
						IList<PhysicalColumn> list = instance.DataRow.Table.Columns;
						if (!delete)
						{
							list = new List<PhysicalColumn>(8);
							foreach (PhysicalColumn physicalColumn in instance.DataRow.Table.Columns)
							{
								if (instance.DataRow.ColumnDirty(physicalColumn))
								{
									list.Add(physicalColumn);
								}
							}
						}
						foreach (OpenMessageInstance openMessageInstance in this.instances)
						{
							if (!object.ReferenceEquals(openMessageInstance, instance))
							{
								bool flag = true;
								if (!delete && nonConflictingChange)
								{
									if (openMessageInstance.Current)
									{
										flag = false;
										foreach (PhysicalColumn column in list)
										{
											if (openMessageInstance.DataRow.ColumnDirty(column))
											{
												flag = true;
												break;
											}
										}
									}
									foreach (PhysicalColumn physicalColumn2 in list)
									{
										if (!openMessageInstance.DataRow.ColumnDirty(physicalColumn2))
										{
											if ((openMessageInstance.Current && !openMessageInstance.Tentative) || (!openMessageInstance.Current && openMessageInstance.Tentative))
											{
												bool flag2 = false;
												if (this.tentativelyOverridenColumns != null)
												{
													foreach (ColumnValue columnValue in this.tentativelyOverridenColumns)
													{
														if (columnValue.Column == physicalColumn2)
														{
															flag2 = true;
															break;
														}
													}
												}
												if (!flag2)
												{
													if (this.tentativelyOverridenColumns == null)
													{
														this.tentativelyOverridenColumns = new List<ColumnValue>(list.Count);
													}
													this.tentativelyOverridenColumns.Add(new ColumnValue(physicalColumn2, openMessageInstance.DataRow.GetValue(context, physicalColumn2)));
												}
											}
											openMessageInstance.DataRow.SetValue(context, physicalColumn2, instance.DataRow.GetValue(context, physicalColumn2), true);
										}
									}
								}
								if (flag)
								{
									openMessageInstance.DataRow.Load(context, list, true);
									if (openMessageInstance.Current)
									{
										openMessageInstance.DataRow.MarkDisconnected();
										openMessageInstance.Current = false;
										openMessageInstance.Tentative = !openMessageInstance.Tentative;
										openMessageInstance.Moved = move;
										openMessageInstance.Deleted = delete;
									}
								}
							}
						}
					}
					context.EndCriticalBlock();
				}
			}

			void IStateObject.OnBeforeCommit(Context context)
			{
			}

			void IStateObject.OnCommit(Context context)
			{
				this.States.InvokeOnCommit(context, this);
			}

			void IStateObject.OnAbort(Context context)
			{
				this.States.InvokeOnAbort(context, this);
			}

			internal void OnCommitImplementation(Context context)
			{
				if (this.instances != null)
				{
					foreach (OpenMessageInstance openMessageInstance in this.instances)
					{
						openMessageInstance.Tentative = false;
					}
				}
				this.tentativelyOverridenColumns = null;
			}

			internal void OnAbortImplementation(Context context)
			{
				using (context.CriticalBlock((LID)53216U, CriticalBlockScope.MailboxShared))
				{
					if (this.instances != null)
					{
						foreach (OpenMessageInstance openMessageInstance in this.instances)
						{
							if (openMessageInstance.Tentative)
							{
								openMessageInstance.Tentative = false;
								openMessageInstance.Moved = false;
								openMessageInstance.Deleted = false;
								if (!openMessageInstance.Current)
								{
									openMessageInstance.Current = true;
									openMessageInstance.DataRow.MarkReconnected();
								}
								else
								{
									openMessageInstance.Current = false;
									openMessageInstance.DataRow.MarkDisconnected();
								}
							}
							if (this.tentativelyOverridenColumns != null)
							{
								bool flag = false;
								foreach (ColumnValue columnValue in this.tentativelyOverridenColumns)
								{
									if (openMessageInstance.DataRow.ColumnDirty((PhysicalColumn)columnValue.Column))
									{
										flag = true;
									}
									else
									{
										openMessageInstance.DataRow.SetValue(context, (PhysicalColumn)columnValue.Column, columnValue.Value, true);
									}
								}
								if (flag && openMessageInstance.Current)
								{
									openMessageInstance.Current = false;
									openMessageInstance.DataRow.MarkDisconnected();
								}
							}
						}
					}
					this.tentativelyOverridenColumns = null;
					context.EndCriticalBlock();
				}
			}

			internal OpenMessageInstance AddInstance(Context context, DataRow dataRow)
			{
				if (this.instances == null)
				{
					this.instances = new List<OpenMessageInstance>(2);
				}
				OpenMessageInstance openMessageInstance = new OpenMessageInstance(this, dataRow);
				this.instances.Add(openMessageInstance);
				if (context.TransactionStarted)
				{
					if (!context.IsStateObjectRegistered(this))
					{
						context.RegisterStateObject(this);
					}
					openMessageInstance.Tentative = true;
				}
				this.CleanUnreferencedInstances(context);
				return openMessageInstance;
			}

			internal void RemoveInstance(Context context, OpenMessageInstance instance)
			{
				this.instances.Remove(instance);
				if (this.instances.Count == 0)
				{
					if (context != null)
					{
						context.UnregisterStateObject(this);
					}
					this.instances = null;
				}
			}

			private void CleanUnreferencedInstances(Context context)
			{
				if (this.instances != null)
				{
					for (int i = this.instances.Count - 1; i >= 0; i--)
					{
						OpenMessageInstance openMessageInstance = this.instances[i];
						if (!openMessageInstance.Referenced)
						{
							this.RemoveInstance(context, openMessageInstance);
						}
					}
				}
			}

			private readonly OpenMessageStates states;

			private readonly int documentId;

			private List<OpenMessageInstance> instances;

			private List<ColumnValue> tentativelyOverridenColumns;
		}
	}
}
