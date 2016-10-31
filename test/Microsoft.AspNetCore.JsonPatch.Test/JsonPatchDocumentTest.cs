// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Test
{
    public class JsonPatchDocumentTest
    {
        [Fact]
        public void InvalidOperation_ThrowsException_OnDeserialization()
        {
            // Arrange
            var serialized = "[{\"value\":\"John\",\"path\":\"/Name\",\"op\":\"foo\"}]";
            var jsonSerializerSettings = new JsonSerializerSettings();
            string actualErrorMessage = null;
            jsonSerializerSettings.Error = (obj, eventArgs) =>
            {
                var exception = eventArgs.ErrorContext.Error;
                if (exception != null && exception.InnerException != null)
                {
                    actualErrorMessage = exception.InnerException.Message;
                }
                eventArgs.ErrorContext.Handled = true;
            };

            // Act
            var jsonPatchException = JsonConvert.DeserializeObject<JsonPatchDocument<Customer>>(
                serialized,
                jsonSerializerSettings);

            // Assert
            Assert.Equal("Invalid JsonPatch operation 'foo'.", actualErrorMessage);
        }

        private class Customer
        {
            public string Name { get; set; }
        }
    }
}
