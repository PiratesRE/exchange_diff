using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class SetObjectWithIdentityTaskBase<TIdentity, TPublicObject, TDataObject> : SetObjectTaskBase<TPublicObject, TDataObject> where TIdentity : IIdentityParameter, new() where TPublicObject : IConfigurable, new() where TDataObject : IConfigurable, new()
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public virtual TIdentity Identity
		{
			get
			{
				return (TIdentity)((object)base.Fields["Identity"]);
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override void InternalValidate()
		{
			if (this.Identity == null && base.ParameterSetName == "Identity")
			{
				base.WriteError(new ArgumentNullException("Identity"), (ErrorCategory)1000, null);
			}
			base.InternalValidate();
		}

		protected override IConfigurable ResolveDataObject()
		{
			return base.GetDataObject<TDataObject>(this.Identity, base.DataSession, this.RootId, base.OptionalIdentityData, null, null);
		}
	}
}
