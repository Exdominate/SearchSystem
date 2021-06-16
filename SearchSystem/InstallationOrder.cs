using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchSystem
{
    public partial class InstallationOrder : Form
    {
        public InstallationOrder()
        {
            InitializeComponent();
            label1.Text = "Порядок установки";
            label2.Text = @"
                1. Установите PostgreSQL;
                2. Создать БД с именем SearchSystem;
                3. Восстановить файл бэкапа для созданной БД;
                4 Только для работы онлайн:
                4.1 очистить таблицы documents, words, wordrefs схемы ss.
            ";
        }
    }
}
