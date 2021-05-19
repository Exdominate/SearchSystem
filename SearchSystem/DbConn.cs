using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NodaTime;
using SearchSystem.Properties;

namespace SearchSystem
{    
    class DbConn    
    {
        private static DbConn db = null;

        // Obtain connection string information from the portal
        private static string Host = "127.0.0.1";
        private static string User = "postgres";
        private static string DBname = "SearchSystem";
        private static string Password = Resources.password;
        private static string Port = "5432";
        private NpgsqlConnection conn;

        public static DbConn getInstance()
        {
            if (db == null)
            {
                db = new DbConn();
            }
            return db;
        }

        private DbConn()
        {
            // Build connection string using parameters from portal
            string connString =
                String.Format(
                    "Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                    Host,
                    User,
                    DBname,
                    Port,
                    Password);


            this.conn = new NpgsqlConnection(connString);
            conn.Open();
            // Place this at the beginning of your program to use NodaTime everywhere (recommended)
            NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
        }

        public NpgsqlDataAdapter getNewDataAdapter(string cmd)
        {
            return new NpgsqlDataAdapter(cmd, this.conn);
        }

        public List<Document> GetAllFromDoc()
        {
            List<Document> result = new List<Document>();
            // Retrieve all rows
            using (var cmd = new NpgsqlCommand("SELECT * FROM ss.documents", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        /*Console.WriteLine(reader.GetInt32(reader.GetOrdinal("documentid")));
                        Console.WriteLine(reader.GetFieldValue<DateTime>(reader.GetOrdinal("datetime")));
                        Console.WriteLine(reader.GetString(reader.GetOrdinal("text")));
                        Console.WriteLine(reader.GetString(reader.GetOrdinal("title")));
                        Console.WriteLine(reader.GetString(reader.GetOrdinal("link")));*/
                        var id = reader.GetInt32(reader.GetOrdinal("documentid"));
                        var datetime = reader.GetFieldValue<DateTime>(reader.GetOrdinal("datetime")).ToString();
                        var text = reader.GetString(reader.GetOrdinal("text"));
                        var title = reader.GetString(reader.GetOrdinal("title"));
                        var link = reader.GetString(reader.GetOrdinal("link"));
                        result.Add(new Document(id, datetime, title, text, link));
                    }
                }
            }
            return result;
        }

        public List<Document> Get5FromDoc()
        {
            List<Document> result = new List<Document>();
            // Retrieve all rows
            using (var cmd = new NpgsqlCommand("SELECT * FROM ss.documents order by documentid limit 5", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(reader.GetOrdinal("documentid"));
                        var datetime = reader.GetFieldValue<DateTime>(reader.GetOrdinal("datetime")).ToString();
                        var text = reader.GetString(reader.GetOrdinal("text"));
                        var title = reader.GetString(reader.GetOrdinal("title"));
                        var link = reader.GetString(reader.GetOrdinal("link"));
                        result.Add(new Document(id, datetime, title, text, link));
                    }
                }
            }
            return result;
        }

        public Dictionary<List<int>, Word> getAllWords()
        {
            Dictionary<List<int>, Word> result = new Dictionary<List<int>, Word>();
            string cmdStr = "SELECT id, word, mentionings FROM ss.words";

            using (var cmd = new NpgsqlCommand(cmdStr, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var lemmId = reader.GetInt32(reader.GetOrdinal("id"));
                        var lemm = reader.GetString(reader.GetOrdinal("word"));
                        var mentngs = reader.GetInt32(reader.GetOrdinal("mentionings"));

                        result.Add(new List<int>(), new Word(lemmId, lemm, mentngs));
                    }
                }
            }

            foreach (var docsLemmPair in result)
            {
                using (var cmd = new NpgsqlCommand($"SELECT docid, times FROM ss.wordrefs where wordid={docsLemmPair.Value.id}", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int docId = reader.GetInt32(reader.GetOrdinal("docid"));
                            int times = reader.GetInt32(reader.GetOrdinal("times"));

                            docsLemmPair.Key.Add(docId);
                            docsLemmPair.Value.refs.Add(docId, times);
                        }
                    }
                }
            }

            return result;
        }

        public List<Word> getWordsForDoc(int documentId)
        {
            List<Word> result = new List<Word>();
            string cmdStr   =  "SELECT w.id, w.word, w.mentionings FROM ss.words w ";
            cmdStr          += "LEFT JOIN ss.wordrefs wr on wr.wordid = w.id ";
            cmdStr          += $"WHERE wr.docid = {documentId}";

            using (var cmd = new NpgsqlCommand(cmdStr, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        var lemmId = reader.GetInt32(reader.GetOrdinal("id"));
                        var lemm = reader.GetString(reader.GetOrdinal("word"));
                        var mentngs = reader.GetInt32(reader.GetOrdinal("mentionings"));
                        
                        result.Add(new Word(lemmId, lemm, mentngs));
                    }                    
                }
            }

            foreach(var lOb in result)
            {
                using (var cmd = new NpgsqlCommand($"SELECT docid, times FROM ss.wordrefs where wordid={lOb.id}", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int docId = reader.GetInt32(reader.GetOrdinal("docid"));
                            int times = reader.GetInt32(reader.GetOrdinal("times"));

                            lOb.refs.Add(docId, times);
                        }
                    }
                }
            }

            return result;
        }

        public List<Word> getWordObjectsForLemms(string[] lemms)
        {
            List<Word> result = new List<Word>();

            int i = 0;
            foreach(var lem in lemms)
            {
                using (var cmd = new NpgsqlCommand($"SELECT id, word, mentionings FROM ss.words WHERE word like '{lem}'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var lemmId = reader.GetInt32(reader.GetOrdinal("id"));
                            var lemm = reader.GetString(reader.GetOrdinal("word"));
                            var mentngs = reader.GetInt32(reader.GetOrdinal("mentionings"));

                            result.Add(new Word(lemmId, lemm, mentngs));
                        }
                        
                    }
                }

                if (i < result.Count)
                {
                    using (var cmd = new NpgsqlCommand($"SELECT docid, times FROM ss.wordrefs where wordid={result[i].id}", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int docId = reader.GetInt32(reader.GetOrdinal("docid"));
                                int times = reader.GetInt32(reader.GetOrdinal("times"));

                                result[i].refs.Add(docId, times);
                            }
                        }
                    }
                    i++;
                }
            }

            return result;
        }

        public int getAmountOfDocuments()
        {
            int result = -1;
            using (var cmd = new NpgsqlCommand("SELECT count(*) FROM ss.documents", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    result = reader.GetInt32(0);
                }
            }
            return result;
        }

        public int getAmountOfDocumentsByLemmId(int lemmId)
        {
            int result = -1;
            using (var cmd = new NpgsqlCommand($"SELECT mentionings FROM ss.words where id={lemmId}", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    result = reader.GetInt32(0);
                }
            }
            return result;
        }

        public int getAmountOfTimesLemmOccured(int lemmId, int docId)
        {
            int result = -1;
            using (var cmd = new NpgsqlCommand($"SELECT times FROM ss.words where wordid={lemmId} and docid={docId}", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    result = reader.GetInt32(0);
                }
            }
            return result;
        }

        public int insertIntoDb(Document doc)
        {
            using (var command = new NpgsqlCommand("insert into ss.documents (datetime, title, text, link) values (@datetime, @title, @text, @link)", conn))
            {                
                command.Parameters.Add(new NpgsqlParameter("datetime", Convert.ToDateTime(doc.date)));
                command.Parameters.AddWithValue("title", doc.title);
                command.Parameters.AddWithValue("text", doc.text);
                command.Parameters.AddWithValue("link", doc.link);

                return command.ExecuteNonQuery();
            }  
        }

        public int insertWord(Word wordObj)
        {
            using (var command = new NpgsqlCommand("insert into ss.words (word, mentionings) values (@word, @mentionings) RETURNING id", conn))
            {
                command.Parameters.AddWithValue("word", wordObj.lemm);
                command.Parameters.AddWithValue("mentionings", wordObj.mentionings);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    return reader.GetInt32(0);
                }             
            }            
        }

        public int insertWordRefs(Word wordObj)
        {
            string cmdStr = "insert into ss.wordrefs (wordid, docid, times) values ";
            if (wordObj.refs.Count == 0) return 0;
            for (int i = 0; i < wordObj.refs.Count; i++)
            {
                cmdStr += $"(@wordid{i}, @docid{i}, @times{i})";
                if (i != wordObj.refs.Count - 1) cmdStr += ", ";
            }

            using (var command = new NpgsqlCommand(cmdStr, conn))
            {
                int i = 0;
                foreach(var lref in wordObj.refs)
                {
                    command.Parameters.AddWithValue($"wordid{i}", wordObj.id);
                    command.Parameters.AddWithValue($"docid{i}", lref.Key);
                    command.Parameters.AddWithValue($"times{i}", lref.Value);
                    i++;
                }

                return command.ExecuteNonQuery();
            }
        }
    }
}