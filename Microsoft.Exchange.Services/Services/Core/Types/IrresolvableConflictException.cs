using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class IrresolvableConflictException : ServicePermanentException
	{
		public IrresolvableConflictException(PropertyConflict[] propertyConflicts) : base(CoreResources.IDs.ErrorIrresolvableConflict)
		{
			if (propertyConflicts != null)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<int>((long)this.GetHashCode(), "IrresolvableConflictException constructor called for '{0}' property conflicts.", propertyConflicts.Length);
				foreach (PropertyConflict propertyConflict in propertyConflicts)
				{
					ExTraceGlobals.ExceptionTracer.TraceError((long)this.GetHashCode(), "Property conflict: DisplayName: '{0}', Resolvable: '{1}', OriginalValue: '{2}', ClientValue: '{3}', ServerValue: '{4}'", new object[]
					{
						(propertyConflict.PropertyDefinition != null) ? propertyConflict.PropertyDefinition.Name : ServiceDiagnostics.HandleNullObjectTrace(propertyConflict.PropertyDefinition),
						propertyConflict.ConflictResolvable,
						ServiceDiagnostics.HandleNullObjectTrace(propertyConflict.OriginalValue),
						ServiceDiagnostics.HandleNullObjectTrace(propertyConflict.ClientValue),
						ServiceDiagnostics.HandleNullObjectTrace(propertyConflict.ServerValue)
					});
				}
			}
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}
