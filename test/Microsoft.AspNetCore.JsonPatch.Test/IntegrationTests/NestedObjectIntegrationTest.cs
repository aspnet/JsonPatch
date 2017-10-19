// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch
{
    public class NestedObjectIntegrationTest
    {
        [Fact]
        public void ReplaceProperty()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                IntegerValue = 1
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.NestedObject.StringProperty, "B");

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.NestedObject.StringProperty);
        }

        [Fact]
        public void ReplaceProperty_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                IntegerValue = 1
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.NestedObject.StringProperty, "B");

            // serialize & deserialize
            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.NestedObject.StringProperty);
        }

        [Fact]
        public void ReplaceNestedObject()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                IntegerValue = 1
            };

            var newNested = new NestedObject() { StringProperty = "B" };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.NestedObject, newNested);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.NestedObject.StringProperty);
        }

        [Fact]
        public void ReplaceNestedObject_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                IntegerValue = 1
            };

            var newNested = new NestedObject() { StringProperty = "B" };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.NestedObject, newNested);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.NestedObject.StringProperty);
        }

        [Fact]
        public void TestPropertyInNestedObject()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                NestedObject = new NestedObject() { StringProperty = "A"}
            };

            // create patch
            var patchDocument = new JsonPatchDocument<NestedObject>();
            patchDocument.Test(o => o.StringProperty, "A");

            // Act
            patchDocument.ApplyTo(targetObject.NestedObject);

            // Assert
            Assert.Equal("A", targetObject.NestedObject.StringProperty);
        }

        [Fact]
        public void TestNestedObject()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                NestedObject = new NestedObject() { StringProperty = "B"}
            };

            var testNested = new NestedObject() { StringProperty = "B" };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Test(o => o.NestedObject, testNested);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.NestedObject.StringProperty);
        }

        [Fact]
        public void TestInList_InvalidPositionTooSmall()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Test(o => o.SimpleObject.IntegerList, 4, -1);

            // Act & Assert
            var exception = Assert.Throws<JsonPatchException>(() => { patchDocument.ApplyTo(targetObject); });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "-1"),
                exception.Message);
        }

        [Fact]
        public void AddReplaces_ExistingProperty()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A"
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObject.StringProperty, "B");

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.SimpleObject.StringProperty);
        }

        [Fact]
        public void AddReplaces_ExistingProperty_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A"
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObject.StringProperty, "B");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.SimpleObject.StringProperty);
        }

        [Fact]
        public void AddToList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObject.IntegerList, 4, 0);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void AddToList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObject.IntegerList, 4, 0);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void AddToIntegerIList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerIList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => (List<int>)o.SimpleObject.IntegerIList, 4, 0);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, targetObject.SimpleObject.IntegerIList);
        }

        [Fact]
        public void AddToIntegerIList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerIList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => (List<int>)o.SimpleObject.IntegerIList, 4, 0);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, targetObject.SimpleObject.IntegerIList);
        }

        [Fact]
        public void AddToNestedIntegerIList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObjectIList = new List<SimpleObject>
                {
                    new SimpleObject
                    {
                        IntegerIList = new List<int>() { 1, 2, 3 }
                    }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => (List<int>)o.SimpleObjectIList[0].IntegerIList, 4, 0);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, targetObject.SimpleObjectIList[0].IntegerIList);
        }

        [Fact]
        public void AddToNestedIntegerIList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObjectIList = new List<SimpleObject>
                {
                    new SimpleObject
                    {
                        IntegerIList = new List<int>() { 1, 2, 3 }
                    }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => (List<int>)o.SimpleObjectIList[0].IntegerIList, 4, 0);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, targetObject.SimpleObjectIList[0].IntegerIList);
        }

        [Fact]
        public void AddToComplextTypeList_SpecifyIndex()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObjectList = new List<SimpleObject>()
                {
                    new SimpleObject
                    {
                        StringProperty = "String1"
                    },
                    new SimpleObject
                    {
                        StringProperty = "String2"
                    }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObjectList[0].StringProperty, "ChangedString1");

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("ChangedString1", targetObject.SimpleObjectList[0].StringProperty);
        }

        [Fact]
        public void AddToComplextTypeList_SpecifyIndex_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObjectList = new List<SimpleObject>()
                {
                    new SimpleObject
                    {
                        StringProperty = "String1"
                    },
                    new SimpleObject
                    {
                        StringProperty = "String2"
                    }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObjectList[0].StringProperty, "ChangedString1");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("ChangedString1", targetObject.SimpleObjectList[0].StringProperty);
        }

        [Fact]
        public void AddToList_InvalidPositionTooLarge()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObject.IntegerList, 4, 4);

            // Act & Assert
            var exception = Assert.Throws<JsonPatchException>(() => { patchDocument.ApplyTo(targetObject); });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "4"),
                exception.Message);

        }

        [Fact]
        public void AddToList_InvalidPositionTooLarge_LogsError()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObject.IntegerList, 4, 4);

            var logger = new TestErrorLogger<SimpleObjectWithNestedObject>();

            patchDocument.ApplyTo(targetObject, logger.LogErrorMessage);


            //Assert
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "4"),
                logger.ErrorMessage);

        }

        [Fact]
        public void AddToList_InvalidPositionTooSmall_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObject.IntegerList, 4, -1);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act & Assert
            var exception = Assert.Throws<JsonPatchException>(() =>
                {
                    deserialized.ApplyTo(targetObject);
                });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "-1"),
                exception.Message);
        }

        [Fact]
        public void AddToListAppend()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObject.IntegerList, 4);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2, 3, 4 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void AddToListAppend_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Add(o => o.SimpleObject.IntegerList, 4);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2, 3, 4 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Remove()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A"
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Remove(o => o.SimpleObject.StringProperty);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Null(targetObject.SimpleObject.StringProperty);
        }

        [Fact]
        public void Remove_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A"
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Remove(o => o.SimpleObject.StringProperty);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Null(targetObject.SimpleObject.StringProperty);
        }

        [Fact]
        public void RemoveFromList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Remove(o => o.SimpleObject.IntegerList, 2);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void RemoveFromList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Remove(o => o.SimpleObject.IntegerList, 2);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void RemoveFromList_InvalidPositionTooLarge()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Remove(o => o.SimpleObject.IntegerList, 3);

            // Act & Assert
            var exception = Assert.Throws<JsonPatchException>(() => { patchDocument.ApplyTo(targetObject); });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "3"),
                exception.Message);
        }

        [Fact]
        public void RemoveFromList_InvalidPositionTooLarge_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Remove(o => o.SimpleObject.IntegerList, 3);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act & Assert
            var exception = Assert.Throws<JsonPatchException>(() =>
                {
                    deserialized.ApplyTo(targetObject);
                });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "3"),
                exception.Message);
        }

        [Fact]
        public void RemoveFromListInvalidPositionTooSmall()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Remove(o => o.SimpleObject.IntegerList, -1);

            // Act & Assert
            var exception = Assert.Throws<JsonPatchException>(() => { patchDocument.ApplyTo(targetObject); });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "-1"),
                exception.Message);
        }
        
        [Fact]
        public void RemoveFromListInvalidPositionTooSmall_LogsError()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Remove(o => o.SimpleObject.IntegerList, -1);

            var logger = new TestErrorLogger<SimpleObjectWithNestedObject>();


            patchDocument.ApplyTo(targetObject, logger.LogErrorMessage);

            // Assert
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "-1"),
                logger.ErrorMessage);
        }

        [Fact]
        public void Remove_FromEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Remove<int>(o => o.SimpleObject.IntegerList);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Remove_FromEndOfList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Remove<int>(o => o.SimpleObject.IntegerList);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Replace()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A",
                    DecimalValue = 10
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.StringProperty, "B");
            patchDocument.Replace(o => o.SimpleObject.DecimalValue, 12);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.SimpleObject.StringProperty);
            Assert.Equal(12, targetObject.SimpleObject.DecimalValue);
        }

        [Fact]
        public void Replace_DTOWithNullCheck()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObjectWithNullCheck()
            {
                SimpleObjectWithNullCheck = new SimpleObjectWithNullCheck()
                {
                    StringProperty = "A"
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObjectWithNullCheck>();
            patchDocument.Replace(o => o.SimpleObjectWithNullCheck.StringProperty, "B");

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.SimpleObjectWithNullCheck.StringProperty);
        }

        [Fact]
        public void Replace_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A",
                    DecimalValue = 10
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.StringProperty, "B");
            patchDocument.Replace(o => o.SimpleObject.DecimalValue, 12);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.SimpleObject.StringProperty);
            Assert.Equal(12, targetObject.SimpleObject.DecimalValue);
        }

        [Fact]
        public void SerializationTests()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A",
                    DecimalValue = 10,
                    DoubleValue = 10,
                    FloatValue = 10,
                    IntegerValue = 10
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.StringProperty, "B");
            patchDocument.Replace(o => o.SimpleObject.DecimalValue, 12);
            patchDocument.Replace(o => o.SimpleObject.DoubleValue, 12);
            patchDocument.Replace(o => o.SimpleObject.FloatValue, 12);
            patchDocument.Replace(o => o.SimpleObject.IntegerValue, 12);

            // serialize & deserialize
            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserizalized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserizalized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("B", targetObject.SimpleObject.StringProperty);
            Assert.Equal(12, targetObject.SimpleObject.DecimalValue);
            Assert.Equal(12, targetObject.SimpleObject.DoubleValue);
            Assert.Equal(12, targetObject.SimpleObject.FloatValue);
            Assert.Equal(12, targetObject.SimpleObject.IntegerValue);
        }

        [Fact]
        public void ReplaceInList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.IntegerList, 5, 0);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 5, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void ReplaceInList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.IntegerList, 5, 0);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 5, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void ReplaceFullList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.IntegerList, new List<int>() { 4, 5, 6 });

            // Act
            patchDocument.ApplyTo(targetObject);

            // Arrange
            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void ReplaceFullList_WithSerialiation()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.IntegerList, new List<int>() { 4, 5, 6 });

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Replace_FullListFromEnumerable()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace<IEnumerable<int>>(o => o.SimpleObject.IntegerList, new List<int>() { 4, 5, 6 });

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Replace_FullListFromEnumerable_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace<IEnumerable<int>>(o => o.SimpleObject.IntegerList, new List<int>() { 4, 5, 6 });

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Replace_FullListWithCollection()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace<IEnumerable<int>>(o => o.SimpleObject.IntegerList, new Collection<int>() { 4, 5, 6 });

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Replace_FullListWithCollection_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace<IEnumerable<int>>(o => o.SimpleObject.IntegerList, new Collection<int>() { 4, 5, 6 });

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Replace_AtEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.IntegerList, 5);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2, 5 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Replace_AtEndOfList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.IntegerList, 5);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2, 5 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Replace_InList_InvalidInvalidPositionTooLarge()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.IntegerList, 5, 3);

            // Act & Assert
            var exception = Assert.Throws<JsonPatchException>(() => { patchDocument.ApplyTo(targetObject); });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "3"),
                exception.Message);
        }

        [Fact]
        public void Replace_InList_InvalidPositionTooSmall()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.IntegerList, 5, -1);

            // Act & Assert
            var exception = Assert.Throws<JsonPatchException>(() => { patchDocument.ApplyTo(targetObject); });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "-1"),
                exception.Message);
        }

        [Fact]
        public void Replace_InList_InvalidPositionTooSmall_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.IntegerList, 5, -1);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act & Assert
            var exception = Assert.Throws<JsonPatchException>(() => { deserialized.ApplyTo(targetObject); });
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "-1"),
                exception.Message);
        }

        [Fact]
        public void Replace_InList_InvalidPositionTooSmall_LogsError()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Replace(o => o.SimpleObject.IntegerList, 5, -1);

            var logger = new TestErrorLogger<SimpleObjectWithNestedObject>();


            patchDocument.ApplyTo(targetObject, logger.LogErrorMessage);


            // Assert
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", "-1"),
                logger.ErrorMessage);
        }

        [Fact]
        public void Copy()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A",
                    AnotherStringProperty = "B"
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.StringProperty, o => o.SimpleObject.AnotherStringProperty);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("A", targetObject.SimpleObject.AnotherStringProperty);
        }

        [Fact]
        public void Copy_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A",
                    AnotherStringProperty = "B"
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.StringProperty, o => o.SimpleObject.AnotherStringProperty);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("A", targetObject.SimpleObject.AnotherStringProperty);
        }

        [Fact]
        public void CopyInList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerList, 1);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void CopyInList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerList, 1);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void CopyFromList_ToEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerList);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2, 3, 1 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void CopyFromList_ToEndOfList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerList);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2, 3, 1 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void CopyFromList_ToNonList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerValue);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(1, targetObject.SimpleObject.IntegerValue);
        }

        [Fact]
        public void CopyFromList_ToNonList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerValue);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(1, targetObject.SimpleObject.IntegerValue);
        }

        [Fact]
        public void CopyFromNonList_ToList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerValue = 5,
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.IntegerValue, o => o.SimpleObject.IntegerList, 0);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void CopyFromNonList_ToList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerValue = 5,
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.IntegerValue, o => o.SimpleObject.IntegerList, 0);
            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void CopyToEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerValue = 5,
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.IntegerValue, o => o.SimpleObject.IntegerList);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void CopyToEndOfList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerValue = 5,
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.SimpleObject.IntegerValue, o => o.SimpleObject.IntegerList);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Copy_DeepClonesObject()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A",
                    AnotherStringProperty = "B"
                },
                InheritedObject = new InheritedObject()
                {
                    StringProperty = "C",
                    AnotherStringProperty = "D"
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.InheritedObject, o => o.SimpleObject);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("C", targetObject.SimpleObject.StringProperty);
            Assert.Equal("D", targetObject.SimpleObject.AnotherStringProperty);
            Assert.Equal("C", targetObject.InheritedObject.StringProperty);
            Assert.Equal("D", targetObject.InheritedObject.AnotherStringProperty);
            Assert.NotSame(targetObject.SimpleObject.StringProperty, targetObject.InheritedObject.StringProperty);
        }

        [Fact]
        public void Copy_KeepsObjectType()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject(),
                InheritedObject = new InheritedObject()
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.InheritedObject, o => o.SimpleObject);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(typeof(InheritedObject), targetObject.SimpleObject.GetType());
        }

        [Fact]
        public void Copy_BreaksObjectReference()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject(),
                InheritedObject = new InheritedObject()
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Copy(o => o.InheritedObject, o => o.SimpleObject);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.NotSame(targetObject.SimpleObject, targetObject.InheritedObject);
        }

        [Fact]
        public void Move()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A",
                    AnotherStringProperty = "B"
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.StringProperty, o => o.SimpleObject.AnotherStringProperty);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("A", targetObject.SimpleObject.AnotherStringProperty);
            Assert.Null(targetObject.SimpleObject.StringProperty);
        }

        [Fact]
        public void Move_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A",
                    AnotherStringProperty = "B"
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.StringProperty, o => o.SimpleObject.AnotherStringProperty);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("A", targetObject.SimpleObject.AnotherStringProperty);
            Assert.Null(targetObject.SimpleObject.StringProperty);
        }

        [Fact]
        public void Move_KeepsObjectReference()
        {
            // Arrange
            var sDto = new SimpleObject()
            {
                StringProperty = "A",
                AnotherStringProperty = "B"
            };
            var iDto = new InheritedObject()
            {
                StringProperty = "C",
                AnotherStringProperty = "D"
            };
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = sDto,
                InheritedObject = iDto
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.InheritedObject, o => o.SimpleObject);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("C", targetObject.SimpleObject.StringProperty);
            Assert.Equal("D", targetObject.SimpleObject.AnotherStringProperty);
            Assert.Same(iDto, targetObject.SimpleObject);
            Assert.Null(targetObject.InheritedObject);
        }

        [Fact]
        public void Move_KeepsObjectReference_WithSerialization()
        {
            // Arrange
            var sDto = new SimpleObject()
            {
                StringProperty = "A",
                AnotherStringProperty = "B"
            };
            var iDto = new InheritedObject()
            {
                StringProperty = "C",
                AnotherStringProperty = "D"
            };
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = sDto,
                InheritedObject = iDto
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.InheritedObject, o => o.SimpleObject);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal("C", targetObject.SimpleObject.StringProperty);
            Assert.Equal("D", targetObject.SimpleObject.AnotherStringProperty);
            Assert.Same(iDto, targetObject.SimpleObject);
            Assert.Null(targetObject.InheritedObject);
        }

        [Fact]
        public void MoveInList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerList, 1);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 2, 1, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void MoveInList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerList, 1);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 2, 1, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Move_KeepsObjectReferenceInList()
        {
            // Arrange
            var sDto1 = new SimpleObject() { IntegerValue = 1 };
            var sDto2 = new SimpleObject() { IntegerValue = 2 };
            var sDto3 = new SimpleObject() { IntegerValue = 3 };
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObjectList = new List<SimpleObject>() {
                    sDto1,
                    sDto2,
                    sDto3
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObjectList, 0, o => o.SimpleObjectList, 1);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<SimpleObject>() { sDto2, sDto1, sDto3 }, targetObject.SimpleObjectList);
            Assert.Equal(2, targetObject.SimpleObjectList[0].IntegerValue);
            Assert.Equal(1, targetObject.SimpleObjectList[1].IntegerValue);
            Assert.Same(sDto2, targetObject.SimpleObjectList[0]);
            Assert.Same(sDto1, targetObject.SimpleObjectList[1]);
        }

        [Fact]
        public void Move_KeepsObjectReferenceInList_WithSerialization()
        {
            // Arrange
            var sDto1 = new SimpleObject() { IntegerValue = 1 };
            var sDto2 = new SimpleObject() { IntegerValue = 2 };
            var sDto3 = new SimpleObject() { IntegerValue = 3 };
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObjectList = new List<SimpleObject>() {
                    sDto1,
                    sDto2,
                    sDto3
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObjectList, 0, o => o.SimpleObjectList, 1);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<SimpleObject>() { sDto2, sDto1, sDto3 }, targetObject.SimpleObjectList);
            Assert.Equal(2, targetObject.SimpleObjectList[0].IntegerValue);
            Assert.Equal(1, targetObject.SimpleObjectList[1].IntegerValue);
            Assert.Same(sDto2, targetObject.SimpleObjectList[0]);
            Assert.Same(sDto1, targetObject.SimpleObjectList[1]);
        }

        [Fact]
        public void MoveFromList_ToEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerList);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 2, 3, 1 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void MoveFromList_ToEndOfList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerList);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 2, 3, 1 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void MoveFromList_ToNonList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerValue);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 2, 3 }, targetObject.SimpleObject.IntegerList);
            Assert.Equal(1, targetObject.SimpleObject.IntegerValue);
        }

        [Fact]
        public void MoveFromList_ToNonList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerList, 0, o => o.SimpleObject.IntegerValue);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 2, 3 }, targetObject.SimpleObject.IntegerList);
            Assert.Equal(1, targetObject.SimpleObject.IntegerValue);
        }

        [Fact]
        public void MoveFromList_ToNonList_BetweenHierarchy()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerList, 0, o => o.IntegerValue);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 2, 3 }, targetObject.SimpleObject.IntegerList);
            Assert.Equal(1, targetObject.IntegerValue);
        }

        [Fact]
        public void MoveFromList_ToNonList_BetweenHierarchy_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerList, 0, o => o.IntegerValue);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(new List<int>() { 2, 3 }, targetObject.SimpleObject.IntegerList);
            Assert.Equal(1, targetObject.IntegerValue);
        }

        [Fact]
        public void MoveFromNonList_ToList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerValue = 5,
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerValue, o => o.SimpleObject.IntegerList, 0);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(0, targetObject.IntegerValue);
            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void MoveFromNonList_ToList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerValue = 5,
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerValue, o => o.SimpleObject.IntegerList, 0);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(0, targetObject.IntegerValue);
            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void MoveToEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerValue = 5,
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerValue, o => o.SimpleObject.IntegerList);

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal(0, targetObject.IntegerValue);
            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void MoveToEndOfList_WithSerialization()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerValue = 5,
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            // create patch
            var patchDocument = new JsonPatchDocument<SimpleObjectWithNestedObject>();
            patchDocument.Move(o => o.SimpleObject.IntegerValue, o => o.SimpleObject.IntegerList);

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument<SimpleObjectWithNestedObject>>(serialized);

            // Act
            deserialized.ApplyTo(targetObject);

            // Assert
            Assert.Equal(0, targetObject.IntegerValue);
            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, targetObject.SimpleObject.IntegerList);
        }
    }
}
