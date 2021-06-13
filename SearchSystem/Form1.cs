using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Npgsql;

namespace SearchSystem
{
    

    public partial class Form1 : Form
    {
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }

        public void onFormLoad(object sender, EventArgs e)
        {
                
        }

        private void setOptionsDataGridView1()
        {
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns[0].HeaderText = "Название";
            dataGridView1.Columns[1].HeaderText = "Ссылка";
            dataGridView1.Columns[2].HeaderText = "Текст";
            dataGridView1.Columns[3].HeaderText = "Дата";
            dataGridView1.Columns[0].Width = 100;
            dataGridView1.Columns[3].Width = 80;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.RowHeadersVisible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            string cmd = ("SELECT title, link, text, datetime, documentid FROM ss.documents");
            NpgsqlDataAdapter da = DbConn.getInstance().getNewDataAdapter(cmd);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dt.ReduceRows();

            dataGridView1.DataSource = dt.DefaultView;
            setOptionsDataGridView1();
            HideIrrelevantRows(20);
            this.setStatus("Запущено", Color.ForestGreen);
        }

        private void HideIrrelevantRows(int count)
        {
            if (count == 0)
            {
                dataGridView1.DataSource = null;                
                return;
            }

            int i = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (i++ >= count) row.Visible = false;
            }
        }

        public void setStatus(string status, Color color)
        {            
            label1.Text = "Статус: " + status;
            label1.ForeColor = color;
            label1.Update();
            label1.Refresh();
        }        

        private void search_Click(object sender, EventArgs e)
        {            
            if (!Word.isCollectionUpToDate())
            {
                this.setStatus("загрузка", Color.Orange);                
                Word.setWordsCollection(DbConn.getInstance().getAllWords());                
            }

            this.setStatus("выполняется поиск", Color.Orange);
            int count = 0;
            Search searchQuery = new Search(textBox1.Text);
            List < Document > docs = DbConn.getInstance().GetAllFromDoc();
            Dictionary<int, double> relevation = new Dictionary<int, double>();

            double curRelevation;
            foreach(Document doc in docs)
            {
                curRelevation = searchQuery.scalarProduct(Document.getDocumentVector(doc.documentID));
                relevation.Add(doc.documentID, curRelevation);
                if (curRelevation != 0) count++;
            }
            
            dataGridView1.DataSource = dt.ApplySort((r1, r2) =>
            {
                var val1 = relevation[Int32.Parse(r1["documentid"].ToString())];
                var val2 = relevation[Int32.Parse(r2["documentid"].ToString())];                
                return val2.CompareTo(val1);
            });

            setOptionsDataGridView1();
            HideIrrelevantRows(count);            
            dataGridView1.Update();
            dataGridView1.Refresh();
            this.setStatus("поиск окончен", Color.ForestGreen);
        }

        private void loadCollection_Click(object sender, EventArgs e)
        {
            this.setStatus("загрузка", Color.Orange);
            Word.setWordsCollection(DbConn.getInstance().getAllWords());
            this.setStatus("загружено", Color.ForestGreen);
        }

        private void loadIndex_Click(object sender, EventArgs e)
        {
            this.setStatus("загрузка данных из интернета...", Color.Orange);
            List<Document> docs;
            docs = new Crawler().Index();
            docs.ForEach(doc =>
            {
                var count = DbConn.getInstance().insertIntoDb(doc);
                if (count < 1) Console.WriteLine(doc.title);
            });

            docs = DbConn.getInstance().GetAllFromDoc();
            HashSet<string> uniqueLemms = new HashSet<string>();
            Dictionary<string, Word> lemmsObjs = new Dictionary<string, Word>();

            docs.ForEach(doc =>
            {
                DeepMorphy.Model.MorphInfo[] docLemms = Parser.getAllLemms(doc.text);

                foreach (var lemInfo in docLemms)
                {
                    string lem = lemInfo.BestTag.Lemma != null ? lemInfo.BestTag.Lemma : lemInfo.Text;                    
                    if (uniqueLemms.Add(lem))
                    {
                        lemmsObjs.Add(lem, new Word(lem, doc.documentID));
                    }
                    else
                    {
                        int value;
                        if (lemmsObjs[lem].refs.TryGetValue(doc.documentID, out value)) // if word already mentioned in this doc
                        {
                            lemmsObjs[lem].refs[doc.documentID]++;
                        }
                        else
                        {
                            lemmsObjs[lem].mentionings++;
                            lemmsObjs[lem].refs.Add(doc.documentID, 1);
                        }
                    }
                }
            });

            foreach (var lOb in lemmsObjs)
            {
                lOb.Value.id = DbConn.getInstance().insertWord(lOb.Value);
                DbConn.getInstance().insertWordRefs(lOb.Value);
            }
            this.setStatus("загрузка завершена", Color.ForestGreen);
        }
    }
}
