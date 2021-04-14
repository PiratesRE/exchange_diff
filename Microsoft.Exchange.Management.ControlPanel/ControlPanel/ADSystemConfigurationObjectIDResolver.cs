using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ADSystemConfigurationObjectIDResolver : AdObjectResolver
	{
		private static PropertyDefinition[] Properties { get; set; }

		private ADSystemConfigurationObjectIDResolver()
		{
			ADSystemConfigurationObjectIDResolver.Properties = new PropertyDefinition[]
			{
				ADObjectSchema.Id
			};
		}

		public ADObjectId ResolveObject(ADObjectId objectId)
		{
			if (objectId == null)
			{
				throw new FaultException(new ArgumentNullException("objectId").Message);
			}
			IEnumerable<ADObjectId> enumerable = this.ResolveObjects(new ADObjectId[]
			{
				objectId
			});
			if (enumerable == null)
			{
				return null;
			}
			return enumerable.FirstOrDefault<ADObjectId>();
		}

		public IEnumerable<ADObjectId> ResolveObjects(IEnumerable<ADObjectId> objectIds)
		{
			return from row in base.ResolveObjects<ADObjectId>(objectIds, ADSystemConfigurationObjectIDResolver.Properties, (ADRawEntry e) => e.Id)
			select row;
		}

		internal override IDirectorySession CreateAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, base.TenantSessionSetting, 60, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\Pickers\\ADSystemConfigurationObjectIDResolver.cs");
		}

		internal static readonly ADSystemConfigurationObjectIDResolver Instance = new ADSystemConfigurationObjectIDResolver();
	}
}
