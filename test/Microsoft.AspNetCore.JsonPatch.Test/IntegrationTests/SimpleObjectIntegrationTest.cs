// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch
{
    public class SimpleObjectIntegrationTest
    {
        [Fact]
        public void AddToListInList()
        {
            // Arrange
            var simpleObject = new SimpleObjectWithNestedObject()
            {
                SimpleObjectList = new List<SimpleObject>()
                {
                     new SimpleObject()
                     {
                         IntegerList = new List<int>() { 1, 2, 3 }
                     }
                }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("SimpleObjectList/0/IntegerList/0", 4);

            // Act
            patchDocument.ApplyTo(simpleObject);

            // Assert
            Assert.Equal(new List<int>() { 4, 1, 2, 3 }, simpleObject.SimpleObjectList[0].IntegerList);
        }

        [Theory]
        [InlineData("20")]
        [InlineData("-1")]
        public void AddToListInList_InvalidPosition(string position)
        {
            // Arrange
            var simpleObject = new SimpleObjectWithNestedObject()
            {
                SimpleObjectList = new List<SimpleObject>()
                {
                     new SimpleObject()
                     {
                        IntegerList = new List<int>() { 1, 2, 3 }
                     }
                }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Add("SimpleObjectList/0/IntegerList/" + position, 4);

            // Act
            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                patchDocument.ApplyTo(simpleObject);
            });

            // Assert
            Assert.Equal(
                string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", position),
                exception.Message);
        }

        [Fact]
        public void Copy()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                StringProperty = "A",
                AnotherStringProperty = "B"
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("StringProperty", "AnotherStringProperty");

            // Act
            patchDocument.ApplyTo(targetObject);

            // Assert
            Assert.Equal("A", targetObject.AnotherStringProperty);
        }

        [Fact]
        public void CopyInList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("IntegerList/0", "IntegerList/1");

            // Act
            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 1, 2, 3 }, targetObject.IntegerList);
        }

        [Fact]
        public void CopyFromListToEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("IntegerList/0", "IntegerList/-");

            // Act
            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2, 3, 1 }, targetObject.IntegerList);
        }

        [Fact]
        public void CopyFromListToNonList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("IntegerList/0", "IntegerValue");

            // Act
            patchDocument.ApplyTo(targetObject);

            Assert.Equal(1, targetObject.IntegerValue);
        }

        [Fact]
        public void CopyFromNonListToList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("IntegerValue", "IntegerList/0");

            // Act
            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, targetObject.IntegerList);
        }

        [Fact]
        public void CopyToEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("IntegerValue", "IntegerList/-");

            // Act
            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, targetObject.IntegerList);
        }

        [Fact]
        public void NestedCopy()
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

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("SimpleObject/StringProperty", "SimpleObject/AnotherStringProperty");

            // Act
            patchDocument.ApplyTo(targetObject);

            Assert.Equal("A", targetObject.SimpleObject.AnotherStringProperty);
        }

        [Fact]
        public void NestedCopyInList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("SimpleObject/IntegerList/0", "SimpleObject/IntegerList/1");

            // Act
            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void NestedCopyFromListToEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("SimpleObject/IntegerList/0", "SimpleObject/IntegerList/-");

            // Act
            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2, 3, 1 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void NestedCopyFromListToNonList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("SimpleObject/IntegerList/0", "SimpleObject/IntegerValue");

            // Act
            patchDocument.ApplyTo(targetObject);

            Assert.Equal(1, targetObject.SimpleObject.IntegerValue);
        }

        [Fact]
        public void NestedCopyFromNonListToList()
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

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("SimpleObject/IntegerValue", "SimpleObject/IntegerList/0");

            // Act
            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void NestedCopyToEndOfList()
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

            var patchDocument = new JsonPatchDocument();
            patchDocument.Copy("SimpleObject/IntegerValue", "SimpleObject/IntegerList/-");

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void Move()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                StringProperty = "A",
                AnotherStringProperty = "B"
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("StringProperty", "AnotherStringProperty");

            patchDocument.ApplyTo(targetObject);

            Assert.Equal("A", targetObject.AnotherStringProperty);
            Assert.Null(targetObject.StringProperty);
        }

        [Fact]
        public void MoveInList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("IntegerList/0", "IntegerList/1");

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 2, 1, 3 }, targetObject.IntegerList);
        }

        [Fact]
        public void MoveFromListToEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("IntegerList/0", "IntegerList/-");

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 2, 3, 1 }, targetObject.IntegerList);
        }

        [Fact]
        public void MoveFromListToNonList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("IntegerList/0", "IntegerValue");

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 2, 3 }, targetObject.IntegerList);
            Assert.Equal(1, targetObject.IntegerValue);
        }

        [Fact]
        public void MoveFromNonListToList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("IntegerValue", "IntegerList/0");

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(0, targetObject.IntegerValue);
            Assert.Equal(new List<int>() { 5, 1, 2, 3 }, targetObject.IntegerList);
        }

        [Fact]
        public void MoveToEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerValue = 5,
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Move("IntegerValue", "IntegerList/-");

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(0, targetObject.IntegerValue);
            Assert.Equal(new List<int>() { 1, 2, 3, 5 }, targetObject.IntegerList);
        }

        [Fact]
        public void Remove()
        {
            var targetObject = new SimpleObject()
            {
                StringProperty = "A"
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("StringProperty");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Null(targetObject.StringProperty);
        }

        [Fact]
        public void RemoveFromList()
        {
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("IntegerList/2");

            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2 }, targetObject.IntegerList);
        }

        [Theory]
        [InlineData("3")]
        [InlineData("-1")]
        public void RemoveFromList_InvalidPosition(string position)
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("IntegerList/" + position);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                patchDocument.ApplyTo(targetObject);
            });
            Assert.Equal(
               string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", position),
                exception.Message);
        }

        [Fact]
        public void RemoveFromEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("IntegerList/-");

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2 }, targetObject.IntegerList);
        }

        [Fact]
        public void NestedRemove()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    StringProperty = "A"
                }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("SimpleObject/StringProperty");

            patchDocument.ApplyTo(targetObject);

            Assert.Null(targetObject.SimpleObject.StringProperty);
        }

        [Fact]
        public void NestedRemoveFromList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("SimpleObject/IntegerList/2");

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2 }, targetObject.SimpleObject.IntegerList);
        }

        [Theory]
        [InlineData("20")]
        [InlineData("-1")]
        public void NestedRemove_FromList_InvalidPosition(string position)
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("SimpleObject/IntegerList/" + position);

            var exception = Assert.Throws<JsonPatchException>(() =>
            {
                patchDocument.ApplyTo(targetObject);
            });
            Assert.Equal(
               string.Format("The index value provided by path segment '{0}' is out of bounds of the array size.", position),
               exception.Message);
        }

        [Fact]
        public void NestedRemove_FromEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObjectWithNestedObject()
            {
                SimpleObject = new SimpleObject()
                {
                    IntegerList = new List<int>() { 1, 2, 3 }
                }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Remove("SimpleObject/IntegerList/-");

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2 }, targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void ReplaceGuid()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                GuidValue = Guid.NewGuid()
            };

            var newGuid = Guid.NewGuid();
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("GuidValue", newGuid);

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(newGuid, targetObject.GuidValue);
        }

        [Fact]
        public void SerializeAndReplaceNestedObject()
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

            var newDTO = new SimpleObject()
            {
                DoubleValue = 1
            };

            // create patch
            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("SimpleObject", newDTO);

            // serialize & deserialize
            var serialized = JsonConvert.SerializeObject(patchDocument);
            var deserialized = JsonConvert.DeserializeObject<JsonPatchDocument>(serialized);

            deserialized.ApplyTo(targetObject);

            Assert.Equal(1, targetObject.SimpleObject.DoubleValue);
            Assert.Equal(0, targetObject.SimpleObject.IntegerValue);
            Assert.Null(targetObject.SimpleObject.IntegerList);
        }

        [Fact]
        public void ReplaceInList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("IntegerList/0", 5);

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 5, 2, 3 }, targetObject.IntegerList);
        }

        [Fact]
        public void ReplaceFullList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("IntegerList", new List<int>() { 4, 5, 6 });

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.IntegerList);
        }

        [Fact]
        public void ReplaceInListInList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                SimpleObjectList = new List<SimpleObject>() {
                new SimpleObject() {
                    IntegerList = new List<int>(){1,2,3}
                }}
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("SimpleObjectList/0/IntegerList/0", 4);

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(4, targetObject.SimpleObjectList.First().IntegerList.First());
        }

        [Fact]
        public void ReplaceFullList_FromEnumerable()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("IntegerList", new List<int>() { 4, 5, 6 });

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.IntegerList);
        }

        [Fact]
        public void ReplaceFullList_WithCollection()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("IntegerList", new Collection<int>() { 4, 5, 6 });

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 4, 5, 6 }, targetObject.IntegerList);
        }

        [Fact]
        public void ReplaceAtEndOfList()
        {
            // Arrange
            var targetObject = new SimpleObject()
            {
                IntegerList = new List<int>() { 1, 2, 3 }
            };

            var patchDocument = new JsonPatchDocument();
            patchDocument.Replace("IntegerList/-", 5);

            patchDocument.ApplyTo(targetObject);

            Assert.Equal(new List<int>() { 1, 2, 5 }, targetObject.IntegerList);
        }
    }
}
