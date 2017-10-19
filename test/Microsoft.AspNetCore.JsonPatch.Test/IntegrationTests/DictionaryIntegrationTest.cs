// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch
{
    public class DictionaryTest
    {
        [Fact]
        public void Add_WhenDictionary_ValueIsNonObject_Succeeds()
        {
            // Arrange
            var model = new IntDictionary();
            model.DictionaryOfStringToInteger["one"] = 1;
            model.DictionaryOfStringToInteger["two"] = 2;
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("/DictionaryOfStringToInteger/three", 3);

            // Act
            patchDocument.ApplyTo(model);

            // Assert
            Assert.Equal(3, model.DictionaryOfStringToInteger.Count);
            Assert.Equal(1, model.DictionaryOfStringToInteger["one"]);
            Assert.Equal(2, model.DictionaryOfStringToInteger["two"]);
            Assert.Equal(3, model.DictionaryOfStringToInteger["three"]);
        }

        [Fact]
        public void Remove_WhenDictionary_ValueIsNonObject_Succeeds()
        {
            // Arrange
            var model = new IntDictionary();
            model.DictionaryOfStringToInteger["one"] = 1;
            model.DictionaryOfStringToInteger["two"] = 2;
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("/DictionaryOfStringToInteger/two");

            // Act
            patchDocument.ApplyTo(model);

            // Assert
            Assert.Equal(1, model.DictionaryOfStringToInteger.Count);
            Assert.Equal(1, model.DictionaryOfStringToInteger["one"]);
        }

        [Fact]
        public void Replace_WhenDictionary_ValueIsNonObject_Succeeds()
        {
            // Arrange
            var model = new IntDictionary();
            model.DictionaryOfStringToInteger["one"] = 1;
            model.DictionaryOfStringToInteger["two"] = 2;
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("/DictionaryOfStringToInteger/two", 20);

            // Act
            patchDocument.ApplyTo(model);

            // Assert
            Assert.Equal(2, model.DictionaryOfStringToInteger.Count);
            Assert.Equal(1, model.DictionaryOfStringToInteger["one"]);
            Assert.Equal(20, model.DictionaryOfStringToInteger["two"]);
        }

        private class Customer
        {
            public string Name { get; set; }
            public Address Address { get; set; }
        }

        private class Address
        {
            public string City { get; set; }
        }

        private class IntDictionary
        {
            public IDictionary<string, int> DictionaryOfStringToInteger { get; } = new Dictionary<string, int>();
        }

        private class CustomerDictionary
        {
            public IDictionary<string, Customer> DictionaryOfStringToCustomer { get; } = new Dictionary<string, Customer>();
        }

        [Fact]
        public void Replace_WhenDictionary_ValueAPocoType_Succeeds()
        {
            // Arrange
            var key1 = "key1";
            var value1 = new Customer() { Name = "Jamesss" };
            var key2 = "key2";
            var value2 = new Customer() { Name = "Mike" };
            var model = new CustomerDictionary();
            model.DictionaryOfStringToCustomer[key1] = value1;
            model.DictionaryOfStringToCustomer[key2] = value2;
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace($"/DictionaryOfStringToCustomer/{key1}/Name", "James");

            // Act
            patchDocument.ApplyTo(model);

            // Assert
            Assert.Equal(2, model.DictionaryOfStringToCustomer.Count);
            var actualValue1 = model.DictionaryOfStringToCustomer[key1];
            Assert.NotNull(actualValue1);
            Assert.Equal("James", actualValue1.Name);
        }

        [Fact]
        public void Replace_WhenDictionary_ValueAPocoType_Succeeds_WithSerialization()
        {
            // Arrange
            var key1 = "key1";
            var value1 = new Customer() { Name = "Jamesss" };
            var key2 = "key2";
            var value2 = new Customer() { Name = "Mike" };
            var model = new CustomerDictionary();
            model.DictionaryOfStringToCustomer[key1] = value1;
            model.DictionaryOfStringToCustomer[key2] = value2;
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace($"/DictionaryOfStringToCustomer/{key1}/Name", "James");
            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<CustomerDictionary>>(serialized);

            // Act
            patchDocument.ApplyTo(model);

            // Assert
            Assert.Equal(2, model.DictionaryOfStringToCustomer.Count);
            var actualValue1 = model.DictionaryOfStringToCustomer[key1];
            Assert.NotNull(actualValue1);
            Assert.Equal("James", actualValue1.Name);
        }

        [Fact]
        public void Replace_WhenDictionary_ValueAPocoType_WithEscaping_Succeeds()
        {
            // Arrange
            var key1 = "Foo/Name";
            var value1 = new Customer() { Name = "Jamesss" };
            var key2 = "Foo";
            var value2 = new Customer() { Name = "Mike" };
            var model = new CustomerDictionary();
            model.DictionaryOfStringToCustomer[key1] = value1;
            model.DictionaryOfStringToCustomer[key2] = value2;
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace($"/DictionaryOfStringToCustomer/Foo~1Name/Name", "James");

            // Act
            patchDocument.ApplyTo(model);

            // Assert
            Assert.Equal(2, model.DictionaryOfStringToCustomer.Count);
            var actualValue1 = model.DictionaryOfStringToCustomer[key1];
            var actualValue2 = model.DictionaryOfStringToCustomer[key2];
            Assert.NotNull(actualValue1);
            Assert.Equal("James", actualValue1.Name);
            Assert.Equal("Mike", actualValue2.Name);

        }

        [Fact]
        public void Replace_DeepNested_DictionaryValue_Succeeds()
        {
            // Arrange
            var key1 = "key1";
            var value1 = new Customer() { Name = "Jamesss" };
            var key2 = "key2";
            var value2 = new Customer() { Name = "Mike" };
            var model = new CustomerDictionary();
            model.DictionaryOfStringToCustomer[key1] = value1;
            model.DictionaryOfStringToCustomer[key2] = value2;
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace($"/DictionaryOfStringToCustomer/{key1}/Name", "James");

            // Act
            patchDocument.ApplyTo(model);

            // Assert
            Assert.Equal(2, model.DictionaryOfStringToCustomer.Count);
            var actualValue1 = model.DictionaryOfStringToCustomer[key1];
            Assert.NotNull(actualValue1);
            Assert.Equal("James", actualValue1.Name);
        }

        [Fact]
        public void Replace_DeepNested_DictionaryValue_Succeeds_WithSerialization()
        {
            // Arrange
            var key1 = "key1";
            var value1 = new Customer() { Name = "James", Address = new Address { City = "Redmond" } };
            var key2 = "key2";
            var value2 = new Customer() { Name = "Mike", Address = new Address { City = "Seattle" } };
            var model = new CustomerDictionary();
            model.DictionaryOfStringToCustomer[key1] = value1;
            model.DictionaryOfStringToCustomer[key2] = value2;
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace($"/DictionaryOfStringToCustomer/{key1}/Address/City", "Bellevue");
            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<CustomerDictionary>>(serialized);

            // Act
            patchDocument.ApplyTo(model);

            // Assert
            Assert.Equal(2, model.DictionaryOfStringToCustomer.Count);
            var actualValue1 = model.DictionaryOfStringToCustomer[key1];
            Assert.NotNull(actualValue1);
            Assert.Equal("James", actualValue1.Name);
            var address = actualValue1.Address;
            Assert.NotNull(address);
            Assert.Equal("Bellevue", address.City);
        }

        class Class9
        {
            public List<string> StringList { get; set; } = new List<string>();
        }
    }
}
