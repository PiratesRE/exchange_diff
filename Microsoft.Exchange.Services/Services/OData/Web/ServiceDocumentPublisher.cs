using System;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Services.OData.Model;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class ServiceDocumentPublisher : DocumentPublisher
	{
		public ServiceDocumentPublisher(HttpContext httpContext, ServiceModel serviceModel) : base(httpContext, serviceModel)
		{
		}

		protected override void WriteDocument(ODataMessageWriter odataWriter)
		{
			odataWriter.WriteServiceDocument(this.GenerateSerivceDocument());
		}

		private ODataServiceDocument GenerateSerivceDocument()
		{
			ODataServiceDocument odataServiceDocument = new ODataServiceDocument();
			odataServiceDocument.EntitySets = (from entitySet in ExtensionMethods.EntitySets(base.ServiceModel.EdmModel.EntityContainer)
			select new ODataEntitySetInfo
			{
				Name = entitySet.Name,
				Title = entitySet.Name,
				Url = new Uri(entitySet.Name, UriKind.RelativeOrAbsolute)
			}).ToList<ODataEntitySetInfo>();
			odataServiceDocument.Singletons = (from singleton in ExtensionMethods.Singletons(base.ServiceModel.EdmModel.EntityContainer)
			select new ODataSingletonInfo
			{
				Name = singleton.Name,
				Title = singleton.Name,
				Url = new Uri(singleton.Name, UriKind.RelativeOrAbsolute)
			}).ToList<ODataSingletonInfo>();
			odataServiceDocument.FunctionImports = (from functionImport in ExtensionMethods.OperationImports(base.ServiceModel.EdmModel.EntityContainer).OfType<IEdmFunctionImport>()
			where functionImport.IncludeInServiceDocument
			select new ODataFunctionImportInfo
			{
				Name = functionImport.Name,
				Title = functionImport.Name,
				Url = new Uri(functionImport.Name, UriKind.RelativeOrAbsolute)
			}).ToList<ODataFunctionImportInfo>();
			return odataServiceDocument;
		}
	}
}
