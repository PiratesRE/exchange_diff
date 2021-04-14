using System;
using System.Security.AccessControl;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RemoteDestinationFolder : RemoteFolder, IDestinationFolder, IFolder, IDisposable
	{
		public RemoteDestinationFolder(IMailboxReplicationProxyService mrsProxy, long handle, byte[] folderId, RemoteDestinationMailbox mailbox) : base(mrsProxy, handle, folderId, mailbox)
		{
			this.exportBufferSizeKB = mailbox.ExportBufferSizeKB;
		}

		IFxProxy IDestinationFolder.GetFxProxy(FastTransferFlags flags)
		{
			byte[] data;
			long handle;
			if (base.ServerVersion[30])
			{
				handle = base.MrsProxy.IDestinationFolder_GetFxProxy2(base.Handle, (int)flags, out data);
			}
			else
			{
				handle = base.MrsProxy.IDestinationFolder_GetFxProxy(base.Handle, out data);
			}
			IDataMessage getDataResponseMsg = FxProxyGetObjectDataResponseMessage.Deserialize(DataMessageOpcode.FxProxyGetObjectDataResponse, data, base.MrsProxyClient.UseCompression);
			BufferedTransmitter destination = new BufferedTransmitter(new RemoteDataImport(base.MrsProxy, handle, getDataResponseMsg), this.exportBufferSizeKB, true, base.MrsProxyClient.UseBuffering, base.MrsProxyClient.UseCompression);
			AsynchronousTransmitter destination2 = new AsynchronousTransmitter(destination, true);
			return new FxProxyTransmitter(destination2, true);
		}

		bool IDestinationFolder.SetSearchCriteria(RestrictionData restriction, byte[][] entryIds, SearchCriteriaFlags flags)
		{
			return base.MrsProxy.IDestinationFolder_SetSearchCriteria(base.Handle, restriction, entryIds, (int)flags);
		}

		PropProblemData[] IDestinationFolder.SetSecurityDescriptor(SecurityProp secProp, RawSecurityDescriptor sd)
		{
			byte[] array;
			if (sd != null)
			{
				array = new byte[sd.BinaryLength];
				sd.GetBinaryForm(array, 0);
			}
			else
			{
				array = null;
			}
			return base.MrsProxy.IDestinationFolder_SetSecurityDescriptor(base.Handle, (int)secProp, array);
		}

		void IDestinationFolder.SetReadFlagsOnMessages(SetReadFlags flags, byte[][] entryIds)
		{
			CommonUtils.ProcessInBatches<byte[]>(entryIds, 1000, delegate(byte[][] batch)
			{
				this.MrsProxy.IDestinationFolder_SetReadFlagsOnMessages(this.Handle, (int)flags, batch);
			});
		}

		void IDestinationFolder.SetMessageProps(byte[] entryId, PropValueData[] propValues)
		{
			if (!base.ServerVersion[40])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IDestinationFolder_SetMessageProps");
			}
			base.MrsProxy.IDestinationFolder_SetMessageProps(base.Handle, entryId, propValues);
		}

		void IDestinationFolder.SetRules(RuleData[] rules)
		{
			if (!base.ServerVersion[8])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IDestinationFolder_SetRules");
			}
			base.MrsProxy.IDestinationFolder_SetRules(base.Handle, rules);
		}

		void IDestinationFolder.SetACL(SecurityProp secProp, PropValueData[][] aclData)
		{
			if (!base.ServerVersion[8])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IDestinationFolder_SetACL");
			}
			base.MrsProxy.IDestinationFolder_SetACL(base.Handle, (int)secProp, aclData);
		}

		void IDestinationFolder.SetExtendedAcl(AclFlags aclFlags, PropValueData[][] aclData)
		{
			if (!base.ServerVersion[51])
			{
				throw new UnsupportedRemoteServerVersionWithOperationPermanentException(base.MrsProxyClient.ServerName, base.ServerVersion.ToString(), "IDestinationFolder_SetExtendedAcl");
			}
			base.MrsProxy.IDestinationFolder_SetExtendedAcl(base.Handle, (int)aclFlags, aclData);
		}

		void IDestinationFolder.Flush()
		{
		}

		Guid IDestinationFolder.LinkMailPublicFolder(LinkMailPublicFolderFlags flags, byte[] objectId)
		{
			return base.MrsProxy.IDestinationFolder_LinkMailPublicFolder(base.Handle, flags, objectId);
		}

		private readonly int exportBufferSizeKB;
	}
}
