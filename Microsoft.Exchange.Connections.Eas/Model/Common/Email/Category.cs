using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Common.Email
{
	[XmlType(Namespace = "Email", TypeName = "Category")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Category
	{
		[XmlText]
		public string Name { get; set; }
	}
}
