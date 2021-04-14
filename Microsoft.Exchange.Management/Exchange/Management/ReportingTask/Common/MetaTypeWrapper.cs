using System;
using System.Collections.ObjectModel;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class MetaTypeWrapper : MetaType
	{
		public MetaTypeWrapper(MetaModelWrapper metaModelWrapper, MetaType metaType, MetaTableWrapper metaTableWrapper)
		{
			this.metaModelWrapper = metaModelWrapper;
			this.innerMetaType = metaType;
			this.metaTableWrapper = metaTableWrapper;
		}

		public override MetaType GetInheritanceType(Type type)
		{
			return this.innerMetaType.GetInheritanceType(type);
		}

		public override MetaType GetTypeForInheritanceCode(object code)
		{
			return this.innerMetaType.GetTypeForInheritanceCode(code);
		}

		public override MetaDataMember GetDataMember(MemberInfo member)
		{
			return this.innerMetaType.GetDataMember(member);
		}

		public override MetaModel Model
		{
			get
			{
				return this.metaModelWrapper;
			}
		}

		public override MetaTable Table
		{
			get
			{
				return this.metaTableWrapper;
			}
		}

		public override Type Type
		{
			get
			{
				return this.innerMetaType.Type;
			}
		}

		public override string Name
		{
			get
			{
				return this.innerMetaType.Name;
			}
		}

		public override bool IsEntity
		{
			get
			{
				return this.innerMetaType.IsEntity;
			}
		}

		public override bool CanInstantiate
		{
			get
			{
				return this.innerMetaType.CanInstantiate;
			}
		}

		public override MetaDataMember DBGeneratedIdentityMember
		{
			get
			{
				return this.innerMetaType.DBGeneratedIdentityMember;
			}
		}

		public override MetaDataMember VersionMember
		{
			get
			{
				return this.innerMetaType.VersionMember;
			}
		}

		public override MetaDataMember Discriminator
		{
			get
			{
				return this.innerMetaType.Discriminator;
			}
		}

		public override bool HasUpdateCheck
		{
			get
			{
				return this.innerMetaType.HasUpdateCheck;
			}
		}

		public override bool HasInheritance
		{
			get
			{
				return this.innerMetaType.HasInheritance;
			}
		}

		public override bool HasInheritanceCode
		{
			get
			{
				return this.innerMetaType.HasInheritanceCode;
			}
		}

		public override object InheritanceCode
		{
			get
			{
				return this.innerMetaType.InheritanceCode;
			}
		}

		public override bool IsInheritanceDefault
		{
			get
			{
				return this.innerMetaType.IsInheritanceDefault;
			}
		}

		public override MetaType InheritanceRoot
		{
			get
			{
				return this.innerMetaType.InheritanceRoot;
			}
		}

		public override MetaType InheritanceBase
		{
			get
			{
				return this.innerMetaType.InheritanceBase;
			}
		}

		public override MetaType InheritanceDefault
		{
			get
			{
				return this.innerMetaType.InheritanceDefault;
			}
		}

		public override ReadOnlyCollection<MetaType> InheritanceTypes
		{
			get
			{
				return this.innerMetaType.InheritanceTypes;
			}
		}

		public override bool HasAnyLoadMethod
		{
			get
			{
				return this.innerMetaType.HasAnyLoadMethod;
			}
		}

		public override bool HasAnyValidateMethod
		{
			get
			{
				return this.innerMetaType.HasAnyValidateMethod;
			}
		}

		public override ReadOnlyCollection<MetaType> DerivedTypes
		{
			get
			{
				return this.innerMetaType.DerivedTypes;
			}
		}

		public override ReadOnlyCollection<MetaDataMember> DataMembers
		{
			get
			{
				return this.innerMetaType.DataMembers;
			}
		}

		public override ReadOnlyCollection<MetaDataMember> PersistentDataMembers
		{
			get
			{
				return this.innerMetaType.PersistentDataMembers;
			}
		}

		public override ReadOnlyCollection<MetaDataMember> IdentityMembers
		{
			get
			{
				return this.innerMetaType.IdentityMembers;
			}
		}

		public override ReadOnlyCollection<MetaAssociation> Associations
		{
			get
			{
				return this.innerMetaType.Associations;
			}
		}

		public override MethodInfo OnLoadedMethod
		{
			get
			{
				return this.innerMetaType.OnLoadedMethod;
			}
		}

		public override MethodInfo OnValidateMethod
		{
			get
			{
				return this.innerMetaType.OnValidateMethod;
			}
		}

		private MetaType innerMetaType;

		private MetaModelWrapper metaModelWrapper;

		private MetaTableWrapper metaTableWrapper;
	}
}
