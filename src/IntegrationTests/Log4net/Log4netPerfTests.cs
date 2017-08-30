﻿using System;
using System.Diagnostics;
using System.Threading;
using FluentAssertions;
using log4net;
using log4net.Repository.Hierarchy;
using Logzio.DotNet.Core.Bootstrap;
using Logzio.DotNet.Core.Shipping;
using Logzio.DotNet.IntegrationTests.Listener;
using Logzio.DotNet.Log4net;
using NUnit.Framework;

namespace Logzio.DotNet.IntegrationTests.Log4net
{
    [TestFixture]
    public class Log4netPerfTests
    {
        private LogzioListenerDummy _dummy;

        [SetUp]
        public void Setup()
        {
            _dummy = new LogzioListenerDummy();
            _dummy.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _dummy.Stop();
        }

        [Test]
        public void MeasurePerfForLog4net()
        {
            var bufferSize = 100;
            var logzioAppender = SetupAppender(bufferSize);
            logzioAppender.AddDebug(true);
            var logger = LogManager.GetLogger(typeof (Log4netSanityTests));

            var stopwatch = Stopwatch.StartNew();

            var logsAmount = 1000;
            for (var i = 0; i < logsAmount; i++)
            {
                logger.Info("A Fish");
            }

            stopwatch.Stop();
            Console.WriteLine("Total time: " + stopwatch.Elapsed);
            stopwatch.Elapsed.Should().BeLessOrEqualTo(TimeSpan.FromMilliseconds(100));

            new Bootstraper().Resolve<IShipper>().WaitForSendLogsTask();

            logzioAppender.Close();
            LogManager.Shutdown();

            _dummy.Requests.Count.Should().BeInRange((int)Math.Ceiling((decimal) (logsAmount / bufferSize)),
                (int)Math.Floor((decimal)logsAmount / bufferSize));
        }

        private static LogzioAppender SetupAppender(int bufferSize)
        {
            var hierarchy = (Hierarchy) LogManager.GetRepository(typeof(LogzioAppender).Assembly);
            var logzioAppender = new LogzioAppender();
            logzioAppender.AddToken("DKJiomZjbFyVvssJDmUAWeEOSNnDARWz");
            logzioAppender.AddListenerUrl(LogzioListenerDummy.DefaultUrl);
            logzioAppender.AddBufferSize(bufferSize);
            hierarchy.Root.AddAppender(logzioAppender);
            hierarchy.Configured = true;
            return logzioAppender;
        }
    }
}