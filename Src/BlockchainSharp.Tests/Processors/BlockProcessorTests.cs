﻿namespace BlockchainSharp.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using BlockchainSharp.Core;
    using BlockchainSharp.Core.Types;
    using BlockchainSharp.Processors;
    using BlockchainSharp.Tests.TestUtils;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BlockProcessorTests
    {
        [TestMethod]
        public void CreateWithBlockChain()
        {
            BlockProcessor processor = FactoryHelper.CreateBlockProcessor();

            Assert.IsNotNull(processor.BlockChain);
        }

        [TestMethod]
        public void ProcessGenesisBlock()
        {
            Block genesis = new Block(0, null);
            BlockProcessor processor = FactoryHelper.CreateBlockProcessor();

            processor.ProcessBlock(genesis);

            Assert.IsNotNull(processor.BlockChain);
            Assert.AreEqual(0ul, processor.BlockChain.BestBlockNumber);
            Assert.AreEqual(genesis, processor.BlockChain.GetBlock(0));
        }

        [TestMethod]
        public void ProcessTwoBlocks()
        {
            Block genesis = new Block(0, null);
            Block block = new Block(1, genesis.Hash);

            BlockProcessor processor = FactoryHelper.CreateBlockProcessor();

            processor.ProcessBlock(genesis);
            processor.ProcessBlock(block);

            Assert.IsNotNull(processor.BlockChain);
            Assert.AreEqual(1ul, processor.BlockChain.BestBlockNumber);
            Assert.AreEqual(genesis, processor.BlockChain.GetBlock(0));
            Assert.AreEqual(block, processor.BlockChain.GetBlock(1));
        }

        [TestMethod]
        public void ProcessTwoBlocksAndOneUncle()
        {
            Block genesis = new Block(0, null);
            Block block = new Block(1, genesis.Hash);
            Block uncle = new Block(1, genesis.Hash);

            BlockProcessor processor = FactoryHelper.CreateBlockProcessor();

            processor.ProcessBlock(genesis);
            processor.ProcessBlock(block);
            processor.ProcessBlock(uncle);

            Assert.IsNotNull(processor.BlockChain);
            Assert.AreEqual(1ul, processor.BlockChain.BestBlockNumber);
            Assert.AreEqual(genesis, processor.BlockChain.GetBlock(0));
            Assert.AreEqual(block, processor.BlockChain.GetBlock(1));
        }

        [TestMethod]
        public void ProcessTwoBlocksAndTwoUncles()
        {
            Block genesis = new Block(0, null);
            Block block = new Block(1, genesis.Hash);

            Address from = new Address();
            Address to = new Address();

            Transaction transaction = new Transaction(from, to, new BigInteger(2));

            Block uncle1 = new Block(1, genesis.Hash, new Transaction[] { transaction });
            Block uncle2 = new Block(2, uncle1.Hash);

            BlockProcessor processor = FactoryHelper.CreateBlockProcessor();

            processor.ProcessBlock(genesis);
            processor.ProcessBlock(block);
            processor.ProcessBlock(uncle1);
            processor.ProcessBlock(uncle2);

            Assert.IsNotNull(processor.BlockChain);
            Assert.AreEqual(2ul, processor.BlockChain.BestBlockNumber);
            Assert.AreEqual(genesis, processor.BlockChain.GetBlock(0));
            Assert.AreEqual(uncle1, processor.BlockChain.GetBlock(1));
            Assert.AreEqual(uncle2, processor.BlockChain.GetBlock(2));
        }

        [TestMethod]
        public void ProcessTwoBlocksAndThreeUnclesInReverse()
        {
            Block genesis = new Block(0, null);
            Block block = new Block(1, genesis.Hash);

            Address from = new Address();
            Address to = new Address();

            Transaction transaction = new Transaction(from, to, new BigInteger(2));

            Block uncle1 = new Block(1, genesis.Hash, new Transaction[] { transaction });
            Block uncle2 = new Block(2, uncle1.Hash);
            Block uncle3 = new Block(3, uncle2.Hash);

            BlockProcessor processor = FactoryHelper.CreateBlockProcessor();

            Assert.AreEqual(BlockProcess.Imported, processor.ProcessBlock(genesis));
            Assert.AreEqual(BlockProcess.Imported, processor.ProcessBlock(block));
            Assert.AreEqual(BlockProcess.MissingAncestor, processor.ProcessBlock(uncle3));
            Assert.AreEqual(BlockProcess.MissingAncestor, processor.ProcessBlock(uncle2));
            Assert.AreEqual(BlockProcess.Imported, processor.ProcessBlock(uncle1));

            Assert.IsNotNull(processor.BlockChain);
            Assert.AreEqual(3ul, processor.BlockChain.BestBlockNumber);
            Assert.AreEqual(genesis, processor.BlockChain.GetBlock(0));
            Assert.AreEqual(uncle1, processor.BlockChain.GetBlock(1));
            Assert.AreEqual(uncle2, processor.BlockChain.GetBlock(2));
            Assert.AreEqual(uncle3, processor.BlockChain.GetBlock(3));
        }
    }
}
