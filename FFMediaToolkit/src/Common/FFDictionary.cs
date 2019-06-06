﻿namespace FFMediaToolkit.Common
{
    using System;
    using System.Collections.Generic;
    using FFMediaToolkit.Helpers;
    using FFmpeg.AutoGen;

    /// <summary>
    /// Represents a wrapper of <see cref="AVDictionary"/>. Used for applying codec and container settings.
    /// </summary>
    public unsafe class FFDictionary : Wrapper<AVDictionary>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FFDictionary"/> class.
        /// </summary>
        public FFDictionary()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FFDictionary"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary to copy.</param>
        public FFDictionary(Dictionary<string, string> dictionary)
            : base(null)
            => Copy(dictionary);

        /// <summary>
        /// Initializes a new instance of the <see cref="FFDictionary"/> class from given <see cref="AVDictionary"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="AVDictionary"/>.</param>
        internal FFDictionary(AVDictionary* dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        /// Gets the number of elements in the dictionary.
        /// </summary>
        public int Count => Pointer == null ? 0 : ffmpeg.av_dict_count(Pointer);

        /// <summary>
        /// Gets or sets the value with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        public string this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }

        /// <summary>
        /// Gets the value with specified key.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <param name="matchCase">If <see langword="true"/> matches case.</param>
        /// <returns>The value with specified key. If the key not exist, returns <see langword="null"/>.</returns>
        public string Get(string key, bool matchCase = true)
        {
            var ptr = ffmpeg.av_dict_get(Pointer, key, null, matchCase ? ffmpeg.AV_DICT_MATCH_CASE : 0);
            return ptr != null ? StringConverter.StringFromUtf8(new IntPtr(ptr)) : null;
        }

        /// <summary>
        /// Sets the value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set(string key, string value)
        {
            var ptr = Pointer;
            ffmpeg.av_dict_set(&ptr, key, value, 0);
            UpdatePointer(ptr);
        }

        /// <summary>
        /// Copies items from specified dictionary to this <see cref="FFDictionary"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary to copy.</param>
        public void Copy(Dictionary<string, string> dictionary)
        {
            foreach (var item in dictionary)
            {
                this[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// Updates the pointer to the dictionary.
        /// </summary>
        /// <param name="pointer">The pointer to the <see cref="AVDictionary"/>.</param>
        internal void Update(AVDictionary* pointer) => UpdatePointer(pointer);

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            if (Pointer != null)
            {
                var ptr = Pointer;
                ffmpeg.av_dict_free(&ptr);
            }
        }
    }
}