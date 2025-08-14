using Shouldly;
using WackyRaces.Domain.Types;
using WackyRaces.Domain.Exceptions;

namespace WackyRaces.Domain.UnitTests.Types;

[TestClass]
public sealed class NextValueTests
{
    [TestClass]
    public sealed class RowIdNextValueTests
    {
        [TestMethod]
        public void ShouldReturn_NextRowValue()
        {
            // Arrange
            var row1 = new RowId(1);
            var row5 = new RowId(5);
            var row99 = new RowId(99);

            // Act
            var nextRow1 = row1.NextValue();
            var nextRow5 = row5.NextValue();
            var nextRow99 = row99.NextValue();

            // Assert
            nextRow1.Value.ShouldBe(2);
            nextRow5.Value.ShouldBe(6);
            nextRow99.Value.ShouldBe(100);
        }

        [TestMethod]
        public void ShouldReturnSequentialValues()
        {
            // Arrange
            var row = new RowId(1);

            // Act & Assert
            row.Value.ShouldBe(1);

            row = row.NextValue();
            row.Value.ShouldBe(2);

            row = row.NextValue();
            row.Value.ShouldBe(3);

            row = row.NextValue();
            row.Value.ShouldBe(4);

            row = row.NextValue();
            row.Value.ShouldBe(5);
        }

        [TestMethod]
        public void ShouldWork_WithLargeNumbers()
        {
            // Arrange
            var largeRow = new RowId(999999);

            // Act
            var nextRow = largeRow.NextValue();

            // Assert
            nextRow.Value.ShouldBe(1000000);
        }
    }

    [TestClass]
    public sealed class RowIdPreviousValueTests
    {
        [TestMethod]
        public void ShouldReturn_PreviousRowValue()
        {
            // Arrange
            var row2 = new RowId(2);
            var row5 = new RowId(5);
            var row100 = new RowId(100);

            // Act
            var prevRow2 = row2.PreviousValue();
            var prevRow5 = row5.PreviousValue();
            var prevRow100 = row100.PreviousValue();

            // Assert
            prevRow2.Value.ShouldBe(1);
            prevRow5.Value.ShouldBe(4);
            prevRow100.Value.ShouldBe(99);
        }

        [TestMethod]
        public void ShouldReturnSequentialValues_Backwards()
        {
            // Arrange
            var row = new RowId(5);

            // Act & Assert
            row.Value.ShouldBe(5);

            row = row.PreviousValue();
            row.Value.ShouldBe(4);

            row = row.PreviousValue();
            row.Value.ShouldBe(3);

            row = row.PreviousValue();
            row.Value.ShouldBe(2);

            row = row.PreviousValue();
            row.Value.ShouldBe(1);
        }

        [TestMethod]
        public void ShouldThrow_WhenTryingToGetPreviousOfOne()
        {
            // Arrange
            var row1 = new RowId(1);

            // Act & Assert
            Should.Throw<RowIdOutOfRangeException>(() => row1.PreviousValue());
        }

        [TestMethod]
        public void ShouldWork_WithLargeNumbers()
        {
            // Arrange
            var largeRow = new RowId(1000000);

            // Act
            var prevRow = largeRow.PreviousValue();

            // Assert
            prevRow.Value.ShouldBe(999999);
        }
    }

    [TestClass]
    public sealed class ColumnIdNextValueTests
    {
        [TestMethod]
        public void ShouldReturn_NextColumnValue()
        {
            // Arrange
            var colA = new ColumnId('A');
            var colB = new ColumnId('B');
            var colM = new ColumnId('M');
            var colY = new ColumnId('Y');

            // Act
            var nextColA = colA.NextValue();
            var nextColB = colB.NextValue();
            var nextColM = colM.NextValue();
            var nextColY = colY.NextValue();

            // Assert
            nextColA.Value.ShouldBe('B');
            nextColB.Value.ShouldBe('C');
            nextColM.Value.ShouldBe('N');
            nextColY.Value.ShouldBe('Z');
        }

        [TestMethod]
        public void ShouldReturnSequentialValues()
        {
            // Arrange
            var column = new ColumnId('A');

            // Act & Assert
            column.Value.ShouldBe('A');

            column = column.NextValue();
            column.Value.ShouldBe('B');

            column = column.NextValue();
            column.Value.ShouldBe('C');

            column = column.NextValue();
            column.Value.ShouldBe('D');

            column = column.NextValue();
            column.Value.ShouldBe('E');
        }

        [TestMethod]
        public void ShouldThrow_WhenTryingToGetNextAfterZ()
        {
            // Arrange
            var colZ = new ColumnId('Z');

            // Act & Assert
            Should.Throw<ColumnIdOutOfRangeException>(() => colZ.NextValue());
        }

        [TestMethod]
        public void ShouldWork_WithIntegerConstructor()
        {
            // Arrange
            var col1 = new ColumnId(1); // 'A'
            var col26 = new ColumnId(26); // 'Z'

            // Act
            var nextCol1 = col1.NextValue();

            // Assert
            col1.Value.ShouldBe('A');
            nextCol1.Value.ShouldBe('B');

            // col26 is 'Z', so NextValue should throw
            Should.Throw<ColumnIdOutOfRangeException>(() => col26.NextValue());
        }

        [TestMethod]
        public void ShouldWork_WithLowercaseInput()
        {
            // Arrange
            var cola = new ColumnId('a'); // Should be converted to 'A'
            var colz = new ColumnId('z'); // Should be converted to 'Z'

            // Act
            var nextCola = cola.NextValue();

            // Assert
            cola.Value.ShouldBe('A');
            nextCola.Value.ShouldBe('B');

            // colz is 'Z', so NextValue should throw
            Should.Throw<ColumnIdOutOfRangeException>(() => colz.NextValue());
        }
    }

    [TestClass]
    public sealed class ColumnIdPreviousValueTests
    {
        [TestMethod]
        public void ShouldReturn_PreviousColumnValue()
        {
            // Arrange
            var colB = new ColumnId('B');
            var colC = new ColumnId('C');
            var colN = new ColumnId('N');
            var colZ = new ColumnId('Z');

            // Act
            var prevColB = colB.PreviousValue();
            var prevColC = colC.PreviousValue();
            var prevColN = colN.PreviousValue();
            var prevColZ = colZ.PreviousValue();

            // Assert
            prevColB.Value.ShouldBe('A');
            prevColC.Value.ShouldBe('B');
            prevColN.Value.ShouldBe('M');
            prevColZ.Value.ShouldBe('Y');
        }

        [TestMethod]
        public void ShouldReturnSequentialValues_Backwards()
        {
            // Arrange
            var column = new ColumnId('E');

            // Act & Assert
            column.Value.ShouldBe('E');

            column = column.PreviousValue();
            column.Value.ShouldBe('D');

            column = column.PreviousValue();
            column.Value.ShouldBe('C');

            column = column.PreviousValue();
            column.Value.ShouldBe('B');

            column = column.PreviousValue();
            column.Value.ShouldBe('A');
        }

        [TestMethod]
        public void ShouldThrow_WhenTryingToGetPreviousOfA()
        {
            // Arrange
            var colA = new ColumnId('A');

            // Act & Assert
            Should.Throw<ColumnIdOutOfRangeException>(() => colA.PreviousValue());
        }

        [TestMethod]
        public void ShouldWork_WithIntegerConstructor()
        {
            // Arrange
            var col2 = new ColumnId(2); // 'B'
            var col1 = new ColumnId(1); // 'A'

            // Act
            var prevCol2 = col2.PreviousValue();

            // Assert
            col2.Value.ShouldBe('B');
            prevCol2.Value.ShouldBe('A');

            // col1 is 'A', so PreviousValue should throw
            Should.Throw<ColumnIdOutOfRangeException>(() => col1.PreviousValue());
        }

        [TestMethod]
        public void ShouldWork_WithLowercaseInput()
        {
            // Arrange
            var colb = new ColumnId('b'); // Should be converted to 'B'
            var cola = new ColumnId('a'); // Should be converted to 'A'

            // Act
            var prevColb = colb.PreviousValue();

            // Assert
            colb.Value.ShouldBe('B');
            prevColb.Value.ShouldBe('A');

            // cola is 'A', so PreviousValue should throw
            Should.Throw<ColumnIdOutOfRangeException>(() => cola.PreviousValue());
        }
    }
}
