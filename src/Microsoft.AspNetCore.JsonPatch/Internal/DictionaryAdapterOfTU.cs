// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace Microsoft.AspNetCore.JsonPatch.Internal
{
    public class DictionaryAdapter<TKey, TValue> : IAdapter
    {
        public bool TryAdd(
            object target,
            string segment,
            IContractResolver contractResolver,
            object value,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;

            // As per JsonPatch spec, if a key already exists, adding should replace the existing value
            var convertedKey = (TKey)ConversionResultProvider.ConvertTo(key, typeof(TKey)).ConvertedInstance;
            dictionary[convertedKey] = ConvertValue(dictionary, convertedKey, value);

            errorMessage = null;
            return true;
        }

        public bool TryGet(
            object target,
            string segment,
            IContractResolver contractResolver,
            out object value,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;
            var convertedKey = (TKey)ConversionResultProvider.ConvertTo(key, typeof(TKey)).ConvertedInstance;

            if (!dictionary.ContainsKey(convertedKey))
            {
                value = null;
                errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
                return false;
            }

            value = dictionary[convertedKey];
            errorMessage = null;
            return true;
        }

        public bool TryRemove(
            object target,
            string segment,
            IContractResolver contractResolver,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;
            var convertedKey = (TKey)ConversionResultProvider.ConvertTo(key, typeof(TKey)).ConvertedInstance;

            // As per JsonPatch spec, the target location must exist for remove to be successful
            if (!dictionary.ContainsKey(convertedKey))
            {
                errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
                return false;
            }

            dictionary.Remove(convertedKey);

            errorMessage = null;
            return true;
        }

        public bool TryReplace(
            object target,
            string segment,
            IContractResolver contractResolver,
            object value,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;
            var convertedKey = (TKey)ConversionResultProvider.ConvertTo(key, typeof(TKey)).ConvertedInstance;

            // As per JsonPatch spec, the target location must exist for remove to be successful
            if (!dictionary.ContainsKey(convertedKey))
            {
                errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
                return false;
            }

            dictionary[convertedKey] = ConvertValue(dictionary, convertedKey, value);

            errorMessage = null;
            return true;
        }

        public bool TryTraverse(
            object target,
            string segment,
            IContractResolver contractResolver,
            out object nextTarget,
            out string errorMessage)
        {
            var contract = (JsonDictionaryContract)contractResolver.ResolveContract(target.GetType());
            var key = contract.DictionaryKeyResolver(segment);
            var dictionary = (IDictionary<TKey, TValue>)target;
            var convertedKey = (TKey)ConversionResultProvider.ConvertTo(key, typeof(TKey)).ConvertedInstance;

            if (dictionary.ContainsKey(convertedKey))
            {
                nextTarget = dictionary[convertedKey];
                errorMessage = null;
                return true;
            }
            else
            {
                nextTarget = null;
                errorMessage = null;
                return false;
            }
        }

        private TValue ConvertValue(IDictionary<TKey, TValue> dictionary, TKey key, object newValue)
        {
            if (dictionary.TryGetValue(key, out var existingValue))
            {
                if (existingValue != null)
                {
                    var conversionResult = ConversionResultProvider.ConvertTo(newValue, existingValue.GetType());
                    if (conversionResult.CanBeConverted)
                    {
                        return (TValue)conversionResult.ConvertedInstance;
                    }
                }
            }
            return (TValue)newValue;
        }
    }
}
