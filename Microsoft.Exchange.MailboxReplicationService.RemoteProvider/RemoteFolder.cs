using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class RemoteFolder : RemoteObject, IFolder, IDisposable
	{
		protected RemoteFolder(IMailboxReplicationProxyService mrsProxy, long handle, byte[] folderId, RemoteMailbox mailbox) : base(mrsProxy, handle)
		{
			this.folderId = folderId;
			this.mailbox = mailbox;
		}

		public byte[] FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public RemoteMailbox Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		private protected string FolderName { protected get; private set; }

		byte[] IFolder.GetFolderId()
		{
			MrsTracer.ProxyClient.Function("IFolder.GetFolderId(): {0}", new object[]
			{
				this.FolderName
			});
			return this.FolderId;
		}

		List<MessageRec> IFolder.EnumerateMessages(EnumerateMessagesFlags emFlags, PropTag[] additionalPtagsToLoad)
		{
			MrsTracer.ProxyClient.Function("IFolder.EnumerateMessages(): {0}", new object[]
			{
				this.FolderName
			});
			bool flag;
			List<MessageRec> list = base.MrsProxy.IFolder_EnumerateMessagesPaged2(base.Handle, emFlags, DataConverter<PropTagConverter, PropTag, int>.GetData(additionalPtagsToLoad), out flag);
			while (flag)
			{
				List<MessageRec> collection = base.MrsProxy.IFolder_EnumerateMessagesNextBatch(base.Handle, out flag);
				list.AddRange(collection);
			}
			foreach (MessageRec messageRec in list)
			{
				messageRec.FolderId = this.folderId;
			}
			MrsTracer.ProxyClient.Debug("IFolder.EnumerateMessages(): {0} returned {1} messages", new object[]
			{
				this.FolderName,
				list.Count
			});
			return list;
		}

		FolderRec IFolder.GetFolderRec(PropTag[] additionalPtagsToLoad, GetFolderRecFlags flags)
		{
			if (!base.ServerVersion[15])
			{
				return base.MrsProxy.IFolder_GetFolderRec2(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(additionalPtagsToLoad));
			}
			FolderRec folderRec = base.MrsProxy.IFolder_GetFolderRec3(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(additionalPtagsToLoad), (int)flags);
			this.FolderName = folderRec.FolderName;
			MrsTracer.ProxyClient.Function("IFolder.GetFolderRec(): {0}", new object[]
			{
				this.FolderName
			});
			return folderRec;
		}

		RawSecurityDescriptor IFolder.GetSecurityDescriptor(SecurityProp secProp)
		{
			MrsTracer.ProxyClient.Function("IFolder.GetSecurityDescriptor(): {0}", new object[]
			{
				this.FolderName
			});
			byte[] array = base.MrsProxy.IFolder_GetSecurityDescriptor(base.Handle, (int)secProp);
			if (array == null)
			{
				return null;
			}
			return new RawSecurityDescriptor(array, 0);
		}

		void IFolder.SetContentsRestriction(RestrictionData restriction)
		{
			MrsTracer.ProxyClient.Function("IFolder.SetContentsRestriction(): {0}", new object[]
			{
				this.FolderName
			});
			if (!base.ServerVersion[8])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IFolder_SetContentsRestriction");
			}
			base.MrsProxy.IFolder_SetContentsRestriction(base.Handle, restriction);
		}

		PropValueData[] IFolder.GetProps(PropTag[] pta)
		{
			MrsTracer.ProxyClient.Function("IFolder.GetProps(): {0}", new object[]
			{
				this.FolderName
			});
			if (base.ServerVersion[8])
			{
				return base.MrsProxy.IFolder_GetProps(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(pta));
			}
			if (this is RemoteSourceFolder)
			{
				return base.MrsProxy.ISourceFolder_GetProps(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(pta));
			}
			throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IFolder_GetProps");
		}

		void IFolder.GetSearchCriteria(out RestrictionData restriction, out byte[][] entryIds, out SearchState state)
		{
			MrsTracer.ProxyClient.Function("IFolder.GetSearchCriteria(): {0}", new object[]
			{
				this.FolderName
			});
			if (base.ServerVersion[8])
			{
				int num;
				base.MrsProxy.IFolder_GetSearchCriteria(base.Handle, out restriction, out entryIds, out num);
				state = (SearchState)num;
				return;
			}
			if (this is RemoteSourceFolder)
			{
				int num;
				base.MrsProxy.ISourceFolder_GetSearchCriteria(base.Handle, out restriction, out entryIds, out num);
				state = (SearchState)num;
				return;
			}
			throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IFolder_GetProps");
		}

		void IFolder.DeleteMessages(byte[][] entryIds)
		{
			MrsTracer.ProxyClient.Function("IFolder.DeleteMessages(): {0}", new object[]
			{
				this.FolderName
			});
			if (base.ServerVersion[8])
			{
				CommonUtils.ProcessInBatches<byte[]>(entryIds, 1000, delegate(byte[][] batch)
				{
					base.MrsProxy.IFolder_DeleteMessages(base.Handle, batch);
				});
				return;
			}
			if (this is RemoteDestinationFolder)
			{
				CommonUtils.ProcessInBatches<byte[]>(entryIds, 1000, delegate(byte[][] batch)
				{
					base.MrsProxy.IDestinationFolder_DeleteMessages(base.Handle, batch);
				});
				return;
			}
			throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IFolder_DeleteMessages");
		}

		RuleData[] IFolder.GetRules(PropTag[] extraProps)
		{
			MrsTracer.ProxyClient.Function("IFolder.GetRules(): {0}", new object[]
			{
				this.FolderName
			});
			if (!base.ServerVersion[8])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IFolder_GetRules");
			}
			return base.MrsProxy.IFolder_GetRules(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(extraProps));
		}

		PropValueData[][] IFolder.GetACL(SecurityProp secProp)
		{
			MrsTracer.ProxyClient.Function("IFolder.GetACL(): {0}", new object[]
			{
				this.FolderName
			});
			if (!base.ServerVersion[8])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IFolder_GetACL");
			}
			return base.MrsProxy.IFolder_GetACL(base.Handle, (int)secProp);
		}

		PropValueData[][] IFolder.GetExtendedAcl(AclFlags aclFlags)
		{
			MrsTracer.ProxyClient.Function("IFolder.GetExtendedAcl(): {0}", new object[]
			{
				this.FolderName
			});
			if (!base.ServerVersion[51])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IFolder_GetExtendedAcl");
			}
			return base.MrsProxy.IFolder_GetExtendedAcl(base.Handle, (int)aclFlags);
		}

		List<MessageRec> IFolder.LookupMessages(PropTag ptagToLookup, List<byte[]> keysToLookup, PropTag[] additionalPtagsToLoad)
		{
			MrsTracer.ProxyClient.Function("IFolder.LookupMessages(): {0}", new object[]
			{
				this.FolderName
			});
			if (!base.ServerVersion[16])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IFolder_LookupMessages");
			}
			return base.MrsProxy.IFolder_LookupMessages(base.Handle, DataConverter<PropTagConverter, PropTag, int>.GetData(ptagToLookup), keysToLookup.ToArray(), DataConverter<PropTagConverter, PropTag, int>.GetData(additionalPtagsToLoad));
		}

		PropProblemData[] IFolder.SetProps(PropValueData[] pva)
		{
			MrsTracer.ProxyClient.Function("IFolder.SetProps(): {0}", new object[]
			{
				this.FolderName
			});
			return base.MrsProxy.IFolder_SetProps(base.Handle, pva);
		}

		protected const int BatchSize = 1000;

		private byte[] folderId;

		private RemoteMailbox mailbox;
	}
}
