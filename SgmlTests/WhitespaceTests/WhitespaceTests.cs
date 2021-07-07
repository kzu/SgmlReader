﻿/*
 * Original work Copyright (c) 2008 MindTouch. All rights reserved. 
 * Modified Work Copyright (c) 2016 Microsoft Corporation. All rights reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 */

using Sgml;
using System;
using System.IO;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SGMLTests
{

    /// <summary>https://github.com/lovettchris/SgmlReader/issues/15</summary>
    [TestFixture]
    public class WhitespaceTests
    {
        [Test]
        public void DoBradescoTrailingWhitespaceTest()
        {
            // bradesco.ofx is licensed under the MIT license. From github.com/kevencarneiro/OFXSharp
            string bradescoOfxText = Tests.ReadTestResource(name: "bradesco.ofx");
            SgmlDtd ofx160Dtd      = Tests.LoadDtd(docType: "OFX", name: "ofx160.dtd");

            XmlDocument asXml = ConvertSgmlToXml(ofx160Dtd, bradescoOfxText);
            //asXml.Dump();

            XmlNode codeNode = asXml.SelectSingleNode("//*[local-name()='CODE']");
            string sonrsStatusCodeInnerText = codeNode.InnerText;

            Assert.IsFalse(condition: sonrsStatusCodeInnerText.EndsWith("\r\n"), message: "There should be no trailing whitespace in read elements' innerText.");
        }

        private static XmlDocument ConvertSgmlToXml(SgmlDtd sgmlDtd, string sgmlText)
        {
            using (StringReader rdr = new StringReader(sgmlText))
            {
                return ConvertSgmlToXml(sgmlDtd, rdr);
            }
        }

        private static XmlDocument ConvertSgmlToXml(SgmlDtd sgmlDtd, TextReader reader)
        {
            SgmlReader sgmlReader = new SgmlReader();
            sgmlReader.WhitespaceHandling = WhitespaceHandling.None;
            sgmlReader.InputStream = reader;
            sgmlReader.DocType = sgmlDtd.Name; // "OFX"; ?
            sgmlReader.Dtd = sgmlDtd;

            XmlDocument doc = new XmlDocument();
            using (XmlWriter xmlWriter = doc.CreateNavigator().AppendChild())
            {
                while (!sgmlReader.EOF)
                {
                    xmlWriter.WriteNode(sgmlReader, defattr: true);
                }
            }

            return doc;
        }
    }
}
