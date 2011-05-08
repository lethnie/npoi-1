/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

namespace TestCases.SS.UserModel
{
    using System;
    using NPOI.SS;
    using NPOI.SS.UserModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections;

    /**
     * A base class for testing implementations of
     * {@link NPOI.SS.usermodel.Row}
     */
    public abstract class BaseTestRow
    {

        /**
         * @return an object that provides test data in  / XSSF specific way
         */
        private ITestDataProvider _testDataProvider;
        protected BaseTestRow(ITestDataProvider testDataProvider)
        {
            _testDataProvider = testDataProvider;
        }

        [TestMethod]
        public void TestLastAndFirstColumns()
        {
            Workbook workbook = _testDataProvider.CreateWorkbook();
            Sheet sheet = workbook.CreateSheet();
            Row row = sheet.CreateRow(0);
            Assert.AreEqual(-1, row.FirstCellNum);
            Assert.AreEqual(-1, row.LastCellNum);

            //Getting cells from an empty row should returns null
            for (int i = 0; i < 10; i++) Assert.IsNull(row.GetCell(i));

            row.CreateCell(2);
            Assert.AreEqual(2, row.FirstCellNum);
            Assert.AreEqual(3, row.LastCellNum);

            row.CreateCell(1);
            Assert.AreEqual(1, row.FirstCellNum);
            Assert.AreEqual(3, row.LastCellNum);

            // check the exact case reported in 'bug' 43901 - notice that the cellNum is '0' based
            row.CreateCell(3);
            Assert.AreEqual(1, row.FirstCellNum);
            Assert.AreEqual(4, row.LastCellNum);
        }

        /**
         * Make sure that there is no cross-talk between rows especially with GetFirstCellNum and GetLastCellNum
         * This test was Added in response to bug report 44987.
         */
        [TestMethod]
        public void TestBoundsInMultipleRows()
        {
            Workbook workbook = _testDataProvider.CreateWorkbook();
            Sheet sheet = workbook.CreateSheet();
            Row rowA = sheet.CreateRow(0);

            rowA.CreateCell(10);
            rowA.CreateCell(5);
            Assert.AreEqual(5, rowA.FirstCellNum);
            Assert.AreEqual(11, rowA.LastCellNum);

            Row rowB = sheet.CreateRow(1);
            rowB.CreateCell(15);
            rowB.CreateCell(30);
            Assert.AreEqual(15, rowB.FirstCellNum);
            Assert.AreEqual(31, rowB.LastCellNum);

            Assert.AreEqual(5, rowA.FirstCellNum);
            Assert.AreEqual(11, rowA.LastCellNum);
            rowA.CreateCell(50);
            Assert.AreEqual(51, rowA.LastCellNum);

            Assert.AreEqual(31, rowB.LastCellNum);
        }
        [TestMethod]
        public void TestRemoveCell()
        {
            Workbook workbook = _testDataProvider.CreateWorkbook();
            Sheet sheet = workbook.CreateSheet();
            Row row = sheet.CreateRow(0);

            Assert.AreEqual(0, row.PhysicalNumberOfCells);
            Assert.AreEqual(-1, row.LastCellNum);
            Assert.AreEqual(-1, row.FirstCellNum);

            row.CreateCell(1);
            Assert.AreEqual(2, row.LastCellNum);
            Assert.AreEqual(1, row.FirstCellNum);
            Assert.AreEqual(1, row.PhysicalNumberOfCells);
            row.CreateCell(3);
            Assert.AreEqual(4, row.LastCellNum);
            Assert.AreEqual(1, row.FirstCellNum);
            Assert.AreEqual(2, row.PhysicalNumberOfCells);
            row.RemoveCell(row.GetCell(3));
            Assert.AreEqual(2, row.LastCellNum);
            Assert.AreEqual(1, row.FirstCellNum);
            Assert.AreEqual(1, row.PhysicalNumberOfCells);
            row.RemoveCell(row.GetCell(1));
            Assert.AreEqual(0, row.LastCellNum);
            Assert.AreEqual(-1, row.FirstCellNum);
            Assert.AreEqual(0, row.PhysicalNumberOfCells);

            workbook = _testDataProvider.WriteOutAndReadBack(workbook);
            sheet = workbook.GetSheetAt(0);
            row = sheet.GetRow(0);
            Assert.AreEqual(-1, row.LastCellNum);
            Assert.AreEqual(-1, row.FirstCellNum);
            Assert.AreEqual(0, row.PhysicalNumberOfCells);
        }

        public void BaseTestRowBounds(int maxRowNum)
        {
            Workbook workbook = _testDataProvider.CreateWorkbook();
            Sheet sheet = workbook.CreateSheet();
            //Test low row bound
            sheet.CreateRow(0);
            //Test low row bound exception
            try
            {
                sheet.CreateRow(-1);
                Assert.Fail("expected exception");
            }
            catch (ArgumentException e)
            {
                // expected during successful test
                Assert.IsTrue(e.Message.StartsWith("Invalid row number (-1)"));
            }

            //Test high row bound
            sheet.CreateRow(maxRowNum);
            //Test high row bound exception
            try
            {
                sheet.CreateRow(maxRowNum + 1);
                Assert.Fail("expected exception");
            }
            catch (ArgumentException e)
            {
                // expected during successful test
                Assert.AreEqual("Invalid row number (" + (maxRowNum + 1) + ") outside allowable range (0.." + maxRowNum + ")", e.Message);
            }
        }

        public void BaseTestCellBounds(int maxCellNum)
        {
            Workbook workbook = _testDataProvider.CreateWorkbook();
            Sheet sheet = workbook.CreateSheet();

            Row row = sheet.CreateRow(0);
            //Test low cell bound
            try
            {
                Cell cell = row.CreateCell(-1);
                Assert.Fail("expected exception");
            }
            catch (ArgumentException e)
            {
                // expected during successful test
                Assert.IsTrue(e.Message.StartsWith("Invalid column index (-1)"));
            }

            //Test high cell bound
            try
            {
                Cell cell = row.CreateCell(maxCellNum + 1);
                Assert.Fail("expected exception");
            }
            catch (ArgumentException e)
            {
                // expected during successful test
                Assert.IsTrue(e.Message.StartsWith("Invalid column index (" + (maxCellNum + 1) + ")"));
            }
            for (int i = 0; i < maxCellNum; i++)
            {
                Cell cell = row.CreateCell(i);
            }
            Assert.AreEqual(maxCellNum, row.PhysicalNumberOfCells);
            workbook = _testDataProvider.WriteOutAndReadBack(workbook);
            sheet = workbook.GetSheetAt(0);
            row = sheet.GetRow(0);
            Assert.AreEqual(maxCellNum, row.PhysicalNumberOfCells);
            for (int i = 0; i < maxCellNum; i++)
            {
                Cell cell = row.GetCell(i);
                Assert.AreEqual(i, cell.ColumnIndex);
            }

        }

        /**
         * Prior to patch 43901, POI was producing files with the wrong last-column
         * number on the row
         */
        [TestMethod]
        public void TestLastCellNumIsCorrectAfterAddCell_bug43901()
        {
            Workbook workbook = _testDataProvider.CreateWorkbook();
            Sheet sheet = workbook.CreateSheet("test");
            Row row = sheet.CreateRow(0);

            // New row has last col -1
            Assert.AreEqual(-1, row.LastCellNum);
            if (row.LastCellNum == 0)
            {
                Assert.Fail("Identified bug 43901");
            }

            // Create two cells, will return one higher
            //  than that for the last number
            row.CreateCell(0);
            Assert.AreEqual(1, row.LastCellNum);
            row.CreateCell(255);
            Assert.AreEqual(256, row.LastCellNum);
        }

        /**
         * Tests for the missing/blank cell policy stuff
         */
        [TestMethod]
        public void TestGetCellPolicy()
        {
            Workbook workbook = _testDataProvider.CreateWorkbook();
            Sheet sheet = workbook.CreateSheet("test");
            Row row = sheet.CreateRow(0);

            // 0 -> string
            // 1 -> num
            // 2 missing
            // 3 missing
            // 4 -> blank
            // 5 -> num
            row.CreateCell(0).SetCellValue("test");
            row.CreateCell(1).SetCellValue(3.2);
            row.CreateCell(4, CellType.BLANK);
            row.CreateCell(5).SetCellValue(4);

            // First up, no policy given, uses default
            Assert.AreEqual(CellType.STRING, row.GetCell(0).CellType);
            Assert.AreEqual(CellType.NUMERIC, row.GetCell(1).CellType);
            Assert.AreEqual(null, row.GetCell(2));
            Assert.AreEqual(null, row.GetCell(3));
            Assert.AreEqual(CellType.BLANK, row.GetCell(4).CellType);
            Assert.AreEqual(CellType.NUMERIC, row.GetCell(5).CellType);

            // RETURN_NULL_AND_BLANK - same as default
            Assert.AreEqual(CellType.STRING, row.GetCell(0, MissingCellPolicy.RETURN_NULL_AND_BLANK).CellType);
            Assert.AreEqual(CellType.NUMERIC, row.GetCell(1, MissingCellPolicy.RETURN_NULL_AND_BLANK).CellType);
            Assert.AreEqual(null, row.GetCell(2, MissingCellPolicy.RETURN_NULL_AND_BLANK));
            Assert.AreEqual(null, row.GetCell(3, MissingCellPolicy.RETURN_NULL_AND_BLANK));
            Assert.AreEqual(CellType.BLANK, row.GetCell(4, MissingCellPolicy.RETURN_NULL_AND_BLANK).CellType);
            Assert.AreEqual(CellType.NUMERIC, row.GetCell(5, MissingCellPolicy.RETURN_NULL_AND_BLANK).CellType);

            // RETURN_BLANK_AS_NULL - nearly the same
            Assert.AreEqual(CellType.STRING, row.GetCell(0, MissingCellPolicy.RETURN_BLANK_AS_NULL).CellType);
            Assert.AreEqual(CellType.NUMERIC, row.GetCell(1, MissingCellPolicy.RETURN_BLANK_AS_NULL).CellType);
            Assert.AreEqual(null, row.GetCell(2, MissingCellPolicy.RETURN_BLANK_AS_NULL));
            Assert.AreEqual(null, row.GetCell(3, MissingCellPolicy.RETURN_BLANK_AS_NULL));
            Assert.AreEqual(null, row.GetCell(4, MissingCellPolicy.RETURN_BLANK_AS_NULL));
            Assert.AreEqual(CellType.NUMERIC, row.GetCell(5, MissingCellPolicy.RETURN_BLANK_AS_NULL).CellType);

            // CREATE_NULL_AS_BLANK - Creates as needed
            Assert.AreEqual(CellType.STRING, row.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK).CellType);
            Assert.AreEqual(CellType.NUMERIC, row.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).CellType);
            Assert.AreEqual(CellType.BLANK, row.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).CellType);
            Assert.AreEqual(CellType.BLANK, row.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK).CellType);
            Assert.AreEqual(CellType.BLANK, row.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK).CellType);
            Assert.AreEqual(CellType.NUMERIC, row.GetCell(5, MissingCellPolicy.CREATE_NULL_AS_BLANK).CellType);

            // Check Created ones get the right column
            Assert.AreEqual(0, row.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK).ColumnIndex);
            Assert.AreEqual(1, row.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).ColumnIndex);
            Assert.AreEqual(2, row.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).ColumnIndex);
            Assert.AreEqual(3, row.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK).ColumnIndex);
            Assert.AreEqual(4, row.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK).ColumnIndex);
            Assert.AreEqual(5, row.GetCell(5, MissingCellPolicy.CREATE_NULL_AS_BLANK).ColumnIndex);


            // Now change the cell policy on the workbook, check
            //  that that is now used if no policy given
            workbook.MissingCellPolicy = (MissingCellPolicy.RETURN_BLANK_AS_NULL);

            Assert.AreEqual(CellType.STRING, row.GetCell(0).CellType);
            Assert.AreEqual(CellType.NUMERIC, row.GetCell(1).CellType);
            Assert.AreEqual(null, row.GetCell(2));
            Assert.AreEqual(null, row.GetCell(3));
            Assert.AreEqual(null, row.GetCell(4));
            Assert.AreEqual(CellType.NUMERIC, row.GetCell(5).CellType);
        }
        [TestMethod]
        public void TestRowHeight()
        {
            Workbook workbook = _testDataProvider.CreateWorkbook();
            Sheet sheet = workbook.CreateSheet();
            Row row1 = sheet.CreateRow(0);

            Assert.AreEqual(sheet.DefaultRowHeight, row1.Height);

            sheet.DefaultRowHeightInPoints = (20);
            row1.Height = ((short)-1); //reset the row height
            Assert.AreEqual(20.0f, row1.HeightInPoints, 0F);
            Assert.AreEqual(20 * 20, row1.Height);

            Row row2 = sheet.CreateRow(1);
            row2.Height = ((short)310);
            Assert.AreEqual(310, row2.Height);
            Assert.AreEqual(310F / 20, row2.HeightInPoints, 0F);

            Row row3 = sheet.CreateRow(2);
            row3.HeightInPoints = (25.5f);
            Assert.AreEqual((short)(25.5f * 20), row3.Height);
            Assert.AreEqual(25.5f, row3.HeightInPoints, 0F);

            Row row4 = sheet.CreateRow(3);
            Assert.IsFalse(row4.ZeroHeight);
            row4.ZeroHeight = (true);
            Assert.IsTrue(row4.ZeroHeight);

            workbook = _testDataProvider.WriteOutAndReadBack(workbook);
            sheet = workbook.GetSheetAt(0);

            row1 = sheet.GetRow(0);
            row2 = sheet.GetRow(1);
            row3 = sheet.GetRow(2);
            row4 = sheet.GetRow(3);
            Assert.AreEqual(20.0f, row1.HeightInPoints, 0F);
            Assert.AreEqual(20 * 20, row1.Height);

            Assert.AreEqual(310, row2.Height);
            Assert.AreEqual(310F / 20, row2.HeightInPoints, 0F);

            Assert.AreEqual((short)(25.5f * 20), row3.Height);
            Assert.AreEqual(25.5f, row3.HeightInPoints, 0F);

            Assert.IsFalse(row1.ZeroHeight);
            Assert.IsFalse(row2.ZeroHeight);
            Assert.IsFalse(row3.ZeroHeight);
            Assert.IsTrue(row4.ZeroHeight);
        }

        /**
         * Test Adding cells to a row in various places and see if we can find them again.
         */
        [TestMethod]
        public void TestGetCellEnumerator()
        {
            Workbook wb = _testDataProvider.CreateWorkbook();
            Sheet sheet = wb.CreateSheet();
            Row row = sheet.CreateRow(0);

            // One cell at the beginning
            Cell cell1 = row.CreateCell(1);
            IEnumerator it = row.GetCellEnumerator();
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell1 == it.Current);
            Assert.IsFalse(it.MoveNext());

            // Add another cell at the end
            Cell cell2 = row.CreateCell(99);
            it = row.GetCellEnumerator();
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell1 == it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell2 == it.Current);

            // Add another cell at the beginning
            Cell cell3 = row.CreateCell(0);
            it = row.GetCellEnumerator();
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell3 == it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell1 == it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell2 == it.Current);

            // Replace cell1
            Cell cell4 = row.CreateCell(1);
            it = row.GetCellEnumerator();
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell3 == it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell4 == it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell2 == it.Current);
            Assert.IsFalse(it.MoveNext());

            // Add another cell, specifying the cellType
            Cell cell5 = row.CreateCell(2, CellType.STRING);
            it = row.GetCellEnumerator();
            Assert.IsNotNull(cell5);
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell3 == it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell4 == it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell5 == it.Current);
            Assert.IsTrue(it.MoveNext());
            Assert.IsTrue(cell2 == it.Current);
            Assert.AreEqual(CellType.STRING, cell5.CellType);
        }
    }

}



