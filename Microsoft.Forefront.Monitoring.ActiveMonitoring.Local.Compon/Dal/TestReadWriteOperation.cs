using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class TestReadWriteOperation : DalProbeOperation
	{
		[XmlAttribute]
		public DalType Database { get; set; }

		[XmlAttribute]
		public string DataType { get; set; }

		[XmlAttribute]
		public string OrganizationTag { get; set; }

		public XElement DataObject { get; set; }

		[XmlAttribute]
		public string QueryString { get; set; }

		[XmlAttribute]
		public string RootId { get; set; }

		[XmlAttribute]
		public string CheckProperties { get; set; }

		[XmlAttribute]
		public bool MatchAllProperties { get; set; }

		[XmlAttribute]
		public bool MatchAllObjects { get; set; }

		public override void Execute(IDictionary<string, object> variables)
		{
			ADObjectDeserializerOperation adobjectDeserializerOperation = new ADObjectDeserializerOperation
			{
				Type = this.DataType,
				DataObject = this.DataObject,
				Return = "$saved"
			};
			adobjectDeserializerOperation.Execute(variables);
			SaveOperation saveOperation = new SaveOperation
			{
				Object = "$saved",
				DataType = this.DataType,
				Database = this.Database,
				OrganizationTag = this.OrganizationTag
			};
			saveOperation.Execute(variables);
			FindOperation findOperation = new FindOperation
			{
				QueryString = this.QueryString,
				RootId = this.RootId,
				DataType = this.DataType,
				Database = this.Database,
				OrganizationTag = this.OrganizationTag,
				Return = "$found"
			};
			findOperation.Execute(variables);
			object value = DalProbeOperation.GetValue("$found", variables);
			IEnumerable foundObjs = this.ValidateObject(variables, value);
			IConfigurable configurable = (IConfigurable)DalProbeOperation.GetValue("$saved", variables);
			this.ValidateProperties(configurable, foundObjs);
			DeleteOperation deleteOperation = new DeleteOperation
			{
				Id = ((ADObjectId)configurable.Identity).ObjectGuid,
				DataType = this.DataType,
				Database = this.Database,
				OrganizationTag = this.OrganizationTag
			};
			deleteOperation.Execute(variables);
		}

		private IEnumerable ValidateObject(IDictionary<string, object> variables, object foundObj)
		{
			if (foundObj == null)
			{
				throw new DataValidationException(new ObjectValidationError(new LocalizedString(string.Format("Error reading with queryFilter {0}", this.QueryString)), ((IConfigurable)DalProbeOperation.GetValue("$saved", variables)).Identity, this.Database.ToString()));
			}
			IEnumerable enumerable = foundObj as IEnumerable;
			if (enumerable == null)
			{
				enumerable = new object[]
				{
					foundObj
				};
			}
			return enumerable;
		}

		private void ValidateProperties(object savedObj, IEnumerable foundObjs)
		{
			if (string.IsNullOrWhiteSpace(this.CheckProperties))
			{
				return;
			}
			string[] array = this.CheckProperties.Split(new char[]
			{
				' '
			});
			ValidationError validationError = null;
			foreach (object value in foundObjs)
			{
				validationError = null;
				foreach (string text in array)
				{
					object propertyValue = DalProbeOperation.GetPropertyValue(savedObj, text.Split(new char[]
					{
						'.'
					}), 0);
					object propertyValue2 = DalProbeOperation.GetPropertyValue(value, text.Split(new char[]
					{
						'.'
					}), 0);
					if (object.Equals(propertyValue, propertyValue2))
					{
						if (!this.MatchAllProperties)
						{
							validationError = null;
							break;
						}
					}
					else
					{
						validationError = new ObjectValidationError(new LocalizedString(string.Format("Error comparing property {0}. Value saved is {1}. Value read is {2}", text, propertyValue, propertyValue2)), ((IConfigurable)savedObj).Identity, this.Database.ToString());
						if (this.MatchAllProperties)
						{
							break;
						}
					}
				}
				if (validationError == null && !this.MatchAllObjects)
				{
					return;
				}
				if (validationError != null && this.MatchAllObjects)
				{
					break;
				}
			}
			if (validationError != null)
			{
				throw new DataValidationException(validationError);
			}
		}

		private const string SavedVar = "$saved";

		private const string FoundVar = "$found";
	}
}
