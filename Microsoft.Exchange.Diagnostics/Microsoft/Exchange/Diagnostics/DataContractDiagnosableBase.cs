using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class DataContractDiagnosableBase<TArgument> : IDiagnosable where TArgument : DiagnosableArgument, new()
	{
		private protected TArgument Arguments { protected get; private set; }

		public TObject Deserialize<TObject>(XmlReader reader)
		{
			XmlObjectSerializer xmlObjectSerializer = this.CreateSerializer(typeof(TObject));
			object obj = xmlObjectSerializer.ReadObject(reader);
			return (TObject)((object)obj);
		}

		public virtual string GetDiagnosticComponentName()
		{
			return base.GetType().Name;
		}

		public virtual XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			this.Arguments = Activator.CreateInstance<TArgument>();
			TArgument arguments = this.Arguments;
			return arguments.RunDiagnosticOperation(delegate
			{
				TArgument arguments2 = this.Arguments;
				arguments2.Initialize(parameters);
				object diagnosticResult = this.GetDiagnosticResult();
				if (diagnosticResult == null)
				{
					XName name = this.GetDiagnosticComponentName();
					TArgument arguments3 = this.Arguments;
					return new XElement(name, new XText(arguments3.GetSupportedArguments()));
				}
				XDocument xdocument = new XDocument();
				using (XmlWriter xmlWriter = xdocument.CreateWriter())
				{
					XmlObjectSerializer xmlObjectSerializer = this.CreateSerializer(diagnosticResult.GetType());
					xmlObjectSerializer.WriteObject(xmlWriter, diagnosticResult);
				}
				return xdocument.Root;
			});
		}

		protected virtual XmlObjectSerializer CreateSerializer(Type type)
		{
			string rootNamespace = type.Namespace ?? string.Empty;
			return new DataContractSerializer(type, type.Name, rootNamespace, Array<Type>.Empty, int.MaxValue, false, true, null, this.CreateDataContractResolver());
		}

		protected virtual DataContractResolver CreateDataContractResolver()
		{
			return null;
		}

		protected abstract object GetDiagnosticResult();
	}
}
