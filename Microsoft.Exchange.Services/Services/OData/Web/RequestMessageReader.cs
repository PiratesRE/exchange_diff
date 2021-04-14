using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.OData.Model;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class RequestMessageReader
	{
		public RequestMessageReader(ODataContext odataContext)
		{
			ArgumentValidator.ThrowIfNull("odataContext", odataContext);
			this.ODataContext = odataContext;
		}

		public ODataContext ODataContext { get; private set; }

		public Entity ReadPostEntity()
		{
			if (this.ODataContext.HttpContext.Request.ContentLength == 0)
			{
				return null;
			}
			Entity rootEntity = null;
			Dictionary<ODataNavigationLink, Entity> navigationEntities = new Dictionary<ODataNavigationLink, Entity>();
			this.HandleReadException(delegate
			{
				using (ODataMessageReader odataMessageReader = this.CreateODataMessageReader())
				{
					ODataReader odataReader = odataMessageReader.CreateODataEntryReader(this.ODataContext.NavigationSource, this.ODataContext.EntityType);
					ODataNavigationLink odataNavigationLink = null;
					while (odataReader.Read())
					{
						switch (odataReader.State)
						{
						case 4:
						{
							ODataEntry entry = (ODataEntry)odataReader.Item;
							Entity entity = this.BuildDataEntityFromEntry(entry);
							if (odataNavigationLink != null)
							{
								navigationEntities.Add(odataNavigationLink, entity);
							}
							else
							{
								rootEntity = entity;
							}
							break;
						}
						case 5:
							odataNavigationLink = (ODataNavigationLink)odataReader.Item;
							break;
						case 6:
							odataNavigationLink = null;
							break;
						}
					}
				}
			});
			this.CompileNavigationProperties(rootEntity, navigationEntities);
			return rootEntity;
		}

		public IDictionary<string, object> ReadPostParameters()
		{
			IDictionary<string, object> parameterCollections = new Dictionary<string, object>();
			if (this.ODataContext.HttpContext.Request.ContentLength == 0)
			{
				return parameterCollections;
			}
			this.HandleReadException(delegate
			{
				OperationSegment operationSegment = (OperationSegment)this.ODataContext.ODataPath.EntitySegment;
				IEdmOperation edmOperation = operationSegment.Operations.First<IEdmOperation>();
				using (ODataMessageReader odataMessageReader = this.CreateODataMessageReader())
				{
					ODataParameterReader odataParameterReader = odataMessageReader.CreateODataParameterReader(edmOperation);
					while (odataParameterReader.Read())
					{
						switch (odataParameterReader.State)
						{
						case 1:
							parameterCollections[odataParameterReader.Name] = odataParameterReader.Value;
							break;
						case 2:
						{
							ODataCollectionReader odataCollectionReader = odataParameterReader.CreateCollectionReader();
							List<ODataValue> list = new List<ODataValue>();
							while (odataCollectionReader.Read())
							{
								if (odataCollectionReader.Item is ODataValue)
								{
									list.Add((ODataValue)odataCollectionReader.Item);
								}
							}
							ODataCollectionValue value = new ODataCollectionValue
							{
								Items = list
							};
							parameterCollections[odataParameterReader.Name] = value;
							break;
						}
						}
					}
				}
			});
			return parameterCollections;
		}

		private Entity BuildDataEntityFromEntry(ODataEntry entry)
		{
			Entity entity = EntityTypeManager.CreateEntityByType(entry.TypeName);
			foreach (ODataProperty odataProperty in entry.Properties)
			{
				PropertyDefinition propertyDefinition = entity.Schema.ResolveProperty(odataProperty.Name);
				entity[propertyDefinition] = ODataObjectModelConverter.ConvertFromPropertyValue(propertyDefinition, odataProperty.Value);
			}
			return entity;
		}

		private void CompileNavigationProperties(Entity rootEntity, Dictionary<ODataNavigationLink, Entity> navigationEntities)
		{
			foreach (KeyValuePair<ODataNavigationLink, Entity> keyValuePair in navigationEntities)
			{
				PropertyDefinition propertyDefinition = rootEntity.Schema.ResolveProperty(keyValuePair.Key.Name);
				rootEntity[propertyDefinition] = keyValuePair.Value;
			}
		}

		private ODataMessageReader CreateODataMessageReader()
		{
			ODataMessageReaderSettings odataMessageReaderSettings = new ODataMessageReaderSettings
			{
				BaseUri = this.ODataContext.HttpContext.GetServiceRootUri()
			};
			RequestMessage requestMessage = new RequestMessage(this.ODataContext.HttpContext);
			return new ODataMessageReader(requestMessage, odataMessageReaderSettings, this.ODataContext.EdmModel);
		}

		private void HandleReadException(Action action)
		{
			try
			{
				action();
			}
			catch (ODataException oDataException)
			{
				throw new RequestBodyReadException(oDataException);
			}
			catch (WebException innerException)
			{
				throw new HttpRequestTransportException(innerException);
			}
			catch (IOException innerException2)
			{
				throw new HttpRequestTransportException(innerException2);
			}
			catch (SocketException innerException3)
			{
				throw new HttpRequestTransportException(innerException3);
			}
		}
	}
}
