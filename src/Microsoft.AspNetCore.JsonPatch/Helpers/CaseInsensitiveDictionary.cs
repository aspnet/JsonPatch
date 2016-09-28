// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch.Exceptions;

namespace Microsoft.AspNetCore.JsonPatch.Helpers
{
    internal class CaseInsensitiveDictionary<TKey, TValue> : ICaseInsensitiveDictionary
    {
        private readonly IDictionary<TKey, TValue> _targetDictionary;

        public CaseInsensitiveDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _targetDictionary = dictionary;
        }

        public void Add(object key, object value)
        {
            _targetDictionary.Add(GetKey(key), CastTo<TValue>(value));
        }

        public object Get(object key)
        {
            var actualKey = GetKey(key);
            return _targetDictionary[actualKey];
        }

        public void Set(object key, object value)
        {
            var actualKey = GetKey(key);
            _targetDictionary[actualKey] = CastTo<TValue>(value);
        }

        public void Remove(object key)
        {
            var actualKey = GetKey(key);
            _targetDictionary.Remove(actualKey);
        }

        public bool ContainsKey(object key)
        {
            var actualKey = GetKey(key);
            return _targetDictionary.ContainsKey(actualKey);
        }

        private TKey GetKey(object key)
        {
            if (typeof(TKey) != typeof(string))
            {
                var conversionResult = ConversionResultProvider.ConvertTo(typeof(TKey), key);
                if (!conversionResult.CanBeConverted)
                {
                    throw new JsonPatchException(
                        message: Resources.FormatInvalidDictionaryKeyFormat(key, typeof(TKey).FullName),
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
