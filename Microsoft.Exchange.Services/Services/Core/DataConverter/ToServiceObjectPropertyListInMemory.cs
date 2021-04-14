using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ToServiceObjectPropertyListInMemory : ToServiceObjectPropertyList
	{
		public ToServiceObjectPropertyListInMemory(Shape shape, ResponseShape responseShape) : base(shape, responseShape, StaticParticipantResolver.DefaultInstance)
		{
		}

		protected override bool IsErrorReturnedForInvalidBaseShapeProperty
		{
			get
			{
				return false;
			}
		}

		protected override bool IsPropertyRequiredInShape
		{
			get
			{
				return false;
			}
		}

		public ServiceObject ConvertStoreObjectPropertiesToServiceObject(StoreObject storeObject, ServiceObject serviceObject)
		{
			return base.ConvertStoreObjectPropertiesToServiceObject(null, storeObject, serviceObject);
		}

		protected override void ConvertPropertyCommandToServiceObject(IToServiceObjectCommand propertyCommand)
		{
			PropertyCommand.ToServiceObjectInMemoryOnly(delegate
			{
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						PropertyCommand propertyCommand2 = propertyCommand as PropertyCommand;
						if (propertyCommand2 != null && propertyCommand2.ToServiceObjectRequiresMailboxAccess)
						{
							return;
						}
						this.<>n__FabricatedMethod5(propertyCommand);
					});
				}
				catch (GrayException ex)
				{
					if (ExTraceGlobals.ServiceCommandBaseDataTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.ServiceCommandBaseDataTracer.TraceError<string, string>((long)this.GetHashCode(), "[ToServiceObjectPropertyListInMemory::ConvertStoreObjectPropertiesToServiceObject] Encountered PropertyRequestFailedException.  Exception: '{0}'. \n Property: {1} IgnoreCorruptPropertiesWhenRendering is true, so processing will continue.", ex.ToString(), propertyCommand.GetType().Name);
					}
				}
			});
		}
	}
}
