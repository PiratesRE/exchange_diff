using System;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MapiDestinationFolder : MapiFolder, IDestinationFolder, IFolder, IDisposable
	{
		public void SetExtendedProps(PropTag[] promotedProperties, RestrictionData[] restrictions, SortOrderData[] views, ICSViewData[] icsViews)
		{
			MrsTracer.Provider.Function("MapiDestinationFolder.SetExtendedProps", new object[0]);
			if (promotedProperties != null && promotedProperties.Length > 0)
			{
				ExecutionContext.Create(new DataContext[]
				{
					new OperationDataContext("MapiDestinationFolder.SetPromotedProps", OperationType.None),
					new PropTagsDataContext(promotedProperties)
				}).Execute(delegate
				{
					using (this.Mailbox.RHTracker.Start())
					{
						using (MapiTable contentsTable = this.Folder.GetContentsTable(ContentsTableFlags.DeferredErrors))
						{
							contentsTable.SetColumns(promotedProperties);
							contentsTable.QueryRows(1);
						}
					}
				});
			}
			if (restrictions != null && restrictions.Length > 0)
			{
				MrsTracer.Provider.Debug("Applying restrictions.", new object[0]);
				for (int i = 0; i < restrictions.Length; i++)
				{
					RestrictionData rd = restrictions[i];
					ExecutionContext.Create(new DataContext[]
					{
						new OperationDataContext("MapiDestinationFolder.ApplyRestriction", OperationType.None),
						new RestrictionDataContext(rd)
					}).Execute(delegate
					{
						Restriction native = DataConverter<RestrictionConverter, Restriction, RestrictionData>.GetNative(rd);
						using (this.Mailbox.RHTracker.Start())
						{
							using (MapiTable contentsTable = this.Folder.GetContentsTable(ContentsTableFlags.DeferredErrors))
							{
								using (new SortLCIDContext(this.Folder.MapiStore, rd.LCID))
								{
									contentsTable.Restrict(native);
									contentsTable.QueryRows(1);
								}
							}
						}
					});
				}
			}
			if (views != null && views.Length > 0)
			{
				MrsTracer.Provider.Debug("Applying views.", new object[0]);
				for (int j = 0; j < views.Length; j++)
				{
					SortOrderData sod = views[j];
					ExecutionContext.Create(new DataContext[]
					{
						new OperationDataContext("MapiDestinationFolder.ApplySortOrder", OperationType.None),
						new SortOrderDataContext(sod)
					}).Execute(delegate
					{
						SortOrder native = DataConverter<SortOrderConverter, SortOrder, SortOrderData>.GetNative(sod);
						ContentsTableFlags contentsTableFlags = ContentsTableFlags.DeferredErrors;
						if (sod.FAI)
						{
							contentsTableFlags |= ContentsTableFlags.Associated;
						}
						else if (sod.Conversation)
						{
							contentsTableFlags |= ContentsTableFlags.ShowConversations;
						}
						using (this.Mailbox.RHTracker.Start())
						{
							using (MapiTable contentsTable = this.Folder.GetContentsTable(contentsTableFlags))
							{
								using (new SortLCIDContext(this.Folder.MapiStore, sod.LCID))
								{
									contentsTable.SortTable(native, SortTableFlags.None);
									contentsTable.QueryRows(1);
								}
							}
						}
					});
				}
			}
			if (icsViews != null && icsViews.Length > 0)
			{
				MrsTracer.Provider.Debug("Applying ICS views.", new object[0]);
				for (int k = 0; k < icsViews.Length; k++)
				{
					ICSViewData icsView = icsViews[k];
					ExecutionContext.Create(new DataContext[]
					{
						new OperationDataContext("MapiDestinationFolder.ApplyICSView", OperationType.None),
						new ICSViewDataContext(icsView)
					}).Execute(delegate
					{
						ManifestConfigFlags flags = ManifestConfigFlags.Associated | ManifestConfigFlags.Normal;
						if (icsView.Conversation)
						{
							flags = ManifestConfigFlags.Conversations;
						}
						using (this.Mailbox.RHTracker.Start())
						{
							using (MapiManifest mapiManifest = this.Folder.CreateExportManifest())
							{
								PropTag[] array = Array<PropTag>.Empty;
								if (icsView.CoveringPropertyTags != null)
								{
									array = new PropTag[icsView.CoveringPropertyTags.Length];
									for (int l = 0; l < icsView.CoveringPropertyTags.Length; l++)
									{
										array[l] = (PropTag)icsView.CoveringPropertyTags[l];
									}
								}
								mapiManifest.Configure(flags, null, null, MapiDestinationFolder.DummyManifestContentsCallback.Instance, array);
								while (mapiManifest.Synchronize() != ManifestStatus.Done)
								{
								}
							}
						}
					});
				}
			}
		}

		bool IDestinationFolder.SetSearchCriteria(RestrictionData restriction, byte[][] entryIds, SearchCriteriaFlags flags)
		{
			Restriction native = DataConverter<RestrictionConverter, Restriction, RestrictionData>.GetNative(restriction);
			using (base.Mailbox.RHTracker.Start())
			{
				base.Folder.SetSearchCriteria(native, entryIds, flags);
			}
			return true;
		}

		PropProblemData[] IDestinationFolder.SetSecurityDescriptor(SecurityProp secProp, RawSecurityDescriptor sd)
		{
			if (sd == null || (secProp != SecurityProp.NTSD && base.Mailbox.IsTitanium))
			{
				return null;
			}
			PropProblem[] a;
			using (base.Mailbox.RHTracker.Start())
			{
				a = base.Folder.SetSecurityDescriptor(secProp, sd);
			}
			return DataConverter<PropProblemConverter, PropProblem, PropProblemData>.GetData(a);
		}

		IFxProxy IDestinationFolder.GetFxProxy(FastTransferFlags flags)
		{
			MrsTracer.Provider.Function("MapiDestinationFolder.GetFxProxy", new object[0]);
			IFxProxy result;
			using (base.Mailbox.RHTracker.Start())
			{
				IMapiFxProxy fxProxyCollector = base.Folder.GetFxProxyCollector();
				IFxProxy proxy = new FxProxyBudgetWrapper(fxProxyCollector, true, new Func<IDisposable>(base.Mailbox.RHTracker.Start), new Action<uint>(base.Mailbox.RHTracker.Charge));
				IFxProxy fxProxy = new FxProxyWrapper(proxy, null);
				result = fxProxy;
			}
			return result;
		}

		void IDestinationFolder.SetReadFlagsOnMessages(SetReadFlags flags, byte[][] entryIds)
		{
			MapiUtils.ProcessMapiCallInBatches<byte[]>(entryIds, delegate(byte[][] batch)
			{
				using (this.Mailbox.RHTracker.Start())
				{
					this.Folder.SetReadFlags(flags, batch);
				}
			});
		}

		void IDestinationFolder.SetMessageProps(byte[] entryId, PropValueData[] propValues)
		{
			throw new NotImplementedException();
		}

		void IDestinationFolder.SetRules(RuleData[] rules)
		{
			Rule[] native = DataConverter<RuleConverter, Rule, RuleData>.GetNative(rules);
			using (base.Mailbox.RHTracker.Start())
			{
				base.Folder.SetRules(native);
			}
		}

		void IDestinationFolder.SetACL(SecurityProp secProp, PropValueData[][] aclData)
		{
			if (base.Mailbox.ServerVersion >= Server.E15MinVersion)
			{
				MrsTracer.Provider.Warning("MAPI provider does not support SetACL against E15+ mailboxes", new object[0]);
				return;
			}
			PropValueData[][] acl = ((IFolder)this).GetACL(secProp);
			FolderACL folderACL = new FolderACL(aclData);
			RowEntry[] array = folderACL.UpdateExisting(acl);
			if (array == null)
			{
				MrsTracer.Provider.Debug("Folder ACL is unchanged.", new object[0]);
				return;
			}
			MrsTracer.Provider.Debug("Applying {0} updates to the folder ACL.", new object[]
			{
				array.Length
			});
			ModifyTableFlags modifyTableFlags = ModifyTableFlags.None;
			if (secProp == SecurityProp.FreeBusyNTSD)
			{
				modifyTableFlags |= ModifyTableFlags.FreeBusy;
			}
			using (base.Mailbox.RHTracker.Start())
			{
				using (MapiModifyTable mapiModifyTable = (MapiModifyTable)base.Folder.OpenProperty(PropTag.AclTable, InterfaceIds.IExchangeModifyTable, 0, OpenPropertyFlags.DeferredErrors))
				{
					mapiModifyTable.ModifyTable(modifyTableFlags, array);
				}
			}
		}

		void IDestinationFolder.SetExtendedAcl(AclFlags aclFlags, PropValueData[][] aclData)
		{
			throw new NotImplementedException();
		}

		void IDestinationFolder.Flush()
		{
		}

		Guid IDestinationFolder.LinkMailPublicFolder(LinkMailPublicFolderFlags flags, byte[] objectId)
		{
			return base.Mailbox.LinkMailPublicFolder(base.FolderId, flags, objectId);
		}

		private class DummyManifestContentsCallback : IMapiManifestCallback
		{
			internal static MapiDestinationFolder.DummyManifestContentsCallback Instance
			{
				get
				{
					return MapiDestinationFolder.DummyManifestContentsCallback.instance;
				}
			}

			public ManifestCallbackStatus Change(byte[] entryId, byte[] sourceKey, byte[] changeKey, byte[] changeList, DateTime lastModificationTime, ManifestChangeType changeType, bool associated, PropValue[] props)
			{
				return ManifestCallbackStatus.Stop;
			}

			public ManifestCallbackStatus Delete(byte[] entryId, bool softDelete, bool expiry)
			{
				return ManifestCallbackStatus.Stop;
			}

			public ManifestCallbackStatus ReadUnread(byte[] entryId, bool read)
			{
				return ManifestCallbackStatus.Stop;
			}

			private static MapiDestinationFolder.DummyManifestContentsCallback instance = new MapiDestinationFolder.DummyManifestContentsCallback();
		}
	}
}
