// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch.Exceptions;

namespace Microsoft.AspNetCore.JsonPatch.Helpers
{
    internal class DictionaryWrapper<TKey, TValue> : IDictionaryWrapper
    {
        private readonly IDictionary<TKey, TValue> _targetDictionary;
        private readonly bool useDynamic;

        public DictionaryWrapper(IDictionary<TKey, TValue> targetDictionary)
        {
            _targetDictionary = targetDictionary;
            KeyType = typeof(TKey);
            ValueType = typeof(TValue);

            // For the case when target dictionary is an ExpandoObject (which implements IDictionary<string, object>)
            useDynamic = (KeyType == typeof(string)) && (ValueType == typeof(object));
        }

        public Type KeyType { get; }

        public Type ValueType { get; }

        public object GetValueForCaseInsensitiveKey(object key)
        {
            var foundKey = GetKeyUsingCaseInsensitiveSearch(key);
            return _targetDictionary[foundKey];
        }

        public void SetValueForCaseInsensitiveKey(object key, object value)
        {
            var foundKey = GetKeyUsingCaseInsensitiveSearch(key);

            // If the target object is an ExpandoObject, then the type of the value in the key-value pair is 'object'
            // and so we cannot deserialize the raw json to a particular type.
            // But if a value for a key already exists, then we try to figure out the type and deserialize the value to
            // this type.
            if (useDynamic && _targetDictionary.ContainsKey(foundKey) && _targetDictionary[foundKey] != null)
            {
                var result = ConversionHelper.ConvertToActualType(_targetDictionary[foundKey].GetType(), value);
                if (result.CanBeConverted)
                {
                    _targetDictionary[foundKey] = CastTo<TValue>(result.ConvertedInstance);
                    return;
                }
            }

            _targetDictionary[foundKey] = CastTo<TValue>(value);
        }

        public void RemoveValueForCaseInsensitiveKey(object key)
        {
            var foundKey = GetKeyUsingCaseInsensitiveSearch(key);
            _targetDictionary.Remove(foundKey);
        }

        public bool ContainsCaseInsensitiveKey(object key)
        {
            var foundKey = GetKeyUsingCaseInsensitiveSearch(key);
            return _targetDictionary.ContainsKey(foundKey);
        }

        private TKey GetKeyUsingCaseInsensitiveSearch(object key)
        {
            // Example: a Guid key
            if (KeyType != typeof(string))
            {
                var conversionResult = ConversionHelper.ConvertToActualType(KeyType, key);
                if (!conversionResult.CanBeConverted)
                {
                    throw new JsonPatchException(
                        message: Resources.FormatInvalidDictionaryKeyFormat(key, KeyType.FullName),
                        innerException: null);
                }
                return CastTo<TKey>(conversionResult.ConvertedInstance);
            }

            var keyToFind = (string)key;
            foreach (var currentKey in _targetDictionary.Keys)
            {
                var keyInDictionary = CastTo<string>(currentKey);
                if (string.Equals(keyToFind, keyInDictionary, StringComparison.OrdinalIgnoreCase))
                {
                    return CastTo<TKey>(keyInDictionary);
                }
            }
            return CastTo<TKey>(keyToFind);
        }

        private TModel CastTo<TModel>(object model)
        {
            return model is TModel ? (TModel)model : default(TModel);
        }
    }
}
