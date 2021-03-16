// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using FluentAssertions;
using FluentAssertions.Common;
using NUnit.Framework;

namespace Azure.Iot.TimeSeriesInsights.Tests
{
    public class TimeSeriesInsightsTypesTests : E2eTestBase
    {
        private static readonly TimeSpan s_retryDelay = TimeSpan.FromSeconds(10);

        // This is the GUID that TSI uses to represent the default type for a Time Series Instance.
        // TODO: replace hardcoding the Type GUID when the Types resource has been implemented.
        private const string DefaultType = "1be09af9-f089-4d6b-9f0b-48018b5f7393";
        private const int MaxNumberOfRetries = 10;

        public TimeSeriesInsightsTypesTests(bool isAsync)
            : base(isAsync)
        {
        }

        [Test]
        public async Task TimeSeriesInsightsTypes_Lifecycle()
        {
            // Arrange
            TimeSeriesInsightsClient client = GetClient();
            var timeSeriesTypes = new List<TimeSeriesType>();
            var timeSeriesTypesNames = new string[2]
            {
                "type1",
                "type2"
            };
            var value = new TimeSeriesVariable();
            value.Kind = "Numeric";
            var variables = new Dictionary<string, TimeSeriesVariable>();
            variables.Add("testVariable", value);

            // create type name a unique name
            // create dictionary of variables
            foreach (string name in timeSeriesTypesNames)
            {
                timeSeriesTypes.Add(new TimeSeriesType(name, variables));
            }

            // Act and assert
            try
            {
                // Get all Time Series instances in the environment
                AsyncPageable<TimeSeriesType> getAllTypesResponse = client.GetTimeSeriesTypesAsync();

                int numOfTypes = 0;
                await foreach (TimeSeriesType tsiType in getAllTypesResponse)
                {
                    numOfTypes++;
                    tsiType.Should().NotBeNull();
                }

                // Create TSI types
                Response<InstancesOperationError[]> createInstancesResult = await client
                    .CreateOrReplaceTimeSeriesTypesAsync(timeSeriesTypes)
                    .ConfigureAwait(false);

                // Assert that the result error array does not contain any object that is set
                createInstancesResult.Value.Should().OnlyContain((errorResult) => errorResult == null);

                // This retry logic was added as the TSI instance are not immediately available after creation
                await TestRetryHelper.RetryAsync<Response<InstancesOperationResult[]>>(async () =>
                {
                    // Get the created instances by Ids
                    Response<TimeSeriesTypeOperationResult[]> getInstancesByNamesResult = await client
                        .GetTimeSeriesTypesbyNamesAsync(timeSeriesTypesNames)
                        .ConfigureAwait(false);

                    getInstancesByNamesResult.Value.Length.Should().Be(timeSeriesTypes.Count);
                    foreach (TimeSeriesTypeOperationResult typesResult in getInstancesByNamesResult.Value)
                    {
                        typesResult.TimeSeriesType.Should().NotBeNull();
                        typesResult.Error.Should().BeNull();
                        typesResult.TimeSeriesType.Id.Should().Be(DefaultType);
                        typesResult.TimeSeriesType.Variables.Count.Should().Be(1);
                        typesResult.TimeSeriesType.Variables.IsSameOrEqualTo(variables);
                    }
                    return null;
                }, MaxNumberOfRetries, s_retryDelay);
            }
            finally
            {
                // clean up
            }
        }
    }
}
