﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.AI.DocumentTranslation.Models;
using Azure.Core;

namespace Azure.AI.DocumentTranslation
{
    [CodeGenModel("ErrorV2")]
    public partial class DocumentTranslationError
    {
        /// <summary> Enums containing high level error codes. </summary>
        [CodeGenMember("Code")]
        public DocumentTranslationErrorCode? ErrorCode { get; }

        internal InnerErrorV2 InnerError { get; }

        internal DocumentTranslationError(DocumentTranslationErrorCode? errorCode, string message, string target, InnerErrorV2 innerError)
        {
            if (innerError != null)
            {
                // Assigns the inner error, which should be only one level down.
                ErrorCode = innerError.Code;
                Message = innerError.Message;
                Target = innerError.Target;
            }
            else
            {
                ErrorCode = errorCode;
                Message = message;
                Target = target;
            }
        }
    }
}
