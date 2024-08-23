using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Statements
{
    /// <summary>
    /// Summary description for SqlGenerator.
    /// </summary>
    public sealed class SqlGenerator
    {
        /// <summary>
        /// Creates SQL command text for a specified statement
        /// </summary>
        /// <param name="statement">The statement to build the SQL command text.</param>
        /// <returns>The SQL command text for the statement.</returns>
        public static string BuildQuery(IStatement statement)
        {
            string sqlText = string.Empty;

            if (statement is Select)
            {
                sqlText = BuildSelectQuery(statement);
            }
            else if (statement is Insert)
            {
                sqlText = BuildInsertQuery(statement);
            }
            else if (statement is Update)
            {
                sqlText = BuildUpdateQuery(statement);
            }
            else if (statement is Delete)
            {
                sqlText = BuildDeleteQuery(statement);
            }

            return sqlText;
        }


        /// <summary>
        /// Creates an select SQL command text for a specified statement
        /// </summary>
        /// <param name="statement">The statement to build the SQL command text.</param>
        /// <returns>The SQL command text for the statement.</returns>
        private static string BuildSelectQuery(IStatement statement)
        {
            StringBuilder output = new StringBuilder();
            Select select = (Select)statement;
            int columnCount = statement.ParameterMap.PropertiesList.Count;

            output.Append("SELECT ");

            // Create the list of columns
            for (int i = 0; i < columnCount; i++)
            {
                ParameterProperty property = (ParameterProperty)statement.ParameterMap.PropertiesList[i];

                if (i < (columnCount - 1))
                {
                    output.Append("\t" + property.ColumnName + " as " + property.PropertyName + ",");
                }
                else
                {
                    output.Append("\t" + property.ColumnName + " as " + property.PropertyName);
                }
            }

            output.Append(" FROM ");
            output.Append("\t" + select.Generate.Table + "");

            // Create the where clause

            string[] compositeKeyList = select.Generate.By.Split(new Char[] { ',' });
            if (compositeKeyList.Length > 0 && select.Generate.By.Length > 0)
            {
                output.Append(" WHERE ");
                int length = compositeKeyList.Length;
                for (int i = 0; i < length; i++)
                {
                    string columnName = compositeKeyList[i];

                    if (i > 0)
                    {
                        output.Append("\tAND " + columnName + " = ?");
                    }
                    else
                    {
                        output.Append("\t" + columnName + " = ?");
                    }
                }
            }

            // 'Select All' case
            if (statement.ParameterClass == null)
            {
                // The ParameterMap is just used to build the query
                // to avoid problems later, we set it to null
                statement.ParameterMap = null;
            }

            return output.ToString();
        }


        /// <summary>
        /// Creates an insert SQL command text for a specified statement
        /// </summary>
        /// <param name="statement">The statement to build the SQL command text.</param>
        /// <returns>The SQL command text for the statement.</returns>
        private static string BuildInsertQuery(IStatement statement)
        {
            StringBuilder output = new StringBuilder();
            Insert insert = (Insert)statement;
            int columnCount = statement.ParameterMap.PropertiesList.Count;

            output.Append("INSERT INTO " + insert.Generate.Table + " (");

            // Create the parameter list
            for (int i = 0; i < columnCount; i++)
            {
                ParameterProperty property = (ParameterProperty)statement.ParameterMap.PropertiesList[i];

                // Append the column name as a parameter of the insert statement
                if (i < (columnCount - 1))
                {
                    output.Append("\t" + property.ColumnName + ",");
                }
                else
                {
                    output.Append("\t" + property.ColumnName + "");
                }
            }

            output.Append(") VALUES (");

            // Create the values list
            for (int i = 0; i < columnCount; i++)
            {
                ParameterProperty property = (ParameterProperty)statement.ParameterMap.PropertiesList[i];

                // Append the necessary line breaks and commas
                if (i < (columnCount - 1))
                {
                    output.Append("\t?,");
                }
                else
                {
                    output.Append("\t?");
                }
            }

            output.Append(")");

            return output.ToString();
        }


        /// <summary>
        /// Creates an update SQL command text for a specified statement
        /// </summary>
        /// <param name="statement">The statement to build the SQL command text.</param>
        /// <returns>The SQL command text for the statement.</returns>
        private static string BuildUpdateQuery(IStatement statement)
        {
            StringBuilder output = new StringBuilder();
            Update update = (Update)statement;
            int columnCount = statement.ParameterMap.PropertiesList.Count;
            string[] keysList = update.Generate.By.Split(',');

            output.Append("UPDATE ");
            output.Append("\t" + update.Generate.Table + " ");
            output.Append("SET ");

            // Create the set statement
            for (int i = 0; i < columnCount; i++)
            {
                ParameterProperty property = (ParameterProperty)statement.ParameterMap.PropertiesList[i];

                // Ignore key columns
                if (update.Generate.By.IndexOf(property.ColumnName) < 0)
                {
                    if (i < (columnCount - keysList.Length - 1))
                    {
                        output.Append("\t" + property.ColumnName + " = ?,");
                    }
                    else
                    {
                        output.Append("\t" + property.ColumnName + " = ? ");
                    }
                }
            }

            output.Append(" WHERE ");

            // Create the where clause
            int length = keysList.Length;
            for (int i = 0; i < length; i++)
            {
                string columnName = keysList[i];

                if (i > 0)
                {
                    output.Append("\tAND " + columnName + " = ?");
                }
                else
                {
                    output.Append("\t " + columnName + " = ?");
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Creates an delete SQL command text for a specified statement
        /// </summary>
        /// <param name="statement">The statement to build the SQL command text.</param>
        /// <returns>The SQL command text for the statement.</returns>
        private static string BuildDeleteQuery(IStatement statement)
        {
            StringBuilder output = new StringBuilder();
            Delete delete = (Delete)statement;
            string[] keysList = delete.Generate.By.Split(',');

            output.Append("DELETE FROM");
            output.Append("\t" + delete.Generate.Table + "");
            output.Append(" WHERE ");

            // Create the where clause
            int length = keysList.Length;
            for (int i = 0; i < keysList.Length; i++)
            {
                string columnName = keysList[i].Trim();

                if (i > 0)
                {
                    output.Append("\tAND " + columnName + " = ?");
                }
                else
                {
                    output.Append("\t " + columnName + " = ?");
                }
            }

            return output.ToString();
        }
    }
}
