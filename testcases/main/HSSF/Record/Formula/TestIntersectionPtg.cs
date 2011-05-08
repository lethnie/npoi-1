        
/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is1 distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

namespace TestCases.HSSF.Record.Formula
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NPOI.HSSF.Record.Formula;
    using NPOI.HSSF.UserModel;

    /**
     * Tests for {@link IntersectionPtg}.
     *
     * @author Daniel Noll (daniel at nuix dot com dot au)
     */
    [TestClass]
    public class TestIntersectionPtg : AbstractPtgTestCase
    {
        /**
         * Tests Reading a file containing this ptg.
         */
        [TestMethod]
        public void TestReading()
        {
            HSSFWorkbook workbook = LoadWorkbook("IntersectionPtg.xls");
            NPOI.SS.UserModel.Cell cell = workbook.GetSheetAt(0).GetRow(4).GetCell(2);
            Assert.AreEqual(5.0, cell.NumericCellValue, 0.0, "Wrong cell value");
            Assert.AreEqual("SUM(A1:B2 B2:C3)", cell.CellFormula, "Wrong cell formula");
        }
    }
}

