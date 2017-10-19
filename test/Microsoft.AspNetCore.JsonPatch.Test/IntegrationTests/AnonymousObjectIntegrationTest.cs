// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Test.IntegrationTests
{
    public class AnonymousObjectIntegrationTest
    {
        [Fact]
        public void ShouldAddToList_WithDifferentCase()
        {
            var targetObject = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("integerlist/0", 4);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, targetObject.IntegerList);
        }

        [Fact]
        public void AddToList_InvalidPositionTooLarge()
        {
            var targetObject = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("IntegerList/4", 4);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(targetObject);
            });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "4"),
                exception.Message);
        }

        [Fact]
        public void AddToList_AtBeginning()
        {
            var targetObject = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("IntegerList/0", 4);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, targetObject.IntegerList);
        }

        [Fact]
        public void AddToList_InvalidPositionTooSmall()
        {
            var targetObject = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("IntegerList/-1", 4);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(targetObject);
            });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "-1"),
                exception.Message);
        }

        [Fact]
        public void AddToList_Append()
        {
            var targetObject = new
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("IntegerList/-", 4);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2, 3, 4 }, targetObject.IntegerList);
        }

        [Fact]
        public void AddNewProperty_ToNestedAnonymousObject_ShouldFail()
        {
            dynamic targetObject = new
            {
                Test = 1,
                nested = new { }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("Nested/NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(targetObject);
            });
            Assert.Equal(
                string.Format("The target location specified by path segment '{0}' was not found.", "NewInt"),
                exception.Message);
        }

        [Fact]
        public void AddNewProperty_ToTypedObject_ShouldFail()
        {
            dynamic targetObject = new
            {
                Test = 1,
                nested = new NestedObject()
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("Nested/NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(targetObject);
            });
            Assert.Equal(
                string.Format("The target location specified by path segment '{0}' was not found.", "NewInt"),
                exception.Message);
        }

        [Fact]
        public void Add_DoesNotReplace()
        {
            var targetObject = new
            {
                StringProperty = "A"
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("StringProperty", "B");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(targetObject);
            });
            Assert.Equal(
                string.Format("The property at path '{0}' could not be updated.", "StringProperty"),
                exception.Message);
        }

        [Fact]
        public void RemoveProperty_ShouldFail_IfRootIsAnonymous()
        {
            dynamic targetObject = new
            {
                Test = 1
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("Test");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(targetObject);
            });
            Assert.Equal(
                string.Format("The property at path '{0}' could not be updated.", "Test"),
                exception.Message);
        }

        [Fact]
        public void ReplaceGuid_ExpandoObject_InAnonymous()
        {
            dynamic nestedObject = new ExpandoObject();
            nestedObject.GuidValue = Guid.NewGuid();

            dynamic targetObject = new
            {
                NestedObject = nestedObject
            };

            var newGuid = Guid.NewGuid();
            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("nestedobject/GuidValue", newGuid);

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(newGuid, targetObject.NestedObject.GuidValue);
        }
    }
}
