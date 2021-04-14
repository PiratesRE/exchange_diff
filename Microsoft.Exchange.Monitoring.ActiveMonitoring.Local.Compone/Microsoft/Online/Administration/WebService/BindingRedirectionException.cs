using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "BindingRedirectionException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	public class BindingRedirectionException : MsolAdministrationException
	{
		[DataMember]
		public string[] Locations
		{
			get
			{
				return this.LocationsField;
			}
			set
			{
				this.LocationsField = value;
			}
		}

		private string[] LocationsField;
	}
}
