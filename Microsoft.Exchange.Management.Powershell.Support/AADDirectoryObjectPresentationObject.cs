using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.WindowsAzure.ActiveDirectory;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public class AADDirectoryObjectPresentationObject : ConfigurableObject
	{
		internal AADDirectoryObjectPresentationObject(DirectoryObject directoryObject) : base(new SimpleProviderPropertyBag())
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2012);
			AADDirectoryObjectPresentationObject[] members;
			if (directoryObject.members == null)
			{
				members = null;
			}
			else
			{
				members = (from member in directoryObject.members
				select AADPresentationObjectFactory.Create(member)).ToArray<AADDirectoryObjectPresentationObject>();
			}
			this.Members = members;
			this.ObjectId = directoryObject.objectId;
			this.ObjectType = directoryObject.objectType;
			AADDirectoryObjectPresentationObject[] owners;
			if (directoryObject.owners == null)
			{
				owners = null;
			}
			else
			{
				owners = (from owner in directoryObject.owners
				select AADPresentationObjectFactory.Create(owner)).ToArray<AADDirectoryObjectPresentationObject>();
			}
			this.Owners = owners;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<AADDirectoryObjectPresentationObjectSchema>();
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ObjectId);
			}
		}

		public AADDirectoryObjectPresentationObject[] Members
		{
			get
			{
				return (AADDirectoryObjectPresentationObject[])this[AADDirectoryObjectPresentationObjectSchema.Members];
			}
			set
			{
				this[AADDirectoryObjectPresentationObjectSchema.Members] = value;
			}
		}

		public string ObjectId
		{
			get
			{
				return (string)this[AADDirectoryObjectPresentationObjectSchema.ObjectId];
			}
			set
			{
				this[AADDirectoryObjectPresentationObjectSchema.ObjectId] = value;
			}
		}

		public string ObjectType
		{
			get
			{
				return (string)this[AADDirectoryObjectPresentationObjectSchema.ObjectType];
			}
			set
			{
				this[AADDirectoryObjectPresentationObjectSchema.ObjectType] = value;
			}
		}

		public AADDirectoryObjectPresentationObject[] Owners
		{
			get
			{
				return (AADDirectoryObjectPresentationObject[])this[AADDirectoryObjectPresentationObjectSchema.Owners];
			}
			set
			{
				this[AADDirectoryObjectPresentationObjectSchema.Owners] = value;
			}
		}

		public override string ToString()
		{
			return this.ObjectId;
		}
	}
}
