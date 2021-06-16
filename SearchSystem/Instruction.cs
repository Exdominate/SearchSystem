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
    public partial class Instruction : Form
    {
        public Instruction()
        {
            InitializeComponent();
            label1.Text = "Инструкция по работе с приложением";
            label2.Text = @"
После загрузки приложения, если все требования по установке были выполнены:
1. При работе онлайн нажмите загрузить данные из интернета. Дождитесь окончания загрузки данных.
2. При работе оффлайн первый шаг делать не нужно. Напишите поисковый запрос и нажмите поиск.
            ";
        }
    }
}
