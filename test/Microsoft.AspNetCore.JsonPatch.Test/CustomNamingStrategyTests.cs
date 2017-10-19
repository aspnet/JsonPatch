// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Test.Dynamic
{
    public class CustomNamingStrategyTests
    {
        [Fact]
        public void AddProperty_ToDynamicTestObject_WithCustomNamingStrategy()
        {
            // Arrange
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new TestNamingStrategy()
            };

            dynamic obj = new DynamicTestObject();
            obj.Test = 1;

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("NewInt", 1);
            patchDocument.ContractResolver = contractResolver;

            // Act
            patchDocument.ApplyTo(obj);

            // Assert
            Assert.Equal(1, obj.customNewInt);
            Assert.Equal(1, obj.Test);
        }

        [Fact]
        public void CopyPropertyValue_ToDynamicTestObject_WithCustomNamingStrategy()
        {
            // Arrange
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new TestNamingStrategy()
            };

            dynamic obj = new DynamicTestObject();
            obj.customStringProperty = "A";
            obj.customAnotherStringProperty = "B";

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("StringProperty", "AnotherStringProperty");
            patchDocument.ContractResolver = contractResolver;

            // Act
            patchDocument.ApplyTo(obj);

            // Assert
            Assert.Equal("A", obj.customAnotherStringProperty);
        }

        [Fact]
        public void MovePropertyValue_ForExpandoObject_WithCustomNamingStrategy()
        {
            // Arrange
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new TestNamingStrategy()
            };

            dynamic obj = new ExpandoObject();
            obj.customStringProperty = "A";
            obj.customAnotherStringProperty = "B";

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("StringProperty", "AnotherStringProperty");
            patchDocument.ContractResolver = contractResolver;

            // Act
            patchDocument.ApplyTo(obj);
            var cont = obj as IDictionary<string, object>;
            cont.TryGetValue("customStringProperty", out var valueFromDictionary);

            // Assert
            Assert.Equal("A", obj.customAnotherStringProperty);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void RemoveProperty_FromDictionaryObject_WithCustomNamingStrategy()
        {
            // Arrange
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new TestNamingStrategy()
            };

            var obj = new Dictionary<string, int>()
            {
                { "customTest", 1},
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("Test");
            patchDocument.ContractResolver = contractResolver;

            // Act
            patchDocument.ApplyTo(obj);
            var cont = obj as IDictionary<string, int>;
            cont.TryGetValue("customTest", out var valueFromDictionary);

            // Assert
            Assert.Equal(0, valueFromDictionary);
        }

        [Fact]
        public void ReplacePropertyValue_ForExpandoObject_WithCustomNamingStrategy()
        {
            // Arrange
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new TestNamingStrategy()
            };

            dynamic obj = new ExpandoObject();
            obj.customTest = 1;

            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("Test", 2);
            patchDocument.ContractResolver = contractResolver;

            // Act
            patchDocument.ApplyTo(obj);

            // Assert
            Assert.Equal(2, obj.customTest);
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
