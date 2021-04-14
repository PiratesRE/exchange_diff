using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal class SupportShellHelper : MarshalByRefObject
	{
		public KeyValuePair<string, Type>[] GetAllPropertiesAndValueTypes()
		{
			return (from property in ContextPropertySchema.AllProperties
			select new KeyValuePair<string, Type>(property.Name, (property.Type.Assembly.GlobalAssemblyCache || (property.Type.IsEnum && property.Type.Assembly == typeof(SupportShellHelper).Assembly)) ? property.Type : typeof(string))).ToArray<KeyValuePair<string, Type>>();
		}
	}
}
