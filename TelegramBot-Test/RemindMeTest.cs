using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelegramBot.Commands;

namespace TelegramBot
{
    [TestClass]
    public class RemindMeTest
    {
        [TestMethod]
        public void TestParserTommorow()
        {
            RemindMeCommand com = new RemindMeCommand(null);
            Assert.AreEqual(DateTime.Now.Date.AddDays(1).AddHours(12), com.GetDate("tommorrow"));
        }

        [TestMethod]
        public void TestParserToday()
        {
            RemindMeCommand com = new RemindMeCommand(null);
            Assert.AreEqual(DateTime.Now.Date.AddHours(12), com.GetDate("today"));
        }

        [TestMethod]
        public void TestParser20Days()
        {
            RemindMeCommand com = new RemindMeCommand(null);
            Assert.AreEqual(DateTime.Now.AddDays(20.0).Date, com.GetDate("in 20 days").Date);
        }

        [TestMethod]
        public void TestEvery()
        {
            RemindMeCommand com = new RemindMeCommand(null);
            Assert.AreEqual(DateTime.Parse("31/12"), com.GetDate("every 31 december").Date);
        }
    }
}
