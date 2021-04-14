using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class BaseRow
	{
		protected BaseRow(Identity identity, IConfigurable configurationObject)
		{
			this.Identity = identity;
			this.ConfigurationObject = configurationObject;
		}

		protected BaseRow(ADObjectId identity, IConfigurable configurationObject) : this(identity.ToIdentity(), configurationObject)
		{
		}

		protected BaseRow(ADObject configurationObject) : this(configurationObject.ToIdentity(), configurationObject)
		{
		}

		protected BaseRow(IConfigurable configurationObject) : this((ADObjectId)configurationObject.Identity, configurationObject)
		{
		}

		protected BaseRow() : this(null, null)
		{
		}

		public IConfigurable ConfigurationObject { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public Identity Identity { get; private set; }
	}
}
