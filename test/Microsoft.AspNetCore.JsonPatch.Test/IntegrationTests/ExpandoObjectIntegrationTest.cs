// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch
{
    public class ExpandoObjectIntegrationTest
    {
        [Fact]
        public void AddNewProperty_ToExpandoObject()
        {
            dynamic obj = new ExpandoObject();
            obj.Test = 1;

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(obj);

            Assert.Equal(1, obj.NewInt);
            Assert.Equal(1, obj.Test);
        }

        [Fact]
        public void AddNewProperty_ToExpandoOject_InTypedObject()
        {
            var targetObject = new NestedObject()
            {
                DynamicProperty = new ExpandoObject()
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("DynamicProperty/NewInt", 1);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(1, targetObject.DynamicProperty.NewInt);
        }

        [Fact]
        public void AddNewProperty_ToTypedObject_InExpandoObject()
        {
            dynamic dynamicProperty = new ExpandoObject();
            dynamicProperty.StringProperty = "A";

            var targetObject = new NestedObject()
            {
                DynamicProperty = dynamicProperty
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("DynamicProperty/StringProperty", "B");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("B", targetObject.DynamicProperty.StringProperty);
        }

        [Fact]
        public void AddReplaces_ExistingProperty()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.StringProperty = "A";

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("StringProperty", "B");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("B", targetObject.StringProperty);
        }

        [Fact]
        public void AddReplaces_ExistingProperty_InNestedObject()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.InBetweenFirst = new ExpandoObject();
            targetObject.InBetweenFirst.InBetweenSecond = new ExpandoObject();
            targetObject.InBetweenFirst.InBetweenSecond.StringProperty = "A";

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("/InBetweenFirst/InBetweenSecond/StringProperty", "B");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("B", targetObject.InBetweenFirst.InBetweenSecond.StringProperty);
        }

        [Fact]
        public void AddReplaces_InNestedObject_InDynamicObject()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.Nested = new NestedObject();
            targetObject.Nested.DynamicProperty = new ExpandoObject();
            targetObject.Nested.DynamicProperty.InBetweenFirst = new ExpandoObject();
            targetObject.Nested.DynamicProperty.InBetweenFirst.InBetweenSecond = new ExpandoObject();
            targetObject.Nested.DynamicProperty.InBetweenFirst.InBetweenSecond.StringProperty = "A";

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("/Nested/DynamicProperty/InBetweenFirst/InBetweenSecond/StringProperty", "B");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("B", targetObject.Nested.DynamicProperty.InBetweenFirst.InBetweenSecond.StringProperty);
        }

        [Fact]
        public void ShouldNotReplaceProperty_WithDifferentCase()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.StringProperty = "A";

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("stringproperty", "B");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("A", targetObject.StringProperty);
            Assert.Equal("B", targetObject.stringproperty);
        }

        [Fact]
        public void AddToList_NegativePosition()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.IntegerList = new List<int>() { 1, 2, 3 };

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
        public void Copy()
        {
            dynamic targetObject = new ExpandoObject();

            targetObject.StringProperty = "A";
            targetObject.AnotherStringProperty = "B";

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("StringProperty", "AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(targetObject);

            Assert.Equal("A", targetObject.AnotherStringProperty);
        }

        [Fact]
        public void CopyInList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("IntegerList/0", "IntegerList/1");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 1, 2, 3 }, targetObject.IntegerList);
        }

        [Fact]
        public void CopyToEndOfList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.IntegerValue = 5;
            targetObject.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("IntegerValue", "IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, targetObject.IntegerList);
        }

        [Fact]
        public void NestedCopyInList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.SimpleObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("SimpleObject/IntegerList/0", "SimpleObject/IntegerList/1");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void NestedCopy_FromList_ToEndOfList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.SimpleObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("SimpleObject/IntegerList/0", "SimpleObject/IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2, 3, 1 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void NestedCopy_FromList_ToNonList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.SimpleObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("SimpleObject/IntegerList/0", "SimpleObject/IntegerValue");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(targetObject);

            Assert.Equal(1, targetObject.SimpleObject.IntegerValue);
        }

        [Fact]
        public void NestedCopy_FromNonList_ToList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.SimpleObject = new SimpleObject()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("SimpleObject/IntegerValue", "SimpleObject/IntegerList/0");
            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Move()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.StringProperty = "A";
            targetObject.AnotherStringProperty = "B";

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("StringProperty", "AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("A", targetObject.AnotherStringProperty);

            var cont = targetObject as IDictionary<string, object>;
            cont.TryGetValue("StringProperty", out object valueFromDictionary);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void Move_ToNonExistingProperty()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.StringProperty = "A";

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("StringProperty", "AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("A", targetObject.AnotherStringProperty);

            var cont = targetObject as IDictionary<string, object>;
            cont.TryGetValue("StringProperty", out var valueFromDictionary);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void Move_ExpandoObjectProperty_ToTypedObject()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.StringProperty = "A";
            targetObject.SimpleDTO = new SimpleObject() { AnotherStringProperty = "B" };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("StringProperty", "SimpleDTO/AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("A", targetObject.SimpleDTO.AnotherStringProperty);

            var cont = targetObject as IDictionary<string, object>;
            cont.TryGetValue("StringProperty", out object valueFromDictionary);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void NestedMove()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.Nested = new SimpleObject()
            {
                StringProperty = "A",
                AnotherStringProperty = "B"
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("Nested/StringProperty", "Nested/AnotherStringProperty");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal("A", targetObject.Nested.AnotherStringProperty);
            Assert.Null(targetObject.Nested.StringProperty);
        }

        [Fact]
        public void NestedMoveInList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.Nested = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("Nested/IntegerList/0", "Nested/IntegerList/1");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 2, 1, 3 }, targetObject.Nested.IntegerList);
        }

        [Fact]
        public void NestedMove_FromList_ToEndOfList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.Nested = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("Nested/IntegerList/0", "Nested/IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);
            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 2, 3, 1 }, targetObject.Nested.IntegerList);
        }

        [Fact]
        public void NestedMove_FromList_ToNonList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.Nested = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("Nested/IntegerList/0", "Nested/IntegerValue");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 2, 3 }, targetObject.Nested.IntegerList);
            Assert.Equal(1, targetObject.Nested.IntegerValue);
        }

        [Fact]
        public void NestedMove_FromNonList_ToList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.Nested = new SimpleObject()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("Nested/IntegerValue", "Nested/IntegerList/0");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(0, targetObject.Nested.IntegerValue);
            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, targetObject.Nested.IntegerList);
        }

        [Fact]
        public void MoveToEndOfList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.IntegerValue = 5;
            targetObject.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("IntegerValue", "IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            var cont = targetObject as IDictionary<string, object>;
            cont.TryGetValue("IntegerValue", out var valueFromDictionary);
            Assert.Null(valueFromDictionary);

            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, targetObject.IntegerList);
        }

        [Fact]
        public void RemoveProperty_ShouldFail_IfItDoesntExist()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.Test = 1;

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("NonExisting");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(targetObject);
            });
            Assert.Equal(
                string.Format("The target location specified by path segment '{0}' was not found.", "NonExisting"),
                exception.Message);
        }

        [Fact]
        public void RemoveProperty()
        {
            dynamic obj = new ExpandoObject();
            obj.Test = 1;

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("Test");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(obj);

            var cont = obj as IDictionary<string, object>;
            cont.TryGetValue("Test", out object valueFromDictionary);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void RemoveProperty_MixedCase_ThrowsPathNotFoundException()
        {
            dynamic obj = new ExpandoObject();
            obj.Test = 1;

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("test");

            var serialized = JsonConvert.SerializeObject(patchDocument);
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
        public void RemoveNestedProperty()
        {
            dynamic obj = new ExpandoObject();
            obj.Test = new ExpandoObject();
            obj.Test.AnotherTest = "A";

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("Test");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(obj);

            var cont = obj as IDictionary<string, object>;
            cont.TryGetValue("Test", out object valueFromDictionary);
            Assert.Null(valueFromDictionary);
        }

        [Fact]
        public void RemoveNestedProperty_MixedCase_ThrowsPathNotFoundException()
        {
            dynamic obj = new ExpandoObject();
            obj.Test = new ExpandoObject();
            obj.Test.AnotherTest = "A";

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("test");

            var serialized = JsonConvert.SerializeObject(patchDocument);
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
        public void NestedRemoveFromList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.SimpleDTO = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("SimpleDTO/IntegerList/2");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2 }, targetObject.SimpleDTO.IntegerList);
        }

        [Fact]
        public void NestedRemove_FromList_InvalidPositionTooLarge()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.SimpleDTO = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("SimpleDTO/IntegerList/3");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                deserialized.ApplyTo(targetObject);
            });
            Assert.Equal(
               string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "3"),
                exception.Message);
        }

        [Fact]
        public void NestedRemove_FromList_InvalidPositionTooSmall()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.SimpleDTO = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("SimpleDTO/IntegerList/-1");

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
        public void NestedRemove_FromEndOfList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.SimpleDTO = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("SimpleDTO/IntegerList/-");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);
            Assert.Equal(new List<int>() { 1, 2 }, targetObject.SimpleDTO.IntegerList);
        }

        [Fact]
        public void ReplaceGuid()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.GuidValue = Guid.NewGuid();

            var newGuid = Guid.NewGuid();
            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("GuidValue", newGuid);

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(newGuid, targetObject.GuidValue);
        }

        [Fact]
        public void ReplaceNestedObject()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.SimpleDTO = new SimpleObject()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var newDTO = new SimpleObject()
            {
                DoubleValue = 1
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("SimpleDTO", newDTO);

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(1, targetObject.SimpleDTO.DoubleValue);
            Assert.Equal(0, targetObject.SimpleDTO.IntegerValue);
            Assert.Null(targetObject.SimpleDTO.IntegerList);
        }

        [Fact]
        public void ReplaceInList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("IntegerList/0", 5);

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 5, 2, 3 }, targetObject.IntegerList);
        }

        [Fact]
        public void ReplaceFullList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("IntegerList", new List<int>() { 4, 5, 6 });

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.IntegerList);
        }

        [Fact]
        public void ReplaceAdds_ToEndOfList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.SimpleDTOList = new List<SimpleObject>() {
                new SimpleObject() {
                    IntegerList = new List<int>(){1,2,3}
                }};

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("SimpleDTOList/0/IntegerList/-", 4);

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(4, targetObject.SimpleDTOList[0].IntegerList[2]);
        }

        [Fact]
        public void Replace_FullList_WithCollection()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("IntegerList", new Collection<int>() { 4, 5, 6 });

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.IntegerList);
        }

        [Fact]
        public void Replace_AtEndOfList()
        {
            dynamic targetObject = new ExpandoObject();
            targetObject.IntegerList = new List<int>() { 1, 2, 3 };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("IntegerList/-", 5);

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2, 5 }, targetObject.IntegerList);
        }
    }
}

