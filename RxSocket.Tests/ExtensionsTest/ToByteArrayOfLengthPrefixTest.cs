﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RxSocket.Tests
{
    public class ToByteArrayOfLengthPrefixTest
    {
        [Fact]
        public async Task T01()
        {
            Assert.Throws<ArgumentNullException>(() => ConversionsWithLengthPrefixEx.ToByteArrayOfLengthPrefix((byte[])null).ToList());

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await (new byte[] { 0, 0, 0, 0, 0 }).ToObservable().ToByteArrayOfLengthPrefix().FirstOrDefaultAsync());
        }

        [Theory]
        [InlineData(new byte[] { 0 }, new byte[] { 0, 0, 0, 1, 0 })]
        [InlineData(new byte[] { 65, 0 }, new byte[] { 0, 0, 0, 2, 65, 0 })]
        [InlineData(new byte[] { 65, 0, 66, 0 }, new byte[] { 0, 0, 0, 4, 65, 0, 66, 0 })]
        public async Task T02(byte[] result, byte[] bytes)
        {
            Assert.Equal(result, bytes.ToByteArrayOfLengthPrefix().SingleOrDefault());
            Assert.Equal(result, await bytes.ToObservable().ToByteArrayOfLengthPrefix());
        }

    }
}