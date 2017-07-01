using ApiLibs.Telegram;
using NUnit.Framework;
using System;
using TelegramBot;

namespace TelegramBotTest
{
    public class PrivilegeTest
    {
        LaurentiaBot bot;

        [SetUp]
        public void SetUp()
        {
            bot = new LaurentiaBot(new TelegramService("a", "a"), 1);
        }

        [Test]
        public void TestAdminTrue()
        {
            TestCommand com = new TestCommand(null);
            com.testPrivilege = Privilege.Admin;

            bot.Add(com);
            Assert.IsTrue(bot.hasPrivilige(1, com));
        }

        [Test]
        public void TestAdminFalse()
        {
            TestCommand com = new TestCommand(null);
            com.testPrivilege = Privilege.Admin;

            bot.Add(com);
            Assert.IsFalse(bot.hasPrivilige(2, com));
            Assert.IsFalse(bot.hasPrivilige(-1, com));
        }

        [Test]
        public void TestPublicTrue()
        {
            TestCommand com = new TestCommand(null);
            com.testPrivilege = Privilege.Public;

            bot.Add(com);
            Assert.IsTrue(bot.hasPrivilige(500, com));
        }

        [Test]
        public void TestBlackList()
        {
            TestCommand com = new TestCommand(null);
            com.testPrivilege = Privilege.BlackList;

            bot.Add(com);
            Assert.IsTrue(bot.hasPrivilige(2, com));
            com.PrivilegeList.Add(2);
            Assert.IsFalse(bot.hasPrivilige(2, com));
        }

        [Test]
        public void TestWhiteList()
        {
            TestCommand com = new TestCommand(null);
            com.testPrivilege = Privilege.WhiteList;

            bot.Add(com);
            Assert.IsFalse(bot.hasPrivilige(2, com));
            com.PrivilegeList.Add(2);
            Assert.IsTrue(bot.hasPrivilige(2, com));
        }

        [Test]
        public void TestWhiteListAdmin()
        {
            TestCommand com = new TestCommand(null);
            com.testPrivilege = Privilege.WhiteList;

            bot.Add(com);
            Assert.IsTrue(bot.hasPrivilige(1, com));
        }
    }

    internal class TestCommand : Command
    {
        public TestCommand(TelegramService tgs) : base(tgs)
        {
        }

        public Privilege testPrivilege;
        public override Privilege privilege => testPrivilege;
        public override string CommandName => "test";


    }
}
