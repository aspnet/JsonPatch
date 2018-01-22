﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;

namespace Microsoft.AspNetCore.JsonPatch.Adapters
{
    /// <inheritdoc />
    public class ObjectAdapterTyped<TParsedPath, TObjectVisitor> : IObjectAdapterWithTest
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ObjectAdapterTyped{TParsedPath, TObjectVisitor}"/>.
        /// </summary>
        /// <param name="contractResolver">The <see cref="IContractResolver"/>.</param>
        /// <param name="logErrorAction">The <see cref="Action"/> for logging <see cref="JsonPatchError"/>.</param>
        public ObjectAdapterTyped(
            IContractResolver contractResolver,
            Action<JsonPatchError> logErrorAction)
        {
            ContractResolver = contractResolver ?? throw new ArgumentNullException(nameof(contractResolver));
            LogErrorAction = logErrorAction;
        }

        /// <summary>
        /// Gets or sets the <see cref="IContractResolver"/>.
        /// </summary>
        public IContractResolver ContractResolver { get; }

        /// <summary>
        /// Action for logging <see cref="JsonPatchError"/>.
        /// </summary>
        public Action<JsonPatchError> LogErrorAction { get; }

        public virtual void Add(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            Add(operation.path, operation.value, objectToApplyTo, operation);
        }

        /// <summary>
        /// Add is used by various operations (eg: add, copy, ...), yet through different operations;
        /// This method allows code reuse yet reporting the correct operation on error
        /// </summary>
        protected virtual void Add(
            string path,
            object value,
            object objectToApplyTo,
            Operation operation)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            IParsedPath parsedPath = Activator.CreateInstance(typeof(TParsedPath), new object[] { path }) as IParsedPath;
            IObjectVisitor visitor = Activator.CreateInstance(typeof(TObjectVisitor), new object[] { parsedPath, ContractResolver }) as IObjectVisitor;

            var target = objectToApplyTo;
            if (!visitor.TryVisit(ref target, out var adapter, out var errorMessage))
            {
                var error = CreatePathNotFoundError(objectToApplyTo, path, operation, errorMessage);
                ErrorReporter(error);
                return;
            }

            if (!adapter.TryAdd(target, parsedPath.LastSegment, ContractResolver, value, out errorMessage))
            {
                var error = CreateOperationFailedError(objectToApplyTo, path, operation, errorMessage);
                ErrorReporter(error);
                return;
            }
        }

        public virtual void Move(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            // Get value at 'from' location and add that value to the 'path' location
            if (TryGetValue(operation.from, objectToApplyTo, operation, out var propertyValue))
            {
                // remove that value
                Remove(operation.from, objectToApplyTo, operation);

                // add that value to the path location
                Add(operation.path,
                    propertyValue,
                    objectToApplyTo,
                    operation);
            }
        }

        public virtual void Remove(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            Remove(operation.path, objectToApplyTo, operation);
        }

        /// <summary>
        /// Remove is used by various operations (eg: remove, move, ...), yet through different operations;
        /// This method allows code reuse yet reporting the correct operation on error.  The return value
        /// contains the type of the item that has been removed (and a bool possibly signifying an error)
        /// This can be used by other methods, like replace, to ensure that we can pass in the correctly
        /// typed value to whatever method follows.
        /// </summary>
        protected virtual void Remove(string path, object objectToApplyTo, Operation operationToReport)
        {
            IParsedPath parsedPath = Activator.CreateInstance(typeof(TParsedPath), new object[] { path }) as IParsedPath;
            IObjectVisitor visitor = Activator.CreateInstance(typeof(TObjectVisitor), new object[] { parsedPath, ContractResolver }) as IObjectVisitor;

            var target = objectToApplyTo;
            if (!visitor.TryVisit(ref target, out var adapter, out var errorMessage))
            {
                var error = CreatePathNotFoundError(objectToApplyTo, path, operationToReport, errorMessage);
                ErrorReporter(error);
                return;
            }

            if (!adapter.TryRemove(target, parsedPath.LastSegment, ContractResolver, out errorMessage))
            {
                var error = CreateOperationFailedError(objectToApplyTo, path, operationToReport, errorMessage);
                ErrorReporter(error);
                return;
            }
        }

        public virtual void Replace(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            IParsedPath parsedPath = Activator.CreateInstance(typeof(TParsedPath), new object[] { operation.path }) as IParsedPath;
            IObjectVisitor visitor = Activator.CreateInstance(typeof(TObjectVisitor), new object[] { parsedPath, ContractResolver }) as IObjectVisitor;

            var target = objectToApplyTo;
            if (!visitor.TryVisit(ref target, out var adapter, out var errorMessage))
            {
                var error = CreatePathNotFoundError(objectToApplyTo, operation.path, operation, errorMessage);
                ErrorReporter(error);
                return;
            }

            if (!adapter.TryReplace(target, parsedPath.LastSegment, ContractResolver, operation.value, out errorMessage))
            {
                var error = CreateOperationFailedError(objectToApplyTo, operation.path, operation, errorMessage);
                ErrorReporter(error);
                return;
            }
        }

        public virtual void Copy(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            // Get value at 'from' location and add that value to the 'path' location
            if (TryGetValue(operation.from, objectToApplyTo, operation, out var propertyValue))
            {
                // Create deep copy
                var copyResult = ConversionResultProvider.CopyTo(propertyValue, propertyValue.GetType());
                if (copyResult.CanBeConverted)
                {
                    Add(operation.path,
                        copyResult.ConvertedInstance,
                        objectToApplyTo,
                        operation);
                }
                else
                {
                    var error = CreateOperationFailedError(objectToApplyTo, operation.path, operation, Resources.FormatCannotCopyProperty(operation.from));
                    ErrorReporter(error);
                    return;
                }
            }
        }

        public virtual void Test(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            IParsedPath parsedPath = Activator.CreateInstance(typeof(TParsedPath), new object[] { operation.path }) as IParsedPath;
            IObjectVisitor visitor = Activator.CreateInstance(typeof(TObjectVisitor), new object[] { parsedPath, ContractResolver }) as IObjectVisitor;

            var target = objectToApplyTo;
            if (!visitor.TryVisit(ref target, out var adapter, out var errorMessage))
            {
                var error = CreatePathNotFoundError(objectToApplyTo, operation.path, operation, errorMessage);
                ErrorReporter(error);
                return;
            }

            if (!adapter.TryTest(target, parsedPath.LastSegment, ContractResolver, operation.value, out errorMessage))
            {
                var error = CreateOperationFailedError(objectToApplyTo, operation.path, operation, errorMessage);
                ErrorReporter(error);
                return;
            }
        }

        protected virtual bool TryGetValue(
            string fromLocation,
            object objectToGetValueFrom,
            Operation operation,
            out object propertyValue)
        {
            if (fromLocation == null)
            {
                throw new ArgumentNullException(nameof(fromLocation));
            }

            if (objectToGetValueFrom == null)
            {
                throw new ArgumentNullException(nameof(objectToGetValueFrom));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            propertyValue = null;

            IParsedPath parsedPath = Activator.CreateInstance(typeof(TParsedPath), new object[] { operation.path }) as IParsedPath;
            IObjectVisitor visitor = Activator.CreateInstance(typeof(TObjectVisitor), new object[] { parsedPath, ContractResolver }) as IObjectVisitor;

            var target = objectToGetValueFrom;
            if (!visitor.TryVisit(ref target, out var adapter, out var errorMessage))
            {
                var error = CreatePathNotFoundError(objectToGetValueFrom, fromLocation, operation, errorMessage);
                ErrorReporter(error);
                return false;
            }

            if (!adapter.TryGet(target, parsedPath.LastSegment, ContractResolver, out propertyValue, out errorMessage))
            {
                var error = CreateOperationFailedError(objectToGetValueFrom, fromLocation, operation, errorMessage);
                ErrorReporter(error);
                return false;
            }

            return true;
        }

        protected virtual Action<JsonPatchError> ErrorReporter
        {
            get
            {
                return LogErrorAction ?? Internal.ErrorReporter.Default;
            }
        }

        protected virtual JsonPatchError CreateOperationFailedError(object target, string path, Operation operation, string errorMessage)
        {
            return new JsonPatchError(
                target,
                operation,
                errorMessage ?? Resources.FormatCannotPerformOperation(operation.op, path));
        }

        protected virtual JsonPatchError CreatePathNotFoundError(object target, string path, Operation operation, string errorMessage)
        {
            return new JsonPatchError(
                target,
                operation,
                errorMessage ?? Resources.FormatTargetLocationNotFound(operation.op, path));
        }
    }
}