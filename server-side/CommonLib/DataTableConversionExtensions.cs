using System.Data;
using TestServiceGRPC;

namespace CommonLib;

public static class DataTableConversionExtensions
{
    // Converts a TableReply to a DataTable
    public static DataTable FromTableReply(this TableReply tableReply)
    {
        var dataTable = new DataTable();

        // Add columns (assuming all rows have the same columns)
        if (tableReply.Rows.Any())
            foreach (var column in tableReply.Rows.First().Columns)
                dataTable.Columns.Add(column.Column, typeof(string)); // Assuming all data is string

        // Add rows
        foreach (var row in tableReply.Rows)
        {
            var dataRow = dataTable.NewRow();

            foreach (var columnValue in row.Columns) 
                dataRow[columnValue.Column] = columnValue.Value;

            dataTable.Rows.Add(dataRow);
        }

        return dataTable;
    }

    public static DataRow AddTableRow(this DataTable dataTable, TableRow tableRow)
    {
        // Todo: Add support form other data types
        foreach (var columnValue in tableRow.Columns)
            if (!dataTable.Columns.Contains(columnValue.Column))
                dataTable.Columns.Add(columnValue.Column, typeof(string));

        var dataRow = dataTable.NewRow();

        foreach (var columnValue in tableRow.Columns)
            dataRow[columnValue.Column] = columnValue.Value;

        dataTable.Rows.Add(dataRow);

        return dataRow;
    }

    // Converts a DataTable to a TableReply
    public static TableReply ToTableReply(this DataTable dataTable)
    {
        var tableReply = new TableReply();

        foreach (DataRow dataRow in dataTable.Rows)
        {
            var tableRow = new TableRow();

            foreach (DataColumn column in dataTable.Columns)
            {
                var columnValue = new ColumnValue
                {
                    Column = column.ColumnName,
                    Value = dataRow[column].ToString()
                };

                tableRow.Columns.Add(columnValue);
            }

            tableReply.Rows.Add(tableRow);
        }

        return tableReply;
    }
}
