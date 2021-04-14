using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class RoleDefinition : BackgroundJobBackendBase
	{
		public Guid RoleId
		{
			get
			{
				return (Guid)this[RoleDefinition.RoleIdProperty];
			}
			set
			{
				this[RoleDefinition.RoleIdProperty] = value;
			}
		}

		public string RoleName
		{
			get
			{
				return (string)this[RoleDefinition.RoleNameProperty];
			}
			set
			{
				this[RoleDefinition.RoleNameProperty] = value;
			}
		}

		public string RoleVersion
		{
			get
			{
				return (string)this[RoleDefinition.RoleVersionProperty];
			}
			set
			{
				this[RoleDefinition.RoleVersionProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = JobDefinitionProperties.RoleIdProperty;

		internal static readonly BackgroundJobBackendPropertyDefinition RoleNameProperty = new BackgroundJobBackendPropertyDefinition("RoleName", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition RoleVersionProperty = new BackgroundJobBackendPropertyDefinition("RoleVersion", typeof(string), PropertyDefinitionFlags.Mandatory, null);
	}
}
