using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Core.DocumentModel;

namespace Microsoft.Exchange.Inference.Common
{
	internal class Document : IDocument, IPropertyBag, IReadOnlyPropertyBag
	{
		public Document(IIdentity identity, DocumentOperation operation, IDocumentAdapter documentAdapter, bool initializeNestedDocuments) : this(identity, operation, documentAdapter)
		{
			if (initializeNestedDocuments)
			{
				this.internalNestedDocuments = new List<IDocument>();
			}
		}

		public Document(IIdentity identity, DocumentOperation operation, IDocumentAdapter documentAdapter)
		{
			Util.ThrowOnNullArgument(identity, "identity");
			this.propertyBag = new PropertyBag();
			this.documentAdapter = documentAdapter;
			this.propertyBag.SetProperty<IIdentity>(DocumentSchema.Identity, identity);
			this.propertyBag.SetProperty<DocumentOperation>(DocumentSchema.Operation, operation);
			this.diagnosticsSession = DiagnosticsSession.CreateDocumentDiagnosticsSession(identity, ExTraceGlobals.CoreDocumentModelTracer);
		}

		public IIdentity Identity
		{
			get
			{
				return this.GetProperty<IIdentity>(DocumentSchema.Identity);
			}
		}

		public DocumentOperation Operation
		{
			get
			{
				return this.GetProperty<DocumentOperation>(DocumentSchema.Operation);
			}
		}

		public IIdentity ParentIdentity
		{
			get
			{
				object obj = null;
				this.TryGetProperty(DocumentSchema.ParentIdentity, out obj);
				return (IIdentity)obj;
			}
		}

		public ReadOnlyCollection<IDocument> NestedDocuments
		{
			get
			{
				if (this.internalNestedDocuments == null)
				{
					return null;
				}
				if (this.nestedDocuments == null)
				{
					this.nestedDocuments = new ReadOnlyCollection<IDocument>(this.internalNestedDocuments);
				}
				return this.nestedDocuments;
			}
		}

		public ICollection<DocumentFailureDescription> Failures
		{
			get
			{
				return this.failures;
			}
		}

		protected IDiagnosticsSession DiagnosticSession
		{
			get
			{
				return this.diagnosticsSession;
			}
		}

		protected IDocumentAdapter DocumentAdapter
		{
			get
			{
				return this.documentAdapter;
			}
			set
			{
				this.documentAdapter = value;
			}
		}

		public TPropertyValue GetProperty<TPropertyValue>(PropertyDefinition property)
		{
			object obj;
			if (!this.TryGetProperty(property, out obj))
			{
				throw new PropertyErrorException(property.Name);
			}
			if (!typeof(TPropertyValue).IsAssignableFrom(obj.GetType()))
			{
				throw new PropertyTypeErrorException(property.Name);
			}
			return (TPropertyValue)((object)obj);
		}

		public virtual bool TryGetProperty(PropertyDefinition property, out object value)
		{
			this.DiagnosticSession.TraceDebug<PropertyDefinition>("DocumentModel - GetProperty: {0}", property);
			bool result;
			if (this.documentAdapter != null && this.documentAdapter.ContainsPropertyMapping(property))
			{
				this.DiagnosticSession.TraceDebug<PropertyDefinition>("DocumentModel - Getting property {0} from adapter", property);
				result = this.documentAdapter.TryGetProperty(property, out value);
			}
			else
			{
				this.DiagnosticSession.TraceDebug<PropertyDefinition>("DocumentModel - Getting property {0} from property bag", property);
				result = this.propertyBag.TryGetProperty(property, out value);
			}
			this.DiagnosticSession.TraceDebug<PropertyDefinition, object>("DocumentModel - GotProperty: {0},{1}", property, value);
			return result;
		}

		public virtual void SetProperty(PropertyDefinition property, object value)
		{
			this.DiagnosticSession.TraceDebug<PropertyDefinition, object>("DocumentModel - SetProperty: {0}, {1}", property, value);
			if (this.documentAdapter != null && this.documentAdapter.ContainsPropertyMapping(property))
			{
				this.DiagnosticSession.TraceDebug<PropertyDefinition>("DocumentModel - Setting property {0} through the adapter", property);
				this.documentAdapter.SetProperty(property, value);
				return;
			}
			if (this.DiagnosticSession.IsTraceEnabled(TraceType.DebugTrace))
			{
				object arg = null;
				this.TryGetProperty(property, out arg);
				this.DiagnosticSession.TraceDebug<PropertyDefinition, object>("DocumentModel - SetProperty: {0}, original value {1}:", property, arg);
			}
			this.DiagnosticSession.TraceDebug<PropertyDefinition>("DocumentModel - Setting property {0} onto the property bag", property);
			this.propertyBag.SetProperty(property, value);
		}

		public void AddDocument(IDocument document)
		{
			Util.ThrowOnNullArgument(document, "document");
			if (this.internalNestedDocuments == null)
			{
				this.internalNestedDocuments = new List<IDocument>();
			}
			this.internalNestedDocuments.Add(document);
		}

		public int RemoveDocuments(ICollection<IDocument> documentsToRemove)
		{
			Util.ThrowOnNullOrEmptyArgument<IDocument>(documentsToRemove, "documentsToRemove");
			if (this.internalNestedDocuments == null)
			{
				return 0;
			}
			return this.internalNestedDocuments.RemoveAll(new Predicate<IDocument>(documentsToRemove.Contains));
		}

		public void CopyPropertiesTo(Document document)
		{
			Util.ThrowOnNullArgument(document, "document");
			foreach (KeyValuePair<PropertyDefinition, object> keyValuePair in this.propertyBag.Values)
			{
				document.SetProperty(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private void SetProperty<TPropertyValue>(PropertyDefinition property, TPropertyValue value)
		{
			this.propertyBag.SetProperty<TPropertyValue>(property, value);
		}

		private bool TryGetProperty<TPropertyValue>(PropertyDefinition property, out object value)
		{
			bool flag = this.TryGetProperty(property, out value);
			if (flag)
			{
				Util.ThrowOnMismatchType<TPropertyValue>(value, property.Name);
			}
			return flag;
		}

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly List<DocumentFailureDescription> failures = new List<DocumentFailureDescription>();

		private PropertyBag propertyBag;

		private IDocumentAdapter documentAdapter;

		private ReadOnlyCollection<IDocument> nestedDocuments;

		private List<IDocument> internalNestedDocuments;
	}
}
