// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch
{
    public class JsonPatchDocumentTest
    {
        [Fact]
        public void InvalidPathAtBeginningShouldThrowException()
        {
            var patchDocument = new JsonPatchDocument();
            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                patchDocument.Add("//NewInt", 1);
            });
            Assert.Equal(
               "The provided string '//NewInt' is an invalid path.",
                exception.Message);
        }

        [Fact]
        public void InvalidPathAtEndShouldThrowException()
        {
            var patchDocument = new JsonPatchDocument();
            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                patchDocument.Add("NewInt//", 1);
            });
            Assert.Equal(
               "The provided string 'NewInt//' is an invalid path.",
                exception.Message);
        }

        [Fact]
        public void InvalidPathWithDotShouldThrowException()
        {
            var patchDocument = new JsonPatchDocument();
            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                patchDocument.Add("NewInt.Test", 1);
            });
            Assert.Equal(
               "The provided string 'NewInt.Test' is an invalid path.",
                exception.Message);
        }

        [Fact]
        public void NonGenericPatchDocToGenericMustSerialize()
        {
            var targetObject = new SimpleObject()
            {
                StringProperty = "A",
                AnotherStringProperty = "B"
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("StringProperty", "AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObject>>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("A", targetObject.AnotherStringProperty);
        }

        [Fact]
        public void GenericPatchDocToNonGenericMustSerialize()
        {
            var targetObject = new SimpleObject()
            {
                StringProperty = "A",
                AnotherStringProperty = "B"
            };

            var patchDocTyped = new JsonPatchDocument<SimpleObject>();
            patchDocTyped.Copy(o => o.StringProperty, o => o.AnotherStringProperty);

            var patchDocUntyped = new JsonPatchDocument();
            patchDocUntyped.Copy("StringProperty", "AnotherStringProperty");

            var serializedTyped = JsonConvert.SerializeObject(patchDocTyped);
            var serializedUntyped = JsonConvert.SerializeObject(patchDocUntyped);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serializedTyped);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("A", targetObject.AnotherStringProperty);
        }

        [Fact]
        public void Deserialization_Successful_ForValidJsonPatchDocument()
        {
            // Arrange
            var doc = new SimpleObject()
            {
                StringProperty = "A",
                DecimalValue = 10,
                DoubleValue = 10,
                FloatValue = 10,
                IntegerValue = 10
            };

            // create patch
            var patchDoc = new JsonPatchDocument<SimpleObject>();
            patchDoc.Replace(o => o.StringProperty, "B");
            patchDoc.Replace(o => o.DecimalValue, 12);
            patchDoc.Replace(o => o.DoubleValue, 12);
            patchDoc.Replace(o => o.FloatValue, 12);
            patchDoc.Replace(o => o.IntegerValue, 12);

            // default: no envelope
            var serialized = JsonConvert.SerializeObject(patchDoc);

            // Act
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObject>>(serialized);

            // Assert
            Assert.IsType<JsonPatchDocument<SimpleObject>>(deserialized);
        }

        [Fact]
        public void Deserialization_Fails_ForInvalidJsonPatchDocument()
        {
            // Arrange
            var serialized = "{\"Operations\": [{ \"op\": \"replace\", \"path\": \"/title\", \"value\": \"New Title\"}]}";

            // Act & Assert
            var exception = Assert.Throws<JsonSerializationException>(() =>
            {
                var deserialized
                    = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            });

            Assert.Equal("The JSON patch document was malformed and could not be parsed.", exception.Message);
        }

        [Fact]
        public void Deserialization_Fails_ForInvalidTypedJsonPatchDocument()
        {
            // Arrange
            var serialized = "{\"Operations\": [{ \"op\": \"replace\", \"path\": \"/title\", \"value\": \"New Title\"}]}";

            // Act & Assert
            var exception = Assert.Throws<JsonSerializationException>(() =>
            {
                var deserialized
                    = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObject>>(serialized);
            });

            Assert.Equal("The JSON patch document was malformed and could not be parsed.", exception.Message);
        }
    }
}