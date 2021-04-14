using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.RequestDispatch;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class RecipientQuery
	{
		protected IRecipientSession ADRecipientSession
		{
			get
			{
				return this.adRecipientSession;
			}
		}

		protected PropertyDefinition[] PropertyDefinitionArray
		{
			get
			{
				return this.propertyDefinitionArray;
			}
		}

		internal static EmailAddress CreateEmailAddressFromADRecipient(ADRecipient adRecipient)
		{
			return new EmailAddress((string)adRecipient[ADRecipientSchema.DisplayName], ((SmtpAddress)adRecipient[ADRecipientSchema.PrimarySmtpAddress]).ToString(), ProxyAddressPrefix.Smtp.PrimaryPrefix);
		}

		internal RecipientQuery(ClientContext clientContext, ADObjectId queryBaseDn, OrganizationId organizationId, DateTime queryPrepareDeadline, PropertyDefinition[] propertyDefinitionArray) : this(clientContext, queryBaseDn, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), queryPrepareDeadline, propertyDefinitionArray)
		{
		}

		internal RecipientQuery(ClientContext clientContext, ADObjectId queryBaseDn, ADSessionSettings adSessionSettings, DateTime queryPrepareDeadline, PropertyDefinition[] propertyDefinitionArray) : this(clientContext, DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, queryBaseDn, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.IgnoreInvalid, null, adSessionSettings, ConfigScopes.TenantSubTree, 113, ".ctor", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\RequestDispatch\\RecipientQuery.cs"), queryPrepareDeadline, propertyDefinitionArray)
		{
		}

		internal RecipientQuery(ClientContext clientContext, IRecipientSession adRecipientSession, DateTime queryPrepareDeadline, PropertyDefinition[] propertyDefinitionArray)
		{
			this.clientContext = clientContext;
			this.queryPrepareDeadline = queryPrepareDeadline;
			this.propertyDefinitionArray = propertyDefinitionArray;
			this.adRecipientSession = adRecipientSession;
		}

		internal RecipientQueryResults Query(EmailAddress[] emailAddressArray)
		{
			return new RecipientQueryResults(this, emailAddressArray);
		}

		internal IEnumerable<RecipientData> LookupRecipientsBatchAtIndex(EmailAddress[] emailAddressArray, int index, out int batchStartIndex)
		{
			int batchSize;
			RecipientQuery.CalculateBatchIndexAndSize(emailAddressArray, index, out batchStartIndex, out batchSize);
			return this.LookupRecipientsBatch(emailAddressArray, batchStartIndex, batchSize);
		}

		private static void CalculateBatchIndexAndSize(EmailAddress[] emailAddressArray, int index, out int batchStartIndex, out int batchSize)
		{
			batchStartIndex = index - index % ADRecipientObjectSession.ReadMultipleMaxBatchSize;
			batchSize = ((batchStartIndex + ADRecipientObjectSession.ReadMultipleMaxBatchSize > emailAddressArray.Length) ? (emailAddressArray.Length - batchStartIndex) : ADRecipientObjectSession.ReadMultipleMaxBatchSize);
		}

		private IEnumerable<RecipientData> LookupRecipientsBatch(EmailAddress[] emailAddressArray, int batchStartIndex, int batchSize)
		{
			TimeSpan timeSpan = this.queryPrepareDeadline - DateTime.UtcNow;
			if (timeSpan <= TimeSpan.Zero)
			{
				return this.HandleErrorFromRecipientLookup(new TimeoutExpiredException("LookupRecipientsBatchBegin"), emailAddressArray, batchStartIndex, batchSize, 53052U);
			}
			List<int> indexList;
			ProxyAddress[] proxyAddressArray = this.GetProxyAddressArray(emailAddressArray, batchStartIndex, batchSize, out indexList);
			IEnumerable<RecipientData> result;
			try
			{
				this.clientContext.CheckOverBudget();
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3815124285U);
				this.adRecipientSession.ServerTimeout = new TimeSpan?(timeSpan);
				if (this.adRecipientSession.SessionSettings != null)
				{
					this.adRecipientSession.SessionSettings.AccountingObject = this.clientContext.Budget;
				}
				result = this.LookupRecipientsBatchInternal(emailAddressArray, batchStartIndex, batchSize, proxyAddressArray, indexList);
			}
			catch (OverBudgetException e)
			{
				result = this.HandleErrorFromRecipientLookup(e, emailAddressArray, batchStartIndex, batchSize, 46908U);
			}
			catch (ArgumentException e2)
			{
				result = this.HandleErrorFromRecipientLookup(e2, emailAddressArray, batchStartIndex, batchSize, 63292U);
			}
			catch (FormatException e3)
			{
				result = this.HandleErrorFromRecipientLookup(e3, emailAddressArray, batchStartIndex, batchSize, 38716U);
			}
			catch (LocalizedException e4)
			{
				result = this.HandleErrorFromRecipientLookup(e4, emailAddressArray, batchStartIndex, batchSize, 55100U);
			}
			return result;
		}

		private ProxyAddress[] GetProxyAddressArray(EmailAddress[] emailAddressArray, int startIndex, int size, out List<int> indexList)
		{
			List<ProxyAddress> list = new List<ProxyAddress>(size);
			indexList = new List<int>(size);
			for (int i = 0; i < size; i++)
			{
				EmailAddress emailAddress = emailAddressArray[startIndex + i];
				if (emailAddress == null)
				{
					ExTraceGlobals.RequestRoutingTracer.TraceError<object, int>((long)this.GetHashCode(), "{0}: Null email address detected at position {1}.", TraceContext.Get(), i);
				}
				else
				{
					ProxyAddress proxyAddress = ProxyAddress.Parse(emailAddress.RoutingType, emailAddress.Address);
					if (proxyAddress is InvalidProxyAddress)
					{
						ExTraceGlobals.RequestRoutingTracer.TraceError<object, EmailAddress>((long)this.GetHashCode(), "{0}: The specified address {1} is not a valid proxy address.", TraceContext.Get(), emailAddress);
					}
					else
					{
						list.Add(proxyAddress);
						indexList.Add(i);
					}
				}
			}
			return list.ToArray();
		}

		private IEnumerable<RecipientData> LookupRecipientsBatchInternal(EmailAddress[] emailAddressArray, int startIndex, int size, ProxyAddress[] proxyAddressArray, List<int> indexList)
		{
			Result<ADRawEntry>[] resultsArray = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				resultsArray = this.adRecipientSession.FindByProxyAddresses(proxyAddressArray, this.propertyDefinitionArray);
			});
			if (adoperationResult.Exception == null)
			{
				return this.ProcessResults(emailAddressArray, startIndex, size, resultsArray, indexList);
			}
			return this.HandleErrorFromRecipientLookup(adoperationResult.Exception, emailAddressArray, startIndex, proxyAddressArray.Length, 42812U);
		}

		private IEnumerable<RecipientData> ProcessResults(EmailAddress[] emailAddressArray, int startIndex, int size, Result<ADRawEntry>[] resultsArray, List<int> indexList)
		{
			for (int index = 0; index < size; index++)
			{
				int resultIndex = indexList.IndexOf(index);
				if (-1 == resultIndex)
				{
					EmailAddress invalidAddress = emailAddressArray[startIndex + index];
					yield return RecipientData.Create(emailAddressArray[startIndex + index], new InvalidSmtpAddressException(Strings.descInvalidSmtpAddress((invalidAddress == null) ? "null" : invalidAddress.ToString())));
				}
				else
				{
					Result<ADRawEntry> result = resultsArray[resultIndex];
					if (result.Error == null)
					{
						yield return RecipientData.Create(emailAddressArray[startIndex + index], result.Data, this.propertyDefinitionArray);
					}
					else
					{
						yield return RecipientData.Create(emailAddressArray[startIndex + index], result.Error);
					}
				}
			}
			yield break;
		}

		private IEnumerable<RecipientData> HandleErrorFromRecipientLookup(Exception e, EmailAddress[] emailAddressArray, int start, int length, uint locationIdentifier)
		{
			LocalizedException exception;
			if (e is LocalizedException)
			{
				exception = (LocalizedException)e;
				ErrorHandler.SetErrorCodeIfNecessary(exception, ErrorConstants.FreeBusyGenerationFailed);
			}
			else if (e is ADTimelimitExceededException)
			{
				exception = new TimeoutExpiredException("LookupRecipients:ADTimeLimit");
			}
			else
			{
				exception = new MailRecipientNotFoundException(e, locationIdentifier);
			}
			ExTraceGlobals.RequestRoutingTracer.TraceError<object, LocalizedException>((long)this.GetHashCode(), "{0}: Failed to lookup recipients in AD. Exception {1}", TraceContext.Get(), exception);
			for (int i = 0; i < length; i++)
			{
				yield return RecipientData.Create(emailAddressArray[start + i], exception);
			}
			yield break;
		}

		private ClientContext clientContext;

		private DateTime queryPrepareDeadline;

		private PropertyDefinition[] propertyDefinitionArray;

		private IRecipientSession adRecipientSession;
	}
}
