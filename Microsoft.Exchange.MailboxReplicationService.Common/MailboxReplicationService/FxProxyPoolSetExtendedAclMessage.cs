using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolSetExtendedAclMessage : DataMessageBase
	{
		public FxProxyPoolSetExtendedAclMessage(AclFlags aclFlags, PropValueData[][] aclData)
		{
			this.aclFlags = aclFlags;
			this.aclData = aclData;
		}

		public FxProxyPoolSetExtendedAclMessage(byte[] blob)
		{
			FxProxyPoolSetExtendedAclMessage.AclRec aclRec = CommonUtils.DataContractDeserialize<FxProxyPoolSetExtendedAclMessage.AclRec>(blob);
			if (aclRec != null)
			{
				this.aclFlags = aclRec.AclFlags;
				this.aclData = aclRec.AclData;
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolSetExtendedAcl
				};
			}
		}

		public AclFlags AclFlags
		{
			get
			{
				return this.aclFlags;
			}
		}

		public PropValueData[][] AclData
		{
			get
			{
				return this.aclData;
			}
		}

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyPoolSetExtendedAclMessage(data);
		}

		protected override int GetSizeInternal()
		{
			int num = 4;
			if (this.AclData != null)
			{
				foreach (PropValueData[] array2 in this.AclData)
				{
					foreach (PropValueData propValueData in array2)
					{
						num += propValueData.GetApproximateSize();
					}
				}
			}
			return num;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolSetExtendedAcl;
			data = CommonUtils.DataContractSerialize<FxProxyPoolSetExtendedAclMessage.AclRec>(new FxProxyPoolSetExtendedAclMessage.AclRec
			{
				AclFlags = this.aclFlags,
				AclData = this.AclData
			});
		}

		private readonly AclFlags aclFlags;

		private readonly PropValueData[][] aclData;

		[DataContract]
		internal sealed class AclRec
		{
			[DataMember(IsRequired = true)]
			public AclFlags AclFlags { get; set; }

			[DataMember(IsRequired = true)]
			public PropValueData[][] AclData { get; set; }
		}
	}
}
