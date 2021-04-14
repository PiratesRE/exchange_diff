using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class Container : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return Container.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return Container.mostDerivedClass;
			}
		}

		public MultiValuedProperty<byte[]> EdgeSyncCookies
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[ContainerSchema.EdgeSyncCookies];
			}
			internal set
			{
				this[ContainerSchema.EdgeSyncCookies] = value;
			}
		}

		public byte[] EncryptionKey0
		{
			get
			{
				return (byte[])this[ContainerSchema.CanaryData0];
			}
			internal set
			{
				this[ContainerSchema.CanaryData0] = value;
			}
		}

		public byte[] EncryptionKey1
		{
			get
			{
				return (byte[])this[ContainerSchema.CanaryData1];
			}
			internal set
			{
				this[ContainerSchema.CanaryData1] = value;
			}
		}

		public byte[] EncryptionKey2
		{
			get
			{
				return (byte[])this[ContainerSchema.CanaryData2];
			}
			internal set
			{
				this[ContainerSchema.CanaryData2] = value;
			}
		}

		internal Container GetChildContainer(string commonName)
		{
			return base.Session.Read<Container>(base.Id.GetChildId(commonName));
		}

		internal Container GetParentContainer()
		{
			return base.Session.Read<Container>(base.Id.Parent);
		}

		private static ContainerSchema schema = ObjectSchema.GetInstance<ContainerSchema>();

		private static string mostDerivedClass = "msExchContainer";
	}
}
