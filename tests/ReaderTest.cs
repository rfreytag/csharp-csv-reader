﻿/*
 * 2006 - 2018 Ted Spence, http://tedspence.com
 * License: http://www.apache.org/licenses/LICENSE-2.0 
 * Home page: https://github.com/tspence/csharp-csv-reader
 */
using System;
using NUnit.Framework;
using CSVFile;
#if HAS_ASYNC
using System.Threading.Tasks;
#endif

// ReSharper disable StringLiteralTypo

namespace CSVTestSuite
{
    [TestFixture]
    public class ReaderTest
    {
        [Test]
        public void TestBasicReader()
        {
            var source = "Name,Title,Phone\n" +
                         "JD,Doctor,x234\n" +
                         "Janitor,Janitor,x235\n" +
                         "\"Dr. Reed, " + Environment.NewLine + "Eliot\",\"Private \"\"Practice\"\"\",x236\n" +
                         "Dr. Kelso,Chief of Medicine,x100";

            // Skip header row
            var settings = new CSVSettings()
            {
                HeaderRowIncluded = false
            };

            // Convert into stream
            using (var cr = CSVReader.FromString(source, settings))
            {
                var i = 0;
                foreach (var line in cr)
                {
                    Assert.AreEqual(3, line.Length);
                    switch (i)
                    {
                        case 0:
                            Assert.AreEqual("Name", line[0]);
                            Assert.AreEqual("Title", line[1]);
                            Assert.AreEqual("Phone", line[2]);
                            break;
                        case 1:
                            Assert.AreEqual("JD", line[0]);
                            Assert.AreEqual("Doctor", line[1]);
                            Assert.AreEqual("x234", line[2]);
                            break;
                        case 2:
                            Assert.AreEqual("Janitor", line[0]);
                            Assert.AreEqual("Janitor", line[1]);
                            Assert.AreEqual("x235", line[2]);
                            break;
                        case 3:
                            Assert.AreEqual("Dr. Reed, " + Environment.NewLine + "Eliot", line[0]);
                            Assert.AreEqual("Private \"Practice\"", line[1]);
                            Assert.AreEqual("x236", line[2]);
                            break;
                        case 4:
                            Assert.AreEqual("Dr. Kelso", line[0]);
                            Assert.AreEqual("Chief of Medicine", line[1]);
                            Assert.AreEqual("x100", line[2]);
                            break;
                        default:
                            Assert.IsTrue(false, "Should not get here");
                            break;
                    }
                    i++;
                }
            }
        }

        [Test]
        public void TestDanglingFields()
        {
            var source = "Name,Title,Phone,Dangle\n" +
                "JD,Doctor,x234,\n" +
                "Janitor,Janitor,x235,\n" +
                "\"Dr. Reed, " + Environment.NewLine + "Eliot\",\"Private \"\"Practice\"\"\",x236,\n" +
                "Dr. Kelso,Chief of Medicine,x100,\n" +
                ",,,";

            // Skip header row
            var settings = new CSVSettings()
            {
                HeaderRowIncluded = false
            };

            // Convert into stream
            using (var cr = CSVReader.FromString(source, settings))
            {
                var i = 0;
                foreach (var line in cr)
                {
                    Assert.AreEqual(4, line.Length);
                    switch (i)
                    {
                        case 0:
                            Assert.AreEqual("Name", line[0]);
                            Assert.AreEqual("Title", line[1]);
                            Assert.AreEqual("Phone", line[2]);
                            Assert.AreEqual("Dangle", line[3]);
                            break;
                        case 1:
                            Assert.AreEqual("JD", line[0]);
                            Assert.AreEqual("Doctor", line[1]);
                            Assert.AreEqual("x234", line[2]);
                            Assert.AreEqual("", line[3]);
                            break;
                        case 2:
                            Assert.AreEqual("Janitor", line[0]);
                            Assert.AreEqual("Janitor", line[1]);
                            Assert.AreEqual("x235", line[2]);
                            Assert.AreEqual("", line[3]);
                            break;
                        case 3:
                            Assert.AreEqual("Dr. Reed, " + Environment.NewLine + "Eliot", line[0]);
                            Assert.AreEqual("Private \"Practice\"", line[1]);
                            Assert.AreEqual("x236", line[2]);
                            Assert.AreEqual("", line[3]);
                            break;
                        case 4:
                            Assert.AreEqual("Dr. Kelso", line[0]);
                            Assert.AreEqual("Chief of Medicine", line[1]);
                            Assert.AreEqual("x100", line[2]);
                            Assert.AreEqual("", line[3]);
                            break;
                        case 5:
                            Assert.AreEqual("", line[0]);
                            Assert.AreEqual("", line[1]);
                            Assert.AreEqual("", line[2]);
                            Assert.AreEqual("", line[3]);
                            break;
                        default:
                            Assert.IsTrue(false, "Should not get here");
                            break;
                    }

                    i++;
                }
            }
        }

        [Test]
        public void TestAlternateDelimiterQualifiers()
        {
            var source = "Name\tTitle\tPhone\n" +
                         "JD\tDoctor\tx234\n" +
                         "Janitor\tJanitor\tx235\n" +
                         "\"Dr. Reed, " + Environment.NewLine + "Eliot\"\t\"Private \"\"Practice\"\"\"\tx236\n" +
                         "Dr. Kelso\tChief of Medicine\tx100";

            // Convert into stream
            var settings = new CSVSettings() { HeaderRowIncluded = true, FieldDelimiter = '\t' };
            using (var cr = CSVReader.FromString(source, settings))
            {
                Assert.AreEqual("Name", cr.Headers[0]);
                Assert.AreEqual("Title", cr.Headers[1]);
                Assert.AreEqual("Phone", cr.Headers[2]);
                var i = 1;
                foreach (var line in cr)
                {
                    Assert.AreEqual(3, line.Length);
                    switch (i)
                    {
                        case 1:
                            Assert.AreEqual("JD", line[0]);
                            Assert.AreEqual("Doctor", line[1]);
                            Assert.AreEqual("x234", line[2]);
                            break;
                        case 2:
                            Assert.AreEqual("Janitor", line[0]);
                            Assert.AreEqual("Janitor", line[1]);
                            Assert.AreEqual("x235", line[2]);
                            break;
                        case 3:
                            Assert.AreEqual("Dr. Reed, " + Environment.NewLine + "Eliot", line[0]);
                            Assert.AreEqual("Private \"Practice\"", line[1]);
                            Assert.AreEqual("x236", line[2]);
                            break;
                        case 4:
                            Assert.AreEqual("Dr. Kelso", line[0]);
                            Assert.AreEqual("Chief of Medicine", line[1]);
                            Assert.AreEqual("x100", line[2]);
                            break;
                    }

                    i++;
                }
            }
        }
        
        
        [Test]
        public void TestSepLineOverride()
        {
            // The "sep=" line is a feature of Microsoft Excel that makes CSV files work more smoothly with
            // European data sets where a comma is used in numerical separation.  If present, it overrides
            // the FieldDelimiter setting from the CSV Settings.
            var source = "sep=|\n" +
                         "Name|Title|Phone\n" +
                         "JD|Doctor|x234\n" +
                         "Janitor|Janitor|x235\n" +
                         "\"Dr. Reed, " + Environment.NewLine + "Eliot\"|\"Private \"\"Practice\"\"\"|x236\n" +
                         "Dr. Kelso|Chief of Medicine|x100";

            // Convert into stream
            var settings = new CSVSettings() { HeaderRowIncluded = true, FieldDelimiter = '\t', AllowSepLine = true };
            using (var cr = CSVReader.FromString(source, settings))
            {
                // The field delimiter should have been changed, but the original object should remain the same
                Assert.AreEqual('\t', settings.FieldDelimiter);
                Assert.AreEqual('|', cr.Settings.FieldDelimiter);
                Assert.AreEqual("Name", cr.Headers[0]);
                Assert.AreEqual("Title", cr.Headers[1]);
                Assert.AreEqual("Phone", cr.Headers[2]);
                var i = 1;
                foreach (var line in cr)
                {
                    switch (i)
                    {
                        case 1:
                            Assert.AreEqual("JD", line[0]);
                            Assert.AreEqual("Doctor", line[1]);
                            Assert.AreEqual("x234", line[2]);
                            break;
                        case 2:
                            Assert.AreEqual("Janitor", line[0]);
                            Assert.AreEqual("Janitor", line[1]);
                            Assert.AreEqual("x235", line[2]);
                            break;
                        case 3:
                            Assert.AreEqual("Dr. Reed, " + Environment.NewLine + "Eliot", line[0]);
                            Assert.AreEqual("Private \"Practice\"", line[1]);
                            Assert.AreEqual("x236", line[2]);
                            break;
                        case 4:
                            Assert.AreEqual("Dr. Kelso", line[0]);
                            Assert.AreEqual("Chief of Medicine", line[1]);
                            Assert.AreEqual("x100", line[2]);
                            break;
                    }

                    i++;
                }
            }
        }
        
#if HAS_ASYNC_IENUM
        [Test]
        public async Task TestAsyncReader()
        {
            // The "sep=" line is a feature of Microsoft Excel that makes CSV files work more smoothly with
            // European data sets where a comma is used in numerical separation.  If present, it overrides
            // the FieldDelimiter setting from the CSV Settings.
            var source = "sep=|\n" +
                         "Name|Title|Phone\n" +
                         "JD|Doctor|x234\n" +
                         "Janitor|Janitor|x235\n" +
                         "\"Dr. Reed, " + Environment.NewLine + "Eliot\"|\"Private \"\"Practice\"\"\"|x236\n" +
                         "Dr. Kelso|Chief of Medicine|x100";

            // Convert into stream
            var settings = new CSVSettings() { HeaderRowIncluded = true, FieldDelimiter = '\t', AllowSepLine = true };
            using (var cr = CSVReader.FromString(source, settings))
            {
                // The field delimiter should have been changed, but the original object should remain the same
                Assert.AreEqual('\t', settings.FieldDelimiter);
                Assert.AreEqual('|', cr.Settings.FieldDelimiter);
                Assert.AreEqual("Name", cr.Headers[0]);
                Assert.AreEqual("Title", cr.Headers[1]);
                Assert.AreEqual("Phone", cr.Headers[2]);
                var i = 1;
                await foreach (var line in cr)
                {
                    switch (i)
                    {
                        case 1:
                            Assert.AreEqual("JD", line[0]);
                            Assert.AreEqual("Doctor", line[1]);
                            Assert.AreEqual("x234", line[2]);
                            break;
                        case 2:
                            Assert.AreEqual("Janitor", line[0]);
                            Assert.AreEqual("Janitor", line[1]);
                            Assert.AreEqual("x235", line[2]);
                            break;
                        case 3:
                            Assert.AreEqual("Dr. Reed, " + Environment.NewLine + "Eliot", line[0]);
                            Assert.AreEqual("Private \"Practice\"", line[1]);
                            Assert.AreEqual("x236", line[2]);
                            break;
                        case 4:
                            Assert.AreEqual("Dr. Kelso", line[0]);
                            Assert.AreEqual("Chief of Medicine", line[1]);
                            Assert.AreEqual("x100", line[2]);
                            break;
                    }

                    i++;
                }
            }
        }
#endif
    }
}
