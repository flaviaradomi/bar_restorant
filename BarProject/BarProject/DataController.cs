﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace BarProject
{
    class DataController
    {

        string ConnectionString;

        public DataController()
        {
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BarProjectDBConnectionString"].ConnectionString;
        }

        public bool konfirmoPerdorues(string Username, String Password)
        {
            string PasswordDatabase = string.Empty;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT Password FROM Users where Email = '" + Username + "';", connection);
                
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        PasswordDatabase = reader.GetString(0);
                    }
                }

                reader.Close();

            }

            if (Password.Equals(PasswordDatabase))
            {
                return true;
            }
            else return false;

        }


        public UserModel returnUserID(string email)
        {
            UserModel userModel = new UserModel();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID, Role, Name FROM Users where Email = '" + email + "';", connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                int UserID = 0;
                string Role = string.Empty;
                string Emer = string.Empty;

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        UserID = reader.GetInt32(0);
                        Role = reader.GetString(1);
                        Emer = reader.GetString(2);
                    }
                        
                }

                reader.Close();

                userModel.ID = UserID;
                userModel.Roli = Role;
                userModel.Emri = Emer;

            }

            return userModel;
        }


        public bool regjistroPerdorues (UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("Insert into Users (Name, Role, Email, Password) values ('" + user.Emri + "' , '" + user.Roli + "', '" + user.Email + "', '" + user.Password + "');", connection);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                    return true;
                else
                    return false;
            }
        }


        public bool shtoKategori(string emerKategori, string pershkrimKategori, PictureBox fotoKategori)
        {


            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                /*Start a local transaction*/
                SqlTransaction sqlTran = connection.BeginTransaction();

                /*Enlist a command in the current transaction*/
                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;

                /*initializing memory stream class for creating a stream of binary numbers*/
                MemoryStream ms = new MemoryStream();

                /*saving the image in raw format from picture box*/
                fotoKategori.Image.Save(ms, fotoKategori.Image.RawFormat);

                 /*Array of Binary numbers that have been converted*/
                byte[] foto = ms.GetBuffer();

                /*closing the memory stream*/
                ms.Close();


                try
                {
                    command.Parameters.AddWithValue("@emri", emerKategori);
                    command.Parameters.AddWithValue("@pershkrimi", pershkrimKategori);
                    command.Parameters.AddWithValue("@foto", foto);

                    command.CommandText = 
                        "Insert into Kategoria (EmriKategori, PershkrimKategori, FotoKategori) values (@emri, @pershkrimi, @foto)";

                    command.ExecuteNonQuery();

                    // Commit the transaction.
                    sqlTran.Commit();

                    connection.Close();

                    return true;

                }
                catch(Exception )
                {
                    connection.Close();
                    return false;
                }

            }
        }


        public ArrayList ngarkoKategorite()
        {
            ArrayList kategoriList = new ArrayList();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID , EmriKategori, FotoKategori from Kategoria", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DataModel model = new DataModel();
                        model.ID = reader.GetInt32(0);
                        model.Emri = reader.GetString(1);
                        model.Foto = (byte[])reader[2];

                        kategoriList.Add(model);
                    }
                }

                reader.Close();
            }

            return kategoriList;
        }

        public bool fshiKategori(int kategoriID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("Delete from Kategoria where ID = ' " + kategoriID + "'", connection);

                    connection.Open();

                    if (command.ExecuteNonQuery() > 0)
                    {
                        connection.Close();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch(Exception )
            {
                return false;
            }
        }


        public DataModel getKategori (int kategori)
        {
            DataModel model = new DataModel();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID , EmriKategori, PershkrimKategori, FotoKategori from Kategoria where ID ='" + kategori +"'", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        model.ID = reader.GetInt32(0);
                        model.Emri = reader.GetString(1);
                        model.Pershkrimi = reader.GetString(2);
                        model.Foto = (byte[])reader[3];
                    }
                }
                reader.Close();
            }
            return model;
        }

        public string getKategoriEmer(int kategori)
        {
            string emerKategori = string.Empty;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT EmriKategori from Kategoria where ID ='" + kategori + "'", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        emerKategori = reader.GetString(0);
                    }
                }
                reader.Close();
            }
            return emerKategori;
        }

        public int getKategori(string kategori)
        {
            DataModel model = new DataModel();

            int id = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID from Kategoria where EmriKategori ='" + kategori + "'", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                        
                    }
                }
                reader.Close();
            }
            return id;
        }


        public bool editKategori(DataModel kategoriModel)
        {
            using(SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                /*Start a local transaction*/
                SqlTransaction sqlTran = connection.BeginTransaction();

                /*Enlist a command in the current transaction*/ 
                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;

                try
                {
                    command.Parameters.AddWithValue("@KategoriID", kategoriModel.ID);
                    command.Parameters.AddWithValue("@Emri", kategoriModel.Emri);
                    command.Parameters.AddWithValue("@Pershkrimi", kategoriModel.Pershkrimi);
                    command.Parameters.AddWithValue("@Foto", kategoriModel.Foto);

                    command.CommandText =
                        "Update Kategoria set EmriKategori = @Emri, PershkrimKategori = @Pershkrimi, FotoKategori = @Foto where ID = @KategoriID";

                    command.ExecuteNonQuery();

                    sqlTran.Commit();

                    connection.Close();

                    return true;

                }
                catch(Exception)
                {
                    connection.Close();
                    return false;
                }

            }

        }


        public bool ShtoTavolina (string emerTavoline)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlTransaction sqlTran = connection.BeginTransaction();
                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;
                try
                {
                    command.Parameters.AddWithValue("@emri", emerTavoline);
                    command.CommandText= "Insert into Tavoline (EmerTavoline) values (@emri)";
                    command.ExecuteNonQuery();
                    sqlTran.Commit();
                    connection.Close();
                    return true;
                }
                catch (Exception e)
                {
                    connection.Close();
                    return false;

                }
 
            }
        }


        public bool ShtoFurnitor(string emerFurnitor)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlTransaction sqlTran = connection.BeginTransaction();
                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran ;
                try 
                {
                    command.Parameters.AddWithValue("@emer", emerFurnitor);
                    command.CommandText = "Insert into Furnitor (EmerFurnitor) values (@emer)";
                    command.ExecuteNonQuery();
                    sqlTran.Commit();
                    connection.Close();
                    return true;
                }
                catch(Exception e)
                {
                    connection.Close();
                    return false;
                }
            }

         }


        public ArrayList ngarkoTavolina()
        {
            ArrayList TavolinaList = new ArrayList();
            using (SqlConnection connection = new SqlConnection(ConnectionString)) 
            {
                SqlCommand command = new SqlCommand("SELECT ID, EmerTavoline FROM Tavoline" ,connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if(reader.HasRows)
                {
                    while (reader.Read()) 
                    {
                        DataModel model = new DataModel();
                        model.ID = reader.GetInt32(0);
                        model.Emri = reader.GetString(1);
                        TavolinaList.Add(model);
                    }
                }
                reader.Close();
            }
            return TavolinaList;           
        }


        public bool fshiTavolina(int idTavolina)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                   SqlCommand command = new SqlCommand("Delete from Tavoline where ID = ' " + idTavolina + "'", connection);
                   connection.Open();

                   if (command.ExecuteNonQuery() > 0)
                   {
                       connection.Close();
                       return true;
                   }
                   else
                       return false;
                }
            }

            catch (Exception ) 
            {
                return false;
            }
        }


        public ArrayList ngarkoTFurnitor()
        {
            ArrayList FurnitorList = new ArrayList();
            using(SqlConnection connection= new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID, EmerFurnitor from Furnitor",connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if(reader.HasRows)
               {

                while(reader.Read())
                {
                    DataModel model = new DataModel();
                    model.ID = reader.GetInt32(0);
                    model.Emri = reader.GetString(1);
                    FurnitorList.Add(model);
                }
                   
              }
                   reader.Close(); 
            }
            return FurnitorList;
        }


        public DataModel getTavoline(int idTavoline)
        {
            DataModel tavolineModel = new DataModel();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID , EmerTavoline from Tavoline where ID ='" + idTavoline + "'", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tavolineModel.ID = reader.GetInt32(0);
                        tavolineModel.Emri = reader.GetString(1);
                       
                    }
                }
                reader.Close();
            }
            return tavolineModel;
        }


        public DataModel getFurnitor(int idFurnitor)
        {
            DataModel furnitorModel = new DataModel();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID , EmerFurnitor from Furnitor where ID ='" + idFurnitor + "'", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        furnitorModel.ID = reader.GetInt32(0);
                        furnitorModel.Emri = reader.GetString(1);

                    }
                }
                reader.Close();
            }
            return furnitorModel;
        }

        public int getFurnitorID(string furnitorName)
        {
            int furnitorID = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID  from Furnitor where EmerFurnitor ='" + furnitorName + "'", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        furnitorID = reader.GetInt32(0);

                    }
                }
                reader.Close();
            }
            return furnitorID;
        }

        public bool editTavolina(DataModel tavolinaModel)
        {
           using(SqlConnection connection = new SqlConnection(ConnectionString))
           {
               connection.Open();
               SqlTransaction sqlTran = connection.BeginTransaction();
               SqlCommand command = connection.CreateCommand();
               command.Transaction = sqlTran;
               try
               {
                   command.Parameters.AddWithValue("@id", tavolinaModel.ID );
                   command.Parameters.AddWithValue("@emer", tavolinaModel.Emri);

                   command.CommandText = "Update Tavoline set EmerTavoline = @emer where ID = @id";
                   command.ExecuteNonQuery();
                   sqlTran.Commit();
                   connection.Close();

                   return true;
               }

               catch (Exception e)
               {
                   connection.Close();
                   return false;
               }
           }

        }

        public bool fshiFurnitor(int idFurnitor)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("Delete from Furnitor where ID = ' " + idFurnitor + "'", connection);
                    connection.Open();

                    if (command.ExecuteNonQuery() > 0)
                    {
                        connection.Close();
                        return true;
                    }
                    else
                        return false;
                }
            }

            catch (Exception)
            {
                return false;
            }
        }

        public bool editFurnitor(DataModel furnitorModel) 
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString)) 
            {
                connection.Open();
                SqlTransaction sqlTran = connection.BeginTransaction();
                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;
                try
                {
                    command.Parameters.AddWithValue("@id", furnitorModel.ID);
                    command.Parameters.AddWithValue("@emer",furnitorModel.Emri);
                    command.CommandText = "Update Furnitor set EmerFurnitor = @emer where ID = @id";
                    command.ExecuteNonQuery();
                    sqlTran.Commit();
                    connection.Close();

                    return true;
                }

                catch (Exception e) 
                {
                    connection.Close();
                    return false;
                }
            }
        
        }

        public ArrayList ngarkoTeGjitheProduktet()
        {
            ArrayList produktet = new ArrayList();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID, EmerProdukt, CmimProdukt, ProduktKategoriID, FotoProdukt FROM Produktet", connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        DataModel produkt = new DataModel();
                        produkt.ID = reader.GetInt32(0);
                        produkt.Emri = reader.GetString(1);
                        produkt.Cmimi = reader.GetDecimal(2);
                        int  idkategori = reader.GetInt32(3);
                        produkt.Kategoria = getKategoriEmer(idkategori);
                        if (!(reader[4] == null))
                        {
                            produkt.Foto = (byte[])reader[4];
                        }
                         
                        produktet.Add(produkt);

                    }
                }
            }

            return produktet;
        }

        public ArrayList ngarkoProduktetNgaKategoria(int kategoriaID)
        {
            ArrayList produktet = new ArrayList();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID, EmerProdukt, CmimProdukt, ProduktKategoriID, FotoProdukt FROM Produktet Where ProduktKategoriID = '" + kategoriaID + "'", connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DataModel produkt = new DataModel();
                        produkt.ID = reader.GetInt32(0);
                        produkt.Emri = reader.GetString(1);
                        produkt.Cmimi = reader.GetDecimal(2);
                        int idkategori = reader.GetInt32(3);
                        produkt.Kategoria = getKategoriEmer(idkategori);
                        if (!(reader[4] == null))
                        {
                            produkt.Foto = (byte[])reader[4];
                        }

                        produktet.Add(produkt);

                    }
                }
            }

            return produktet;
        }

        public bool ShtoProdukt(string emerProdukt, decimal cmimiProdukt, int kategoriaId, string pershkrimProdukt, PictureBox fotoProdukt) 
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlTransaction sqlTran = connection.BeginTransaction();
                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;

                MemoryStream ms = new MemoryStream();
                fotoProdukt.Image.Save(ms, fotoProdukt.Image.RawFormat);
                byte[] foto = ms.GetBuffer();
                ms.Close();

                try
                {
                    command.Parameters.AddWithValue("@emri", emerProdukt);
                    command.Parameters.AddWithValue("@cmimi", cmimiProdukt);
                    command.Parameters.AddWithValue("@pershkrimi", pershkrimProdukt);
                    command.Parameters.AddWithValue("@id", kategoriaId);
                    command.Parameters.AddWithValue("@foto", foto);

                    command.CommandText = "Insert into Produktet (EmerProdukt, CmimProdukt, ProduktKategoriID, PershkrimProdukt, FotoProdukt) values (@emri, @cmimi,  @id, @pershkrimi, @foto)";

                    command.ExecuteNonQuery();
                    sqlTran.Commit();
                    connection.Close();

                    return true;

                }
                catch (Exception ex)
                {
                    connection.Close();
                    return false;
                }

            }
        }

        public DataModel getProdukt(int produktID)
        {
            DataModel model = new DataModel();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID , EmerProdukt, CmimProdukt, PershkrimProdukt, FotoProdukt from Produktet where ID ='" + produktID + "'", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        model.ID = reader.GetInt32(0);
                        model.Emri = reader.GetString(1);
                        model.Cmimi = reader.GetDecimal(2);
                        model.Pershkrimi = reader.GetString(3);
                        model.Foto = (byte[])reader[4];
                    }
                }
                reader.Close();
            }
            return model;
        }

        public string getKategoriaNameFromProdukt(int produktID)
        {

            string kategoriName = string.Empty;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT EmriKategori  from Kategoria where ID = ( SELECT p.ProduktKategoriID FROM Produktet AS p WHERE p.ID = '" + produktID + "') ", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        kategoriName = reader.GetString(0);
                    }
                }
                reader.Close();
            }
            return kategoriName;
        }


        public bool fshiProdukt(int produktID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("Delete from Produktet where ID = ' " + produktID + "'", connection);

                    connection.Open();

                    if (command.ExecuteNonQuery() > 0)
                    {
                        connection.Close();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool editProdukt(int id ,string emri,decimal cmimi,int kategoria,string pershkrimi, byte[] foto)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                SqlTransaction sqlTran = connection.BeginTransaction();

                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;

                try
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.Parameters.AddWithValue("@EmerProdukt",emri);
                    command.Parameters.AddWithValue("@CmimProdukt", cmimi);
                    command.Parameters.AddWithValue("@ProduktKategoriID", kategoria);
                    command.Parameters.AddWithValue("@PershkrimProdukt", pershkrimi);
                    command.Parameters.AddWithValue("@FotoProdukt", foto);

                    command.CommandText =
                        "Update Produktet set EmerProdukt = @EmerProdukt, CmimProdukt = @CmimProdukt, ProduktKategoriID = @ProduktKategoriID, PershkrimProdukt = @PershkrimProdukt, FotoProdukt = @FotoProdukt where ID = @ID";

                    command.ExecuteNonQuery();

                    sqlTran.Commit();

                    connection.Close();

                    return true;

                }
                catch (Exception)
                {
                    connection.Close();
                    return false;
                }

            }

        }



        public bool shtoFurnizim (DataModel furnizimi, ArrayList produktet)
        {
            int furnizimID = returnFurnizimID();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();


                SqlTransaction sqlTran = connection.BeginTransaction();

                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;

                try
                {
                    command.Parameters.AddWithValue("@Fatura", furnizimi.fatura);
                    command.Parameters.AddWithValue("@DataFurnizim", furnizimi.dataFature);
                    command.Parameters.AddWithValue("@Totali", furnizimi.Total);
                    command.Parameters.AddWithValue("@FurnitorID", furnizimi.furnitorID);


                    command.CommandText =
                        "Insert into Furnizim (FurnitorID, NrFature, DataFurnizim, Total) values (@FurnitorID, @Fatura, @DataFurnizim, @Totali)";
                    command.ExecuteNonQuery();

                    foreach (DataModel model in produktet)
                    {
                        command.CommandText =
                            "Insert into ProduktFurnizim (EmerProdukt, SasiProdukt, CmimProdukt, Total, FurnizimID) values (' " + model.Emri + " ' , '" + model.Sasia + "', '" + model.Cmimi + "', '" + model.Total + "', '" + furnizimID + "' )";
                        command.ExecuteNonQuery();
                    }

                    sqlTran.Commit();

                    connection.Close();

                    return true;

                }
                catch(Exception e)
                {

                    MessageBox.Show(e.Message);
                    connection.Close();
                    return false;
                }


            }

        }



        public int returnFurnizimID()
        {
            int furnizimID = 1;
            try
            {

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("SELECT MAX(ID) FROM Furnizim;", connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            furnizimID = reader.GetInt32(0);
                        }
                    }
                    reader.Close();

                    furnizimID = furnizimID + 1;

                    return furnizimID;
                }
            }
            catch (Exception)
            {
                return furnizimID;
            }
        }

        public DataModel returnFurnizim(int furnizimID)
        {
            DataModel furnizim = new DataModel();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("SELECT ID, FurnitorID, NrFature, Total, DataFurnizim  FROM Furnizim WHERE ID = '" +furnizimID+"';", connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            furnizim.ID = reader.GetInt32(0);
                            furnizim.furnitorID = reader.GetInt32(1);
                            furnizim.fatura = reader.GetString(2);
                            furnizim.Total = reader.GetDecimal(3);
                            furnizim.dataFature = reader.GetDateTime(4);
                        }
                    }
                    reader.Close();


                    return furnizim;
                }
            }
            catch (Exception)
            {
                return furnizim;
            }
        }


        public ArrayList returnProdukteFurnizim(int furnizimID)
        {
            ArrayList produktFurnizim = new ArrayList();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("SELECT ID, EmerProdukt, SasiProdukt, CmimProdukt, Total FROM ProduktFurnizim WHERE FurnizimID = '" + furnizimID + "';", connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataModel model = new DataModel();
                            model.ID = reader.GetInt32(0);
                            model.Emri = reader.GetString(1);
                            model.Sasia = reader.GetInt32(2);
                            model.Cmimi = reader.GetDecimal(3);
                            model.Total = reader.GetDecimal(4);

                            produktFurnizim.Add(model);
                        }
                    }
                    reader.Close();


                    return produktFurnizim;
                }
            }
            catch (Exception)
            {
                return produktFurnizim;
            }
        }


        public string getFurnitorName(int idFurnitor)
        {
            string furnitorName = string.Empty;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT  EmerFurnitor from Furnitor where ID ='" + idFurnitor + "'", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        furnitorName = reader.GetString(0);

                    }
                }
                reader.Close();
            }
            return furnitorName;
        }


        public bool fshiProduktFurnizim(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand command = new SqlCommand("Delete from ProduktFurnizim where ID = ' " + id + "'", connection);

                    connection.Open();

                    if (command.ExecuteNonQuery() > 0)
                    {
                        connection.Close();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }




        public bool editFurnizim(DataModel furnizimi, ArrayList produktet)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();


                SqlTransaction sqlTran = connection.BeginTransaction();

                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;

                try
                {
                    command.Parameters.AddWithValue("@ID", furnizimi.ID);
                    command.Parameters.AddWithValue("@Fatura", furnizimi.fatura);
                    command.Parameters.AddWithValue("@Totali", furnizimi.Total);
                    command.Parameters.AddWithValue("@FurnitorID", furnizimi.furnitorID);


                    command.CommandText =
                        "UPDATE Furnizim SET  NrFature = @Fatura, Total =  @Totali, FurnitorID = @FurnitorID  WHERE ID = @ID";
                    command.ExecuteNonQuery();

                    foreach (DataModel model in produktet)
                    {
                        command.CommandText =
                            "UPDATE ProduktFurnizim SET EmerProdukt = '" + model.Emri + "', SasiProdukt = '" + model.Sasia + "', CmimProdukt = '" + model.Cmimi + "', Total = '" + model.Total + "' WHERE ID = '"+ model.ID +"'";
                        command.ExecuteNonQuery();
                    }

                    sqlTran.Commit();

                    connection.Close();

                    return true;

                }
                catch (Exception e)
                {

                    MessageBox.Show(e.Message);
                    connection.Close();
                    return false;
                }


            }

        }


        public bool fshiFurnizim(int furnizimID)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();


                SqlTransaction sqlTran = connection.BeginTransaction();

                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;

                try
                {
                    command.CommandText =
                        "DELETE FROM  ProduktFurnizim WHERE FurnizimID = '" + furnizimID + "'";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        "DELETE FROM  Furnizim  WHERE ID = '" + furnizimID + "'";
                    command.ExecuteNonQuery();


                    sqlTran.Commit();

                    connection.Close();

                    return true;

                }
                catch (Exception e)
                {

                    MessageBox.Show(e.Message);
                    connection.Close();
                    return false;
                }


            }

        }


        public ArrayList ngarkoTeGjitheFurnizimet()
        {
            ArrayList furnizimet = new ArrayList();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID, NrFature, DataFurnizim, Total, FurnitorID FROM Furnizim", connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DataModel furnizim = new DataModel();
                        furnizim.ID = reader.GetInt32(0);
                        furnizim.fatura = reader.GetString(1);
                        furnizim.dataFature = reader.GetDateTime(2);
                        furnizim.Total = reader.GetDecimal(3);
                        furnizim.furnitorID = reader.GetInt32(4);

                        furnizimet.Add(furnizim);

                    }
                }
            }

            return furnizimet;
        }


        public ArrayList ngarkoFurnizimetNgaFurnitor(int furnitorID)
        {
            ArrayList furnizimet = new ArrayList();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ID, NrFature, DataFurnizim, Total, FurnitorID FROM Furnizim Where FurnitorID = '" + furnitorID + "'", connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DataModel furnizim = new DataModel();
                        furnizim.ID = reader.GetInt32(0);
                        furnizim.fatura = reader.GetString(1);
                        furnizim.dataFature = reader.GetDateTime(2);
                        furnizim.Total = reader.GetDecimal(3);
                        furnizim.furnitorID = reader.GetInt32(4);

                        furnizimet.Add(furnizim);

                    }
                }
            }

            return furnizimet;
        }



















    }
}
