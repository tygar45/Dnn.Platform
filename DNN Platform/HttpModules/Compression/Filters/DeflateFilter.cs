﻿#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2014
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion
#region Usings
using System.IO;
using System.IO.Compression;

#endregion
namespace DotNetNuke.HttpModules.Compression
{
    /// <summary>
    /// Summary description for DeflateFilter.
    /// </summary>
    public class DeflateFilter : CompressingFilter
    {
        private readonly DeflateStream _stream;

        public DeflateFilter(Stream baseStream) : base(baseStream)
        {
            _stream = new DeflateStream(baseStream, CompressionMode.Compress);
        }

        public override string ContentEncoding
        {
            get
            {
                return "deflate";
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!HasWrittenHeaders)
            {
                WriteHeaders();
            }
            _stream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            _stream.Close();
        }

        public override void Flush()
        {
            _stream.Flush();
        }
    }
}
