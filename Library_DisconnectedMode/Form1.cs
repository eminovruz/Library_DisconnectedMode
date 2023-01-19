using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Library_DisconnectedMode
{
    public partial class Form1 : Form
    {
        SqlConnection? connection = null;
        SqlDataReader? reader = null;
        DataTable? table = null;

        SqlDataAdapter? adapter = null;
        DataSet? dataSet = null;
        SqlCommandBuilder? commandBuilder = null;

        public Form1()
        {
            InitializeComponent();
            string? connectionString = "Data Source=DESKTOP-8V8B7U4\\MSSQLSERVER01;Initial Catalog=Library;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection = new(connectionString);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            BringAuthorsData();
            BringCategoryData();
            FillDataGridView();
        }
        private void FillDataGridView()
        {
            adapter = new SqlDataAdapter("SELECT * FROM BOOKS", connection);
            dataSet = new DataSet();

            adapter.Fill(dataSet, "Books");

            dataGridView1.DataSource = dataSet.Tables["Books"];
        }

        private void BringAuthorsData()
        {
            try
            {
                connection?.Open();

                string? commandText = @"SELECT * FROM AUTHORS";

                using var command = new SqlCommand(commandText, connection);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    authors_cmbx.Items.Add(reader[1]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { connection?.Close(); reader?.Close(); }
        }

        private void BringCategoryData()
        {
            try
            {
                connection?.Open();

                string? commandText = @"SELECT * FROM Categories";

                using var command = new SqlCommand(commandText, connection);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    category_cmbx.Items.Add(reader[1]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { connection?.Close(); reader?.Close(); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string? commandText = @"SELECT  * FROM Books
JOIN Authors ON Authors.Id = Books.Id_Author
JOIN Categories ON Categories.Id = Books.Id_Category
WHERE Authors.FirstName = @p1 AND Categories.[Name] = @p2";

                adapter = new SqlDataAdapter(commandText, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@p1", authors_cmbx.SelectedItem);
                adapter.SelectCommand.Parameters.AddWithValue("@p2", category_cmbx.SelectedItem);

                dataSet = new DataSet();

                adapter.Fill(dataSet, "ExecutedBooks");

                dataGridView1.DataSource = dataSet.Tables["ExecutedBooks"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection?.Close();
                reader?.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string? commandText = @"SELECT * FROM Books
                WHERE Books.[Name] = @p1";

                adapter = new SqlDataAdapter(commandText, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@p1", textBox1.Text);

                dataSet = new DataSet();

                adapter.Fill(dataSet, "SearchedBooks");
                dataGridView1.DataSource = dataSet.Tables["SearchedBooks"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FillDataGridView();
        }

        private void button4_Click(object sender, EventArgs e) // delete
        {
            adapter = new SqlDataAdapter("DELETE Books WHERE Books.[Name]='SQL'", connection);
            adapter.Update(dataSet.Tables["Books"]);
        }
    }
}