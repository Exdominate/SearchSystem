﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSystem
{
    public static class DataTableExtensions
    {
        public static DataView ApplySort(this DataTable table, Comparison<DataRow> comparison)
        {
            DataTable clone = table.Clone();
            List<DataRow> rows = new List<DataRow>();
            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            rows.Sort(comparison);

            foreach (DataRow row in rows)
            {
                clone.Rows.Add(row.ItemArray);
            }

            return clone.DefaultView;
        }
    }
}
