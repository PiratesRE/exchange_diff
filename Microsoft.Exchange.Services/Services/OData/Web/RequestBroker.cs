using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services.OData.Model;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Metadata;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class RequestBroker : DisposeTrackableBase
	{
		public RequestBroker(HttpContext httpContext)
		{
			this.HttpContext = httpContext;
			this.RequestDetailsLogger = RequestDetailsLogger.Current;
		}

		public HttpContext HttpContext { get; private set; }

		private ODataContext ODataContext { get; set; }

		private RequestDetailsLogger RequestDetailsLogger { get; set; }

		private ServiceModel ServiceModel { get; set; }

		protected override void InternalDispose(bool disposing)
		{
			if (this.ODataContext != null)
			{
				this.ODataContext.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RequestBroker>(this);
		}

		public Task Process()
		{
			Task result;
			try
			{
				this.RequestDetailsLogger.Set(ActivityStandardMetadata.Action, "OData:");
				this.InitializeServiceModel();
				DocumentPublisher documentPublisher = null;
				if (this.IsDocumentRequest(out documentPublisher))
				{
					result = documentPublisher.Publish().ContinueWith(new Action<Task>(this.ExecutionComplete));
				}
				else
				{
					this.RequestDetailsLogger.UpdateLatency(ServiceLatencyMetadata.HttpPipelineLatency, this.RequestDetailsLogger.ActivityScope.TotalMilliseconds);
					this.InitializeODataContext();
					Task<ODataResponse> task = this.SelectOperation(this.ODataContext);
					Task task2 = task.ContinueWith(new Action<Task<ODataResponse>>(this.ExecutionComplete));
					result = task2;
				}
			}
			catch (Exception ex)
			{
				result = this.CreateErrorTask(ex);
			}
			return result;
		}

		private void InitializeServiceModel()
		{
			this.ServiceModel = ServiceModel.Version1Model.Member;
		}

		private void InitializeODataContext()
		{
			try
			{
				Uri uri = this.NormalizeRequestUrl(this.HttpContext.GetRequestUri());
				ODataUriParser odataUriParser = new ODataUriParser(this.ServiceModel.EdmModel, this.HttpContext.GetServiceRootUri(), uri)
				{
					Resolver = new StringAsEnumResolver
					{
						EnableCaseInsensitive = true
					}
				};
				odataUriParser.UrlConventions = ODataUrlConventions.KeyAsSegment;
				ODataPath odataPath = odataUriParser.ParsePath();
				ODataPathWrapper odataPath2 = new ODataPathWrapper(odataPath);
				this.ValidatePath(odataPath2);
				this.ODataContext = new ODataContext(this.HttpContext, uri, this.ServiceModel, odataPath2, odataUriParser);
			}
			catch (ODataException odataException)
			{
				throw new UrlResolutionException(odataException);
			}
		}

		private Uri NormalizeRequestUrl(Uri requestUri)
		{
			UriBuilder uriBuilder = new UriBuilder(requestUri);
			if (!uriBuilder.Path.EndsWith("/"))
			{
				UriBuilder uriBuilder2 = uriBuilder;
				uriBuilder2.Path += "/";
			}
			if (this.HttpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
			{
				IEnumerable<IEdmOperation> enumerable = this.ServiceModel.EdmModel.SchemaElements.OfType<IEdmOperation>();
				foreach (IEdmOperation edmOperation in enumerable)
				{
					string text = "/" + edmOperation.Name + "/";
					if (uriBuilder.Path.EndsWith(text, StringComparison.OrdinalIgnoreCase))
					{
						uriBuilder.Path = uriBuilder.Path.Replace(text, "/" + ExtensionMethods.FullName(edmOperation) + "/");
					}
				}
			}
			return uriBuilder.Uri;
		}

		private void ValidatePath(ODataPathWrapper odataPath)
		{
		}

		private bool IsDocumentRequest(out DocumentPublisher publisher)
		{
			publisher = null;
			string text = this.HttpContext.Request.RawUrl.TrimEnd(new char[]
			{
				'/'
			}).ToLower();
			if (text.EndsWith("/odata"))
			{
				publisher = new ServiceDocumentPublisher(this.HttpContext, this.ServiceModel);
				return true;
			}
			if (text.EndsWith("/odata/$metadata"))
			{
				publisher = new MetadataPublisher(this.HttpContext, this.ServiceModel);
				return true;
			}
			return false;
		}

		private Task<ODataResponse> SelectOperation(ODataContext context)
		{
			OperationSelector operationSelector = new OperationSelector(context);
			ODataRequest odataRequest = operationSelector.SelectOperation();
			odataRequest.ODataContext.RequestDetailsLogger.Set(ActivityStandardMetadata.Action, odataRequest.GetOperationNameForLogging());
			try
			{
				odataRequest.LoadFromHttpRequest();
			}
			catch (HttpRequestTransportException readException)
			{
				throw new RequestBodyReadException(readException);
			}
			ODataPermission.Create(odataRequest).Check();
			return ODataTask.CreateTask(odataRequest);
		}

		private void ExecutionComplete(Task<ODataResponse> operationTask)
		{
			try
			{
				if (operationTask.Exception != null)
				{
					this.RequestDetailsLogger.AppendGenericError("ODataCommandException", operationTask.Exception.ToString());
					this.WriteException(operationTask.Exception);
				}
				else
				{
					ODataResponse result = operationTask.Result;
					result.WriteHttpResponse();
				}
			}
			catch (HttpResponseTransportException ex)
			{
				this.RequestDetailsLogger.AppendGenericError("HttpResponseTransportException", ex.ToString());
			}
			catch (Exception ex2)
			{
				this.RequestDetailsLogger.AppendGenericError("UnknownResponseException", ex2.ToString());
				this.WriteException(ex2);
			}
		}

		private void ExecutionComplete(Task publishTask)
		{
			try
			{
				if (publishTask.Exception != null)
				{
					this.RequestDetailsLogger.AppendGenericError("ODataCommandException", publishTask.Exception.ToString());
					this.WriteException(publishTask.Exception);
				}
			}
			catch (HttpResponseTransportException ex)
			{
				this.RequestDetailsLogger.AppendGenericError("HttpResponseTransportException", ex.ToString());
			}
			catch (Exception ex2)
			{
				this.RequestDetailsLogger.AppendGenericError("UnknownResponseException", ex2.ToString());
				this.WriteException(ex2);
			}
		}

		private void WriteException(Exception exception)
		{
			try
			{
				ResponseMessageWriter responseMessageWriter = new ResponseMessageWriter(this.HttpContext, this.ServiceModel);
				responseMessageWriter.WriteError(exception);
			}
			catch (HttpResponseTransportException ex)
			{
				this.RequestDetailsLogger.AppendGenericError("HttpResponseTransportException", ex.ToString());
			}
		}

		private Task CreateErrorTask(Exception ex)
		{
			return Task.Factory.StartNew(delegate()
			{
				this.ProcessException(ex);
			});
		}

		private void ProcessException(Exception ex)
		{
			this.RequestDetailsLogger.Set(ServiceCommonMetadata.GenericErrors, ex.ToString());
			if (this.ServiceModel == null)
			{
				this.HttpContext.Response.StatusCode = 500;
				this.HttpContext.Response.Output.Write(ex.ToString());
				return;
			}
			this.WriteException(ex);
		}
	}
}
