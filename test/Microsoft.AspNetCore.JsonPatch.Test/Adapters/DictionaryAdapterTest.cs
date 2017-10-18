﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Microsoft.AspNetCore.JsonPatch.Internal
{
    public class DictionaryAdapterTest
    {
        [Fact]
        public void Add_KeyWhichAlreadyExists_ReplacesExistingValue()
        {
            // Arrange
            var key = "Status";
            var dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
            dictionary[key] = 404;
            var dictionaryAdapter = new DictionaryAdapter<string, int>();
            var resolver = new DefaultContractResolver();

            // Act
            var addStatus = dictionaryAdapter.TryAdd(dictionary, key, resolver, 200, out var message);

            // Assert
            Assert.True(addStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
            Assert.Single(dictionary);
            Assert.Equal(200, dictionary[key]);
        }

        [Fact]
        public void Add_IntKeyWhichAlreadyExists_ReplacesExistingValue()
        {
            // Arrange
            var intKey = 1;
            var dictionary = new Dictionary<int, object>();
            dictionary[intKey] = "Mike";
            var dictionaryAdapter = new DictionaryAdapter<int, object>();
            var resolver = new DefaultContractResolver();

            // Act
            var addStatus = dictionaryAdapter.TryAdd(dictionary, intKey.ToString(), resolver, "James", out var message);

            // Assert
            Assert.True(addStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
            Assert.Single(dictionary);
            Assert.Equal("James", dictionary[intKey]);
        }

        [Fact]
        public void GetInvalidKey_ThrowsInvalidPathSegmentException()
        {
            // Arrange
            var dictionaryAdapter = new DictionaryAdapter<int, object>();
            var resolver = new DefaultContractResolver();
            var key = 1;
            var dictionary = new Dictionary<int, object>();

            // Act
            var addStatus = dictionaryAdapter.TryAdd(dictionary, key.ToString(), resolver, "James", out var message);

            // Assert
            Assert.True(addStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
            Assert.Single(dictionary);
            Assert.Equal("James", dictionary[key]);

            // Act
            var guidKey = new Guid();
            var getStatus = dictionaryAdapter.TryGet(dictionary, guidKey.ToString(), resolver, out var outValue, out message);

            // Assert
            Assert.False(getStatus);
            Assert.Equal(
                string.Format("The provided path segment '{0}' cannot be converted to the target type.", guidKey.ToString()),
                message);
            Assert.Null(outValue);
        }

        [Fact]
        public void Get_UsingCaseSensitiveKey_FailureScenario()
        {
            // Arrange
            var dictionaryAdapter = new DictionaryAdapter<string, object>();
            var resolver = new DefaultContractResolver();
            var nameKey = "Name";
            var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);

            // Act
            var addStatus = dictionaryAdapter.TryAdd(dictionary, nameKey, resolver, "James", out var message);

            // Assert
            Assert.True(addStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
            Assert.Single(dictionary);
            Assert.Equal("James", dictionary[nameKey]);

            // Act
            var getStatus = dictionaryAdapter.TryGet(dictionary, nameKey.ToUpper(), resolver, out var outValue, out message);

            // Assert
            Assert.False(getStatus);
            Assert.Equal(
                string.Format("The target location specified by path segment '{0}' was not found.", nameKey.ToUpper()),
                message);
            Assert.Null(outValue);
        }

        [Fact]
        public void Get_UsingCaseSensitiveKey_SuccessScenario()
        {
            // Arrange
            var dictionaryAdapter = new DictionaryAdapter<string, object>();
            var resolver = new DefaultContractResolver();
            var nameKey = "Name";
            var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);

            // Act
            var addStatus = dictionaryAdapter.TryAdd(dictionary, nameKey, resolver, "James", out var message);

            // Assert
            Assert.True(addStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
            Assert.Single(dictionary);
            Assert.Equal("James", dictionary[nameKey]);

            // Act
            addStatus = dictionaryAdapter.TryGet(dictionary, nameKey, resolver, out var outValue, out message);

            // Assert
            Assert.True(addStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
            Assert.Equal("James", outValue?.ToString());
        }

        [Fact]
        public void ReplacingExistingItem()
        {
            // Arrange
            var nameKey = "Name";
            var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);
            dictionary.Add(nameKey, "Mike");
            var dictionaryAdapter = new DictionaryAdapter<string, object>();
            var resolver = new DefaultContractResolver();

            // Act
            var replaceStatus = dictionaryAdapter.TryReplace(dictionary, nameKey, resolver, "James", out var message);

            // Assert
            Assert.True(replaceStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
            Assert.Single(dictionary);
            Assert.Equal("James", dictionary[nameKey]);
        }

        [Fact]
        public void ReplacingExistingItem_WithGuidKey()
        {
            // Arrange
            var guidKey = new Guid();
            var dictionary = new Dictionary<Guid, object>();
            dictionary.Add(guidKey, "Mike");
            var dictionaryAdapter = new DictionaryAdapter<Guid, object>();
            var resolver = new DefaultContractResolver();

            // Act
            var replaceStatus = dictionaryAdapter.TryReplace(dictionary, guidKey.ToString(), resolver, "James", out var message);

            // Assert
            Assert.True(replaceStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
            Assert.Single(dictionary);
            Assert.Equal("James", dictionary[guidKey]);
        }

        [Fact]
        public void ReplacingWithInvalidValue_ThrowsInvalidValueForPropertyException()
        {
            // Arrange
            var guidKey = new Guid();
            var dictionary = new Dictionary<Guid, int>();
            dictionary.Add(guidKey, 5);
            var dictionaryAdapter = new DictionaryAdapter<Guid, int>();
            var resolver = new DefaultContractResolver();

            // Act
            var replaceStatus = dictionaryAdapter.TryReplace(dictionary, guidKey.ToString(), resolver, "test", out var message);

            // Assert
            Assert.False(replaceStatus);
            Assert.Equal(
                string.Format("The value '{0}' is invalid for target location.", "test"),
                message);
            Assert.Equal(5, dictionary[guidKey]);
        }

        [Fact]
        public void Replace_NonExistingKey_Fails()
        {
            // Arrange
            var nameKey = "Name";
            var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);
            var dictionaryAdapter = new DictionaryAdapter<string, object>();
            var resolver = new DefaultContractResolver();

            // Act
            var replaceStatus = dictionaryAdapter.TryReplace(dictionary, nameKey, resolver, "Mike", out var message);

            // Assert
            Assert.False(replaceStatus);
            Assert.Equal(
                string.Format("The target location specified by path segment '{0}' was not found.", nameKey),
                message);
            Assert.Empty(dictionary);
        }

        [Fact]
        public void Remove_NonExistingKey_Fails()
        {
            // Arrange
            var nameKey = "Name";
            var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);
            var dictionaryAdapter = new DictionaryAdapter<string, object>();
            var resolver = new DefaultContractResolver();

            // Act
            var removeStatus = dictionaryAdapter.TryRemove(dictionary, nameKey, resolver, out var message);

            // Assert
            Assert.False(removeStatus);
            Assert.Equal(
                string.Format("The target location specified by path segment '{0}' was not found.", nameKey),
                message);
            Assert.Empty(dictionary);
        }

        [Fact]
        public void Remove_RemovesFromDictionary()
        {
            // Arrange
            var nameKey = "Name";
            var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);
            dictionary[nameKey] = "James";
            var dictionaryAdapter = new DictionaryAdapter<string, object>();
            var resolver = new DefaultContractResolver();

            // Act
            var removeStatus = dictionaryAdapter.TryRemove(dictionary, nameKey, resolver, out var message);

            //Assert
            Assert.True(removeStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
            Assert.Empty(dictionary);
        }

        [Fact]
        public void Remove_RemovesFromDictionary_WithUriKey()
        {
            // Arrange
            var uriKey = new Uri("http://www.test.com/name");
            var dictionary = new Dictionary<Uri, object>();
            dictionary[uriKey] = "James";
            var dictionaryAdapter = new DictionaryAdapter<Uri, object>();
            var resolver = new DefaultContractResolver();

            // Act
            var removeStatus = dictionaryAdapter.TryRemove(dictionary, uriKey.ToString(), resolver, out var message);

            //Assert
            Assert.True(removeStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
            Assert.Empty(dictionary);
        }

        [Fact]
        public void Test_DoesNotThrowException_IfTestIsSuccessful()
        {
            // Arrange
            var key = "Name";
            var dictionary = new Dictionary<string, object>();
            dictionary[key] = "James";
            var dictionaryAdapter = new DictionaryAdapter<string, object>();
            var resolver = new DefaultContractResolver();

            // Act
            var testStatus = dictionaryAdapter.TryTest(dictionary, key, resolver, "James", out var message);

            //Assert
            Assert.True(testStatus);
            Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        }

        [Fact]
        public void Test_ThrowsJsonPatchException_IfTestFails()
        {
            // Arrange
            var key = "Name";
            var dictionary = new Dictionary<string, object>();
            dictionary[key] = "James";
            var dictionaryAdapter = new DictionaryAdapter<string, object>();
            var resolver = new DefaultContractResolver();
            var expectedErrorMessage = "The current value 'James' at path 'Name' is not equal to the test value 'John'.";

            // Act
            var testStatus = dictionaryAdapter.TryTest(dictionary, key, resolver, "John", out var errorMessage);

            //Assert
            Assert.False(testStatus);
            Assert.Equal(expectedErrorMessage, errorMessage);
        }
    }
}
