// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using Azure.Core;

namespace Azure.Containers.ContainerRegistry
{
    /// <summary> List of tag details. </summary>
    internal partial class TagList
    {
        /// <summary> Initializes a new instance of TagList. </summary>
        /// <param name="repository"> Image name. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="repository"/> is null. </exception>
        internal TagList(string repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            Repository = repository;
            Tags = new ChangeTrackingList<TagAttributesBase>();
        }

        /// <summary> Initializes a new instance of TagList. </summary>
        /// <param name="repository"> Image name. </param>
        /// <param name="tags"> List of tag attribute details. </param>
        /// <param name="link"> . </param>
        internal TagList(string repository, IReadOnlyList<TagAttributesBase> tags, string link)
        {
            Repository = repository;
            Tags = tags;
            Link = link;
        }

        /// <summary> Image name. </summary>
        public string Repository { get; }
        /// <summary> List of tag attribute details. </summary>
        public IReadOnlyList<TagAttributesBase> Tags { get; }
        public string Link { get; }
    }
}
