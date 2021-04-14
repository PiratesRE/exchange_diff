using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services;
using System.Data.Services.Providers;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Library.Annotations;
using Microsoft.Data.Edm.Library.Values;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	[DiagnosticsBehavior]
	[ReportingBehavior]
	internal class ReportingService : DataService<IReportingDataSource>, IServiceProvider
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", 3);
			config.UseVerboseErrors = Global.GetAppSettingAsBool("IncludeVerboseErrorsInReponse", false);
			config.DataServiceBehavior.MaxProtocolVersion = 2;
			int num;
			if (!int.TryParse(ConfigurationManager.AppSettings["EntitysetPageSize"], out num))
			{
				num = 1000;
			}
			config.SetEntitySetPageSize("*", num);
			config.AnnotationsBuilder = new Func<IEdmModel, IEnumerable<IEdmModel>>(ReportingService.BuildAnnotations);
			bool flag;
			bool.TryParse(ConfigurationManager.AppSettings[ReportingService.AppSettingEnableRwsVersionZeroKey], out flag);
			if (flag)
			{
				ReportingVersion.EnableVersionZero();
			}
		}

		public object GetService(Type serviceType)
		{
			object provider = null;
			if (serviceType == typeof(IDataServiceMetadataProvider))
			{
				ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.GetMetadataProviderLatency, delegate
				{
					provider = this.GetMetadataProvider();
				});
			}
			if (serviceType == typeof(IDataServiceQueryProvider))
			{
				ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.GetQueryProviderLatency, delegate
				{
					provider = this.GetQueryProvider(this.metadataProvider);
				});
			}
			return provider;
		}

		protected override void HandleException(HandleExceptionArgs args)
		{
			base.HandleException(args);
			ServiceDiagnostics.ReportUnhandledException(args.Exception, HttpContext.Current);
		}

		private static IEnumerable<IEdmModel> BuildAnnotations(IEdmModel model)
		{
			IEdmEntityContainer edmEntityContainer = ExtensionMethods.EntityContainers(model).SingleOrDefault((IEdmEntityContainer ec) => ec.Name == "TenantReportingWebService");
			EdmModel edmModel = new EdmModel();
			edmModel.AddReferencedModel(model);
			ReportingSchema reportingSchema = ReportingService.GetReportingSchema();
			foreach (IEntity entity in reportingSchema.Entities.Values)
			{
				IEdmEntitySet edmEntitySet = edmEntityContainer.FindEntitySet(entity.Name);
				if (edmEntitySet != null)
				{
					ReportingService.AddAnnotation(edmModel, edmEntitySet, "ReportTitle", entity.Annotation.ReportTitle);
					if (entity.Annotation.Xaxis != null)
					{
						string value = string.Join(",", entity.Annotation.Xaxis);
						if (!string.IsNullOrEmpty(value))
						{
							ReportingService.AddAnnotation(edmModel, edmEntitySet, "X-Axis", value);
						}
					}
					ReportingService.AddAnnotation(edmModel, edmEntitySet, "Y-Axis", string.Join(",", entity.Annotation.Yaxis));
				}
			}
			return new IEdmModel[]
			{
				edmModel
			};
		}

		private static void AddAnnotation(EdmModel annotationModel, IEdmEntitySet entitySet, string name, string value)
		{
			EdmValueTerm edmValueTerm = new EdmValueTerm("TenantReporting", name, 14);
			EdmValueAnnotation edmValueAnnotation = new EdmValueAnnotation(entitySet, edmValueTerm, new EdmStringConstant(value));
			annotationModel.AddVocabularyAnnotation(edmValueAnnotation);
		}

		private static ReportingSchema GetReportingSchema()
		{
			HttpContext httpContext = HttpContext.Current;
			string currentReportingVersion = ReportingVersion.GetCurrentReportingVersion(httpContext);
			return ReportingSchema.GetReportingSchema(currentReportingVersion);
		}

		private IDataServiceMetadataProvider GetMetadataProvider()
		{
			this.metadataProvider = (ReportingMetadataProvider)HttpRuntime.Cache[this.GetCacheKey()];
			if (this.metadataProvider != null)
			{
				return this.metadataProvider;
			}
			ReportingSchema reportingSchema = ReportingService.GetReportingSchema();
			this.metadataProvider = new ReportingMetadataProvider(reportingSchema);
			HttpRuntime.Cache.Insert(this.GetCacheKey(), this.metadataProvider, null, (DateTime)ExDateTime.UtcNow.Add(ReportingService.MetadataProviderCacheMaxAge), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
			return this.metadataProvider;
		}

		private IDataServiceQueryProvider GetQueryProvider(IDataServiceMetadataProvider metadata)
		{
			if (this.queryProvider == null)
			{
				ReportingSchema reportingSchema = ReportingService.GetReportingSchema();
				this.queryProvider = new ReportingQueryProvider(metadata, reportingSchema);
			}
			return this.queryProvider;
		}

		private string GetCacheKey()
		{
			HttpContext httpContext = HttpContext.Current;
			string currentReportingVersion = ReportingVersion.GetCurrentReportingVersion(httpContext);
			return string.Format(CultureInfo.InvariantCulture, "Exchange_Reporting_Metadata_{0}_{1}", new object[]
			{
				RbacPrincipal.Current.CacheKeys[0],
				currentReportingVersion
			});
		}

		private const string AppSettingIncludeVerboseErrorsInReponse = "IncludeVerboseErrorsInReponse";

		private const int DefaultPageSize = 1000;

		private const string AppSettingEntitysetPageSize = "EntitysetPageSize";

		public static readonly string AppSettingEnableRwsVersionZeroKey = "EnableRwsVersionZero";

		private static readonly TimeSpan MetadataProviderCacheMaxAge = new TimeSpan(0, 15, 0);

		private IDataServiceMetadataProvider metadataProvider;

		private IDataServiceQueryProvider queryProvider;
	}
}
