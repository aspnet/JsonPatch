// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch
{
    public class JsonPatchDocumentJsonPropertyAttributeTest
    {
        [Fact]
        public void Add_ToRoot_OfListOfObjects_AtEndOfList()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<List<JsonPropertyObject>>();
            patchDocument.Add(p => p, new JsonPropertyObject());

            var serialized = JsonConvert.SerializeObject(patchDocument);

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithAnotherNameObject>>(serialized);
            var pathToCheck = deserialized.Operations.First().path;

            // Assert
            Assert.Equal("/-", pathToCheck);
        }

        [Fact]
        public void Add_ToRoot_OfListOfObjects_AtGivenPosition()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<List<JsonPropertyObject>>();
            patchDocument.Add(p => p[3], new JsonPropertyObject());

            var serialized = JsonConvert.SerializeObject(patchDocument);

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithAnotherNameObject>>(serialized);
            var pathToCheck = deserialized.Operations.First().path;

            // Assert
            Assert.Equal("/3", pathToCheck);
        }

        [Fact]
        public void Add_WithExpression_RespectsJsonPropertyName_ForModelProperty()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<JsonPropertyObject>();
            patchDocument.Add(p => p.Name, "John");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            // serialized value should have "AnotherName" as path
            // deserialize to a JsonPatchDocument<JsonPropertyWithAnotherNameDTO> to check

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithAnotherNameObject>>(serialized);
            var pathToCheck = deserialized.Operations.First().path;

            // Assert
            Assert.Equal("/AnotherName", pathToCheck);
        }

        [Fact]
        public void Add_WithExpressionOnStringProperty_FallsbackToPropertyName_WhenJsonPropertyName_IsEmpty()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<JsonPropertyWithNoPropertyName>();
            patchDocument.Add(m => m.StringProperty, "Test");
            var serialized = JsonConvert.SerializeObject(patchDocument);

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithNoPropertyName>>(serialized);

            // Assert
            var pathToCheck = deserialized.Operations.First().path;
            Assert.Equal("/StringProperty", pathToCheck);
        }

        [Fact]
        public void Add_WithExpressionOnArrayProperty_FallsbackToPropertyName_WhenJsonPropertyName_IsEmpty()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<JsonPropertyWithNoPropertyName>();
            patchDocument.Add(m => m.ArrayProperty, "James");
            var serialized = JsonConvert.SerializeObject(patchDocument);

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithNoPropertyName>>(serialized);
            var pathToCheck = deserialized.Operations.First().path;

            // Assert
            Assert.Equal("/ArrayProperty/-", pathToCheck);
        }

        [Fact]
        public void Add_WithExpression_RespectsJsonPropertyName_WhenApplyingToDifferentlyTypedClassWithPropertyMatchingJsonPropertyName()
        {
            // Arrange
            var patchDocToSerialize = new JsonPatchDocument<JsonPropertyObject>();
            patchDocToSerialize.Add(p => p.Name, "John");

            var targetObject = new JsonPropertyWithAnotherNameObject()
            {
                AnotherName = "InitialValue"
            };

            var serialized = JsonConvert.SerializeObject(patchDocToSerialize);

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithAnotherNameObject>>
                (serialized);

            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("John", targetObject.AnotherName);
        }

        [Fact]
        public void Add_WithExpression_RespectsJsonPropertyName_WhenApplyingToSameTypedClassWithMatchingJsonPropertyName()
        {
            // Arrange
            var patchDocToSerialize = new JsonPatchDocument<JsonPropertyObject>();
            patchDocToSerialize.Add(p => p.Name, "John");

            var targetObject = new JsonPropertyObject()
            {
                Name = "InitialValue"
            };

            var serialized = JsonConvert.SerializeObject(patchDocToSerialize);

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyObject>>
                (serialized);

            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("John", targetObject.Name);
        }

        [Fact]
        public void Add_OnApplyFromJson_RespectsJsonPropertyNameOnJsonDocument()
        {
            // Arrange
            var targetObject = new JsonPropertyObject()
            {
                Name = "InitialValue"
            };

            // serialization should serialize to "AnotherName"
            var serialized = "[{\"value\":\"Kevin\",\"path\":\"/AnotherName\",\"op\":\"add\"}]";

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyObject>>(serialized);
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("Kevin", targetObject.Name);
        }

        [Fact]
        public void Remove_OnApplyFromJson_RespectsJsonPropertyNameOnJsonDocument()
        {
            // Arrange
            var targetObject = new JsonPropertyObject()
            {
                Name = "InitialValue"
            };

            // serialization should serialize to "AnotherName"
            var serialized = "[{\"path\":\"/AnotherName\",\"op\":\"remove\"}]";

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyObject>>(serialized);
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Null(targetObject.Name);
        }

        [Fact]
        public void Add_OnApplyFromJson_RespectsInheritedJsonPropertyNameOnJsonDocument()
        {
            // Arrange
            var targetObject = new JsonPropertyWithInheritanceObject()
            {
                Name = "InitialName"
            };

            // serialization should serialize to "AnotherName"
            var serialized = "[{\"value\":\"Kevin\",\"path\":\"/AnotherName\",\"op\":\"add\"}]";

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithInheritanceObject>>(serialized);
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("Kevin", targetObject.Name);
        }

        [Fact]
        public void Add_WithExpression_RespectsJsonPropertyName_ForInheritedModelProperty()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<JsonPropertyWithInheritanceObject>();
            patchDocument.Add(p => p.Name, "John");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            // serialized value should have "AnotherName" as path
            // deserialize to a JsonPatchDocument<JsonPropertyWithAnotherNameDTO> to check

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithAnotherNameObject>>(serialized);
            var pathToCheck = deserialized.Operations.First().path;

            // Assert
            Assert.Equal("/AnotherName", pathToCheck);
        }

        [Fact]
        public void Add_OnApplyFromJson_EscapingHandledOnComplexJsonPropertyNameOnJsonDocument()
        {
            // Arrange
            var targetObject = new JsonPropertyComplexNameObject()
            {
                FooSlashBars = "InitialName",
                FooSlashTilde = new SimpleObject
                {
                    StringProperty = "Initial Value"
                }
            };

            // serialization should serialize to "AnotherName"
            var serialized = "[{\"value\":\"Kevin\",\"path\":\"/foo~1bar~0\",\"op\":\"add\"},{\"value\":\"Final Value\",\"path\":\"/foo~1~0/StringProperty\",\"op\":\"replace\"}]";

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyComplexNameObject>>(serialized);
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("Kevin", targetObject.FooSlashBars);
            Assert.Equal("Final Value", targetObject.FooSlashTilde.StringProperty);
        }

        [Fact]
        public void Move_WithExpression_FallsbackToPropertyName_WhenJsonPropertyName_IsEmpty()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<JsonPropertyWithNoPropertyName>();
            patchDocument.Move(m => m.StringProperty, m => m.StringProperty2);
            var serialized = JsonConvert.SerializeObject(patchDocument);

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithNoPropertyName>>(serialized);

            // Assert
            var fromPath = deserialized.Operations.First().from;
            Assert.Equal("/StringProperty", fromPath);
            var toPath = deserialized.Operations.First().path;
            Assert.Equal("/StringProperty2", toPath);
        }

        [Fact]
        public void Add_WithExpression_AndCustomContractResolver_UsesPropertyName_SetByContractResolver()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<JsonPropertyWithNoPropertyName>();
            patchDocument.ContractResolver = new CustomContractResolver();
            patchDocument.Add(m => m.SSN, "123-45-6789");
            var serialized = JsonConvert.SerializeObject(patchDocument);

            // Act
            var deserialized =
                JsonConvert.DeserializeObject<JsonPatchDocument<JsonPropertyWithNoPropertyName>>(serialized);
            var path = deserialized.Operations.First().path;

            // Assert
            Assert.Equal("/SocialSecurityNumber", path);
        }

        private class JsonPropertyWithNoPropertyName
        {
            [JsonProperty]
            public string StringProperty { get; set; }

            [JsonProperty]
            public string[] ArrayProperty { get; set; }

            [JsonProperty]
            public string StringProperty2 { get; set; }

            [JsonProperty]
            public string SSN { get; set; }
        }

        private class CustomContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var jsonProperty = base.CreateProperty(member, memberSerialization);

                if (jsonProperty.PropertyName == "SSN")
                {
                    jsonProperty.PropertyName = "SocialSecurityNumber";
                }

                return jsonProperty;
            }
        }
    }
}
