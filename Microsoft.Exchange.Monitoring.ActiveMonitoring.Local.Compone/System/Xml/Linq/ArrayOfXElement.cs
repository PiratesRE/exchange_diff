using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace System.Xml.Linq
{
	[CollectionDataContract(Name = "ArrayOfXElement", Namespace = "http://schemas.datacontract.org/2004/07/System.Xml.Linq", ItemName = "XElement")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class ArrayOfXElement : List<XmlElement>
	{
	}
}
