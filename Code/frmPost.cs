using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace Blogs
{
    public partial class frmPost : Form
    {
        private ConnectingToDatabase db = new ConnectingToDatabase();
        int userIDReceived;
        int keyOwnerReceived;
        int postIDReceived;
        public frmPost(int idUserLoggedIn, int keyOwner, int idPost)
        {
            InitializeComponent();
            userIDReceived = idUserLoggedIn;
            keyOwnerReceived = keyOwner;
            postIDReceived = idPost;
        }
        //Checking in load if user came here through Create post or Edit post and then disable/buttons suited 
        private void frmPost_Load(object sender, EventArgs e)                                                              
        {
            db.OpenConnection();
            if (keyOwnerReceived == 1) //User came through Create post which then enables Create button 
            {
                btnCreate.Enabled = true;
                btnEdit.Enabled = false;
            }
            else if (keyOwnerReceived == 2) //User came through Edit post which then enables Edit button and calls a method 
            {
                btnCreate.Enabled = false;
                btnEdit.Enabled = true;
                AcceptData(db.conn);
            }
        }
        //A method called when user pressed Edit post and fills in text boxes with data(title and content of post that user wants to edit)
        private void AcceptData(NpgsqlConnection conn) 
        {
            string query = "SELECT title, content FROM \"post\" WHERE id= @id";
            NpgsqlCommand command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("@id", postIDReceived);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                txtTitle.Text = reader[0].ToString();
                txtContent.Text = reader[1].ToString();
                reader.Close();
            }
        }
        //Create button event which calls a method for creating a new post
        private void btnCreate_Click(object sender, EventArgs e) 
        {
            CreatePost(db.conn);
        }
        //A method which sends a query to db and creates a new post with Today's timestamp
        private void CreatePost(NpgsqlConnection conn)
        {
            if (txtTitle.Text != "" && txtContent.Text != "")
            {
                DateTime dateTime = DateTime.UtcNow.Date;
                string query = "insert into post(title, content, date, app_user_id) " +
                    "VALUES('" + txtTitle.Text.ToString() + "','" + txtContent.Text.ToString() + "','" + dateTime.ToString("yyyy/MM/dd") + "','" + userIDReceived.ToString() + "')";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                MessageBox.Show("Post successfully created!", "Message!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Missing title and/or content!", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }
        //Edit button event which calls a method for editing a post
        private void btnEdit_Click(object sender, EventArgs e) 
        {
            ConfirmEdit(db.conn);
        }
        //A method which sends a query to db and changes info about a post with Today's timestamp
        private void ConfirmEdit(NpgsqlConnection conn) 
        {
            if (txtTitle.Text != "" && txtContent.Text != "")
            {
                string query = "UPDATE post SET title=@title, content=@content, date=@date, app_user_id=@app_user_id WHERE id= @id";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.Parameters.AddWithValue("@title", txtTitle.Text);
                command.Parameters.AddWithValue("@content", txtContent.Text);
                NpgsqlParameter npgsqlParameter = command.Parameters.AddWithValue("@date", DateTime.Now);
                command.Parameters.AddWithValue("@app_user_id", userIDReceived);
                command.Parameters.AddWithValue("@id", postIDReceived);
                command.ExecuteNonQuery();
                MessageBox.Show("Post successfully changed!", "Message!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Missing title and/or content!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        
    }
}
