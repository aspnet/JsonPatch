// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Test.Dynamic
{
    public class DynamicObjectAdapterTests
    {
        // "add" tests
        [Fact]
        public void AddNewPropertyToDynamicObject()
        {
            // Arrange
            dynamic obj = new DynamicTestObject();
            obj.Test = 1;

            var patchDoc = new JsonPatchDocument();
            patchDoc.Add("NewInt", 1);

            // Act
            patchDoc.ApplyTo(obj);

            // Assert
            Assert.Equal(1, obj.NewInt);
            Assert.Equal(1, obj.Test);
        }

        [Fact]
        public void AddNewPropertyToDynamicOjectInTypedObject()
        {
            // Arrange
            var doc = new NestedDTO()
            {
                DynamicProperty = new DynamicTestObject()
            };

            var patchDoc = new JsonPatchDocument();
            patchDoc.Add("DynamicProperty/NewInt", 1);
            
            // Act
            patchDoc.ApplyTo(doc);

            // Assert
            Assert.Equal(1, doc.DynamicProperty.NewInt);
        }

        [Fact]
        public void AddNewPropertyToTypedObjectInDynamicObject()
        {
            dynamic dynamicProperty = new DynamicTestObject();
            dynamicProperty.StringProperty = "A";

            var doc = new NestedDTO()
            {
                DynamicProperty = dynamicProperty
            };

            // create patch
            var patchDoc = new JsonPatchDocument();
            patchDoc.Add("DynamicProperty/StringProperty", "B");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("B", doc.DynamicProperty.StringProperty);
        }

        [Fact]
        public void AddResultsShouldReplace()
        {
            dynamic doc = new DynamicTestObject();
            doc.StringProperty = "A";

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("StringProperty", "B");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("B", doc.StringProperty);
        }

        [Fact]
        public void AddResultsShouldReplaceInNested()
        {
            dynamic doc = new DynamicTestObject();
            doc.InBetweenFirst = new DynamicTestObject();
            doc.InBetweenFirst.InBetweenSecond = new DynamicTestObject();
            doc.InBetweenFirst.InBetweenSecond.StringProperty = "A";

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("/InBetweenFirst/InBetweenSecond/StringProperty", "B");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("B", doc.InBetweenFirst.InBetweenSecond.StringProperty);
        }

        [Fact]
        public void AddResultsShouldReplaceInNestedInDynamic()
        {
            dynamic doc = new DynamicTestObject();
            doc.Nested = new NestedDTO();
            doc.Nested.DynamicProperty = new DynamicTestObject();
            doc.Nested.DynamicProperty.InBetweenFirst = new DynamicTestObject();
            doc.Nested.DynamicProperty.InBetweenFirst.InBetweenSecond = new DynamicTestObject();
            doc.Nested.DynamicProperty.InBetweenFirst.InBetweenSecond.StringProperty = "A";

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("/Nested/DynamicProperty/InBetweenFirst/InBetweenSecond/StringProperty", "B");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("B", doc.Nested.DynamicProperty.InBetweenFirst.InBetweenSecond.StringProperty);
        }

        [Fact]
        public void ShouldNotBeAbleToAddToNonExistingPropertyThatIsNotTheRoot()
        {
            //Adding to a Nonexistent Target
            //
            //   An example target JSON document:
            //   { "foo": "bar" }
            //   A JSON Patch document:
            //   [
            //        { "op": "add", "path": "/baz/bat", "value": "qux" }
            //      ]
            //   This JSON Patch document, applied to the target JSON document above,
            //   would result in an error (therefore, it would not be applied),
            //   because the "add" operation's target location that references neither
            //   the root of the document, nor a member of an existing object, nor a
            //   member of an existing array.

            var doc = new NestedDTO()
            {
                DynamicProperty = new DynamicTestObject()
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Add("DynamicProperty/OtherProperty/IntProperty", 1);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(doc);
            });
            Assert.Equal(
                string.Format(
                    "The target location specified by path segment '{0}' was not found.",
                    "OtherProperty"),
                exception.Message);
        }

        // "copy" tests
        [Fact]
        public void Copy()
        {
            dynamic doc = new DynamicTestObject();

            doc.StringProperty = "A";
            doc.AnotherStringProperty = "B";

            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("StringProperty", "AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(doc);

            Assert.Equal("A", doc.AnotherStringProperty);
        }

        [Fact]
        public void CopyInList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("IntegerList/0", "IntegerList/1");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 1, 2, 3 }, doc.IntegerList);
        }

        [Fact]
        public void CopyFromListToEndOfList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("IntegerList/0", "IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 2, 3, 1 }, doc.IntegerList);
        }

        [Fact]
        public void CopyFromListToNonList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("IntegerList/0", "IntegerValue");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(1, doc.IntegerValue);
        }

        [Fact]
        public void CopyFromNonListToList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerValue = 5;
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("IntegerValue", "IntegerList/0");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, doc.IntegerList);
        }

        [Fact]
        public void CopyToEndOfList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerValue = 5;
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("IntegerValue", "IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, doc.IntegerList);
        }

        [Fact]
        public void NestedCopy()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                StringProperty = "A",
                AnotherStringProperty = "B"
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("SimpleDTO/StringProperty", "SimpleDTO/AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("A", doc.SimpleDTO.AnotherStringProperty);
        }

        [Fact]
        public void NestedCopyInList()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("SimpleDTO/IntegerList/0", "SimpleDTO/IntegerList/1");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 1, 2, 3 }, doc.SimpleDTO.IntegerList);
        }

        [Fact]
        public void NestedCopyFromListToEndOfList()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("SimpleDTO/IntegerList/0", "SimpleDTO/IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 2, 3, 1 }, doc.SimpleDTO.IntegerList);
        }

        [Fact]
        public void NestedCopyFromListToNonList()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("SimpleDTO/IntegerList/0", "SimpleDTO/IntegerValue");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(doc);

            Assert.Equal(1, doc.SimpleDTO.IntegerValue);
        }

        [Fact]
        public void NestedCopyFromNonListToList()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("SimpleDTO/IntegerValue", "SimpleDTO/IntegerList/0");
            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, doc.SimpleDTO.IntegerList);
        }

        [Fact]
        public void NestedCopyToEndOfList()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Copy("SimpleDTO/IntegerValue", "SimpleDTO/IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, doc.SimpleDTO.IntegerList);
        }

        // "move" tests
        [Fact]
        public void Move()
        {
            dynamic doc = new DynamicTestObject();
            doc.StringProperty = "A";
            doc.AnotherStringProperty = "B";

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("StringProperty", "AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("A", doc.AnotherStringProperty);

            object valueFromDictionary;
            doc.TryGetValue("StringProperty", out valueFromDictionary);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void MoveToNonExisting()
        {
            dynamic doc = new DynamicTestObject();
            doc.StringProperty = "A";

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("StringProperty", "AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("A", doc.AnotherStringProperty);

            object valueFromDictionary;
            doc.TryGetValue("StringProperty", out valueFromDictionary);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void MoveDynamicToTyped()
        {
            dynamic doc = new DynamicTestObject();
            doc.StringProperty = "A";
            doc.SimpleDTO = new SimpleDTO() { AnotherStringProperty = "B" };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("StringProperty", "SimpleDTO/AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("A", doc.SimpleDTO.AnotherStringProperty);

            object valueFromDictionary;
            doc.TryGetValue("StringProperty", out valueFromDictionary);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void MoveTypedToDynamic()
        {
            dynamic doc = new DynamicTestObject();
            doc.StringProperty = "A";
            doc.SimpleDTO = new SimpleDTO() { AnotherStringProperty = "B" };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("SimpleDTO/AnotherStringProperty", "StringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("B", doc.StringProperty);
            Assert.Null(doc.SimpleDTO.AnotherStringProperty);
        }

        [Fact]
        public void NestedMove()
        {
            dynamic doc = new DynamicTestObject();
            doc.Nested = new SimpleDTO()
            {
                StringProperty = "A",
                AnotherStringProperty = "B"
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("Nested/StringProperty", "Nested/AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal("A", doc.Nested.AnotherStringProperty);
            Assert.Null(doc.Nested.StringProperty);
        }

        [Fact]
        public void MoveInList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("IntegerList/0", "IntegerList/1");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 2, 1, 3 }, doc.IntegerList);
        }

        [Fact]
        public void NestedMoveInList()
        {
            dynamic doc = new DynamicTestObject();
            doc.Nested = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("Nested/IntegerList/0", "Nested/IntegerList/1");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 2, 1, 3 }, doc.Nested.IntegerList);
        }

        [Fact]
        public void MoveFromListToEndOfList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("IntegerList/0", "IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 2, 3, 1 }, doc.IntegerList);
        }

        [Fact]
        public void NestedMoveFromListToEndOfList()
        {
            dynamic doc = new DynamicTestObject();
            doc.Nested = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("Nested/IntegerList/0", "Nested/IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 2, 3, 1 }, doc.Nested.IntegerList);
        }

        [Fact]
        public void MoveFomListToNonList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("IntegerList/0", "IntegerValue");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 2, 3 }, doc.IntegerList);
            Assert.Equal(1, doc.IntegerValue);
        }

        [Fact]
        public void NestedMoveFomListToNonList()
        {
            dynamic doc = new DynamicTestObject();
            doc.Nested = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("Nested/IntegerList/0", "Nested/IntegerValue");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 2, 3 }, doc.Nested.IntegerList);
            Assert.Equal(1, doc.Nested.IntegerValue);
        }

        [Fact]
        public void MoveFromNonListToList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerValue = 5;
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("IntegerValue", "IntegerList/0");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            object valueFromDictionary;
            doc.TryGetValue("IntegerValue", out valueFromDictionary);
            Assert.Equal(0, valueFromDictionary);

            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, doc.IntegerList);
        }

        [Fact]
        public void NestedMoveFromNonListToList()
        {
            dynamic doc = new DynamicTestObject();
            doc.Nested = new SimpleDTO()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("Nested/IntegerValue", "Nested/IntegerList/0");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(0, doc.Nested.IntegerValue);
            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, doc.Nested.IntegerList);
        }

        [Fact]
        public void MoveToEndOfList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerValue = 5;
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("IntegerValue", "IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            object valueFromDictionary;
            doc.TryGetValue("IntegerValue", out valueFromDictionary);
            Assert.Equal(0, valueFromDictionary);

            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, doc.IntegerList);
        }

        [Fact]
        public void NestedMoveToEndOfList()
        {
            dynamic doc = new DynamicTestObject();
            doc.Nested = new SimpleDTO()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Move("Nested/IntegerValue", "Nested/IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(0, doc.Nested.IntegerValue);
            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, doc.Nested.IntegerList);
        }

        // "remove" tests
        [Fact]
        public void RemovePropertyShouldFailIfItDoesntExist()
        {
            dynamic doc = new DynamicTestObject();
            doc.Test = 1;

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("NonExisting");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(doc);
            });
            Assert.Equal(
                string.Format("The target location specified by path segment '{0}' was not found.", "NonExisting"),
                exception.Message);
        }

        [Fact]
        public void RemovePropertyFromDynamicObject()
        {
            dynamic obj = new DynamicTestObject();
            obj.Test = 1;

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("Test");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(obj);

            object valueFromDictionary;

            obj.TryGetValue("Test", out valueFromDictionary);
            Assert.Equal(0, valueFromDictionary);
        }

        [Fact]
        public void RemovePropertyFromDynamicObjectMixedCaseThrowsPathNotFoundException()
        {
            dynamic obj = new DynamicTestObject();
            obj.Test = 1;

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("test");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(obj);
            });

            Assert.Equal(
                string.Format(
                    "The target location specified by path segment '{0}' was not found.",
                    "test"),
                exception.Message);
        }

        [Fact]
        public void RemoveNestedPropertyFromDynamicObject()
        {
            dynamic obj = new DynamicTestObject();
            obj.Test = new DynamicTestObject();
            obj.Test.AnotherTest = "A";

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("Test");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(obj);

            object valueFromDictionary;

            obj.TryGetValue("Test", out valueFromDictionary);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void RemoveNestedPropertyFromDynamicObjectMixedCaseThrowsPathNotFoundException()
        {
            dynamic obj = new DynamicTestObject();
            obj.Test = new DynamicTestObject();
            obj.Test.AnotherTest = "A";

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("test");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(obj);
            });

            Assert.Equal(
                string.Format(
                    "The target location specified by path segment '{0}' was not found.",
                    "test"),
                exception.Message);
        }

        [Fact]
        public void NestedRemove()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                StringProperty = "A"
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("SimpleDTO/StringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);
            Assert.Null(doc.SimpleDTO.StringProperty);
        }

        [Fact]
        public void NestedRemoveMixedCaseThrowsPathNotFoundException()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                StringProperty = "A"
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("Simpledto/stringProperty");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(doc);
            });

            Assert.Equal(
               string.Format("The target location specified by path segment '{0}' was not found.",
               "Simpledto"),
                exception.Message);
        }

        [Fact]
        public void NestedRemoveFromList()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("SimpleDTO/IntegerList/2");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 2 }, doc.SimpleDTO.IntegerList);
        }

        [Fact]
        public void NestedRemoveFromListMixedCase()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("SimpleDTO/Integerlist/2");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 2 }, doc.SimpleDTO.IntegerList);
        }

        [Fact]
        public void NestedRemoveFromListInvalidPositionTooLarge()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("SimpleDTO/IntegerList/3");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(doc);
            });
            Assert.Equal(
               string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "3"),
                exception.Message);
        }

        [Fact]
        public void NestedRemoveFromListInvalidPositionTooSmall()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("SimpleDTO/IntegerList/-1");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(doc);
            });
            Assert.Equal(
               string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "-1"),
               exception.Message);
        }

        [Fact]
        public void NestedRemoveFromEndOfList()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Remove("SimpleDTO/IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);
            Assert.Equal(new List<int>() { 1, 2 }, doc.SimpleDTO.IntegerList);
        }

        // "replace" tests
        [Fact]
        public void ReplaceGuidTestDynamicObject()
        {
            dynamic doc = new DynamicTestObject();
            doc.GuidValue = Guid.NewGuid();

            var newGuid = Guid.NewGuid();
            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Replace("GuidValue", newGuid);

            // serialize & deserialize
            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserizalized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserizalized.ApplyTo(doc);

            Assert.Equal(newGuid, doc.GuidValue);
        }

        [Fact]
        public void ReplaceGuidTestDynamicObjectInAnonymous()
        {
            dynamic nestedObject = new DynamicTestObject();
            nestedObject.GuidValue = Guid.NewGuid();

            dynamic doc = new
            {
                NestedObject = nestedObject
            };

            var newGuid = Guid.NewGuid();
            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Replace("nestedobject/GuidValue", newGuid);

            // serialize & deserialize
            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserizalized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserizalized.ApplyTo(doc);

            Assert.Equal(newGuid, doc.NestedObject.GuidValue);
        }

        [Fact]
        public void ReplaceNestedObjectTest()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTO = new SimpleDTO()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var newDTO = new SimpleDTO()
            {
                DoubleValue = 1
            };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Replace("SimpleDTO", newDTO);

            // serialize & deserialize
            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(1, doc.SimpleDTO.DoubleValue);
            Assert.Equal(0, doc.SimpleDTO.IntegerValue);
            Assert.Null(doc.SimpleDTO.IntegerList);
        }

        [Fact]
        public void ReplaceInList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Replace("IntegerList/0", 5);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 5, 2, 3 }, doc.IntegerList);
        }

        [Fact]
        public void ReplaceFullList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Replace("IntegerList", new List<int>() { 4, 5, 6 });

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 4, 5, 6 }, doc.IntegerList);
        }

        [Fact]
        public void ReplaceInListInList()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTOList = new List<SimpleDTO>() {
                new SimpleDTO() {
                    IntegerList = new List<int>(){1,2,3}
                }};

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Replace("SimpleDTOList/0/IntegerList/0", 4);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(4, doc.SimpleDTOList[0].IntegerList[0]);
        }

        [Fact]
        public void ReplaceInListInListAtEnd()
        {
            dynamic doc = new DynamicTestObject();
            doc.SimpleDTOList = new List<SimpleDTO>() {
                new SimpleDTO() {
                    IntegerList = new List<int>(){1,2,3}
                }};

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Replace("SimpleDTOList/0/IntegerList/-", 4);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(4, doc.SimpleDTOList[0].IntegerList[2]);
        }

        [Fact]
        public void ReplaceFullListFromEnumerable()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Replace("IntegerList", new List<int>() { 4, 5, 6 });

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 4, 5, 6 }, doc.IntegerList);
        }

        [Fact]
        public void ReplaceFullListWithCollection()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Replace("IntegerList", new Collection<int>() { 4, 5, 6 });

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 4, 5, 6 }, doc.IntegerList);
        }

        [Fact]
        public void ReplaceAtEndOfList()
        {
            dynamic doc = new DynamicTestObject();
            doc.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            JsonPatchDocument patchDoc = new JsonPatchDocument();
            patchDoc.Replace("IntegerList/-", 5);

            var serialized = JsonConvert.SerializeObject(patchDoc);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(doc);

            Assert.Equal(new List<int>() { 1, 2, 5 }, doc.IntegerList);
        }
    }
}
