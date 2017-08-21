// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Test.Dynamic
{
    public class CustomNamingStrategyTests
    {
        [Fact]
        public void AddProperty_ToExpandoObject_WithCustomNamingStrategy()
        {
            // Arrange
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new TestNamingStrategy()
            };

            dynamic doc = new ExpandoObject();
            doc.Test = 1;

            // create patch
            var patchDoc = new JsonPatchDocument();
            patchDoc.Add("NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ContractResolver = contractResolver;

            // Act
            deserialized.ApplyTo(doc);

            // Assert
            Assert.Equal(1, doc.customNewInt);
            Assert.Equal(1, doc.Test);
        }

        [Fact]
        public void CopyPropertyValue_ForExpandoObject_WithCustomNamingStrategy()
        {
            // Arrange
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new TestNamingStrategy()
            };

            dynamic doc = new ExpandoObject();
            doc.customStringProperty = "A";
            doc.customAnotherStringProperty = "B";

            var patchDoc = new JsonPatchDocument();
            patchDoc.Copy("StringProperty", "AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ContractResolver = contractResolver;

            // Act
            deserialized.ApplyTo(doc);

            // Assert
            Assert.Equal("A", doc.customAnotherStringProperty);
        }

        [Fact]
        public void MovePropertyValue_ForExpandoObject_WithCustomNamingStrategy()
        {
            // Arrange
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new TestNamingStrategy()
            };

            dynamic doc = new ExpandoObject();
            doc.customStringProperty = "A";
            doc.customAnotherStringProperty = "B";

            // create patch
            var patchDoc = new JsonPatchDocument();
            patchDoc.Move("StringProperty", "AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ContractResolver = contractResolver;

            // Act
            deserialized.ApplyTo(doc);
            var cont = doc as IDictionary<string, object>;
            cont.TryGetValue("customStringProperty", out var valueFromDictionary);

            // Assert
            Assert.Equal("A", doc.customAnotherStringProperty);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void RemoveProperty_FromExpandoObject_WithCustomNamingStrategy()
        {
            // Arrange
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new TestNamingStrategy()
            };

            dynamic doc = new ExpandoObject();
            doc.customTest = 1;

            // create patch
            var patchDoc = new JsonPatchDocument();
            patchDoc.Remove("Test");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ContractResolver = contractResolver;

            // Act
            deserialized.ApplyTo(doc);
            var cont = doc as IDictionary<string, object>;
            cont.TryGetValue("customTest", out var valueFromDictionary);

            // Assert
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void ReplacePropertyValue_ForExpandoObject_WithCustomNamingStrategy()
        {
            // Arrange
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new TestNamingStrategy()
            };

            dynamic doc = new ExpandoObject();
            doc.customTest = 1;

            var patchDoc = new JsonPatchDocument();
            patchDoc.Replace("Test", 2);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ContractResolver = contractResolver;

            // Act
            deserialized.ApplyTo(doc);

            // Assert
            Assert.Equal(2, doc.customTest);
        }

        private class TestNamingStrategy : NamingStrategy
        {
            public new bool ProcessDictionaryKeys => true;

            public override string GetDictionaryKey(string key)
            {
                return "custom" + key;
            }

            protected override string ResolvePropertyName(string name)
            {
                return name;
            }
        }
    }
}
